using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using ImageMagick;
using R1Engine.Serialize;
using UnityEngine;

namespace R1Engine
{
    public abstract class Gameloft_BaseManager : IGameManager
    {

        public GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
        {
            new GameInfo_World(0, Enumerable.Range(0, 1).ToArray()),
        });

        public GameAction[] GetGameActions(GameSettings settings) => new GameAction[]
        {
            new GameAction("Export Resources", false, true, (input, output) => ExportResourcesAsync(settings, output, false)),
        };

        public async UniTask ExportResourcesAsync(GameSettings settings, string outputDir, bool convert)
        {
            throw new NotImplementedException();
        }

        public UniTask SaveLevelAsync(Context context, Unity_Level level) => throw new NotImplementedException();

        public async UniTask LoadFilesAsync(Context context) {
            throw new NotImplementedException();
        }

		public UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures) {
			throw new NotImplementedException();
		}
	}
}