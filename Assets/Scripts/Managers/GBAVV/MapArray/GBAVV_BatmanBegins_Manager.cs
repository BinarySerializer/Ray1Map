using System.Collections.Generic;

namespace R1Engine
{
    public class GBAVV_BatmanBegins_Manager : GBAVV_MapArray_BaseManager
    {
        public override int LevelsCount => 50;
        public override bool HasAssignedObjTypeGraphics => true;

        public override string[] Languages => new string[]
        {
            "English",
            "French",
            "German",
            "Italian",
            "Spanish",
            "Dutch",
        };

        public override uint[] GraphicsDataPointers => new uint[]
        {
            0x0834AF14,
            0x0835A890,
            0x0837E8F0,
            0x08383610,
            0x0838B6C0,
            0x083AAB74,
            0x083AB7E4,
            0x083AD68C,
            0x083B5E98,
            0x083B7278,
            0x083B8994,
            0x083BBFC0,
            0x083BC9A0,
            0x083C4894,
            0x083C539C,
            0x083DF43C,
            0x08A25F0C,
            0x08AF87AC,
            0x08B3A2C0,
            0x08B88138,
            0x08B8CBA0,
            0x08B96540,
            0x08C67CDC,
        };

        public override Dictionary<int, GBAVV_ScriptCommand.CommandType> ScriptCommands => new Dictionary<int, GBAVV_ScriptCommand.CommandType>()
        {
            [0501] = GBAVV_ScriptCommand.CommandType.Name,
            [0502] = GBAVV_ScriptCommand.CommandType.Script,
            [0506] = GBAVV_ScriptCommand.CommandType.Return,
            [0702] = GBAVV_ScriptCommand.CommandType.Dialog,
        };

        public override uint[] ScriptPointers => new uint[]
        {
            0x08069E88, // script_waitForInputOrTime
            0x0806B1BC, // notFound
            0x0806B5F4, // waitForPagedText
            0x0806B65C, // missing
            0x0806B6A4, // OT01_A1S1L1
            0x0806B720, // Level1a1Start
            0x0806B79C, // OT01_A1S1L1a
            0x0806B838, // OT02_A1S1L2
            0x0806B8D4, // OT03_A1S1L4
            0x0806B99C, // batmanAsksAboutMask
            0x0806BA14, // OT12_A2S1L2
            0x0806BA68, // OT17_A2S2L2
            0x0806BAE4, // Level2a2Start
            0x0806BB38, // OT16_A2S2L1
            0x0806BBF8, // OT05_A2S1L1
            0x0806BCB8, // OT06_A2S1L1
            0x0806BD30, // OT07_A2S1L1
            0x0806BDA8, // OT08_A2S1L1
            0x0806BE20, // OT09_A2S1L1
            0x0806BE98, // OT10_A2S1L1
            0x0806BF10, // OT11_A2S1L1
            0x0806BF88, // Level2b1End
            0x0806BFE0, // Level2b2Start
            0x0806C058, // OT18_A2S2L2
            0x0806C0AC, // Level2b2End
            0x0806C128, // Level2c2Start
            0x0806C17C, // Level2c2End
            0x0806C1D0, // Level2c3End
            0x0806C228, // OT21_A2S2L2a
            0x0806C2C8, // NinjaCopSpot1
            0x0806C320, // NinjaCopSpot2
            0x0806C378, // NinjaCopSpot3
            0x0806C3CC, // OT22_A2S3L1
            0x0806C420, // CopSpot1
            0x0806C474, // CopSpot2
            0x0806C4C8, // CopSpot3
            0x0806C51C, // OT23_A3S1L1
            0x0806C5B8, // OT24_A3S1L3
            0x0806C674, // LtTaunt
            0x0806C6CC, // OT25_A3S1L3a
            0x0806C748, // OT25_A3S1L3b
            0x0806C7E8, // OT25_A3S1L3c
            0x0806C83C, // OT26_A3S2L1
            0x0806C8D8, // OT27_A3S2L1
            0x0806C974, // GunmanTaunt
            0x0806C9C8, // ConsigTaunt
            0x0806CA40, // OT28_A3S2L3
            0x0806CAE0, // OT28_A3S2L3a
            0x0806CBEC, // OT28_A3S2L3c
            0x0806CC68, // OT28_A3S2L3d
            0x0806CCBC, // OT29_A3S3L1
            0x0806CD58, // OT30_A3S3L3
            0x0806CDF4, // OT31_A3S3L3
            0x0806CE70, // OT32_A3S3L4a
            0x0806CEC8, // OT32_A3S3L4b
            0x0806CF20, // OT32_A3S3L4c
            0x0806CF78, // OT32_A3S3L4d
            0x0806CFD0, // OT32_A3S3L4e
            0x0806D028, // OT32_A3S3L4f
            0x0806D07C, // OT33_A4S1L1
            0x0806D118, // OT34_A4S1L1
            0x0806D1B8, // OT34_A4S1L1a
            0x0806D210, // OT34_A4S1L1b
            0x0806D268, // OT34_A4S1L1c
            0x0806D2C0, // OT34_A4S1L1d
            0x0806D318, // OT35_A4S1L1a
            0x0806D370, // OT35_A4S1L1b
            0x0806D3EC, // OT35_A4S1L1c
            0x0806D440, // OT36_A4S1L1
            0x0806D4DC, // OT37_A4S1L2
            0x0806D578, // OT38_A4S1L2
            0x0806D5F0, // OT39_A4S1L3
            0x0806D690, // OT40_A4S1L3a
            0x0806D70C, // OT40_A4S1L3b
            0x0806D788, // OT40_A4S1L3c
            0x0806D7E0, // OT40_A4S1L3d
            0x0806D858, // OT41_A4S2L1
            0x0806D8F4, // OT42_A4S2L2
            0x0806D948, // OT43_A4S2L3
            0x0806DA0C, // OT44_A4S2L3a
            0x0806DAAC, // OT44_A4S2L3b
            0x0806DB28, // OT44_A4S2L3c
            0x0806DB80, // OT44a_A4S3L1
            0x0806DC20, // BerserkerTaunt
            0x0806DC98, // OT45_A5S1L1
            0x0806DD34, // OT46_A5S1L3
            0x0806DE3C, // OT47_A5S2L1
            0x0806DEFC, // OT48_A5S2L3
            0x0806DFBC, // OT49_A5S2L4
            0x0806E05C, // OT49a_A5S2L4a
            0x0806E0B4, // OT49a_A5S2L4b
            0x0806E10C, // OT49a_A5S2L4c
            0x0806E188, // OT49a_A5S2L4d
            0x0806E1DC, // OT50_A5S3L3
            0x0806E258, // HelicopterTaunt
            0x0806E2AC, // OT51_A6S1L1
            0x0806E3B4, // OT52_A6S1L2
            0x0806E498, // OT53_A6S1L3
            0x0806E558, // OT54_A6S1L3
            0x0806E63C, // OT55_A7S1L1
            0x0806E744, // OT56_A7S1L3
            0x0806E804, // OT57_A7S2L3
            0x0806E910, // OT58_A7S2L3a
            0x0806E968, // OT58_A7S2L3b
            0x0806E9C0, // OT58_A7S2L3c
            0x0806EA18, // OT58_A7S2L3d
            0x0806EA70, // OT58_A7S2L3e
            0x0806EAC8, // OT58_A7S2L3f
            0x0806EB1C, // OT59_A7S2L3
            0x0806EBB8, // OT60_A7S2L3
            0x0806EC34, // jumpInstruction
            0x0806EC94, // gadgetSelectInstruction
            0x0806ECEC, // rollInstruction
            0x0806ED44, // runInstruction
            0x0806EDA4, // awareWalkInstruction
            0x0806EE28, // pipeAttackInstruction
            0x0806EE84, // shurikenInstruction
            0x0806EEE0, // runNoiseInstruction
            0x0806EF40, // hideShadowInstruction
            0x0806EFA0, // breakLightInstruction
            0x0806EFF8, // communicatorA01
            0x0806F094, // craneBoss01
            0x0806F134, // speakerHeadTest
            0x080BB9E8, // nintendo_license
            0x080BBA40, // movie_license
            0x080BBA74, // movie_mainMenu
            0x080BBAC0, // CS_GenericSave
            0x080BBB64, // CS07_A3S3
            0x080BBC4C, // unlockedNothing
            0x080BBCA8, // unlockedHardMode
            0x080BBD00, // unlockedMenus
            0x080BC6CC, // waitForPagedText
            0x080BC73C, // CS_unlockedNone
            0x080BC7B8, // CS_unlockedHard
            0x080BC838, // CS_unlockedMenus
            0x080BC948, // movie_intro
            0x080C014C, // rasDebrisSpawnerPause
            0x080C0B58, // helicopterCreate
            0x080C0BFC, // heliBombCreate
            0x080C0D20, // fallingNinjaCreate
            0x080C0E58, // fallingMonasteryNinjaCreate
            0x080C0F34, // bossInmateCreate
            0x080C0F90, // bossInmateManagerCreate
            0x080C1044, // bossCraneSprayCreate
            0x080C1168, // "wait"
            0x080C126C, // "fear"
            0x080C135C, // bossCraneSprayToxinCreate
            0x080C14E4, // "tryToHit"
            0x080C15D4, // ducardCreate
            0x080C1760, // ducardNinjaSpawnerCreate
            0x080C17C0, // ducardNinjaManagerCreate
            0x080C1834, // rasCeilingSectionCreate
            0x080C1890, // rasPillarSectionCreate
            0x080C18F8, // rasDebrisSpawnerGoLeft
            0x080C1954, // rasDebrisSpawnerCreate
            0x080C19A0, // LieutenantCreate
            0x080C19EC, // bossGunmanCreate
            0x080C1A38, // conLockerDoorCreate
            0x080C1AD4, // conLockerNPCCreate
            0x080C1B64, // falconeThugSpawnerCreate
            0x080C1BD0, // flassZsaszCreate
            0x080C1C38, // flassChainCreate
            0x080C1C80, // flassFenceCreate
            0x080C1CD4, // bossFlassCreate
            0x080C1D10, // bossZsaszCreate
            0x080C1D5C, // bossThugUziCreate
            0x080C1D98, // bossCraneCreate
            0x080C1E50, // craneToxinCanCreate
            0x080C1FDC, // bossCraneBerserkerCreate
            0x080C2040, // bossDucardsNinjaManagerCreate
            0x080C20D4, // bossDucardsNinjaDucardCreate
            0x080C2178, // windowNinjaSpawnerCreate
            0x080C2218, // bossScarecrowCreate
            0x080C225C, // rasNinjaSpawnerCreate
            0x080C2308, // "moveRight"
            0x080C2434, // "stop"
            0x080C2524, // "playerToRight"
            0x080C2648, // "setMove"
            0x080C2778, // "moveLeft"
            0x080C2888, // "playerToLeft"
            0x080C2978, // helicopterUpdate
            0x080C29EC, // "waitForCollision"
            0x080C2ADC, // heliBombUpdate
            0x080C2B6C, // absorbHit
            0x080C2C00, // bossInmateTurnOn
            0x080C2CB0, // bossInmateManagerTurnOn
            0x080C2D1C, // ducardNinjaSpawnerSpawn
            0x080C2D5C, // ducardNinjaManagerTakeHit
            0x080C2DFC, // ducardNinjaManagerMain
            0x080C2FD4, // rasCeilingSectionFall
            0x080C30A0, // "takeHit"
            0x080C3190, // "takeHit"
            0x080C32A0, // "die"
            0x080C3390, // rasPillarSectionTakeHit
            0x080C342C, // rasDebrisSpawnerSpawn
            0x080C34D4, // LieutenantTurnOn
            0x080C35E8, // bossGunmanTurnOn
            0x080C36A8, // conLockerDoorClose
            0x080C3764, // conLockerNPCTurnOff
            0x080C382C, // "rightSide"
            0x080C395C, // "leftSide"
            0x080C3A54, // falconeThugSpawnerSpawn
            0x080C3AEC, // flassZsaszRelease
            0x080C3BC0, // flassZsaszTakeHit
            0x080C3C54, // flassFenceDie
            0x080C3D08, // bossFlassTurnOn
            0x080C3E04, // bossZsaszTurnOn
            0x080C3F28, // bossThugUziTurnOn
            0x080C4008, // "follow"
            0x080C40F8, // bossCraneTurnOn
            0x080C4184, // toxinMoveX5
            0x080C41F0, // toxinMoveX4
            0x080C425C, // toxinMoveX3
            0x080C42C8, // toxinMoveX2
            0x080C4334, // toxinMoveX1
            0x080C43FC, // bossCraneBerserkerSpawn
            0x080C4474, // bossDucardsNinjaManagerEnemyKilled
            0x080C44E8, // bossDucardsNinjaDucardTurnOn
            0x080C45FC, // bossScarecrowTurnOn
            0x080C46B0, // rasNinjaSpawnerSpawn
            0x080C474C, // "resetTimer"
            0x080C489C, // helicopterUpdateV
            0x080C496C, // "don'tCrash"
            0x080C4A5C, // helicopterPassOverPlayer
            0x080C4AD4, // bossInmateManagerTakeHit
            0x080C4B24, // LieutenantDefeat
            0x080C4B5C, // bossGunmanDefeat
            0x080C4BA4, // conLockerDoorTakeHit
            0x080C4C70, // "rightSide"
            0x080C4D94, // "leftSide"
            0x080C4E84, // falconeThugSpawnerSpawn2
            0x080C4EE4, // zsaszJump
            0x080C5008, // "replace"
            0x080C50F8, // bossCraneEnemyKilled
            0x080C5174, // bossCraneThrow
            0x080C5214, // bossCraneGoLeft
            0x080C5294, // bossCraneGoRight
            0x080C5314, // bossScarecrowDefeat
            0x080C5360, // bossCraneWalk
            0x080C53C4, // bossCraneIdle
            0x080C56DC, // explodingBarrelCreate
            0x080C578C, // fireCombustCreate
            0x080C582C, // firePainfulCombustCreate
            0x080C58BC, // firePainfulCreate
            0x080C5958, // explodingBarrelTakeHit
            0x080C5A44, // fireCombustIgnite
            0x080C5AE4, // firePainfulCombustIgnite
            0x080C5B90, // explodingBarrelExplode
            0x080C5C58, // explodingBarrelTakeHit2
            0x080C61A0, // batswarmStart
            0x080C62DC, // "fall"
            0x080C63CC, // fallingDebris1Create
            0x080C64A4, // "spawn"
            0x080C6594, // fallingDebrisSpawner1Create
            0x080C65E4, // collapsingCeilingCreate
            0x080C6638, // triggeredExplosionCreate
            0x080C669C, // doorAISpawnerCreate
            0x080C6758, // doorThug1Create
            0x080C681C, // skylightFallingGlassCreate
            0x080C687C, // arsonistCreate
            0x080C6900, // "looking"
            0x080C69F0, // grapelDialogTriggerCreate
            0x080C6A54, // collapsingCeilingCollapse
            0x080C6AEC, // triggeredExplosionExplode
            0x080C6B94, // doorAISpawn
            0x080C6C34, // arsonistTurnOn
            0x080C6CD8, // grapnelDialogOff
            0x080C6F80, // copGoRight
            0x080C7270, // laserCreate
            0x080C7340, // toxinCloudCreate
            0x080C7408, // toxinCloudSpawnerCreate
            0x080C7460, // toxinCloudSpawnerMasterCreate
            0x080C74B8, // toxinCloudSpawnerSlaveCreate
            0x080C74FC, // copIdle
            0x080C7540, // copGoLeft
            0x080C75C0, // copCreate
            0x080C7678, // copDelayedCreate
            0x080C772C, // ninjaCopCreate
            0x080C77D4, // laserPause
            0x080C7864, // "resume"
            0x080C7954, // "pauseThenGoLeft"
            0x080C7A54, // "justGoLeft"
            0x080C7B44, // laserLeft
            0x080C7BCC, // "resume"
            0x080C7CBC, // "pauseThenGoRight"
            0x080C7DBC, // "justGoRight"
            0x080C7EAC, // laserRight
            0x080C7F2C, // "resume"
            0x080C801C, // "pauseThenGoUp"
            0x080C8118, // "justGoUp"
            0x080C8208, // laserUp
            0x080C8290, // "resume"
            0x080C8380, // "pauseThenGoDown"
            0x080C8480, // "justGoDown"
            0x080C8570, // laserDown
            0x080C863C, // "spawn"
            0x080C872C, // toxinCloudSpawnerEnable
            0x080C8778, // toxinCloudSpawnerMasterEnable
            0x080C87C8, // toxinCloudSpawnerSlaveEnable
            0x080C8828, // randomBark
            0x080C8924, // copSpot
            0x080C8A00, // ninjaCopSpot
            0x080C8AB8, // copHearPlayer
            0x080C8B10, // ninjaCopHearPlayer
            0x080C8C88, // "facingRight"
            0x080C8D9C, // "facingLeft"
            0x080C8E8C, // copPatrol
            0x080C8F74, // "facingRight"
            0x080C9064, // "facingLeft"
            0x080C9154, // ninjaCopPatrol
            0x080C923C, // ninjaCopTurnOn
            0x080C9290, // laserStop
            0x080C92D8, // laserGoLeft
            0x080C9330, // laserGoRight
            0x080C9378, // laserGoUp
            0x080C93C0, // laserGoDown
            0x080C9420, // toxinCloudSpawnerSpawn
            0x080C9520, // "goLeft"
            0x080C963C, // "goRight"
            0x080C972C, // copWalk
            0x080C97C4, // "checkHeight"
            0x080C98B4, // copCheckNearDistance
            0x080C990C, // copCheckFarDistance
            0x080C9958, // "checkHeight"
            0x080C9A48, // ninjaCopCheckNearDistance
            0x080C9A94, // ninjaCopCheckFarDistance
            0x080CA028, // collapsingPlatformCreate
            0x080CA0BC, // rollingBarrelCreate
            0x080CA11C, // fallingObjectCreate
            0x080CA198, // breakableDoorCreate
            0x080CA224, // intercomCreate
            0x080CA2B4, // collapsingPlatformDestroy
            0x080CA360, // collapsingPlatformCollapse
            0x080CA3E4, // rollingBarrelRelease
            0x080CA4D4, // "fall"
            0x080CA5D0, // fallingObjectFall
            0x080CA6D8, // breakableDoorTakeHit
            0x080CA75C, // absorbHit
            0x080CA7FC, // intercomLoop
            0x080CA91C, // rollingBarrelTakeHit
            0x080CA988, // rollingBarrelAbsorbHit
            0x080CA9B4, // breakableDoorDie
            0x080CAA4C, // intercomFinish
            0x080CAA9C, // rollingBarrelDie
            0x080CABBC, // movingPlatformON
            0x080CADA8, // movingPlatformOFF
            0x080CB224, // movingPlatformPause
            0x080CB2A8, // "resume"
            0x080CB398, // "pauseThenGoLeft"
            0x080CB498, // "justGoLeft"
            0x080CB588, // movingPlatformLeft
            0x080CB604, // "resume"
            0x080CB6F4, // "pauseThenGoRight"
            0x080CB7F4, // "justGoRight"
            0x080CB8E4, // movingPlatformRight
            0x080CB95C, // "resume"
            0x080CBA4C, // "pauseThenGoUp"
            0x080CBB48, // "justGoUp"
            0x080CBC38, // movingPlatformUp
            0x080CBCB4, // "resume"
            0x080CBDA4, // "pauseThenGoDown"
            0x080CBEA4, // "justGoDown"
            0x080CBF94, // movingPlatformDown
            0x080CC01C, // "start"
            0x080CC114, // "stop"
            0x080CC204, // movingPlatformPauseLeft
            0x080CC280, // "start"
            0x080CC370, // movingPlatformPauseRight
            0x080CC3E8, // "start"
            0x080CC4D8, // movingPlatformPauseUp
            0x080CC550, // "start"
            0x080CC640, // movingPlatformPauseDown
            0x080CC6D0, // movingPlatformCreate
            0x080CC788, // movingMonasteryDoorCreate
            0x080CC7D8, // falconeContainerCreate
            0x080CC840, // movingPlatformStop
            0x080CC888, // movingPlatformGoLeft
            0x080CC8D0, // movingPlatformGoRight
            0x080CC914, // movingPlatformGoUp
            0x080CC95C, // movingPlatformGoDown
            0x080CC998, // absorbHit
            0x080CC9D4, // falconeContainerFall
            0x080CCA98, // falconeContainerUpdate
            0x080CD0B4, // genericNPC
            0x080CD100, // masterNinjaNPC
            0x080CD154, // npcBGsprite
            0x080CD184, // "watch"
            0x080CD274, // npcCraneCreate
            0x080CD310, // npcQuinzelleCreate
            0x080CD3D0, // npcLuciousCreate
            0x080CD434, // talkToVictim
            0x080CD490, // npcQuinzelleCountAI
            0x080CD51C, // doorCreate
            0x080CD650, // windowBreak
            0x080CD6B4, // circuitBoxBreak
            0x080CEDA0, // crumblingStone
            0x080CEE04, // SFXing
            0x080CEEF4, // hazardCreate
            0x080CEF64, // skyLightCreate
            0x080CEFFC, // restrictedDoorCreate
            0x080CF060, // intercomDoorCreate
            0x080CF0C8, // endSectionOnKillCreate
            0x080CF11C, // windowCreate
            0x080CF174, // circuitBoxCreate
            0x080CF1CC, // boxCreate
            0x080CF24C, // stealthBreakableCreate
            0x080CF2AC, // lightsOn
            0x080CF318, // lightsOff
            0x080CF3D4, // windowThug1
            0x080CF4D8, // cellCreate
            0x080CF5F8, // looseCellCreate
            0x080CF750, // securityDialogCreate
            0x080CF790, // crumbleScript
            0x080CF834, // skyLightReady
            0x080CF8BC, // skyLightText
            0x080CF90C, // endSectionOnKillEnd
            0x080CF960, // boxBreak
            0x080CF9E4, // stealthBreakableBreak
            0x080CFA38, // cellRelease
            0x080CFB18, // securityDialogPlay
            0x080CFB7C, // skyLightBreak
            0x080CFD94, // spawnSparks
            0x080CFE94, // sparks
            0x080CFF60, // onSwitchSwitchedReverse
            0x080D086C, // toggleSwitchCreate
            0x080D08FC, // onSwitchSwitched
            0x080D0984, // onSwitchCreate
            0x080D09FC, // onSwitchCreateReverse
            0x080D0A80, // singleSwitchCreate
            0x080D0B04, // monasteryMeleeSwitchCreate
            0x080D0B74, // monasteryStarSwitchCreate
            0x080D0C00, // areaTriggerCreate
            0x080D0C6C, // dialogTriggerBigCreate
            0x080D0CE0, // deathByFallCreate
            0x080D0D58, // blendFullCreate
            0x080D0DD4, // blendHalfCreate
            0x080D0E50, // blendNoneCreate
            0x080D0EE0, // createLightSwitch
            0x080D0F6C, // "switchOn"
            0x080D1074, // "switchOff"
            0x080D1164, // toggleSwitchSwitched
            0x080D1200, // singleSwitchSwitched
            0x080D126C, // monasterySwitchSwitched
            0x080D12B8, // alphaInvisible
            0x080D12E8, // alphaHalf
            0x080D1318, // alphaSolid
            0x080D1370, // lightSwitched
            0x080D1458, // lightFlicker
        };

        public override uint ObjTypesPointer => 0x08008f38;
        public override int ObjTypesCount => 268;

        public override ObjTypeInit[] ObjTypeInitInfos => new ObjTypeInit[]
        {
            new ObjTypeInit(-1, -1, null), // 0
            new ObjTypeInit(-1, -1, null), // 1
            new ObjTypeInit(-1, -1, null), // 2
            new ObjTypeInit(-1, -1, null), // 3
            new ObjTypeInit(4, 1, null), // 4
            new ObjTypeInit(14, 1, null), // 5
            new ObjTypeInit(15, 1, null), // 6
            new ObjTypeInit(1, 138, "npcBGsprite"), // 7
            new ObjTypeInit(20, 32, "genericNPC"), // 8
            new ObjTypeInit(2, 0, "npcBGsprite"), // 9
            new ObjTypeInit(2, 2, "npcBGsprite"), // 10
            new ObjTypeInit(1, 2, "npcCraneCreate"), // 11
            new ObjTypeInit(2, 3, "npcQuinzelleCreate"), // 12
            new ObjTypeInit(2, 4, "npcLuciousCreate"), // 13
            new ObjTypeInit(2, 41, "npcBGsprite"), // 14
            new ObjTypeInit(17, 31, "masterNinjaNPC"), // 15
            new ObjTypeInit(-1, -1, null), // 16
            new ObjTypeInit(-1, -1, null), // 17
            new ObjTypeInit(-1, -1, null), // 18
            new ObjTypeInit(-1, -1, null), // 19
            new ObjTypeInit(-1, -1, null), // 20
            new ObjTypeInit(-1, -1, null), // 21
            new ObjTypeInit(-1, -1, null), // 22
            new ObjTypeInit(-1, -1, null), // 23
            new ObjTypeInit(-1, -1, null), // 24
            new ObjTypeInit(-1, -1, null), // 25
            new ObjTypeInit(-1, -1, null), // 26
            new ObjTypeInit(4, 1, null), // 27
            new ObjTypeInit(11, 0, null), // 28
            new ObjTypeInit(11, 0, null), // 29
            new ObjTypeInit(0, 9, null), // 30
            new ObjTypeInit(0, 10, null), // 31
            new ObjTypeInit(0, 11, null), // 32
            new ObjTypeInit(0, 12, null), // 33
            new ObjTypeInit(19, 0, null), // 34
            new ObjTypeInit(0, 38, null), // 35
            new ObjTypeInit(22, 0, null), // 36
            new ObjTypeInit(18, 0, null), // 37
            new ObjTypeInit(0, 40, "copCreate"), // 38
            new ObjTypeInit(17, 21, null), // 39
            new ObjTypeInit(17, 0, "ninjaCopCreate"), // 40
            new ObjTypeInit(17, 1, null), // 41
            new ObjTypeInit(17, 28, "fallingNinjaCreate"), // 42
            new ObjTypeInit(17, 28, "fallingMonasteryNinjaCreate"), // 43
            new ObjTypeInit(22, 21, null), // 44
            new ObjTypeInit(21, 0, null), // 45
            new ObjTypeInit(21, 18, null), // 46
            new ObjTypeInit(17, 23, null), // 47
            new ObjTypeInit(0, 40, "copDelayedCreate"), // 48
            new ObjTypeInit(22, 0, "bossInmateCreate"), // 49
            new ObjTypeInit(22, 0, null), // 50
            new ObjTypeInit(11, 0, "bossInmateManagerCreate"), // 51
            new ObjTypeInit(1, 132, "bossCraneSprayCreate"), // 52
            new ObjTypeInit(10, 14, "bossCraneSprayToxinCreate"), // 53
            new ObjTypeInit(20, 0, "ducardCreate"), // 54
            new ObjTypeInit(17, 21, "ducardNinjaSpawnerCreate"), // 55
            new ObjTypeInit(17, 32, "ducardNinjaManagerCreate"), // 56
            new ObjTypeInit(1, 24, null), // 57
            new ObjTypeInit(7, 31, "rasPillarSectionCreate"), // 58
            new ObjTypeInit(1, 28, "rasCeilingSectionCreate"), // 59
            new ObjTypeInit(7, 24, "rasDebrisSpawnerCreate"), // 60
            new ObjTypeInit(19, 20, "LieutenantCreate"), // 61
            new ObjTypeInit(19, 20, null), // 62
            new ObjTypeInit(0, 12, "bossGunmanCreate"), // 63
            new ObjTypeInit(0, 39, null), // 64
            new ObjTypeInit(1, 50, null), // 65
            new ObjTypeInit(1, 108, "conLockerDoorCreate"), // 66
            new ObjTypeInit(1, 110, "conLockerNPCCreate"), // 67
            new ObjTypeInit(1, 103, "helicopterCreate"), // 68
            new ObjTypeInit(1, 106, "heliBombCreate"), // 69
            new ObjTypeInit(1, 75, null), // 70
            new ObjTypeInit(0, 4, "falconeThugSpawnerCreate"), // 71
            new ObjTypeInit(1, 76, "bossFlassCreate"), // 72
            new ObjTypeInit(1, 76, null), // 73
            new ObjTypeInit(1, 79, "flassChainCreate"), // 74
            new ObjTypeInit(1, 78, "flassZsaszCreate"), // 75
            new ObjTypeInit(1, 100, "flassFenceCreate"), // 76
            new ObjTypeInit(1, 0, "bossCraneCreate"), // 77
            new ObjTypeInit(1, 0, null), // 78
            new ObjTypeInit(10, 11, "craneToxinCanCreate"), // 79
            new ObjTypeInit(18, 0, "bossCraneBerserkerCreate"), // 80
            new ObjTypeInit(18, 0, "bossZsaszCreate"), // 81
            new ObjTypeInit(18, 0, null), // 82
            new ObjTypeInit(21, 0, "bossThugUziCreate"), // 83
            new ObjTypeInit(1, 111, "bossScarecrowCreate"), // 84
            new ObjTypeInit(1, 112, null), // 85
            new ObjTypeInit(20, 0, "bossDucardsNinjaDucardCreate"), // 86
            new ObjTypeInit(11, 0, "bossDucardsNinjaManagerCreate"), // 87
            new ObjTypeInit(20, 0, null), // 88
            new ObjTypeInit(17, 0, "rasNinjaSpawnerCreate"), // 89
            new ObjTypeInit(1, 50, "npcBGsprite"), // 90
            new ObjTypeInit(-1, -1, null), // 91
            new ObjTypeInit(21, 27, null), // 92
            new ObjTypeInit(7, 40, "doorAISpawnerCreate"), // 93
            new ObjTypeInit(0, 24, "doorThug1Create"), // 94
            new ObjTypeInit(7, 65, "windowNinjaSpawnerCreate"), // 95
            new ObjTypeInit(18, 23, "cellCreate"), // 96
            new ObjTypeInit(18, 23, "looseCellCreate"), // 97
            new ObjTypeInit(-1, -1, null), // 98
            new ObjTypeInit(-1, -1, null), // 99
            new ObjTypeInit(-1, -1, null), // 100
            new ObjTypeInit(-1, -1, null), // 101
            new ObjTypeInit(-1, -1, null), // 102
            new ObjTypeInit(-1, -1, null), // 103
            new ObjTypeInit(-1, -1, null), // 104
            new ObjTypeInit(-1, -1, null), // 105
            new ObjTypeInit(-1, -1, null), // 106
            new ObjTypeInit(-1, -1, null), // 107
            new ObjTypeInit(-1, -1, null), // 108
            new ObjTypeInit(-1, -1, null), // 109
            new ObjTypeInit(-1, -1, null), // 110
            new ObjTypeInit(-1, -1, null), // 111
            new ObjTypeInit(-1, -1, null), // 112
            new ObjTypeInit(-1, -1, null), // 113
            new ObjTypeInit(-1, -1, null), // 114
            new ObjTypeInit(-1, -1, null), // 115
            new ObjTypeInit(-1, -1, null), // 116
            new ObjTypeInit(-1, -1, null), // 117
            new ObjTypeInit(8, 5, "lightsOn"), // 118
            new ObjTypeInit(8, 5, "lightsOff"), // 119
            new ObjTypeInit(-1, -1, null), // 120
            new ObjTypeInit(-1, -1, null), // 121
            new ObjTypeInit(8, 5, "areaTriggerCreate"), // 122
            new ObjTypeInit(8, 6, "areaTriggerCreate"), // 123
            new ObjTypeInit(8, 5, "areaTriggerCreate"), // 124
            new ObjTypeInit(-1, -1, null), // 125
            new ObjTypeInit(8, 6, "blendFullCreate"), // 126
            new ObjTypeInit(8, 6, "blendHalfCreate"), // 127
            new ObjTypeInit(8, 6, "blendNoneCreate"), // 128
            new ObjTypeInit(8, 6, "deathByFallCreate"), // 129
            new ObjTypeInit(-1, -1, null), // 130
            new ObjTypeInit(6, 15, "crumblingStone"), // 131
            new ObjTypeInit(7, 25, "breakableDoorCreate"), // 132
            new ObjTypeInit(7, 28, "breakableDoorCreate"), // 133
            new ObjTypeInit(9, 12, "stealthBreakableCreate"), // 134
            new ObjTypeInit(7, 63, "fallingObjectCreate"), // 135
            new ObjTypeInit(7, 7, "rollingBarrelCreate"), // 136
            new ObjTypeInit(7, 75, "toggleSwitchCreate"), // 137
            new ObjTypeInit(7, 75, "onSwitchCreateReverse"), // 138
            new ObjTypeInit(12, 35, "monasteryMeleeSwitchCreate"), // 139
            new ObjTypeInit(12, 33, "monasteryMeleeSwitchCreate"), // 140
            new ObjTypeInit(7, 62, "intercomCreate"), // 141
            new ObjTypeInit(7, 42, "intercomDoorCreate"), // 142
            new ObjTypeInit(12, 37, "monasteryStarSwitchCreate"), // 143
            new ObjTypeInit(7, 57, "doorCreate"), // 144
            new ObjTypeInit(7, 57, "restrictedDoorCreate"), // 145
            new ObjTypeInit(-1, -1, null), // 146
            new ObjTypeInit(7, 35, "skyLightCreate"), // 147
            new ObjTypeInit(7, 38, "restrictedDoorCreate"), // 148
            new ObjTypeInit(7, 40, "doorCreate"), // 149
            new ObjTypeInit(-1, -1, null), // 150
            new ObjTypeInit(7, 40, "restrictedDoorCreate"), // 151
            new ObjTypeInit(7, 42, "doorCreate"), // 152
            new ObjTypeInit(7, 42, "restrictedDoorCreate"), // 153
            new ObjTypeInit(-1, -1, null), // 154
            new ObjTypeInit(7, 44, "doorCreate"), // 155
            new ObjTypeInit(7, 44, "restrictedDoorCreate"), // 156
            new ObjTypeInit(-1, -1, null), // 157
            new ObjTypeInit(7, 46, "doorCreate"), // 158
            new ObjTypeInit(7, 46, "restrictedDoorCreate"), // 159
            new ObjTypeInit(-1, -1, null), // 160
            new ObjTypeInit(7, 47, "doorCreate"), // 161
            new ObjTypeInit(7, 47, "restrictedDoorCreate"), // 162
            new ObjTypeInit(-1, -1, null), // 163
            new ObjTypeInit(7, 49, "doorCreate"), // 164
            new ObjTypeInit(7, 49, "restrictedDoorCreate"), // 165
            new ObjTypeInit(-1, -1, null), // 166
            new ObjTypeInit(7, 52, "doorCreate"), // 167
            new ObjTypeInit(7, 52, "restrictedDoorCreate"), // 168
            new ObjTypeInit(-1, -1, null), // 169
            new ObjTypeInit(7, 54, "doorCreate"), // 170
            new ObjTypeInit(7, 54, "restrictedDoorCreate"), // 171
            new ObjTypeInit(-1, -1, null), // 172
            new ObjTypeInit(7, 58, "doorCreate"), // 173
            new ObjTypeInit(7, 58, "restrictedDoorCreate"), // 174
            new ObjTypeInit(-1, -1, null), // 175
            new ObjTypeInit(7, 76, "createLightSwitch"), // 176
            new ObjTypeInit(7, 57, "intercomDoorCreate"), // 177
            new ObjTypeInit(7, 66, "onSwitchCreateReverse"), // 178
            new ObjTypeInit(7, 67, "onSwitchCreate"), // 179
            new ObjTypeInit(7, 69, "onSwitchCreate"), // 180
            new ObjTypeInit(7, 70, "onSwitchCreateReverse"), // 181
            new ObjTypeInit(7, 72, "singleSwitchCreate"), // 182
            new ObjTypeInit(7, 72, "singleSwitchCreate"), // 183
            new ObjTypeInit(8, 5, "securityDialogCreate"), // 184
            new ObjTypeInit(7, 69, "singleSwitchCreate"), // 185
            new ObjTypeInit(7, 19, "explodingBarrelCreate"), // 186
            new ObjTypeInit(7, 12, "fireCombustCreate"), // 187
            new ObjTypeInit(7, 12, "firePainfulCombustCreate"), // 188
            new ObjTypeInit(-1, -1, null), // 189
            new ObjTypeInit(7, 14, "firePainfulCreate"), // 190
            new ObjTypeInit(-1, -1, null), // 191
            new ObjTypeInit(7, 22, "firePainfulCreate"), // 192
            new ObjTypeInit(7, 11, "laserCreate"), // 193
            new ObjTypeInit(7, 10, "laserCreate"), // 194
            new ObjTypeInit(10, 12, "toxinCloudSpawnerCreate"), // 195
            new ObjTypeInit(10, 12, "toxinCloudSpawnerMasterCreate"), // 196
            new ObjTypeInit(10, 12, "toxinCloudSpawnerSlaveCreate"), // 197
            new ObjTypeInit(10, 12, "toxinCloudCreate"), // 198
            new ObjTypeInit(-1, -1, null), // 199
            new ObjTypeInit(7, 4, "windowCreate"), // 200
            new ObjTypeInit(12, 7, "hazardCreate"), // 201
            new ObjTypeInit(12, 4, "circuitBoxCreate"), // 202
            new ObjTypeInit(-1, -1, null), // 203
            new ObjTypeInit(12, 6, "spawnSparks"), // 204
            new ObjTypeInit(-1, -1, null), // 205
            new ObjTypeInit(-1, -1, null), // 206
            new ObjTypeInit(7, 2, "boxCreate"), // 207
            new ObjTypeInit(-1, -1, null), // 208
            new ObjTypeInit(12, 16, "windowThug1"), // 209
            new ObjTypeInit(-1, -1, null), // 210
            new ObjTypeInit(-1, -1, null), // 211
            new ObjTypeInit(-1, -1, null), // 212
            new ObjTypeInit(-1, -1, null), // 213
            new ObjTypeInit(-1, -1, null), // 214
            new ObjTypeInit(-1, -1, null), // 215
            new ObjTypeInit(-1, -1, null), // 216
            new ObjTypeInit(-1, -1, null), // 217
            new ObjTypeInit(-1, -1, null), // 218
            new ObjTypeInit(-1, -1, null), // 219
            new ObjTypeInit(-1, -1, null), // 220
            new ObjTypeInit(-1, -1, null), // 221
            new ObjTypeInit(-1, -1, null), // 222
            new ObjTypeInit(-1, -1, null), // 223
            new ObjTypeInit(-1, -1, null), // 224
            new ObjTypeInit(-1, -1, null), // 225
            new ObjTypeInit(10, 19, "sparks"), // 226
            new ObjTypeInit(7, 2, "fallingDebrisSpawner1Create"), // 227
            new ObjTypeInit(7, 24, "fallingDebris1Create"), // 228
            new ObjTypeInit(7, 18, "triggeredExplosionCreate"), // 229
            new ObjTypeInit(12, 27, "collapsingCeilingCreate"), // 230
            new ObjTypeInit(16, 4, "batswarmStart"), // 231
            new ObjTypeInit(9, 4, "falconeContainerCreate"), // 232
            new ObjTypeInit(12, 30, "skylightFallingGlassCreate"), // 233
            new ObjTypeInit(22, 1, "arsonistCreate"), // 234
            new ObjTypeInit(8, 5, "grapelDialogTriggerCreate"), // 235
            new ObjTypeInit(-1, -1, null), // 236
            new ObjTypeInit(-1, -1, null), // 237
            new ObjTypeInit(-1, -1, null), // 238
            new ObjTypeInit(-1, -1, null), // 239
            new ObjTypeInit(-1, -1, null), // 240
            new ObjTypeInit(-1, -1, null), // 241
            new ObjTypeInit(-1, -1, null), // 242
            new ObjTypeInit(-1, -1, null), // 243
            new ObjTypeInit(-1, -1, null), // 244
            new ObjTypeInit(-1, -1, null), // 245
            new ObjTypeInit(-1, -1, null), // 246
            new ObjTypeInit(-1, -1, null), // 247
            new ObjTypeInit(-1, -1, null), // 248
            new ObjTypeInit(-1, -1, null), // 249
            new ObjTypeInit(-1, -1, null), // 250
            new ObjTypeInit(-1, -1, null), // 251
            new ObjTypeInit(-1, -1, null), // 252
            new ObjTypeInit(-1, -1, null), // 253
            new ObjTypeInit(-1, -1, null), // 254
            new ObjTypeInit(-1, -1, null), // 255
            new ObjTypeInit(9, 1, "movingPlatformCreate"), // 256
            new ObjTypeInit(9, 0, "movingPlatformCreate"), // 257
            new ObjTypeInit(9, 10, "collapsingPlatformCreate"), // 258
            new ObjTypeInit(9, 7, "collapsingPlatformCreate"), // 259
            new ObjTypeInit(9, 16, "movingPlatformCreate"), // 260
            new ObjTypeInit(9, 15, "movingPlatformCreate"), // 261
            new ObjTypeInit(9, 3, "movingMonasteryDoorCreate"), // 262
            new ObjTypeInit(9, 2, "movingPlatformCreate"), // 263
            new ObjTypeInit(9, 14, "collapsingPlatformCreate"), // 264
            new ObjTypeInit(9, 6, "movingPlatformCreate"), // 265
            new ObjTypeInit(5, 4, null), // 266
            new ObjTypeInit(5, 4, null), // 267
        };
    }
}