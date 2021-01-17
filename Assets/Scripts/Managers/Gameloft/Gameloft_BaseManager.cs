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
        public abstract string[] ResourceFiles { get; }
        public abstract string[] SingleResourceFiles { get; }

        public GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
        {
            new GameInfo_World(0, Enumerable.Range(0, 1).ToArray()),
        });

        public GameAction[] GetGameActions(GameSettings settings) => new GameAction[]
        {
            new GameAction("Export Resources", false, true, (input, output) => ExportResourcesAsync(settings, output, false)),
        };

        public async UniTask ExportResourcesAsync(GameSettings settings, string outputDir, bool convert) {
            using (var context = new Context(settings)) {
                foreach(var filePath in ResourceFiles) {
                    var f = await context.AddLinearSerializedFileAsync(filePath);
                    SerializerObject s = context.Deserializer;
                    s.DoAt(f.StartPointer, () => {
                        try {
                            var resf = s.SerializeObject<Gameloft_ResourceFile>(default, name: f.filePath);
                            ExportResourceFile(resf, s, Path.Combine(outputDir,filePath));
                        } catch (Exception ex) {
                            Debug.LogError(ex);
                        }
                    });
                    await Controller.WaitIfNecessary();
                }
            }
        }

        public enum ResourceType {
            Graphics,
            PNG,
            MIDI,
            WAV,
            Other
        }

        public void ExportResourceFile(Gameloft_ResourceFile resf, SerializerObject s, string outputDir) {
            for (int i = 0; i < resf.ResourcesCount; i++) {
                var res = resf.SerializeResource<Gameloft_DummyResource>(s, default, i, name: $"Resource_{i}");
                var restype = ResourceType.Other;
                if (res.Data.Length >= 5) {
                    using (Reader reader = new Reader(new MemoryStream(res.Data), isLittleEndian: false)) {
                        if (reader.ReadUInt32() == 0x89504E47) {
                            restype = ResourceType.PNG;
                        }
                        reader.BaseStream.Position = 0;
                        if (reader.ReadUInt32() == 0x4D546864) {
                            restype = ResourceType.MIDI;
                        }
                        reader.BaseStream.Position = 0;
                        if (reader.ReadUInt32() == 0xDF030101) {
                            restype = ResourceType.Graphics;
                        }
                        reader.BaseStream.Position = 0;
                        if (reader.ReadUInt32() == 0x52494646) {
                            restype = ResourceType.WAV;
                        }
                        reader.BaseStream.Position = 0;
                    }
                }
                string extension = "dat";
                switch (restype) {
                    case ResourceType.Graphics: extension = "graphics"; break;
                    case ResourceType.MIDI: extension = "mid"; break;
                    case ResourceType.PNG: extension = "png"; break;
                    case ResourceType.WAV: extension = "wav"; break;
                }
                Util.ByteArrayToFile(Path.Combine(outputDir, $"{i}.{extension}"), res.Data);
            }
        }

        public async UniTask SaveLevelAsync(Context context, Unity_Level level) {
            await UniTask.CompletedTask;
            throw new NotImplementedException();
        }

        public async UniTask LoadFilesAsync(Context context) {
            await UniTask.CompletedTask;
            throw new NotImplementedException();
        }

		public async UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures) {
            await UniTask.CompletedTask;
            throw new NotImplementedException();
		}
	}
}