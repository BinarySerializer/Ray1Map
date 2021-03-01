using System;
using System.Collections.Generic;

namespace R1Engine
{
    public abstract class GBAVV_SpyroFusion_Manager : GBAVV_Fusion_Manager
    {
        public override LevInfo[] LevInfos => Levels;

        public static LevInfo[] Levels = new LevInfo[]
        {
            new LevInfo(4, "Dragon Castles - Part 1"),
            new LevInfo(6, "Dragon Castles - Part 2"),
            new LevInfo(1, "Gate Crank"),
            new LevInfo(68, "Castle Chaos"),
            new LevInfo(36, "Fall In, Roll Out - Easy"),
            new LevInfo(48, "Fall In, Roll Out - Normal"),
            new LevInfo(49, "Fall In, Roll Out - Hard"),
            new LevInfo(11, "Rumble on the Ramparts"),
            new LevInfo(31, "Castle Cruisin' - Easy"),
            new LevInfo(54, "Castle Cruisin' - Normal"),
            new LevInfo(55, "Castle Cruisin' - Hard"),
            new LevInfo(39, "Altitude Adjustment - Easy"),
            new LevInfo(58, "Altitude Adjustment - Normal"),
            new LevInfo(59, "Altitude Adjustment - Hard"),
            new LevInfo(7, "Portal Rush"),
            new LevInfo(17, "Gem Rush"),
            new LevInfo(92, "Crash Bandicoot"),
            new LevInfo(75, "Gem Chaser"),
            new LevInfo(79, "Dragon Drop"),
            new LevInfo(73, "Death From Behind"),

            new LevInfo(12, "Arctic Cliffs - Part 1"),
            new LevInfo(13, "Arctic Cliffs - Part 2"),
            new LevInfo(9, "Fire and Ice"),
            new LevInfo(33, "Snow Steps - Easy"),
            new LevInfo(64, "Snow Steps - Normal"),
            new LevInfo(65, "Snow Steps - Hard"),
            new LevInfo(28, "Arctic Attack - Easy"),
            new LevInfo(44, "Arctic Attack - Normal"),
            new LevInfo(45, "Arctic Attack - Hard"),
            new LevInfo(85, "Blizzard Balls"),
            new LevInfo(37, "Tread Lightly - Easy"),
            new LevInfo(50, "Tread Lightly - Normal"),
            new LevInfo(51, "Tread Lightly - Hard"),
            new LevInfo(5, "Ice Chopper"),
            new LevInfo(18, "Portal Rush"),
            new LevInfo(19, "Gem Rush"),
            new LevInfo(3, "Crush and Gulp"),
            new LevInfo(76, "Gem Chaser"),
            new LevInfo(81, "Icicle Canyon"),
            new LevInfo(70, "Sheep Shearin'"),
            new LevInfo(89, "Bridge Fight"),

            new LevInfo(0, "Fire Mountains - Part 1"),
            new LevInfo(14, "Fire Mountains - Part 2"),
            new LevInfo(10, "Wall of Fire"),
            new LevInfo(30, "Fire Fight - Easy"),
            new LevInfo(42, "Fire Fight - Normal"),
            new LevInfo(43, "Fire Fight - Hard"),
            new LevInfo(86, "Ring Of Fire"),
            new LevInfo(90, "Turn Up the Heat"),
            new LevInfo(2, "Pull Of Lava"),
            new LevInfo(40, "Hot Wings - Easy"),
            new LevInfo(60, "Hot Wings - Normal"),
            new LevInfo(61, "Hot Wings - Hard"),
            new LevInfo(20, "Portal Rush"),
            new LevInfo(21, "Gem Rush"),
            new LevInfo(27, "Nina Cortex"),
            new LevInfo(77, "Gem Chaser"),
            new LevInfo(80, "Lava Fields"),
            new LevInfo(74, "Sheep Shakedown"),

            new LevInfo(15, "Wumpa Jungle - Part 1"),
            new LevInfo(16, "Wumpa Jungle - Part 2"),
            new LevInfo(8, "Crank It Up"),
            new LevInfo(32, "Riptocs And Rockets - Easy"),
            new LevInfo(56, "Riptocs And Rockets - Normal"),
            new LevInfo(57, "Riptocs And Rockets - Hard"),
            new LevInfo(41, "Treetop Flight - Easy"),
            new LevInfo(62, "Treetop Flight - Normal"),
            new LevInfo(63, "Treetop Flight - Hard"),
            new LevInfo(35, "Falling To Pieces"),
            new LevInfo(87, "Riptoc Repellent"),
            new LevInfo(38, "Dragon Assault - Easy"),
            new LevInfo(52, "Dragon Assault - Normal"),
            new LevInfo(53, "Dragon Assault - Hard"),
            new LevInfo(22, "Portal Rush"),
            new LevInfo(23, "Gem Rush"),
            new LevInfo(72, "Neo Cortex"),
            new LevInfo(78, "Gem Chaser"),
            new LevInfo(71, "Gem Hop"),
            new LevInfo(82, "Sheep Chase"),

            new LevInfo(24, "Tech Park - Part 1"),
            new LevInfo(83, "Tech Park - Part 2"),
            new LevInfo(91, "Gravity Well"),
            new LevInfo(29, "Space Shoot - Easy"),
            new LevInfo(46, "Space Shoot - Normal"),
            new LevInfo(47, "Space Shoot - Hard"),
            new LevInfo(84, "Tech Tug"),
            new LevInfo(34, "Sky Walker - Easy"),
            new LevInfo(66, "Sky Walker - Normal"),
            new LevInfo(67, "Sky Walker - Hard"),
            new LevInfo(88, "Tech Deflect"),
            new LevInfo(25, "Portal Rush"),
            new LevInfo(26, "Gem Rush"),
            new LevInfo(69, "Space Chase"),
        };

        public override int ObjTypesCount => 282;
        public override ObjTypeInit[] ObjTypeInitInfos { get; } = new ObjTypeInit[]
        {
            new ObjTypeInit(-1, -1, null), // 0
            new ObjTypeInit(-1, -1, null), // 1
            new ObjTypeInit(19, 6, null), // 2
            new ObjTypeInit(21, 0, null), // 3
            new ObjTypeInit(19, 6, null), // 4
            new ObjTypeInit(19, 6, null), // 5
            new ObjTypeInit(10, 6, null), // 6
            new ObjTypeInit(24, 7, null), // 7
            new ObjTypeInit(8, 5, null), // 8
            new ObjTypeInit(2, 0, null), // 9
            new ObjTypeInit(10, 6, null), // 10
            new ObjTypeInit(23, 60, null), // 11
            new ObjTypeInit(23, 60, null), // 12
            new ObjTypeInit(23, 67, null), // 13
            new ObjTypeInit(23, 64, null), // 14
            new ObjTypeInit(23, 66, null), // 15
            new ObjTypeInit(23, 69, null), // 16
            new ObjTypeInit(23, 2, null), // 17
            new ObjTypeInit(23, 0, null), // 18
            new ObjTypeInit(23, 4, null), // 19
            new ObjTypeInit(23, 1, null), // 20
            new ObjTypeInit(23, 3, null), // 21
            new ObjTypeInit(23, 3, "gemJoust"), // 22
            new ObjTypeInit(23, 3, null), // 23
            new ObjTypeInit(23, 3, "gem2Joust"), // 24
            new ObjTypeInit(23, 3, "gem3Joust"), // 25
            new ObjTypeInit(23, 3, "gem4Joust"), // 26
            new ObjTypeInit(23, 74, "triggerScript"), // 27
            new ObjTypeInit(23, 79, "trigger1Script"), // 28
            new ObjTypeInit(23, 76, "trigger2Script"), // 29
            new ObjTypeInit(23, 77, "trigger3Script"), // 30
            new ObjTypeInit(23, 78, "trigger4Script"), // 31
            new ObjTypeInit(23, 75, "trigger5Script"), // 32
            new ObjTypeInit(23, 73, "triggerRightScript"), // 33
            new ObjTypeInit(23, 71, "triggerDownScript"), // 34
            new ObjTypeInit(23, 72, "triggerLeftScript"), // 35
            new ObjTypeInit(23, 70, "triggerUpScript"), // 36
            new ObjTypeInit(23, 80, "checkpoint"), // 37
            new ObjTypeInit(23, 81, "triggerJoustPlatform"), // 38
            new ObjTypeInit(23, 82, "sheepPound"), // 39
            new ObjTypeInit(23, 83, "sheepDrop"), // 40
            new ObjTypeInit(23, 84, "sheepHerder"), // 41
            new ObjTypeInit(23, 85, "triggerCandleFlame"), // 42
            new ObjTypeInit(23, 79, "joustCounter"), // 43
            new ObjTypeInit(23, 76, "spawnLabAssRear"), // 44
            new ObjTypeInit(23, 77, "spawnLabAssFront"), // 45
            new ObjTypeInit(23, 86, "ninaBossEndTrigger"), // 46
            new ObjTypeInit(23, 36, null), // 47
            new ObjTypeInit(23, 30, null), // 48
            new ObjTypeInit(23, 43, null), // 49
            new ObjTypeInit(23, 44, null), // 50
            new ObjTypeInit(23, 44, null), // 51
            new ObjTypeInit(23, 55, null), // 52
            new ObjTypeInit(23, 43, null), // 53
            new ObjTypeInit(23, 56, null), // 54
            new ObjTypeInit(23, 57, "hiddenPortalC_C"), // 55
            new ObjTypeInit(23, 57, "hiddenPortalC_J"), // 56
            new ObjTypeInit(23, 57, "hiddenPortalC_S"), // 57
            new ObjTypeInit(23, 57, "hiddenPortalI_C"), // 58
            new ObjTypeInit(23, 57, "hiddenPortalI_J"), // 59
            new ObjTypeInit(23, 57, "hiddenPortalI_S"), // 60
            new ObjTypeInit(23, 57, "hiddenPortalF_C"), // 61
            new ObjTypeInit(23, 57, "hiddenPortalF_J"), // 62
            new ObjTypeInit(23, 57, "hiddenPortalF_S"), // 63
            new ObjTypeInit(23, 57, "hiddenPortalJ_C"), // 64
            new ObjTypeInit(23, 57, "hiddenPortalJ_J"), // 65
            new ObjTypeInit(23, 57, "hiddenPortalJ_S"), // 66
            new ObjTypeInit(1, 131, "poopASpyro"), // 67
            new ObjTypeInit(1, 54, "lavaFountain"), // 68
            new ObjTypeInit(1, 1, "floorSpikesScript"), // 69
            new ObjTypeInit(1, 3, "wallSpikesScript"), // 70
            new ObjTypeInit(1, 63, "icicleScript"), // 71
            new ObjTypeInit(1, 64, "icicleFallingScript"), // 72
            new ObjTypeInit(1, 78, "venusPlant"), // 73
            new ObjTypeInit(1, 100, "swinginAxe"), // 74
            new ObjTypeInit(1, 5, "electrix"), // 75
            new ObjTypeInit(1, 4, "electrix"), // 76
            new ObjTypeInit(1, 102, "plantVines"), // 77
            new ObjTypeInit(1, 125, "lavaSpawner"), // 78
            new ObjTypeInit(1, 127, "lavaSpurt"), // 79
            new ObjTypeInit(1, 128, "moatWorm"), // 80
            new ObjTypeInit(1, 134, "landmine"), // 81
            new ObjTypeInit(1, 38, "rhynocPatrolScript"), // 82
            new ObjTypeInit(1, 6, "infernoLabAssPatrolScript"), // 83
            new ObjTypeInit(1, 30, "rhynocThrowScript"), // 84
            new ObjTypeInit(1, 32, "rhynocRockScript"), // 85
            new ObjTypeInit(1, 44, "sheepPatrol"), // 86
            new ObjTypeInit(1, 56, "rhynocFly"), // 87
            new ObjTypeInit(1, 60, "rhynocJungleFly"), // 88
            new ObjTypeInit(1, 70, "rhynocClubPatrol"), // 89
            new ObjTypeInit(1, 76, "infernoLabAssShoot"), // 90
            new ObjTypeInit(1, 77, null), // 91
            new ObjTypeInit(1, 94, "labAssPatrol"), // 92
            new ObjTypeInit(1, 96, "labAssCamo"), // 93
            new ObjTypeInit(1, 72, "rhynocIcePatrol"), // 94
            new ObjTypeInit(1, 49, "butterflyFlutter"), // 95
            new ObjTypeInit(1, 99, "coconutThrown"), // 96
            new ObjTypeInit(1, 45, "sheepFall"), // 97
            new ObjTypeInit(1, 123, "bunny"), // 98
            new ObjTypeInit(1, 130, "sheepShoot"), // 99
            new ObjTypeInit(1, 53, "sheepRun"), // 100
            new ObjTypeInit(1, 45, "sheepChaseSpawn"), // 101
            new ObjTypeInit(1, 76, "walkerLabAssFloater"), // 102
            new ObjTypeInit(1, 132, null), // 103
            new ObjTypeInit(13, 0, "genericNPC"), // 104
            new ObjTypeInit(13, 1, "genericNPC"), // 105
            new ObjTypeInit(13, 2, "genericNPC"), // 106
            new ObjTypeInit(13, 5, "blinkySnowAppear"), // 107
            new ObjTypeInit(13, 5, "blinkyDirtAppear"), // 108
            new ObjTypeInit(13, 6, "genericNPC"), // 109
            new ObjTypeInit(13, 8, "genericNPC"), // 110
            new ObjTypeInit(13, 7, "genericNPC"), // 111
            new ObjTypeInit(13, 9, "genericNPC"), // 112
            new ObjTypeInit(13, 10, "genericNPC"), // 113
            new ObjTypeInit(13, 11, "genericNPC"), // 114
            new ObjTypeInit(13, 12, "genericNPC"), // 115
            new ObjTypeInit(13, 13, "genericNPC"), // 116
            new ObjTypeInit(13, 14, "genericNPC"), // 117
            new ObjTypeInit(13, 15, "genericNPC"), // 118
            new ObjTypeInit(13, 16, "genericNPC"), // 119
            new ObjTypeInit(1, 14, "crushRunStartScript"), // 120
            new ObjTypeInit(1, 18, "gulpSpawny"), // 121
            new ObjTypeInit(1, 22, "gulpRocketScript"), // 122
            new ObjTypeInit(1, 83, "ninaSpawn"), // 123
            new ObjTypeInit(5, 0, "roofScript"), // 124
            new ObjTypeInit(1, 86, "ninaCrateL"), // 125
            new ObjTypeInit(5, 7, "ninaProfCage"), // 126
            new ObjTypeInit(5, 8, "ninaCocoCage"), // 127
            new ObjTypeInit(1, 86, "ninaCrateR"), // 128
            new ObjTypeInit(1, 106, "cortexFly"), // 129
            new ObjTypeInit(1, 108, "cortexMine"), // 130
            new ObjTypeInit(1, 114, null), // 131
            new ObjTypeInit(5, 1, "boilerScript"), // 132
            new ObjTypeInit(1, 26, "gulpScorch"), // 133
            new ObjTypeInit(23, 74, "crushCounter"), // 134
            new ObjTypeInit(3, 2, null), // 135
            new ObjTypeInit(3, 0, null), // 136
            new ObjTypeInit(3, 1, null), // 137
            new ObjTypeInit(3, 3, null), // 138
            new ObjTypeInit(3, 4, null), // 139
            new ObjTypeInit(3, 5, null), // 140
            new ObjTypeInit(3, 6, null), // 141
            new ObjTypeInit(3, 15, null), // 142
            new ObjTypeInit(3, 14, null), // 143
            new ObjTypeInit(3, 16, null), // 144
            new ObjTypeInit(3, 17, null), // 145
            new ObjTypeInit(3, 18, null), // 146
            new ObjTypeInit(3, 19, null), // 147
            new ObjTypeInit(3, 20, null), // 148
            new ObjTypeInit(3, 21, null), // 149
            new ObjTypeInit(3, 26, null), // 150
            new ObjTypeInit(3, 23, null), // 151
            new ObjTypeInit(3, 24, null), // 152
            new ObjTypeInit(3, 25, null), // 153
            new ObjTypeInit(3, 22, null), // 154
            new ObjTypeInit(3, 28, null), // 155
            new ObjTypeInit(3, 27, null), // 156
            new ObjTypeInit(3, 30, "triggerIceTorch"), // 157
            new ObjTypeInit(3, 29, null), // 158
            new ObjTypeInit(3, 31, null), // 159
            new ObjTypeInit(3, 32, null), // 160
            new ObjTypeInit(3, 33, null), // 161
            new ObjTypeInit(3, 7, null), // 162
            new ObjTypeInit(3, 8, null), // 163
            new ObjTypeInit(3, 9, null), // 164
            new ObjTypeInit(3, 10, null), // 165
            new ObjTypeInit(3, 11, "triggerTorchSmall"), // 166
            new ObjTypeInit(3, 12, "triggerTorchLarge"), // 167
            new ObjTypeInit(13, 16, null), // 168
            new ObjTypeInit(11, 10, null), // 169
            new ObjTypeInit(11, 44, null), // 170
            new ObjTypeInit(11, 45, null), // 171
            new ObjTypeInit(11, 47, null), // 172
            new ObjTypeInit(11, 50, null), // 173
            new ObjTypeInit(11, 57, null), // 174
            new ObjTypeInit(11, 56, null), // 175
            new ObjTypeInit(11, 63, null), // 176
            new ObjTypeInit(11, 38, null), // 177
            new ObjTypeInit(11, 24, null), // 178
            new ObjTypeInit(11, 32, null), // 179
            new ObjTypeInit(11, 21, null), // 180
            new ObjTypeInit(11, 23, null), // 181
            new ObjTypeInit(11, 1, null), // 182
            new ObjTypeInit(11, 2, null), // 183
            new ObjTypeInit(11, 12, null), // 184
            new ObjTypeInit(11, 5, null), // 185
            new ObjTypeInit(11, 20, null), // 186
            new ObjTypeInit(11, 10, null), // 187
            new ObjTypeInit(23, 12, "platformUpScript"), // 188
            new ObjTypeInit(23, 13, "platformRightScript"), // 189
            new ObjTypeInit(23, 14, "platformRightScript"), // 190
            new ObjTypeInit(23, 18, "platformUpScript"), // 191
            new ObjTypeInit(23, 15, "platformRightScript"), // 192
            new ObjTypeInit(23, 19, "platformUpScript"), // 193
            new ObjTypeInit(23, 16, "platformUpScript"), // 194
            new ObjTypeInit(23, 17, "platformRightScript"), // 195
            new ObjTypeInit(23, 20, "platformIdleScript"), // 196
            new ObjTypeInit(23, 16, "joustPlatformL"), // 197
            new ObjTypeInit(23, 23, "bouncyBranch"), // 198
            new ObjTypeInit(23, 24, "platformRightScript"), // 199
            new ObjTypeInit(23, 25, "platformUpScript"), // 200
            new ObjTypeInit(23, 16, "joustPlatformR"), // 201
            new ObjTypeInit(23, 16, "joustPlatform2L"), // 202
            new ObjTypeInit(23, 16, "joustPlatform2R"), // 203
            new ObjTypeInit(23, 17, "joustPlatform3L"), // 204
            new ObjTypeInit(23, 17, "joustPlatform3R"), // 205
            new ObjTypeInit(23, 17, "joustPlatform4L"), // 206
            new ObjTypeInit(23, 17, "joustPlatform4R"), // 207
            new ObjTypeInit(20, 6, null), // 208
            new ObjTypeInit(7, 7, null), // 209
            new ObjTypeInit(-1, -1, null), // 210
            new ObjTypeInit(14, 4, null), // 211
            new ObjTypeInit(14, 5, null), // 212
            new ObjTypeInit(14, 6, null), // 213
            new ObjTypeInit(14, 7, null), // 214
            new ObjTypeInit(14, 8, null), // 215
            new ObjTypeInit(14, 9, null), // 216
            new ObjTypeInit(14, 10, null), // 217
            new ObjTypeInit(14, 11, null), // 218
            new ObjTypeInit(14, 12, null), // 219
            new ObjTypeInit(14, 13, null), // 220
            new ObjTypeInit(14, 14, null), // 221
            new ObjTypeInit(14, 15, null), // 222
            new ObjTypeInit(14, 16, null), // 223
            new ObjTypeInit(14, 17, null), // 224
            new ObjTypeInit(14, 18, null), // 225
            new ObjTypeInit(14, 19, null), // 226
            new ObjTypeInit(14, 28, null), // 227
            new ObjTypeInit(14, 22, null), // 228
            new ObjTypeInit(14, 23, null), // 229
            new ObjTypeInit(14, 24, null), // 230
            new ObjTypeInit(14, 25, null), // 231
            new ObjTypeInit(14, 26, null), // 232
            new ObjTypeInit(14, 27, null), // 233
            new ObjTypeInit(14, 21, null), // 234
            new ObjTypeInit(14, 30, null), // 235
            new ObjTypeInit(14, 31, null), // 236
            new ObjTypeInit(14, 32, null), // 237
            new ObjTypeInit(14, 33, null), // 238
            new ObjTypeInit(14, 34, null), // 239
            new ObjTypeInit(14, 35, null), // 240
            new ObjTypeInit(14, 36, null), // 241
            new ObjTypeInit(14, 29, null), // 242
            new ObjTypeInit(14, 44, null), // 243
            new ObjTypeInit(14, 38, null), // 244
            new ObjTypeInit(14, 39, null), // 245
            new ObjTypeInit(14, 40, null), // 246
            new ObjTypeInit(14, 41, null), // 247
            new ObjTypeInit(14, 42, null), // 248
            new ObjTypeInit(14, 43, null), // 249
            new ObjTypeInit(14, 37, null), // 250
            new ObjTypeInit(21, 16, null), // 251
            new ObjTypeInit(21, 15, null), // 252
            new ObjTypeInit(21, 17, null), // 253
            new ObjTypeInit(21, 14, null), // 254
            new ObjTypeInit(21, 24, null), // 255
            new ObjTypeInit(21, 28, null), // 256
            new ObjTypeInit(21, 31, null), // 257
            new ObjTypeInit(21, 35, null), // 258
            new ObjTypeInit(21, 38, null), // 259
            new ObjTypeInit(21, 41, null), // 260
            new ObjTypeInit(21, 24, null), // 261
            new ObjTypeInit(21, 44, "sheepHerd"), // 262
            new ObjTypeInit(21, 46, "topDownPortal"), // 263
            new ObjTypeInit(21, 47, "topDownPortal"), // 264
            new ObjTypeInit(21, 41, null), // 265
            new ObjTypeInit(21, 52, "chaseWall"), // 266
            new ObjTypeInit(21, 46, "topDownPortalFail"), // 267
            new ObjTypeInit(4, 20, null), // 268
            new ObjTypeInit(4, 29, "breakoutRhynocScript"), // 269
            new ObjTypeInit(4, 37, null), // 270
            new ObjTypeInit(4, 26, "breakoutLabAssShooterScript"), // 271
            new ObjTypeInit(4, 40, "breakoutRhynocShieldScript"), // 272
            new ObjTypeInit(4, 35, "breakoutLabAssProjectileScript"), // 273
            new ObjTypeInit(4, 29, "breakoutLabAssScript"), // 274
            new ObjTypeInit(4, 43, "breakoutRhynocBallScript"), // 275
            new ObjTypeInit(-1, -1, null), // 276 // Appears in most levels - Sparx?
            new ObjTypeInit(4, 29, "globalController"), // 277
            new ObjTypeInit(6, 9, null), // 278
            new ObjTypeInit(15, 7, null), // 279
            new ObjTypeInit(1, 137, "bouncySheep"), // 280
            new ObjTypeInit(24, 26, null), // 281
        };

        public override Dictionary<int, GBAVV_ScriptCommand.CommandType> ScriptCommands { get; } = new Dictionary<int, GBAVV_ScriptCommand.CommandType>()
        {
            [0501] = GBAVV_ScriptCommand.CommandType.Name,
            [0502] = GBAVV_ScriptCommand.CommandType.Script,
            [0503] = GBAVV_ScriptCommand.CommandType.SkipNextIfInputCheck,
            [0504] = GBAVV_ScriptCommand.CommandType.SkipNextIfField08,
            [0505] = GBAVV_ScriptCommand.CommandType.Reset,
            [0506] = GBAVV_ScriptCommand.CommandType.Return,
            [0507] = GBAVV_ScriptCommand.CommandType.SetUnknownInputData,
            [0508] = GBAVV_ScriptCommand.CommandType.Wait,
            // 0509 is a duplicate of 0510, but never used
            [0510] = GBAVV_ScriptCommand.CommandType.WaitWhileInputCheck,

            [0702] = GBAVV_ScriptCommand.CommandType.Dialog,
            
            [0800] = GBAVV_ScriptCommand.CommandType.IsFlipped,
            [0807] = GBAVV_ScriptCommand.CommandType.Animation,
            [0812] = GBAVV_ScriptCommand.CommandType.IsEnabled,
            [0813] = GBAVV_ScriptCommand.CommandType.ConditionalScript,
            [0821] = GBAVV_ScriptCommand.CommandType.Movement_X,
            [0822] = GBAVV_ScriptCommand.CommandType.Movement_Y,
            [0844] = GBAVV_ScriptCommand.CommandType.SpawnObject,
            //[0863] = GBAVV_ScriptCommand.CommandType.SecondaryAnimation, // TODO: Find
            //[0871] = GBAVV_ScriptCommand.CommandType.PlaySound, // TODO: Find

            [1000] = GBAVV_ScriptCommand.CommandType.DialogPortrait

        };
        public override int DialogScriptsCount => 70;
    }

    public class GBAVV_SpyroFusionEU_Manager : GBAVV_SpyroFusion_Manager
    {
        public override uint ObjTypesPointer => throw new NotImplementedException();

        public override uint[] AnimSetPointers => new uint[]
        {
            0x08278288,
            0x0827D254,
            0x0829BB54,
            0x0829D434,
            0x0829F894,
            0x082A4478,
            0x082A770C,
            0x082B04F4,
            0x082B2308,
            0x082B3468,
            0x082B3F00,
            0x082B7630,
            0x082D0AF0,
            0x082D13E0,
            0x082D52E0,
            0x082DF454,
            0x082E07E8,
            0x082E405C,
            0x082E6508,
            0x082E749C,
            0x082EFCF0,
            0x082F1C9C,
            0x082F60F8,
            0x082FBAF4,
            0x08305550
        };

        public override uint[] ScriptPointers => new uint[]
        {
            0x0805E464, // genericNPC
            0x0805E4CC, // talkToVictim
            0x0805E764, // script_waitForInputOrTime
            0x080606A8, // notFound
            0x08060950, // waitForPagedText
            0x080609B8, // missing
            0x08060A00, // Sparx01H01
            0x08060A54, // Sparx01H02
            0x08060AA8, // Sparx01H03
            0x08060AFC, // Blinky01A01
            0x08060B74, // Bianca01A01
            0x08060BEC, // Bianca01A03
            0x08060C64, // Blinky01A03
            0x08060CB8, // Hunter01B01
            0x08060D54, // Blinky01B04
            0x08060DA8, // Blinky01B05
            0x08060E20, // Blinky01B07
            0x08060E78, // Moneybags01A01
            0x08060EF4, // Moneybags01B01
            0x08060F48, // Boss0101
            0x08060F9C, // Sparx02H01
            0x08061018, // Professor02A01
            0x080610D8, // Coco02A01
            0x08061174, // Hunter02A01
            0x080611C8, // Byrd02A01
            0x08061240, // Crash02B03
            0x08061298, // Moneybags02A01
            0x08061314, // Moneybags02B01
            0x08061368, // Boss0201
            0x080613BC, // Sparx03H01
            0x08061434, // Crunch03A01
            0x08061488, // Blinky03B04
            0x080614DC, // Crash03B01
            0x08061534, // Moneybags03A01
            0x0806158C, // Moneybags03B01
            0x080615E0, // Boss0301
            0x0806167C, // Boss0304
            0x080616D0, // Boss0305
            0x08061724, // Boss0306
            0x08061778, // Hunter04A01
            0x080617CC, // Crash04A01
            0x08061820, // Agent04B04
            0x08061878, // Moneybags04A01
            0x080618D0, // Moneybags04B01
            0x08061924, // Boss0401
            0x080619C4, // Professor05A01
            0x08061A18, // Crash05A01
            0x08061A70, // Moneybags05A01
            0x08061AEC, // Moneybags05B01
            0x08061B40, // Boss0501
            0x08061B94, // Boss0502
            0x08061C0C, // Boss0504
            0x08061C84, // Boss0506
            0x08061D30, // instructionsRaiseTheGate
            0x08061DB4, // instructionsIceWallMelt
            0x08061E40, // instructionsFireWallExtinguish
            0x08061EC8, // instructionsRockVinePull
            0x08061F4C, // instructionsTugOfWar
            0x08061FD0, // instructionsIceChopper
            0x08062078, // instructionsAirAttack
            0x08062120, // instructionsSpyroGlide
            0x080621CC, // instructionsDragonAssault
            0x08062274, // instructionsDeflection
            0x08062360, // instructionsJeep
            0x0806244C, // instructionsWalker
            0x08062518, // instructionsBridgeFight
            0x08062604, // instructionsGemRush
            0x08062664, // instructionsPortalRush
            0x080626C0, // instructionsChase
            0x08062764, // instructionsJoust
            0x080627E8, // instructionsPlatforms
            0x0806286C, // instructionsSheepHerd
            0x08062914, // instructionsSheepBounce
            0x080629BC, // instructionsSheepShake
            0x08062A64, // instructionsSheepJeep
            0x080EE4E8, // movie_license
            0x080EE598, // movie_credits
            0x080EE60C, // movie_secretCredits
            0x080EE704, // "titleText"
            0x080EEED0, // waitForPagedText
            0x080EF03C, // takeCrateHitR
            0x080EF0C4, // ninaBossEndTrigger
            0x080EF14C, // boilerScript
            0x080EF1A8, // roofScript
            0x080EF220, // genericCrateHit
            0x080EF32C, // leftish
            0x080EF428, // rightish
            0x080EF518, // runningBackAndForth
            0x080EF638, // gotoNextRound
            0x080EF728, // mainLoop
            0x080EF818, // ninaSpawn
            0x080EF890, // ninaCrateL
            0x080EF8E4, // ninaCrateR
            0x080EF950, // ninaProfCage
            0x080EFA20, // ninaCocoCage
            0x080EFAF4, // boilerOpen
            0x080EFB38, // moveUp
            0x080EFB88, // ninaRunLeft
            0x080EFC58, // ninaRunRight
            0x080EFD40, // genericCrateCode
            0x080EFE54, // raiseRoof
            0x080F3344, // spawnLabAssRear
            0x080F33E8, // spawnLabAssFront
            0x080F3484, // takeHornDive
            0x080F356C, // takeFlame
            0x080F36B0, // attack
            0x080F37A0, // idle
            0x080F3890, // venusPlant
            0x080F394C, // turninright
            0x080F3A54, // turninleft
            0x080F3B44, // "patrol"
            0x080F3C34, // rhynocTurnScript
            0x080F3CC0, // rhynocPatrolScript
            0x080F3D94, // rhynocIcePatrol
            0x080F3E50, // turninright
            0x080F3F40, // turninleft
            0x080F4030, // rhynocIcePatrolTurn
            0x080F40F0, // rhynocClubPatrol
            0x080F41C8, // whamright
            0x080F42C4, // whamleft
            0x080F43B4, // rhynocClubTurn
            0x080F440C, // rhynocClubWham
            0x080F44B4, // rhynocThrowScript
            0x080F4600, // "WaitingForKill"
            0x080F46F0, // rhynocRockScript
            0x080F47B4, // rhynocFly
            0x080F4848, // rhynocHoverUp
            0x080F48D0, // rhynocHoverDown
            0x080F4968, // rhynocJungleFly
            0x080F4A00, // rhynocJungleHoverUp
            0x080F4A90, // rhynocJungleHoverDown
            0x080F4B30, // turninright
            0x080F4C20, // turninleft
            0x080F4D10, // infernoLabAssTurnScript
            0x080F4E7C, // infernoLabAssPatrolScript
            0x080F4FA0, // firin
            0x080F5090, // idlin
            0x080F5180, // infernoLabAssShoot
            0x080F520C, // labAssPatrol
            0x080F52C8, // walkin
            0x080F53B8, // walkin
            0x080F54A8, // labAssTurn
            0x080F5570, // throwin
            0x080F5660, // labAssCamo
            0x080F5714, // flying
            0x080F5804, // coconutThrown
            0x080F58C8, // coconutExplode
            0x080F5948, // walkerLabAssFloater
            0x080F5A94, // gulpSpawny
            0x080F5AF4, // gulpBarrageScript
            0x080F5BD0, // gulpFireScript
            0x080F5C7C, // gulpDead
            0x080F5D78, // gulpHit
            0x080F5E98, // "WaitingForBoom"
            0x080F5F94, // gulpRocketScript
            0x080F60E4, // "WaitingForBoom"
            0x080F61E0, // gulpRocketRepel
            0x080F62B4, // gulpScorch
            0x080F62F4, // gulpScorched
            0x080F6348, // gulpF1Script
            0x080F63C0, // gulpF2Script
            0x080F6444, // gulpF3Script
            0x080F64BC, // gulpF4Script
            0x080F6534, // gulpF5Script
            0x080F65A4, // gulpFUpScript
            0x080F6628, // crushCounter
            0x080F6694, // counting
            0x080F6784, // crushCounting
            0x080F67DC, // crushRunStartScript
            0x080F6864, // headinleft
            0x080F6974, // headinright
            0x080F6A64, // crushRunScript
            0x080F6AF4, // crushStop
            0x080F6B94, // crushStun
            0x080F6C44, // weregood
            0x080F6D34, // crushGetUp
            0x080F6D88, // headinleft
            0x080F6E78, // headinright
            0x080F6F84, // goincrazy
            0x080F7074, // crushRocketed
            0x080F7148, // crushTurn
            0x080F7208, // turninright
            0x080F7310, // turninleft
            0x080F7400, // crushCharge
            0x080F748C, // crushUpAndAtThem
            0x080F7558, // crushFlame
            0x080F75EC, // crushStayPut
            0x080F7664, // cortexFly
            0x080F770C, // cortexFlipUp
            0x080F77D0, // cortexFlipDown
            0x080F788C, // cortexFlipDownHurt
            0x080F7958, // cortexDropMine
            0x080F7A14, // droppin
            0x080F7B04, // cortexMine
            0x080F7BD8, // cortexUberBlast
            0x080F7C50, // cortexOw
            0x080F7D18, // cortexLaughAtPunyMortal
            0x080F7DDC, // cortexReload
            0x080F7E48, // cortexDefeat
            0x080F7EDC, // cortexEscape
            0x080F7F90, // weregood
            0x080F8080, // crushUpCharge
            0x080F80D8, // lavaFountain
            0x080F8154, // floorSpikesScript
            0x080F81E0, // wallSpikesScript
            0x080F8278, // icicleScript
            0x080F831C, // "fall"
            0x080F8418, // icicleFallingScript
            0x080F84C8, // "sfx"
            0x080F85B8, // swinginAxe
            0x080F8620, // electrix
            0x080F8680, // plantVines
            0x080F8708, // lavaSpawner
            0x080F8768, // lavaSpurt
            0x080F885C, // "attackL"
            0x080F8958, // "attackR"
            0x080F8A48, // moatWorm
            0x080F8AC8, // landmine
            0x080F8B20, // landmineBoom
            0x080F8B64, // chaseWall
            0x080F8BB8, // chaseWallHit1
            0x080F8C0C, // chaseWallHit2
            0x080F8C64, // chaseWallDead
            0x080F8D60, // flipping
            0x080F8EA4, // platformer
            0x080F8F94, // triggerJoustPlatform
            0x080F90A8, // spawnJoustPlatformL
            0x080F91B0, // spawnJoustPlatformR
            0x080F929C, // turnin
            0x080F938C, // turnin
            0x080F947C, // triggerJoustPlatformTurn
            0x080F94EC, // platformCount
            0x080F9538, // joustPlatformL
            0x080F95B8, // joustPlatformR
            0x080F9644, // joustPlatform2L
            0x080F96D0, // joustPlatform2R
            0x080F975C, // joustPlatform3L
            0x080F97E8, // joustPlatform3R
            0x080F9874, // joustPlatform4L
            0x080F9900, // joustPlatform4R
            0x080F99D0, // gemJoust
            0x080F9AC0, // gem2Joust
            0x080F9BB0, // gem3Joust
            0x080F9CA0, // gem4Joust
            0x080F9DA4, // joustPlatformFall
            0x080F9DF8, // joustCounter
            0x080F9E54, // joustCount
            0x080F9F1C, // triggerScript
            0x080F9F5C, // triggerUpScript
            0x080F9FA0, // triggerDownScript
            0x080F9FE4, // triggerLeftScript
            0x080FA028, // triggerRightScript
            0x080FA068, // trigger1Script
            0x080FA0A8, // trigger2Script
            0x080FA0E8, // trigger3Script
            0x080FA128, // trigger4Script
            0x080FA168, // trigger5Script
            0x080FA1B8, // checkpoint
            0x080FA204, // topDownPortal
            0x080FA248, // topDownPortalFail
            0x080FA288, // triggerCandleFlame
            0x080FA2EC, // lightCandle
            0x080FA368, // triggerTorchSmall
            0x080FA3D0, // lightTorchSmall
            0x080FA44C, // triggerTorchLarge
            0x080FA4A0, // lightTorchLarge
            0x080FA518, // triggerIceTorch
            0x080FA56C, // lightIceTorch
            0x080FA5E0, // NPCScript
            0x080FA614, // NPCTalkScript
            0x080FA694, // blinkyDirtAppear
            0x080FA758, // blinkySnowAppear
            0x080FA818, // spawnPortal
            0x080FA908, // hiddenPortalC_C
            0x080FA98C, // spawnPortal
            0x080FAA7C, // hiddenPortalC_J
            0x080FAB00, // spawnPortal
            0x080FABF0, // hiddenPortalC_S
            0x080FAC74, // hiddenPortalI_C
            0x080FACE4, // hiddenPortalI_J
            0x080FAD54, // hiddenPortalI_S
            0x080FADD8, // hiddenPortalF_C
            0x080FAE48, // hiddenPortalF_J
            0x080FAEB8, // hiddenPortalF_S
            0x080FAF3C, // hiddenPortalJ_C
            0x080FAFAC, // hiddenPortalJ_J
            0x080FB01C, // hiddenPortalJ_S
            0x080FB0A8, // hiddenPortalSpawnC
            0x080FB11C, // hiddenPortalSpawnJ
            0x080FB190, // hiddenPortalSpawnS
            0x080FB1E4, // poopASpyro
            0x080FB3B8, // bouncyBranchBounce
            0x080FB41C, // idle
            0x080FB50C, // bouncyBranch
            0x080FB57C, // platformUpScript
            0x080FB610, // platformDownScript
            0x080FB680, // platformLeftScript
            0x080FB6FC, // platformRightScript
            0x080FB76C, // platformIdleScript
            0x080FB7B4, // platformCrumbleScript
            0x080FB8EC, // checkspawn
            0x080FB9F0, // sheepHornDive
            0x080FBAB8, // sheepPound
            0x080FBAF8, // sheepPounded
            0x080FBB48, // sheepDrop
            0x080FBBBC, // droppin
            0x080FBCAC, // sheepDropped
            0x080FBD18, // sheepFall
            0x080FBE08, // movin
            0x080FBF0C, // movin
            0x080FC028, // "eating"
            0x080FC118, // "hungry"
            0x080FC208, // sheepPatrol
            0x080FC2EC, // turnRight
            0x080FC3E8, // turnLeft
            0x080FC4F8, // "startle"
            0x080FC5E8, // sheepTurn
            0x080FC724, // sheepDie
            0x080FC878, // butterflyFlutter
            0x080FC910, // sniff
            0x080FCA18, // bunny
            0x080FCB44, // bunnyDie
            0x080FCC48, // bouncySheep
            0x080FCD0C, // herding
            0x080FCDFC, // sheepHerder
            0x080FCEC0, // again
            0x080FCFD8, // sheepHerd
            0x080FD078, // sheepHerdKill
            0x080FD0B4, // sheepHerded
            0x080FD160, // sheepChaseSpawn
            0x080FD274, // sheepRun
            0x080FD2EC, // sheepJump
            0x080FD35C, // sheepJumpBig
            0x080FD3CC, // sheepJumpDown
            0x080FD474, // bouncySheepBounce
            0x080FD544, // lavaFountainScript
            0x080FD628, // sheepShoot
            0x080FD6DC, // childKilled
            0x080FD7A0, // breakoutLabAssProjectileScript
            0x080FD860, // "InnerLoop"
            0x080FD950, // globalController
            0x080FD9D4, // breakoutLabAssShooterScript
            0x080FDA64, // breakoutLabAssScript
            0x080FDAF8, // breakoutRhynocShieldScript
            0x080FDBA0, // breakoutRhynocBallScript
            0x080FDC30, // breakoutRhynocScript
            0x080FDCC8, // breakoutWallOnScript
            0x080FDD1C, // stepLeft
            0x080FDD64, // stepRight
            0x080FDDAC, // stepDown
            0x080FDDE4, // newWave
            0x080FDE28, // genericDeathScript
            0x080FDEF8, // DieAndDropBallScript
            0x080FDF40, // genericDrop
            0x080FDFA0, // genericTurnRightAndDrop
            0x080FDFE8, // genericTurnLeftAndDrop
            0x080FE030, // breakoutWallOffScript
            0x080FE090, // genericTurnLeftScript
            0x080FE0E4, // genericTurnRightScript
        };

        public override int LanguagesCount => 5;
    }

    public class GBAVV_SpyroFusionUS_Manager : GBAVV_SpyroFusion_Manager
    {
        public override uint ObjTypesPointer => 0x0800e468;

        public override uint[] AnimSetPointers => new uint[]
        {
            0x082627EC,
            0x082677B8,
            0x082860B8,
            0x08287998,
            0x08289DF8,
            0x0828E9DC,
            0x08291C70,
            0x0829AA58,
            0x0829C86C,
            0x0829D9CC,
            0x0829E464,
            0x082A1B94,
            0x082BB054,
            0x082BB944,
            0x082BF844,
            0x082C99D8,
            0x082CAD6C,
            0x082CE5E0,
            0x082D0AAC,
            0x082D1A40,
            0x082DA294,
            0x082DC548,
            0x082E09A4,
            0x082E63A0,
            0x082EFDFC,
        };

        public override uint[] ScriptPointers => new uint[]
        {
            0x0805E1D4, // genericNPC
            0x0805E23C, // talkToVictim
            0x0805E4D4, // script_waitForInputOrTime
            0x08060418, // notFound
            0x080606C0, // waitForPagedText
            0x08060728, // missing
            0x08060770, // Sparx01H01
            0x080607C4, // Sparx01H02
            0x08060818, // Sparx01H03
            0x0806086C, // Blinky01A01
            0x080608E4, // Bianca01A01
            0x0806095C, // Bianca01A03
            0x080609D4, // Blinky01A03
            0x08060A28, // Hunter01B01
            0x08060AC4, // Blinky01B04
            0x08060B18, // Blinky01B05
            0x08060B90, // Blinky01B07
            0x08060BE8, // Moneybags01A01
            0x08060C64, // Moneybags01B01
            0x08060CB8, // Boss0101
            0x08060D0C, // Sparx02H01
            0x08060D88, // Professor02A01
            0x08060E48, // Coco02A01
            0x08060EE4, // Hunter02A01
            0x08060F38, // Byrd02A01
            0x08060FB0, // Crash02B03
            0x08061008, // Moneybags02A01
            0x08061084, // Moneybags02B01
            0x080610D8, // Boss0201
            0x0806112C, // Sparx03H01
            0x080611A4, // Crunch03A01
            0x080611F8, // Blinky03B04
            0x0806124C, // Crash03B01
            0x080612A4, // Moneybags03A01
            0x080612FC, // Moneybags03B01
            0x08061350, // Boss0301
            0x080613EC, // Boss0304
            0x08061440, // Boss0305
            0x08061494, // Boss0306
            0x080614E8, // Hunter04A01
            0x0806153C, // Crash04A01
            0x08061590, // Agent04B04
            0x080615E8, // Moneybags04A01
            0x08061640, // Moneybags04B01
            0x08061694, // Boss0401
            0x08061734, // Professor05A01
            0x08061788, // Crash05A01
            0x080617E0, // Moneybags05A01
            0x0806185C, // Moneybags05B01
            0x080618B0, // Boss0501
            0x08061904, // Boss0502
            0x0806197C, // Boss0504
            0x080619F4, // Boss0506
            0x08061AA0, // instructionsRaiseTheGate
            0x08061B24, // instructionsIceWallMelt
            0x08061BB0, // instructionsFireWallExtinguish
            0x08061C38, // instructionsRockVinePull
            0x08061CBC, // instructionsTugOfWar
            0x08061D40, // instructionsIceChopper
            0x08061DE8, // instructionsAirAttack
            0x08061E90, // instructionsSpyroGlide
            0x08061F3C, // instructionsDragonAssault
            0x08061FE4, // instructionsDeflection
            0x080620D0, // instructionsJeep
            0x080621BC, // instructionsWalker
            0x08062288, // instructionsBridgeFight
            0x08062374, // instructionsGemRush
            0x080623D4, // instructionsPortalRush
            0x08062430, // instructionsChase
            0x080624D4, // instructionsJoust
            0x08062558, // instructionsPlatforms
            0x080625DC, // instructionsSheepHerd
            0x08062684, // instructionsSheepBounce
            0x0806272C, // instructionsSheepShake
            0x080627D4, // instructionsSheepJeep
            0x080D8A30, // movie_license
            0x080D8AE0, // movie_credits
            0x080D8B54, // movie_secretCredits
            0x080D8C4C, // "titleText"
            0x080D9434, // waitForPagedText
            0x080D9528, // takeCrateHitL
            0x080D95A0, // takeCrateHitR
            0x080D9628, // ninaBossEndTrigger
            0x080D96B0, // boilerScript
            0x080D970C, // roofScript
            0x080D9784, // genericCrateHit
            0x080D9890, // leftish
            0x080D998C, // rightish
            0x080D9A7C, // runningBackAndForth
            0x080D9B9C, // gotoNextRound
            0x080D9C8C, // mainLoop
            0x080D9D7C, // ninaSpawn
            0x080D9DF4, // ninaCrateL
            0x080D9E48, // ninaCrateR
            0x080D9EB4, // ninaProfCage
            0x080D9F84, // ninaCocoCage
            0x080DA058, // boilerOpen
            0x080DA09C, // moveUp
            0x080DA0EC, // ninaRunLeft
            0x080DA1BC, // ninaRunRight
            0x080DA2A4, // genericCrateCode
            0x080DA3B8, // raiseRoof
            0x080DD8A8, // spawnLabAssRear
            0x080DD94C, // spawnLabAssFront
            0x080DD9E8, // takeHornDive
            0x080DDAD0, // takeFlame
            0x080DDC14, // attack
            0x080DDD04, // idle
            0x080DDDF4, // venusPlant
            0x080DDEB0, // turninright
            0x080DDFB8, // turninleft
            0x080DE0A8, // "patrol"
            0x080DE198, // rhynocTurnScript
            0x080DE224, // rhynocPatrolScript
            0x080DE2F8, // rhynocIcePatrol
            0x080DE3B4, // turninright
            0x080DE4A4, // turninleft
            0x080DE594, // rhynocIcePatrolTurn
            0x080DE654, // rhynocClubPatrol
            0x080DE72C, // whamright
            0x080DE828, // whamleft
            0x080DE918, // rhynocClubTurn
            0x080DE970, // rhynocClubWham
            0x080DEA18, // rhynocThrowScript
            0x080DEB64, // "WaitingForKill"
            0x080DEC54, // rhynocRockScript
            0x080DED18, // rhynocFly
            0x080DEDAC, // rhynocHoverUp
            0x080DEE34, // rhynocHoverDown
            0x080DEECC, // rhynocJungleFly
            0x080DEF64, // rhynocJungleHoverUp
            0x080DEFF4, // rhynocJungleHoverDown
            0x080DF094, // turninright
            0x080DF184, // turninleft
            0x080DF274, // infernoLabAssTurnScript
            0x080DF3E0, // infernoLabAssPatrolScript
            0x080DF504, // firin
            0x080DF5F4, // idlin
            0x080DF6E4, // infernoLabAssShoot
            0x080DF770, // labAssPatrol
            0x080DF82C, // walkin
            0x080DF91C, // walkin
            0x080DFA0C, // labAssTurn
            0x080DFAD4, // throwin
            0x080DFBC4, // labAssCamo
            0x080DFC78, // flying
            0x080DFD68, // coconutThrown
            0x080DFE2C, // coconutExplode
            0x080DFEAC, // walkerLabAssFloater
            0x080DFFF8, // gulpSpawny
            0x080E0058, // gulpBarrageScript
            0x080E0134, // gulpFireScript
            0x080E01E0, // gulpDead
            0x080E02DC, // gulpHit
            0x080E03FC, // "WaitingForBoom"
            0x080E04F8, // gulpRocketScript
            0x080E0648, // "WaitingForBoom"
            0x080E0744, // gulpRocketRepel
            0x080E0818, // gulpScorch
            0x080E0858, // gulpScorched
            0x080E08AC, // gulpF1Script
            0x080E0924, // gulpF2Script
            0x080E09A8, // gulpF3Script
            0x080E0A20, // gulpF4Script
            0x080E0A98, // gulpF5Script
            0x080E0B08, // gulpFUpScript
            0x080E0B8C, // crushCounter
            0x080E0BF8, // counting
            0x080E0CE8, // crushCounting
            0x080E0D40, // crushRunStartScript
            0x080E0DC8, // headinleft
            0x080E0ED8, // headinright
            0x080E0FC8, // crushRunScript
            0x080E1058, // crushStop
            0x080E10F8, // crushStun
            0x080E11A8, // weregood
            0x080E1298, // crushGetUp
            0x080E12EC, // headinleft
            0x080E13DC, // headinright
            0x080E14E8, // goincrazy
            0x080E15D8, // crushRocketed
            0x080E16AC, // crushTurn
            0x080E176C, // turninright
            0x080E1874, // turninleft
            0x080E1964, // crushCharge
            0x080E19F0, // crushUpAndAtThem
            0x080E1ABC, // crushFlame
            0x080E1B50, // crushStayPut
            0x080E1BC8, // cortexFly
            0x080E1C70, // cortexFlipUp
            0x080E1D34, // cortexFlipDown
            0x080E1DF0, // cortexFlipDownHurt
            0x080E1EBC, // cortexDropMine
            0x080E1F78, // droppin
            0x080E2068, // cortexMine
            0x080E213C, // cortexUberBlast
            0x080E21B4, // cortexOw
            0x080E227C, // cortexLaughAtPunyMortal
            0x080E2340, // cortexReload
            0x080E23AC, // cortexDefeat
            0x080E2440, // cortexEscape
            0x080E24F4, // weregood
            0x080E25E4, // crushUpCharge
            0x080E263C, // lavaFountain
            0x080E26B8, // floorSpikesScript
            0x080E2744, // wallSpikesScript
            0x080E27DC, // icicleScript
            0x080E2880, // "fall"
            0x080E297C, // icicleFallingScript
            0x080E2A2C, // "sfx"
            0x080E2B1C, // swinginAxe
            0x080E2B84, // electrix
            0x080E2BE4, // plantVines
            0x080E2C6C, // lavaSpawner
            0x080E2CCC, // lavaSpurt
            0x080E2DC0, // "attackL"
            0x080E2EBC, // "attackR"
            0x080E2FAC, // moatWorm
            0x080E302C, // landmine
            0x080E3084, // landmineBoom
            0x080E30C8, // chaseWall
            0x080E311C, // chaseWallHit1
            0x080E3170, // chaseWallHit2
            0x080E31C8, // chaseWallDead
            0x080E32C4, // flipping
            0x080E3408, // platformer
            0x080E34F8, // triggerJoustPlatform
            0x080E360C, // spawnJoustPlatformL
            0x080E3714, // spawnJoustPlatformR
            0x080E3800, // turnin
            0x080E38F0, // turnin
            0x080E39E0, // triggerJoustPlatformTurn
            0x080E3A50, // platformCount
            0x080E3A9C, // joustPlatformL
            0x080E3B1C, // joustPlatformR
            0x080E3BA8, // joustPlatform2L
            0x080E3C34, // joustPlatform2R
            0x080E3CC0, // joustPlatform3L
            0x080E3D4C, // joustPlatform3R
            0x080E3DD8, // joustPlatform4L
            0x080E3E64, // joustPlatform4R
            0x080E3F34, // gemJoust
            0x080E4024, // gem2Joust
            0x080E4114, // gem3Joust
            0x080E4204, // gem4Joust
            0x080E4308, // joustPlatformFall
            0x080E435C, // joustCounter
            0x080E43B8, // joustCount
            0x080E4480, // triggerScript
            0x080E44C0, // triggerUpScript
            0x080E4504, // triggerDownScript
            0x080E4548, // triggerLeftScript
            0x080E458C, // triggerRightScript
            0x080E45CC, // trigger1Script
            0x080E460C, // trigger2Script
            0x080E464C, // trigger3Script
            0x080E468C, // trigger4Script
            0x080E46CC, // trigger5Script
            0x080E471C, // checkpoint
            0x080E4768, // topDownPortal
            0x080E47AC, // topDownPortalFail
            0x080E47EC, // triggerCandleFlame
            0x080E4850, // lightCandle
            0x080E48CC, // triggerTorchSmall
            0x080E4934, // lightTorchSmall
            0x080E49B0, // triggerTorchLarge
            0x080E4A04, // lightTorchLarge
            0x080E4A7C, // triggerIceTorch
            0x080E4AD0, // lightIceTorch
            0x080E4B44, // NPCScript
            0x080E4B78, // NPCTalkScript
            0x080E4BF8, // blinkyDirtAppear
            0x080E4CBC, // blinkySnowAppear
            0x080E4D7C, // spawnPortal
            0x080E4E6C, // hiddenPortalC_C
            0x080E4EF0, // spawnPortal
            0x080E4FE0, // hiddenPortalC_J
            0x080E5064, // spawnPortal
            0x080E5154, // hiddenPortalC_S
            0x080E51D8, // hiddenPortalI_C
            0x080E5248, // hiddenPortalI_J
            0x080E52B8, // hiddenPortalI_S
            0x080E533C, // hiddenPortalF_C
            0x080E53AC, // hiddenPortalF_J
            0x080E541C, // hiddenPortalF_S
            0x080E54A0, // hiddenPortalJ_C
            0x080E5510, // hiddenPortalJ_J
            0x080E5580, // hiddenPortalJ_S
            0x080E560C, // hiddenPortalSpawnC
            0x080E5680, // hiddenPortalSpawnJ
            0x080E56F4, // hiddenPortalSpawnS
            0x080E5748, // poopASpyro
            0x080E591C, // bouncyBranchBounce
            0x080E5980, // idle
            0x080E5A70, // bouncyBranch
            0x080E5AE0, // platformUpScript
            0x080E5B74, // platformDownScript
            0x080E5BE4, // platformLeftScript
            0x080E5C60, // platformRightScript
            0x080E5CD0, // platformIdleScript
            0x080E5D18, // platformCrumbleScript
            0x080E5E50, // checkspawn
            0x080E5F54, // sheepHornDive
            0x080E601C, // sheepPound
            0x080E605C, // sheepPounded
            0x080E60AC, // sheepDrop
            0x080E6120, // droppin
            0x080E6210, // sheepDropped
            0x080E627C, // sheepFall
            0x080E636C, // movin
            0x080E6470, // movin
            0x080E658C, // "eating"
            0x080E667C, // "hungry"
            0x080E676C, // sheepPatrol
            0x080E6850, // turnRight
            0x080E694C, // turnLeft
            0x080E6A5C, // "startle"
            0x080E6B4C, // sheepTurn
            0x080E6C88, // sheepDie
            0x080E6DDC, // butterflyFlutter
            0x080E6E74, // sniff
            0x080E6F7C, // bunny
            0x080E70A8, // bunnyDie
            0x080E71AC, // bouncySheep
            0x080E7270, // herding
            0x080E7360, // sheepHerder
            0x080E7424, // again
            0x080E753C, // sheepHerd
            0x080E75DC, // sheepHerdKill
            0x080E7618, // sheepHerded
            0x080E76C4, // sheepChaseSpawn
            0x080E77D8, // sheepRun
            0x080E7850, // sheepJump
            0x080E78C0, // sheepJumpBig
            0x080E7930, // sheepJumpDown
            0x080E79D8, // bouncySheepBounce
            0x080E7AA8, // lavaFountainScript
            0x080E7B8C, // sheepShoot
            0x080E7C40, // childKilled
            0x080E7D04, // breakoutLabAssProjectileScript
            0x080E7DC4, // "InnerLoop"
            0x080E7EB4, // globalController
            0x080E7F38, // breakoutLabAssShooterScript
            0x080E7FC8, // breakoutLabAssScript
            0x080E805C, // breakoutRhynocShieldScript
            0x080E8104, // breakoutRhynocBallScript
            0x080E8194, // breakoutRhynocScript
            0x080E822C, // breakoutWallOnScript
            0x080E8280, // stepLeft
            0x080E82C8, // stepRight
            0x080E8310, // stepDown
            0x080E8348, // newWave
            0x080E838C, // genericDeathScript
            0x080E845C, // DieAndDropBallScript
            0x080E84A4, // genericDrop
            0x080E8504, // genericTurnRightAndDrop
            0x080E854C, // genericTurnLeftAndDrop
            0x080E8594, // breakoutWallOffScript
            0x080E85F4, // genericTurnLeftScript
            0x080E8648, // genericTurnRightScript
        };
    }

    public class GBAVV_SpyroFusionUS2_Manager : GBAVV_SpyroFusion_Manager
    {
        public override uint ObjTypesPointer => throw new NotImplementedException();

        public override uint[] AnimSetPointers => new uint[]
        {
            0x0826285C,
            0x08267828,
            0x08286128,
            0x08287A08,
            0x08289E68,
            0x0828EA4C,
            0x08291CE0,
            0x0829AAC8,
            0x0829C8DC,
            0x0829DA3C,
            0x0829E4D4,
            0x082A1C04,
            0x082BB0C4,
            0x082BB9B4,
            0x082BF8B4,
            0x082C9A48,
            0x082CADDC,
            0x082CE650,
            0x082D0B1C,
            0x082D1AB0,
            0x082DA304,
            0x082DC5B8,
            0x082E0A14,
            0x082E6410,
            0x082EFE6C
        };

        public override uint[] ScriptPointers => new uint[]
        {
            0x0805E244, // genericNPC
            0x0805E2AC, // talkToVictim
            0x0805E544, // script_waitForInputOrTime
            0x08060488, // notFound
            0x08060730, // waitForPagedText
            0x08060798, // missing
            0x080607E0, // Sparx01H01
            0x08060834, // Sparx01H02
            0x08060888, // Sparx01H03
            0x080608DC, // Blinky01A01
            0x08060954, // Bianca01A01
            0x080609CC, // Bianca01A03
            0x08060A44, // Blinky01A03
            0x08060A98, // Hunter01B01
            0x08060B34, // Blinky01B04
            0x08060B88, // Blinky01B05
            0x08060C00, // Blinky01B07
            0x08060C58, // Moneybags01A01
            0x08060CD4, // Moneybags01B01
            0x08060D28, // Boss0101
            0x08060D7C, // Sparx02H01
            0x08060DF8, // Professor02A01
            0x08060EB8, // Coco02A01
            0x08060F54, // Hunter02A01
            0x08060FA8, // Byrd02A01
            0x08061020, // Crash02B03
            0x08061078, // Moneybags02A01
            0x080610F4, // Moneybags02B01
            0x08061148, // Boss0201
            0x0806119C, // Sparx03H01
            0x08061214, // Crunch03A01
            0x08061268, // Blinky03B04
            0x080612BC, // Crash03B01
            0x08061314, // Moneybags03A01
            0x0806136C, // Moneybags03B01
            0x080613C0, // Boss0301
            0x0806145C, // Boss0304
            0x080614B0, // Boss0305
            0x08061504, // Boss0306
            0x08061558, // Hunter04A01
            0x080615AC, // Crash04A01
            0x08061600, // Agent04B04
            0x08061658, // Moneybags04A01
            0x080616B0, // Moneybags04B01
            0x08061704, // Boss0401
            0x080617A4, // Professor05A01
            0x080617F8, // Crash05A01
            0x08061850, // Moneybags05A01
            0x080618CC, // Moneybags05B01
            0x08061920, // Boss0501
            0x08061974, // Boss0502
            0x080619EC, // Boss0504
            0x08061A64, // Boss0506
            0x08061B10, // instructionsRaiseTheGate
            0x08061B94, // instructionsIceWallMelt
            0x08061C20, // instructionsFireWallExtinguish
            0x08061CA8, // instructionsRockVinePull
            0x08061D2C, // instructionsTugOfWar
            0x08061DB0, // instructionsIceChopper
            0x08061E58, // instructionsAirAttack
            0x08061F00, // instructionsSpyroGlide
            0x08061FAC, // instructionsDragonAssault
            0x08062054, // instructionsDeflection
            0x08062140, // instructionsJeep
            0x0806222C, // instructionsWalker
            0x080622F8, // instructionsBridgeFight
            0x080623E4, // instructionsGemRush
            0x08062444, // instructionsPortalRush
            0x080624A0, // instructionsChase
            0x08062544, // instructionsJoust
            0x080625C8, // instructionsPlatforms
            0x0806264C, // instructionsSheepHerd
            0x080626F4, // instructionsSheepBounce
            0x0806279C, // instructionsSheepShake
            0x08062844, // instructionsSheepJeep
            0x080D8AA0, // movie_license
            0x080D8B50, // movie_credits
            0x080D8BC4, // movie_secretCredits
            0x080D8CBC, // "titleText"
            0x080D94A4, // waitForPagedText
            0x080D9610, // takeCrateHitR
            0x080D9698, // ninaBossEndTrigger
            0x080D9720, // boilerScript
            0x080D977C, // roofScript
            0x080D97F4, // genericCrateHit
            0x080D9900, // leftish
            0x080D99FC, // rightish
            0x080D9AEC, // runningBackAndForth
            0x080D9C0C, // gotoNextRound
            0x080D9CFC, // mainLoop
            0x080D9DEC, // ninaSpawn
            0x080D9E64, // ninaCrateL
            0x080D9EB8, // ninaCrateR
            0x080D9F24, // ninaProfCage
            0x080D9FF4, // ninaCocoCage
            0x080DA0C8, // boilerOpen
            0x080DA10C, // moveUp
            0x080DA15C, // ninaRunLeft
            0x080DA22C, // ninaRunRight
            0x080DA314, // genericCrateCode
            0x080DA428, // raiseRoof
            0x080DD918, // spawnLabAssRear
            0x080DD9BC, // spawnLabAssFront
            0x080DDA58, // takeHornDive
            0x080DDB40, // takeFlame
            0x080DDC84, // attack
            0x080DDD74, // idle
            0x080DDE64, // venusPlant
            0x080DDF20, // turninright
            0x080DE028, // turninleft
            0x080DE118, // "patrol"
            0x080DE208, // rhynocTurnScript
            0x080DE294, // rhynocPatrolScript
            0x080DE368, // rhynocIcePatrol
            0x080DE424, // turninright
            0x080DE514, // turninleft
            0x080DE604, // rhynocIcePatrolTurn
            0x080DE6C4, // rhynocClubPatrol
            0x080DE79C, // whamright
            0x080DE898, // whamleft
            0x080DE988, // rhynocClubTurn
            0x080DE9E0, // rhynocClubWham
            0x080DEA88, // rhynocThrowScript
            0x080DEBD4, // "WaitingForKill"
            0x080DECC4, // rhynocRockScript
            0x080DED88, // rhynocFly
            0x080DEE1C, // rhynocHoverUp
            0x080DEEA4, // rhynocHoverDown
            0x080DEF3C, // rhynocJungleFly
            0x080DEFD4, // rhynocJungleHoverUp
            0x080DF064, // rhynocJungleHoverDown
            0x080DF104, // turninright
            0x080DF1F4, // turninleft
            0x080DF2E4, // infernoLabAssTurnScript
            0x080DF450, // infernoLabAssPatrolScript
            0x080DF574, // firin
            0x080DF664, // idlin
            0x080DF754, // infernoLabAssShoot
            0x080DF7E0, // labAssPatrol
            0x080DF89C, // walkin
            0x080DF98C, // walkin
            0x080DFA7C, // labAssTurn
            0x080DFB44, // throwin
            0x080DFC34, // labAssCamo
            0x080DFCE8, // flying
            0x080DFDD8, // coconutThrown
            0x080DFE9C, // coconutExplode
            0x080DFF1C, // walkerLabAssFloater
            0x080E0068, // gulpSpawny
            0x080E00C8, // gulpBarrageScript
            0x080E01A4, // gulpFireScript
            0x080E0250, // gulpDead
            0x080E034C, // gulpHit
            0x080E046C, // "WaitingForBoom"
            0x080E0568, // gulpRocketScript
            0x080E06B8, // "WaitingForBoom"
            0x080E07B4, // gulpRocketRepel
            0x080E0888, // gulpScorch
            0x080E08C8, // gulpScorched
            0x080E091C, // gulpF1Script
            0x080E0994, // gulpF2Script
            0x080E0A18, // gulpF3Script
            0x080E0A90, // gulpF4Script
            0x080E0B08, // gulpF5Script
            0x080E0B78, // gulpFUpScript
            0x080E0BFC, // crushCounter
            0x080E0C68, // counting
            0x080E0D58, // crushCounting
            0x080E0DB0, // crushRunStartScript
            0x080E0E38, // headinleft
            0x080E0F48, // headinright
            0x080E1038, // crushRunScript
            0x080E10C8, // crushStop
            0x080E1168, // crushStun
            0x080E1218, // weregood
            0x080E1308, // crushGetUp
            0x080E135C, // headinleft
            0x080E144C, // headinright
            0x080E1558, // goincrazy
            0x080E1648, // crushRocketed
            0x080E171C, // crushTurn
            0x080E17DC, // turninright
            0x080E18E4, // turninleft
            0x080E19D4, // crushCharge
            0x080E1A60, // crushUpAndAtThem
            0x080E1B2C, // crushFlame
            0x080E1BC0, // crushStayPut
            0x080E1C38, // cortexFly
            0x080E1CE0, // cortexFlipUp
            0x080E1DA4, // cortexFlipDown
            0x080E1E60, // cortexFlipDownHurt
            0x080E1F2C, // cortexDropMine
            0x080E1FE8, // droppin
            0x080E20D8, // cortexMine
            0x080E21AC, // cortexUberBlast
            0x080E2224, // cortexOw
            0x080E22EC, // cortexLaughAtPunyMortal
            0x080E23B0, // cortexReload
            0x080E241C, // cortexDefeat
            0x080E24B0, // cortexEscape
            0x080E2564, // weregood
            0x080E2654, // crushUpCharge
            0x080E26AC, // lavaFountain
            0x080E2728, // floorSpikesScript
            0x080E27B4, // wallSpikesScript
            0x080E284C, // icicleScript
            0x080E28F0, // "fall"
            0x080E29EC, // icicleFallingScript
            0x080E2A9C, // "sfx"
            0x080E2B8C, // swinginAxe
            0x080E2BF4, // electrix
            0x080E2C54, // plantVines
            0x080E2CDC, // lavaSpawner
            0x080E2D3C, // lavaSpurt
            0x080E2E30, // "attackL"
            0x080E2F2C, // "attackR"
            0x080E301C, // moatWorm
            0x080E309C, // landmine
            0x080E30F4, // landmineBoom
            0x080E3138, // chaseWall
            0x080E318C, // chaseWallHit1
            0x080E31E0, // chaseWallHit2
            0x080E3238, // chaseWallDead
            0x080E3334, // flipping
            0x080E3478, // platformer
            0x080E3568, // triggerJoustPlatform
            0x080E367C, // spawnJoustPlatformL
            0x080E3784, // spawnJoustPlatformR
            0x080E3870, // turnin
            0x080E3960, // turnin
            0x080E3A50, // triggerJoustPlatformTurn
            0x080E3AC0, // platformCount
            0x080E3B0C, // joustPlatformL
            0x080E3B8C, // joustPlatformR
            0x080E3C18, // joustPlatform2L
            0x080E3CA4, // joustPlatform2R
            0x080E3D30, // joustPlatform3L
            0x080E3DBC, // joustPlatform3R
            0x080E3E48, // joustPlatform4L
            0x080E3ED4, // joustPlatform4R
            0x080E3FA4, // gemJoust
            0x080E4094, // gem2Joust
            0x080E4184, // gem3Joust
            0x080E4274, // gem4Joust
            0x080E4378, // joustPlatformFall
            0x080E43CC, // joustCounter
            0x080E4428, // joustCount
            0x080E44F0, // triggerScript
            0x080E4530, // triggerUpScript
            0x080E4574, // triggerDownScript
            0x080E45B8, // triggerLeftScript
            0x080E45FC, // triggerRightScript
            0x080E463C, // trigger1Script
            0x080E467C, // trigger2Script
            0x080E46BC, // trigger3Script
            0x080E46FC, // trigger4Script
            0x080E473C, // trigger5Script
            0x080E478C, // checkpoint
            0x080E47D8, // topDownPortal
            0x080E481C, // topDownPortalFail
            0x080E485C, // triggerCandleFlame
            0x080E48C0, // lightCandle
            0x080E493C, // triggerTorchSmall
            0x080E49A4, // lightTorchSmall
            0x080E4A20, // triggerTorchLarge
            0x080E4A74, // lightTorchLarge
            0x080E4AEC, // triggerIceTorch
            0x080E4B40, // lightIceTorch
            0x080E4BB4, // NPCScript
            0x080E4BE8, // NPCTalkScript
            0x080E4C68, // blinkyDirtAppear
            0x080E4D2C, // blinkySnowAppear
            0x080E4DEC, // spawnPortal
            0x080E4EDC, // hiddenPortalC_C
            0x080E4F60, // spawnPortal
            0x080E5050, // hiddenPortalC_J
            0x080E50D4, // spawnPortal
            0x080E51C4, // hiddenPortalC_S
            0x080E5248, // hiddenPortalI_C
            0x080E52B8, // hiddenPortalI_J
            0x080E5328, // hiddenPortalI_S
            0x080E53AC, // hiddenPortalF_C
            0x080E541C, // hiddenPortalF_J
            0x080E548C, // hiddenPortalF_S
            0x080E5510, // hiddenPortalJ_C
            0x080E5580, // hiddenPortalJ_J
            0x080E55F0, // hiddenPortalJ_S
            0x080E567C, // hiddenPortalSpawnC
            0x080E56F0, // hiddenPortalSpawnJ
            0x080E5764, // hiddenPortalSpawnS
            0x080E57B8, // poopASpyro
            0x080E598C, // bouncyBranchBounce
            0x080E59F0, // idle
            0x080E5AE0, // bouncyBranch
            0x080E5B50, // platformUpScript
            0x080E5BE4, // platformDownScript
            0x080E5C54, // platformLeftScript
            0x080E5CD0, // platformRightScript
            0x080E5D40, // platformIdleScript
            0x080E5D88, // platformCrumbleScript
            0x080E5EC0, // checkspawn
            0x080E5FC4, // sheepHornDive
            0x080E608C, // sheepPound
            0x080E60CC, // sheepPounded
            0x080E611C, // sheepDrop
            0x080E6190, // droppin
            0x080E6280, // sheepDropped
            0x080E62EC, // sheepFall
            0x080E63DC, // movin
            0x080E64E0, // movin
            0x080E65FC, // "eating"
            0x080E66EC, // "hungry"
            0x080E67DC, // sheepPatrol
            0x080E68C0, // turnRight
            0x080E69BC, // turnLeft
            0x080E6ACC, // "startle"
            0x080E6BBC, // sheepTurn
            0x080E6CF8, // sheepDie
            0x080E6E4C, // butterflyFlutter
            0x080E6EE4, // sniff
            0x080E6FEC, // bunny
            0x080E7118, // bunnyDie
            0x080E721C, // bouncySheep
            0x080E72E0, // herding
            0x080E73D0, // sheepHerder
            0x080E7494, // again
            0x080E75AC, // sheepHerd
            0x080E764C, // sheepHerdKill
            0x080E7688, // sheepHerded
            0x080E7734, // sheepChaseSpawn
            0x080E7848, // sheepRun
            0x080E78C0, // sheepJump
            0x080E7930, // sheepJumpBig
            0x080E79A0, // sheepJumpDown
            0x080E7A48, // bouncySheepBounce
            0x080E7B18, // lavaFountainScript
            0x080E7BFC, // sheepShoot
            0x080E7CB0, // childKilled
            0x080E7D74, // breakoutLabAssProjectileScript
            0x080E7E34, // "InnerLoop"
            0x080E7F24, // globalController
            0x080E7FA8, // breakoutLabAssShooterScript
            0x080E8038, // breakoutLabAssScript
            0x080E80CC, // breakoutRhynocShieldScript
            0x080E8174, // breakoutRhynocBallScript
            0x080E8204, // breakoutRhynocScript
            0x080E829C, // breakoutWallOnScript
            0x080E82F0, // stepLeft
            0x080E8338, // stepRight
            0x080E8380, // stepDown
            0x080E83B8, // newWave
            0x080E83FC, // genericDeathScript
            0x080E84CC, // DieAndDropBallScript
            0x080E8514, // genericDrop
            0x080E8574, // genericTurnRightAndDrop
            0x080E85BC, // genericTurnLeftAndDrop
            0x080E8604, // breakoutWallOffScript
            0x080E8664, // genericTurnLeftScript
            0x080E86B8, // genericTurnRightScript
        };
    }

    public class GBAVV_SpyroFusionJP_Manager : GBAVV_SpyroFusion_Manager
    {
        public override uint ObjTypesPointer => throw new NotImplementedException();

        public override uint[] AnimSetPointers => new uint[]
        {
            0x08264E68,
            0x08269E34,
            0x08288734,
            0x0828A014,
            0x0828C474,
            0x08291058,
            0x082942EC,
            0x0829D0D4,
            0x0829EEE8,
            0x082A0048,
            0x082A0AE0,
            0x082A4210,
            0x082BD6D0,
            0x082BDFC0,
            0x082C1EC0,
            0x082CC034,
            0x082CD3C8,
            0x082D0C3C,
            0x082D30E8,
            0x082D407C,
            0x082DC8D0,
            0x082DE87C,
            0x082E2CD8,
            0x082E8770,
            0x082F21CC
        };

        public override uint[] ScriptPointers => new uint[]
        {
            0x08060444, // genericNPC
            0x080604AC, // talkToVictim
            0x08060744, // script_waitForInputOrTime
            0x08062688, // notFound
            0x08062930, // waitForPagedText
            0x08062998, // missing
            0x080629E0, // Sparx01H01
            0x08062A34, // Sparx01H02
            0x08062A88, // Sparx01H03
            0x08062ADC, // Blinky01A01
            0x08062B54, // Bianca01A01
            0x08062BCC, // Bianca01A03
            0x08062C44, // Blinky01A03
            0x08062C98, // Hunter01B01
            0x08062D34, // Blinky01B04
            0x08062D88, // Blinky01B05
            0x08062E00, // Blinky01B07
            0x08062E58, // Moneybags01A01
            0x08062ED4, // Moneybags01B01
            0x08062F28, // Boss0101
            0x08062F7C, // Sparx02H01
            0x08062FF8, // Professor02A01
            0x080630B8, // Coco02A01
            0x08063154, // Hunter02A01
            0x080631A8, // Byrd02A01
            0x08063220, // Crash02B03
            0x08063278, // Moneybags02A01
            0x080632F4, // Moneybags02B01
            0x08063348, // Boss0201
            0x0806339C, // Sparx03H01
            0x08063414, // Crunch03A01
            0x08063468, // Blinky03B04
            0x080634BC, // Crash03B01
            0x08063514, // Moneybags03A01
            0x0806356C, // Moneybags03B01
            0x080635C0, // Boss0301
            0x0806365C, // Boss0304
            0x080636B0, // Boss0305
            0x08063704, // Boss0306
            0x08063758, // Hunter04A01
            0x080637AC, // Crash04A01
            0x08063800, // Agent04B04
            0x08063858, // Moneybags04A01
            0x080638B0, // Moneybags04B01
            0x08063904, // Boss0401
            0x080639A4, // Professor05A01
            0x080639F8, // Crash05A01
            0x08063A50, // Moneybags05A01
            0x08063ACC, // Moneybags05B01
            0x08063B20, // Boss0501
            0x08063B74, // Boss0502
            0x08063BEC, // Boss0504
            0x08063C64, // Boss0506
            0x08063D10, // instructionsRaiseTheGate
            0x08063D94, // instructionsIceWallMelt
            0x08063E20, // instructionsFireWallExtinguish
            0x08063EA8, // instructionsRockVinePull
            0x08063F2C, // instructionsTugOfWar
            0x08063FB0, // instructionsIceChopper
            0x08064058, // instructionsAirAttack
            0x08064100, // instructionsSpyroGlide
            0x080641AC, // instructionsDragonAssault
            0x08064254, // instructionsDeflection
            0x08064340, // instructionsJeep
            0x0806442C, // instructionsWalker
            0x080644F8, // instructionsBridgeFight
            0x080645E4, // instructionsGemRush
            0x08064644, // instructionsPortalRush
            0x080646A0, // instructionsChase
            0x08064744, // instructionsJoust
            0x080647C8, // instructionsPlatforms
            0x0806484C, // instructionsSheepHerd
            0x080648F4, // instructionsSheepBounce
            0x0806499C, // instructionsSheepShake
            0x08064A44, // instructionsSheepJeep
            0x080DB0D4, // movie_license
            0x080DB184, // movie_credits
            0x080DB1F8, // movie_secretCredits
            0x080DB2FC, // "titleText"
            0x080DBAB0, // waitForPagedText
            0x080DBC1C, // takeCrateHitR
            0x080DBCA4, // ninaBossEndTrigger
            0x080DBD2C, // boilerScript
            0x080DBD88, // roofScript
            0x080DBE00, // genericCrateHit
            0x080DBF0C, // leftish
            0x080DC008, // rightish
            0x080DC0F8, // runningBackAndForth
            0x080DC218, // gotoNextRound
            0x080DC308, // mainLoop
            0x080DC3F8, // ninaSpawn
            0x080DC470, // ninaCrateL
            0x080DC4C4, // ninaCrateR
            0x080DC530, // ninaProfCage
            0x080DC600, // ninaCocoCage
            0x080DC6D4, // boilerOpen
            0x080DC718, // moveUp
            0x080DC768, // ninaRunLeft
            0x080DC838, // ninaRunRight
            0x080DC920, // genericCrateCode
            0x080DCA34, // raiseRoof
            0x080DFF24, // spawnLabAssRear
            0x080DFFC8, // spawnLabAssFront
            0x080E0064, // takeHornDive
            0x080E014C, // takeFlame
            0x080E0290, // attack
            0x080E0380, // idle
            0x080E0470, // venusPlant
            0x080E052C, // turninright
            0x080E0634, // turninleft
            0x080E0724, // "patrol"
            0x080E0814, // rhynocTurnScript
            0x080E08A0, // rhynocPatrolScript
            0x080E0974, // rhynocIcePatrol
            0x080E0A30, // turninright
            0x080E0B20, // turninleft
            0x080E0C10, // rhynocIcePatrolTurn
            0x080E0CD0, // rhynocClubPatrol
            0x080E0DA8, // whamright
            0x080E0EA4, // whamleft
            0x080E0F94, // rhynocClubTurn
            0x080E0FEC, // rhynocClubWham
            0x080E1094, // rhynocThrowScript
            0x080E11E0, // "WaitingForKill"
            0x080E12D0, // rhynocRockScript
            0x080E1394, // rhynocFly
            0x080E1428, // rhynocHoverUp
            0x080E14B0, // rhynocHoverDown
            0x080E1548, // rhynocJungleFly
            0x080E15E0, // rhynocJungleHoverUp
            0x080E1670, // rhynocJungleHoverDown
            0x080E1710, // turninright
            0x080E1800, // turninleft
            0x080E18F0, // infernoLabAssTurnScript
            0x080E1A5C, // infernoLabAssPatrolScript
            0x080E1B80, // firin
            0x080E1C70, // idlin
            0x080E1D60, // infernoLabAssShoot
            0x080E1DEC, // labAssPatrol
            0x080E1EA8, // walkin
            0x080E1F98, // walkin
            0x080E2088, // labAssTurn
            0x080E2150, // throwin
            0x080E2240, // labAssCamo
            0x080E22F4, // flying
            0x080E23E4, // coconutThrown
            0x080E24A8, // coconutExplode
            0x080E2528, // walkerLabAssFloater
            0x080E2674, // gulpSpawny
            0x080E26D4, // gulpBarrageScript
            0x080E27B0, // gulpFireScript
            0x080E285C, // gulpDead
            0x080E2958, // gulpHit
            0x080E2A78, // "WaitingForBoom"
            0x080E2B74, // gulpRocketScript
            0x080E2CC4, // "WaitingForBoom"
            0x080E2DC0, // gulpRocketRepel
            0x080E2E94, // gulpScorch
            0x080E2ED4, // gulpScorched
            0x080E2F28, // gulpF1Script
            0x080E2FA0, // gulpF2Script
            0x080E3024, // gulpF3Script
            0x080E309C, // gulpF4Script
            0x080E3114, // gulpF5Script
            0x080E3184, // gulpFUpScript
            0x080E3208, // crushCounter
            0x080E3274, // counting
            0x080E3364, // crushCounting
            0x080E33BC, // crushRunStartScript
            0x080E3444, // headinleft
            0x080E3554, // headinright
            0x080E3644, // crushRunScript
            0x080E36D4, // crushStop
            0x080E3774, // crushStun
            0x080E3824, // weregood
            0x080E3914, // crushGetUp
            0x080E3968, // headinleft
            0x080E3A58, // headinright
            0x080E3B64, // goincrazy
            0x080E3C54, // crushRocketed
            0x080E3D28, // crushTurn
            0x080E3DE8, // turninright
            0x080E3EF0, // turninleft
            0x080E3FE0, // crushCharge
            0x080E406C, // crushUpAndAtThem
            0x080E4138, // crushFlame
            0x080E41CC, // crushStayPut
            0x080E4244, // cortexFly
            0x080E42EC, // cortexFlipUp
            0x080E43B0, // cortexFlipDown
            0x080E446C, // cortexFlipDownHurt
            0x080E4538, // cortexDropMine
            0x080E45F4, // droppin
            0x080E46E4, // cortexMine
            0x080E47B8, // cortexUberBlast
            0x080E4830, // cortexOw
            0x080E48F8, // cortexLaughAtPunyMortal
            0x080E49BC, // cortexReload
            0x080E4A28, // cortexDefeat
            0x080E4ABC, // cortexEscape
            0x080E4B70, // weregood
            0x080E4C60, // crushUpCharge
            0x080E4CB8, // lavaFountain
            0x080E4D34, // floorSpikesScript
            0x080E4DC0, // wallSpikesScript
            0x080E4E58, // icicleScript
            0x080E4EFC, // "fall"
            0x080E4FF8, // icicleFallingScript
            0x080E50A8, // "sfx"
            0x080E5198, // swinginAxe
            0x080E5200, // electrix
            0x080E5260, // plantVines
            0x080E52E8, // lavaSpawner
            0x080E5348, // lavaSpurt
            0x080E543C, // "attackL"
            0x080E5538, // "attackR"
            0x080E5628, // moatWorm
            0x080E56A8, // landmine
            0x080E5700, // landmineBoom
            0x080E5744, // chaseWall
            0x080E5798, // chaseWallHit1
            0x080E57EC, // chaseWallHit2
            0x080E5844, // chaseWallDead
            0x080E5940, // flipping
            0x080E5A84, // platformer
            0x080E5B74, // triggerJoustPlatform
            0x080E5C88, // spawnJoustPlatformL
            0x080E5D90, // spawnJoustPlatformR
            0x080E5E7C, // turnin
            0x080E5F6C, // turnin
            0x080E605C, // triggerJoustPlatformTurn
            0x080E60CC, // platformCount
            0x080E6118, // joustPlatformL
            0x080E6198, // joustPlatformR
            0x080E6224, // joustPlatform2L
            0x080E62B0, // joustPlatform2R
            0x080E633C, // joustPlatform3L
            0x080E63C8, // joustPlatform3R
            0x080E6454, // joustPlatform4L
            0x080E64E0, // joustPlatform4R
            0x080E65B0, // gemJoust
            0x080E66A0, // gem2Joust
            0x080E6790, // gem3Joust
            0x080E6880, // gem4Joust
            0x080E6984, // joustPlatformFall
            0x080E69D8, // joustCounter
            0x080E6A34, // joustCount
            0x080E6AFC, // triggerScript
            0x080E6B3C, // triggerUpScript
            0x080E6B80, // triggerDownScript
            0x080E6BC4, // triggerLeftScript
            0x080E6C08, // triggerRightScript
            0x080E6C48, // trigger1Script
            0x080E6C88, // trigger2Script
            0x080E6CC8, // trigger3Script
            0x080E6D08, // trigger4Script
            0x080E6D48, // trigger5Script
            0x080E6D98, // checkpoint
            0x080E6DE4, // topDownPortal
            0x080E6E28, // topDownPortalFail
            0x080E6E68, // triggerCandleFlame
            0x080E6ECC, // lightCandle
            0x080E6F48, // triggerTorchSmall
            0x080E6FB0, // lightTorchSmall
            0x080E702C, // triggerTorchLarge
            0x080E7080, // lightTorchLarge
            0x080E70F8, // triggerIceTorch
            0x080E714C, // lightIceTorch
            0x080E71C0, // NPCScript
            0x080E71F4, // NPCTalkScript
            0x080E7274, // blinkyDirtAppear
            0x080E7338, // blinkySnowAppear
            0x080E73F8, // spawnPortal
            0x080E74E8, // hiddenPortalC_C
            0x080E756C, // spawnPortal
            0x080E765C, // hiddenPortalC_J
            0x080E76E0, // spawnPortal
            0x080E77D0, // hiddenPortalC_S
            0x080E7854, // hiddenPortalI_C
            0x080E78C4, // hiddenPortalI_J
            0x080E7934, // hiddenPortalI_S
            0x080E79B8, // hiddenPortalF_C
            0x080E7A28, // hiddenPortalF_J
            0x080E7A98, // hiddenPortalF_S
            0x080E7B1C, // hiddenPortalJ_C
            0x080E7B8C, // hiddenPortalJ_J
            0x080E7BFC, // hiddenPortalJ_S
            0x080E7C88, // hiddenPortalSpawnC
            0x080E7CFC, // hiddenPortalSpawnJ
            0x080E7D70, // hiddenPortalSpawnS
            0x080E7DC4, // poopASpyro
            0x080E7F98, // bouncyBranchBounce
            0x080E7FFC, // idle
            0x080E80EC, // bouncyBranch
            0x080E815C, // platformUpScript
            0x080E81F0, // platformDownScript
            0x080E8260, // platformLeftScript
            0x080E82DC, // platformRightScript
            0x080E834C, // platformIdleScript
            0x080E8394, // platformCrumbleScript
            0x080E84CC, // checkspawn
            0x080E85D0, // sheepHornDive
            0x080E8698, // sheepPound
            0x080E86D8, // sheepPounded
            0x080E8728, // sheepDrop
            0x080E879C, // droppin
            0x080E888C, // sheepDropped
            0x080E88F8, // sheepFall
            0x080E89E8, // movin
            0x080E8AEC, // movin
            0x080E8C08, // "eating"
            0x080E8CF8, // "hungry"
            0x080E8DE8, // sheepPatrol
            0x080E8ECC, // turnRight
            0x080E8FC8, // turnLeft
            0x080E90D8, // "startle"
            0x080E91C8, // sheepTurn
            0x080E9304, // sheepDie
            0x080E9458, // butterflyFlutter
            0x080E94F0, // sniff
            0x080E95F8, // bunny
            0x080E9724, // bunnyDie
            0x080E9828, // bouncySheep
            0x080E98EC, // herding
            0x080E99DC, // sheepHerder
            0x080E9AA0, // again
            0x080E9BB8, // sheepHerd
            0x080E9C58, // sheepHerdKill
            0x080E9C94, // sheepHerded
            0x080E9D40, // sheepChaseSpawn
            0x080E9E54, // sheepRun
            0x080E9ECC, // sheepJump
            0x080E9F3C, // sheepJumpBig
            0x080E9FAC, // sheepJumpDown
            0x080EA054, // bouncySheepBounce
            0x080EA124, // lavaFountainScript
            0x080EA208, // sheepShoot
            0x080EA2BC, // childKilled
            0x080EA380, // breakoutLabAssProjectileScript
            0x080EA440, // "InnerLoop"
            0x080EA530, // globalController
            0x080EA5B4, // breakoutLabAssShooterScript
            0x080EA644, // breakoutLabAssScript
            0x080EA6D8, // breakoutRhynocShieldScript
            0x080EA780, // breakoutRhynocBallScript
            0x080EA810, // breakoutRhynocScript
            0x080EA8A8, // breakoutWallOnScript
            0x080EA8FC, // stepLeft
            0x080EA944, // stepRight
            0x080EA98C, // stepDown
            0x080EA9C4, // newWave
            0x080EAA08, // genericDeathScript
            0x080EAAD8, // DieAndDropBallScript
            0x080EAB20, // genericDrop
            0x080EAB80, // genericTurnRightAndDrop
            0x080EABC8, // genericTurnLeftAndDrop
            0x080EAC10, // breakoutWallOffScript
            0x080EAC70, // genericTurnLeftScript
            0x080EACC4, // genericTurnRightScript
        };
    }
}