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
                        // I've used the EU version as a base which is why it has the most comments and such. The JP version
                        // has two additional sets which I added at the end of the array to avoid messing with the indexes.

                        // TODO: Fill out palettes
                        GBAIsometricRegion.EU => new (uint, uint)[]
                        {
                            (0x081D1C4C, 0x081fc810), // 0 (Mode7 Spyro)
                            (0x081E63FC, 0x081FC830), // 1 (Mode7 Spyro projectile)
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
                            (0x08242C6C, 0x081fccb0), // 19 (Mode7 enemy)
                            (0x0824353C, 0x081fcd10), // 20 (Mode7 enemy) // TODO: Has alternate palette as well
                            (0x0824A180, 0x081fccb0), // 21 (Mode7 enemy)
                            (0x0824AAC8, 0x00), // 22
                            (0x0824F4B8, 0x00), // 23
                            (0x08259404, 0x00), // 24
                            (0x0825EF04, 0x00), // 25
                            (0x08260B88, 0x00), // 26
                            (0x082627B4, 0x00), // 27
                            (0x0826534C, 0x00), // 28

                            (0x084B08D8, 0x084aff38), // 29 (Spyro)
                            (0x08560C64, 0x084aff58), // 30 (flame)
                            (0x0858699C, 0x084aff78), // 31 (Sparx)
                            (0x08586F98, 0x084aff78), // 32 (Sparx, duplicate of 31)
                            (0x08587594, 0x084aff38), // 33 (shadow)
                            (0x085BDB1C, 0x00), // 34
                            (0x085BE9A0, 0x084aff38), // 35 (HUD Spyro)
                            (0x085BF5D8, 0x084aff78), // 36 (menu Sparx)
                            (0x085BF7D4, 0x084aff98), // 37 (red gem)
                            (0x085C0038, 0x084aff98), // 38 (red gem effect)
                            (0x085C06C8, 0x084aff98), // 39 (green gem)
                            (0x085C0F2C, 0x084aff98), // 40 (green gem effect)
                            (0x085C15BC, 0x084affb8), // 41 (blue gem)
                            (0x085C1FF8, 0x084affb8), // 42 (blue gem effect)
                            (0x085C2688, 0x084affb8), // 43 (yellow gem)
                            (0x085C2E2C, 0x084affb8), // 44 (yellow gem effect)
                            (0x085C34BC, 0x084aff98), // 45 (HUD gem)
                            (0x085C3A34, 0x084affb8), // 46 (HUD digits)
                            (0x085C404C, 0x084b0018), // 47 (gem container flame)
                            (0x085C73E8, 0x084afff8), // 48 (gem container charge)
                            (0x085CA784, 0x00), // 49
                            (0x085CCC60, 0x00), // 50 (fairy) // TODO: Different palettes per sprites, one is 0x084affd8
                            (0x085D9974, 0x084b0038), // 51 (portal) (combined sprites)
                            (0x085DA448, 0x084b0058), // 52 (frozen fairy)
                            (0x085DDF90, 0x00), // 53 (HUD fairy) // TODO: Different palettes per sprites, one is 0x084affd8
                            (0x085DE1B4, 0x00), // 54
                            (0x085DE38C, 0x00), // 55
                            (0x085DE6C8, 0x00), // 56 (locked chest)
                            (0x085E18F8, 0x00), // 57 (chest key)
                            (0x085E1EF4, 0x00), // 58 (HUD chest key)
                            (0x085E1FAC, 0x00), // 59
                            (0x085E6020, 0x084b0058), // 60 (life)
                            (0x085E6198, 0x084aff58), // 61 (life cork)
                            (0x085E61F0, 0x00), // 62
                            (0x085E65A8, 0x00), // 63 (gem digits)
                            (0x085E6E24, 0x084b0098), // 64 (rabbit)
                            (0x085EB484, 0x00), // 65
                            (0x085EC154, 0x00), // 66 (pumpkin)

                            (0x085ECB48, 0x00), // 67 (Hunter)
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
                        GBAIsometricRegion.US => new (uint SpriteSetOffset, uint PaletteOffset)[]
                        {
                            (0x081CE584, 0x081F9148), // 0
                            (0x081E2D34, 0x081F9168), // 1
                            (0x081E3AD0, 0x00), // 2
                            (0x081E65FC, 0x00), // 3
                            (0x081F073C, 0x00), // 4
                            (0x081F0C88, 0x00), // 5
                            (0x081F1194, 0x00), // 6
                            (0x081F15F8, 0x00), // 7
                            (0x081F19FC, 0x00), // 8
                            (0x0821EFA4, 0x00), // 9
                            (0x08221880, 0x00), // 10
                            (0x0822446C, 0x00), // 11
                            (0x08227630, 0x00), // 12
                            (0x08228040, 0x00), // 13
                            (0x0822ACC0, 0x00), // 14
                            (0x0822BB60, 0x00), // 15
                            (0x0822D888, 0x00), // 16
                            (0x0822EB8C, 0x00), // 17
                            (0x0823572C, 0x00), // 18
                            (0x0823F5A4, 0x081F95E8), // 19
                            (0x0823FE74, 0x081F9648), // 20
                            (0x08246AB8, 0x081F95E8), // 21
                            (0x08247400, 0x00), // 22
                            (0x0824BDF0, 0x00), // 23
                            (0x08255D3C, 0x00), // 24
                            (0x0825B83C, 0x00), // 25
                            (0x0825D4C0, 0x00), // 26
                            (0x0825F0EC, 0x00), // 27
                            (0x08261C84, 0x00), // 28
                            (0x08490318, 0x0848F978), // 29
                            (0x085406C4, 0x0848F998), // 30
                            (0x085663FC, 0x0848F9B8), // 31
                            (0x085669F8, 0x0848F9B8), // 32
                            (0x08566FF4, 0x0848F978), // 33
                            (0x0859D57C, 0x00), // 34
                            (0x0859E400, 0x0848F978), // 35
                            (0x0859F038, 0x0848F9B8), // 36
                            (0x0859F234, 0x0848F9D8), // 37
                            (0x0859FA98, 0x0848F9D8), // 38
                            (0x085A0128, 0x0848F9D8), // 39
                            (0x085A098C, 0x0848F9D8), // 40
                            (0x085A101C, 0x081F9108), // 41
                            (0x085A1A58, 0x081F9108), // 42
                            (0x085A20E8, 0x081F9108), // 43
                            (0x085A288C, 0x081F9108), // 44
                            (0x085A2F1C, 0x0848F9D8), // 45
                            (0x085A3494, 0x081F9108), // 46
                            (0x085A3AAC, 0x0848FA58), // 47
                            (0x085A6E48, 0x0848FA38), // 48
                            (0x085AA1E4, 0x00), // 49
                            (0x085AC6C0, 0x00), // 50
                            (0x085B93D4, 0x0848FA78), // 51
                            (0x085B9EA8, 0x0848FA98), // 52
                            (0x085BD9F0, 0x00), // 53
                            (0x085BDC14, 0x00), // 54
                            (0x085BDDEC, 0x00), // 55
                            (0x085BE128, 0x00), // 56
                            (0x085C1358, 0x00), // 57
                            (0x085C1954, 0x00), // 58
                            (0x085C1A0C, 0x00), // 59
                            (0x085C5A80, 0x0848FA98), // 60
                            (0x085C5BF8, 0x0848F998), // 61
                            (0x085C5C50, 0x00), // 62
                            (0x085C6008, 0x00), // 63
                            (0x085C6884, 0x0848FAD8), // 64
                            (0x085CAEE4, 0x00), // 65
                            (0x085CBBB4, 0x00), // 66
                            (0x085CC5A8, 0x00), // 67
                            (0x085DA184, 0x00), // 68
                            (0x085EA9B0, 0x00), // 69
                            (0x085F6C1C, 0x00), // 70
                            (0x085F7560, 0x0848FB98), // 71
                            (0x085FD53C, 0x00), // 72
                            (0x085FE1E0, 0x0848FBD8), // 73
                            (0x08604DB0, 0x0848FC18), // 74
                            (0x08606FE4, 0x00), // 75
                            (0x08607D20, 0x00), // 76
                            (0x08608BF0, 0x0848FC78), // 77
                            (0x08618DB4, 0x0848FC98), // 78
                            (0x0862C60C, 0x00), // 79
                            (0x08632464, 0x00), // 80
                            (0x0863282C, 0x00), // 81
                            (0x08634154, 0x00), // 82
                            (0x0863428C, 0x00), // 83
                            (0x08639F64, 0x00), // 84
                            (0x0863A938, 0x00), // 85
                            (0x0863AD74, 0x00), // 86
                            (0x0863BB78, 0x0848FD18), // 87
                            (0x0864ACBC, 0x0848FD38), // 88
                            (0x0865B2D8, 0x00), // 89
                            (0x0865F638, 0x00), // 90
                            (0x086659A4, 0x00), // 91
                            (0x08666904, 0x00), // 92
                            (0x08666FFC, 0x00), // 93
                            (0x08667570, 0x00), // 94
                            (0x086685B4, 0x0848FDB8), // 95
                            (0x0867010C, 0x0848FDD8), // 96
                            (0x08675DC4, 0x00), // 97
                            (0x0867FA04, 0x00), // 98
                            (0x08681A68, 0x00), // 99
                            (0x08688F3C, 0x00), // 100
                            (0x0868937C, 0x00), // 101
                            (0x0868A080, 0x00), // 102
                            (0x08692798, 0x00), // 103
                            (0x0869FDD0, 0x00), // 104
                            (0x086A49F8, 0x00), // 105
                            (0x086A7344, 0x00), // 106
                            (0x086A74C8, 0x00), // 107
                            (0x086A9140, 0x00), // 108
                            (0x086AA190, 0x0848FEF8), // 109
                            (0x086AF47C, 0x00), // 110
                            (0x086B01E8, 0x00), // 111
                            (0x086B3210, 0x00), // 112
                            (0x086B32E8, 0x00), // 113
                            (0x086B409C, 0x00), // 114
                            (0x086B40F4, 0x00), // 115
                            (0x086B4518, 0x00), // 116
                            (0x086B5C6C, 0x0848FF78), // 117
                            (0x086BD064, 0x0848FF98), // 118
                            (0x086C5958, 0x00), // 119
                            (0x086C7D64, 0x00), // 120
                            (0x086C7EFC, 0x00), // 121
                            (0x086C801C, 0x00), // 122
                            (0x086C8C54, 0x08490018), // 123
                            (0x086D074C, 0x08490038), // 124
                            (0x086DC5A4, 0x00), // 125
                            (0x086E3028, 0x00), // 126
                            (0x086E3F40, 0x00), // 127
                            (0x086E4164, 0x00), // 128
                            (0x086E4400, 0x00), // 129
                            (0x086E4598, 0x00), // 130
                            (0x086E4CB4, 0x00), // 131
                            (0x086E5B38, 0x08490098), // 132
                            (0x086EE350, 0x084900B8), // 133
                            (0x086FA2A8, 0x00), // 134
                            (0x086FE948, 0x00), // 135
                            (0x086FEB14, 0x00), // 136
                            (0x086FF0D0, 0x00), // 137
                            (0x08700258, 0x08490138), // 138
                            (0x087071B0, 0x08490158), // 139
                            (0x08717E68, 0x00), // 140
                            (0x0871BDF8, 0x00), // 141
                            (0x0871C3A0, 0x00), // 142
                            (0x0871D274, 0x00), // 143
                            (0x0871D36C, 0x00), // 144
                            (0x0871DFCC, 0x00), // 145
                            (0x0871E4F0, 0x00), // 146
                            (0x0871F074, 0x084901D8), // 147
                            (0x0872A40C, 0x00), // 148
                            (0x0872ACE0, 0x00), // 149
                            (0x0872BD24, 0x08490258), // 150
                            (0x08731B70, 0x00), // 151
                            (0x08737184, 0x00), // 152
                            (0x0873729C, 0x00), // 153
                            (0x0873788C, 0x00), // 154
                            (0x08738888, 0x0805EC88), // 155
                        },
                        GBAIsometricRegion.JP => new (uint SpriteSetOffset, uint PaletteOffset)[]
                        {
                            (0x081D3D50, 0x081FE914), // 0
                            (0x081E8500, 0x081FE934), // 1
                            (0x081E929C, 0x00), // 2
                            (0x081EBDC8, 0x00), // 3
                            (0x081F5F08, 0x00), // 4
                            (0x081F6454, 0x00), // 5
                            (0x081F6960, 0x00), // 6
                            (0x081F6DC4, 0x00), // 7
                            (0x081F71C8, 0x00), // 8
                            (0x08224770, 0x00), // 9
                            (0x0822704C, 0x00), // 10
                            (0x08229C38, 0x00), // 11
                            (0x0822CDFC, 0x00), // 12
                            (0x0822D80C, 0x00), // 13
                            (0x0823048C, 0x00), // 14
                            (0x0823132C, 0x00), // 15
                            (0x08233054, 0x00), // 16
                            (0x08234358, 0x00), // 17
                            (0x0823AEF8, 0x00), // 18
                            (0x08244D70, 0x081FEDB4), // 19
                            (0x08245640, 0x081FEE14), // 20
                            (0x0824C284, 0x081FEDB4), // 21
                            (0x0824CBCC, 0x00), // 22
                            (0x082515BC, 0x00), // 23
                            (0x0825B508, 0x00), // 24
                            (0x08261008, 0x00), // 25
                            (0x08262C8C, 0x00), // 26
                            (0x082648B8, 0x00), // 27
                            (0x08267450, 0x00), // 28
                            (0x084A9F40, 0x084A9580), // 29 (modified)
                            (0x08564628, 0x084A95A0), // 30
                            (0x0858A360, 0x084A95C0), // 31
                            (0x0858A95C, 0x084A95C0), // 32 (modified)
                            (0x0858AF58, 0x084A9580), // 33
                            (0x085C14E0, 0x00), // 34
                            (0x085C2364, 0x084A9580), // 35
                            (0x085C2F9C, 0x084A95C0), // 36
                            (0x085C3198, 0x084A95E0), // 37
                            (0x085C39FC, 0x084A95E0), // 38
                            (0x085C408C, 0x084A95E0), // 39
                            (0x085C48F0, 0x084A95E0), // 40
                            (0x085C4F80, 0x081FE8D4), // 41
                            (0x085C59BC, 0x081FE8D4), // 42 (modified)
                            (0x085C604C, 0x081FE8D4), // 43
                            (0x085C67F0, 0x081FE8D4), // 44 (modified)
                            (0x085C6E80, 0x084A95E0), // 45
                            (0x085C73F8, 0x081FE8D4), // 46
                            (0x085C7A10, 0x084A9660), // 47
                            (0x085CADAC, 0x084A9640), // 48
                            (0x085CE148, 0x00), // 49
                            (0x085D0624, 0x00), // 50
                            (0x085DD338, 0x084A9680), // 51
                            (0x085DDE0C, 0x084A96A0), // 52
                            (0x085E1954, 0x00), // 53
                            (0x085E1B78, 0x00), // 54
                            (0x085E1D50, 0x00), // 55
                            (0x085E208C, 0x00), // 56
                            (0x085E52BC, 0x00), // 57
                            (0x085E58B8, 0x00), // 58
                            (0x085E5970, 0x00), // 59
                            (0x085E99E4, 0x084A96A0), // 60
                            (0x085E9B5C, 0x084A95A0), // 61
                            (0x085E9BB4, 0x00), // 62
                            (0x085E9F6C, 0x00), // 63
                            (0x085EA7E8, 0x084A96E0), // 64
                            (0x085EEE48, 0x00), // 65
                            (0x085EFB18, 0x00), // 66
                            (0x085F050C, 0x00), // 67
                            (0x085FE0E8, 0x00), // 68
                            (0x0860E914, 0x00), // 69 (modified)
                            (0x0861AB60, 0x00), // 70
                            (0x0861BD78, 0x084A97A0), // 71
                            (0x08621D54, 0x00), // 72 (modified)
                            (0x08622958, 0x084A97E0), // 73
                            (0x08629528, 0x084A9820), // 74
                            (0x0862B75C, 0x00), // 75
                            (0x0862C498, 0x00), // 76
                            (0x0862D368, 0x084A9880), // 77
                            (0x0863D52C, 0x084A98A0), // 78
                            (0x08650D84, 0x00), // 79
                            (0x08656BDC, 0x00), // 80
                            (0x08656FA4, 0x00), // 81
                            (0x086588CC, 0x00), // 82
                            (0x08658A04, 0x00), // 83
                            (0x0865E6DC, 0x00), // 84
                            (0x0865F0B0, 0x00), // 85
                            (0x0865F4EC, 0x00), // 86
                            (0x086602F0, 0x084A9920), // 87
                            (0x0866F434, 0x084A9940), // 88
                            (0x0867FA50, 0x00), // 89
                            (0x08683DB0, 0x00), // 90
                            (0x0868A11C, 0x00), // 91
                            (0x0868B07C, 0x00), // 92
                            (0x0868B774, 0x00), // 93
                            (0x0868BCE8, 0x00), // 94
                            (0x0868CD2C, 0x084A99C0), // 95
                            (0x08694884, 0x084A99E0), // 96
                            (0x0869A53C, 0x00), // 97
                            (0x0869FE5C, 0x00), // 98
                            (0x086A1EC0, 0x00), // 99
                            (0x086A9394, 0x00), // 100
                            (0x086A97D4, 0x00), // 101
                            (0x086AA4D8, 0x00), // 102
                            (0x086B2BF0, 0x00), // 103
                            (0x086C0228, 0x00), // 104 (modified)
                            (0x086C4E30, 0x00), // 105
                            (0x086C777C, 0x00), // 106
                            (0x086C7900, 0x00), // 107
                            (0x086C9578, 0x00), // 108
                            (0x086CA5C8, 0x084A9B00), // 109
                            (0x086CF8B4, 0x00), // 110
                            (0x086D0620, 0x00), // 111
                            (0x086D3648, 0x00), // 112
                            (0x086D3720, 0x00), // 113
                            (0x086D44D4, 0x00), // 114
                            (0x086D452C, 0x00), // 115
                            (0x086D4950, 0x00), // 116
                            (0x086D60A4, 0x084A9B80), // 117
                            (0x086DD49C, 0x084A9BA0), // 118
                            (0x086E5D90, 0x00), // 119
                            (0x086E819C, 0x00), // 120
                            (0x086E8334, 0x00), // 121
                            (0x0873BD90, 0x00), // 122
                            (0x0873C9C8, 0x084A9C20), // 123
                            (0x0872DFE8, 0x084A9C40), // 124
                            (0x087444C0, 0x00), // 125
                            (0x08739E40, 0x00), // 126 (modified)
                            (0x0873B298, 0x00), // 127
                            (0x0874AF44, 0x00), // 128
                            (0x0874B1E0, 0x00), // 129
                            (0x0874B378, 0x00), // 130
                            (0x086E8454, 0x00), // 131
                            (0x086E92D8, 0x084A9CA0), // 132
                            (0x086F1AF0, 0x084A9CC0), // 133
                            (0x086FDA48, 0x00), // 134
                            (0x087020E8, 0x00), // 135
                            (0x087022B4, 0x00), // 136
                            (0x08702870, 0x00), // 137
                            (0x087039F8, 0x084A9D40), // 138
                            (0x0870A950, 0x084A9D60), // 139
                            (0x0871B608, 0x00), // 140
                            (0x0871F598, 0x00), // 141
                            (0x0871FB40, 0x00), // 142
                            (0x08720A14, 0x00), // 143
                            (0x08720F48, 0x00), // 144
                            (0x08721BA8, 0x00), // 145
                            (0x087220CC, 0x00), // 146
                            (0x08722C50, 0x084A9DE0), // 147
                            (0x0873B4BC, 0x00), // 148
                            (0x0874BA94, 0x00), // 149
                            (0x0874CAD8, 0x084A9E80), // 150
                            (0x08752924, 0x00), // 151
                            (0x08757F38, 0x00), // 152
                            (0x08758050, 0x00), // 153
                            (0x08758640, 0x00), // 154
                            (0x0875963C, 0x084A9EE0), // 155

                            (0x0861B4A4, 0x00), // 156 (unique)
                            (0x08720B0C, 0x00), // 157 (unique)
                        },
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