using System.Collections.Generic;
using Cysharp.Threading.Tasks;

using System.IO;
using System.Linq;
using BinarySerializer;
using UnityEngine;

namespace Ray1Map.GBAVV
{
    public abstract class GBAVV_Crash1_Manager : GBAVV_Crash_BaseManager
    {
        public override int LocTableCount => 70;

        public override GameAction[] GetGameActions(GameSettings settings)
        {
            return base.GetGameActions(settings).Concat(new GameAction[]
            {
                new GameAction("Export level icons", false, true, (input, output) => ExportLevelIcons(settings, output)),
            }).ToArray();
        }

        public override GBAVV_BaseROM LoadROMForExport(Context context) => FileFactory.Read<GBAVV_ROM_Crash1>(GetROMFilePath, context, (s, d) => d.CurrentLevInfo = LevInfos.First());
        public override GBAVV_ROM_Generic LoadROMForMode7Export(Context context, int level) => FileFactory.Read<GBAVV_ROM_Crash1>(GetROMFilePath, context, (s, d) => d.CurrentLevInfo = new CrashLevInfo(GBAVV_Generic_MapInfo.GBAVV_Crash_MapType.Mode7, (short)level, null));

        public override async UniTask ExportCutscenesAsync(GameSettings settings, string outputDir)
        {
            using (var context = new Ray1MapContext(settings))
            {
                await LoadFilesAsync(context);

                // Read the rom
                var rom = FileFactory.Read<GBAVV_ROM_Crash1>(GetROMFilePath, context, (s, d) => d.CurrentLevInfo = LevInfos.First());

                for (int cutsceneIndex = 0; cutsceneIndex < rom.CutsceneTable.Length; cutsceneIndex++)
                {
                    var c = rom.CutsceneTable[cutsceneIndex];

                    for (int frameIndex = 0; frameIndex < c.Frames.Length; frameIndex++)
                    {
                        var frame = c.Frames[frameIndex].Graphics;

                        if (frame.ImageData == null)
                            continue;

                        var pal = Util.ConvertGBAPalette(frame.Palette, transparentIndex: null);

                        var tex = TextureHelpers.CreateTexture2D(240, 160);

                        for (int y = 0; y < tex.height; y++)
                        {
                            for (int x = 0; x < tex.width; x++)
                            {
                                tex.SetPixel(x, tex.height - y - 1, pal[frame.ImageData[y * tex.width + x]]);
                            }
                        }

                        tex.Apply();

                        Util.ByteArrayToFile(Path.Combine(outputDir, $"{cutsceneIndex}-{frameIndex}.png"), tex.EncodeToPNG());
                    }
                }
            }
        }

        public async UniTask ExportLevelIcons(GameSettings settings, string outputDir)
        {
            using (var context = new Ray1MapContext(settings))
            {
                // Load the files
                await LoadFilesAsync(context);

                // Read the rom
                var rom = FileFactory.Read<GBAVV_ROM_Crash1>(GetROMFilePath, context, (s, d) => d.CurrentLevInfo = new CrashLevInfo(null, 0, null));

                // Enumerate every level icon
                for (int i = 0; i < rom.WorldMapLevelIcons.Length; i++)
                {
                    var tex = rom.WorldMapLevelIcons[i].ToTexture2D();

                    Util.ByteArrayToFile(Path.Combine(outputDir, $"{i}.png"), tex.EncodeToPNG());
                }
            }
        }

        public override async UniTask<Unity_Level> LoadAsync(Context context)
        {
            Controller.DetailedState = "Loading data";
            await Controller.WaitIfNecessary();

            var rom = FileFactory.Read<GBAVV_ROM_Crash1>(GetROMFilePath, context, (s, r) => r.CurrentLevInfo = LevInfos[context.GetR1Settings().Level]);
            var map = rom.CurrentMapInfo;

            if (map.Crash_MapType == GBAVV_Generic_MapInfo.GBAVV_Crash_MapType.Mode7)
                return await LoadMode7Async(context, rom, rom.CurrentMode7LevelInfo);
            else
                return await LoadMap2DAsync(context, rom, rom.CurrentMapInfo, rom.GetTheme);
        }

        public override int[] Mode7AnimSetCounts => new int[]
        {
            41, 47, 47
        };
        public override int Mode7LevelsCount => 7;

        public override IEnumerable<string> GetAdditionalLocStrings(GBAVV_ROM_Crash rom, int langIndex) => ((GBAVV_ROM_Crash1)rom).CutsceneStrings[langIndex].Cutscenes.SelectMany(c => c).Select(s => s?.String?.Text);

        public override CrashLevInfo[] LevInfos => Levels;
        public static CrashLevInfo[] Levels = new CrashLevInfo[]
        {
            new CrashLevInfo(0, 0, "Jungle Jam"),
            new CrashLevInfo(0, CrashLevInfo.Type.Bonus, "Jungle Jam"),
            new CrashLevInfo(1, 0, "Shipwrecked"),
            new CrashLevInfo(2, 0, "Temple of Boom"),
            new CrashLevInfo(2, CrashLevInfo.Type.Bonus, "Temple of Boom"),
            new CrashLevInfo(2, CrashLevInfo.Type.Challenge, "Temple of Boom"),
            new CrashLevInfo(3, 0, "Frostbite Cavern - Part 1"),
            new CrashLevInfo(3, 1, "Frostbite Cavern - Part 2"),
            new CrashLevInfo(3, 2, "Frostbite Cavern - Part 3"),
            new CrashLevInfo(3, CrashLevInfo.Type.Bonus, "Frostbite Cavern"),
            new CrashLevInfo(4, 0, "Just in Slime"),
            new CrashLevInfo(4, CrashLevInfo.Type.Bonus, "Just in Slime"),
            new CrashLevInfo(5, 0, "Snow Crash - Part 1"),
            new CrashLevInfo(5, 1, "Snow Crash - Part 2"),
            new CrashLevInfo(5, 2, "Snow Crash - Part 3"),
            new CrashLevInfo(5, CrashLevInfo.Type.Bonus, "Snow Crash"),
            new CrashLevInfo(5, CrashLevInfo.Type.Challenge, "Snow Crash"),
            new CrashLevInfo(6, 0, "Rocket Racket"),
            new CrashLevInfo(7, 0, "Just Hangin'"),
            new CrashLevInfo(7, CrashLevInfo.Type.Bonus, "Just Hangin'"),
            new CrashLevInfo(7, CrashLevInfo.Type.Challenge, "Just Hangin'"),
            new CrashLevInfo(8, 0, "Shark Attack"),
            new CrashLevInfo(9, 0, "Ruined"),
            new CrashLevInfo(9, CrashLevInfo.Type.Bonus, "Ruined"),
            new CrashLevInfo(10, 0, "Snow Job - Part 1"),
            new CrashLevInfo(10, 1, "Snow Job - Part 2"),
            new CrashLevInfo(10, 2, "Snow Job - Part 3"),
            new CrashLevInfo(10, CrashLevInfo.Type.Bonus, "Snow Job"),
            new CrashLevInfo(11, 0, "Ace of Space"),
            new CrashLevInfo(11, CrashLevInfo.Type.Bonus, "Ace of Space"),
            new CrashLevInfo(11, CrashLevInfo.Type.Challenge, "Ace of Space"),
            new CrashLevInfo(12, 0, "Sunken City"),
            new CrashLevInfo(13, 0, "Down the Hole"),
            new CrashLevInfo(13, CrashLevInfo.Type.Bonus, "Down the Hole"),
            new CrashLevInfo(14, 0, "Blimp Bonanza"),
            new CrashLevInfo(15, 0, "Star to Finish"),
            new CrashLevInfo(15, CrashLevInfo.Type.Bonus, "Star to Finish"),
            new CrashLevInfo(16, 0, "Air Supply"),
            new CrashLevInfo(17, 0, "No-fly Zone"),
            new CrashLevInfo(18, 0, "Drip, drip, drip"),
            new CrashLevInfo(18, CrashLevInfo.Type.Bonus, "Drip, drip, drip"),
            new CrashLevInfo(19, 0, "Final Countdown"),
            new CrashLevInfo(19, CrashLevInfo.Type.Bonus, "Final Countdown"),
            new CrashLevInfo(20, 0, "Dingodile"),
            new CrashLevInfo(21, 0, "N. Gin"),
            new CrashLevInfo(22, 0, "Tiny"),
            new CrashLevInfo(23, 0, "Neo Cortex"),
            new CrashLevInfo(24, 0, "Mega-mix"),
        };
    }

    public class GBAVV_Crash1EU_Manager : GBAVV_Crash1_Manager
    {
        public override uint[] GraphicsDataPointers => new uint[]
        {
            0x084a5600
        };
    }
    public class GBAVV_Crash1US_Manager : GBAVV_Crash1_Manager
    {
        public override uint[] GraphicsDataPointers => new uint[]
        {
            0x084a3624
        };
    }
    public class GBAVV_Crash1JP_Manager : GBAVV_Crash1_Manager
    {
        public override int LocTableCount => 73;
        public override uint[] GraphicsDataPointers => new uint[]
        {
            0x084a63f8
        };
    }
}