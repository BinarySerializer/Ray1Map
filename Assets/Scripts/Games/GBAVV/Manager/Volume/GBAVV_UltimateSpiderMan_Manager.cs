using System.Collections.Generic;

namespace Ray1Map.GBAVV
{
    public abstract class GBAVV_UltimateSpiderMan_Manager : GBAVV_Volume_BaseManager
    {
        // Metadata
        public override int VolumesCount => 7;
        public override bool HasAssignedObjTypeGraphics => true;

        // Scripts
        public override Dictionary<int, GBAVV_ScriptCommand.CommandType> ScriptCommands => new Dictionary<int, GBAVV_ScriptCommand.CommandType>()
        {
            [1201] = GBAVV_ScriptCommand.CommandType.Name,
            [1206] = GBAVV_ScriptCommand.CommandType.Return,
            [1602] = GBAVV_ScriptCommand.CommandType.Dialog,
        };

        // Levels
        public override LevInfo[] LevInfos => Levels;
        public static LevInfo[] Levels => new LevInfo[]
        {
            new LevInfo(0, 0, 0, "Small Time", "", ""),
            new LevInfo(0, 0, 1, "Small Time", "", ""),
            new LevInfo(0, 1, 0, "Small Time", "", ""),
            new LevInfo(0, 1, 1, "Small Time", "", ""),
            new LevInfo(0, 2, 0, "Small Time", "", ""),
            new LevInfo(0, 2, 1, "Small Time", "", ""),
            new LevInfo(1, 0, 0, "Frenzy", "", ""),
            new LevInfo(1, 0, 1, "Frenzy", "", ""),
            new LevInfo(1, 1, 0, "Frenzy", "", ""),
            new LevInfo(1, 1, 1, "Frenzy", "", ""),
            new LevInfo(1, 2, 0, "Frenzy", "", ""),
            new LevInfo(2, 0, 0, "Hidden Charges", "", ""),
            new LevInfo(2, 0, 1, "Hidden Charges", "", ""),
            new LevInfo(2, 1, 0, "Hidden Charges", "", ""),
            new LevInfo(2, 1, 1, "Hidden Charges", "", ""),
            new LevInfo(2, 2, 0, "Hidden Charges", "", ""),
            new LevInfo(3, 0, 0, "Starving Artist", "", ""),
            new LevInfo(3, 0, 1, "Starving Artist", "", ""),
            new LevInfo(3, 1, 0, "Starving Artist", "", ""),
            new LevInfo(3, 1, 1, "Starving Artist", "", ""),
            new LevInfo(3, 2, 0, "Starving Artist", "", ""),
            new LevInfo(4, 0, 0, "Hunted", "", ""),
            new LevInfo(4, 0, 1, "Hunted", "", ""),
            new LevInfo(4, 1, 0, "Hunted", "", ""),
            new LevInfo(4, 1, 1, "Hunted", "", ""),
            new LevInfo(4, 2, 0, "Hunted", "", ""),
            new LevInfo(5, 0, 0, "Silver and Cold", "", ""),
            new LevInfo(5, 0, 1, "Silver and Cold", "", ""),
            new LevInfo(5, 0, 2, "Silver and Cold", "", ""),
            new LevInfo(5, 1, 0, "Silver and Cold", "", ""),
            new LevInfo(5, 2, 0, "Silver and Cold", "", ""),
            new LevInfo(6, 0, 0, "Lab Test", "", ""),
            new LevInfo(6, 1, 0, "Lab Test", "", ""),
            new LevInfo(6, 1, 1, "Lab Test", "", ""),
            new LevInfo(6, 1, 2, "Lab Test", "", ""),
            new LevInfo(6, 1, 3, "Lab Test", "", ""),
            new LevInfo(6, 2, 0, "Lab Test", "", ""),
        };

        public override ObjTypeInit[] ObjTypeInitInfos => new ObjTypeInit[]
        {
            new ObjTypeInit(-1, -1, null), // 0
            new ObjTypeInit(-1, -1, null), // 1
            new ObjTypeInit(-1, -1, null), // 2
            new ObjTypeInit(-1, -1, null), // 3
            new ObjTypeInit(-1, -1, null), // 4
            new ObjTypeInit(-1, -1, null), // 5
            new ObjTypeInit(17, 0, "spideyHealthLarge"), // 6
            new ObjTypeInit(17, 1, "spideyHealthSmall"), // 7
            new ObjTypeInit(17, 2, "spideyWebLarge"), // 8
            new ObjTypeInit(17, 3, "spideyWebSmall"), // 9
            new ObjTypeInit(17, 4, "spideyWebFluid1"), // 10
            new ObjTypeInit(17, 4, "spideyWebFluid2"), // 11
            new ObjTypeInit(17, 4, "spideyWebFluid3"), // 12
            new ObjTypeInit(17, 7, "spideyWeb1"), // 13
            new ObjTypeInit(17, 7, "spideyWeb2"), // 14
            new ObjTypeInit(17, 6, "spideyAgility1"), // 15
            new ObjTypeInit(17, 6, "spideyAgility2"), // 16
            new ObjTypeInit(17, 8, "venomAdaptation"), // 17
            new ObjTypeInit(17, 12, "venomFeed1"), // 18
            new ObjTypeInit(17, 12, "venomFeed2"), // 19
            new ObjTypeInit(17, 11, "venomToughness1"), // 20
            new ObjTypeInit(17, 11, "venomToughness2"), // 21
            new ObjTypeInit(17, 13, "venomCombat1"), // 22
            new ObjTypeInit(17, 13, "venomCombat2"), // 23
            new ObjTypeInit(17, 10, "venomInvincibility"), // 24
            new ObjTypeInit(17, 14, "venomInvincibilitySmall"), // 25
            new ObjTypeInit(-1, -1, null), // 26
            new ObjTypeInit(21, 5, null), // 27
            new ObjTypeInit(10, 1, "breakableDoor"), // 28
            new ObjTypeInit(10, 2, "breakableDoorSewer"), // 29
            new ObjTypeInit(10, 5, "breakableGeneric"), // 30
            new ObjTypeInit(10, 9, "breakableGeneric"), // 31
            new ObjTypeInit(10, 10, "breakableGeneric"), // 32
            new ObjTypeInit(10, 15, "breakableGeneric"), // 33
            new ObjTypeInit(10, 16, "breakableGeneric"), // 34
            new ObjTypeInit(10, 19, "bombIdle"), // 35
            new ObjTypeInit(10, 23, "gasTank"), // 36
            new ObjTypeInit(10, 26, "breakableCrate"), // 37
            new ObjTypeInit(10, 29, "breakableBGJumpable"), // 38
            new ObjTypeInit(10, 32, "breakableBGNonJumpable"), // 39
            new ObjTypeInit(10, 35, "breakableBGJumpableOneHit"), // 40
            new ObjTypeInit(10, 38, "breakableSolid"), // 41
            new ObjTypeInit(10, 40, "breakableBGJumpableOneHit"), // 42
            new ObjTypeInit(20, 0, "switchableDoor"), // 43
            new ObjTypeInit(14, 1, "laserBeamIdle"), // 44
            new ObjTypeInit(14, 7, "laserSwitchBlueIdle"), // 45
            new ObjTypeInit(14, 3, "laserBeamIdle"), // 46
            new ObjTypeInit(14, 9, "laserSwitchRedIdle"), // 47
            new ObjTypeInit(14, 5, "laserBeamIdle"), // 48
            new ObjTypeInit(14, 11, "laserSwitchYellowIdle"), // 49
            new ObjTypeInit(20, 2, "switchOnce"), // 50
            new ObjTypeInit(20, 4, "bankRubbleIdle"), // 51
            new ObjTypeInit(20, 5, "bankWallIdle"), // 52
            new ObjTypeInit(20, 6, "museumWallIdle"), // 53
            new ObjTypeInit(20, 9, "solidIdle"), // 54
            new ObjTypeInit(14, 35, "steamValveIdle"), // 55
            new ObjTypeInit(14, 37, "steamWallIdle"), // 56
            new ObjTypeInit(14, 35, "gasValveIdle"), // 57
            new ObjTypeInit(14, 37, "gasWallIdle"), // 58
            new ObjTypeInit(-1, -1, null), // 59
            new ObjTypeInit(-1, -1, null), // 60
            new ObjTypeInit(-1, -1, null), // 61
            new ObjTypeInit(-1, -1, null), // 62
            new ObjTypeInit(-1, -1, null), // 63
            new ObjTypeInit(-1, -1, null), // 64
            new ObjTypeInit(-1, -1, null), // 65
            new ObjTypeInit(-1, -1, null), // 66
            new ObjTypeInit(-1, -1, null), // 67
            new ObjTypeInit(4, 16, null), // 68
            new ObjTypeInit(4, 16, null), // 69
            new ObjTypeInit(4, 16, null), // 70
            new ObjTypeInit(9, 17, null), // 71
            new ObjTypeInit(9, 17, null), // 72
            new ObjTypeInit(5, 16, null), // 73
            new ObjTypeInit(5, 16, null), // 74
            new ObjTypeInit(-1, -1, null), // 75
            new ObjTypeInit(-1, -1, null), // 76
            new ObjTypeInit(1, 17, null), // 77
            new ObjTypeInit(1, 17, null), // 78
            new ObjTypeInit(8, 15, null), // 79
            new ObjTypeInit(8, 15, null), // 80
            new ObjTypeInit(2, 16, null), // 81
            new ObjTypeInit(2, 16, null), // 82
            new ObjTypeInit(7, 20, null), // 83
            new ObjTypeInit(7, 20, null), // 84
            new ObjTypeInit(-1, -1, null), // 85
            new ObjTypeInit(-1, -1, null), // 86
            new ObjTypeInit(1, 17, null), // 87
            new ObjTypeInit(-1, -1, null), // 88
            new ObjTypeInit(-1, -1, null), // 89
            new ObjTypeInit(-1, -1, null), // 90
            new ObjTypeInit(-1, -1, null), // 91
            new ObjTypeInit(-1, -1, null), // 92
            new ObjTypeInit(-1, -1, null), // 93
            new ObjTypeInit(-1, -1, null), // 94
            new ObjTypeInit(-1, -1, null), // 95
            new ObjTypeInit(16, 1, "helpPromptIdle"), // 96
            new ObjTypeInit(16, 12, "victim"), // 97
            new ObjTypeInit(16, 12, "victim"), // 98
            new ObjTypeInit(16, 12, "victim"), // 99
            new ObjTypeInit(16, 12, "victim"), // 100
            new ObjTypeInit(16, 4, "victim"), // 101
            new ObjTypeInit(16, 4, "victim"), // 102
            new ObjTypeInit(16, 4, "victim"), // 103
            new ObjTypeInit(16, 4, "victim"), // 104
            new ObjTypeInit(16, 12, "TAMVictimRescueImmediately"), // 105
            new ObjTypeInit(16, 12, "TAMVictimRescueImmediately"), // 106
            new ObjTypeInit(16, 12, "TAMVictimRescueImmediately"), // 107
            new ObjTypeInit(16, 12, "TAMVictimRescueImmediately"), // 108
            new ObjTypeInit(16, 4, "TAMVictimRescueImmediately"), // 109
            new ObjTypeInit(16, 4, "TAMVictimRescueImmediately"), // 110
            new ObjTypeInit(16, 4, "TAMVictimRescueImmediately"), // 111
            new ObjTypeInit(16, 4, "TAMVictimRescueImmediately"), // 112
            new ObjTypeInit(16, 19, "victim"), // 113
            new ObjTypeInit(16, 19, "victim"), // 114
            new ObjTypeInit(16, 19, "victim"), // 115
            new ObjTypeInit(16, 19, "victim"), // 116
            new ObjTypeInit(16, 8, "victim"), // 117
            new ObjTypeInit(16, 8, "victim"), // 118
            new ObjTypeInit(16, 8, "victim"), // 119
            new ObjTypeInit(16, 8, "victim"), // 120
            new ObjTypeInit(16, 19, "TAMVictim"), // 121
            new ObjTypeInit(16, 19, "TAMVictim"), // 122
            new ObjTypeInit(16, 19, "TAMVictim"), // 123
            new ObjTypeInit(16, 19, "TAMVictim"), // 124
            new ObjTypeInit(16, 8, "TAMVictim"), // 125
            new ObjTypeInit(16, 8, "TAMVictim"), // 126
            new ObjTypeInit(16, 8, "TAMVictim"), // 127
            new ObjTypeInit(16, 8, "TAMVictim"), // 128
            new ObjTypeInit(16, 12, "venomTAMLevelVictimActivate"), // 129
            new ObjTypeInit(16, 12, "venomTAMLevelVictimActivate"), // 130
            new ObjTypeInit(16, 12, "venomTAMLevelVictimActivate"), // 131
            new ObjTypeInit(16, 12, "venomTAMLevelVictimActivate"), // 132
            new ObjTypeInit(16, 4, "venomTAMLevelVictimActivate"), // 133
            new ObjTypeInit(16, 4, "venomTAMLevelVictimActivate"), // 134
            new ObjTypeInit(16, 4, "venomTAMLevelVictimActivate"), // 135
            new ObjTypeInit(16, 4, "venomTAMLevelVictimActivate"), // 136
            new ObjTypeInit(16, 12, "victimRun"), // 137
            new ObjTypeInit(16, 12, "victimRun"), // 138
            new ObjTypeInit(16, 12, "victimRun"), // 139
            new ObjTypeInit(16, 12, "victimRun"), // 140
            new ObjTypeInit(16, 4, "victimRun"), // 141
            new ObjTypeInit(16, 4, "victimRun"), // 142
            new ObjTypeInit(16, 4, "victimRun"), // 143
            new ObjTypeInit(16, 4, "victimRun"), // 144
            new ObjTypeInit(6, 1, "sableStartRun"), // 145
            new ObjTypeInit(6, 1, "SableRunCreate"), // 146
            new ObjTypeInit(16, 2, "shockerSpeaks"), // 147
            new ObjTypeInit(21, 1, "venomRunning"), // 148
            new ObjTypeInit(1, 11, "commandoRunCreate"), // 149
            new ObjTypeInit(16, 3, null), // 150
            new ObjTypeInit(15, 16, "TAMTrigger"), // 151
            new ObjTypeInit(15, 17, "TAMTrigger"), // 152
            new ObjTypeInit(15, 19, "TAMTrigger"), // 153
            new ObjTypeInit(15, 18, "TAMTrigger"), // 154
            new ObjTypeInit(15, 20, "TAMTriggerBigger"), // 155
            new ObjTypeInit(15, 19, "proximityTAMTargetIdle"), // 156
            new ObjTypeInit(20, 7, "TAMWallSetup"), // 157
            new ObjTypeInit(-1, -1, null), // 158
            new ObjTypeInit(14, 13, "hydrantOpen"), // 159
            new ObjTypeInit(14, 17, "hydrantDrippingLinked"), // 160
            new ObjTypeInit(14, 16, "hydrantOpenLinked"), // 161
            new ObjTypeInit(14, 12, "hydrantClosed"), // 162
            new ObjTypeInit(14, 20, "electricIdle"), // 163
            new ObjTypeInit(14, 21, "electricIdle"), // 164
            new ObjTypeInit(14, 27, "electricWallHorizontalIdle"), // 165
            new ObjTypeInit(14, 28, "electricWallVerticalIdle"), // 166
            new ObjTypeInit(14, 22, "generatorIdle"), // 167
            new ObjTypeInit(14, 25, "electricSwitchIdle"), // 168
            new ObjTypeInit(14, 27, "electricWallHorizontalHide"), // 169
            new ObjTypeInit(14, 28, "electricWallVerticalHide"), // 170
            new ObjTypeInit(14, 32, "TAMElectricWallHide"), // 171
            new ObjTypeInit(14, 30, "TAMElectricWallHide"), // 172
            new ObjTypeInit(14, 41, null), // 173
            new ObjTypeInit(14, 42, null), // 174
            new ObjTypeInit(14, 43, null), // 175
            new ObjTypeInit(14, 44, null), // 176
            new ObjTypeInit(14, 48, "hydrantOpen"), // 177
            new ObjTypeInit(14, 47, "hydrantDrippingLinked"), // 178
            new ObjTypeInit(14, 49, "hydrantOpenLinked"), // 179
            new ObjTypeInit(14, 46, "hydrantClosed"), // 180
            new ObjTypeInit(14, 51, null), // 181
            new ObjTypeInit(14, 56, "ceilingTurretSetup"), // 182
            new ObjTypeInit(14, 57, "ceilingTurretSetup"), // 183
            new ObjTypeInit(14, 63, "wallTurretSetup"), // 184
            new ObjTypeInit(14, 64, "wallTurretSetup"), // 185
            new ObjTypeInit(18, 0, null), // 186
            new ObjTypeInit(18, 2, null), // 187
            new ObjTypeInit(18, 2, "offscreenSpawner"), // 188
            new ObjTypeInit(18, 2, "fight"), // 189
            new ObjTypeInit(18, 2, "fight"), // 190
            new ObjTypeInit(18, 2, "fight"), // 191
            new ObjTypeInit(18, 2, "fight"), // 192
            new ObjTypeInit(18, 2, "fight"), // 193
            new ObjTypeInit(21, 85, "venomEndRunCreate"), // 194
            new ObjTypeInit(21, 85, "venomCreate"), // 195
            new ObjTypeInit(18, 2, "fight"), // 196
            new ObjTypeInit(18, 2, "carnageFight"), // 197
            new ObjTypeInit(21, 1, "venomEndJumpCreate"), // 198
            new ObjTypeInit(21, 1, "venomStartRun"), // 199
            new ObjTypeInit(13, 1, "endOfSectionGeneric"), // 200
            new ObjTypeInit(13, 3, "endOfSectionGeneric"), // 201
            new ObjTypeInit(11, 6, null), // 202
            new ObjTypeInit(11, 7, null), // 203
            new ObjTypeInit(11, 8, null), // 204
            new ObjTypeInit(11, 9, null), // 205
            new ObjTypeInit(11, 10, null), // 206
            new ObjTypeInit(11, 11, null), // 207
            new ObjTypeInit(11, 12, null), // 208
            new ObjTypeInit(11, 13, null), // 209
            new ObjTypeInit(11, 14, null), // 210
            new ObjTypeInit(11, 16, null), // 211
            new ObjTypeInit(11, 17, null), // 212
            new ObjTypeInit(11, 18, null), // 213
            new ObjTypeInit(11, 19, null), // 214
            new ObjTypeInit(11, 20, null), // 215
            new ObjTypeInit(11, 21, null), // 216
            new ObjTypeInit(11, 22, null), // 217
            new ObjTypeInit(11, 23, null), // 218
            new ObjTypeInit(-1, -1, null), // 219
        };
    }
    public class GBAVV_UltimateSpiderManEU_Manager : GBAVV_UltimateSpiderMan_Manager
    {
        public override string[] Languages => new string[]
        {
            "English"
        };

        public override uint[] GraphicsDataPointers => new uint[]
        {
            0x08361478,
            0x0836380C,
            0x0836A99C,
            0x08372E28,
            0x0837B518,
            0x08382458,
            0x0838A340,
            0x08393038,
            0x0839AAB4,
            0x083A3390,
            0x083AA67C,
            0x083AE9FC,
            0x083B2310,
            0x083BC760,
            0x083BCDE0,
            0x083CDD30,
            0x083D7644,
            0x083DAE0C,
            0x083DF914,
            0x083DFB30,
            0x083F430C,
            0x083F51A8,
        };

        public override uint[] ScriptPointers => new uint[]
        {
            0x0803673C, // script_waitForInputOrTime
            0x08064414, // offscreenSpawner
            0x08065A10, // breakableGeneric
            0x08065A8C, // breakableDoor
            0x08065AE4, // breakableCrate
            0x08065B4C, // breakableDoorSewer
            0x08065BDC, // bombIdle
            0x08065C70, // setDeletePending
            0x08065D60, // proximityTAMTargetIdle
            0x08065DEC, // gasTank
            0x08065E98, // breakableSolid
            0x08065F20, // breakableBGJumpable
            0x08065F90, // breakableBGJumpableOneHit
            0x08065FF0, // breakableBGNonJumpable
            0x08066060, // breakableDoorDie
            0x08066118, // breakableGenericHit
            0x080661D0, // IsFlipped
            0x080662CC, // NotFlipped
            0x080663D0, // breakableDoorSewerFlipFlop
            0x08066444, // bombDie
            0x080664E0, // gasTankHit
            0x08066548, // breakableSolidDie
            0x080665CC, // breakableBGJumpableDie
            0x08066610, // breakableGenericDie
            0x080666A0, // breakableGenericTakeHit
            0x08066700, // gasTankDie
            0x08066794, // gasTankTakeHit
            0x08066A00, // endOfSectionGeneric
            0x08066A6C, // endOfSectionIdle
            0x08066B1C, // endOfSectionActive
            0x08066B98, // endOfSectionDialog
            0x08066C00, // endOfSectionDone
            0x08066C54, // hydrantBurstLinked
            0x080681C8, // hydrantOpen
            0x08068254, // hydrantClosed
            0x080682D0, // hydrantWebbedLinked
            0x08068344, // hydrantOpenLinked
            0x080683A4, // hydrantDrippingLinked
            0x080683E4, // electricIdle
            0x08068428, // ceilingTurretSetup
            0x08068474, // wallTurretSetup
            0x080684D4, // hydrantWebbed
            0x0806853C, // hydrantWebbedLong
            0x080685C0, // takeHit
            0x080686B0, // hydrantTakeDamage
            0x08068748, // setDeletePending
            0x08068838, // turretDeath
            0x08068908, // ceilingTurretIdle
            0x08068984, // wallTurretIdle
            0x08068A58, // turretSetup
            0x08068AEC, // turretWebbed
            0x08068BA8, // "facingDown"
            0x08068CB8, // "facingUp"
            0x08068DA8, // ceilingTurretTryAttack
            0x08068E3C, // "facingRight"
            0x08068F50, // "facingLeft"
            0x08069040, // wallTurretTryAttack
            0x080690AC, // turretAttack
            0x08069200, // helpPromptIdle
            0x08069250, // helpPromptDialog
            0x0806A8F0, // setDeletePending
            0x0806A9E0, // spideyHealthLarge
            0x0806AA5C, // spideyHealthSmall
            0x0806AAD4, // spideyWebLarge
            0x0806AB4C, // spideyWebSmall
            0x0806ABFC, // spideyWebFluid1
            0x0806ACDC, // spideyWebFluid2
            0x0806ADBC, // spideyWebFluid3
            0x0806AE8C, // spideyWeb1
            0x0806AF08, // spideyWeb2
            0x0806AF88, // spideyAgility1
            0x0806B008, // spideyAgility2
            0x0806B0A4, // venomAdaptation
            0x0806B258, // venomFeed1
            0x0806B2D4, // venomFeed2
            0x0806B354, // venomToughness1
            0x0806B3D4, // venomToughness2
            0x0806B454, // venomCombat1
            0x0806B4D4, // venomCombat2
            0x0806B544, // venomInvincibility
            0x0806B5B8, // venomInvincibilitySmall
            0x0806B624, // helpWebFluid
            0x0806B6BC, // powerupPoof
            0x0806B760, // powerupIdle
            0x0806B7F4, // help
            0x0806B900, // help
            0x0806B9F0, // spideyWeb
            0x0806BAA4, // help
            0x0806BBB0, // help
            0x0806BCA0, // spideyAgility
            0x0806BD58, // help
            0x0806BE6C, // help
            0x0806BF5C, // venomFeed
            0x0806C020, // help
            0x0806C134, // help
            0x0806C224, // venomToughness
            0x0806C2DC, // help
            0x0806C3F0, // help
            0x0806C4E0, // venomCombat
            0x0806C560, // helpWeb1
            0x0806C5EC, // helpWeb2
            0x0806C67C, // helpAgility1
            0x0806C70C, // helpAgility2
            0x0806C790, // helpFeed1
            0x0806C814, // helpFeed2
            0x0806C89C, // helpToughness1
            0x0806C924, // helpToughness2
            0x0806C9A8, // helpCombat1
            0x0806CA2C, // helpCombat2
            0x0806CB70, // fight
            0x0806CD44, // intro
            0x0806CE74, // loop
            0x0806CF70, // fight
            0x0806D0A8, // fight
            0x0806EF50, // switchableDoor
            0x0806EFA4, // switchOnce
            0x0806EFF8, // laserBeamIdle
            0x0806F064, // laserSwitchBlueIdle
            0x0806F0BC, // laserSwitchYellowIdle
            0x0806F110, // laserSwitchRedIdle
            0x0806F158, // steamWallIdle
            0x0806F1C8, // steamValveIdle
            0x0806F218, // gasWallIdle
            0x0806F294, // gasValveIdle
            0x0806F2F4, // electricWallHorizontalIdle
            0x0806F378, // electricWallHorizontalHide
            0x0806F3E0, // electricWallVerticalIdle
            0x0806F46C, // electricWallVerticalHide
            0x0806F4DC, // generatorIdle
            0x0806F550, // electricSwitchIdle
            0x0806F5A8, // TAMElectricWallHide
            0x0806F5FC, // bankRubbleIdle
            0x0806F640, // solidIdle
            0x0806F688, // bankWallIdle
            0x0806F6D8, // museumWallIdle
            0x0806F74C, // switchableDoorOpen
            0x0806F7D8, // activateSwitchOnce
            0x0806F87C, // laserBeamDisable
            0x0806F8F8, // laserSwitchBlueDeactivate
            0x0806F9B0, // laserSwitchYellowDeactivate
            0x0806FA68, // laserSwitchRedDeactivate
            0x0806FB58, // setDeletePending
            0x0806FC48, // steamWallDisable
            0x0806FCB8, // steamValveSmash
            0x0806FD64, // gasWallDisable
            0x0806FDD4, // gasValveSmash
            0x0806FEA8, // electricWallHorizontalDisable
            0x0806FF18, // electricWallVerticalDisable
            0x0806FF94, // generatorSmash
            0x08070064, // electricSwitchDeactivate
            0x08070104, // TAMElectricWallIdle
            0x080701A4, // bankRubbleAppear
            0x0807021C, // solidAppear
            0x0807027C, // bankWallDie
            0x080702D4, // musuemWallDie
            0x0807034C, // TAMElectricWallDisable
            0x08071558, // venomEndJumpCreate
            0x080715C0, // setDeletePending
            0x080716B0, // venomStartRun
            0x08071750, // venomEndRunCreate
            0x08071794, // venomCreate
            0x080717F8, // venomTAMLevelVictimActivate
            0x08071888, // venomEndJump
            0x080719A0, // "endRunOffScreenTest"
            0x08071A90, // venomEndRun
            0x08071BD8, // "setLeftResponse"
            0x08071CFC, // "setRightResponse"
            0x08071DEC, // venomSetup
            0x08071ED0, // saved1
            0x08071FC8, // saved2
            0x080720CC, // venomTAMLevelVictim
            0x08072180, // showObject
            0x080721D4, // broadcastAndDie
            0x080722C4, // venomOffScreenTest
            0x0807230C, // venomHitResponseLeft
            0x080723FC, // venomHitResponseRight
            0x0807250C, // "fallingReaction"
            0x080725FC, // venomRunning
            0x08072660, // venomAirDowning
            0x080726E0, // "nextTest"
            0x080727D0, // venomAirUpping
            0x08072824, // venomStandardHitResponseBeginning
            0x080728D0, // venomFalling
            0x08072A08, // pause
            0x08072AF8, // carnageFight
            0x08072CC4, // intro
            0x08072DBC, // loop
            0x08072EB8, // fight
            0x08073238, // fight
            0x080738A4, // fight
            0x08073C94, // waitForPagedText
            0x080789B8, // victimRun
            0x08078A84, // saved1
            0x08078B7C, // saved2
            0x08078C94, // setDeletePending
            0x08078D84, // victim
            0x08078E6C, // TAMVictim
            0x08078EF8, // TAMVictimRescueImmediately
            0x08078F7C, // sableStartRun
            0x08079018, // SableRunCreate
            0x0807906C, // venomRunning
            0x08079130, // commandoRunCreate
            0x08079190, // shockerSpeaks
            0x08079200, // completeTAMAndDie
            0x080792F0, // victimOffScreenTest
            0x08079378, // "falling"
            0x08079474, // victimFallToGround
            0x08079514, // whimperingLoop
            0x08079604, // whimperUntilRescued
            0x08079668, // notSaved
            0x08079758, // whimperUntilSaved
            0x080797B4, // TAMvictimActivated
            0x08079898, // TAMVictimRescueImmediatelyActivated
            0x08079954, // whimper
            0x080799A8, // sableRun
            0x08079A80, // commandoRunning
            0x08079F30, // TAMTrigger
            0x08079FDC, // TAMTriggerBigger
            0x0807A078, // TAMWallSetup
            0x0807A0E8, // setDeletePending
            0x0807A1D8, // TAMTriggerActivate
            0x0807A218, // TAMWallDeath
            0x0807A268, // TAMWallLoop
            0x0807A2DC, // "waitUntilPlayerLeaves"
            0x0807A3CC, // TAMWallPlayerNear
            0x081054A0, // notFound
            0x0810575C, // waitForPagedText
            0x081057C4, // missing
            0x08105818, // rescueTAM
            0x0810586C, // failTAMBomb
            0x081058C4, // powerupWebFluid
            0x08105918, // powerWeb1
            0x0810596C, // powerWeb2
            0x081059C4, // powerAgility1
            0x08105A1C, // powerAgility2
            0x08105A74, // powerAdaptation
            0x08105AC8, // powerFeed1
            0x08105B1C, // powerFeed2
            0x08105B74, // powerToughness1
            0x08105BCC, // powerToughness2
            0x08105C24, // powerCombat1
            0x08105C7C, // powerCombat2
            0x08105CDC, // c1SmallTime1aS_intro
            0x08105D60, // c1SmallTime1aS_mid
            0x08105E28, // c1SmallTime1bS_zip
            0x08105E88, // c1SmallTime1bS_uppercut
            0x08105EE4, // c1SmallTime1bS_end
            0x08105FB0, // c1SmallTime2aS_intro
            0x08106010, // c1SmallTime2aS_airPunch
            0x0810606C, // c2Frenzy1a_intro
            0x081060C4, // c2Frenzy1a_zip
            0x08106120, // c2Frenzy2a_intro
            0x0810617C, // c2Frenzy3V_intro
            0x0810621C, // c3Heist1a_intro
            0x08106274, // c3Heist2b_intro
            0x08106338, // c3Heist2b_end
            0x081063B4, // c3Heist3s_intro
            0x08106458, // c6Silvery3S_intro
            0x081064B8, // c3Heist2aS_sectionEnd
            0x08106514, // c4WorkOfArt1a_intro
            0x08106570, // c4WorkOfArt2a_intro
            0x08106614, // c4WorkofArt3a_intro
            0x08106694, // c5WorkOfArt1a_intro
            0x081066F4, // c5Unshielded3a_intro
            0x08106750, // c6Silvery1_intro
            0x081067A8, // c6Silvery1_comm
            0x08106824, // c6Silvery1_fire
            0x08106884, // c6Silvery2Sable_intro
            0x081068E4, // c6Silvery3Venom_intro
            0x08106940, // c7LabTests2a_intro
            0x0810699C, // c7LabTests2b_intro
            0x081069FC, // c7LabTests2b_deadEnd
            0x08106A60, // c7LabTests2aV_startSearch
            0x08106AC4, // c7LabTests2aV_loseSearch
            0x08106B28, // c7LabTests2bS_startSearch
            0x08106B8C, // c7LabTests2bS_loseSearch
            0x08106BF0, // c7LabTests2bS_sectionEnd
            0x08106C54, // c7LabTests2cV_loseSearch
            0x08106CB8, // c7LabTests2dS_loseSearch
            0x08106D0C, // venomTrain1
            0x08106D90, // spideyInst6
            0x08106DE4, // spideyInst7
            0x08106E38, // spideyInst8
            0x08106E8C, // spideyInst9
            0x08106EE4, // spideyInst10
            0x08106F40, // infoWebFluidDetail
            0x08106F98, // infoSwingDetail
            0x08106FF4, // infoCaptureDetail
            0x08107048, // TAMBlock
            0x081070A4, // c1SmallTime2aS_Exit
            0x081070F8, // TAMBombFail
            0x08107174, // TAMSableFail
            0x081071EC, // TAMCivilianDeathFail
            0x08107260, // TAMVenomEscapeFail
            0x081072D4, // TAMTraskEscapeFail
            0x08107344, // BankWallBlock
            0x0810739C, // SewerWallBlock
            0x081073F4, // RunningCommando
            0x0810E05C, // movie_license
            0x0810E0F0, // movie_intro
            0x0810E248, // movie_title
            0x0810E2C0, // movie_credits
            0x0810E33C, // c0Intro
            0x0810E770, // c1Intro
            0x0810E8BC, // c1Outro
            0x0810E96C, // c2Intro
            0x0810EAD0, // c2Outro
            0x0810EC88, // c3Intro
            0x0810EDC8, // c3Outro
            0x0810EF68, // c4Intro
            0x0810F0F0, // c4Outro
            0x0810F2D8, // c5Intro
            0x0810F364, // c5Outro
            0x0810F414, // c6Intro
            0x0810F4C4, // c6Outro
            0x0810F5B4, // c7aIntro
            0x0810F938, // c7aOutro
            0x0810FA04, // c7dIntro
            0x0810FB3C, // challengeEnd
            0x0810FC4C, // normalEnd
            0x0810FD3C, // c7Outro
            0x081103CC, // waitForFlcOrA
            0x08110454, // waitForPagedText
            0x081104C0, // c7Credits
        };
    }
    public class GBAVV_UltimateSpiderManUS_Manager : GBAVV_UltimateSpiderMan_Manager
    {
        public override string[] Languages => new string[]
        {
            "English"
        };

        public override uint[] GraphicsDataPointers => new uint[]
        {
            0x08361478,
            0x0836380C,
            0x0836A99C,
            0x08372E28,
            0x0837B518,
            0x08382458,
            0x0838A340,
            0x08393038,
            0x0839AAB4,
            0x083A3390,
            0x083AA67C,
            0x083AE9FC,
            0x083B2310,
            0x083BC760,
            0x083BCDE0,
            0x083CDD30,
            0x083D7644,
            0x083DAE0C,
            0x083DF914,
            0x083DFB30,
            0x083F430C,
            0x083F51A8,
        };

        public override uint[] ScriptPointers => new uint[]
        {
            0x0803673C, // script_waitForInputOrTime
            0x08064414, // offscreenSpawner
            0x08065A10, // breakableGeneric
            0x08065A8C, // breakableDoor
            0x08065AE4, // breakableCrate
            0x08065B4C, // breakableDoorSewer
            0x08065BDC, // bombIdle
            0x08065C70, // setDeletePending
            0x08065D60, // proximityTAMTargetIdle
            0x08065DEC, // gasTank
            0x08065E98, // breakableSolid
            0x08065F20, // breakableBGJumpable
            0x08065F90, // breakableBGJumpableOneHit
            0x08065FF0, // breakableBGNonJumpable
            0x08066060, // breakableDoorDie
            0x08066118, // breakableGenericHit
            0x080661D0, // IsFlipped
            0x080662CC, // NotFlipped
            0x080663D0, // breakableDoorSewerFlipFlop
            0x08066444, // bombDie
            0x080664E0, // gasTankHit
            0x08066548, // breakableSolidDie
            0x080665CC, // breakableBGJumpableDie
            0x08066610, // breakableGenericDie
            0x080666A0, // breakableGenericTakeHit
            0x08066700, // gasTankDie
            0x08066794, // gasTankTakeHit
            0x08066A00, // endOfSectionGeneric
            0x08066A6C, // endOfSectionIdle
            0x08066B1C, // endOfSectionActive
            0x08066B98, // endOfSectionDialog
            0x08066C00, // endOfSectionDone
            0x08066C54, // hydrantBurstLinked
            0x080681C8, // hydrantOpen
            0x08068254, // hydrantClosed
            0x080682D0, // hydrantWebbedLinked
            0x08068344, // hydrantOpenLinked
            0x080683A4, // hydrantDrippingLinked
            0x080683E4, // electricIdle
            0x08068428, // ceilingTurretSetup
            0x08068474, // wallTurretSetup
            0x080684D4, // hydrantWebbed
            0x0806853C, // hydrantWebbedLong
            0x080685C0, // takeHit
            0x080686B0, // hydrantTakeDamage
            0x08068748, // setDeletePending
            0x08068838, // turretDeath
            0x08068908, // ceilingTurretIdle
            0x08068984, // wallTurretIdle
            0x08068A58, // turretSetup
            0x08068AEC, // turretWebbed
            0x08068BA8, // "facingDown"
            0x08068CB8, // "facingUp"
            0x08068DA8, // ceilingTurretTryAttack
            0x08068E3C, // "facingRight"
            0x08068F50, // "facingLeft"
            0x08069040, // wallTurretTryAttack
            0x080690AC, // turretAttack
            0x08069200, // helpPromptIdle
            0x08069250, // helpPromptDialog
            0x0806A8F0, // setDeletePending
            0x0806A9E0, // spideyHealthLarge
            0x0806AA5C, // spideyHealthSmall
            0x0806AAD4, // spideyWebLarge
            0x0806AB4C, // spideyWebSmall
            0x0806ABFC, // spideyWebFluid1
            0x0806ACDC, // spideyWebFluid2
            0x0806ADBC, // spideyWebFluid3
            0x0806AE8C, // spideyWeb1
            0x0806AF08, // spideyWeb2
            0x0806AF88, // spideyAgility1
            0x0806B008, // spideyAgility2
            0x0806B0A4, // venomAdaptation
            0x0806B258, // venomFeed1
            0x0806B2D4, // venomFeed2
            0x0806B354, // venomToughness1
            0x0806B3D4, // venomToughness2
            0x0806B454, // venomCombat1
            0x0806B4D4, // venomCombat2
            0x0806B544, // venomInvincibility
            0x0806B5B8, // venomInvincibilitySmall
            0x0806B624, // helpWebFluid
            0x0806B6BC, // powerupPoof
            0x0806B760, // powerupIdle
            0x0806B7F4, // help
            0x0806B900, // help
            0x0806B9F0, // spideyWeb
            0x0806BAA4, // help
            0x0806BBB0, // help
            0x0806BCA0, // spideyAgility
            0x0806BD58, // help
            0x0806BE6C, // help
            0x0806BF5C, // venomFeed
            0x0806C020, // help
            0x0806C134, // help
            0x0806C224, // venomToughness
            0x0806C2DC, // help
            0x0806C3F0, // help
            0x0806C4E0, // venomCombat
            0x0806C560, // helpWeb1
            0x0806C5EC, // helpWeb2
            0x0806C67C, // helpAgility1
            0x0806C70C, // helpAgility2
            0x0806C790, // helpFeed1
            0x0806C814, // helpFeed2
            0x0806C89C, // helpToughness1
            0x0806C924, // helpToughness2
            0x0806C9A8, // helpCombat1
            0x0806CA2C, // helpCombat2
            0x0806CB70, // fight
            0x0806CD44, // intro
            0x0806CE74, // loop
            0x0806CF70, // fight
            0x0806D0A8, // fight
            0x0806EF50, // switchableDoor
            0x0806EFA4, // switchOnce
            0x0806EFF8, // laserBeamIdle
            0x0806F064, // laserSwitchBlueIdle
            0x0806F0BC, // laserSwitchYellowIdle
            0x0806F110, // laserSwitchRedIdle
            0x0806F158, // steamWallIdle
            0x0806F1C8, // steamValveIdle
            0x0806F218, // gasWallIdle
            0x0806F294, // gasValveIdle
            0x0806F2F4, // electricWallHorizontalIdle
            0x0806F378, // electricWallHorizontalHide
            0x0806F3E0, // electricWallVerticalIdle
            0x0806F46C, // electricWallVerticalHide
            0x0806F4DC, // generatorIdle
            0x0806F550, // electricSwitchIdle
            0x0806F5A8, // TAMElectricWallHide
            0x0806F5FC, // bankRubbleIdle
            0x0806F640, // solidIdle
            0x0806F688, // bankWallIdle
            0x0806F6D8, // museumWallIdle
            0x0806F74C, // switchableDoorOpen
            0x0806F7D8, // activateSwitchOnce
            0x0806F87C, // laserBeamDisable
            0x0806F8F8, // laserSwitchBlueDeactivate
            0x0806F9B0, // laserSwitchYellowDeactivate
            0x0806FA68, // laserSwitchRedDeactivate
            0x0806FB58, // setDeletePending
            0x0806FC48, // steamWallDisable
            0x0806FCB8, // steamValveSmash
            0x0806FD64, // gasWallDisable
            0x0806FDD4, // gasValveSmash
            0x0806FEA8, // electricWallHorizontalDisable
            0x0806FF18, // electricWallVerticalDisable
            0x0806FF94, // generatorSmash
            0x08070064, // electricSwitchDeactivate
            0x08070104, // TAMElectricWallIdle
            0x080701A4, // bankRubbleAppear
            0x0807021C, // solidAppear
            0x0807027C, // bankWallDie
            0x080702D4, // musuemWallDie
            0x0807034C, // TAMElectricWallDisable
            0x08071558, // venomEndJumpCreate
            0x080715C0, // setDeletePending
            0x080716B0, // venomStartRun
            0x08071750, // venomEndRunCreate
            0x08071794, // venomCreate
            0x080717F8, // venomTAMLevelVictimActivate
            0x08071888, // venomEndJump
            0x080719A0, // "endRunOffScreenTest"
            0x08071A90, // venomEndRun
            0x08071BD8, // "setLeftResponse"
            0x08071CFC, // "setRightResponse"
            0x08071DEC, // venomSetup
            0x08071ED0, // saved1
            0x08071FC8, // saved2
            0x080720CC, // venomTAMLevelVictim
            0x08072180, // showObject
            0x080721D4, // broadcastAndDie
            0x080722C4, // venomOffScreenTest
            0x0807230C, // venomHitResponseLeft
            0x080723FC, // venomHitResponseRight
            0x0807250C, // "fallingReaction"
            0x080725FC, // venomRunning
            0x08072660, // venomAirDowning
            0x080726E0, // "nextTest"
            0x080727D0, // venomAirUpping
            0x08072824, // venomStandardHitResponseBeginning
            0x080728D0, // venomFalling
            0x08072A08, // pause
            0x08072AF8, // carnageFight
            0x08072CC4, // intro
            0x08072DBC, // loop
            0x08072EB8, // fight
            0x08073238, // fight
            0x080738A4, // fight
            0x08073C94, // waitForPagedText
            0x080789B8, // victimRun
            0x08078A84, // saved1
            0x08078B7C, // saved2
            0x08078C94, // setDeletePending
            0x08078D84, // victim
            0x08078E6C, // TAMVictim
            0x08078EF8, // TAMVictimRescueImmediately
            0x08078F7C, // sableStartRun
            0x08079018, // SableRunCreate
            0x0807906C, // venomRunning
            0x08079130, // commandoRunCreate
            0x08079190, // shockerSpeaks
            0x08079200, // completeTAMAndDie
            0x080792F0, // victimOffScreenTest
            0x08079378, // "falling"
            0x08079474, // victimFallToGround
            0x08079514, // whimperingLoop
            0x08079604, // whimperUntilRescued
            0x08079668, // notSaved
            0x08079758, // whimperUntilSaved
            0x080797B4, // TAMvictimActivated
            0x08079898, // TAMVictimRescueImmediatelyActivated
            0x08079954, // whimper
            0x080799A8, // sableRun
            0x08079A80, // commandoRunning
            0x08079F30, // TAMTrigger
            0x08079FDC, // TAMTriggerBigger
            0x0807A078, // TAMWallSetup
            0x0807A0E8, // setDeletePending
            0x0807A1D8, // TAMTriggerActivate
            0x0807A218, // TAMWallDeath
            0x0807A268, // TAMWallLoop
            0x0807A2DC, // "waitUntilPlayerLeaves"
            0x0807A3CC, // TAMWallPlayerNear
            0x081054A0, // notFound
            0x0810575C, // waitForPagedText
            0x081057C4, // missing
            0x08105818, // rescueTAM
            0x0810586C, // failTAMBomb
            0x081058C4, // powerupWebFluid
            0x08105918, // powerWeb1
            0x0810596C, // powerWeb2
            0x081059C4, // powerAgility1
            0x08105A1C, // powerAgility2
            0x08105A74, // powerAdaptation
            0x08105AC8, // powerFeed1
            0x08105B1C, // powerFeed2
            0x08105B74, // powerToughness1
            0x08105BCC, // powerToughness2
            0x08105C24, // powerCombat1
            0x08105C7C, // powerCombat2
            0x08105CDC, // c1SmallTime1aS_intro
            0x08105D60, // c1SmallTime1aS_mid
            0x08105E28, // c1SmallTime1bS_zip
            0x08105E88, // c1SmallTime1bS_uppercut
            0x08105EE4, // c1SmallTime1bS_end
            0x08105FB0, // c1SmallTime2aS_intro
            0x08106010, // c1SmallTime2aS_airPunch
            0x0810606C, // c2Frenzy1a_intro
            0x081060C4, // c2Frenzy1a_zip
            0x08106120, // c2Frenzy2a_intro
            0x0810617C, // c2Frenzy3V_intro
            0x0810621C, // c3Heist1a_intro
            0x08106274, // c3Heist2b_intro
            0x08106338, // c3Heist2b_end
            0x081063B4, // c3Heist3s_intro
            0x08106458, // c6Silvery3S_intro
            0x081064B8, // c3Heist2aS_sectionEnd
            0x08106514, // c4WorkOfArt1a_intro
            0x08106570, // c4WorkOfArt2a_intro
            0x08106614, // c4WorkofArt3a_intro
            0x08106694, // c5WorkOfArt1a_intro
            0x081066F4, // c5Unshielded3a_intro
            0x08106750, // c6Silvery1_intro
            0x081067A8, // c6Silvery1_comm
            0x08106824, // c6Silvery1_fire
            0x08106884, // c6Silvery2Sable_intro
            0x081068E4, // c6Silvery3Venom_intro
            0x08106940, // c7LabTests2a_intro
            0x0810699C, // c7LabTests2b_intro
            0x081069FC, // c7LabTests2b_deadEnd
            0x08106A60, // c7LabTests2aV_startSearch
            0x08106AC4, // c7LabTests2aV_loseSearch
            0x08106B28, // c7LabTests2bS_startSearch
            0x08106B8C, // c7LabTests2bS_loseSearch
            0x08106BF0, // c7LabTests2bS_sectionEnd
            0x08106C54, // c7LabTests2cV_loseSearch
            0x08106CB8, // c7LabTests2dS_loseSearch
            0x08106D0C, // venomTrain1
            0x08106D90, // spideyInst6
            0x08106DE4, // spideyInst7
            0x08106E38, // spideyInst8
            0x08106E8C, // spideyInst9
            0x08106EE4, // spideyInst10
            0x08106F40, // infoWebFluidDetail
            0x08106F98, // infoSwingDetail
            0x08106FF4, // infoCaptureDetail
            0x08107048, // TAMBlock
            0x081070A4, // c1SmallTime2aS_Exit
            0x081070F8, // TAMBombFail
            0x08107174, // TAMSableFail
            0x081071EC, // TAMCivilianDeathFail
            0x08107260, // TAMVenomEscapeFail
            0x081072D4, // TAMTraskEscapeFail
            0x08107344, // BankWallBlock
            0x0810739C, // SewerWallBlock
            0x081073F4, // RunningCommando
            0x0810E05C, // movie_license
            0x0810E0F0, // movie_intro
            0x0810E248, // movie_title
            0x0810E2C0, // movie_credits
            0x0810E33C, // c0Intro
            0x0810E770, // c1Intro
            0x0810E8BC, // c1Outro
            0x0810E96C, // c2Intro
            0x0810EAD0, // c2Outro
            0x0810EC88, // c3Intro
            0x0810EDC8, // c3Outro
            0x0810EF68, // c4Intro
            0x0810F0F0, // c4Outro
            0x0810F2D8, // c5Intro
            0x0810F364, // c5Outro
            0x0810F414, // c6Intro
            0x0810F4C4, // c6Outro
            0x0810F5B4, // c7aIntro
            0x0810F938, // c7aOutro
            0x0810FA04, // c7dIntro
            0x0810FB3C, // challengeEnd
            0x0810FC4C, // normalEnd
            0x0810FD3C, // c7Outro
            0x081103CC, // waitForFlcOrA
            0x08110454, // waitForPagedText
            0x081104C0, // c7Credits
        };

        public override uint ObjTypesPointer => 0x08027164;
        public override int ObjTypesCount => 220;
    }
}