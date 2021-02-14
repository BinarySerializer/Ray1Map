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

		public override async UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures) {
			await UniTask.CompletedTask;
			var resf = FileFactory.Read<Gameloft_ResourceFile>(GetLevelPath(context.Settings), context);
			var ind = GetLevelResourceIndex(context.Settings);
			var level = resf.SerializeResource<Gameloft_Level3D>(context.Deserializer, default, ind, name: $"Level_{ind}");

			UnityEngine.Debug.Log("Sum rotation: " + level.TrackBlocks.Sum(o => o.DeltaRotation));
			UnityEngine.Debug.Log("Sum height: " + level.TrackBlocks.Sum(o => o.DeltaHeight));
			Vector3 curPos = Vector3.zero;
			float curAngle = 0f;
			float curHeight = 0f;
			int curBlockIndex = 0;
			foreach (var o in level.TrackBlocks) {
				var sphere = GameObject.CreatePrimitive(PrimitiveType.Cube);
				sphere.transform.position = curPos + Vector3.up * curHeight;
				sphere.transform.rotation = Quaternion.Euler(0, curAngle, 0);
				sphere.transform.localScale = new Vector3((level.Types[o.Type].Short2 > 0 ? level.Types[o.Type].Short2 : 1000) / 1000f, 1, 1);

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
				sphere.gameObject.name = $"{o.Type} | {o.Flags} | {o.Unknown}";

				/*UnityEngine.Random.InitState((int)o.Unknown);
				var color = UnityEngine.Random.ColorHSV(0, 1, 0.2f, 1f, 0.8f, 1.0f);*/
				var color = level.Types[o.Type].ColorGround.GetColor();
				sphere.GetComponent<Renderer>().material.color = color;
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