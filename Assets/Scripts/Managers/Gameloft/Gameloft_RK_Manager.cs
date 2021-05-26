using Cysharp.Threading.Tasks;

using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
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

		public virtual string GetLevelPath(int level) => "20";
		public string GetLevelPath(GameSettings settings) => GetLevelPath(settings.Level);
		public virtual string GetRoadTexturesPath(GameSettings settings) => "2";
		public virtual string GetBackgroundsPath(GameSettings settings) => "1";
		public virtual int GetLevelResourceIndex(int level) => level;
		public int GetLevelResourceIndex(GameSettings settings) => GetLevelResourceIndex(settings.Level);

		public override string[] SingleResourceFiles => new string[] {
			"s"
		};

		public override GameInfo_Volume[] GetLevels(GameSettings settings) {
			switch (settings.Game) {
				case Game.Gameloft_RK_HighResHalf:
					return GameInfo_Volume.SingleVolume(new GameInfo_World[] {
						new GameInfo_World(0, Enumerable.Range(0, 8).ToArray()),
					});
				default:
					return GameInfo_Volume.SingleVolume(new GameInfo_World[] {
						new GameInfo_World(0, Enumerable.Range(0, 16).ToArray()),
					});
			}
		}

		public override async UniTask LoadFilesAsync(Context context) {
			await context.AddLinearSerializedFileAsync(GetLevelPath(context.GetR1Settings()));
			await context.AddLinearSerializedFileAsync(GetBackgroundsPath(context.GetR1Settings()));
			await context.AddLinearSerializedFileAsync(GetRoadTexturesPath(context.GetR1Settings()));
			foreach (var fileIndex in Enumerable.Range(0, PuppetCount).Select(i => GetPuppetFileIndex(i)).Distinct()) {
				await context.AddLinearSerializedFileAsync(fileIndex.ToString());
			}
			await context.AddLinearSerializedFileAsync(LocalizationResourceFile.ToString());
		}



		public KeyValuePair<string, string[]>[] LoadLocalization(Context context) {
			var langages = new string[]
			{
				"English",
				"German",
				"Spanish",
				"French",
				"Italian",
				"Portuguese"
			};
			var s = context.Deserializer;
			var resf = FileFactory.Read<Gameloft_ResourceFile>(LocalizationResourceFile.ToString(), context);
			Gameloft_RK_LocalizationTable[] tables = new Gameloft_RK_LocalizationTable[resf.ResourcesCount];
			for (int i = 0; i < tables.Length; i++) {
				tables[i] = resf.SerializeResource<Gameloft_RK_LocalizationTable>(s, default, i, name: $"Localization[{i}]");
			}

			return tables.Select((x, i) => new KeyValuePair<string, string[]>(langages[i / 2] + " - " + ((i % 2 == 0) ? "Menu" : "Race"), x.Strings)).ToArray();
		}

		public virtual int BasePuppetsResourceFile => 10;
		public virtual int PuppetsPerResourceFile => 15;
		public virtual int PuppetCount => 62;
		public virtual int ExtraPuppetsInLastFile => 2;
		public virtual int LocalizationResourceFile => 16;

		public virtual int GetPuppetFileIndex(int puppetIndex) => BasePuppetsResourceFile + puppetIndex / PuppetsPerResourceFile;
		public virtual int GetPuppetResourceIndex(int puppetIndex) => puppetIndex % PuppetsPerResourceFile;
		public virtual PuppetReference[] PuppetReferences {
			get {
				var normalPuppets = Enumerable.Range(0, PuppetCount - ExtraPuppetsInLastFile).Select(pi => new PuppetReference() {
					FileIndex = GetPuppetFileIndex(pi),
					ResourceIndex = GetPuppetResourceIndex(pi)
				});
				for (int i = 0; i < ExtraPuppetsInLastFile; i++) {
					normalPuppets = normalPuppets.Append(new PuppetReference() {
						FileIndex = GetPuppetFileIndex(PuppetCount - ExtraPuppetsInLastFile - 1),
						ResourceIndex = GetPuppetResourceIndex(PuppetCount - ExtraPuppetsInLastFile - 1) + i + 1
					});
				}
				return normalPuppets.ToArray();
			}
		}

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
				models[i] = new Unity_ObjectManager_GameloftRK.PuppetData(i, fileIndex, resIndex, puppets[i], GetCommonDesign(puppets[i]));
			}
			return models;
		}

		public virtual Unity_Layer[] LoadBackgrounds(Context context, Gameloft_RK_Level level) {
			var s = context.Deserializer;
			Gameloft_ResourceFile resf = null;
			List<Unity_Layer> layers = new List<Unity_Layer>();
			for (int i = 0; i < level?.BackgroundLayers?.Length; i++) {
				var bgl = level.BackgroundLayers[i];
				if (resf == null) {
					resf = FileFactory.Read<Gameloft_ResourceFile>(GetBackgroundsPath(context.GetR1Settings()), context);
				}
				var rawData = resf.SerializeResource<Gameloft_DummyResource>(s,default,bgl.ImageResourceIndex, name: $"Backgrounds[{bgl.ImageResourceIndex}]");
				Texture2D tex = TextureHelpers.CreateTexture2D(1,1);
				tex.LoadImage(rawData.Data);
				// TODO: Offset background images correctly.
				// vertical position = bgl.Flags & 0x2 ? (backgroundBottom-tex.height-bgl.PixelsOffset) : bgl.PixelsOffset
				// backgroundBottom is hardcoded per version...
				/*
				 * 44 for 128x128
				 * 73 for 128x160
				 * 110 for 176x208
				 * 107 for 176x220
				 * 126 for 208x208
				 * 107 for 240x320 - s40v5
				 * 177 for 240x320 - W900i
				 * */
				layers.Add(new Unity_Layer_Texture {
					Name = $"Background Layer {i}",
					ShortName = $"BG{i}",
					Texture = tex,
				});
			}
			return layers.ToArray();
		}

		public virtual int Scale => 8;

		public static bool UseSingleRoadTexture(GameSettings s) => s.GameModeSelection == GameModeSelection.RaymanKartMobile_128x128
				|| s.GameModeSelection == GameModeSelection.RaymanKartMobile_128x128_s40v2
				|| s.GameModeSelection == GameModeSelection.RaymanKartMobile_320x240_Broken
				|| s.GameModeSelection == GameModeSelection.RaymanKartMobile_128x160_s40v2a_N6101;

		#region Geometry creation
		public GameObject CreateTrackMesh(Gameloft_RK_Level level, Context context, out Vector2 dimensions, out Vector2 center) {
			float minX = 0, minY = 0, maxX = 0, maxY = 0;
			void adjustDimensions(IEnumerable<Vector3> verts) {
				if(!verts.Any()) return;
				minX = Mathf.Min(minX, verts.Min(v => v.x));
				minY = Mathf.Min(minY, verts.Min(v => v.z));
				maxX = Mathf.Max(maxX, verts.Max(v => v.x));
				maxY = Mathf.Max(maxY, verts.Max(v => v.z));
			}
			// Load road textures
			var resf = FileFactory.Read<Gameloft_ResourceFile>(GetRoadTexturesPath(context.GetR1Settings()), context);
			var roads = new MeshInProgress[level.Types.Length][];
			Dictionary<int, Texture2D> textures = new Dictionary<int, Texture2D>();
			Dictionary<int, bool> textureIsTransparent = new Dictionary<int, bool>();
			bool useSingleRoadTexture = UseSingleRoadTexture(context.GetR1Settings());
			for (int i = 0; i < level.Types.Length; i++) {
				var roadTex0 = !useSingleRoadTexture ? level.Types[i].RoadTexture0 : level.RoadTextureID_0;
				var roadTex1 = !useSingleRoadTexture ? level.Types[i].RoadTexture1 : level.RoadTextureID_1;
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
				for(int j = 0; j < roads[i].Length; j++) {
					var ogTex = roads[i][j].texture;
					var roadTex = i == 0 ? roadTex0 : roadTex1;
					if(textureIsTransparent.ContainsKey(roadTex) && textureIsTransparent[roadTex] == false) continue;
					var pixels = ogTex.GetPixels();
					bool corrected = false;
					for (int p = 0; p < pixels.Length; p++) {
						if (pixels[p].a < 1f) {
							pixels[p] = level.Types[i].ColorGround.GetColor();
							corrected = true;
						}
					}
					if (corrected) {
						var tex = TextureHelpers.CreateTexture2D(ogTex.width, ogTex.height);
						tex.SetPixels(pixels);
						tex.Apply();
						roads[i][j].texture = tex;
					}
					textureIsTransparent[roadTex] = corrected;
				}
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
			float wallHeight = 2f;

			var t = level.TrackBlocks;

			// Road
			var bridge = new MeshInProgress("Bridge");

			// Ground
			var ground = new MeshInProgress("Ground");
			var abyss = new MeshInProgress("Abyss");
			var walls = new MeshInProgress("Walls");

			for (int i = 0; i < t.Length; i++) {
				var curBlock = t[i];
				bool isWallLeft = BitHelpers.ExtractBits(level.Types[curBlock.Type].Flags, 1, 5) == 1;
				bool isWallRight = BitHelpers.ExtractBits(level.Types[curBlock.Type].Flags, 1, 4) == 1;

				bool isBridge = BitHelpers.ExtractBits(level.Types[curBlock.Type].Flags, 1, 0) == 1;
				bool useRoadWidth = BitHelpers.ExtractBits(level.Types[curBlock.Type].Flags, 1, 1) == 1
					|| BitHelpers.ExtractBits(level.Types[curBlock.Type].Flags, 1, 2) == 1;
				bool drawAbyss = BitHelpers.ExtractBits(level.Types[curBlock.Type].Flags, 1, 3) == 1;

				bool previousUseAbyss = i > 0 && BitHelpers.ExtractBits(level.Types[t[i-1].Type].Flags, 1, 3) == 1;
				bool nextUseAbyss = i < t.Length-1 && BitHelpers.ExtractBits(level.Types[t[i + 1].Type].Flags, 1, 3) == 1;
				float roadWidth = (useRoadWidth ? level.Types[curBlock.Type].Width : 3000) / 1000f;
				float totalRoadWidth = roadWidth * roadSizeFactor;
				float roadSizeCur = Mathf.Min(level.DefaultRoadWidth / 1000f,totalRoadWidth);
				float groundSizeCur = totalRoadWidth;
				var road = isBridge ? bridge : roads[curBlock.Type][i%2];
				int roadVertexCount = road.vertices.Count;
				int groundVertexCount = ground.vertices.Count;
				int abyssVertexCount = abyss.vertices.Count;
				int wallsVertexCount = walls.vertices.Count;
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

				if (isWallLeft) {
					walls.vertices.Add(curPosH + q * Vector3.left * roadSizeCur);
					walls.vertices.Add(curPosH + q * Vector3.left * roadSizeCur);
					walls.vertices.Add(curPosH + q * Vector3.left * roadSizeCur + Vector3.up * wallHeight);
					walls.vertices.Add(curPosH + q * Vector3.left * roadSizeCur + Vector3.up * wallHeight);
				}
				if (isWallRight) {
					walls.vertices.Add(curPosH + q * Vector3.right * roadSizeCur);
					walls.vertices.Add(curPosH + q * Vector3.right * roadSizeCur);
					walls.vertices.Add(curPosH + q * Vector3.right * roadSizeCur + Vector3.up * wallHeight);
					walls.vertices.Add(curPosH + q * Vector3.right * roadSizeCur + Vector3.up * wallHeight);
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

				if (isWallLeft) {
					walls.vertices.Add(curPosH + q * Vector3.left * roadSizeCur);
					walls.vertices.Add(curPosH + q * Vector3.left * roadSizeCur);
					walls.vertices.Add(curPosH + q * Vector3.left * roadSizeCur + Vector3.up * wallHeight);
					walls.vertices.Add(curPosH + q * Vector3.left * roadSizeCur + Vector3.up * wallHeight);
				}
				if (isWallRight) {
					walls.vertices.Add(curPosH + q * Vector3.right * roadSizeCur);
					walls.vertices.Add(curPosH + q * Vector3.right * roadSizeCur);
					walls.vertices.Add(curPosH + q * Vector3.right * roadSizeCur + Vector3.up * wallHeight);
					walls.vertices.Add(curPosH + q * Vector3.right * roadSizeCur + Vector3.up * wallHeight);
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

				if (isWallLeft) {
					int spacing = isWallRight ? 8 : 4;
					// Inward
					walls.triangles.Add(wallsVertexCount + 0);
					walls.triangles.Add(wallsVertexCount + 2);
					walls.triangles.Add(wallsVertexCount + spacing + 2);
					walls.triangles.Add(wallsVertexCount + spacing + 2);
					walls.triangles.Add(wallsVertexCount + spacing + 0);
					walls.triangles.Add(wallsVertexCount + 0);
					// Outward
					walls.triangles.Add(wallsVertexCount + 1 + 0);
					walls.triangles.Add(wallsVertexCount + 1 + spacing + 2);
					walls.triangles.Add(wallsVertexCount + 1 + 2);
					walls.triangles.Add(wallsVertexCount + 1 + spacing + 0);
					walls.triangles.Add(wallsVertexCount + 1 + spacing + 2);
					walls.triangles.Add(wallsVertexCount + 1 + 0);
				}
				if (isWallRight) {
					int spacing = isWallLeft ? 8 : 4;
					int offset = isWallLeft ? 4 : 0;
					// Inward
					walls.triangles.Add(wallsVertexCount + offset + 0);
					walls.triangles.Add(wallsVertexCount + offset + spacing + 2);
					walls.triangles.Add(wallsVertexCount + offset + 2);
					walls.triangles.Add(wallsVertexCount + offset + spacing + 0);
					walls.triangles.Add(wallsVertexCount + offset + spacing + 2);
					walls.triangles.Add(wallsVertexCount + offset + 0);
					// Outward
					walls.triangles.Add(wallsVertexCount + offset + 1 + 0);
					walls.triangles.Add(wallsVertexCount + offset + 1 + 2);
					walls.triangles.Add(wallsVertexCount + offset + 1 + spacing + 2);
					walls.triangles.Add(wallsVertexCount + offset + 1 + spacing + 2);
					walls.triangles.Add(wallsVertexCount + offset + 1 + spacing + 0);
					walls.triangles.Add(wallsVertexCount + offset + 1 + 0);
				}

				// Test colors
				var bridgeCol = (i % 2 == 0) ? level.Color_dk_BridgeLight : level.Color_dj_BridgeDark;
				var roadCol = isBridge ? (bridgeCol ?? level.Types[curBlock.Type].ColorGround).GetColor() : Color.white;
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

				var wallColor = ((i % 2 == 0) ? level.Color_bH_Wall0 : level.Color_bI_Wall1).GetColor();
				if (isWallLeft) {
					walls.colors.Add(wallColor);
					walls.colors.Add(wallColor);
					walls.colors.Add(wallColor);
					walls.colors.Add(wallColor);
				}
				if (isWallRight) {
					walls.colors.Add(wallColor);
					walls.colors.Add(wallColor);
					walls.colors.Add(wallColor);
					walls.colors.Add(wallColor);
				}
				if (isWallLeft) {
					walls.colors.Add(wallColor);
					walls.colors.Add(wallColor);
					walls.colors.Add(wallColor);
					walls.colors.Add(wallColor);
				}
				if (isWallRight) {
					walls.colors.Add(wallColor);
					walls.colors.Add(wallColor);
					walls.colors.Add(wallColor);
					walls.colors.Add(wallColor);
				}


				// Normals
				//normalsRoad[(i * 8) + 0] =
			}

			GameObject gaoParent = new GameObject("Track");
			gaoParent.transform.position = Vector3.zero;

			// Road
			foreach (var road in roads.SelectMany(r => r).Append(bridge)) {
				adjustDimensions(road.vertices);
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
				adjustDimensions(ground.vertices);
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
				adjustDimensions(abyss.vertices);
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


			// Abyss
			if(walls.vertices.Count > 0) {
				Mesh wallsMesh = new Mesh();
				wallsMesh.SetVertices(walls.vertices);
				wallsMesh.SetTriangles(walls.triangles, 0);
				wallsMesh.SetColors(walls.colors);
				wallsMesh.RecalculateNormals();
				GameObject gao = new GameObject("Walls mesh");
				MeshFilter mf = gao.AddComponent<MeshFilter>();
				MeshRenderer mr = gao.AddComponent<MeshRenderer>();
				gao.layer = LayerMask.NameToLayer("3D Collision");
				gao.transform.SetParent(gaoParent.transform);
				gao.transform.localScale = Vector3.one;
				gao.transform.localPosition = Vector3.zero;
				mf.mesh = wallsMesh;
				mr.material = Controller.obj.levelController.controllerTilemap.unlitMaterial;
			}

			gaoParent.transform.localScale = Vector3.one * Scale;

			center = new Vector2(minX,maxY);
			dimensions = new Vector2(maxX - minX, maxY - minY);

			return gaoParent;

		}

		public Mesh GetObject3DMesh(Gameloft_RK_Level.Object3D o, Gameloft_RK_Level.TrackBlock trkblk = null, bool flipX = false) {
			Color currentColor = Color.white;
			int curCount = 0;
			MeshInProgress mesh = new MeshInProgress($"3D Object {o.Offset}");
			Vector3 pt0, pt1, pt2;
			Vector3 pt0n, pt1n, pt2n;
			int curShape = 0;
			Vector3 nextPos = Vector3.forward;
			Quaternion nextAngle = Quaternion.identity;
			var zMultiplier = 1000f;
			var curShapeMultiplier = 0.1f;
			if (trkblk != null) {
				var heightMultiplier = 0.025f;
				nextPos = new Vector3(0, trkblk.DeltaHeight * heightMultiplier, 1f);
				nextAngle = Quaternion.Euler(0, trkblk.DeltaRotation * (flipX ? 1 : -1), 0);
			}


			foreach (var c in o.Commands) {
				switch (c.Type) {
					case Gameloft_RK_Level.Object3D.Command.CommandType.Color:
						currentColor = c.Color.GetColor();
						break;
					case Gameloft_RK_Level.Object3D.Command.CommandType.DrawTriangle:
						pt0 = new Vector3(c.Positions[0].X, c.Positions[0].Y, curShape * curShapeMultiplier);
						pt1 = new Vector3(c.Positions[1].X, c.Positions[1].Y, curShape * curShapeMultiplier);
						pt2 = new Vector3(c.Positions[2].X, c.Positions[2].Y, curShape * curShapeMultiplier);
						pt0n = zMultiplier * nextPos + nextAngle * new Vector3(c.Positions[0].X, c.Positions[0].Y, curShape * curShapeMultiplier);
						pt1n = zMultiplier * nextPos + nextAngle * new Vector3(c.Positions[1].X, c.Positions[1].Y, curShape * curShapeMultiplier);
						pt2n = zMultiplier * nextPos + nextAngle * new Vector3(c.Positions[2].X, c.Positions[2].Y, curShape * curShapeMultiplier);
						pt0 = Vector3.LerpUnclamped(pt0, pt0n, c.Positions[0].Z);
						pt1 = Vector3.LerpUnclamped(pt1, pt1n, c.Positions[1].Z);
						pt2 = Vector3.LerpUnclamped(pt2, pt2n, c.Positions[2].Z);
						mesh.vertices.Add(pt0 / 1000f);
						mesh.vertices.Add(pt2 / 1000f);
						mesh.vertices.Add(pt1 / 1000f);
						//Controller.print(expectedNormal);
						mesh.colors.Add(currentColor);
						mesh.colors.Add(currentColor);
						mesh.colors.Add(currentColor);

						// Clockwise winding order
						Vector3 expectedNormal = Vector3.Cross(pt1 - pt0, pt2 - pt1);
						if (expectedNormal.z >= 0) {
							mesh.triangles.Add(curCount);
							mesh.triangles.Add(curCount + 1);
							mesh.triangles.Add(curCount + 2);
							// Back
							mesh.triangles.Add(curCount);
							mesh.triangles.Add(curCount + 2);
							mesh.triangles.Add(curCount + 1);
						} else {
							mesh.triangles.Add(curCount);
							mesh.triangles.Add(curCount + 2);
							mesh.triangles.Add(curCount + 1);
							// Back
							mesh.triangles.Add(curCount);
							mesh.triangles.Add(curCount + 1);
							mesh.triangles.Add(curCount + 2);
						}
						curCount += 3;
						curShape -= 1;
						break;
					case Gameloft_RK_Level.Object3D.Command.CommandType.DrawLine:
						pt0 = new Vector3(c.Positions[0].X, c.Positions[0].Y, curShape * curShapeMultiplier);
						pt1 = new Vector3(c.Positions[1].X, c.Positions[1].Y, curShape * curShapeMultiplier);
						pt0n = zMultiplier * nextPos + nextAngle * new Vector3(c.Positions[0].X, c.Positions[0].Y, curShape * curShapeMultiplier);
						pt1n = zMultiplier * nextPos + nextAngle * new Vector3(c.Positions[1].X, c.Positions[1].Y, curShape * curShapeMultiplier);
						pt0 = Vector3.LerpUnclamped(pt0, pt0n, c.Positions[0].Z);
						pt1 = Vector3.LerpUnclamped(pt1, pt1n, c.Positions[1].Z);
						var diff = pt1 - pt0;
						var lineThickness = (Quaternion.Euler(0, 0, 90) * diff).normalized * 5;
						if (lineThickness.x == 0 && lineThickness.y == 0) {
							lineThickness = new Vector3(1,1,0) * 5f;
						}
						mesh.vertices.Add((pt0 - lineThickness) / 1000f);
						mesh.vertices.Add((pt0 + lineThickness) / 1000f);
						mesh.vertices.Add((pt1 - lineThickness) / 1000f);
						mesh.vertices.Add((pt1 + lineThickness) / 1000f);
						mesh.colors.Add(currentColor);
						mesh.colors.Add(currentColor);
						mesh.colors.Add(currentColor);
						mesh.colors.Add(currentColor);
						mesh.triangles.Add(curCount);
						mesh.triangles.Add(curCount + 1);
						mesh.triangles.Add(curCount + 2);
						mesh.triangles.Add(curCount + 1);
						mesh.triangles.Add(curCount + 3);
						mesh.triangles.Add(curCount + 2);
						// Backfaces
						mesh.triangles.Add(curCount);
						mesh.triangles.Add(curCount + 2);
						mesh.triangles.Add(curCount + 1);
						mesh.triangles.Add(curCount + 1);
						mesh.triangles.Add(curCount + 2);
						mesh.triangles.Add(curCount + 3);
						curShape -= 1;
						curCount += 4;
						break;
					case Gameloft_RK_Level.Object3D.Command.CommandType.DrawRectangle:
						mesh.vertices.Add(new Vector3(c.Rectangle.X, c.Rectangle.Y, curShape * curShapeMultiplier) / 1000f);
						mesh.vertices.Add(new Vector3(c.Rectangle.X, c.Rectangle.Y + c.Rectangle.Height, curShape * curShapeMultiplier) / 1000f);
						mesh.vertices.Add(new Vector3(c.Rectangle.X + c.Rectangle.Width, c.Rectangle.Y, curShape * curShapeMultiplier) / 1000f);
						mesh.vertices.Add(new Vector3(c.Rectangle.X + c.Rectangle.Width, c.Rectangle.Y + c.Rectangle.Height, curShape * curShapeMultiplier) / 1000f);
						mesh.colors.Add(currentColor);
						mesh.colors.Add(currentColor);
						mesh.colors.Add(currentColor);
						mesh.colors.Add(currentColor);
						mesh.triangles.Add(curCount);
						mesh.triangles.Add(curCount + 1);
						mesh.triangles.Add(curCount + 2);
						mesh.triangles.Add(curCount + 1);
						mesh.triangles.Add(curCount + 3);
						mesh.triangles.Add(curCount + 2);
						// Backfaces
						mesh.triangles.Add(curCount);
						mesh.triangles.Add(curCount + 2);
						mesh.triangles.Add(curCount + 1);
						mesh.triangles.Add(curCount + 1);
						mesh.triangles.Add(curCount + 2);
						mesh.triangles.Add(curCount + 3);
						curCount += 4;
						curShape -= 1;
						break;
					case Gameloft_RK_Level.Object3D.Command.CommandType.FillArc:
						var arc = c.FillArc;
						int numSegments = 15;
						Vector3[] verts = new Vector3[numSegments +1];
						for (int i = 0; i < numSegments; i++) {
							verts[i] = Quaternion.Euler(0, 0, arc.StartAngle + (i/(float)numSegments)*arc.ArcAngle) * Vector3.right;
							mesh.triangles.Add(curCount);
							mesh.triangles.Add(curCount + i + 1);
							mesh.triangles.Add(curCount + i + 2);
							mesh.triangles.Add(curCount);
							mesh.triangles.Add(curCount + i + 2);
							mesh.triangles.Add(curCount + i + 1);
						}
						curShape -= verts.Length;
						verts[numSegments] = Quaternion.Euler(0, 0, arc.StartAngle + arc.ArcAngle) * Vector3.right;
						var center = new Vector3(arc.XPosition + arc.Width / 2f, arc.YPosition + arc.Height / 2f, curShape * curShapeMultiplier) / 1000f;
						var factor = new Vector3(arc.Width / 2f, arc.Height / 2f, 0) / 1000f;
						mesh.vertices.Add(center);
						mesh.colors.Add(currentColor);
						foreach (var vert in verts) {
							mesh.vertices.Add(center + Vector3.Scale(factor, vert));
							mesh.colors.Add(currentColor);
						}
						curCount += verts.Length + 1;
						curShape -= verts.Length;
						break;
				}
			}
			Mesh m = new Mesh();
			m.SetVertices(mesh.vertices);
			m.SetColors(mesh.colors);
			m.SetTriangles(mesh.triangles, 0);
			m.RecalculateNormals();
			return m;
		}

		public Mesh GetSpeedBoostMesh(int xCenter, Gameloft_RK_Level.TrackBlock trkblk = null, bool flipX = false, bool highRes = true) {
			Color currentColor = Color.white;
			MeshInProgress mesh = new MeshInProgress($"Speed Boost");
			Vector3 nextPos = Vector3.forward;
			Quaternion nextAngle = Quaternion.identity;
			var zMultiplier = 1000f;
			if (trkblk != null) {
				var heightMultiplier = 0.025f;
				nextPos = new Vector3(0, trkblk.DeltaHeight * heightMultiplier, 1f);
				nextAngle = Quaternion.Euler(0, trkblk.DeltaRotation * (flipX ? 1 : -1), 0);
			}
			currentColor = Color.white;
			Vector3[] pts = new Vector3[] {
				new Vector3(xCenter - 200, 50, 0),
				new Vector3(xCenter + 200, 50, 0),
				new Vector3(xCenter,50,0)
			};
			var pts_n = pts.Select(p => zMultiplier * nextPos + nextAngle * p).ToArray();
			float scaleLerpValue(float f) => 0.2f + (f * 0.6f);
			if (!highRes) {
				mesh.vertices.Add(Vector3.LerpUnclamped(pts[0], pts_n[0], scaleLerpValue(0f)) / 1000f);
				mesh.vertices.Add(Vector3.LerpUnclamped(pts[2], pts_n[2], scaleLerpValue(0.2f)) / 1000f);
				mesh.vertices.Add(Vector3.LerpUnclamped(pts[1], pts_n[1], scaleLerpValue(0f)) / 1000f);
				mesh.vertices.Add(Vector3.LerpUnclamped(pts[2], pts_n[2], scaleLerpValue(1f)) / 1000f);
				for (int i = 0; i < 4; i++) mesh.colors.Add(currentColor);
				mesh.uvs.Add(new Vector2(0, 0));
				mesh.uvs.Add(new Vector2(0.5f, 0.2f));
				mesh.uvs.Add(new Vector2(1, 0));
				mesh.uvs.Add(new Vector2(0.5f, 1f));


				// Left
				mesh.triangles.Add(0);
				mesh.triangles.Add(3);
				mesh.triangles.Add(1);

				// Right
				mesh.triangles.Add(1);
				mesh.triangles.Add(3);
				mesh.triangles.Add(2);
			} else {
				mesh.vertices.Add(Vector3.LerpUnclamped(pts[0], pts_n[0], scaleLerpValue(0f)) / 1000f);
				mesh.vertices.Add(Vector3.LerpUnclamped(pts[2], pts_n[2], scaleLerpValue(0.2f)) / 1000f);
				mesh.vertices.Add(Vector3.LerpUnclamped(pts[1], pts_n[1], scaleLerpValue(0f)) / 1000f);

				mesh.vertices.Add(Vector3.LerpUnclamped(pts[0], pts_n[0], scaleLerpValue(0.8f)) / 1000f);
				mesh.vertices.Add(Vector3.LerpUnclamped(pts[2], pts_n[2], scaleLerpValue(1f)) / 1000f);
				mesh.vertices.Add(Vector3.LerpUnclamped(pts[1], pts_n[1], scaleLerpValue(0.8f)) / 1000f);
				for (int i = 0; i < 6; i++) mesh.colors.Add(currentColor);
				mesh.uvs.Add(new Vector2(0, 0));
				mesh.uvs.Add(new Vector2(0.5f, 0.2f));
				mesh.uvs.Add(new Vector2(1, 0));
				mesh.uvs.Add(new Vector2(0, 0.8f));
				mesh.uvs.Add(new Vector2(0.5f, 1f));
				mesh.uvs.Add(new Vector2(1, 0.8f));
				// Left
				mesh.triangles.Add(0);
				mesh.triangles.Add(3);
				mesh.triangles.Add(4);

				mesh.triangles.Add(4);
				mesh.triangles.Add(1);
				mesh.triangles.Add(0);

				// Right
				mesh.triangles.Add(1);
				mesh.triangles.Add(4);
				mesh.triangles.Add(5);

				mesh.triangles.Add(5);
				mesh.triangles.Add(2);
				mesh.triangles.Add(1);
			}


			Mesh m = new Mesh();
			m.SetVertices(mesh.vertices);
			m.SetColors(mesh.colors);
			m.SetUVs(0, mesh.uvs);
			m.SetTriangles(mesh.triangles, 0);
			m.RecalculateNormals();
			return m;
		}
		public Texture2D GetSpeedBoostTexture() {
			Texture2D tex = TextureHelpers.CreateTexture2D(4,4);
			Color green = new Color(0 / 255f, 193 / 255f, 20 / 255f);
			Color white = new Color(148 / 255f, 238 / 255f, 118 / 255f);
			for (int y = 0; y < tex.height; y++) {
				for (int x = 0; x < tex.width; x++) {
					if (y % 2 == 0) {
						tex.SetPixel(x, y, green);
					} else {
						tex.SetPixel(x, y, white);
					}
				}
			}
			tex.wrapMode = TextureWrapMode.Repeat;
			tex.filterMode = FilterMode.Bilinear;
			tex.Apply();
			return tex;
		}


		public Mesh GetArchMesh(Gameloft_RK_Level level, int trackBlock) {
			Color currentColor = Color.white;
			Color frontColor = level.Color_Tunnel_Front.GetColor();
			MeshInProgress mesh = new MeshInProgress($"Arch {trackBlock}");
			Vector3 nextPos = Vector3.forward;
			Quaternion nextAngle = Quaternion.identity;
			var zMultiplier = 1000f;
			var curBlock = level.TrackBlocks[trackBlock];
			var heightMultiplier = 0.025f;
			nextPos = new Vector3(0, curBlock.DeltaHeight * heightMultiplier, 1f);
			nextAngle = Quaternion.Euler(0, -curBlock.DeltaRotation, 0);

			bool useRoadWidth = BitHelpers.ExtractBits(level.Types[curBlock.Type].Flags, 1, 1) == 1
				|| BitHelpers.ExtractBits(level.Types[curBlock.Type].Flags, 1, 2) == 1;
			var roadWidth = useRoadWidth ? level.DefaultRoadWidth/1.2f /*level.Types[curBlock.Type].Width*/ : level.DefaultRoadWidth;

			currentColor = (trackBlock % 2 == 0 ? level.Color_Tunnel_0 : level.Color_Tunnel_1).GetColor();
			var tw = roadWidth;
			var tt = level.Tunnel_WallThickness;
			var thMin = Mathf.Min(level.Tunnel_Height, level.Tunnel_Height2);
			var thMax = Mathf.Max(level.Tunnel_Height, level.Tunnel_Height2);
			var th1 = level.Tunnel_Height;
			var th2 = level.Tunnel_Height2;

			Vector3[] pts = new Vector3[] {
				new Vector3((tw + tt), 0, 0),
				new Vector3(tw, 0, 0),
				new Vector3((tw + tt), thMax, 0),
				new Vector3(tw, thMax, 0),

				new Vector3(-(tw + tt), 0, 0),
				new Vector3(-tw, 0, 0),
				new Vector3(-(tw + tt), thMax, 0),
				new Vector3(-tw, thMax, 0),

				// Ceiling front
				new Vector3(-tw, thMin, 0),
				new Vector3(-tw, thMax, 0),
				new Vector3(tw, thMin, 0),
				new Vector3(tw, thMax, 0),

				// Interior
				new Vector3(tw, 0, 0),
				new Vector3(tw, thMin, 0),
				new Vector3(-tw, 0, 0),
				new Vector3(-tw, thMin, 0),

				// Exterior
				new Vector3((tw + tt), 0, 0),
				new Vector3((tw + tt), thMax, 0),
				new Vector3(-(tw + tt), 0, 0),
				new Vector3(-(tw + tt), thMax, 0),

			};
			var pts_n = pts.Select(p => zMultiplier * nextPos + nextAngle * p).ToArray();
			int ptIndex, vertCount = 0;

			// 2 pillars, 1 ceiling
			for (int j = 0; j < 2; j++) {
				var curPts = j == 0 ? pts : pts_n;
				for (int i = 0; i < 3; i++) {
					ptIndex = 4 * i;
					vertCount = mesh.vertices.Count;

					mesh.vertices.Add(curPts[ptIndex + 0] / 1000f);
					mesh.vertices.Add(curPts[ptIndex + 1] / 1000f);
					mesh.vertices.Add(curPts[ptIndex + 2] / 1000f);
					mesh.vertices.Add(curPts[ptIndex + 3] / 1000f);

					mesh.colors.Add(frontColor);
					mesh.colors.Add(frontColor);
					mesh.colors.Add(frontColor);
					mesh.colors.Add(frontColor);

					// Front
					mesh.triangles.Add(vertCount + 0);
					mesh.triangles.Add(vertCount + 2);
					mesh.triangles.Add(vertCount + 1);
					mesh.triangles.Add(vertCount + 2);
					mesh.triangles.Add(vertCount + 3);
					mesh.triangles.Add(vertCount + 1);
					// Back
					mesh.triangles.Add(vertCount + 0);
					mesh.triangles.Add(vertCount + 1);
					mesh.triangles.Add(vertCount + 2);
					mesh.triangles.Add(vertCount + 3);
					mesh.triangles.Add(vertCount + 2);
					mesh.triangles.Add(vertCount + 1);
				}
			}

			for (int j = 0; j < 2; j++) {
				var col = j == 0 ? currentColor : frontColor;
				var startInd = 12 + j*4;
				// Interior / exterior
				vertCount = mesh.vertices.Count;
				mesh.vertices.Add(pts[startInd + 0] / 1000f); // Right
				mesh.vertices.Add(pts[startInd + 1] / 1000f);
				mesh.vertices.Add(pts_n[startInd + 0] / 1000f);
				mesh.vertices.Add(pts_n[startInd + 1] / 1000f);

				mesh.vertices.Add(pts[startInd + 2] / 1000f); // Left
				mesh.vertices.Add(pts[startInd + 3] / 1000f);
				mesh.vertices.Add(pts_n[startInd + 2] / 1000f);
				mesh.vertices.Add(pts_n[startInd + 3] / 1000f);

				mesh.vertices.Add(pts[startInd + 1] / 1000f); // Ceiling
				mesh.vertices.Add(pts[startInd + 3] / 1000f);
				mesh.vertices.Add(pts_n[startInd + 1] / 1000f);
				mesh.vertices.Add(pts_n[startInd + 3] / 1000f);
				for (int i = 0; i < 12; i++) mesh.colors.Add(col);
				for (int i = 0; i < 3; i++) {
					// Front
					if (i % 2 == j) {
						mesh.triangles.Add(vertCount + 4 * i + 0);
						mesh.triangles.Add(vertCount + 4 * i + 2);
						mesh.triangles.Add(vertCount + 4 * i + 1);
						mesh.triangles.Add(vertCount + 4 * i + 2);
						mesh.triangles.Add(vertCount + 4 * i + 3);
						mesh.triangles.Add(vertCount + 4 * i + 1);
					} else {
						mesh.triangles.Add(vertCount + 4 * i + 0);
						mesh.triangles.Add(vertCount + 4 * i + 1);
						mesh.triangles.Add(vertCount + 4 * i + 2);
						mesh.triangles.Add(vertCount + 4 * i + 3);
						mesh.triangles.Add(vertCount + 4 * i + 2);
						mesh.triangles.Add(vertCount + 4 * i + 1);
					}
				}
			}



			Mesh m = new Mesh();
			m.SetVertices(mesh.vertices);
			m.SetColors(mesh.colors);
			m.SetTriangles(mesh.triangles, 0);
			m.RecalculateNormals();
			return m;
		}
		#endregion

		public override async UniTask<Unity_Level> LoadAsync(Context context)
        {
			await UniTask.CompletedTask;
			var resf = FileFactory.Read<Gameloft_ResourceFile>(GetLevelPath(context.GetR1Settings()), context);
			var ind = GetLevelResourceIndex(context.GetR1Settings());
			var level = resf.SerializeResource<Gameloft_RK_Level>(context.Deserializer, default, ind, name: $"Level_{ind}");

			Vector2 dimensions, center;
			GameObject trackMesh = CreateTrackMesh(level, context, out dimensions, out center);
			Vector3 centerPos = new Vector3(-center.x, 0, -center.y) * Scale;
			var scaledCenterPos = new Vector3(centerPos.x, -centerPos.z, centerPos.y) / Scale;
			trackMesh.transform.localPosition = centerPos;

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
			GameObject gao_3dObjParent = null;
			GameObject gao_tunnelParent = null;
			GameObject gao_speedBoostParent = null;
			Texture2D speedboostTex = null;
			/*string[] objGroups = null;
			if (level.Lums != null && level.Lums.Length > 0) {
				objGroups = new string[] { "Normal", "Lums" };
			}*/
			{
				var pos = Vector3.zero + 0.05f * Vector3.up;
				objs.Add(new Unity_Object_GameloftRK(objManager, 0, 0, 0, "Player") {
					Position = scaledCenterPos + new Vector3(pos.x, -pos.z, pos.y)
				});
				objs.Add(new Unity_Object_GameloftRK(objManager, 1, 0, 0, "Player") {
					Position = scaledCenterPos + new Vector3(pos.x, -pos.z, pos.y)
				});
			}
			Vector3[] points = new Vector3[level.TrackBlocks.Length+1];
			foreach (var o in level.TrackBlocks) {
				var sphere = new GameObject();//GameObject.CreatePrimitive(PrimitiveType.Cube);
				sphere.transform.position = curPos + Vector3.up * curHeight;
				sphere.transform.rotation = Quaternion.Euler(0, curAngle, 0);
				points[curBlockIndex] = curPos + Vector3.up * curHeight;

				var lumsForCurrentBlock = level.Lums?.Where(s12 => s12.TrackBlockIndex == curBlockIndex);
				if (lumsForCurrentBlock != null) {
					foreach (var lum in lumsForCurrentBlock) {
						var pos = sphere.transform.TransformPoint(new Vector3(lum.XPosition * 0.001f, 0.05f, 0));
						objs.Add(new Unity_Object_GameloftRK(objManager, 26, 1, 0, "Lum") {
							Position = scaledCenterPos + new Vector3(pos.x, -pos.z, pos.y)
						});
					}
				}
				var cj = level.TrackObjectCollections[curBlockIndex];
				for (int i = 0; i < cj.Count; i++) {
					var ci_ind = cj.InstanceIndex + i;
					var blk = level.TrackObjectInstances[ci_ind];
					var toi = blk.TrackObjectIndex;
					var to = level.TrackObjects[toi];
					if (blk.ObjType == 4) {
						var triggerObject = level.TriggerObjects[to.ObjectType];
						// 0 = jump, 1 = speed boost, 2 = Water, 5 = also speedup apparently?
						// if flag 1 is set, and only flag 1 (so flags == 2): JUST speed boost
						if (triggerObject.Flags == 2 && context.GetR1Settings().GameModeSelection != GameModeSelection.RaymanKartMobile_320x240_Broken) {
							if (gao_speedBoostParent == null) {
								gao_speedBoostParent = new GameObject("Speed Boosts");
								gao_speedBoostParent.transform.localPosition = centerPos;
								gao_speedBoostParent.transform.localRotation = Quaternion.identity;
								gao_speedBoostParent.transform.localScale = Vector3.one;
								speedboostTex = GetSpeedBoostTexture();
							}
							GameObject gp = new GameObject($"Speed Boost");
							gp.transform.SetParent(gao_speedBoostParent.transform);
							gp.transform.localPosition = Vector3.zero;
							bool isHighRes = context.GetR1Settings().GameModeSelection != GameModeSelection.RaymanKartMobile_128x128;
							var m = GetSpeedBoostMesh(to.XPosition, o,highRes: isHighRes);
							GameObject gao = new GameObject();
							MeshFilter mf = gao.AddComponent<MeshFilter>();
							MeshRenderer mr = gao.AddComponent<MeshRenderer>();
							gao.layer = LayerMask.NameToLayer("3D Collision");
							gao.transform.SetParent(gp.transform);
							gao.transform.localScale = new Vector3(blk.FlipX ? -1 : 1, 1, 1f);
							gao.transform.localRotation = Quaternion.Euler(0, curAngle, 0);
							gao.transform.localPosition = sphere.transform.position;
							mf.mesh = m;
							mr.material = Controller.obj.levelController.controllerTilemap.unlitMaterial;
							mr.material.SetTexture("_MainTex", speedboostTex);
							var animTex = gao.AddComponent<AnimatedTextureComponent>();
							animTex.material = mr.material;
							animTex.scrollV = -1f;
							gp.transform.localScale = Vector3.one * Scale;
						}
						// Add regular trigger object
						var pos = sphere.transform.TransformPoint(new Vector3(to.XPosition * 0.001f, 0.05f, 0));
						objs.Add(new Unity_Object_GameloftRK(objManager, to, triggerObject) {
							Position = scaledCenterPos + new Vector3(pos.x, -pos.z, pos.y),
							Instance = blk
						});
					} else if (blk.ObjType == 1) {
						if (blk.TrackObjectIndex < 2) {
							if (gao_tunnelParent == null) {
								gao_tunnelParent = new GameObject("Tunnels");
								gao_tunnelParent.transform.localPosition = centerPos;
								gao_tunnelParent.transform.localRotation = Quaternion.identity;
								gao_tunnelParent.transform.localScale = Vector3.one;
							}
							GameObject gp = new GameObject($"TrackObjIndex: {blk.TrackObjectIndex}");
							gp.transform.SetParent(gao_tunnelParent.transform);
							gp.transform.localPosition = Vector3.zero;
							//var m = meshes[blk.TrackObjectIndex];
							var m = GetArchMesh(level, curBlockIndex);
							GameObject gao = new GameObject();
							MeshFilter mf = gao.AddComponent<MeshFilter>();
							MeshRenderer mr = gao.AddComponent<MeshRenderer>();
							gao.layer = LayerMask.NameToLayer("3D Collision");
							gao.transform.SetParent(gp.transform);
							gao.transform.localScale = new Vector3(blk.FlipX ? -1 : 1, 1, 1f);
							gao.transform.localRotation = Quaternion.Euler(0, curAngle, 0);
							gao.transform.localPosition = sphere.transform.position;
							mf.mesh = m;
							mr.material = Controller.obj.levelController.controllerTilemap.unlitMaterial;
							gp.transform.localScale = Vector3.one * Scale;
						}
					} else if (blk.ObjType == 5) {
						if (gao_3dObjParent == null) {
							gao_3dObjParent = new GameObject("3D Objects");
							gao_3dObjParent.transform.localPosition = centerPos;
							gao_3dObjParent.transform.localRotation = Quaternion.identity;
							gao_3dObjParent.transform.localScale = Vector3.one;
						}
						GameObject gp = new GameObject($"TrackObjIndex: {blk.TrackObjectIndex}");
						gp.transform.SetParent(gao_3dObjParent.transform);
						gp.transform.localPosition = Vector3.zero;
						//var m = meshes[blk.TrackObjectIndex];
						var m = GetObject3DMesh(level.Objects3D[blk.TrackObjectIndex], o, blk.FlipX);
						GameObject gao = new GameObject();
						MeshFilter mf = gao.AddComponent<MeshFilter>();
						MeshRenderer mr = gao.AddComponent<MeshRenderer>();
						gao.layer = LayerMask.NameToLayer("3D Collision");
						gao.transform.SetParent(gp.transform);
						gao.transform.localScale = new Vector3(blk.FlipX ? -1 : 1, 1, 1f);
						gao.transform.localRotation = Quaternion.Euler(0, curAngle, 0);
						gao.transform.localPosition = sphere.transform.position;
						mf.mesh = m;
						mr.material = Controller.obj.levelController.controllerTilemap.unlitMaterial;
						gp.transform.localScale = Vector3.one * Scale;
						//gp.transform.name = s8.ObjectType.ToString();
					} else {
						var type = level.ObjectTypes[to.ObjectType];
						var pos = sphere.transform.TransformPoint(new Vector3(to.XPosition * 0.001f, 0.05f + type.YPosition * 0.001f, 0));
						objs.Add(new Unity_Object_GameloftRK(objManager, to, type) {
							Position = scaledCenterPos + new Vector3(pos.x, -pos.z, pos.y),
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
			gaoParent.transform.localScale = Vector3.one * Scale;
			gaoParent.transform.localPosition = centerPos;
			points[curBlockIndex] = curPos + Vector3.up * curHeight;

			// Create minimap objects
			/*curPos = Vector3.zero + Vector3.up * 100;
			//curAngle = 0;
			for(int i = 0; i < level.MapSpriteMapping?.Length; i++) {
				var b = level.MapSpriteMapping[i];
				var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				sphere.transform.position = curPos;
				var v = b.Vector2;

				curPos += new Vector3(v.x,0,-v.y) / 2f;

				curBlockIndex++;
			}*/

			// Load objects
			var unityObjs = objs;

			// Initialize layers
			var parent3d = Controller.obj.levelController.editor.layerTiles.transform;
			var layers = new List<Unity_Layer>();
			layers.AddRange(LoadBackgrounds(context, level));
			layers.Add(new Unity_Layer_GameObject(true, isAnimated: false) {
				Name = "Track",
				ShortName = $"TRK",
				Graphics = trackMesh,
				Dimensions = dimensions * Scale * 8 * 2
			});
			trackMesh.transform.SetParent(parent3d);
			if (gao_tunnelParent != null) {
				layers.Add(new Unity_Layer_GameObject(true, isAnimated: false) {
					Name = "Tunnels",
					ShortName = $"TUN",
					Graphics = gao_tunnelParent
				});
				gao_tunnelParent.transform.SetParent(parent3d);
			}
			if (gao_3dObjParent != null) {
				layers.Add(new Unity_Layer_GameObject(true, isAnimated: false) {
					Name = "3D Objects",
					ShortName = $"3DO",
					Graphics = gao_3dObjParent
				});
				gao_3dObjParent.transform.SetParent(parent3d);
			}
			if (gao_speedBoostParent != null) {
				layers.Add(new Unity_Layer_GameObject(true, isAnimated: true) {
					Name = "Speed Boosts",
					ShortName = $"SPB",
					Graphics = gao_speedBoostParent
				});
				gao_speedBoostParent.transform.SetParent(parent3d);
			}

			// Return level
			return new Unity_Level(
				layers: layers.ToArray(),
				objManager: objManager,
				isometricData: new Unity_IsometricData {
					CollisionWidth = 0,
					CollisionHeight = 0,
					TilesWidth = 0,
					TilesHeight = 0,
					Collision = null,
					Scale = Vector3.one * 8,
					ViewAngle = Quaternion.Euler(90,0,0),
					CalculateYDisplacement = () => 0,
					CalculateXDisplacement = () => 0,
					ObjectScale = Vector3.one,
				},
				//objectGroups: objGroups,
				eventData: unityObjs,
				localization: LoadLocalization(context),
				defaultLayer: 0,
				defaultCollisionLayer: 0,
				cellSize: 8,
				trackManager: new Unity_TrackManager_Gameloft_RK() {
					Points = points.Select(p => scaledCenterPos + new Vector3(p.x, -p.z, p.y)).ToArray()
				});

			///throw new NotImplementedException();
		}
	}
}