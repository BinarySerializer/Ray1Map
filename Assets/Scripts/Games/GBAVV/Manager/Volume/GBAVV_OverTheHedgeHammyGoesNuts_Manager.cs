using System.Collections.Generic;

namespace Ray1Map.GBAVV
{
    public abstract class GBAVV_OverTheHedgeHammyGoesNuts_Manager : GBAVV_Volume_BaseManager
    {
        // Metadata
        public override int VolumesCount => 4;

        // Scripts
        public override Dictionary<int, GBAVV_ScriptCommand.CommandType> ScriptCommands => new Dictionary<int, GBAVV_ScriptCommand.CommandType>()
        {
            [1301] = GBAVV_ScriptCommand.CommandType.Name,
            [1306] = GBAVV_ScriptCommand.CommandType.Return,
            [1502] = GBAVV_ScriptCommand.CommandType.Dialog,
        };

        // Levels
        public override LevInfo[] LevInfos => Levels;
        public static LevInfo[] Levels => new LevInfo[]
        {
            new LevInfo(0, 0, 0, "", "L01_special1_tutorial", "Snow in Autumn"),
            new LevInfo(0, 1, 0, "", "L02_forest1_story", "Golfing"),
            new LevInfo(0, 2, 0, "", "L03_forest2_story", "Don't Get Pushy"),
            new LevInfo(0, 3, 0, "", "L04_backyard1_story", "Boris Bust-Out"),
            new LevInfo(0, 4, 0, "", "L05_forest3_story", "Rise of the Triangle"),
            new LevInfo(0, 5, 0, "", "L06_backyard2_story", "Beaver Day Afternoon"),
            new LevInfo(0, 6, 0, "", "L07_special2_varient1", "Depelter Turbo"),
            new LevInfo(0, 7, 0, "", "L08_forest4_story", "Practice Makes Par-fect"),
            new LevInfo(0, 8, 0, "", "L09_backyard3_story", "Try, Try Again"),
            new LevInfo(0, 9, 0, "", "L10_forest5_story", "The Blasters"),
            new LevInfo(0, 10, 0, "", "L11_backyard4_story", "FANtastic"),
            new LevInfo(0, 11, 0, "", "L12a_houseL1_story", "Promises, Promises"),
            new LevInfo(0, 12, 0, "", "L12b_houseK1_story", "Cool Food"),
            new LevInfo(0, 13, 0, "", "L13_special3_pinball1", "Meet the Family"),
            new LevInfo(0, 14, 0, "", "L14_forest6_story", "Ozzie Test"),
            new LevInfo(0, 15, 0, "", "L15_backyard5_story", "Windmills"),
            new LevInfo(0, 16, 0, "", "L16a_houseL2_story", "Mouse Trap"),
            new LevInfo(0, 17, 0, "", "L16b_houseK2_story", "Ozzie's Trek"),
            new LevInfo(0, 18, 0, "", "L17_construction1_story", "Sign Language"),
            new LevInfo(0, 19, 0, "", "L18_backyard6_story", "Gno More Gnomes"),
            new LevInfo(0, 20, 0, "", "L19_special4_varient2", "Depelter Redux"),
            new LevInfo(0, 21, 0, "", "L20a_houseL3_story", "Cake Time"),
            new LevInfo(0, 22, 0, "", "L20b_houseK3_story", "Too Much Cake"),
            new LevInfo(0, 23, 0, "", "L21_construction2_story", "Vacuums Suck"),
            new LevInfo(0, 24, 0, "", "L22a_houseL4_story", "Vase City"),
            new LevInfo(0, 25, 0, "", "L22b_houseK4_story", "Vase Off"),
            new LevInfo(0, 26, 0, "", "L23_construction3_story", "The Key's the Key"),
            new LevInfo(0, 27, 0, "", "L24a_houseL5_story", "Triplet Trouble"),
            new LevInfo(0, 28, 0, "", "L24b_houseK5_story", "Operation Opossum"),
            new LevInfo(0, 29, 0, "", "L25_special5_pinball2", "WinBall Break"),
            new LevInfo(0, 30, 0, "", "L26_construction4_story", "Loopy Situation"),
            new LevInfo(0, 31, 0, "", "L27a_houseL6_story", "Inside the Closet"),
            new LevInfo(0, 32, 0, "", "L27b_houseK6_story", "FOODFOODFOOD"),
            new LevInfo(0, 33, 0, "", "L28_construction5_story", "Twirl Wind"),
            new LevInfo(0, 34, 0, "", "L29_construction6_story", "Done and Doner"),
            new LevInfo(0, 35, 0, "", "L30_special6_boss", "Boris's 12"),
            new LevInfo(1, 0, 0, "", "L02_forest1_challenge", "Challenge 01"),
            new LevInfo(1, 1, 0, "", "L03_forest2_challenge", "Challenge 02"),
            new LevInfo(1, 2, 0, "", "L04_backyard1_challenge", "Challenge 03"),
            new LevInfo(1, 3, 0, "", "L05_forest3_challenge", "Challenge 04"),
            new LevInfo(1, 4, 0, "", "L06_backyard2_challenge", "Challenge 05"),
            new LevInfo(1, 5, 0, "", "L08_forest4_challenge", "Challenge 06"),
            new LevInfo(1, 6, 0, "", "L09_backyard3_challenge", "Challenge 07"),
            new LevInfo(1, 7, 0, "", "L10_forest5_challenge", "Challenge 08"),
            new LevInfo(1, 8, 0, "", "L11_backyard4_challenge", "Challenge 09"),
            new LevInfo(1, 9, 0, "", "L12a_houseL1_challenge", "Challenge 10"),
            new LevInfo(1, 10, 0, "", "L12b_houseK1_challenge", "Challenge 11"),
            new LevInfo(1, 11, 0, "", "L14_forest6_challenge", "Challenge 12"),
            new LevInfo(1, 12, 0, "", "L15_backyard5_challenge", "Challenge 13"),
            new LevInfo(1, 13, 0, "", "L16a_houseL2_challenge", "Challenge 14"),
            new LevInfo(1, 14, 0, "", "L16b_houseK2_challenge", "Challenge 15"),
            new LevInfo(1, 15, 0, "", "L17_construction1_challenge", "Challenge 16"),
            new LevInfo(1, 16, 0, "", "L18_backyard6_challenge", "Challenge 17"),
            new LevInfo(1, 17, 0, "", "L20a_houseL3_challenge", "Challenge 18"),
            new LevInfo(1, 18, 0, "", "L20b_houseK3_challenge", "Challenge 19"),
            new LevInfo(1, 19, 0, "", "L21_construction2_challenge", "Challenge 20"),
            new LevInfo(1, 20, 0, "", "L22a_houseL4_challenge", "Challenge 21"),
            new LevInfo(1, 21, 0, "", "L22b_houseK4_challenge", "Challenge 22"),
            new LevInfo(1, 22, 0, "", "L23_construction3_challenge", "Challenge 23"),
            new LevInfo(1, 23, 0, "", "L24a_houseL5_challenge", "Challenge 24"),
            new LevInfo(1, 24, 0, "", "L24b_houseK5_challenge", "Challenge 25"),
            new LevInfo(1, 25, 0, "", "L26_construction4_challenge", "Challenge 26"),
            new LevInfo(1, 26, 0, "", "L27a_houseL6_challenge", "Challenge 27"),
            new LevInfo(1, 27, 0, "", "L27b_houseK6_challenge", "Challenge 28"),
            new LevInfo(1, 28, 0, "", "L28_construction5_challenge", "Challenge 29"),
            new LevInfo(1, 29, 0, "", "L29_construction6_challenge", "Challenge 30"),
            new LevInfo(2, 0, 0, "", "L02_forest1_quick", ""),
            new LevInfo(2, 1, 0, "", "L03_forest2_quick", ""),
            new LevInfo(2, 2, 0, "", "L05_forest3_quick", ""),
            new LevInfo(2, 3, 0, "", "L08_forest4_quick", ""),
            new LevInfo(2, 4, 0, "", "L10_forest5_quick", ""),
            new LevInfo(2, 5, 0, "", "L14_forest6_quick", ""),
            new LevInfo(2, 6, 0, "", "L04_backyard1_quick", ""),
            new LevInfo(2, 7, 0, "", "L06_backyard2_quick", ""),
            new LevInfo(2, 8, 0, "", "L09_backyard3_quick", ""),
            new LevInfo(3, 0, 0, "", "L02_forest1_quick", ""),
            new LevInfo(3, 1, 0, "", "L03_forest2_quick", ""),
            new LevInfo(3, 2, 0, "", "L05_forest3_quick", ""),
            new LevInfo(3, 3, 0, "", "L08_forest4_quick", ""),
            new LevInfo(3, 4, 0, "", "L10_forest5_quick", ""),
            new LevInfo(3, 5, 0, "", "L14_forest6_quick", ""),
            new LevInfo(3, 6, 0, "", "L04_backyard1_quick", ""),
        };
    }
    public class GBAVV_OverTheHedgeHammyGoesNutsEU_Manager : GBAVV_OverTheHedgeHammyGoesNuts_Manager
    {
        public override string[] Languages => new string[]
        {
            "Dutch",
            "English",
            "French",
            "Italian",
            "German",
            "Spanish",
        };

        public override uint[] GraphicsDataPointers => new uint[]
        {
            0x081DBFAC,
            0x081E172C,
            0x081E3954,
            0x081E5C98,
            0x081E7B60,
            0x081EBAC0,
            0x081ECD68,
            0x081EFF64,
            0x081F9094,
            0x0821ACA0,
            0x0821CBD4,
            0x0821CC44,
            0x0821CE7C,
            0x0821F7B8,
            0x082275B0,
            0x08229D88,
            0x0822BC78,
            0x0822C0D4,
            0x0822F284,
            0x0822FF88,
        };

        public override uint[] ScriptPointers => new uint[]
        {
            0x08034D88, // script_waitForInputOrTime
            0x080564D0, // L01_Intro
            0x0805654C, // L01_IntroCutscene
            0x080569B4, // L04_Intro
            0x08056A1C, // L04_IntroCutscene
            0x08056C8C, // L04_Outro
            0x08056CF4, // L04_OutroCutscene
            0x08056EEC, // L05_Intro
            0x08056F54, // L05_IntroCutscene
            0x0805729C, // L06_Intro
            0x08057304, // L06_IntroCutscene
            0x08057658, // L07_Intro
            0x080576C0, // L07_IntroCutscene
            0x0805796C, // L07_Outro
            0x080579D4, // L07_OutroCutscene
            0x08057C44, // L08_Intro
            0x08057CAC, // L08_IntroCutscene
            0x08057E80, // L10_Intro
            0x08057EE8, // L10_IntroCutscene
            0x080582FC, // L12a_Intro
            0x08058364, // L12a_IntroCutscene
            0x080586A0, // L13_Intro
            0x08058708, // L13_IntroCutscene
            0x08058ED0, // L14_Intro
            0x08058F38, // L14_IntroCutscene
            0x080590D0, // L17_Intro
            0x08059138, // L17_IntroCutscene
            0x08059408, // L17_Outro
            0x08059470, // L17_OutroCutscene
            0x080595FC, // L18_Outro
            0x08059664, // L18_OutroCutscene
            0x08059844, // L20a_Intro
            0x080598AC, // L20a_IntroCutscene
            0x08059B34, // L22a_Intro
            0x08059B9C, // L22a_IntroCutscene
            0x08059DAC, // L22b_Outro
            0x08059E14, // L22b_OutroCutscene
            0x0805A120, // L24a_Intro
            0x0805A188, // L24a_IntroCutscene
            0x0805A4D0, // L25_Intro
            0x0805A538, // L25_IntroCutscene
            0x0805A8D4, // L27a_Intro
            0x0805A93C, // L27a_IntroCutscene
            0x0805ACCC, // L28_Intro
            0x0805AD34, // L28_IntroCutscene
            0x0805AFA4, // L29_Outro
            0x0805B00C, // L29_OutroCutscene
            0x0805B564, // L30_Intro
            0x0805B5CC, // L30_IntroCutscene
            0x0805B9EC, // L30_Outro
            0x0805BA54, // L30_OutroCutscene
            0x0805C03C, // waitForPagedText
            0x0805C0F4, // waitForPagedTextPage
            0x0805C174, // notFound
            0x0805C544, // waitForPagedText
            0x0805C5C4, // missing
            0x0805C60C, // ID_OUT_01
            0x0805C6CC, // ID_OUT_30
            0x0805C760, // cFailStrokes
            0x0805C7C0, // L01_01_A
            0x0805C82C, // L01_01_B
            0x0805C928, // L01_02_A
            0x0805C9B8, // L01_03_A
            0x0805CA18, // L01_04_A
            0x0805CA78, // L01_98_A
            0x0805CB68, // L02_01_A
            0x0805CBD4, // L02_01_B
            0x0805CC70, // L02_02_A
            0x0805CCDC, // L02_98_A
            0x0805CD9C, // L03_01_A
            0x0805CE38, // L03_01_B
            0x0805CEA4, // L03_01_C
            0x0805CF10, // L03_01_D
            0x0805CFAC, // L03_02_A
            0x0805D00C, // L03_98_A
            0x0805D06C, // L04_01_A
            0x0805D1C8, // L05_01_A
            0x0805D264, // L05_02_A
            0x0805D330, // L06_01_A
            0x0805D3CC, // L06_01_B
            0x0805D468, // L06_98_A
            0x0805D534, // L07_01_A
            0x0805D5A0, // L07_98_A
            0x0805D600, // L08_01_A
            0x0805D75C, // L08_98_A
            0x0805D81C, // L09_01_A
            0x0805D888, // L09_01_B
            0x0805D8F4, // L09_01_C
            0x0805D990, // L09_98_A
            0x0805DA20, // L10_01_A
            0x0805DA8C, // L10_01_B
            0x0805DB28, // L10_98_A
            0x0805DC24, // L11_01_A
            0x0805DCC0, // L11_01_B
            0x0805DD8C, // L11_98_A
            0x0805DE1C, // L12a_01_A
            0x0805DE88, // L12a_01_B
            0x0805DEF4, // L12a_01_C
            0x0805DFC0, // L12b_01_A
            0x0805E05C, // L12b_01_B
            0x0805E0F8, // L12b_98_A
            0x0805E188, // L13_01_A
            0x0805E284, // L13_01_B
            0x0805E2F0, // L13_01_C
            0x0805E3BC, // L13_98_A
            0x0805E47C, // L14_01_A
            0x0805E518, // L14_02_A
            0x0805E578, // L14_98_A
            0x0805E638, // L15_01_A
            0x0805E6A4, // L15_01_B
            0x0805E710, // L15_01_C
            0x0805E77C, // L15_98_A
            0x0805E83C, // L16a_01_A
            0x0805E8A8, // L16a_01_B
            0x0805E944, // L16a_98_A
            0x0805E9D4, // L16b_01_A
            0x0805EAA0, // L17_01_A
            0x0805EB0C, // L17_01_B
            0x0805EBA8, // L18_01_A
            0x0805EC14, // L18_01_B
            0x0805ED10, // L19_01_A
            0x0805EDDC, // L19_98_A
            0x0805EE3C, // L20a_01_A
            0x0805EF08, // L20a_01_B
            0x0805EF74, // L20a_01_C
            0x0805EFE0, // L20a_01_D
            0x0805F04C, // L20a_02_A
            0x0805F0AC, // L20a_98_A
            0x0805F16C, // L20b_98_A
            0x0805F1FC, // L21_01_A
            0x0805F298, // L21_01_B
            0x0805F304, // L21_01_C
            0x0805F370, // L21_01_D
            0x0805F40C, // L21_98_A
            0x0805F49C, // L22a_01_A
            0x0805F508, // L22a_01_B
            0x0805F5D4, // L22a_98_A
            0x0805F634, // L22b_01_A
            0x0805F6A0, // L22b_98_A
            0x0805F700, // L23_01_A
            0x0805F76C, // L23_01_B
            0x0805F808, // L23_98_A
            0x0805F898, // L24a_01_A
            0x0805F904, // L24a_01_B
            0x0805F9A0, // L24a_98_A
            0x0805FAC0, // L24b_01_A
            0x0805FBEC, // L24b_98_A
            0x0805FCAC, // L25_98_A
            0x0805FD3C, // L26_01_A
            0x0805FE38, // L27a_01_A
            0x0805FED4, // L27a_98_A
            0x0805FF64, // L27b_01_A
            0x08060090, // L27b_98_A
            0x08060120, // L28_01_A
            0x0806018C, // L28_01_B
            0x080601F8, // L28_01_C
            0x080602C4, // L28_98_A
            0x08060384, // L29_01_A
            0x080603F0, // L29_01_B
            0x0806051C, // L30_01_A
            0x08060588, // L30_98_A
            0x08060614, // bossCup
            0x080606B4, // waitForPagedTextPage
            0x080F8FF0, // movie_healthWarning
            0x080F903C, // movie_healthWarningDutch
            0x080F9084, // movie_healthWarningUK
            0x080F90D0, // movie_healthWarningFrench
            0x080F911C, // movie_healthWarningItalian
            0x080F9168, // movie_healthWarningGerman
            0x080F91B4, // movie_healthWarningSpanish
            0x080F91F4, // movie_license
            0x080F9234, // movie_licenseUK
            0x080F9278, // movie_licenseFrench
            0x080F92BC, // movie_licenseGerman
            0x080F9304, // movie_licenseItalian
            0x080F9348, // movie_licenseDutch
            0x080F9390, // movie_licenseSpanish
            0x080F93F8, // movie_credits
            0x080F94C0, // "waiting"
            0x080F95B0, // healthSafetyWait
            0x080F964C, // fadeWait
            0x080FB430, // waitForPagedText
            0x080FB4F4, // waitForPagedTextPage
        };
    }
    public class GBAVV_OverTheHedgeHammyGoesNutsUS_Manager : GBAVV_OverTheHedgeHammyGoesNuts_Manager
    {
        public override string[] Languages => new string[]
        {
            "English"
        };

        public override uint[] GraphicsDataPointers => new uint[]
        {
            0x081AAF5C,
            0x081B06DC,
            0x081B2904,
            0x081B4C48,
            0x081B6B10,
            0x081BAA70,
            0x081BBD18,
            0x081BEF14,
            0x081C8044,
            0x081E9C50,
            0x081EBB84,
            0x081EBBF4,
            0x081EBE2C,
            0x081EE768,
            0x081F6560,
            0x081F8D38,
            0x081FAC28,
            0x081FB084,
            0x081FE234,
            0x081FEF38,
        };

        public override uint[] ScriptPointers => new uint[]
        {
            0x0803498C, // script_waitForInputOrTime
            0x080560D4, // L01_Intro
            0x08056150, // L01_IntroCutscene
            0x080565B8, // L04_Intro
            0x08056620, // L04_IntroCutscene
            0x08056890, // L04_Outro
            0x080568F8, // L04_OutroCutscene
            0x08056AF0, // L05_Intro
            0x08056B58, // L05_IntroCutscene
            0x08056EA0, // L06_Intro
            0x08056F08, // L06_IntroCutscene
            0x0805725C, // L07_Intro
            0x080572C4, // L07_IntroCutscene
            0x08057570, // L07_Outro
            0x080575D8, // L07_OutroCutscene
            0x08057848, // L08_Intro
            0x080578B0, // L08_IntroCutscene
            0x08057A84, // L10_Intro
            0x08057AEC, // L10_IntroCutscene
            0x08057F00, // L12a_Intro
            0x08057F68, // L12a_IntroCutscene
            0x080582A4, // L13_Intro
            0x0805830C, // L13_IntroCutscene
            0x08058AD4, // L14_Intro
            0x08058B3C, // L14_IntroCutscene
            0x08058CD4, // L17_Intro
            0x08058D3C, // L17_IntroCutscene
            0x0805900C, // L17_Outro
            0x08059074, // L17_OutroCutscene
            0x08059200, // L18_Outro
            0x08059268, // L18_OutroCutscene
            0x08059448, // L20a_Intro
            0x080594B0, // L20a_IntroCutscene
            0x08059738, // L22a_Intro
            0x080597A0, // L22a_IntroCutscene
            0x080599B0, // L22b_Outro
            0x08059A18, // L22b_OutroCutscene
            0x08059D24, // L24a_Intro
            0x08059D8C, // L24a_IntroCutscene
            0x0805A0D4, // L25_Intro
            0x0805A13C, // L25_IntroCutscene
            0x0805A4D8, // L27a_Intro
            0x0805A540, // L27a_IntroCutscene
            0x0805A8D0, // L28_Intro
            0x0805A938, // L28_IntroCutscene
            0x0805ABA8, // L29_Outro
            0x0805AC10, // L29_OutroCutscene
            0x0805B168, // L30_Intro
            0x0805B1D0, // L30_IntroCutscene
            0x0805B5F0, // L30_Outro
            0x0805B658, // L30_OutroCutscene
            0x0805BC40, // waitForPagedText
            0x0805BCF8, // waitForPagedTextPage
            0x0805BD78, // notFound
            0x0805C148, // waitForPagedText
            0x0805C1C8, // missing
            0x0805C210, // ID_OUT_01
            0x0805C2D0, // ID_OUT_30
            0x0805C364, // cFailStrokes
            0x0805C3C4, // L01_01_A
            0x0805C430, // L01_01_B
            0x0805C52C, // L01_02_A
            0x0805C5BC, // L01_03_A
            0x0805C61C, // L01_04_A
            0x0805C67C, // L01_98_A
            0x0805C76C, // L02_01_A
            0x0805C7D8, // L02_01_B
            0x0805C874, // L02_02_A
            0x0805C8E0, // L02_98_A
            0x0805C9A0, // L03_01_A
            0x0805CA3C, // L03_01_B
            0x0805CAA8, // L03_01_C
            0x0805CB14, // L03_01_D
            0x0805CBB0, // L03_02_A
            0x0805CC10, // L03_98_A
            0x0805CC70, // L04_01_A
            0x0805CDCC, // L05_01_A
            0x0805CE68, // L05_02_A
            0x0805CF34, // L06_01_A
            0x0805CFD0, // L06_01_B
            0x0805D06C, // L06_98_A
            0x0805D138, // L07_01_A
            0x0805D1A4, // L07_98_A
            0x0805D204, // L08_01_A
            0x0805D360, // L08_98_A
            0x0805D420, // L09_01_A
            0x0805D48C, // L09_01_B
            0x0805D4F8, // L09_01_C
            0x0805D594, // L09_98_A
            0x0805D624, // L10_01_A
            0x0805D690, // L10_01_B
            0x0805D72C, // L10_98_A
            0x0805D828, // L11_01_A
            0x0805D8C4, // L11_01_B
            0x0805D990, // L11_98_A
            0x0805DA20, // L12a_01_A
            0x0805DA8C, // L12a_01_B
            0x0805DAF8, // L12a_01_C
            0x0805DBC4, // L12b_01_A
            0x0805DC60, // L12b_01_B
            0x0805DCFC, // L12b_98_A
            0x0805DD8C, // L13_01_A
            0x0805DE88, // L13_01_B
            0x0805DEF4, // L13_01_C
            0x0805DFC0, // L13_98_A
            0x0805E080, // L14_01_A
            0x0805E11C, // L14_02_A
            0x0805E17C, // L14_98_A
            0x0805E23C, // L15_01_A
            0x0805E2A8, // L15_01_B
            0x0805E314, // L15_01_C
            0x0805E380, // L15_98_A
            0x0805E440, // L16a_01_A
            0x0805E4AC, // L16a_01_B
            0x0805E548, // L16a_98_A
            0x0805E5D8, // L16b_01_A
            0x0805E6A4, // L17_01_A
            0x0805E710, // L17_01_B
            0x0805E7AC, // L18_01_A
            0x0805E818, // L18_01_B
            0x0805E914, // L19_01_A
            0x0805E9E0, // L19_98_A
            0x0805EA40, // L20a_01_A
            0x0805EB0C, // L20a_01_B
            0x0805EB78, // L20a_01_C
            0x0805EBE4, // L20a_01_D
            0x0805EC50, // L20a_02_A
            0x0805ECB0, // L20a_98_A
            0x0805ED70, // L20b_98_A
            0x0805EE00, // L21_01_A
            0x0805EE9C, // L21_01_B
            0x0805EF08, // L21_01_C
            0x0805EF74, // L21_01_D
            0x0805F010, // L21_98_A
            0x0805F0A0, // L22a_01_A
            0x0805F10C, // L22a_01_B
            0x0805F1D8, // L22a_98_A
            0x0805F238, // L22b_01_A
            0x0805F2A4, // L22b_98_A
            0x0805F304, // L23_01_A
            0x0805F370, // L23_01_B
            0x0805F40C, // L23_98_A
            0x0805F49C, // L24a_01_A
            0x0805F508, // L24a_01_B
            0x0805F5A4, // L24a_98_A
            0x0805F6C4, // L24b_01_A
            0x0805F7F0, // L24b_98_A
            0x0805F8B0, // L25_98_A
            0x0805F940, // L26_01_A
            0x0805FA3C, // L27a_01_A
            0x0805FAD8, // L27a_98_A
            0x0805FB68, // L27b_01_A
            0x0805FC94, // L27b_98_A
            0x0805FD24, // L28_01_A
            0x0805FD90, // L28_01_B
            0x0805FDFC, // L28_01_C
            0x0805FEC8, // L28_98_A
            0x0805FF88, // L29_01_A
            0x0805FFF4, // L29_01_B
            0x08060120, // L30_01_A
            0x0806018C, // L30_98_A
            0x08060218, // bossCup
            0x080602B8, // waitForPagedTextPage
            0x080C83CC, // movie_license
            0x080C840C, // movie_licenseUK
            0x080C8450, // movie_licenseFrench
            0x080C8494, // movie_licenseGerman
            0x080C84DC, // movie_licenseItalian
            0x080C8520, // movie_licenseDutch
            0x080C8568, // movie_licenseSpanish
            0x080C85D0, // movie_credits
            0x080C8648, // fadeWait
            0x080CA3E4, // waitForPagedText
            0x080CA4A8, // waitForPagedTextPage
        };
    }
}