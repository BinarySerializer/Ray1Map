using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R1Engine
{
    public abstract class GBAVV_Crash_BaseManager : GBAVV_Generic_BaseManager
    {
        // Metadata
        public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
        {
            new GameInfo_World(0, Enumerable.Range(0, LevInfos.Length).ToArray()),
        });

        // Helpers
        public void GetAvailableCollisionTypes(GBAVV_ROM_Crash rom)
        {
            var handledTypes = new HashSet<KeyValuePair<ushort, int>>();

            var levelIndex = 0;
            foreach (var level in rom.LevelInfos)
            {
                foreach (var map in level.LevelData.Maps)
                    handleMap(map);

                handleMap(level.LevelData.BonusMap);
                handleMap(level.LevelData.ChallengeMap);

                void handleMap(GBAVV_Generic_MapInfo map)
                {
                    if (map?.MapData2D == null)
                        return;

                    var collisionTiles = GetTileMap(map.MapData2D.CollisionLayer, map.MapData2D.LayersBlock.CollisionLayerData, false, isCollision: true);

                    foreach (var c in collisionTiles.Select(x => x.Data.CollisionType))
                    {
                        if (handledTypes.Any(x => x.Key == c))
                            continue;

                        handledTypes.Add(new KeyValuePair<ushort, int>(c, levelIndex));
                    }
                }

                levelIndex++;
            }

            var str = new StringBuilder();

            foreach (var c in handledTypes.OrderBy(x => BitHelpers.ExtractBits(x.Key, 8, 8)).ThenBy(x => BitHelpers.ExtractBits(x.Key, 8, 0)))
                str.AppendLine($"Type: {BitHelpers.ExtractBits(c.Key, 8, 8):00}, Shape: {BitHelpers.ExtractBits(c.Key, 8, 0):00}, Value: {c.Key:0000}, Level: {c.Value:00}");

            str.ToString().CopyToClipboard();
        }
        public void GetCommonIsometricCollisionIndices(IEnumerable<GBAVV_Isometric_CollisionType[]> types)
        {
            var commonTypes = new List<KeyValuePair<Pointer, byte[]>>();

            var levelIndex = 0;

            var output = new StringBuilder();

            foreach (var levelTypes in types)
            {
                output.AppendLine($"case {levelIndex}:");
                output.AppendLine($"    switch (index)");
                output.AppendLine($"    {{");

                var tIndex = 0;
                foreach (var t in levelTypes)
                {
                    if (!commonTypes.Any(x => x.Key == t.FunctionPointer_0 && x.Value.SequenceEqual(t.Bytes_10)))
                        commonTypes.Add(new KeyValuePair<Pointer, byte[]>(t.FunctionPointer_0, t.Bytes_10));

                    output.AppendLine($"        case {tIndex}: return {commonTypes.FindIndex(x => x.Key == t.FunctionPointer_0 && x.Value.SequenceEqual(t.Bytes_10))};");
                    tIndex++;
                }

                output.AppendLine($"    }}");
                output.AppendLine($"    break;");

                levelIndex++;
            }

            output.ToString().CopyToClipboard();
        }

        // Localization
        public const string LocTableID = "LocTable";
        public abstract int LocTableCount { get; }
        public override (Dictionary<string, string[]>, Dictionary<Pointer, int>) LoadLocalization(GBAVV_BaseROM rom)
        {
            var crashRom = (GBAVV_ROM_Crash)rom;

            var settings = rom.Context.Settings;

            var langages = new string[]
            {
                "English",
                "French",
                "German",
                "Spanish",
                "Italian",
                "Dutch"
            };

            if (settings.GameModeSelection == GameModeSelection.Crash1GBAJP || settings.GameModeSelection == GameModeSelection.Crash2GBAJP)
                langages[0] = "Japanese";

            return (crashRom.LocTables?.Select((x, i) =>
            {
                var str = x.Strings.Concat(GetAdditionalLocStrings(crashRom, i)).ToArray(); ;

                return new
                {
                    Lang = langages[i],
                    Strings = str
                };
            }).ToDictionary(x => x.Lang, x => x.Strings), null);
        }
        public virtual IEnumerable<string> GetAdditionalLocStrings(GBAVV_ROM_Crash rom, int langIndex) => new string[0];

        // Levels
        public abstract CrashLevInfo[] LevInfos { get; }
        public class CrashLevInfo
        {
            public CrashLevInfo(int levelIndex, int mapIndex, string displayName)
            {
                LevelIndex = levelIndex;
                MapIndex = mapIndex;
                MapType = Type.Normal;
                DisplayName = displayName;
            }
            public CrashLevInfo(int levelIndex, Type mapType, string displayName)
            {
                LevelIndex = levelIndex;
                MapIndex = -1;
                MapType = mapType;
                DisplayName = $"{displayName} - {mapType.ToString().Replace("Challenge", "Gem Route")}";
            }
            public CrashLevInfo(GBAVV_Generic_MapInfo.GBAVV_MapType? specialMapType, short index3D, string displayName)
            {
                LevelIndex = -1;
                MapIndex = -1;
                SpecialMapType = specialMapType;
                Index3D = index3D;
                DisplayName = displayName;
            }

            public int LevelIndex { get; }
            public int MapIndex { get; }
            public Type MapType { get; }
            public GBAVV_Generic_MapInfo.GBAVV_MapType? SpecialMapType { get; }
            public short Index3D { get; }
            public string DisplayName { get; set; }

            public bool IsSpecialMap => LevelIndex == -1;
            public bool IsWorldMap => IsSpecialMap && SpecialMapType == null;

            public enum Type
            {
                Normal,
                Bonus,
                Challenge
            }
        }
    }
}