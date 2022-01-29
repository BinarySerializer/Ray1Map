using System;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometricSettings
    {
        public GBAIsometricSettings() { }
        public GBAIsometricSettings(GBAIsometricEngineVersion engineVersion, GBAIsometricRegion region)
        {
            MajorEngineVersion = engineVersion switch
            {
                GBAIsometricEngineVersion.Spyro1 => GBAIsometricMajorEngineVersion.Ice,
                GBAIsometricEngineVersion.Spyro2 => GBAIsometricMajorEngineVersion.Dragon,
                GBAIsometricEngineVersion.Spyro3 => GBAIsometricMajorEngineVersion.Dragon,
                GBAIsometricEngineVersion.Tron => GBAIsometricMajorEngineVersion.Dragon,
                GBAIsometricEngineVersion.Rayman => GBAIsometricMajorEngineVersion.Rayman,
                _ => throw new ArgumentOutOfRangeException(nameof(engineVersion), engineVersion, null)
            };
            EngineVersion = engineVersion;
            Region = region;

            switch (engineVersion)
            {
                case GBAIsometricEngineVersion.Spyro1:
                    Ice_SpriteSetOffsets = region switch
                    {
                        // TODO: Fill out palettes
                        GBAIsometricRegion.EU => new (uint, uint)[]
                        {
                            (0x081D1C4C, 0x00), // 0
                            (0x081E63FC, 0x00), // 1
                            (0x081E7198, 0x00), // 2
                            (0x081E9CC4, 0x00), // 3
                            (0x081F3E04, 0x00), // 4
                            (0x081F4350, 0x00), // 5
                            (0x081F485C, 0x00), // 6
                            (0x081F4CC0, 0x00), // 7
                            (0x081F50C4, 0x00), // 8
                            (0x0822266C, 0x00), // 9
                            (0x08224F48, 0x00), // 10
                            (0x08227B34, 0x00), // 11
                            (0x0822ACF8, 0x00), // 12
                            (0x0822B708, 0x00), // 13
                            (0x0822E388, 0x00), // 14
                            (0x0822F228, 0x00), // 15
                            (0x08230F50, 0x00), // 16
                            (0x08232254, 0x00), // 17
                            (0x08238DF4, 0x00), // 18
                            (0x08242C6C, 0x00), // 19
                            (0x0824353C, 0x00), // 20
                            (0x0824A180, 0x00), // 21
                            (0x0824AAC8, 0x00), // 22
                            (0x0824F4B8, 0x00), // 23
                            (0x08259404, 0x00), // 24
                            (0x0825EF04, 0x00), // 25
                            (0x08260B88, 0x00), // 26
                            (0x082627B4, 0x00), // 27
                            (0x0826534C, 0x00), // 28

                            (0x084B08D8, 0x084aff38), // 29 (Spyro)
                            (0x08560C64, 0x00), // 30
                            (0x0858699C, 0x00), // 31
                            (0x08586F98, 0x00), // 32
                            (0x08587594, 0x00), // 33
                            (0x085BDB1C, 0x00), // 34
                            (0x085BE9A0, 0x00), // 35
                            (0x085BF5D8, 0x00), // 36
                            (0x085BF7D4, 0x084aff98), // 37 (red gem)
                            (0x085C0038, 0x00), // 38
                            (0x085C06C8, 0x084aff98), // 39 (green gem)
                            (0x085C0F2C, 0x00), // 40
                            (0x085C15BC, 0x084affb8), // 41 (blue gem)
                            (0x085C1FF8, 0x00), // 42
                            (0x085C2688, 0x084affb8), // 43 (yellow gem)
                            (0x085C2E2C, 0x00), // 44
                            (0x085C34BC, 0x084aff98), // 45 (HUD gem)
                            (0x085C3A34, 0x00), // 46
                            (0x085C404C, 0x084b0018), // 47 (gem container flame)
                            (0x085C73E8, 0x084afff8), // 48 (gem container charge)
                            (0x085CA784, 0x00), // 49
                            (0x085CCC60, 0x00), // 50
                            (0x085D9974, 0x084b0038), // 51 (portal) (combined sprites)
                            (0x085DA448, 0x00), // 52
                            (0x085DDF90, 0x084affd8), // 53 (HUD fairy) // TODO: Different palettes per sprites?
                            (0x085DE1B4, 0x00), // 54
                            (0x085DE38C, 0x00), // 55
                            (0x085DE6C8, 0x00), // 56
                            (0x085E18F8, 0x00), // 57
                            (0x085E1EF4, 0x00), // 58
                            (0x085E1FAC, 0x00), // 59
                            (0x085E6020, 0x084b0058), // 60 (life)
                            (0x085E6198, 0x084aff58), // 61 (life cork)
                            (0x085E61F0, 0x00), // 62
                            (0x085E65A8, 0x00), // 63
                            (0x085E6E24, 0x084b0098), // 64 (rabbit)
                            (0x085EB484, 0x00), // 65
                            (0x085EC154, 0x00), // 66

                            (0x085ECB48, 0x00), // 67
                            (0x085FA724, 0x00), // 68 (Bianca)
                            (0x0860AF50, 0x00), // 69 (Moneybags)
                            (0x086171BC, 0x00), // 70

                            (0x08617B00, 0x084b0158), // 71 (frog)
                            (0x0861DADC, 0x00), // 72
                            (0x0861E7AC, 0x084b0198), // 73 (sheep)
                            (0x0862537C, 0x084b01d8), // 74 (ice fodder)
                            (0x086275B0, 0x00), // 75

                            (0x086282EC, 0x00), // 76
                            (0x086291BC, 0x084b0238), // 77 (enemy)
                            (0x08639380, 0x084b0258), // 78 (enemy)
                            (0x0864CBD8, 0x00), // 79
                            (0x08652A30, 0x00), // 80
                            (0x08652DF8, 0x00), // 81
                            (0x08654720, 0x00), // 82
                            (0x08654858, 0x00), // 83
                            (0x0865A530, 0x00), // 84
                            (0x0865AF04, 0x00), // 85

                            (0x0865B340, 0x00), // 86
                            (0x0865C144, 0x084b02d8), // 87 (enemy)
                            (0x0866B288, 0x084b02f8), // 88 (enemy)
                            (0x0867B8A4, 0x00), // 89
                            (0x0867FC04, 0x00), // 90
                            (0x08685F70, 0x00), // 91
                            (0x08686ED0, 0x00), // 92
                            (0x086875C8, 0x00), // 93

                            (0x08687B3C, 0x00), // 94
                            (0x08688B80, 0x084b0378), // 95 (enemy)
                            (0x086906D8, 0x084b0398), // 96 (enemy)
                            (0x08696390, 0x00), // 97
                            (0x0869BCB0, 0x00), // 98
                            (0x0869DD14, 0x00), // 99
                            (0x086A51E8, 0x00), // 100

                            (0x086A5628, 0x00), // 101
                            (0x086A632C, 0x00), // 102
                            (0x086AEA44, 0x00), // 103
                            (0x086BC07C, 0x00), // 104
                            (0x086C0CA4, 0x00), // 105
                            (0x086C35F0, 0x00), // 106
                            (0x086C3774, 0x00), // 107 (machine) (combined sprites)

                            (0x086C53EC, 0x00), // 108
                            (0x086C643C, 0x084b04b8), // 109 (enemy)
                            (0x086CB728, 0x00), // 110
                            (0x086CC494, 0x00), // 111
                            (0x086CF4BC, 0x00), // 112
                            (0x086CF594, 0x00), // 113
                            (0x086D0348, 0x00), // 114
                            (0x086D03A0, 0x00), // 115

                            (0x086D07C4, 0x00), // 116
                            (0x086D1F18, 0x084b0538), // 117 (enemy)
                            (0x086D9310, 0x084b0558), // 118 (enemy)
                            (0x086E1C04, 0x00), // 119
                            (0x086E4010, 0x00), // 120
                            (0x086E41A8, 0x00), // 121

                            (0x086E42C8, 0x00), // 122
                            (0x086E4F00, 0x084b05d8), // 123 (enemy)
                            (0x086EC9F8, 0x084b05f8), // 124 (enemy)
                            (0x086F8850, 0x00), // 125
                            (0x086FF2D4, 0x00), // 126
                            (0x087001EC, 0x00), // 127
                            (0x08700410, 0x00), // 128
                            (0x087006AC, 0x00), // 129
                            (0x08700844, 0x00), // 130

                            (0x08700F60, 0x00), // 131
                            (0x08701DE4, 0x084b0658), // 132 (enemy)
                            (0x0870A5FC, 0x084b0678), // 133 (enemy)
                            (0x08716554, 0x00), // 134
                            (0x0871ABF4, 0x00), // 135
                            (0x0871ADC0, 0x00), // 136

                            (0x0871B37C, 0x00), // 137
                            (0x0871C504, 0x084b06f8), // 138 (enemy)
                            (0x0872345C, 0x084b0718), // 139 (enemy)
                            (0x08734114, 0x00), // 140
                            (0x087380A4, 0x00), // 141
                            (0x0873864C, 0x00), // 142
                            (0x08739520, 0x00), // 143
                            (0x08739618, 0x00), // 144
                            (0x0873A278, 0x00), // 145

                            (0x0873A79C, 0x00), // 146
                            (0x0873B320, 0x084b0798), // 147 (enemy)
                            (0x087466B8, 0x00), // 148

                            (0x08746F8C, 0x00), // 149
                            (0x08747FD0, 0x084b0818), // 150 (enemy)
                            (0x0874DE1C, 0x00), // 151
                            (0x08753430, 0x00), // 152
                            (0x08753548, 0x00), // 153

                            (0x08753B38, 0x00), // 154
                            (0x08754B34, 0x084b0878), // 155 (Grendor) (combined sprites)
                        },
                        // TODO: Fill out
                        GBAIsometricRegion.US => new (uint SpriteSetOffset, uint PaletteOffset)[0],
                        GBAIsometricRegion.JP => new (uint SpriteSetOffset, uint PaletteOffset)[0],
                        _ => throw new ArgumentOutOfRangeException(nameof(region), region, null)
                    };
                    ResourcesCount = region switch
                    {
                        GBAIsometricRegion.EU => 83,
                        GBAIsometricRegion.US => 83,
                        GBAIsometricRegion.JP => 84,
                        _ => throw new ArgumentOutOfRangeException(nameof(region), region, null)
                    };
                    CutsceneMapsCount = 21;
                    MenuPageCount = 10;
                    Languages = region switch
                    {
                        GBAIsometricRegion.EU => new[] { "English", "French", "Spanish", "German", "Italian" },
                        GBAIsometricRegion.US => new[] { "English" },
                        GBAIsometricRegion.JP => new[] { "Japanese" },
                        _ => throw new ArgumentOutOfRangeException(nameof(region), region, null)
                    };
                    break;
                
                case GBAIsometricEngineVersion.Spyro2:
                    ResourcesCount = region switch
                    {
                        GBAIsometricRegion.EU => 1514,
                        GBAIsometricRegion.US => 1509,
                        _ => throw new ArgumentOutOfRangeException(nameof(region), region, null)
                    };
                    PortraitsCount = 31;
                    DialogCount = 300;
                    CutsceneMapsCount = 11;
                    LevelMapsCount = 14;
                    TotalLevelsCount = 25;
                    ObjectTypesCount = 509;
                    AnimSetsCount = 162;
                    LevelDataCount = 39;
                    Languages = region switch
                    {
                        GBAIsometricRegion.EU => new [] { "English", "French", "Spanish", "German", "Italian" },
                        GBAIsometricRegion.US => new [] { "English" },
                        _ => throw new ArgumentOutOfRangeException(nameof(region), region, null)
                    };
                    break;
                
                case GBAIsometricEngineVersion.Spyro3:
                    ResourcesCount = region switch
                    {
                        GBAIsometricRegion.EU => 2269,
                        GBAIsometricRegion.US => 2180,
                        _ => throw new ArgumentOutOfRangeException(nameof(region), region, null)
                    };
                    PortraitsCount = 38;
                    DialogCount = 344;
                    PrimaryLevelCount = 14;
                    LevelMapsCount = 21;
                    TotalLevelsCount = 91;
                    ObjectTypesCount = 772;
                    AnimSetsCount = region switch
                    {
                        GBAIsometricRegion.EU => 194,
                        GBAIsometricRegion.US => 196,
                        _ => throw new ArgumentOutOfRangeException(nameof(region), region, null)
                    };
                    LevelDataCount = 80;
                    MenuPageCount = 18;
                    Languages = region switch
                    {
                        GBAIsometricRegion.EU => new[] { "English", "Dutch", "Spanish", "Italian", "German", "French" },
                        GBAIsometricRegion.US => new[] { "English" },
                        _ => throw new ArgumentOutOfRangeException(nameof(region), region, null)
                    };
                    break;
                
                case GBAIsometricEngineVersion.Tron:
                    ResourcesCount = 2148;
                    PortraitsCount = 57 - 1;
                    ObjectTypesCount = 352;
                    AnimSetsCount = 128;
                    LevelDataCount = 20;
                    break;
                
                case GBAIsometricEngineVersion.Rayman:
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(engineVersion), engineVersion, null);
            }
        }

        public GBAIsometricMajorEngineVersion MajorEngineVersion { get; set; }
        public GBAIsometricEngineVersion EngineVersion { get; set; }
        public GBAIsometricRegion Region { get; set; }

        // The game loads sprite sets into an array per level, with the objects referencing them by index in their
        // type init functions. Since all of this is entirely hard-coded, including the palettes, it's easier to create
        // a global list of all sprite sets and their associated palette.
        public (uint SpriteSetOffset, uint PaletteOffset)[] Ice_SpriteSetOffsets { get; set; }

        public int ResourcesCount { get; set; }
        public int PortraitsCount { get; set; }
        public int DialogCount { get; set; }
        public int CutsceneMapsCount { get; set; }
        public int PrimaryLevelCount { get; set; }
        public int LevelMapsCount { get; set; }
        public int TotalLevelsCount { get; set; }
        public int ObjectTypesCount { get; set; }
        public int AnimSetsCount { get; set; }
        public int LevelDataCount { get; set; }
        public int MenuPageCount { get; set; }

        public string[] Languages { get; set; }
    }
}