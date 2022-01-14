using Cysharp.Threading.Tasks;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinarySerializer;
using BinarySerializer.Audio;
using BinarySerializer.GBA;
using UnityEngine;

namespace Ray1Map.GBA
{
    public class GBA_R3NGage_Manager : GBA_R3_Manager
    {
        public override string GetROMFilePath(Context context) => $"rayman3.dat";
        public string ExeFilePath => "rayman3.app";

        public override IEnumerable<int>[] WorldLevels => new IEnumerable<int>[]
        {
            Enumerable.Range(0, 8), // World 1
            Enumerable.Range(8, 9), // World 2
            Enumerable.Range(17, 13), // World 3
            Enumerable.Range(30, 10), // World 4
            Enumerable.Range(40, 8), // Bonus
            Enumerable.Range(48, 6), // Ly
            Enumerable.Range(54, 5), // World
            Enumerable.Range(59, 10), // Multiplayer
        };

        public override int[] MenuLevels => new int[]
        {
            103,
            131
        };

        public override int DLCLevelCount => 0;
        public override bool HasR3SinglePakLevel => false;

        public override int[] AdditionalSprites4bpp => Enumerable.Range(79, 103 - 79).Concat(Enumerable.Range(104, 131 - 104)).Concat(Enumerable.Range(133, 140 - 133)).ToArray();

        public override int[] AdditionalSprites8bpp => new int[]
        {
            132
        };

        public override async UniTask ExtractVignetteAsync(GameSettings settings, string outputDir)
        {
            // Create a context
            using (var context = new Ray1MapContext(settings))
            {
                // Load the ROM
                await LoadFilesAsync(context);

                // Load the data
                var dataBlock = LoadDataBlock(context);

                // Get the deserialize
                var s = context.Deserializer;

                const int vignetteCount = 22;
                const int vignetteStart = 143;
                const int palStart = 165;

                for (int i = 0; i < vignetteCount; i++)
                {
                    // Decode image data
                    byte[] data = null;
                    s.DoAt(dataBlock.UiOffsetTable.GetPointer(vignetteStart + i), () => s.DoEncoded(new GBA_LZSSEncoder(), () => data = s.SerializeArray<byte>(default, s.CurrentLength)));

                    // Read palette
                    var palette = s.DoAt(dataBlock.UiOffsetTable.GetPointer(palStart + i), () => s.SerializeObjectArray<RGBA5551Color>(default, 256));

                    // Create a texture
                    var tex = TextureHelpers.CreateTexture2D(176, 208);

                    // Set pixels
                    for (int y = 0; y < tex.height; y++)
                    {
                        for (int x = 0; x < tex.width; x++)
                        {
                            var c = palette[data[y * tex.width + x]].GetColor();

                            // Remove transparency
                            c.a = 1;

                            // Set pixel and reverse height
                            tex.SetPixel(x, tex.height - y - 1, c);
                        }
                    }

                    tex.Apply();

                    // Export
                    Util.ByteArrayToFile(Path.Combine(outputDir, $"Vig_{i}.png"), tex.EncodeToPNG());
                }

                ExtractVignetteCreditsIcons(context, dataBlock, 141, outputDir);
            }
        }

        public override GBA_Data LoadDataBlock(Context context) => FileFactory.Read<GBA_Data>(context, GetROMFilePath(context), (_, x) => x.Pre_SerializeLocalization = false);
        public override GBA_Localization LoadLocalizationTable(Context context) => FileFactory.Read<GBA_R3Ngage_ExeFile>(context, ExeFilePath).Localization;

        public override async UniTask LoadFilesAsync(Context context)
        {
            await context.AddLinearFileAsync(GetROMFilePath(context));
            await context.AddMemoryMappedFile(ExeFilePath, 0x0fffff84);
        }


        // Game actions
        public override GameAction[] GetGameActions(GameSettings settings) {
            return base.GetGameActions(settings).Concat(new GameAction[]
            {
                new GameAction("Export Music & Sounds", false, true, (input, output) => ExportMusicAsync(settings, output)),
            }).ToArray();
        }

        // NOTE: In order to use this the FileSystem class needs to be updated to allow file stream sharing for read/write streams. Also note that this method will overwrite the GBA rom file, so keep a backup! All N-Gage data is appended rather than replaced, so the data can be swapped out by changing the offsets. Only the level offsets are automatically changed to use the N-Gage versions, thus keeping the menu etc.
        public async UniTask AddNGageToGBAAsync(GameModeSelection gbaGameModeSelection)
        {
            // Create a GBA manager
            var gbaManager = new GBA_R3_Manager();

            // Create a GBA context
            using (var gbaContext = new Ray1MapContext(new GameSettings(gbaGameModeSelection, Settings.GameDirectories[gbaGameModeSelection], 0, 0)))
            {
                // Load GBA files
                await gbaManager.LoadFilesAsync(gbaContext);

                // Create an N-Gage context
                using (var ngageContext = new Ray1MapContext(new GameSettings(GameModeSelection.Rayman3NGage, Settings.GameDirectories[GameModeSelection.Rayman3NGage], 0, 0)))
                {
                    // Load N-Gage files
                    await LoadFilesAsync(ngageContext);

                    // Read the GBA data
                    var gbaData = gbaManager.LoadDataBlock(gbaContext);

                    // Read N-Gage data
                    var ngageData = LoadDataBlock(ngageContext);

                    // Get offsets
                    var gbaBase = gbaData.UiOffsetTable.Offset;
                    var ngageNewBase = ((MemoryMappedFile)gbaContext.GetFile(gbaManager.GetROMFilePath(gbaContext))).StartPointer + ((MemoryMappedFile)gbaContext.GetFile(gbaManager.GetROMFilePath(gbaContext))).Length;
                    var ngageOffset = (ngageNewBase - gbaBase) / 4;

                    // Read all N-Gage blocks
                    var s = ngageContext.Deserializer;
                    var ngageBlocks = Enumerable.Range(0, ngageData.UiOffsetTable.OffsetsCount).Select(x => ngageData.UiOffsetTable.GetPointer(x)).Select(x => s.DoAt(x, () => s.SerializeObject<GBA_DummyBlock>(default))).ToArray();

                    // Keep track of relocated blocks
                    var relocated = new HashSet<GBA_DummyBlock>();

                    // Recursively relocate all offsets
                    foreach (var block in ngageBlocks)
                        RelocateBlock(block);

                    void RelocateBlock(GBA_DummyBlock block)
                    {
                        // Don't relocate blocks which have been relocated
                        if (relocated.Contains(block))
                            return;
                        relocated.Add(block);

                        // Relocate all sub-blocks
                        foreach (var subBlock in block.SubBlocks)
                            RelocateBlock(subBlock);

                        // Relocate offsets
                        for (int i = 0; i < block.OffsetTable.Offsets.Length; i++)
                            block.OffsetTable.Offsets[i] = (int)(block.OffsetTable.Offsets[i] + ngageOffset);

                        // Update the offsets for the R1Serializable
                        block.Init(ngageNewBase + block.Offset.FileOffset);
                        block.OffsetTable.Init(ngageNewBase + block.Offset.FileOffset + block.BlockSize);
                    }

                    // Write the blocks
                    var ss = ngageContext.Serializer;
                    foreach (var block in ngageBlocks)
                        ss.DoAt(block.Offset, () => ss.SerializeObject(block));

                    // Update level offsets
                    for (int i = 0; i < 65; i++)
                        ss.DoAt(gbaBase + 4 + (i * 4), () => ss.Serialize<int>((int)((ngageBlocks[i].Offset.FileOffset - gbaBase.FileOffset) / 4)));
                }
            }
        }


        public async UniTask ExportMusicAsync(GameSettings settings, string outputPath) {
            using (var context = new Ray1MapContext(settings)) {
                // Load the ROM
                await LoadFilesAsync(context);

                // Load the data
                var dataBlock = LoadDataBlock(context);

                // Get the deserialize
                var s = context.Deserializer;

                // Track order matches the the digiBLAST version sorted based on filenames in alphabetical order :)
                string[] musicnames = new string[] {
                    "ancients",
                    "baddreams",
                    "barbeslide",
                    "barrel",
                    "barrel_BA",
                    "bigplatform",
                    "bigtrees",
                    "boss1",
                    "boss34",
                    "canopy",
                    "death",
                    "echocave",
                    "enemy1",
                    "enemy2",
                    "fairyglades",
                    "finalboss",
                    "firestone",
                    "happyslide",
                    "helico",
                    "jano",
                    "lyfree",
                    "lyfreeVOX4",
                    "lyrace",
                    "mountain1",
                    "mountain2",
                    "polokus",
                    "precipice",
                    "raytheme",
                    "rockchase",
                    "rocket",
                    "rocket_BA",
                    "sadslide",
                    "ship",
                    "spiderchase",
                    "tag",
                    "tizetre",
                    "tizetre_Swing",
                    "waterski",
                    "win1",
                    "win2",
                    "win3",
                    "Win_BOSS",
                    "woodlight"
                };

                string[] soundnames = new string[] {
                    "unnamed0",
                    "unnamed1",
                    "unnamed2",
                    "unnamed3",
                    "ABA1",
                    "ABD2",
                    "Back01_Mix01",
                    "BallInit_Mix01",
                    "BangGen1_Mix07",
                    "BarlFall_Mix04",
                    "BBQ_Mix10",
                    "BCC1",
                    "BCD1",
                    "BDA",
                    "BDC2",
                    "BeepFX01_Mix02",
                    "BigFoot1_Mix02",
                    "BlobFX02_Mix02",
                    "BodyAtk1_Mix01",
                    "Boing_Mix02",
                    "BombFly_Mix03",
                    "BossHurt_Mix02",
                    "BossVO01_Mix01",
                    "BossVO02_Mix01",
                    "BossVO03_Mix01",
                    "Bounce00_Mix03",
                    "Bounce01_Mix02",
                    "Bounce02_Mix03",
                    "BSB1",
                    "BSNAd2",
                    "CaFlyDie_Mix03",
                    "CageHit_Mix07",
                    "CageSnd1_Mix02",
                    "CageSnd2_Mix02",
                    "CageTrsh_Mix05",
                    "CagoAttk_Mix03",
                    "CagoDie2_Mix01",
                    "CagOno01_Mix01",
                    "CagoTurn_Mix03",
                    "CagouHit_Mix03",
                    "CagouRit_Mix03",
                    "CagoWlk1_Mix02",
                    "CGAA2",
                    "CGAF3",
                    "CGAF4",
                    "Chain_Mix01",
                    "Charge2_Mix04",
                    "Charge_Mix05",
                    "CLAF4",
                    "Cloche01_Mix01",
                    "Combust1_Mix02",
                    "CountDwn_Mix07",
                    "CYC4",
                    "CYG4",
                    "Electric_Mix02",
                    "ETC3",
                    "FishDead_Mix02",
                    "FishJaw1_Mix02",
                    "GhstDead_Mix05",
                    "Globox_Mix04",
                    "Grenad01_Mix03",
                    "Grimace1_Mix04",
                    "GumBlow_Mix01",
                    "GumChew_Mix02",
                    "HandTap1_Mix04",
                    "HandTap2_Mix03",
                    "Helico01_Mix10",
                    "HeliCut_Mix01",
                    "HeliStop_Mix06",
                    "HHCd5",
                    "HHDd5",
                    "Horror_Mix08",
                    "HorseCry_Mix02",
                    "INPA3",
                    "INPAd3",
                    "INPE2",
                    "INPF3",
                    "INPG3",
                    "INPGd4",
                    "Janogrrr_Mix03",
                    "JanoGrwl_Mix03",
                    "JanoRire_Mix01",
                    "JanoShot_Mix01",
                    "JanoSkul_Mix03",
                    "Laser1_Mix02",
                    "Laser2_Mix02",
                    "Laser3_Mix03",
                    "Laser4_Mix01",
                    "LavaBubl_Mix02",
                    "LavaStrt_Mix04",
                    "LightFX1_Mix01",
                    "LineFX01_Mix02",
                    "LumAtk01_Mix02",
                    "LumAtk02_Mix01",
                    "LumBleu_Mix02",
                    "LumBoost_Mix01",
                    "LumGreen_Mix04",
                    "LumHit_Mix03",
                    "LumMauve_Mix02",
                    "LumOrag_Mix06",
                    "LumPeek_Mix01",
                    "LumRed_Mix03",
                    "LumSlvr_Mix02",
                    "LumSwing_Mix03",
                    "LumTimer_Mix02",
                    "LumTotal_Mix02",
                    "LyMagic1_Mix01",
                    "LyMagic2_Mix07",
                    "LyVO1_Mix01",
                    "MAC3",
                    "MachAtk1_Mix01",
                    "MachAtk2_Mix02",
                    "MachMotr_Mix01",
                    "MenuMove",
                    "MetlGate_Mix01",
                    "MinHP",
                    "Missile1_Mix01",
                    "Motor01_Mix12",
                    "MPB3",
                    "MPC2",
                    "MPG2",
                    "MumuDead_Mix04",
                    "MurfHeli_Mix01",
                    "MurfyVO1A_Mix01",
                    "MurfyVO1B_Mix01",
                    "MurfyVO3A_Mix01",
                    "MurfyVO4A_Mix01",
                    "NewPower_Mix06",
                    "NoCombus_Mix04",
                    "OnoEfor1_Mix02",
                    "OnoEfor2_Mix03",
                    "OnoEquil_Mix03",
                    "OnoExpir",
                    "OnoGO_Mix02",
                    "OnoInspi",
                    "OnoJump1",
                    "OnoJump3_Mix01",
                    "OnoJump4_Mix01",
                    "OnoJump5_Mix01",
                    "OnoJump6_Mix01",
                    "OnoPeur1_Mix03",
                    "OnoRcvH1_Mix04",
                    "OnoThrow_Mix02",
                    "OnoWinRM_Mix02",
                    "OnoWin_Mix02",
                    "OnoWoHoo_Mix01",
                    "ORGC4",
                    "pada",
                    "PannelDw_Mix01",
                    "PannelUp_Mix01",
                    "PathFX_Mix01",
                    "PF2Crac_Mix02",
                    "PF2Fall_Mix03",
                    "PIAd4",
                    "PICd4",
                    "PIG3",
                    "PinBall_Mix02",
                    "PiraAtk1_Mix01",
                    "PiraDead_Mix05",
                    "PiraHit1_Mix02",
                    "PiraHit3_Mix03",
                    "PiraHurt_Mix02",
                    "PlumSnd1_Mix03",
                    "PlumSnd2_Mix03",
                    "RaDeath_Mix03",
                    "RayFist2_Mix01",
                    "RayFist_Mix02",
                    "RaySigh_Mix01",
                    "RaySpin_Mix06",
                    "RireMumu_Mix03",
                    "RocktLeg_Mix03",
                    "RoktSpin_Mix03",
                    "RootIn_Mix01",
                    "RootOut_Mix04",
                    "RyVO1_Mix01",
                    "RyVO2_Mix01",
                    "RyVO3_Mix01",
                    "RyVOGlob_Mix02",
                    "RyVOLy_Mix01",
                    "ScaHurt2_Mix02",
                    "ScalDead_Mix02",
                    "ScalFlee_Mix02",
                    "ScalGrrr_Mix02",
                    "ScalHurt_Mix02",
                    "ScalUp_Mix03",
                    "ScaMorf1_Mix02",
                    "ScaMorf2_Mix02",
                    "SHKCd5",
                    "SHKDd4",
                    "SkiLoop1",
                    "SkiWeed_Mix02",
                    "SkulInit_Mix04",
                    "SkullEnd_Mix02",
                    "SkullHit_Mix02",
                    "SkulShak_Mix01",
                    "SlideIn_Mix02",
                    "SlideOut_Mix01",
                    "SND1",
                    "SNF1",
                    "SnoreFX1_Mix01",
                    "SocleFX1_Mix01",
                    "Sparkles_Mix01",
                    "SpherImp_Mix02",
                    "SpidrAtk_Mix02",
                    "Spirale_Mix01",
                    "SplshGen_Mix04",
                    "Store01_Mix01",
                    "Store02_Mix02",
                    "STRDd3",
                    "SuprFist_Mix01",
                    "Switch1_Mix03",
                    "Tag_Mix02",
                    "TAMB",
                    "Thunder1_Mix04",
                    "TiztrVO1_Mix01",
                    "TiztrVO2_Mix01",
                    "TiztrVO3_Mix01",
                    "TiztrVO4_Mix01",
                    "Valid01_Mix01",
                    "VibraFLW_Mix02",
                    "VICd3",
                    "VIG3",
                    "WallSlid_Mix02",
                    "Whistle1_Mix01",
                    "WoodBrk1_Mix04",
                    "WoodImp_Mix03",
                    "YC7XFG1",
                    "YG1C1",
                    "YG1D2",
                    "YG1F2",
                    "YoyoMove_Mix02"
                };

                // Music is stored at the end
                const int trackCount = 43;
                int trackStart = dataBlock.UiOffsetTable.OffsetsCount - trackCount;
                int instrumentBlock = trackStart - 1;

                int soundStart = 190;
                int soundCount = instrumentBlock - soundStart;

                XM[] Tracks = new XM[trackCount];
                XM_Instrument[] Instruments = null;

                for (int i = 0; i < trackCount; i++) {
                    s.DoAt(dataBlock.UiOffsetTable.GetPointer(trackStart + i), () => {
                        Tracks[i] = s.SerializeObject<XM>(Tracks[i], onPreSerialize: t => t.Pre_SerializeInstruments = false, name: $"{nameof(Tracks)}[{i}]");
                    });
                }
                s.DoAt(dataBlock.UiOffsetTable.GetPointer(instrumentBlock), () => {
                    //for (int i = 0; i < trackCount; i++) {
                    ushort instrumentsCount = 0;
					instrumentsCount = s.Serialize<ushort>(instrumentsCount, name: nameof(instrumentsCount));
                    Instruments = s.SerializeObjectArray<XM_Instrument>(Instruments, instrumentsCount, name: nameof(Instruments));
                    //}
                });

                for (int i = 0; i < trackCount; i++) {
                    //Tracks[i].NumInstruments = (ushort)Instruments.Length;
                    Tracks[i].Instruments = new XM_Instrument[Tracks[i].NumInstruments];
                    for (int j = 0; j < Tracks[i].Instruments.Length; j++) {
                        Tracks[i].Instruments[j] = Instruments[j];
                    }
                    Tracks[i].Pre_SerializeInstruments = true;


                    // Get the output path
                    var name = $"music/{musicnames[i]}.xm";
                    var outputFilePath = Path.Combine(outputPath, name);

                    Directory.CreateDirectory(Path.GetDirectoryName(outputFilePath));
                    using (var outputStream = File.Create(outputFilePath)) {
                        using (var xmContext = new Ray1MapContext(outputPath, settings)) {
                            xmContext.AddFile(new StreamFile(context, name, outputStream));
                            FileFactory.Write<XM>(xmContext, name, Tracks[i]);
                        }
                    }
                }
                for (int i = 0; i < soundCount; i++) {
                    var name = $"sounds/{soundnames[i]}.wav";
                    var outputFilePath = Path.Combine(outputPath, name);
                    s.DoAt(dataBlock.UiOffsetTable.GetPointer(soundStart + i), () => {
                        var soundBlock = s.SerializeObject<GBA_DummyBlock>(default, name: $"SoundBlocks[{i}]");
                        Util.ByteArrayToFile(outputFilePath, soundBlock.Data);
                    });
                }
            }
        }

        public override int[] LocalizationGroupLengths => new int[]
        {
            17,
            6,
            9,
            2,
            2,
            3,
            2,
            7,
            35, // Level names
            13,
            1, // Credits
            40, // Multiplayer
        };
        public override string[] Languages => new string[]
        {
            "English",
            "EnglishUS",
            "French",
            "Spanish",
            "Italian",
            "German",
        };
    }
}