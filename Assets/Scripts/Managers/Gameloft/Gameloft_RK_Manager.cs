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
			foreach (var o in level.TrackBlocks) {
				var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				sphere.transform.position = curPos + Vector3.up * curHeight;
				curPos += Quaternion.Euler(0,curAngle,0) * Vector3.forward;
				curHeight += o.DeltaHeight * 0.1f;
				curAngle -= o.DeltaRotation;
				sphere.gameObject.name = $"{o.Type} | {o.Flags} | {o.Unknown}";

				/*UnityEngine.Random.InitState((int)o.Unknown);
				var color = UnityEngine.Random.ColorHSV(0, 1, 0.2f, 1f, 0.8f, 1.0f);*/
				var color = level.Types[o.Type].ColorGround.GetColor();
				sphere.GetComponent<Renderer>().material.color = color;
			}
			throw new NotImplementedException();
		}
	}
}