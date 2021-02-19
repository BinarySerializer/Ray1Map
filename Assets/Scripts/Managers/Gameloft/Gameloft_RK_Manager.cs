using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using System;
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
		}

		public void CreateTrackMesh(Gameloft_Level3D level) {
			Vector3 curPos = Vector3.zero;
			float curAngle = 0f;
			float curHeight = 0f;
			float roadSize = 1f;
			float groundSize = 2f;
			float groundDisplacement = -0.2f;

			var t = level.TrackBlocks;

			// Road
			Vector3[] verticesRoad = new Vector3[(t.Length) * 8];
			Vector3[] normalsRoad = new Vector3[verticesRoad.Length];
			Vector2[] uvsRoad = new Vector2[verticesRoad.Length];
			Color[] colorsRoad = new Color[verticesRoad.Length];
			int[] trianglesRoad = new int[(t.Length) * 12];

			// Ground
			Vector3[] verticesGround = new Vector3[(t.Length) * 8];
			Vector3[] normalsGround = new Vector3[verticesGround.Length];
			Vector2[] uvsGround = new Vector2[verticesGround.Length];
			Color[] colorsGround = new Color[verticesGround.Length];
			int[] trianglesGround = new int[(t.Length) * 12];

			for (int i = 0; i < t.Length; i++) {
				var curBlock = t[i];
				float roadWidthFactor = ((level.Types[curBlock.Type].Flags & 1) == 1 ? level.Types[curBlock.Type].Short2 : 1000) / 1000f;
				float roadSizeCur = roadSize * roadWidthFactor;

				Quaternion q = Quaternion.Euler(0, curAngle, 0);
				var curPosH = curPos + Vector3.up * curHeight;
				verticesRoad[(i * 8) + 0] = curPosH + q * Vector3.left * roadSizeCur;
				verticesRoad[(i * 8) + 1] = curPosH + q * Vector3.right * roadSizeCur;
				verticesRoad[(i * 8) + 2] = curPosH + q * Vector3.left * roadSizeCur;
				verticesRoad[(i * 8) + 3] = curPosH + q * Vector3.right * roadSizeCur;

				verticesGround[(i * 8) + 0] = curPosH + q * Vector3.left * groundSize;
				verticesGround[(i * 8) + 1] = curPosH + q * Vector3.right * groundSize;
				verticesGround[(i * 8) + 2] = curPosH + q * Vector3.left * groundSize;
				verticesGround[(i * 8) + 3] = curPosH + q * Vector3.right * groundSize;

				curPos += q * Vector3.forward;
				curHeight += curBlock.DeltaHeight * 0.05f;
				curAngle -= curBlock.DeltaRotation;

				q = Quaternion.Euler(0, curAngle, 0);
				curPosH = curPos + Vector3.up * curHeight;
				verticesRoad[(i * 8) + 4] = curPosH + q * Vector3.left * roadSizeCur;
				verticesRoad[(i * 8) + 5] = curPosH + q * Vector3.right * roadSizeCur;
				verticesRoad[(i * 8) + 6] = curPosH + q * Vector3.left * roadSizeCur;
				verticesRoad[(i * 8) + 7] = curPosH + q * Vector3.right * roadSizeCur;

				verticesGround[(i * 8) + 4] = curPosH + q * Vector3.left * groundSize;
				verticesGround[(i * 8) + 5] = curPosH + q * Vector3.right * groundSize;
				verticesGround[(i * 8) + 6] = curPosH + q * Vector3.left * groundSize;
				verticesGround[(i * 8) + 7] = curPosH + q * Vector3.right * groundSize;

				// Up
				trianglesRoad[(i * 12) + 0] = (i * 8) + 0;
				trianglesRoad[(i * 12) + 1] = (i * 8) + 4;
				trianglesRoad[(i * 12) + 2] = (i * 8) + 1;
				trianglesRoad[(i * 12) + 3] = (i * 8) + 1;
				trianglesRoad[(i * 12) + 4] = (i * 8) + 4;
				trianglesRoad[(i * 12) + 5] = (i * 8) + 5;
				// Down
				trianglesRoad[(i * 12) + 6] = (i * 8) + 2;
				trianglesRoad[(i * 12) + 7] = (i * 8) + 3;
				trianglesRoad[(i * 12) + 8] = (i * 8) + 7;
				trianglesRoad[(i * 12) + 9] = (i * 8) + 2;
				trianglesRoad[(i * 12) + 10] = (i * 8) + 7;
				trianglesRoad[(i * 12) + 11] = (i * 8) + 6;

				// Up
				trianglesGround[(i * 12) + 0] = (i * 8) + 0;
				trianglesGround[(i * 12) + 1] = (i * 8) + 4;
				trianglesGround[(i * 12) + 2] = (i * 8) + 1;
				trianglesGround[(i * 12) + 3] = (i * 8) + 1;
				trianglesGround[(i * 12) + 4] = (i * 8) + 4;
				trianglesGround[(i * 12) + 5] = (i * 8) + 5;
				// Down
				trianglesGround[(i * 12) + 6] = (i * 8) + 2;
				trianglesGround[(i * 12) + 7] = (i * 8) + 3;
				trianglesGround[(i * 12) + 8] = (i * 8) + 7;
				trianglesGround[(i * 12) + 9] = (i * 8) + 2;
				trianglesGround[(i * 12) + 10] = (i * 8) + 7;
				trianglesGround[(i * 12) + 11] = (i * 8) + 6;

				// Test colors
				var roadCol = (level.Types[curBlock.Type].Flags & 1) == 1 ? (level.Color_dk_BridgeLight ?? level.Types[curBlock.Type].ColorGround).GetColor() : Color.blue;
				colorsRoad[(i * 8) + 0] = roadCol;
				colorsRoad[(i * 8) + 1] = roadCol;
				colorsRoad[(i * 8) + 4] = roadCol;
				colorsRoad[(i * 8) + 5] = roadCol;

				colorsRoad[(i * 8) + 2] = Color.red;
				colorsRoad[(i * 8) + 3] = Color.red;
				colorsRoad[(i * 8) + 6] = Color.red;
				colorsRoad[(i * 8) + 7] = Color.red;

				// Ground colors
				var groundCol = (level.Types[curBlock.Type].Flags & 1) == 1 ? level.Types[curBlock.Type].Color8.GetColor() : level.Types[curBlock.Type].ColorGround.GetColor();
				colorsGround[(i * 8) + 0] = groundCol;
				colorsGround[(i * 8) + 1] = groundCol;
				colorsGround[(i * 8) + 4] = groundCol;
				colorsGround[(i * 8) + 5] = groundCol;
				colorsGround[(i * 8) + 2] = groundCol;
				colorsGround[(i * 8) + 3] = groundCol;
				colorsGround[(i * 8) + 6] = groundCol;
				colorsGround[(i * 8) + 7] = groundCol;

				// Normals
				//normalsRoad[(i * 8) + 0] =
			}

			GameObject gaoParent = new GameObject();
			gaoParent.transform.position = Vector3.zero;

			// Road
			Mesh roadMesh = new Mesh();
			roadMesh.vertices = verticesRoad;
			roadMesh.triangles = trianglesRoad;
			roadMesh.colors = colorsRoad;
			roadMesh.RecalculateNormals();
			GameObject gao = new GameObject("Road mesh");
			MeshFilter mf = gao.AddComponent<MeshFilter>();
			MeshRenderer mr = gao.AddComponent<MeshRenderer>();
			gao.layer = LayerMask.NameToLayer("3D Collision");
			gao.transform.SetParent(gaoParent.transform);
			gao.transform.localScale = Vector3.one;
			gao.transform.localPosition = Vector3.zero;
			mf.mesh = roadMesh;
			mr.material = Controller.obj.levelController.controllerTilemap.isometricCollisionMaterial;

			// Ground
			Mesh groundMesh = new Mesh();
			groundMesh.vertices = verticesGround;
			groundMesh.triangles = trianglesGround;
			groundMesh.colors = colorsGround;
			groundMesh.RecalculateNormals();
			GameObject g_gao = new GameObject("Ground mesh");
			MeshFilter g_mf = g_gao.AddComponent<MeshFilter>();
			MeshRenderer g_mr = g_gao.AddComponent<MeshRenderer>();
			g_gao.layer = LayerMask.NameToLayer("3D Collision");
			g_gao.transform.SetParent(gaoParent.transform);
			g_gao.transform.localScale = Vector3.one;
			g_gao.transform.localPosition = Vector3.zero + Vector3.up * groundDisplacement;
			g_mf.mesh = groundMesh;
			g_mr.material = Controller.obj.levelController.controllerTilemap.isometricCollisionMaterial;

		}

		public override async UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures) {
			await UniTask.CompletedTask;
			var resf = FileFactory.Read<Gameloft_ResourceFile>(GetLevelPath(context.Settings), context);
			var ind = GetLevelResourceIndex(context.Settings);
			var level = resf.SerializeResource<Gameloft_Level3D>(context.Deserializer, default, ind, name: $"Level_{ind}");

			CreateTrackMesh(level);

			UnityEngine.Debug.Log("Sum rotation: " + level.TrackBlocks.Sum(o => o.DeltaRotation));
			UnityEngine.Debug.Log("Sum height: " + level.TrackBlocks.Sum(o => o.DeltaHeight));
			Vector3 curPos = Vector3.zero;
			float curAngle = 0f;
			float curHeight = 0f;
			int curBlockIndex = 0;
			foreach (var o in level.TrackBlocks) {
				var sphere = new GameObject();//GameObject.CreatePrimitive(PrimitiveType.Cube);
				sphere.transform.position = curPos + Vector3.up * curHeight;
				sphere.transform.rotation = Quaternion.Euler(0, curAngle, 0);
				//sphere.transform.localScale = new Vector3((level.Types[o.Type].Short2 > 0 ? level.Types[o.Type].Short2 : 1000) / 1000f, 1, 1);

				var str12 = level.Structs12?.Where(s12 => s12.Struct0Index == curBlockIndex);
				if (str12 != null) {
					foreach (var s12 in str12) {
						var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
						cube.transform.SetParent(sphere.transform);
						cube.transform.localPosition = new Vector3(s12.Unknown * 0.005f,2,0);
						cube.transform.localRotation = Quaternion.identity;
						cube.transform.localScale = Vector3.one * 0.5f;
					}
				}
				var cj = level.TrackObjectCollections[curBlockIndex];
				for (int i = 0; i < cj.Count; i++) {
					var ci_ind = cj.InstanceIndex + i;
					var blk = level.TrackObjectInstances[ci_ind];
					var s8Ind = blk.TrackObjectIndex;
					var s8 = level.TrackObjects[s8Ind];
					var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
					cube.transform.SetParent(sphere.transform);
					cube.transform.localPosition = new Vector3(s8.XPosition * 0.001f, 1, (blk.Int0 - 6) / 12);
					cube.transform.localRotation = Quaternion.identity;
					cube.transform.localScale = Vector3.one * 0.5f;
					cube.gameObject.name = s8.ObjectType.ToString();
					UnityEngine.Random.InitState((int)s8.ObjectType);
					var cubecol = UnityEngine.Random.ColorHSV(0, 1, 0.2f, 1f, 0.8f, 1.0f);
					cube.GetComponent<Renderer>().material.color = cubecol;
				}

				curPos += Quaternion.Euler(0,curAngle,0) * Vector3.forward;
				curHeight += o.DeltaHeight * 0.05f;
				curAngle -= o.DeltaRotation;
				sphere.gameObject.name = $"{curBlockIndex}: {o.Type} | {o.Flags} | {o.Unknown} | {level.Types[o.Type].Flags}";

				/*UnityEngine.Random.InitState((int)o.Unknown);
				var color = UnityEngine.Random.ColorHSV(0, 1, 0.2f, 1f, 0.8f, 1.0f);*/
				//var color = level.Types[o.Type].ColorGround.GetColor();
				//sphere.GetComponent<Renderer>().material.color = color;
				//sphere.GetComponent<Renderer>().enabled = false;
				curBlockIndex++;
			}

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
			throw new NotImplementedException();
		}
	}
}