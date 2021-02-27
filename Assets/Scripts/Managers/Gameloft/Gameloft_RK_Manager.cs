using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
	public class Gameloft_RK_Manager : Gameloft_BaseManager {
		public override string[] ResourceFiles => new string[] {
			"0",
			"1",
			"2",
			"3",
			"4",
			"5",
			"6",
			"7",
			"8",
			"9",
			"10",
			"11",
			"12",
			"13",
			"14",
			"15",
			"16",
			"17",
			"18",
			"19",
			"20",
		};

		public virtual string GetLevelPath(GameSettings settings) => "20";
		public virtual string GetRoadTexturesPath(GameSettings settings) => "2";
		public virtual int GetLevelResourceIndex(GameSettings settings) => settings.Level;

		public override string[] SingleResourceFiles => new string[] {
			"s"
		};

		public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
		{
			new GameInfo_World(0, Enumerable.Range(0, 16).ToArray()),
		});

		public override async UniTask LoadFilesAsync(Context context) {
			await context.AddLinearSerializedFileAsync(GetLevelPath(context.Settings));
			await context.AddLinearSerializedFileAsync(GetRoadTexturesPath(context.Settings));
			foreach (var fileIndex in Enumerable.Range(0, PuppetCount).Select(i => GetPuppetFileIndex(i)).Distinct()) {
				await context.AddLinearSerializedFileAsync(fileIndex.ToString());
			}
		}

		public virtual int BasePuppetsResourceFile => 10;
		public virtual int PuppetsPerResourceFile => 15;

		public virtual int GetPuppetFileIndex(int puppetIndex) => BasePuppetsResourceFile + puppetIndex / PuppetsPerResourceFile;
		public virtual int GetPuppetResourceIndex(int puppetIndex) => puppetIndex % PuppetsPerResourceFile;
		public virtual PuppetReference[] PuppetReferences => Enumerable.Range(0,PuppetCount-2).Select(pi => new PuppetReference() {
			FileIndex = GetPuppetFileIndex(pi),
			ResourceIndex = GetPuppetResourceIndex(pi)
		}).Append(new PuppetReference() {
			FileIndex = GetPuppetFileIndex(PuppetCount-3),
			ResourceIndex = GetPuppetResourceIndex(PuppetCount-3) + 1
		}).Append(new PuppetReference() {
			FileIndex = GetPuppetFileIndex(PuppetCount - 3),
			ResourceIndex = GetPuppetResourceIndex(PuppetCount - 3) + 2
		}).ToArray();
		public virtual int PuppetCount => 62;

		public class PuppetReference {
			public int FileIndex { get; set; }
			public int ResourceIndex { get; set; }
		}

		public virtual Unity_ObjectManager_GameloftRK.PuppetData[] LoadPuppets(Context context) {

			var s = context.Deserializer;
			var refs = PuppetReferences;
			Gameloft_Puppet[] puppets = new Gameloft_Puppet[refs.Length];
			Unity_ObjectManager_GameloftRK.PuppetData[] models = new Unity_ObjectManager_GameloftRK.PuppetData[refs.Length];
			for (int i = 0; i < refs.Length; i++) {
				int fileIndex = refs[i].FileIndex;
				int resIndex = refs[i].ResourceIndex;
				var resf = FileFactory.Read<Gameloft_ResourceFile>(fileIndex.ToString(), context);
				puppets[i] = resf.SerializeResource<Gameloft_Puppet>(s, default, resIndex, name: $"Puppets[{i}]");
				models[i] = new Unity_ObjectManager_GameloftRK.PuppetData(i, fileIndex, resIndex, GetCommonDesign(puppets[i]));
			}
			return models;
		}

		public class MeshInProgress {
			public string name;
			public List<Vector3> vertices = new List<Vector3>();
			public List<Vector3> normals = new List<Vector3>();
			public List<Vector2> uvs = new List<Vector2>();
			public List<Color> colors = new List<Color>();
			public List<int> triangles = new List<int>();
			public Texture2D texture;
			public MeshInProgress(string name, Texture2D texture = null) {
				this.name = name;
				this.texture = texture;
			}
		}

		public void CreateTrackMesh(Gameloft_RK_Level level, Context context) {
			// Load road textures
			var resf = FileFactory.Read<Gameloft_ResourceFile>(GetRoadTexturesPath(context.Settings), context);
			var roads = new MeshInProgress[level.Types.Length][];
			Dictionary<int, Texture2D> textures = new Dictionary<int, Texture2D>();
			for (int i = 0; i < level.Types.Length; i++) {
				var roadTex0 = context.Settings.GameModeSelection != GameModeSelection.RaymanKartMobile_320x240 ? level.Types[i].RoadTexture0 : level.RoadTextureID_0;
				var roadTex1 = context.Settings.GameModeSelection != GameModeSelection.RaymanKartMobile_320x240 ? level.Types[i].RoadTexture1 : level.RoadTextureID_1;
				if (!textures.ContainsKey(roadTex0)) {
					var roadTex = resf.SerializeResource<Gameloft_Puppet>(context.Deserializer, default, roadTex0, name: $"RoadTexture0");
					textures[roadTex0] = GetPuppetImages(roadTex)?[0][0];
				}
				if (!textures.ContainsKey(roadTex1)) {
					var roadTex = resf.SerializeResource<Gameloft_Puppet>(context.Deserializer, default, roadTex1, name: $"RoadTexture1");
					textures[roadTex1] = GetPuppetImages(roadTex)?[0][0];
				}

				roads[i] = new MeshInProgress[2];
				roads[i][0] = new MeshInProgress($"Road Type {i} - 0", textures[roadTex0]);
				roads[i][1] = new MeshInProgress($"Road Type {i} - 1", textures[roadTex1]);
			}

			Vector3 curPos = Vector3.zero;
			float curAngle = 0f;
			float curHeight = 0f;
			float roadSizeFactor = 1f;
			float groundDisplacement = 0f;
			float abyssDisplacement = 0f;
			float abyssDepth = 0.5f;
			float abyssSize = 3f;
			var heightMultiplier = 0.025f;

			var t = level.TrackBlocks;

			// Road
			var bridge = new MeshInProgress("Bridge");

			// Ground
			var ground = new MeshInProgress("Ground");
			var abyss = new MeshInProgress("Abyss");

			for (int i = 0; i < t.Length; i++) {
				var curBlock = t[i];
				bool isBridge = BitHelpers.ExtractBits(level.Types[curBlock.Type].Flags, 1, 0) == 1;
				bool useRoadWidth = BitHelpers.ExtractBits(level.Types[curBlock.Type].Flags, 1, 1) == 1
					|| BitHelpers.ExtractBits(level.Types[curBlock.Type].Flags, 1, 2) == 1;
				bool drawAbyss = BitHelpers.ExtractBits(level.Types[curBlock.Type].Flags, 1, 3) == 1;

				bool previousUseAbyss = i > 0 && BitHelpers.ExtractBits(level.Types[t[i-1].Type].Flags, 1, 3) == 1;
				bool nextUseAbyss = i < t.Length-1 && BitHelpers.ExtractBits(level.Types[t[i + 1].Type].Flags, 1, 3) == 1;
				float roadWidth = (useRoadWidth ? level.Types[curBlock.Type].Width : 3000) / 1000f;
				float totalRoadWidth = roadWidth * roadSizeFactor;
				float roadSizeCur = Mathf.Min(1.1f,totalRoadWidth);
				float groundSizeCur = totalRoadWidth;
				var road = isBridge ? bridge : roads[curBlock.Type][i%2];
				int roadVertexCount = road.vertices.Count;
				int groundVertexCount = ground.vertices.Count;
				int abyssVertexCount = abyss.vertices.Count;
				Quaternion q = Quaternion.Euler(0, curAngle, 0);
				var curPosH = curPos + Vector3.up * curHeight;
				road.vertices.Add(curPosH + q * Vector3.left * roadSizeCur);
				road.vertices.Add(curPosH + q * Vector3.left * roadSizeCur);
				road.vertices.Add(curPosH + q * Vector3.right * roadSizeCur);
				road.vertices.Add(curPosH + q * Vector3.right * roadSizeCur);

				ground.vertices.Add(curPosH + q * Vector3.left * roadSizeCur);
				ground.vertices.Add(curPosH + q * Vector3.left * roadSizeCur);
				ground.vertices.Add(curPosH + q * Vector3.right * roadSizeCur);
				ground.vertices.Add(curPosH + q * Vector3.right * roadSizeCur);
				ground.vertices.Add(curPosH + q * Vector3.left * groundSizeCur);
				ground.vertices.Add(curPosH + q * Vector3.left * groundSizeCur);
				ground.vertices.Add(curPosH + q * Vector3.right * groundSizeCur);
				ground.vertices.Add(curPosH + q * Vector3.right * groundSizeCur);

				if (drawAbyss) {
					var curAbyssDepth = (previousUseAbyss ? Vector3.down * abyssDepth : Vector3.zero);
					abyss.vertices.Add(curPosH + q * Vector3.left * abyssSize + curAbyssDepth);
					abyss.vertices.Add(curPosH + q * Vector3.left * abyssSize + curAbyssDepth);
					abyss.vertices.Add(curPosH + q * Vector3.right * abyssSize + curAbyssDepth);
					abyss.vertices.Add(curPosH + q * Vector3.right * abyssSize + curAbyssDepth);
				}

				curPos += q * Vector3.forward;
				curHeight += curBlock.DeltaHeight * heightMultiplier;
				curAngle -= curBlock.DeltaRotation;

				q = Quaternion.Euler(0, curAngle, 0);
				curPosH = curPos + Vector3.up * curHeight;
				road.vertices.Add(curPosH + q * Vector3.left * roadSizeCur);
				road.vertices.Add(curPosH + q * Vector3.left * roadSizeCur);
				road.vertices.Add(curPosH + q * Vector3.right * roadSizeCur);
				road.vertices.Add(curPosH + q * Vector3.right * roadSizeCur);

				ground.vertices.Add(curPosH + q * Vector3.left * roadSizeCur);
				ground.vertices.Add(curPosH + q * Vector3.left * roadSizeCur);
				ground.vertices.Add(curPosH + q * Vector3.right * roadSizeCur);
				ground.vertices.Add(curPosH + q * Vector3.right * roadSizeCur);
				ground.vertices.Add(curPosH + q * Vector3.left * groundSizeCur);
				ground.vertices.Add(curPosH + q * Vector3.left * groundSizeCur);
				ground.vertices.Add(curPosH + q * Vector3.right * groundSizeCur);
				ground.vertices.Add(curPosH + q * Vector3.right * groundSizeCur);

				if (drawAbyss) {
					var curAbyssDepth = (nextUseAbyss ? Vector3.down * abyssDepth : Vector3.zero);
					abyss.vertices.Add(curPosH + q * Vector3.left * abyssSize + curAbyssDepth);
					abyss.vertices.Add(curPosH + q * Vector3.left * abyssSize + curAbyssDepth);
					abyss.vertices.Add(curPosH + q * Vector3.right * abyssSize + curAbyssDepth);
					abyss.vertices.Add(curPosH + q * Vector3.right * abyssSize + curAbyssDepth);
				}

				// Up
				road.triangles.Add(roadVertexCount + 0);
				road.triangles.Add(roadVertexCount + 4);
				road.triangles.Add(roadVertexCount + 2);
				road.triangles.Add(roadVertexCount + 2);
				road.triangles.Add(roadVertexCount + 4);
				road.triangles.Add(roadVertexCount + 6);
				// Down
				road.triangles.Add(roadVertexCount + 0 + 1);
				road.triangles.Add(roadVertexCount + 2 + 1);
				road.triangles.Add(roadVertexCount + 4 + 1);
				road.triangles.Add(roadVertexCount + 2 + 1);
				road.triangles.Add(roadVertexCount + 6 + 1);
				road.triangles.Add(roadVertexCount + 4 + 1);

				// Up
				ground.triangles.Add(groundVertexCount + 0);
				ground.triangles.Add(groundVertexCount + 4);
				ground.triangles.Add(groundVertexCount + 8);
				ground.triangles.Add(groundVertexCount + 4);
				ground.triangles.Add(groundVertexCount + 12);
				ground.triangles.Add(groundVertexCount + 8);
				ground.triangles.Add(groundVertexCount + 2 + 0);
				ground.triangles.Add(groundVertexCount + 2 + 8);
				ground.triangles.Add(groundVertexCount + 2 + 4);
				ground.triangles.Add(groundVertexCount + 2 + 4);
				ground.triangles.Add(groundVertexCount + 2 + 8);
				ground.triangles.Add(groundVertexCount + 2 + 12);
				// Down
				ground.triangles.Add(groundVertexCount + 1 + 0);
				ground.triangles.Add(groundVertexCount + 1 + 8);
				ground.triangles.Add(groundVertexCount + 1 + 4);
				ground.triangles.Add(groundVertexCount + 1 + 4);
				ground.triangles.Add(groundVertexCount + 1 + 8);
				ground.triangles.Add(groundVertexCount + 1 + 12);
				ground.triangles.Add(groundVertexCount + 1 + 2 + 0);
				ground.triangles.Add(groundVertexCount + 1 + 2 + 4);
				ground.triangles.Add(groundVertexCount + 1 + 2 + 8);
				ground.triangles.Add(groundVertexCount + 1 + 2 + 4);
				ground.triangles.Add(groundVertexCount + 1 + 2 + 12);
				ground.triangles.Add(groundVertexCount + 1 + 2 + 8);

				if (drawAbyss) {
					// Up
					abyss.triangles.Add(abyssVertexCount + 0);
					abyss.triangles.Add(abyssVertexCount + 4);
					abyss.triangles.Add(abyssVertexCount + 2);
					abyss.triangles.Add(abyssVertexCount + 2);
					abyss.triangles.Add(abyssVertexCount + 4);
					abyss.triangles.Add(abyssVertexCount + 6);
					// Down
					abyss.triangles.Add(abyssVertexCount + 0 + 1);
					abyss.triangles.Add(abyssVertexCount + 2 + 1);
					abyss.triangles.Add(abyssVertexCount + 4 + 1);
					abyss.triangles.Add(abyssVertexCount + 2 + 1);
					abyss.triangles.Add(abyssVertexCount + 6 + 1);
					abyss.triangles.Add(abyssVertexCount + 4 + 1);
				}

				// Test colors
				var roadCol = isBridge ? (level.Color_dk_BridgeLight ?? level.Types[curBlock.Type].ColorGround).GetColor() : Color.white;
				road.colors.Add(roadCol);
				road.colors.Add(Color.grey);
				road.colors.Add(roadCol);
				road.colors.Add(Color.grey);

				road.colors.Add(roadCol);
				road.colors.Add(Color.grey);
				road.colors.Add(roadCol);
				road.colors.Add(Color.grey);

				// UVs
				road.uvs.Add(new Vector2(0, 1));
				road.uvs.Add(new Vector2(0, 1));
				road.uvs.Add(new Vector2(2, 1));
				road.uvs.Add(new Vector2(2, 1));
				road.uvs.Add(new Vector2(0, 0));
				road.uvs.Add(new Vector2(0, 0));
				road.uvs.Add(new Vector2(2, 0));
				road.uvs.Add(new Vector2(2, 0));

				// Ground colors
				var groundCol = level.Types[curBlock.Type].ColorGround.GetColor();
				for(int j = 0; j < 16; j++) ground.colors.Add(groundCol);

				if (drawAbyss) {
					//Color colorFog = level.Color_bE_Road2.GetColor();
					Color colorAbyss = level.Types[curBlock.Type].ColorAbyss.GetColor();
					//var abyssCol = Color.Lerp(colorAbyss, colorFog, curHeight / 4f);
					//var abyssCol = level.Color_bF_Fog.GetColor();
					for (int j = 0; j < 8; j++) abyss.colors.Add(colorAbyss);
				}


				// Normals
				//normalsRoad[(i * 8) + 0] =
			}

			GameObject gaoParent = new GameObject();
			gaoParent.transform.position = Vector3.zero;

			// Road
			foreach (var road in roads.SelectMany(r => r).Append(bridge)) {
				Mesh roadMesh = new Mesh();
				roadMesh.SetVertices(road.vertices);
				roadMesh.SetTriangles(road.triangles, 0);
				roadMesh.SetColors(road.colors);
				roadMesh.SetUVs(0, road.uvs);
				roadMesh.RecalculateNormals();
				GameObject gao = new GameObject("Road mesh");
				MeshFilter mf = gao.AddComponent<MeshFilter>();
				MeshRenderer mr = gao.AddComponent<MeshRenderer>();
				gao.layer = LayerMask.NameToLayer("3D Collision");
				gao.transform.SetParent(gaoParent.transform);
				gao.transform.localScale = Vector3.one;
				gao.transform.localPosition = Vector3.zero;
				mf.mesh = roadMesh;
				mr.material = Controller.obj.levelController.controllerTilemap.unlitMaterial;
				if (road.texture != null) {
					road.texture.wrapModeU = TextureWrapMode.Mirror;
					mr.material.SetTexture("_MainTex", road.texture);
				}
			}

			// Ground
			{
				Mesh groundMesh = new Mesh();
				groundMesh.SetVertices(ground.vertices);
				groundMesh.SetTriangles(ground.triangles, 0);
				groundMesh.SetColors(ground.colors);
				groundMesh.RecalculateNormals();
				GameObject gao = new GameObject("Ground mesh");
				MeshFilter mf = gao.AddComponent<MeshFilter>();
				MeshRenderer mr = gao.AddComponent<MeshRenderer>();
				gao.layer = LayerMask.NameToLayer("3D Collision");
				gao.transform.SetParent(gaoParent.transform);
				gao.transform.localScale = Vector3.one;
				gao.transform.localPosition = Vector3.zero + Vector3.up * groundDisplacement;
				mf.mesh = groundMesh;
				mr.material = Controller.obj.levelController.controllerTilemap.unlitMaterial;
			}


			// Abyss
			{
				Mesh abyssMesh = new Mesh();
				abyssMesh.SetVertices(abyss.vertices);
				abyssMesh.SetTriangles(abyss.triangles, 0);
				abyssMesh.SetColors(abyss.colors);
				abyssMesh.RecalculateNormals();
				GameObject gao = new GameObject("Abyss mesh");
				MeshFilter mf = gao.AddComponent<MeshFilter>();
				MeshRenderer mr = gao.AddComponent<MeshRenderer>();
				gao.layer = LayerMask.NameToLayer("3D Collision");
				gao.transform.SetParent(gaoParent.transform);
				gao.transform.localScale = Vector3.one;
				gao.transform.localPosition = Vector3.zero + Vector3.up * abyssDisplacement;
				mf.mesh = abyssMesh;
				mr.material = Controller.obj.levelController.controllerTilemap.unlitMaterial;
			}

			gaoParent.transform.localScale = Vector3.one * 8;

		}

		public Mesh GetObject3DMesh(Gameloft_RK_Level.Object3D o, Gameloft_RK_Level.TrackBlock trkblk = null, bool flipX = false) {
			Mesh m = new Mesh();
			Color currentColor = Color.white;
			int curCount = 0;
			List<Vector3> vertices = new List<Vector3>();
			List<int> triangles = new List<int>();
			List<Color> colors = new List<Color>();
			Vector3 pt0, pt1, pt2;
			int curTri = 0;


			foreach (var c in o.Commands) {
				switch (c.Type) {
					case Gameloft_RK_Level.Object3D.Command.CommandType.Color:
						currentColor = c.Color.GetColor();
						break;
					case Gameloft_RK_Level.Object3D.Command.CommandType.DrawTriangle:
						pt0 = new Vector3(c.Positions[0].X, c.Positions[0].Y, curTri + c.Positions[0].Z * 10000);
						pt1 = new Vector3(c.Positions[1].X, c.Positions[1].Y, curTri + c.Positions[1].Z * 10000);
						pt2 = new Vector3(c.Positions[2].X, c.Positions[2].Y, curTri + c.Positions[2].Z * 10000);
						vertices.Add(pt0 / 1000f);
						vertices.Add(pt2 / 1000f);
						vertices.Add(pt1 / 1000f);
						//Controller.print(expectedNormal);
						colors.Add(currentColor);
						colors.Add(currentColor);
						colors.Add(currentColor);

						// Clockwise winding order
						Vector3 expectedNormal = Vector3.Cross(pt1 - pt0, pt2 - pt1);
						if (expectedNormal.z >= 0) {
							triangles.Add(curCount);
							triangles.Add(curCount + 1);
							triangles.Add(curCount + 2);
							// Back
							triangles.Add(curCount);
							triangles.Add(curCount + 2);
							triangles.Add(curCount + 1);
						} else {
							triangles.Add(curCount);
							triangles.Add(curCount + 2);
							triangles.Add(curCount + 1);
							// Back
							triangles.Add(curCount);
							triangles.Add(curCount + 1);
							triangles.Add(curCount + 2);
						}
						curCount += 3;
						curTri -= 1;
						break;
					case Gameloft_RK_Level.Object3D.Command.CommandType.DrawLine:
						pt0 = new Vector3(c.Positions[0].X, c.Positions[0].Y, curTri + c.Positions[0].Z * 10000);
						pt1 = new Vector3(c.Positions[1].X, c.Positions[1].Y, curTri + c.Positions[1].Z * 10000);
						var diff = pt1 - pt0;
						var lineThickness = (Quaternion.Euler(0, 0, 90) * diff).normalized * 5;
						if (lineThickness.x == 0 && lineThickness.y == 0) {
							lineThickness = new Vector3(1,1,0) * 5f;
						}
						vertices.Add((pt0 - lineThickness) / 1000f);
						vertices.Add((pt0 + lineThickness) / 1000f);
						vertices.Add((pt1 - lineThickness) / 1000f);
						vertices.Add((pt1 + lineThickness) / 1000f);
						colors.Add(currentColor);
						colors.Add(currentColor);
						colors.Add(currentColor);
						colors.Add(currentColor);
						triangles.Add(curCount);
						triangles.Add(curCount + 1);
						triangles.Add(curCount + 2);
						triangles.Add(curCount + 1);
						triangles.Add(curCount + 3);
						triangles.Add(curCount + 2);
						// Backfaces
						triangles.Add(curCount);
						triangles.Add(curCount + 2);
						triangles.Add(curCount + 1);
						triangles.Add(curCount + 1);
						triangles.Add(curCount + 2);
						triangles.Add(curCount + 3);
						curTri -= 1;
						curCount += 4;
						break;
					case Gameloft_RK_Level.Object3D.Command.CommandType.DrawRectangle:
						vertices.Add(new Vector3(c.Rectangle.X, c.Rectangle.Y, curTri) / 1000f);
						vertices.Add(new Vector3(c.Rectangle.X, c.Rectangle.Y + c.Rectangle.Height, curTri) / 1000f);
						vertices.Add(new Vector3(c.Rectangle.X + c.Rectangle.Width, c.Rectangle.Y, curTri) / 1000f);
						vertices.Add(new Vector3(c.Rectangle.X + c.Rectangle.Width, c.Rectangle.Y + c.Rectangle.Height, curTri) / 1000f);
						colors.Add(currentColor);
						colors.Add(currentColor);
						colors.Add(currentColor);
						colors.Add(currentColor);
						triangles.Add(curCount);
						triangles.Add(curCount + 1);
						triangles.Add(curCount + 2);
						triangles.Add(curCount + 1);
						triangles.Add(curCount + 3);
						triangles.Add(curCount + 2);
						// Backfaces
						triangles.Add(curCount);
						triangles.Add(curCount + 2);
						triangles.Add(curCount + 1);
						triangles.Add(curCount + 1);
						triangles.Add(curCount + 2);
						triangles.Add(curCount + 3);
						curCount += 4;
						curTri -= 1;
						break;
					case Gameloft_RK_Level.Object3D.Command.CommandType.FillArc:
						var arc = c.FillArc;
						int numSegments = 15;
						Vector3[] verts = new Vector3[numSegments +1];
						for (int i = 0; i < numSegments; i++) {
							verts[i] = Quaternion.Euler(0, 0, arc.StartAngle + (i/(float)numSegments)*arc.ArcAngle) * Vector3.right;
							triangles.Add(curCount);
							triangles.Add(curCount + i + 1);
							triangles.Add(curCount + i + 2);
							triangles.Add(curCount);
							triangles.Add(curCount + i + 2);
							triangles.Add(curCount + i + 1);
						}
						verts[numSegments] = Quaternion.Euler(0, 0, arc.StartAngle + arc.ArcAngle) * Vector3.right;
						var center = new Vector3(arc.XPosition + arc.Width / 2f, arc.YPosition + arc.Height / 2f, curTri) / 1000f;
						var factor = new Vector3(arc.Width / 2f, arc.Height / 2f, 0) / 1000f;
						vertices.Add(center);
						colors.Add(currentColor);
						foreach (var vert in verts) {
							vertices.Add(center + Vector3.Scale(factor, vert));
							colors.Add(currentColor);
						}
						curCount += verts.Length + 1;
						curTri -= 1;
						break;
				}
			}
			m.SetVertices(vertices);
			m.SetColors(colors);
			m.SetTriangles(triangles, 0);
			m.RecalculateNormals();
			return m;
		}



		public override async UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures) {
			await UniTask.CompletedTask;
			var resf = FileFactory.Read<Gameloft_ResourceFile>(GetLevelPath(context.Settings), context);
			var ind = GetLevelResourceIndex(context.Settings);
			var level = resf.SerializeResource<Gameloft_RK_Level>(context.Deserializer, default, ind, name: $"Level_{ind}");

			CreateTrackMesh(level, context);

			// Load objects
			Mesh[] meshes = level.Objects3D.Select(o => GetObject3DMesh(o)).ToArray();
			var objManager = new Unity_ObjectManager_GameloftRK(context, LoadPuppets(context));
			List<Unity_Object> objs = new List<Unity_Object>();

			UnityEngine.Debug.Log("Sum rotation: " + level.TrackBlocks.Sum(o => o.DeltaRotation));
			UnityEngine.Debug.Log("Sum height: " + level.TrackBlocks.Sum(o => o.DeltaHeight));
			Vector3 curPos = Vector3.zero;
			float curAngle = 0f;
			float curHeight = 0f;
			int curBlockIndex = 0;
			GameObject gaoParent = new GameObject();
			gaoParent.transform.position = Vector3.zero;
			gaoParent.transform.localRotation = Quaternion.identity;
			var heightMultiplier = 0.025f;
			foreach (var o in level.TrackBlocks) {
				var sphere = new GameObject();//GameObject.CreatePrimitive(PrimitiveType.Cube);
				sphere.transform.position = curPos + Vector3.up * curHeight;
				sphere.transform.rotation = Quaternion.Euler(0, curAngle, 0);

				var lumsForCurrentBlock = level.Lums?.Where(s12 => s12.TrackBlockIndex == curBlockIndex);
				if (lumsForCurrentBlock != null) {
					foreach (var lum in lumsForCurrentBlock) {
						var pos = sphere.transform.TransformPoint(new Vector3(lum.XPosition * 0.001f, 0.05f, 0));
						// TODO: Add Lum object here. As waypoint? As lum/coin (load puppet first)?
						// Maybe have a system for objects with custom puppets, so player puppets can be displayed? 
						/*objs.Add(new Unity_Object_GameloftRK(objManager, s8, level.ObjectTypes[s8.ObjectType]) {
							Position = new Vector3(pos.x, -pos.z, pos.y),
							Instance = blk
						});*/
					}
				}
				var cj = level.TrackObjectCollections[curBlockIndex];
				for (int i = 0; i < cj.Count; i++) {
					var ci_ind = cj.InstanceIndex + i;
					var blk = level.TrackObjectInstances[ci_ind];
					var toi = blk.TrackObjectIndex;
					var to = level.TrackObjects[toi];
					var type = level.ObjectTypes[to.ObjectType];
					var pos = sphere.transform.TransformPoint(new Vector3(to.XPosition * 0.001f, 0.05f + type.YPosition * 0.001f, 0));
					//if(blk.ObjType == 4) continue;
					// TODO: Create obj types 4. These are hardcoded it seems.
					// Usually they don't show up, but if Byte2 == 2, they show up as speed boosts
					if (blk.ObjType == 5) {
						GameObject gp = new GameObject($"TrackObjIndex: {blk.TrackObjectIndex}");
						gp.transform.position = Vector3.zero;
						//var m = meshes[blk.TrackObjectIndex];
						var m = GetObject3DMesh(level.Objects3D[blk.TrackObjectIndex], o, blk.FlipX);
						GameObject gao = new GameObject();
						MeshFilter mf = gao.AddComponent<MeshFilter>();
						MeshRenderer mr = gao.AddComponent<MeshRenderer>();
						gao.layer = LayerMask.NameToLayer("3D Collision");
						gao.transform.SetParent(gp.transform);
						gao.transform.localScale = new Vector3(blk.FlipX ? -1 : 1, 1, 0.1f);
						gao.transform.localRotation = Quaternion.Euler(0, curAngle, 0);
						gao.transform.localPosition = sphere.transform.position;
						mf.mesh = m;
						mr.material = Controller.obj.levelController.controllerTilemap.unlitMaterial;
						gp.transform.localScale = Vector3.one * 8;
						//gp.transform.name = s8.ObjectType.ToString();
					} else {
						objs.Add(new Unity_Object_GameloftRK(objManager, to, type) {
							Position = new Vector3(pos.x, -pos.z, pos.y),
							Instance = blk
						});
					}
				}

				curPos += Quaternion.Euler(0,curAngle,0) * Vector3.forward;
				curHeight += o.DeltaHeight * heightMultiplier;
				curAngle -= o.DeltaRotation;
				sphere.gameObject.name = $"{curBlockIndex}: {o.Type} | {o.Flags} | {o.Unknown} | {level.Types[o.Type].Flags}";
				sphere.gameObject.transform.SetParent(gaoParent.transform);
				curBlockIndex++;
			}
			gaoParent.transform.localScale = Vector3.one * 8;

			curPos = Vector3.zero + Vector3.up * 100;
			//curAngle = 0;
			for(int i = 0; i < level.MapSpriteMapping.Length; i++) {
				var b = level.MapSpriteMapping[i];
				var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				sphere.transform.position = curPos;
				var v = b.Vector2;

				curPos += new Vector3(v.x,0,-v.y) / 2f;

				/*UnityEngine.Random.InitState((int)o.Unknown);
				var color = UnityEngine.Random.ColorHSV(0, 1, 0.2f, 1f, 0.8f, 1.0f);*/
				curBlockIndex++;
			}

			// Load objects
			var unityObjs = objs;

			// Return level
			return new Unity_Level(
				maps: new Unity_Map[] {
					new Unity_Map() {
						TileSet = new Unity_TileSet[] {
							new Unity_TileSet(8)
						},
						
					}
				},
				objManager: objManager,
				isometricData: new Unity_IsometricData {
					CollisionWidth = 0,
					CollisionHeight = 0,
					TilesWidth = 38,
					TilesHeight = 24,
					Collision = null,
					Scale = Vector3.one * 8,
					ViewAngle = Quaternion.Euler(90,0,0),
					CalculateYDisplacement = () => 0,
					CalculateXDisplacement = () => 0,
					ObjectScale = Vector3.one,
				},
				eventData: unityObjs,
				//localization: LoadLocalization(context),
				defaultMap: 0,
				defaultCollisionMap: 0,
				cellSize: 8);

			///throw new NotImplementedException();
		}
	}
}