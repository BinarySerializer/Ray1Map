using System;
using Cysharp.Threading.Tasks;

using System.Linq;
using System.Text;
using BinarySerializer;

namespace R1Engine
{
    public abstract class GBAVV_Volume_BaseManager : GBAVV_BaseManager
    {
        // Metadata
        public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
        {
            new GameInfo_World(0, Enumerable.Range(0, LevInfos.Length).ToArray()),
        });
        public abstract int VolumesCount { get; }
        public virtual bool HasAssignedObjTypeGraphics => false;

        // Exports
        public override GBAVV_BaseROM LoadROMForExport(Context context) => FileFactory.Read<GBAVV_ROM_Volume>(GetROMFilePath, context, (s, x) => x.CurrentLevInfo = LevInfos[context.GetR1Settings().Level]);
        public override UniTask ExportCutscenesAsync(GameSettings settings, string outputDir) => throw new System.NotImplementedException();

        // Load
        public override async UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures)
        {
            if (GraphicsDataPointers.Length == 0)
                FindDataInROM(context.Deserializer);

            if (ObjTypeInitInfos == null && ObjTypesCount > 0)
                LogObjTypeInit(context.Deserializer);

            Controller.DetailedState = "Loading data";
            await Controller.WaitIfNecessary();

            var rom = FileFactory.Read<GBAVV_ROM_Volume>(GetROMFilePath, context, (s, x) => x.CurrentLevInfo = LevInfos.ElementAtOrDefault(context.GetR1Settings().Level));
            
            if (LevInfos.Length == 0)
                GenerateLevInfos(rom);

            return await LoadMap2DAsync(context, rom, rom.CurrentMap, HasAssignedObjTypeGraphics);
        }

        // Helpers
        public void GenerateLevInfos(GBAVV_ROM_Volume rom)
        {
            var str = new StringBuilder();

            for (int volumeIndex = 0; volumeIndex < rom.Volumes.Length; volumeIndex++)
            {
                var volume = rom.Volumes[volumeIndex];

                if (volume.PrimaryLevelInfo != null)
                    for (int mapIndex = 0; mapIndex < volume.PrimaryLevelInfo.MapInfos.Length; mapIndex++)
                        str.AppendLine($"new LevInfo({volumeIndex}, -1, {mapIndex}, \"{volume.VolumeName?.DefaultString}\", \"{volume.PrimaryLevelInfo.InternalLevelName}\", \"{volume.PrimaryLevelInfo.LevelName?.DefaultString}\"),");

                for (int levelIndex = 0; levelIndex < volume.LevelInfos.Length; levelIndex++)
                {
                    var level = volume.LevelInfos[levelIndex];

                    for (int mapIndex = 0; mapIndex < level.MapInfos.Length; mapIndex++)
                        str.AppendLine($"new LevInfo({volumeIndex}, {levelIndex}, {mapIndex}, \"{volume.VolumeName?.DefaultString}\", \"{level.InternalLevelName}\", \"{level.LevelName?.DefaultString}\"),");
                }
            }


            str.ToString().CopyToClipboard();
        }

        // Levels
        public abstract LevInfo[] LevInfos { get; }

        public class LevInfo
        {
            public LevInfo(int volume, int level, int map, string volumeName, string internalLevelName, string levelName)
            {
                Volume = volume;
                Level = level;
                Map = map;
                VolumeName = volumeName;
                InternalLevelName = internalLevelName;
                LevelName = levelName;
            }

            public int Volume { get; }
            public int Level { get; }
            public int Map { get; }
            public string VolumeName { get; }
            public string InternalLevelName { get; }
            public string LevelName { get; }
            public string DisplayName
            {
                get
                {
                    var volume = String.IsNullOrWhiteSpace(VolumeName) ? $"{Volume + 1}" : VolumeName;
                    var level = Level == -1 ? " " : $" - {(String.IsNullOrWhiteSpace(LevelName) ? $"{Level + 1}-" : $"{LevelName} ")}";
                    var map = $"{Map + 1}";

                    return $"{volume}{level}{map}";
                }
            }
        }
    }
}