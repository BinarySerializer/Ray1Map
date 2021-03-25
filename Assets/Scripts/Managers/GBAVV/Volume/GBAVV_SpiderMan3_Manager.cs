using System.Collections.Generic;

namespace R1Engine
{
    public abstract class GBAVV_SpiderMan3_Manager : GBAVV_Volume_BaseManager
    {
        // Metadata
        public override int VolumesCount => 4;
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
            new LevInfo(0, 0, 0, "Chapter 1", "", "Search and Rescue"),
            new LevInfo(0, 0, 1, "Chapter 1", "", "Search and Rescue"),
            new LevInfo(0, 1, 0, "Chapter 1", "", "Goblin Hunt"),
            new LevInfo(0, 1, 1, "Chapter 1", "", "Goblin Hunt"),
            new LevInfo(0, 2, 0, "Chapter 1", "", "Spidey Saves"),
            new LevInfo(0, 2, 1, "Chapter 1", "", "Spidey Saves"),
            new LevInfo(0, 3, 0, "Chapter 1", "", "Sandstorm"),
            new LevInfo(0, 4, 0, "Chapter 1", "", "The Dark Side"),
            new LevInfo(1, 0, 0, "Chapter 2", "", "Subway Rumble"),
            new LevInfo(1, 0, 1, "Chapter 2", "", "Subway Rumble"),
            new LevInfo(1, 0, 2, "Chapter 2", "", "Subway Rumble"),
            new LevInfo(1, 1, 0, "Chapter 2", "", "Toxic Shock"),
            new LevInfo(1, 1, 1, "Chapter 2", "", "Toxic Shock"),
            new LevInfo(1, 2, 0, "Chapter 2", "", "Bombs Away"),
            new LevInfo(1, 2, 1, "Chapter 2", "", "Bombs Away"),
            new LevInfo(1, 3, 0, "Chapter 2", "", "'Copter Chase"),
            new LevInfo(1, 3, 1, "Chapter 2", "", "'Copter Chase"),
            new LevInfo(1, 3, 2, "Chapter 2", "", "'Copter Chase"),
            new LevInfo(2, 0, 0, "Chapter 4", "", "Court Disorder"),
            new LevInfo(2, 0, 1, "Chapter 4", "", "Court Disorder"),
            new LevInfo(2, 1, 0, "Chapter 4", "", "Follow That Limo"),
            new LevInfo(2, 1, 1, "Chapter 4", "", "Follow That Limo"),
            new LevInfo(2, 2, 0, "Chapter 4", "", "Kingpin's Mansion"),
            new LevInfo(2, 2, 1, "Chapter 4", "", "Kingpin's Mansion"),
            new LevInfo(2, 3, 0, "Chapter 4", "", "Totally Amped"),
            new LevInfo(2, 3, 1, "Chapter 4", "", "Totally Amped"),
            new LevInfo(3, 0, 0, "Chapter 5", "", "Sand Hassles"),
            new LevInfo(3, 0, 1, "Chapter 5", "", "Sand Hassles"),
            new LevInfo(3, 1, 0, "Chapter 5", "", "Quick Sand"),
            new LevInfo(3, 1, 1, "Chapter 5", "", "Quick Sand"),
            new LevInfo(3, 1, 2, "Chapter 5", "", "Quick Sand"),
        };

        // Obj types
        public override ObjTypeInit[] ObjTypeInitInfos => new ObjTypeInit[]
{
            new ObjTypeInit(-1, -1, null), // 0
            new ObjTypeInit(-1, -1, null), // 1
            new ObjTypeInit(-1, -1, null), // 2
            new ObjTypeInit(-1, -1, null), // 3
            new ObjTypeInit(-1, -1, null), // 4
            new ObjTypeInit(-1, -1, null), // 5
            new ObjTypeInit(-1, -1, null), // 6
            new ObjTypeInit(-1, -1, null), // 7
            new ObjTypeInit(17, 0, "spideyHealthSmall"), // 8
            new ObjTypeInit(17, 5, "webCapturePowerupMain"), // 9
            new ObjTypeInit(17, 6, "damagePowerupMain"), // 10
            new ObjTypeInit(17, 7, "fillRagePowerupMain"), // 11
            new ObjTypeInit(17, 0, "healthRegeneratingMain"), // 12
            new ObjTypeInit(17, 7, "fillRageRegeneratingMain"), // 13
            new ObjTypeInit(-1, -1, null), // 14
            new ObjTypeInit(0, 6, null), // 15
            new ObjTypeInit(7, 1, "breakableDoor"), // 16
            new ObjTypeInit(7, 2, "breakableEntranceStart"), // 17
            new ObjTypeInit(7, 8, "breakableReinforced"), // 18
            new ObjTypeInit(7, 9, "bombIdle"), // 19
            new ObjTypeInit(7, 12, "breakableCrate"), // 20
            new ObjTypeInit(7, 14, "breakableSolid"), // 21
            new ObjTypeInit(23, 0, "switchableDoor"), // 22
            new ObjTypeInit(23, 3, "lockableDoorRedMain"), // 23
            new ObjTypeInit(23, 3, "lockableDoorBlueMain"), // 24
            new ObjTypeInit(23, 3, "lockableDoorYellowMain"), // 25
            new ObjTypeInit(23, 4, "switchOnce"), // 26
            new ObjTypeInit(23, 4, "sandSprinklerSwitchSetup"), // 27
            new ObjTypeInit(19, 21, "valveSwitchSetup"), // 28
            new ObjTypeInit(23, 8, "bankRubbleIdle"), // 29
            new ObjTypeInit(17, 3, "keyRedMain"), // 30
            new ObjTypeInit(17, 3, "keyYellowMain"), // 31
            new ObjTypeInit(17, 3, "keyBlueMain"), // 32
            new ObjTypeInit(-1, -1, null), // 33
            new ObjTypeInit(-1, -1, null), // 34
            new ObjTypeInit(-1, -1, null), // 35
            new ObjTypeInit(-1, -1, null), // 36
            new ObjTypeInit(-1, -1, null), // 37
            new ObjTypeInit(-1, -1, null), // 38
            new ObjTypeInit(-1, -1, null), // 39
            new ObjTypeInit(-1, -1, null), // 40
            new ObjTypeInit(-1, -1, null), // 41
            new ObjTypeInit(2, 11, null), // 42
            new ObjTypeInit(1, 10, null), // 43
            new ObjTypeInit(3, 12, null), // 44
            new ObjTypeInit(3, 12, null), // 45
            new ObjTypeInit(3, 12, null), // 46
            new ObjTypeInit(4, 12, null), // 47
            new ObjTypeInit(2, 11, null), // 48
            new ObjTypeInit(1, 10, null), // 49
            new ObjTypeInit(5, 14, null), // 50
            new ObjTypeInit(6, 14, null), // 51
            new ObjTypeInit(2, 11, null), // 52
            new ObjTypeInit(2, 11, null), // 53
            new ObjTypeInit(5, 14, null), // 54
            new ObjTypeInit(-1, -1, null), // 55
            new ObjTypeInit(-1, -1, null), // 56
            new ObjTypeInit(-1, -1, null), // 57
            new ObjTypeInit(-1, -1, null), // 58
            new ObjTypeInit(21, 6, "cameraTargetStart"), // 59
            new ObjTypeInit(-1, -1, null), // 60
            new ObjTypeInit(-1, -1, null), // 61
            new ObjTypeInit(-1, -1, null), // 62
            new ObjTypeInit(-1, -1, null), // 63
            new ObjTypeInit(-1, -1, null), // 64
            new ObjTypeInit(-1, -1, null), // 65
            new ObjTypeInit(-1, -1, null), // 66
            new ObjTypeInit(-1, -1, null), // 67
            new ObjTypeInit(16, 0, "helpPromptIdle"), // 68
            new ObjTypeInit(16, 1, "victim"), // 69
            new ObjTypeInit(16, 1, "victim"), // 70
            new ObjTypeInit(16, 1, "victim"), // 71
            new ObjTypeInit(16, 1, "victim"), // 72
            new ObjTypeInit(16, 1, "TAMVictimRescueImmediately"), // 73
            new ObjTypeInit(16, 1, "TAMVictimRescueImmediately"), // 74
            new ObjTypeInit(16, 1, "TAMVictimRescueImmediately"), // 75
            new ObjTypeInit(16, 1, "TAMVictimRescueImmediately"), // 76
            new ObjTypeInit(16, 1, "victimCollapseSetup"), // 77
            new ObjTypeInit(16, 1, "victimCollapseSetup"), // 78
            new ObjTypeInit(16, 1, "victimCollapseSetup"), // 79
            new ObjTypeInit(16, 1, "victimCollapseSetup"), // 80
            new ObjTypeInit(14, 1, "TAMTrigger"), // 81
            new ObjTypeInit(14, 4, "TAMTrigger"), // 82
            new ObjTypeInit(14, 3, "TAMTrigger"), // 83
            new ObjTypeInit(14, 5, "TAMTriggerBigger"), // 84
            new ObjTypeInit(14, 4, "proximityTAMTargetIdle"), // 85
            new ObjTypeInit(13, 11, "electricIdle"), // 86
            new ObjTypeInit(13, 8, "electricIdle"), // 87
            new ObjTypeInit(13, 0, "electricWallHorizontalIdle"), // 88
            new ObjTypeInit(13, 1, "electricWallVerticalIdle"), // 89
            new ObjTypeInit(23, 4, "electricSwitchIdle"), // 90
            new ObjTypeInit(13, 0, "electricWallHorizontalHide"), // 91
            new ObjTypeInit(13, 1, "electricWallVerticalHide"), // 92
            new ObjTypeInit(13, 12, null), // 93
            new ObjTypeInit(13, 13, null), // 94
            new ObjTypeInit(13, 14, null), // 95
            new ObjTypeInit(13, 15, null), // 96
            new ObjTypeInit(13, 16, "chemicalSetup"), // 97
            new ObjTypeInit(13, 17, "carKillerScript"), // 98
            new ObjTypeInit(18, 0, null), // 99
            new ObjTypeInit(13, 19, "trainMain"), // 100
            new ObjTypeInit(21, 0, "doorSpawnerStart"), // 101
            new ObjTypeInit(21, 4, null), // 102
            new ObjTypeInit(21, 4, "offscreenSpawner"), // 103
            new ObjTypeInit(21, 0, "doorSpawnerStart"), // 104
            new ObjTypeInit(21, 0, "doorSpawnerStart"), // 105
            new ObjTypeInit(21, 6, "missleReticleStart"), // 106
            new ObjTypeInit(21, 4, "missleSpawnerStart"), // 107
            new ObjTypeInit(12, 0, "goblinBossSetup"), // 108
            new ObjTypeInit(12, 0, "goblinFlyBySetup"), // 109
            new ObjTypeInit(13, 18, "goblinPoliceVanScript"), // 110
            new ObjTypeInit(20, 1, "sandmanChaseMain"), // 111
            new ObjTypeInit(7, 6, "sandmanWallMain"), // 112
            new ObjTypeInit(20, 1, null), // 113
            new ObjTypeInit(-1, -1, null), // 114
            new ObjTypeInit(20, 11, "sandmanFistMain"), // 115
            new ObjTypeInit(19, 2, "sandmanSprayerSetup"), // 116
            new ObjTypeInit(19, 21, "sandmanSwitchSetup"), // 117
            new ObjTypeInit(7, 6, "sandmanWeakWallMain"), // 118
            new ObjTypeInit(7, 6, "sandmanUnbreakableWallMain"), // 119
            new ObjTypeInit(20, 1, "sandmanHazardDummyScript"), // 120
            new ObjTypeInit(13, 21, "dropOffVanMain"), // 121
            new ObjTypeInit(13, 22, "kingpinLimoMain"), // 122
            new ObjTypeInit(13, 22, "kingpinLimoChase"), // 123
            new ObjTypeInit(23, 0, "kingpinDoorSetup"), // 124
            new ObjTypeInit(19, 16, "madBomberFloorMain"), // 125
            new ObjTypeInit(15, 0, "madBomberSetup"), // 126
            new ObjTypeInit(9, 2, null), // 127
            new ObjTypeInit(9, 21, "electroGeneratorSetup"), // 128
            new ObjTypeInit(9, 12, "electroShieldSetup"), // 129
            new ObjTypeInit(-1, -1, null), // 130
            new ObjTypeInit(24, 18, "noiseMakerMain"), // 131
            new ObjTypeInit(7, 8, "venomWallMain"), // 132
            new ObjTypeInit(19, 16, "fallingCeilingMain"), // 133
            new ObjTypeInit(24, 21, "noiseMakerDropperMain"), // 134
            new ObjTypeInit(-1, -1, null), // 135
            new ObjTypeInit(10, 0, "endOfSectionGeneric"), // 136
            new ObjTypeInit(10, 0, "endOfSectionGeneric"), // 137
            new ObjTypeInit(10, 0, "endOfSectionDialogScript"), // 138
            new ObjTypeInit(8, 4, null), // 139
            new ObjTypeInit(8, 5, null), // 140
            new ObjTypeInit(8, 6, null), // 141
            new ObjTypeInit(8, 7, null), // 142
            new ObjTypeInit(21, 0, null), // 143
            new ObjTypeInit(21, 0, null), // 144
            new ObjTypeInit(21, 0, null), // 145
            new ObjTypeInit(18, 1, null), // 146
            new ObjTypeInit(11, 0, "copterCutsceneSetup"), // 147
            new ObjTypeInit(11, 0, "copterSetup"), // 148
            new ObjTypeInit(19, 0, "hydrantOpen"), // 149
            new ObjTypeInit(19, 2, "hydrantClosed"), // 150
            new ObjTypeInit(19, 4, "sprinklerOpen"), // 151
            new ObjTypeInit(19, 6, "sprinklerDrippingLinked"), // 152
            new ObjTypeInit(19, 4, "sprinklerOpenLinked"), // 153
            new ObjTypeInit(19, 3, "sprinklerClosedDrippingLinked"), // 154
            new ObjTypeInit(19, 3, "sprinklerClosed"), // 155
            new ObjTypeInit(19, 3, "sprinklerClosedLinked"), // 156
            new ObjTypeInit(19, 3, "sandSprinklerSetup"), // 157
            new ObjTypeInit(13, 13, "extinguishableFire"), // 158
            new ObjTypeInit(19, 10, "fanBladeSetup"), // 159
            new ObjTypeInit(19, 13, "fanBladeSetup"), // 160
            new ObjTypeInit(19, 17, "securityCameraBegin"), // 161
            new ObjTypeInit(19, 19, "zipPostGeneric"), // 162
            new ObjTypeInit(13, 18, "policevanScript"), // 163
            new ObjTypeInit(13, 17, "carPlatformScript"), // 164
            new ObjTypeInit(19, 14, "platformGenericMain"), // 165
            new ObjTypeInit(19, 16, "breakablePlatformGenericMain"), // 166
            new ObjTypeInit(19, 16, "tamBreakablePlatform"), // 167
            new ObjTypeInit(0, 8, null), // 168
            new ObjTypeInit(0, 7, null), // 169
        };
    }
    public class GBAVV_SpiderMan3EU_Manager : GBAVV_SpiderMan3_Manager
    {
        public override string[] Languages => new string[]
        {
            "English",
        };

        public override uint[] GraphicsDataPointers => new uint[]
        {
            0x081BE4A4,
            0x081BEBBC,
            0x081C2BA4,
            0x081C7884,
            0x081CBC48,
            0x081CF324,
            0x081D326C,
            0x081D6C30,
            0x081D82A8,
            0x081D8E88,
            0x081DD73C,
            0x081DD9A4,
            0x081DDB94,
            0x081DFA4C,
            0x081E2340,
            0x081E6554,
            0x081E85E4,
            0x081E9A78,
            0x081EF57C,
            0x081F0010,
            0x081F24E0,
            0x081F609C,
            0x081F6644,
            0x08207864,
            0x08208280,
        };

        public override uint[] ScriptPointers => new uint[]
        {
            0x08042B28, // script_waitForInputOrTime
            0x0806A70C, // copterSetup
            0x0806A794, // copterCutsceneSetup
            0x0806AA04, // setDeletePending
            0x0806AAF4, // "done with waypoints"
            0x0806ACE4, // copterNextWayPoint1
            0x08070498, // doorSpawnerStart
            0x08070520, // offscreenSpawner
            0x0807057C, // missleReticleStart
            0x080705D4, // missleSpawnerStart
            0x08070664, // doorSpawnerTriggered
            0x080706DC, // doorSpawnerOpenClose
            0x08070788, // "spawnAI"
            0x08070878, // spawnAIoffscreen
            0x08070950, // setDeletePending
            0x08070A40, // missleReticleIdle
            0x08070AF8, // missleSpawnerActivate
            0x08071440, // breakableReinforced
            0x080714E8, // breakableDoor
            0x08071560, // breakableCrate
            0x08071604, // bombIdle
            0x080716B0, // setDeletePending
            0x080717A0, // proximityTAMTargetIdle
            0x0807181C, // breakableSolid
            0x0807189C, // breakableEntranceStart
            0x08071918, // breakableDoorDie
            0x08071A3C, // bombExplode
            0x08071AD4, // bombDie
            0x08071B78, // breakableSolidDie
            0x08071C14, // breakableEntranceDie
            0x08071CA4, // bombDetachHitScripts
            0x08071E2C, // electroShieldTurnOn
            0x08071EA0, // electroShieldTurnOff
            0x0807204C, // electroGeneratorSetup
            0x08072120, // electroShieldSetup
            0x08072434, // endOfSectionGeneric
            0x080724A0, // endOfSectionDialogScript
            0x08072568, // endOfSectionActive
            0x080725FC, // endOfSectionIdle
            0x0807267C, // endOfSectionDone
            0x080726E0, // endOfSectionDialog
            0x08072794, // webCaptureWait
            0x08072818, // doNothing
            0x08072F08, // goblinFlyBySetup
            0x08072F88, // "move"
            0x080731BC, // "done with waypoints"
            0x080733AC, // goblinNextWayPoint1
            0x08073438, // "justtriggered"
            0x08073548, // "delete"
            0x08073738, // goblinNextWayPoint2
            0x08073BEC, // "electricLoop"
            0x08073D1C, // chemicalSetup
            0x08073D6C, // trainMain
            0x08073E8C, // "shakeLoop"
            0x0807409C, // trainTurnOn
            0x08074108, // setDeletePending
            0x080741F8, // trainTurnOff
            0x080743CC, // helpPromptIdle
            0x08074430, // cameraTargetStart
            0x0807448C, // helpPromptDialog
            0x08074578, // "endOfLinks"
            0x0807467C, // "gotoNextWayPoint"
            0x0807476C, // "showlinkedwayPoints"
            0x08074910, // setDeletePending
            0x08074A5C, // moveToNextTarget
            0x08074AE8, // "waiting"
            0x08074BD8, // waitingOnTarget
            0x08074C4C, // "changeTarget"
            0x08074D3C, // "traveling"
            0x08074E2C, // travelingToTarget
            0x080754BC, // dropOffVanWaypoint5Reached
            0x080759B8, // "move"
            0x08075C3C, // kingpinDoorSetup
            0x08075DC8, // "changeTarget"
            0x08075EB8, // "traveling"
            0x08075FA8, // travelingToTarget
            0x08076000, // "waiting"
            0x080760F0, // waitingOnTarget1
            0x08076150, // "waiting"
            0x08076240, // waitingOnTarget
            0x080768E8, // madBomberFloorMain
            0x080769E0, // setDeletePending
            0x08076AD0, // madBomberFloorDestroy
            0x08077608, // setDeletePending
            0x080776F8, // "collect powerup"
            0x080777E8, // "healthLoop"
            0x080778D8, // spideyHealthSmall
            0x08077944, // keyRedMain
            0x08077A28, // keyBlueMain
            0x08077B10, // keyYellowMain
            0x08077C24, // "collect webPowerup"
            0x08077D14, // "webCapture loop"
            0x08077E04, // webCapturePowerupMain
            0x08077E98, // "collect damagePowerup"
            0x08077F88, // "damage loop"
            0x08078078, // damagePowerupMain
            0x08078120, // "collect rageFill"
            0x08078210, // "fillRage loop"
            0x08078300, // fillRagePowerupMain
            0x08078354, // "collect powerup"
            0x08078444, // "healthLoop"
            0x08078534, // healthRegeneratingMain
            0x0807858C, // "collect rageFill"
            0x0807867C, // "fillRage loop"
            0x0807876C, // fillRageRegeneratingMain
            0x080787B4, // powerupPoof
            0x08078864, // powerupIdle
            0x08078B30, // "waitOnKey"
            0x08078C20, // "visible"
            0x08078E14, // "traveling"
            0x08079160, // "waitOnKey"
            0x08079250, // "visible"
            0x08079430, // "traveling"
            0x08079780, // "waitOnKey"
            0x08079870, // "visible"
            0x08079A50, // "traveling"
            0x08079BBC, // fanBladeSetup
            0x0807AE98, // platformGenericMoveLeft
            0x0807AF10, // platformGenericMoveRight
            0x0807AF7C, // platformGenericMoveUp
            0x0807AFE8, // platformGenericMoveDown
            0x0807B4D0, // zipPostReset
            0x0807BAD4, // fanBladeSlowdownHit
            0x0807BF58, // extinguishableFire
            0x0807C0C0, // breakablePlatformGenericMain
            0x0807C178, // tamBreakablePlatform
            0x0807C2D0, // "SpawnAI"
            0x0807C3C0, // "securityCamMainLoop"
            0x0807C564, // "webLineExists"
            0x0807C654, // zipPostWebbed
            0x0807C710, // sandSprinklerTurnOn
            0x0807C848, // carKillerScript
            0x0807C994, // setDeletePending
            0x0807CA84, // extinguishTheFire
            0x0807CB10, // breakablePlatformGenericCrumble
            0x0807CBA8, // tamCompleteCrumble
            0x0807CC0C, // tamTimeoutCrumble
            0x0807CCC8, // securityCamWebbed
            0x0807CD7C, // "waitLoop"
            0x0807CE6C, // zipPostWaitUntilNoWeb
            0x0807CEA8, // zipPostHit
            0x0807D2B8, // setDeletePending
            0x0807DC3C, // sandmanSwitchOn
            0x0807E104, // "runDistance"
            0x0807E1F4, // "isWalking"
            0x0807E32C, // "stopRunDistance"
            0x0807E41C, // "isRunning"
            0x0807E50C, // "isShowing"
            0x0807E5FC, // "sandmanChaseMainLoop"
            0x0807E6EC, // sandmanChaseMain
            0x0807E7B0, // sandmanWallDetachHitScripts
            0x0807E80C, // sandmanWallDamage
            0x0807E9A4, // sandmanUnbreakableWallAttachHitScripts
            0x0807EA08, // sandmanUnbreakableWallDetachHitScripts
            0x0807EBB0, // sandmanSprayerSetup
            0x0807EC58, // sandmanSwitchAttachHitScripts
            0x0807ECCC, // sandmanSwitchDetachHitScripts
            0x0807ED34, // sandmanSwitchSetup
            0x0807EDE4, // "loopUntilPauseFrame"
            0x0807EED4, // sandmanHazardDummyScript
            0x0807F000, // sandmanChaseForwardAttack
            0x0807F0A8, // sandmanChasePoundAttack
            0x0807F174, // "isShowing"
            0x0807F390, // "triggerDialog"
            0x0807F480, // sandmanChaseTriggered
            0x0807F51C, // sandmanWallAttachHitScripts
            0x0807F57C, // sandmanWallTriggered
            0x0807F6AC, // sandmanSprayerTurnOn
            0x0807F744, // sandmanSprayerBreak
            0x0807F810, // "right"
            0x0807F908, // "left"
            0x0807F9F8, // sandmanChaseWalk
            0x0807FCD8, // electricWallVerticalEnable
            0x08080C70, // sandSprinklerSwitchSetup
            0x08080FA0, // switchableDoor
            0x08080FFC, // switchOnce
            0x08081098, // electricWallHorizontalIdle
            0x0808111C, // electricWallHorizontalHide
            0x08081198, // electricWallVerticalDisable
            0x0808122C, // electricWallVerticalIdle
            0x080812B8, // electricWallVerticalHide
            0x0808132C, // electricSwitchIdle
            0x08081390, // TAMElectricWallHide
            0x080813E4, // bankRubbleIdle
            0x080814CC, // "hasKey"
            0x08081618, // "noDialog"
            0x08081708, // "noKey"
            0x080817F8, // "hitVictim"
            0x080818E8, // "mainLoop"
            0x080819D8, // lockableDoorRedMain
            0x08081A6C, // "hasKey"
            0x08081B98, // "noDialog"
            0x08081C88, // "noKey"
            0x08081D78, // "hitVictim"
            0x08081E68, // "mainLoop"
            0x08081F58, // lockableDoorBlueMain
            0x08081FEC, // "hasKey"
            0x08082118, // "noDialog"
            0x08082208, // "noKey"
            0x080822F8, // "hitVictim"
            0x080823E8, // "mainLoop"
            0x080824D8, // lockableDoorYellowMain
            0x08082560, // sandSprinklerSwitchTurnOn
            0x08082630, // valveSwitchSetup
            0x080826BC, // switchableDoorOpen
            0x0808273C, // activateSwitchOnce
            0x080827E8, // setDeletePending
            0x080828D8, // electricWallHorizontalDisable
            0x08082954, // electricSwitchDeactivate
            0x080829F4, // TAMElectricWallIdle
            0x08082A8C, // bankRubbleAppear
            0x08082B60, // valveSwitchActivate
            0x08082C14, // TAMElectricWallDisable
            0x08083088, // noiseMakerDropperWaypoint1
            0x080830E8, // noiseMakerDropperWaypoint2
            0x080834FC, // noiseMakerSwitchTakeHit
            0x080836AC, // "normalWebbed"
            0x0808379C, // noiseMakerSwitchWebbed
            0x0808381C, // noiseMakerSwitchTurnOff
            0x08083A88, // setDeletePending
            0x08083B78, // noiseMakerMain
            0x08083C38, // venomWallMain
            0x08083C9C, // fallingCeilingMain
            0x08083D18, // noiseMakerDropperDrop
            0x08083DBC, // noiseMakerDropperMain
            0x08083E68, // noiseMakerSwitchAttachScripts
            0x08083ED0, // noiseMakerSwitchDetachScripts
            0x08083F5C, // noiseMakerSwitchAttachDamageScripts
            0x0808406C, // "destroy"
            0x0808415C, // venomWallDestroy
            0x080841D0, // fallingCeilingFall
            0x080842C0, // noiseMakerDropperActivate
            0x08084348, // noiseMakerDropperDeactivate
            0x080845F8, // waitForPagedText
            0x08085E54, // saved1
            0x08085F4C, // saved2
            0x08086064, // setDeletePending
            0x08086318, // whimperingLoop
            0x080864FC, // "falling"
            0x080865F8, // victimFallToGround
            0x0808668C, // whimperUntilRescued
            0x080866E0, // TAMVictimRescueImmediatelyActivated
            0x08086790, // "isvisible"
            0x08086880, // whimper
            0x080868E0, // victimCollapseWebbed
            0x08086940, // victimCollapsePull
            0x08086A6C, // disablePull
            0x08086E24, // TAMTrigger
            0x08086EC4, // TAMTriggerBigger
            0x08086F70, // setDeletePending
            0x08087060, // TAMTriggerActivate
            0x080F3ABC, // notFound
            0x080F3E6C, // waitForPagedText
            0x080F3ED4, // missing
            0x080F3F1C, // infoMb1a1
            0x080F3F70, // infoMb1a2
            0x080F3FC4, // infoMb1b1
            0x080F4018, // infoMv3a1
            0x080F409C, // infoMv3a2
            0x080F40F0, // infoMb2a1
            0x080F4144, // infoMb2b1
            0x080F4198, // infoMb2b2
            0x080F41EC, // infoMb2c1
            0x080F4240, // infoMb4b1
            0x080F4294, // infoMb5a1
            0x080F42E8, // infoMb5b1
            0x080F433C, // infoMb5c1
            0x080F43C0, // infoKp1a1
            0x080F4414, // infoKp1b1
            0x080F4468, // infoKp4c1
            0x080F4504, // infoKp4c4
            0x080F4570, // infoMv6a1
            0x080F460C, // infoMv6b1
            0x080F4664, // infoCrawlDetail
            0x080F46B0, // infoSwingDetail
            0x080F4700, // infoSwitchDetail
            0x080F4750, // infoCompassDetail
            0x080F47A4, // infoWebSprinklerDetail
            0x080F47F4, // infoWebFanDetail
            0x080F4844, // infoTimeTamDetail
            0x080F4890, // infoZipDetail
            0x080F48D8, // TAMBlock
            0x080F4920, // infoWebPull
            0x080F496C, // infoWebUppercut
            0x080F49B8, // infoBlackSuit1
            0x080F4A04, // infoBlackSuit4
            0x080F4A54, // infoLoseBlackSuit
            0x080F4AA0, // infoStronger
            0x080F4AF8, // infoDashAttack
            0x080F4B44, // sm3EndOfSand
            0x080F4BC4, // infoWebSecurityCams
            0x080F4C1C, // sm3LimoFight
            0x080F4C78, // infoKingpinChase1
            0x080F4CD0, // sm3LimoChase
            0x080F4D2C, // sm3KingpinOffice
            0x080F4D88, // ingameKP3outro01
            0x080F4EB8, // sm3ElectroTip
            0x080F4F10, // infoElectroKill
            0x080F4F68, // sm3BombDisable
            0x080F4FB0, // failTAMBomb
            0x080F5004, // TAMCivilianDeathFail
            0x080F5070, // TAMfailtoreachSandman
            0x080F50D4, // sm3MoreBombs
            0x080F5120, // sm3SingleBomb
            0x080F5170, // sm3MoreCivilians
            0x080F51BC, // sm3MoreCameras
            0x080F5208, // infoGoblinText1
            0x080F5260, // infoGoblinText2
            0x080F52B8, // infoGoblinText3
            0x080F5314, // infoKickModifier
            0x080F5360, // infoDodgeMove1
            0x080F53AC, // infoDodgeMove2
            0x080F53F8, // infoDodgeMove3
            0x080F5440, // infoGoblin1
            0x080F5498, // infoCollapse
            0x080F54E4, // infoRescueFail
            0x080F5530, // infoEscapeFail
            0x080F557C, // infoPullSave
            0x080F55CC, // infoLosePowerups
            0x080F5628, // infoSandmanIntro
            0x080F5678, // infoSandwallIntro
            0x080F56C4, // infoDropDown
            0x080F570C, // infoWebLeap
            0x080F5758, // infoWebPost2
            0x080F57A4, // infoAvoidTrain
            0x080F57F0, // sm3WaterMain
            0x080F5838, // TAMBombFail
            0x080F58A8, // infoSpiderMode
            0x080F5908, // infoSpideyVSSandman1
            0x080F5968, // infoSpideyVSSandman2
            0x080F59C8, // infoSpideyVSSandman3
            0x080F5A28, // infoSpideyVSSandman4
            0x080F5A88, // infoSpideyVSSandman5
            0x080F5AE8, // infoSpideyVSSandman6
            0x080F5B48, // infoSpideyVSSandman10
            0x080F5BA8, // infoSpideyVSSandman11
            0x080F5C08, // infoSpideyVSSandman12
            0x080F5C68, // infoSpideyVSSandman13
            0x080F5CC8, // infoSpideyVSSandman14
            0x080F5D24, // sm3SubwayComplete
            0x080F5D70, // sm3TrainGotAway
            0x080F5DBC, // sm3FactoryIntro
            0x080F5E18, // sm3ExtinguishFire
            0x080F5E74, // sm3FindCityBombs
            0x080F5ECC, // infoRocketWall
            0x080F5F1C, // infoAnotherWindow
            0x080F5F64, // infoWebBomb
            0x080F5FB0, // infoMadBomber
            0x080F6004, // infoVenom1
            0x080F6058, // infoVenom2
            0x080F60A0, // infoVenom3
            0x080F60E8, // infoVenom4
            0x080F6138, // infoUnlockWebPull
            0x080F6198, // infoUnlockAreaAttacks
            0x080F61F4, // infoUnlockUppercuts
            0x080F6250, // infoUnlockBlacksuit
            0x080F62D4, // waitForMapDialog
            0x080FD8F4, // movie_licenseScreenBootup
            0x080FD93C, // movie_licenseCredits
            0x080FD9B8, // "waiting"
            0x080FDAA8, // health_warning
            0x080FDB64, // movie_intro
            0x080FDC2C, // movie_title
            0x080FDCAC, // movie_credits
            0x080FDD74, // mb_1a_intro
            0x080FDE50, // "madbomber first"
            0x080FDF50, // "kingpin first"
            0x080FE040, // mb_2a_intro
            0x080FE0D0, // mb_3a_intro
            0x080FE1C0, // mb_5a_intro
            0x080FE328, // mv_2a_intro
            0x080FE3DC, // mv_4a_intro
            0x080FE490, // mv_5a_intro
            0x080FE544, // mv_6a_intro
            0x080FE738, // "kingpin first"
            0x080FE828, // "madbomber first"
            0x080FE918, // kp_1a_intro
            0x080FE99C, // kp_1a_outro
            0x080FEA2C, // kp_2_intro
            0x080FEAA4, // kp_3_intro
            0x080FEB24, // mb_5c_boss_outro
            0x080FEBE0, // mv_1b_boss_outro
            0x080FECD0, // mv_2_outro
            0x080FED8C, // mv_3a_boss_outro
            0x080FEE7C, // mv_4_outro
            0x080FF094, // mv_5c_boss_outro
            0x080FF10C, // mv_6b_outro
            0x080FF1BC, // mv_6c_boss_outro
            0x080FF298, // movie_license
            0x080FF31C, // waitForPagedText
        };
    }
    public class GBAVV_SpiderMan3US_Manager : GBAVV_SpiderMan3_Manager
    {
        public override string[] Languages => new string[]
        {
            "English"
        };

        public override uint[] GraphicsDataPointers => new uint[]
        {
            0x081BE4E0,
            0x081BEBF8,
            0x081C2BE0,
            0x081C78C0,
            0x081CBC84,
            0x081CF360,
            0x081D32A8,
            0x081D6C6C,
            0x081D82E4,
            0x081D8EC4,
            0x081DD778,
            0x081DD9E0,
            0x081DDBD0,
            0x081DFA88,
            0x081E237C,
            0x081E6590,
            0x081E8620,
            0x081E9AB4,
            0x081EF5B8,
            0x081F004C,
            0x081F251C,
            0x081F60D8,
            0x081F6680,
            0x082078A0,
            0x082082BC,
        };

        public override uint[] ScriptPointers => new uint[]
        {
            0x08042B28, // script_waitForInputOrTime
            0x0806A70C, // copterSetup
            0x0806A794, // copterCutsceneSetup
            0x0806AA04, // setDeletePending
            0x0806AAF4, // "done with waypoints"
            0x0806ACE4, // copterNextWayPoint1
            0x08070498, // doorSpawnerStart
            0x08070520, // offscreenSpawner
            0x0807057C, // missleReticleStart
            0x080705D4, // missleSpawnerStart
            0x08070664, // doorSpawnerTriggered
            0x080706DC, // doorSpawnerOpenClose
            0x08070788, // "spawnAI"
            0x08070878, // spawnAIoffscreen
            0x08070950, // setDeletePending
            0x08070A40, // missleReticleIdle
            0x08070AF8, // missleSpawnerActivate
            0x08071440, // breakableReinforced
            0x080714E8, // breakableDoor
            0x08071560, // breakableCrate
            0x08071604, // bombIdle
            0x080716B0, // setDeletePending
            0x080717A0, // proximityTAMTargetIdle
            0x0807181C, // breakableSolid
            0x0807189C, // breakableEntranceStart
            0x08071918, // breakableDoorDie
            0x08071A3C, // bombExplode
            0x08071AD4, // bombDie
            0x08071B78, // breakableSolidDie
            0x08071C14, // breakableEntranceDie
            0x08071CA4, // bombDetachHitScripts
            0x08071E2C, // electroShieldTurnOn
            0x08071EA0, // electroShieldTurnOff
            0x0807204C, // electroGeneratorSetup
            0x08072120, // electroShieldSetup
            0x08072434, // endOfSectionGeneric
            0x080724A0, // endOfSectionDialogScript
            0x08072568, // endOfSectionActive
            0x080725FC, // endOfSectionIdle
            0x0807267C, // endOfSectionDone
            0x080726E0, // endOfSectionDialog
            0x08072794, // webCaptureWait
            0x08072818, // doNothing
            0x08072F08, // goblinFlyBySetup
            0x08072F88, // "move"
            0x080731BC, // "done with waypoints"
            0x080733AC, // goblinNextWayPoint1
            0x08073438, // "justtriggered"
            0x08073548, // "delete"
            0x08073738, // goblinNextWayPoint2
            0x08073BEC, // "electricLoop"
            0x08073D1C, // chemicalSetup
            0x08073D6C, // trainMain
            0x08073E8C, // "shakeLoop"
            0x0807409C, // trainTurnOn
            0x08074108, // setDeletePending
            0x080741F8, // trainTurnOff
            0x080743CC, // helpPromptIdle
            0x08074430, // cameraTargetStart
            0x0807448C, // helpPromptDialog
            0x08074578, // "endOfLinks"
            0x0807467C, // "gotoNextWayPoint"
            0x0807476C, // "showlinkedwayPoints"
            0x08074910, // setDeletePending
            0x08074A5C, // moveToNextTarget
            0x08074AE8, // "waiting"
            0x08074BD8, // waitingOnTarget
            0x08074C4C, // "changeTarget"
            0x08074D3C, // "traveling"
            0x08074E2C, // travelingToTarget
            0x080754BC, // dropOffVanWaypoint5Reached
            0x080759B8, // "move"
            0x08075C3C, // kingpinDoorSetup
            0x08075DC8, // "changeTarget"
            0x08075EB8, // "traveling"
            0x08075FA8, // travelingToTarget
            0x08076000, // "waiting"
            0x080760F0, // waitingOnTarget1
            0x08076150, // "waiting"
            0x08076240, // waitingOnTarget
            0x080768E8, // madBomberFloorMain
            0x080769E0, // setDeletePending
            0x08076AD0, // madBomberFloorDestroy
            0x08077608, // setDeletePending
            0x080776F8, // "collect powerup"
            0x080777E8, // "healthLoop"
            0x080778D8, // spideyHealthSmall
            0x08077944, // keyRedMain
            0x08077A28, // keyBlueMain
            0x08077B10, // keyYellowMain
            0x08077C24, // "collect webPowerup"
            0x08077D14, // "webCapture loop"
            0x08077E04, // webCapturePowerupMain
            0x08077E98, // "collect damagePowerup"
            0x08077F88, // "damage loop"
            0x08078078, // damagePowerupMain
            0x08078120, // "collect rageFill"
            0x08078210, // "fillRage loop"
            0x08078300, // fillRagePowerupMain
            0x08078354, // "collect powerup"
            0x08078444, // "healthLoop"
            0x08078534, // healthRegeneratingMain
            0x0807858C, // "collect rageFill"
            0x0807867C, // "fillRage loop"
            0x0807876C, // fillRageRegeneratingMain
            0x080787B4, // powerupPoof
            0x08078864, // powerupIdle
            0x08078B30, // "waitOnKey"
            0x08078C20, // "visible"
            0x08078E14, // "traveling"
            0x08079160, // "waitOnKey"
            0x08079250, // "visible"
            0x08079430, // "traveling"
            0x08079780, // "waitOnKey"
            0x08079870, // "visible"
            0x08079A50, // "traveling"
            0x08079BBC, // fanBladeSetup
            0x0807AE98, // platformGenericMoveLeft
            0x0807AF10, // platformGenericMoveRight
            0x0807AF7C, // platformGenericMoveUp
            0x0807AFE8, // platformGenericMoveDown
            0x0807B4D0, // zipPostReset
            0x0807BAD4, // fanBladeSlowdownHit
            0x0807BF58, // extinguishableFire
            0x0807C0C0, // breakablePlatformGenericMain
            0x0807C178, // tamBreakablePlatform
            0x0807C2D0, // "SpawnAI"
            0x0807C3C0, // "securityCamMainLoop"
            0x0807C564, // "webLineExists"
            0x0807C654, // zipPostWebbed
            0x0807C710, // sandSprinklerTurnOn
            0x0807C848, // carKillerScript
            0x0807C994, // setDeletePending
            0x0807CA84, // extinguishTheFire
            0x0807CB10, // breakablePlatformGenericCrumble
            0x0807CBA8, // tamCompleteCrumble
            0x0807CC0C, // tamTimeoutCrumble
            0x0807CCC8, // securityCamWebbed
            0x0807CD7C, // "waitLoop"
            0x0807CE6C, // zipPostWaitUntilNoWeb
            0x0807CEA8, // zipPostHit
            0x0807D2B8, // setDeletePending
            0x0807DC3C, // sandmanSwitchOn
            0x0807E104, // "runDistance"
            0x0807E1F4, // "isWalking"
            0x0807E32C, // "stopRunDistance"
            0x0807E41C, // "isRunning"
            0x0807E50C, // "isShowing"
            0x0807E5FC, // "sandmanChaseMainLoop"
            0x0807E6EC, // sandmanChaseMain
            0x0807E7B0, // sandmanWallDetachHitScripts
            0x0807E80C, // sandmanWallDamage
            0x0807E9A4, // sandmanUnbreakableWallAttachHitScripts
            0x0807EA08, // sandmanUnbreakableWallDetachHitScripts
            0x0807EBB0, // sandmanSprayerSetup
            0x0807EC58, // sandmanSwitchAttachHitScripts
            0x0807ECCC, // sandmanSwitchDetachHitScripts
            0x0807ED34, // sandmanSwitchSetup
            0x0807EDE4, // "loopUntilPauseFrame"
            0x0807EED4, // sandmanHazardDummyScript
            0x0807F000, // sandmanChaseForwardAttack
            0x0807F0A8, // sandmanChasePoundAttack
            0x0807F174, // "isShowing"
            0x0807F390, // "triggerDialog"
            0x0807F480, // sandmanChaseTriggered
            0x0807F51C, // sandmanWallAttachHitScripts
            0x0807F57C, // sandmanWallTriggered
            0x0807F6AC, // sandmanSprayerTurnOn
            0x0807F744, // sandmanSprayerBreak
            0x0807F810, // "right"
            0x0807F908, // "left"
            0x0807F9F8, // sandmanChaseWalk
            0x0807FCD8, // electricWallVerticalEnable
            0x08080C70, // sandSprinklerSwitchSetup
            0x08080FA0, // switchableDoor
            0x08080FFC, // switchOnce
            0x08081098, // electricWallHorizontalIdle
            0x0808111C, // electricWallHorizontalHide
            0x08081198, // electricWallVerticalDisable
            0x0808122C, // electricWallVerticalIdle
            0x080812B8, // electricWallVerticalHide
            0x0808132C, // electricSwitchIdle
            0x08081390, // TAMElectricWallHide
            0x080813E4, // bankRubbleIdle
            0x080814CC, // "hasKey"
            0x08081618, // "noDialog"
            0x08081708, // "noKey"
            0x080817F8, // "hitVictim"
            0x080818E8, // "mainLoop"
            0x080819D8, // lockableDoorRedMain
            0x08081A6C, // "hasKey"
            0x08081B98, // "noDialog"
            0x08081C88, // "noKey"
            0x08081D78, // "hitVictim"
            0x08081E68, // "mainLoop"
            0x08081F58, // lockableDoorBlueMain
            0x08081FEC, // "hasKey"
            0x08082118, // "noDialog"
            0x08082208, // "noKey"
            0x080822F8, // "hitVictim"
            0x080823E8, // "mainLoop"
            0x080824D8, // lockableDoorYellowMain
            0x08082560, // sandSprinklerSwitchTurnOn
            0x08082630, // valveSwitchSetup
            0x080826BC, // switchableDoorOpen
            0x0808273C, // activateSwitchOnce
            0x080827E8, // setDeletePending
            0x080828D8, // electricWallHorizontalDisable
            0x08082954, // electricSwitchDeactivate
            0x080829F4, // TAMElectricWallIdle
            0x08082A8C, // bankRubbleAppear
            0x08082B60, // valveSwitchActivate
            0x08082C14, // TAMElectricWallDisable
            0x08083088, // noiseMakerDropperWaypoint1
            0x080830E8, // noiseMakerDropperWaypoint2
            0x080834FC, // noiseMakerSwitchTakeHit
            0x080836AC, // "normalWebbed"
            0x0808379C, // noiseMakerSwitchWebbed
            0x0808381C, // noiseMakerSwitchTurnOff
            0x08083A88, // setDeletePending
            0x08083B78, // noiseMakerMain
            0x08083C38, // venomWallMain
            0x08083C9C, // fallingCeilingMain
            0x08083D18, // noiseMakerDropperDrop
            0x08083DBC, // noiseMakerDropperMain
            0x08083E68, // noiseMakerSwitchAttachScripts
            0x08083ED0, // noiseMakerSwitchDetachScripts
            0x08083F5C, // noiseMakerSwitchAttachDamageScripts
            0x0808406C, // "destroy"
            0x0808415C, // venomWallDestroy
            0x080841D0, // fallingCeilingFall
            0x080842C0, // noiseMakerDropperActivate
            0x08084348, // noiseMakerDropperDeactivate
            0x080845F8, // waitForPagedText
            0x08085E54, // saved1
            0x08085F4C, // saved2
            0x08086064, // setDeletePending
            0x08086318, // whimperingLoop
            0x080864FC, // "falling"
            0x080865F8, // victimFallToGround
            0x0808668C, // whimperUntilRescued
            0x080866E0, // TAMVictimRescueImmediatelyActivated
            0x08086790, // "isvisible"
            0x08086880, // whimper
            0x080868E0, // victimCollapseWebbed
            0x08086940, // victimCollapsePull
            0x08086A6C, // disablePull
            0x08086E24, // TAMTrigger
            0x08086EC4, // TAMTriggerBigger
            0x08086F70, // setDeletePending
            0x08087060, // TAMTriggerActivate
            0x080F3ABC, // notFound
            0x080F3E6C, // waitForPagedText
            0x080F3ED4, // missing
            0x080F3F1C, // infoMb1a1
            0x080F3F70, // infoMb1a2
            0x080F3FC4, // infoMb1b1
            0x080F4018, // infoMv3a1
            0x080F409C, // infoMv3a2
            0x080F40F0, // infoMb2a1
            0x080F4144, // infoMb2b1
            0x080F4198, // infoMb2b2
            0x080F41EC, // infoMb2c1
            0x080F4240, // infoMb4b1
            0x080F4294, // infoMb5a1
            0x080F42E8, // infoMb5b1
            0x080F433C, // infoMb5c1
            0x080F43C0, // infoKp1a1
            0x080F4414, // infoKp1b1
            0x080F4468, // infoKp4c1
            0x080F4504, // infoKp4c4
            0x080F4570, // infoMv6a1
            0x080F460C, // infoMv6b1
            0x080F4664, // infoCrawlDetail
            0x080F46B0, // infoSwingDetail
            0x080F4700, // infoSwitchDetail
            0x080F4750, // infoCompassDetail
            0x080F47A4, // infoWebSprinklerDetail
            0x080F47F4, // infoWebFanDetail
            0x080F4844, // infoTimeTamDetail
            0x080F4890, // infoZipDetail
            0x080F48D8, // TAMBlock
            0x080F4920, // infoWebPull
            0x080F496C, // infoWebUppercut
            0x080F49B8, // infoBlackSuit1
            0x080F4A04, // infoBlackSuit4
            0x080F4A54, // infoLoseBlackSuit
            0x080F4AA0, // infoStronger
            0x080F4AF8, // infoDashAttack
            0x080F4B44, // sm3EndOfSand
            0x080F4BC4, // infoWebSecurityCams
            0x080F4C1C, // sm3LimoFight
            0x080F4C78, // infoKingpinChase1
            0x080F4CD0, // sm3LimoChase
            0x080F4D2C, // sm3KingpinOffice
            0x080F4D88, // ingameKP3outro01
            0x080F4EB8, // sm3ElectroTip
            0x080F4F10, // infoElectroKill
            0x080F4F68, // sm3BombDisable
            0x080F4FB0, // failTAMBomb
            0x080F5004, // TAMCivilianDeathFail
            0x080F5070, // TAMfailtoreachSandman
            0x080F50D4, // sm3MoreBombs
            0x080F5120, // sm3SingleBomb
            0x080F5170, // sm3MoreCivilians
            0x080F51BC, // sm3MoreCameras
            0x080F5208, // infoGoblinText1
            0x080F5260, // infoGoblinText2
            0x080F52B8, // infoGoblinText3
            0x080F5314, // infoKickModifier
            0x080F5360, // infoDodgeMove1
            0x080F53AC, // infoDodgeMove2
            0x080F53F8, // infoDodgeMove3
            0x080F5440, // infoGoblin1
            0x080F5498, // infoCollapse
            0x080F54E4, // infoRescueFail
            0x080F5530, // infoEscapeFail
            0x080F557C, // infoPullSave
            0x080F55CC, // infoLosePowerups
            0x080F5628, // infoSandmanIntro
            0x080F5678, // infoSandwallIntro
            0x080F56C4, // infoDropDown
            0x080F570C, // infoWebLeap
            0x080F5758, // infoWebPost2
            0x080F57A4, // infoAvoidTrain
            0x080F57F0, // sm3WaterMain
            0x080F5838, // TAMBombFail
            0x080F58A8, // infoSpiderMode
            0x080F5908, // infoSpideyVSSandman1
            0x080F5968, // infoSpideyVSSandman2
            0x080F59C8, // infoSpideyVSSandman3
            0x080F5A28, // infoSpideyVSSandman4
            0x080F5A88, // infoSpideyVSSandman5
            0x080F5AE8, // infoSpideyVSSandman6
            0x080F5B48, // infoSpideyVSSandman10
            0x080F5BA8, // infoSpideyVSSandman11
            0x080F5C08, // infoSpideyVSSandman12
            0x080F5C68, // infoSpideyVSSandman13
            0x080F5CC8, // infoSpideyVSSandman14
            0x080F5D24, // sm3SubwayComplete
            0x080F5D70, // sm3TrainGotAway
            0x080F5DBC, // sm3FactoryIntro
            0x080F5E18, // sm3ExtinguishFire
            0x080F5E74, // sm3FindCityBombs
            0x080F5ECC, // infoRocketWall
            0x080F5F1C, // infoAnotherWindow
            0x080F5F64, // infoWebBomb
            0x080F5FB0, // infoMadBomber
            0x080F6004, // infoVenom1
            0x080F6058, // infoVenom2
            0x080F60A0, // infoVenom3
            0x080F60E8, // infoVenom4
            0x080F6138, // infoUnlockWebPull
            0x080F6198, // infoUnlockAreaAttacks
            0x080F61F4, // infoUnlockUppercuts
            0x080F6250, // infoUnlockBlacksuit
            0x080F62D4, // waitForMapDialog
            0x080FD8F4, // movie_licenseScreenBootup
            0x080FD93C, // movie_licenseCredits
            0x080FD9B8, // "waiting"
            0x080FDAA8, // health_warning
            0x080FDB64, // movie_intro
            0x080FDC68, // movie_title
            0x080FDCE8, // movie_credits
            0x080FDDB0, // mb_1a_intro
            0x080FDE8C, // "madbomber first"
            0x080FDF8C, // "kingpin first"
            0x080FE07C, // mb_2a_intro
            0x080FE10C, // mb_3a_intro
            0x080FE1FC, // mb_5a_intro
            0x080FE364, // mv_2a_intro
            0x080FE418, // mv_4a_intro
            0x080FE4CC, // mv_5a_intro
            0x080FE580, // mv_6a_intro
            0x080FE774, // "kingpin first"
            0x080FE864, // "madbomber first"
            0x080FE954, // kp_1a_intro
            0x080FE9D8, // kp_1a_outro
            0x080FEA68, // kp_2_intro
            0x080FEAE0, // kp_3_intro
            0x080FEB60, // mb_5c_boss_outro
            0x080FEC1C, // mv_1b_boss_outro
            0x080FED0C, // mv_2_outro
            0x080FEDC8, // mv_3a_boss_outro
            0x080FEEB8, // mv_4_outro
            0x080FF0D0, // mv_5c_boss_outro
            0x080FF148, // mv_6b_outro
            0x080FF1F8, // mv_6c_boss_outro
            0x080FF2D4, // movie_license
            0x080FF358, // waitForPagedText
        };

        public override uint ObjTypesPointer => 0x080322f8;
        public override int ObjTypesCount => 170;
    }
}