using System.Collections.Generic;

namespace R1Engine
{
    public class GBAVV_MadagascarOperationPenguinManager : GBAVV_Volume_BaseManager
    {
        // Metadata
        public override int VolumesCount => 2;

        // Localization
        public override string[] Languages => new string[]
        {
            "English"
        };

        // Graphics
        public override uint[] GraphicsDataPointers => new uint[]
        {
            0x081985D8,
            0x0819A66C,
            0x0819C804,
            0x0819DD4C,
            0x0819DE0C,
            0x081A01E8,
            0x081A03C0,
            0x081A0D1C,
            0x081A351C,
            0x081A3B88,
            0x081A58B8,
            0x081A6AD0,
            0x081A891C,
            0x081A9FB8,
            0x081AA2D4,
            0x081AA578,
            0x081AD074,
            0x081AE80C,
            0x081AF0D0,
            0x081B14FC,
            0x081B3814,
            0x081B3D54,
            0x081B41CC,
            0x081B4D30,
            0x081B7794,
            0x081B8438,
            0x081B91F8,
            0x081BA414,
            0x081BBB5C,
            0x081BDFF8,
            0x081BE0FC,
            0x081BE1BC,
            0x081BE3C0,
            0x081BE434,
            0x081BEDB0,
            0x081BF16C,
            0x081C0A98,
            0x081C1030,
            0x081C28B4,
            0x081C4524,
            0x081C45D4,
            0x081CF1E4,
            0x081D121C,
            0x081D1EB0,
            0x081D3908,
            0x081D498C,
            0x081D559C,
            0x081D85F8,
            0x081D8D74,
            0x081D94D8,
            0x081DA30C,
            0x081DC5D8,
            0x081DCEAC,
            0x081DDD84,
            0x081DFFE0,
            0x081E11B0,
            0x081EA860,
            0x081EC744,
        };

        // Scripts
        public override Dictionary<int, GBAVV_ScriptCommand.CommandType> ScriptCommands => new Dictionary<int, GBAVV_ScriptCommand.CommandType>()
        {
            [1101] = GBAVV_ScriptCommand.CommandType.Name,
            [1106] = GBAVV_ScriptCommand.CommandType.Return,
            [0502] = GBAVV_ScriptCommand.CommandType.Dialog,
        };
        public override uint[] ScriptPointers => new uint[]
        {
            0x080A6F10, // movie_license
            0x080A6FA8, // movie_credits
            0x080A704C, // Miss00_CS_Post
            0x080A7148, // Miss16_CS_Post
            0x080A78FC, // Miss20_CS_Post
            0x080A7ADC, // movie_intro
            0x080A7BD8, // waitForPagedText
            0x080A7C48, // JpegPleaseWait
            0x080A7CCC, // waitForFlcOrA
            0x080C95C4, // script_waitForInputOrTime
            0x080CA1A0, // christmasNPC
            0x080CA1F4, // genericNPC
            0x080CA24C, // infoNPC
            0x080CA2BC, // facePlayer
            0x080CA3D4, // monkeyQuietNPC
            0x080CA4D8, // barfScript
            0x080CA600, // fallDown
            0x080CA67C, // talkToVictim
            0x080CA6E0, // christmasIdle
            0x080CA728, // npcPlayTalk
            0x080CA758, // npcPlayIdle
            0x080CA828, // barf
            0x080CAF64, // invisibleCandySpawner
            0x080CAFC8, // treeOrnament1
            0x080CB008, // treeOrnament2
            0x080CB048, // treeOrnament3
            0x080CB088, // treeOrnament4
            0x080CB0C8, // genericGrass
            0x080CB128, // genericBush
            0x080CB19C, // genericBug
            0x080CB244, // genericGrassIdle
            0x080CB294, // genericGrassOpen
            0x080CB2E4, // genericGrassClose
            0x080CB324, // genericBushIdle
            0x080CB374, // genericBushTouched
            0x080CB3D8, // genericBugPatrol
            0x080CB434, // genericBugWalkLeft
            0x080CB490, // genericBugWalkRight
            0x080CC174, // notFound
            0x080CC78C, // waitForPagedText
            0x080CC7F4, // missing
            0x080CC844, // helpHub_enterTunnel
            0x080CC898, // helpHub_HoH
            0x080CC8F8, // helpHub_bassKnuckles
            0x080CC958, // helpHub_smileAndWave
            0x080CC9B4, // helpHub_parachute
            0x080CCA0C, // helpHub_dive
            0x080CCA64, // helpHub_jetPack
            0x080CCAC0, // helpHub_fruitBasket
            0x080CCB20, // helpHub_sleeperGuards
            0x080CCB74, // Miss00_Pre
            0x080CCCA8, // Miss00_Act_Skipper
            0x080CCD04, // Miss00_Act_Kowalski
            0x080CCD58, // Miss00_Post
            0x080CCDB8, // Miss01_PenguinHub_Pre
            0x080CCF1C, // Miss01_PenguinHub_Act_Skipper
            0x080CCF84, // Miss01_PenguinHub_Trig_Kowalski
            0x080CCFEC, // Miss01_PenguinHub_Act_Kowalski
            0x080CD048, // Miss01_Training_Pre
            0x080CD0BC, // Miss01_Training_Act_Skipper0
            0x080CD124, // Miss01_Training_Act_Skipper1
            0x080CD184, // Miss01_Training_Info_0a
            0x080CD1E4, // Miss01_Training_Info_1a
            0x080CD244, // Miss01_Training_Info_2a
            0x080CD2A4, // Miss01_Training_Info_3a
            0x080CD304, // Miss01_Training_Info_4a
            0x080CD364, // Miss01_Training_Info_5a
            0x080CD3C4, // Miss01_Training_Info_6a
            0x080CD424, // Miss01_Training_Info_7a
            0x080CD484, // Miss01_Training_Info_8a
            0x080CD4E4, // Miss01_Training_Info_9a
            0x080CD548, // Miss01_Training_Info_10a
            0x080CD5AC, // Miss01_Training_Info_11a
            0x080CD610, // Miss01_Training_Info_12a
            0x080CD670, // Miss01_Training_Post
            0x080CD6D0, // Miss02_PenguinHub_Pre
            0x080CD738, // Miss02_PenguinHub_Act_Skipper
            0x080CD7A0, // Miss02_PenguinHub_Trig_Kowalski
            0x080CD808, // Miss02_PenguinHub_Act_Kowalski
            0x080CD874, // Miss02_TobogganHub_Trig_Kowalski
            0x080CD928, // Miss02_TobogganHub_Act_Kowalski0
            0x080CD9B8, // Miss02_TobogganHub_Act_Kowalski1
            0x080CDA14, // Miss02_Toboggan_Pre
            0x080CDA74, // Miss02_Toboggan_Post
            0x080CDB28, // Miss03_PenguinHub_Pre
            0x080CDC74, // Miss03_PenguinHub_Act_Skipper
            0x080CDCD8, // Miss03_PenguinHub_Trig_Rico
            0x080CDD40, // Miss03_PenguinHub_Act_Kowalski
            0x080CDDA8, // Miss03_MelmanArea_Act_Melman
            0x080CDE08, // Miss03_MelmanArea_Post
            0x080CDF64, // Miss04_PenguinHub_Pre
            0x080CE0D4, // Miss04_PenguinHub_Act_Skipper
            0x080CE138, // Miss04_PenguinHub_Trig_Rico
            0x080CE20C, // Miss04_PenguinHub_Act_Kowalski
            0x080CE2BC, // Miss04_ArcticPen_Trig_PolarBear
            0x080CE3D8, // Miss04_ArcticPen_Act_PolarBear0
            0x080CE440, // Miss04_ArcticPen_Act_PolarBear1
            0x080CE4A0, // Miss04_ArcticPen_NoExit
            0x080CE500, // Miss04_ArcticPen_Post
            0x080CE560, // Miss05_PenguinHub_Pre
            0x080CE760, // Miss05_PenguinHub_Act_Skipper
            0x080CE7C8, // Miss05_PenguinHub_Trig_Kowalski
            0x080CE92C, // Miss05_PenguinHub_Act_Kowalski
            0x080CE990, // Miss05_MelmanMinigame_Pre
            0x080CE9FC, // melman
            0x080CEAEC, // Miss05_MelmanMinigame_Post
            0x080CEB44, // Miss05_VisitorCourtyard_Pre
            0x080CEBAC, // Miss05_VisitorCourtyard_NoExit
            0x080CEC14, // Miss05_VisitorCourtyard_Post
            0x080CEC74, // Miss06_PenguinHub_Pre
            0x080CED78, // Miss06_PenguinHub_Act_Skipper
            0x080CEDDC, // Miss06_PenguinHub_Trig_Rico
            0x080CEED4, // Miss06_PenguinHub_Act_Kowalski
            0x080CEF38, // Miss06_MartyArea_Act_Marty
            0x080CEF98, // Miss06_MartyArea_Post
            0x080CF088, // Miss07_PenguinHub_Pre
            0x080CF144, // Miss07_PenguinHub_Act_Skipper
            0x080CF218, // Miss07_PenguinHub_Trig_Kowalski
            0x080CF280, // Miss07_PenguinHub_Act_Kowalski
            0x080CF304, // Miss07_VisitorArea_Pre
            0x080CF3B4, // Miss07_VisitorArea_Act_Ducky
            0x080CF4C8, // Miss07_VisitorArea_Post
            0x080CF600, // Miss08_PenguinHub_Pre
            0x080CF6E0, // Miss08_PenguinHub_Act_Skipper
            0x080CF748, // Miss08_PenguinHub_Trig_Kowalski
            0x080CF840, // Miss08_PenguinHub_Act_Kowalski
            0x080CF8E8, // Miss08_GloriaArea_Pre
            0x080CFA28, // Miss08_GloriaArea_Act_Gloria
            0x080CFA88, // Miss08_GloriaArea_Post
            0x080CFBE4, // Miss09_PenguinHub_Pre
            0x080CFCDC, // Miss09_PenguinHub_Act_Skipper
            0x080CFD44, // Miss09_PenguinHub_Trig_Kowalski
            0x080CFDF4, // Miss09_PenguinHub_Act_Kowalski
            0x080CFE60, // Miss09_TobogganHub_Trig_Kowalski
            0x080CFECC, // Miss09_TobogganHub_Act_Kowalski0
            0x080CFF2C, // Miss09_Toboggan_Post
            0x080CFF98, // Miss10_PenguinHub_Pre
            0x080D0054, // Miss10_PenguinHub_Act_Skipper
            0x080D00B8, // Miss10_PenguinHub_Trig_Rico
            0x080D01B0, // Miss10_PenguinHub_Act_Kowalski
            0x080D0234, // Miss10_BirdCages_Pre
            0x080D0294, // Miss10_BirdCages_NoExit
            0x080D02F4, // Miss10_BirdCages_Post
            0x080D0354, // Miss11_PenguinHub_Pre
            0x080D04E8, // Miss11_PenguinHub_Act_Skipper
            0x080D054C, // Miss11_PenguinHub_Trig_Rico
            0x080D06D4, // Miss11_PenguinHub_Act_Kowalski
            0x080D0738, // Miss11_AlexArea_Act_Alex
            0x080D0798, // Miss11_AlexArea_Post
            0x080D0960, // Miss12_PenguinHub_Pre
            0x080D09D4, // Miss12_PenguinHub_Act_Skipper
            0x080D0A3C, // Miss12_PenguinHub_Trig_Kowalski
            0x080D0AA4, // Miss12_PenguinHub_Act_Kowalski
            0x080D0B04, // Miss12_MonkeyCage_Pre
            0x080D0B6C, // Miss12_MonkeyCage_Trig_Monkeys
            0x080D0C40, // Miss12_MonkeyCage_Act_Monkeys0
            0x080D0CCC, // Miss12_MonkeyCage_Act_Monkeys1
            0x080D0D74, // Miss12_MonkeyCage_Post
            0x080D0DD4, // Miss13_PenguinHub_Pre
            0x080D0E60, // Miss13_PenguinHub_Act_Skipper
            0x080D0EC8, // Miss13_PenguinHub_Trig_Kowalski
            0x080D0F78, // Miss13_PenguinHub_Act_Kowalski
            0x080D0FE4, // Miss13_TobogganHub_Trig_Kowalski
            0x080D1050, // Miss13_TobogganHub_Act_Kowalski0
            0x080D10B0, // Miss13_Tobobban_Post
            0x080D1164, // Miss14_PenguinHub_Pre
            0x080D1268, // Miss14_PenguinHub_Act_Skipper
            0x080D12CC, // Miss14_PenguinHub_Trig_Rico
            0x080D137C, // Miss14_PenguinHub_Act_Kowalski
            0x080D13D8, // Miss14_Trig_Guard
            0x080D142C, // Miss14_Post
            0x080D148C, // Miss15_PenguinHub_Pre
            0x080D1668, // Miss15_PenguinHub_Act_Skipper
            0x080D16D0, // Miss15_PenguinHub_Trig_Kowalski
            0x080D1738, // Miss15_PenguinHub_Act_Kowalski
            0x080D1798, // Miss15_Underground_Post
            0x080D17F8, // Miss16_PenguinHub_Pre
            0x080D186C, // Miss16_PenguinHub_Act_Skipper
            0x080D18D4, // Miss16_PenguinHub_Trig_Kowalski
            0x080D193C, // Miss16_PenguinHub_Act_Kowalski
            0x080D19A0, // Miss16_MrChewMinigame0_Pre
            0x080D1A4C, // Miss16_MrChewMinigame1_Pre
            0x080D1AB0, // Miss16_MrChewMinigame_Post
            0x080D1B78, // Miss17_BoatHub_Pre
            0x080D1BE8, // Miss17_BoatHub_Act_Skipper
            0x080D1C4C, // Miss17_BoatHub_Act_Kowalski
            0x080D1CA8, // Miss17_Ship1_NoExit
            0x080D1D04, // Miss17_Ship1_Post
            0x080D1D60, // Miss18_BoatHub_Pre
            0x080D1E18, // Miss18_BoatHub_Act_Skipper
            0x080D1E7C, // Miss18_BoatHub_Act_Kowalski
            0x080D1ED8, // Miss18_Ship2_NoExit
            0x080D1F34, // Miss18_Ship2_Post
            0x080D1F90, // Miss19_BoatHub_Pre
            0x080D2000, // Miss19_BoatHub_Act_Skipper
            0x080D2064, // Miss19_BoatHub_Act_Kowalski
            0x080D20C4, // Miss19_Antarctica_Post
            0x080D21F8, // Miss20_BoatHub_Pre
            0x080D22B0, // Miss20_BoatHub_Act_Skipper
            0x080D2314, // Miss20_BoatHub_Act_Kowalski
            0x080D2374, // Miss20_Madagascar_Pre
            0x080D24FC, // Miss20_Madagascar_Act_Skipper
            0x080D2570, // Miss20_Madagascar_Act_Kowalski
            0x080D25D8, // Miss20_Madagascar_Act_Gloria0
            0x080D2640, // Miss20_Madagascar_Act_Gloria1
            0x080D26A8, // Miss20_Madagascar_Act_Melman0
            0x080D2710, // Miss20_Madagascar_Act_Melman1
            0x080D2778, // Miss20_Madagascar_Act_Julian0
            0x080D27E0, // Miss20_Madagascar_Act_Julian1
            0x080D2844, // Miss20_Madagascar_Act_Alex1
            0x080D28AC, // Miss20_Madagascar_Act_Marty1
            0x080D2914, // Miss20_Madagascar_Act_Skipper1
            0x080D297C, // Miss20_Madagascar_Act_Kowalski1
            0x080D29D8, // Miss20_Foosa_Pre
            0x080D2AC4, // Miss20_Foosa_Post
            0x080D2BD8, // stealth
            0x080D2CC8, // stealthFailure
            0x080D2D24, // time
            0x080D2E14, // timeFailure
            0x080D2E78, // congratulations
            0x080D2F68, // bestBananas
            0x080D2FBC, // congratulations
            0x080D30AC, // allBananas
            0x080D3100, // congratulations
            0x080D31F0, // moreHealth
            0x080DEEFC, // powerupHealthLarge
            0x080DEF98, // powerupHealthSmall
            0x080DF49C, // goLeft
            0x080DF4F4, // goRight
            0x080DF554, // goRightWithSFX
            0x080DF5B8, // goDown
            0x080DF614, // dropDown
            0x080DF6A4, // SpotlighGuardSleep
            0x080E0F20, // resetSleepTimer
            0x080E10A0, // emptyScript
            0x080E10D0, // goLeftWithSFX
            0x080E1134, // goUp
            0x080E118C, // climbUp
            0x080E1214, // groundBounce
            0x080E1304, // groundPatrolOneHPScript
            0x080E13C0, // groundInvulnerablePatrolScript
            0x080E1480, // hubPorcupineScript
            0x080E1538, // airHBounce
            0x080E1628, // airHOneHPPatrolScript
            0x080E16CC, // airHBounce
            0x080E17BC, // airHOneHPDropperPatrolScript
            0x080E186C, // aBounce
            0x080E195C, // spiderLinearPatrolScript
            0x080E1A10, // spotlightGuardScript
            0x080E1AB0, // snowBoulderScript
            0x080E1B00, // scorpionBlueScript
            0x080E1B48, // scorpionBlueGreenScript
            0x080E1B8C, // scorpionGreyScript
            0x080E1BD4, // scorpionOrangeScript
            0x080E1C1C, // scorpionPurpleScript
            0x080E1C60, // scorpionRedScript
            0x080E1CA0, // ratBrownScript
            0x080E1CE0, // ratWhiteScript
            0x080E1D28, // spiderLinearGreyScript
            0x080E1D70, // spiderLinearRedScript
            0x080E1DBC, // spiderLinearYellowScript
            0x080E1E10, // setupDefaultAIParams
            0x080E1E88, // die
            0x080E1F80, // ouch
            0x080E2070, // AITakeOneDamage
            0x080E20EC, // AIHitNoDamage
            0x080E21C4, // setupStandardHitScripts
            0x080E226C, // handleWallBounces
            0x080E22EC, // toRight
            0x080E23E4, // toLeft
            0x080E250C, // toAttack
            0x080E25FC, // handlePigeonWallBouncesWithAttacks
            0x080E26B0, // spiderHandleFloorCeilBounces
            0x080E275C, // roll
            0x080E284C, // snowBoulderRoll
            0x080E296C, // AIDieNow
            0x080E2A00, // AIPlummetNow
            0x080E2AD4, // AITakeSlashDamage
            0x080E2B78, // AITakeKickDamage
            0x080E35BC, // softGround
            0x080E3648, // wall
            0x080E36B8, // treeBridge
            0x080E372C, // arcticFanScript
            0x080E37BC, // specialCollectableScript
            0x080E3878, // fruitCollectableScript
            0x080E3920, // xmasTreeScript
            0x080E39A0, // areaTriggerScript
            0x080E3A1C, // statsTriggerScript
            0x080E3B40, // fossaSpawnScriptWithDialog
            0x080E3C74, // fossaSpawnScript
            0x080E3DBC, // bossMrChewDialogScript
            0x080E3E78, // gadgetScript
            0x080E3F74, // acceptHit
            0x080E3F9C, // floorCrumble
            0x080E4044, // wallCrumble
            0x080E4098, // bridgeDown
            0x080E40E8, // arcticFanTakeHit
            0x080E416C, // arcticFanDestroy
            0x080E4228, // arcticFanBlow
            0x080E4408, // spawnFossa
            0x080E4474, // fossaKilled
            0x080E44A8, // bossMrChewDecHP
            0x080E5204, // idle
            0x080E52F4, // bouncyPost
            0x080E5358, // bouncyFlower
            0x080E53B0, // bouncyFlowerSimple
            0x080E540C, // airVentScript
            0x080E547C, // jetPackDeadCreate
            0x080E5550, // fruitBasketScript
            0x080E55B8, // snowballsScript
            0x080E563C, // helperPromptScript
            0x080E56AC, // bouncyFlowerRed
            0x080E56F0, // bouncyFlowerWhite
            0x080E5734, // bouncyFlowerBlue
            0x080E5774, // bouncyPostBlue
            0x080E57B4, // bouncyPostRed
            0x080E57F8, // bouncyPostPurple
            0x080E583C, // bouncyBranchGreen
            0x080E5880, // bouncyBranchOrange
            0x080E58C4, // bouncyPostBounce
            0x080E5908, // bouncyFlowerBounce
            0x080E594C, // slingshotPromptR
            0x080E59E0, // B
            0x080E5AD0, // slingshotPromptB
            0x080E5BA0, // lemmingGoRight
            0x080E5D80, // lemmingSpawnerCreate
            0x080E5DF8, // lemmingGoLeft
            0x080E5F08, // patrol
            0x080E6018, // lemmingScript
            0x080E61CC, // lemmingTakeHit
            0x080E62B4, // lemmingSleep
            0x080E6658, // spike
            0x080E66A0, // electricCable
        };

        // Levels
        public override LevInfo[] LevInfos => Levels;
        public static LevInfo[] Levels => new LevInfo[]
        {
            new LevInfo(0, -1, 0, "Adventure", "", "Penguin Pen"),
            new LevInfo(0, 0, 0, "Adventure", "", "Briefing Room"),
            new LevInfo(0, 1, 0, "Adventure", "", "Apartment"),
            new LevInfo(0, 2, 0, "Adventure", "", "Training"),
            new LevInfo(0, 3, 0, "Adventure", "", "Toboggan Courses"),
            new LevInfo(0, 4, 0, "Adventure", "", "Green Circle"),
            new LevInfo(0, 5, 0, "Adventure", "", "Giraffe Pen"),
            new LevInfo(0, 5, 1, "Adventure", "", "Giraffe Pen"),
            new LevInfo(0, 5, 2, "Adventure", "", "Giraffe Pen"),
            new LevInfo(0, 6, 0, "Adventure", "", "Arctic Pen"),
            new LevInfo(0, 6, 1, "Adventure", "", "Arctic Pen"),
            new LevInfo(0, 6, 2, "Adventure", "", "Arctic Pen"),
            new LevInfo(0, 7, 0, "Adventure", "", "Visitor's Courtyard"),
            new LevInfo(0, 7, 1, "Adventure", "", "Visitor's Courtyard"),
            new LevInfo(0, 8, 0, "Adventure", "", "Zebra Pen"),
            new LevInfo(0, 8, 1, "Adventure", "", "Zebra Pen"),
            new LevInfo(0, 8, 2, "Adventure", "", "Zebra Pen"),
            new LevInfo(0, 9, 0, "Adventure", "", "Children's Zoo"),
            new LevInfo(0, 10, 0, "Adventure", "", "Hippo Pen"),
            new LevInfo(0, 10, 1, "Adventure", "", "Hippo Pen"),
            new LevInfo(0, 10, 2, "Adventure", "", "Hippo Pen"),
            new LevInfo(0, 11, 0, "Adventure", "", "Blue Square"),
            new LevInfo(0, 12, 0, "Adventure", "", "Bird Cages"),
            new LevInfo(0, 12, 1, "Adventure", "", "Bird Cages"),
            new LevInfo(0, 13, 0, "Adventure", "", "Lion Pen"),
            new LevInfo(0, 13, 1, "Adventure", "", "Lion Pen"),
            new LevInfo(0, 13, 2, "Adventure", "", "Lion Pen"),
            new LevInfo(0, 14, 0, "Adventure", "", "Monkey Cage"),
            new LevInfo(0, 14, 1, "Adventure", "", "Monkey Cage"),
            new LevInfo(0, 14, 2, "Adventure", "", "Monkey Cage"),
            new LevInfo(0, 15, 0, "Adventure", "", "Black Diamond"),
            new LevInfo(0, 16, 0, "Adventure", "", "Underground"),
            new LevInfo(0, 16, 1, "Adventure", "", "Underground"),
            new LevInfo(0, 17, 0, "Adventure", "", "Subway"),
            new LevInfo(0, 17, 1, "Adventure", "", "Subway"),
            new LevInfo(0, 17, 2, "Adventure", "", "Subway"),
            new LevInfo(1, -1, 0, "Bonus", "", "Engine Room"),
            new LevInfo(1, 0, 0, "Bonus", "", "Control Room"),
            new LevInfo(1, 1, 0, "Bonus", "", "Ship Deck"),
            new LevInfo(1, 2, 0, "Bonus", "", "Below Deck"),
            new LevInfo(1, 3, 0, "Bonus", "", "Antarctica"),
            new LevInfo(1, 4, 0, "Bonus", "", "Madagascar"),
            new LevInfo(1, 4, 1, "Bonus", "", "Madagascar"),
            new LevInfo(1, 4, 2, "Bonus", "", "Madagascar"),
            new LevInfo(1, 4, 3, "Bonus", "", "Madagascar"),
        };
    }
}