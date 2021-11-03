using System.Collections.Generic;

namespace Ray1Map.GBAVV
{
    public abstract class GBAVV_ShrekTheThird_Manager : GBAVV_MapArray_BaseManager
    {
        public override int LevelsCount => 29;

        public override Dictionary<int, GBAVV_ScriptCommand.CommandType> ScriptCommands => new Dictionary<int, GBAVV_ScriptCommand.CommandType>()
        {
            [0502] = GBAVV_ScriptCommand.CommandType.Name,
            [0507] = GBAVV_ScriptCommand.CommandType.Return,
            [0702] = GBAVV_ScriptCommand.CommandType.Dialog,
        };
    }
    public class GBAVV_ShrekTheThirdEU_Manager : GBAVV_ShrekTheThird_Manager
    {
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
            0x082C4D60,
            0x082D8E1C,
            0x082DC574,
            0x082E47A0,
            0x082EDE84,
            0x082F0ECC,
            0x082F1DF4,
            0x082F6C44,
            0x082F71D0,
            0x082F9474,
            0x08302DB0,
            0x083046B8,
            0x08308538,
            0x0830EF94,
            0x08315094,
        };

        public override uint[] ScriptPointers => new uint[]
        {
            0x080919B8, // genericNPC
            0x08091A30, // leprechaunOneShot
            0x08091AF4, // merlinConfused
            0x08091B50, // merlinSetup
            0x08091BD0, // artieBossNPC
            0x08091C78, // boss1NPC
            0x08091D10, // fallDown
            0x08091D8C, // talkToVictim
            0x08091DE4, // merlinSpeaks
            0x08091E38, // merlinWarp
            0x08092058, // script_waitForInputOrTime
            0x08092830, // gameIntroCutscene
            0x08092898, // Q4L4OutroCutscene
            0x08092900, // gameOutroCutscene
            0x08092C80, // waitForPagedText
            0x08092D00, // notFound
            0x08092F00, // waitForPagedText
            0x08092F68, // missing
            0x08092FB8, // merlinNeedMoreMagic
            0x08093010, // merlinMessedUp
            0x08093058, // Q1L1Intro
            0x080930F4, // Q1L1Outro
            0x0809313C, // Q1L2Intro
            0x080931D8, // Q1L3Intro
            0x0809322C, // Q1L3Outro
            0x08093274, // Q1L4Intro
            0x080932EC, // Q1L4Outro
            0x08093334, // Q1L5Intro
            0x080933F4, // Q1L5Outro
            0x0809346C, // Q1L6Intro
            0x080935E0, // Q1L6Warp
            0x08093658, // Q1L6Outro
            0x080936A0, // Q2L1Intro
            0x080936F4, // Q2L2Intro
            0x080937D8, // Q2L2Outro
            0x08093820, // Q2L3Outro
            0x08093868, // Q2L4Outro
            0x080938B0, // Q2L5Intro
            0x08093970, // Q2L5Outro
            0x080939B8, // Q2L6Warp
            0x08093A0C, // Q2L6Outro
            0x08093A54, // Q3L1Intro
            0x08093AA8, // Q3L1Outro
            0x08093AF0, // Q3L2Intro
            0x08093B44, // Q3L3Intro
            0x08093B98, // Q3L3Outro
            0x08093BE0, // Q3L4Outro
            0x08093C28, // Q3L5Intro
            0x08093CA0, // Q3L6Warp
            0x08093D18, // Q3L6Outro
            0x08093D60, // Q4L1Intro
            0x08093DD8, // Q4L2Intro
            0x08093E50, // Q4L4Intro
            0x08093EA4, // Q4L4Outro
            0x08093EEC, // Q4L6Act1
            0x08093F40, // Q4L6Act2
            0x08093FB8, // Q4L6Act3
            0x0809407C, // jumpAndAttack
            0x080940D4, // collectFairies
            0x0809414C, // MerlinItems
            0x080941A0, // wallClimb
            0x080941F4, // treeHelp
            0x08094244, // stench
            0x08094298, // poisonApple
            0x080942F0, // pinocchioScript
            0x080943FC, // hasEnough
            0x080944F8, // notEnough
            0x080945E8, // lastMerlinScript
            0x08094650, // merlinMessedUpDlg
            0x080946A8, // Q1L1OutroDlg
            0x08094724, // Q1L3OutroDlg
            0x080947C4, // Q1L4OutroDlg
            0x08094864, // Q1L6OutroDlg
            0x08094928, // Q2L2OutroDlg
            0x08094980, // Q2L3OutroDlg
            0x080949FC, // Q2L4OutroDlg
            0x08094A78, // Q2L5OutroDlg
            0x08094AD0, // Q2L6OutroDlg
            0x08094B70, // Q3L1OutroDlg
            0x08094BEC, // Q3L3OutroDlg
            0x08094C44, // Q3L4OutroDlg
            0x08094C9C, // Q3L6OutroDlg
            0x08094D3C, // Q4L5IntroDlg
            0x08094DE8, // merlinNotEnoughPlaybills
            0x08094E48, // merlinEnoughPlaybills
            0x080EBB0C, // "waiting"
            0x080EBBFC, // health_and_safety
            0x080EBD14, // movie_credits
            0x080ED5C4, // powerupKeyDrop
            0x080ED628, // powerupFreezeDrop
            0x080ED688, // powerupBombDrop
            0x080ED6F4, // powerupInvincibilityDrop
            0x080ED750, // powerupInvisibilityDrop
            0x080ED820, // fallUp
            0x080ED8B0, // fallDown
            0x080ED97C, // powerupKeyGrab
            0x080EDA0C, // powerupKey
            0x080EDA68, // powerupFreezeGrab
            0x080EDB10, // powerupFreeze
            0x080EDB68, // powerupBombGrab
            0x080EDC00, // powerupBomb
            0x080EDC64, // powerupInvincibilityGrab
            0x080EDD00, // powerupInvincibility
            0x080EDD54, // powerupInvisibilityGrab
            0x080EDDEC, // powerupInvisibility
            0x080EDE48, // powerupHealthLarge
            0x080EDEFC, // powerupHealthSmall
            0x080EDFB0, // powerupHealthSuper
            0x080EE054, // powerupKeyDropFirst
            0x080EE0D0, // powerupFreezeExplode
            0x080EE1B0, // powerupBombExplode
            0x080EE42C, // goRight
            0x080EE490, // goDown
            0x080EE4F0, // goRightWitch
            0x080EE620, // goLeft
            0x080EE684, // goUp
            0x080EE73C, // witchShoot
            0x080EE7C8, // goLeftWitch
            0x080EE84C, // witchScript
            0x080EE940, // peasantPatrol
            0x080EEA3C, // mainLoop
            0x080EEB2C, // normalPeasant
            0x080EEC18, // attacks
            0x080EED08, // mainLoop
            0x080EEE04, // mainLoop
            0x080EEEF4, // crossbowElfScript
            0x080EEFB4, // spike
            0x080EEFF4, // waterSpike
            0x080EF030, // stench
            0x080EF070, // mainLoop
            0x080EF160, // angryPeasant
            0x080EF214, // mainLoop
            0x080EF304, // angryCrossbowElfScript
            0x080EF398, // mainLoop
            0x080EF488, // spikedCeilingScript
            0x080EF504, // followNearestPlayer
            0x080EF560, // faceNearestPlayer
            0x080EF628, // setupStandardHitScripts
            0x080EF6F8, // gotoAttack
            0x080EF7FC, // gotoAttack
            0x080EF8EC, // patrolScript
            0x080EF97C, // setupDefaultAIParams
            0x080EF9FC, // setupStaticAIExceptions
            0x080EFA88, // goRightWitchFirst
            0x080EFAE4, // goLeftWitchFirst
            0x080EFB40, // shieldBlockWitch
            0x080EFBD4, // shootArrow
            0x080EFC7C, // shootArrowDiagonal
            0x080EFD0C, // gotoAttack
            0x080EFDFC, // elfPatrolScript
            0x080EFE8C, // spikedCeilingFall
            0x080EFF50, // spikedCeilingRise
            0x080EFFBC, // freeze
            0x080F0090, // "TestLeft"
            0x080F01A0, // "TestRight"
            0x080F0290, // handleShieldBlock
            0x080F0338, // pause
            0x080F0434, // die
            0x080F052C, // ouch
            0x080F061C, // AITakeButtDamage
            0x080F0740, // AITakeShrekOrDonkeyDamage
            0x080F0808, // AITakeShieldDamage
            0x080F089C, // die
            0x080F098C, // AITakePIBDamage
            0x080F0A54, // boom
            0x080F0B44, // AITakeGBDamage
            0x080F0C0C, // AITakeUnawareDamage
            0x080F0CF0, // AIDieNow
            0x080F0D98, // AIDieIfVisible
            0x080F0E14, // setupUnawareHits
            0x080F0F24, // bananaScript
            0x080F0FB0, // powerPieceScript
            0x080F106C, // sandBagSwitchScript
            0x080F1190, // crumblingBrick
            0x080F1214, // breakableStoneScript
            0x080F12A4, // brickWall
            0x080F1310, // rockWall
            0x080F1384, // doorLocked
            0x080F13E0, // pressureDoor
            0x080F144C, // pressureDoorOpened
            0x080F14BC, // drawBridgeLever
            0x080F1554, // sandBagSwitchOperate
            0x080F1658, // "down"
            0x080F1760, // "up"
            0x080F1850, // drawBridge
            0x080F1914, // turningSwitch
            0x080F1994, // hamsterWheel
            0x080F19D0, // acceptHit
            0x080F1A00, // crumblingBrickCrumble
            0x080F1A8C, // DontTakeShrekDamage
            0x080F1AE4, // brickWallBreak
            0x080F1B6C, // doorLockedOpen
            0x080F1BE0, // pressureDoorOpen
            0x080F1C54, // pressureDoorClose
            0x080F1CCC, // drawBridgeLeverOperate
            0x080F1D6C, // drawBridgeUp
            0x080F1DD0, // drawBridgeDown
            0x080F1E3C, // turningSwitchOperate
            0x080F1EEC, // hamsterWheelTurn
            0x080F1FB0, // genericGrabbable
            0x080F208C, // idle
            0x080F217C, // bouncyMushroom
            0x080F2230, // idle
            0x080F2320, // bouncyPlatform
            0x080F23D4, // idle
            0x080F24C4, // bouncyBranch
            0x080F2514, // idle
            0x080F2604, // bouncyPost
            0x080F2654, // idle
            0x080F2744, // bouncyShell
            0x080F2818, // rollingRockSetup
            0x080F28D0, // reactToKick
            0x080F2958, // reactToWater
            0x080F29E0, // reactToPickup
            0x080F2A2C, // reactToPlatform
            0x080F2A68, // reactToDrop
            0x080F2ABC, // bouncyMushroomBounce
            0x080F2B04, // bouncyPlatformBounce
            0x080F2B48, // bouncyBranchBounce
            0x080F2B8C, // bouncyPostBounce
            0x080F2BD0, // bouncyShellBounce
            0x080F2C50, // rockReactToKick
            0x080F2D34, // handleBounce
            0x080F2E18, // batCaveScript
            0x080F2EDC, // pirateDoorScript
            0x080F2F68, // pirateDoorScript2
            0x080F2FFC, // childKilled
        };
    }
    public class GBAVV_ShrekTheThirdUS_Manager : GBAVV_ShrekTheThird_Manager
    {
        public override string[] Languages => new string[]
        {
            "English"
        };

        public override uint[] GraphicsDataPointers => new uint[]
        {
            0x082B0A0C,
            0x082C4AC8,
            0x082C8220,
            0x082D044C,
            0x082D9B30,
            0x082DCB78,
            0x082DDAA0,
            0x082E28F0,
            0x082E2E7C,
            0x082E5120,
            0x082EEA5C,
            0x082F0364,
            0x082F41E4,
            0x082FAC40,
            0x08300D40,
        };

        public override uint[] ScriptPointers => new uint[]
        {
            0x08091908, // genericNPC
            0x08091980, // leprechaunOneShot
            0x08091A44, // merlinConfused
            0x08091AA0, // merlinSetup
            0x08091B20, // artieBossNPC
            0x08091BC8, // boss1NPC
            0x08091C60, // fallDown
            0x08091CDC, // talkToVictim
            0x08091D34, // merlinSpeaks
            0x08091D88, // merlinWarp
            0x08091FA8, // script_waitForInputOrTime
            0x08092780, // gameIntroCutscene
            0x080927E8, // Q4L4OutroCutscene
            0x08092850, // gameOutroCutscene
            0x08092BD0, // waitForPagedText
            0x08092C50, // notFound
            0x08092E50, // waitForPagedText
            0x08092EB8, // missing
            0x08092F08, // merlinNeedMoreMagic
            0x08092F60, // merlinMessedUp
            0x08092FA8, // Q1L1Intro
            0x08093044, // Q1L1Outro
            0x0809308C, // Q1L2Intro
            0x08093128, // Q1L3Intro
            0x0809317C, // Q1L3Outro
            0x080931C4, // Q1L4Intro
            0x0809323C, // Q1L4Outro
            0x08093284, // Q1L5Intro
            0x08093344, // Q1L5Outro
            0x080933BC, // Q1L6Intro
            0x08093530, // Q1L6Warp
            0x080935A8, // Q1L6Outro
            0x080935F0, // Q2L1Intro
            0x08093644, // Q2L2Intro
            0x08093728, // Q2L2Outro
            0x08093770, // Q2L3Outro
            0x080937B8, // Q2L4Outro
            0x08093800, // Q2L5Intro
            0x080938C0, // Q2L5Outro
            0x08093908, // Q2L6Warp
            0x0809395C, // Q2L6Outro
            0x080939A4, // Q3L1Intro
            0x080939F8, // Q3L1Outro
            0x08093A40, // Q3L2Intro
            0x08093A94, // Q3L3Intro
            0x08093AE8, // Q3L3Outro
            0x08093B30, // Q3L4Outro
            0x08093B78, // Q3L5Intro
            0x08093BF0, // Q3L6Warp
            0x08093C68, // Q3L6Outro
            0x08093CB0, // Q4L1Intro
            0x08093D28, // Q4L2Intro
            0x08093DA0, // Q4L4Intro
            0x08093DF4, // Q4L4Outro
            0x08093E3C, // Q4L6Act1
            0x08093E90, // Q4L6Act2
            0x08093F08, // Q4L6Act3
            0x08093FCC, // jumpAndAttack
            0x08094024, // collectFairies
            0x0809409C, // MerlinItems
            0x080940F0, // wallClimb
            0x08094144, // treeHelp
            0x08094194, // stench
            0x080941E8, // poisonApple
            0x08094240, // pinocchioScript
            0x0809434C, // hasEnough
            0x08094448, // notEnough
            0x08094538, // lastMerlinScript
            0x080945A0, // merlinMessedUpDlg
            0x080945F8, // Q1L1OutroDlg
            0x08094674, // Q1L3OutroDlg
            0x08094714, // Q1L4OutroDlg
            0x080947B4, // Q1L6OutroDlg
            0x08094878, // Q2L2OutroDlg
            0x080948D0, // Q2L3OutroDlg
            0x0809494C, // Q2L4OutroDlg
            0x080949C8, // Q2L5OutroDlg
            0x08094A20, // Q2L6OutroDlg
            0x08094AC0, // Q3L1OutroDlg
            0x08094B3C, // Q3L3OutroDlg
            0x08094B94, // Q3L4OutroDlg
            0x08094BEC, // Q3L6OutroDlg
            0x08094C8C, // Q4L5IntroDlg
            0x08094D38, // merlinNotEnoughPlaybills
            0x08094D98, // merlinEnoughPlaybills
            0x080DE28C, // "waiting"
            0x080DE37C, // health_and_safety
            0x080DE410, // movie_credits
            0x080DFCC0, // powerupKeyDrop
            0x080DFD24, // powerupFreezeDrop
            0x080DFD84, // powerupBombDrop
            0x080DFDF0, // powerupInvincibilityDrop
            0x080DFE4C, // powerupInvisibilityDrop
            0x080DFF1C, // fallUp
            0x080DFFAC, // fallDown
            0x080E0078, // powerupKeyGrab
            0x080E0108, // powerupKey
            0x080E0164, // powerupFreezeGrab
            0x080E020C, // powerupFreeze
            0x080E0264, // powerupBombGrab
            0x080E02FC, // powerupBomb
            0x080E0360, // powerupInvincibilityGrab
            0x080E03FC, // powerupInvincibility
            0x080E0450, // powerupInvisibilityGrab
            0x080E04E8, // powerupInvisibility
            0x080E0544, // powerupHealthLarge
            0x080E05F8, // powerupHealthSmall
            0x080E06AC, // powerupHealthSuper
            0x080E0750, // powerupKeyDropFirst
            0x080E07CC, // powerupFreezeExplode
            0x080E08AC, // powerupBombExplode
            0x080E0B28, // goRight
            0x080E0B8C, // goDown
            0x080E0BEC, // goRightWitch
            0x080E0D1C, // goLeft
            0x080E0D80, // goUp
            0x080E0E38, // witchShoot
            0x080E0EC4, // goLeftWitch
            0x080E0F48, // witchScript
            0x080E103C, // peasantPatrol
            0x080E1138, // mainLoop
            0x080E1228, // normalPeasant
            0x080E1314, // attacks
            0x080E1404, // mainLoop
            0x080E1500, // mainLoop
            0x080E15F0, // crossbowElfScript
            0x080E16B0, // spike
            0x080E16F0, // waterSpike
            0x080E172C, // stench
            0x080E176C, // mainLoop
            0x080E185C, // angryPeasant
            0x080E1910, // mainLoop
            0x080E1A00, // angryCrossbowElfScript
            0x080E1A94, // mainLoop
            0x080E1B84, // spikedCeilingScript
            0x080E1C00, // followNearestPlayer
            0x080E1C5C, // faceNearestPlayer
            0x080E1D24, // setupStandardHitScripts
            0x080E1DF4, // gotoAttack
            0x080E1EF8, // gotoAttack
            0x080E1FE8, // patrolScript
            0x080E2078, // setupDefaultAIParams
            0x080E20F8, // setupStaticAIExceptions
            0x080E2184, // goRightWitchFirst
            0x080E21E0, // goLeftWitchFirst
            0x080E223C, // shieldBlockWitch
            0x080E22D0, // shootArrow
            0x080E2378, // shootArrowDiagonal
            0x080E2408, // gotoAttack
            0x080E24F8, // elfPatrolScript
            0x080E2588, // spikedCeilingFall
            0x080E264C, // spikedCeilingRise
            0x080E26B8, // freeze
            0x080E278C, // "TestLeft"
            0x080E289C, // "TestRight"
            0x080E298C, // handleShieldBlock
            0x080E2A34, // pause
            0x080E2B30, // die
            0x080E2C28, // ouch
            0x080E2D18, // AITakeButtDamage
            0x080E2E3C, // AITakeShrekOrDonkeyDamage
            0x080E2F04, // AITakeShieldDamage
            0x080E2F98, // die
            0x080E3088, // AITakePIBDamage
            0x080E3150, // boom
            0x080E3240, // AITakeGBDamage
            0x080E3308, // AITakeUnawareDamage
            0x080E33EC, // AIDieNow
            0x080E3494, // AIDieIfVisible
            0x080E3510, // setupUnawareHits
            0x080E3620, // bananaScript
            0x080E36AC, // powerPieceScript
            0x080E3768, // sandBagSwitchScript
            0x080E388C, // crumblingBrick
            0x080E3910, // breakableStoneScript
            0x080E39A0, // brickWall
            0x080E3A0C, // rockWall
            0x080E3A80, // doorLocked
            0x080E3ADC, // pressureDoor
            0x080E3B48, // pressureDoorOpened
            0x080E3BB8, // drawBridgeLever
            0x080E3C50, // sandBagSwitchOperate
            0x080E3D54, // "down"
            0x080E3E5C, // "up"
            0x080E3F4C, // drawBridge
            0x080E4010, // turningSwitch
            0x080E4090, // hamsterWheel
            0x080E40CC, // acceptHit
            0x080E40FC, // crumblingBrickCrumble
            0x080E4188, // DontTakeShrekDamage
            0x080E41E0, // brickWallBreak
            0x080E4268, // doorLockedOpen
            0x080E42DC, // pressureDoorOpen
            0x080E4350, // pressureDoorClose
            0x080E43C8, // drawBridgeLeverOperate
            0x080E4468, // drawBridgeUp
            0x080E44CC, // drawBridgeDown
            0x080E4538, // turningSwitchOperate
            0x080E45E8, // hamsterWheelTurn
            0x080E46AC, // genericGrabbable
            0x080E4788, // idle
            0x080E4878, // bouncyMushroom
            0x080E492C, // idle
            0x080E4A1C, // bouncyPlatform
            0x080E4AD0, // idle
            0x080E4BC0, // bouncyBranch
            0x080E4C10, // idle
            0x080E4D00, // bouncyPost
            0x080E4D50, // idle
            0x080E4E40, // bouncyShell
            0x080E4F14, // rollingRockSetup
            0x080E4FCC, // reactToKick
            0x080E5054, // reactToWater
            0x080E50DC, // reactToPickup
            0x080E5128, // reactToPlatform
            0x080E5164, // reactToDrop
            0x080E51B8, // bouncyMushroomBounce
            0x080E5200, // bouncyPlatformBounce
            0x080E5244, // bouncyBranchBounce
            0x080E5288, // bouncyPostBounce
            0x080E52CC, // bouncyShellBounce
            0x080E534C, // rockReactToKick
            0x080E5430, // handleBounce
            0x080E5514, // batCaveScript
            0x080E55D8, // pirateDoorScript
            0x080E5664, // pirateDoorScript2
            0x080E56F8, // childKilled
        };
    }
}