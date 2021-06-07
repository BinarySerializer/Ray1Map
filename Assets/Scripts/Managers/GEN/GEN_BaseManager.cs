using BinarySerializer;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace R1Engine
{
    public class GEN_BaseManager : BaseGameManager
	{
		// Levels
        public override GameInfo_Volume[] GetLevels(GameSettings settings) => new GameInfo_Volume[0];

		// Game actions
		public override GameAction[] GetGameActions(GameSettings settings) => new GameAction[]
		{
			new GameAction("Extract UBI files", false, true, (input, output) => ExtractFilesAsync(settings, output)),
		};
        public async UniTask ExtractFilesAsync(GameSettings settings, string outputDir) {
            using (var context = new R1Context(settings)) {
				var s = context.Deserializer;

				foreach (var filePath in Directory.EnumerateFiles(context.BasePath, "*.ubi", SearchOption.AllDirectories)) {
					string fileName = filePath.Substring(context.BasePath.Length).Replace("\\", "/");
					var file = await context.AddLinearSerializedFileAsync(fileName, Endian.Little);
					GEN_UBI ubi = FileFactory.Read<GEN_UBI>(fileName, context);
				}
				foreach (var filePath in Directory.EnumerateFiles(context.BasePath, "*.UBI", SearchOption.AllDirectories)) {
					string fileName = filePath.Substring(context.BasePath.Length).Replace("\\", "/");
					var file = await context.AddLinearSerializedFileAsync(fileName, Endian.Little);
					GEN_UBI ubi = FileFactory.Read<GEN_UBI>(fileName, context);
				}
			}
        }

		// Load
		public override async UniTask<Unity_Level> LoadAsync(Context context) 
        {
			throw new NotImplementedException();
		}

        public override async UniTask LoadFilesAsync(Context context) {
			throw new NotImplementedException();
		}
    }
}
