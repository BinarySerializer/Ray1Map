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
            new ObjTypeInit(-1, -1, null), // 101
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
            new ObjTypeInit(-1, -1, null), // 268
            new ObjTypeInit(4, 29, "breakoutRhynocScript"), // 269
            new ObjTypeInit(-1, -1, null), // 270
            new ObjTypeInit(4, 26, "breakoutLabAssShooterScript"), // 271
            new ObjTypeInit(4, 40, "breakoutRhynocShieldScript"), // 272
            new ObjTypeInit(4, 35, "breakoutLabAssProjectileScript"), // 273
            new ObjTypeInit(4, 29, "breakoutLabAssScript"), // 274
            new ObjTypeInit(4, 43, "breakoutRhynocBallScript"), // 275
            new ObjTypeInit(-1, -1, null), // 276
            new ObjTypeInit(4, 29, "globalController"), // 277
            new ObjTypeInit(-1, -1, null), // 278
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
    }

    public class GBAVV_SpyroFusionUS_Manager : GBAVV_SpyroFusion_Manager
    {
        public override int ObjTypesCount => 282;
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
}