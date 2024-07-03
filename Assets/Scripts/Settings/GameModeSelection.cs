using BinarySerializer.Ubisoft.Onyx;
using BinarySerializer.Ubisoft.Onyx.NDS;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Ray1Map.Gameloft;
using Ray1Map.GBA;
using Ray1Map.GBAIsometric;
using Ray1Map.GBAKlonoa;
using Ray1Map.GBARRR;
using Ray1Map.GBAVV;
using Ray1Map.GBC;
using Ray1Map.GEN;
using Ray1Map.KlonoaHeroes;
using Ray1Map.PSKlonoa;
using Ray1Map.Psychonauts;
using Ray1Map.Rayman1;
using Ray1Map.Rayman1_Jaguar;
using Ray1Map.SNES;

namespace Ray1Map
{
    /// <summary>
    /// The available game modes to select from
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum GameModeSelection
    {
        #region Rayman 1

        [GameMode(MajorEngineVersion.Rayman1, EngineVersion.R1_PS1, Game.R1_Rayman1, "Rayman 1 (PS1 - US)", typeof(R1_PS1US_Manager))]
        RaymanPS1US,

        [GameMode(MajorEngineVersion.Rayman1, EngineVersion.R1_PS1, Game.R1_Rayman1, "Rayman 1 (PS1 - EU)", typeof(R1_PS1EU_Manager))]
        RaymanPS1EU,

        [GameMode(MajorEngineVersion.Rayman1, EngineVersion.R1_PS1, Game.R1_Rayman1, "Rayman 1 (PS1 - US Demo)", typeof(R1_PS1USDemo_Manager))]
        RaymanPS1USDemo,

        [GameMode(MajorEngineVersion.Rayman1, EngineVersion.R1_PS1, Game.R1_Rayman1, "Rayman 1 (PS1 - EU Demo)", typeof(R1_PS1EUDemo_Manager))]
        RaymanPS1EUDemo,

        [GameMode(MajorEngineVersion.Rayman1, EngineVersion.R1_PS1_JP, Game.R1_Rayman1, "Rayman 1 (PS1 - JP)", typeof(R1_PS1JP_Manager))]
        RaymanPS1Japan,

        [GameMode(MajorEngineVersion.Rayman1, EngineVersion.R1_PS1_JPDemoVol3, Game.R1_Rayman1, "Rayman 1 (PS1 - JP Demo Vol3)", typeof(R1_PS1JPDemoVol3_Manager))]
        RaymanPS1DemoVol3Japan,

        [GameMode(MajorEngineVersion.Rayman1, EngineVersion.R1_PS1_JPDemoVol6, Game.R1_Rayman1, "Rayman 1 (PS1 - JP Demo Vol6)", typeof(R1_PS1JPDemoVol6_Manager))]
        RaymanPS1DemoVol6Japan,

        [GameMode(MajorEngineVersion.Rayman1, EngineVersion.R1_Saturn, Game.R1_Rayman1, "Rayman 1 (Saturn - US)", typeof(R1_Saturn_Manager))]
        RaymanSaturnUS,

        [GameMode(MajorEngineVersion.Rayman1, EngineVersion.R1_Saturn, Game.R1_Rayman1, "Rayman 1 (Saturn - Prototype)", typeof(R1_Saturn_Manager))]
        RaymanSaturnProto,

        [GameMode(MajorEngineVersion.Rayman1, EngineVersion.R1_Saturn, Game.R1_Rayman1, "Rayman 1 (Saturn - EU)", typeof(R1_SaturnEU_Manager))]
        RaymanSaturnEU,

        [GameMode(MajorEngineVersion.Rayman1, EngineVersion.R1_Saturn, Game.R1_Rayman1, "Rayman 1 (Saturn - JP)", typeof(R1_Saturn_Manager))]
        RaymanSaturnJP,

        [GameMode(MajorEngineVersion.Rayman1, EngineVersion.R1_Saturn, Game.R1_Rayman1, "Rayman 1 (Saturn - US Demo)", typeof(R1_Saturn_Manager))]
        RaymanSaturnUSDemo,

        [GameMode(MajorEngineVersion.Rayman1, EngineVersion.R1_Saturn, Game.R1_Rayman1, "Rayman 1 (Saturn - JP Demo)", typeof(R1_SaturnJPDemo_Manager))]
        RaymanSaturnJPDemo,

        [GameMode(MajorEngineVersion.Rayman1, EngineVersion.R1_PC, Game.R1_Rayman1, "Rayman 1 (PC - 1.00)", typeof(R1_PC_Manager))]
        RaymanPC_1_00,

        [GameMode(MajorEngineVersion.Rayman1, EngineVersion.R1_PC, Game.R1_Rayman1, "Rayman 1 (PC - 1.10)", typeof(R1_PC_Manager))]
        RaymanPC_1_10,

        [GameMode(MajorEngineVersion.Rayman1, EngineVersion.R1_PC, Game.R1_Rayman1, "Rayman 1 (PC - 1.12)", typeof(R1_PC_Manager))]
        RaymanPC_1_12,

        [GameMode(MajorEngineVersion.Rayman1, EngineVersion.R1_PC, Game.R1_Rayman1, "Rayman 1 (PC - 1.20)", typeof(R1_PC_Manager))]
        RaymanPC_1_20,

        [GameMode(MajorEngineVersion.Rayman1, EngineVersion.R1_PC, Game.R1_Rayman1, "Rayman 1 (PC - 1.21 JP)", typeof(R1_PC_Manager))]
        RaymanPC_1_21_JP,

        [GameMode(MajorEngineVersion.Rayman1, EngineVersion.R1_PC, Game.R1_Rayman1, "Rayman 1 (PC - 1.21)", typeof(R1_PC_Manager))]
        RaymanPC_1_21,

        [GameMode(MajorEngineVersion.Rayman1, EngineVersion.R1_PC, Game.R1_Rayman1, "Rayman 1 (PC - Demo 1)", typeof(R1_PC_Manager))]
        RaymanPC_Demo_1,

        [GameMode(MajorEngineVersion.Rayman1, EngineVersion.R1_PC, Game.R1_Rayman1, "Rayman 1 (PC - Demo 2)", typeof(R1_PC_Manager))]
        RaymanPC_Demo_2,

        [GameMode(MajorEngineVersion.Rayman1, EngineVersion.R1_PC_Kit, Game.R1_Designer, "Rayman Gold (PC - Demo)", typeof(R1_Kit_Manager))]
        RaymanGoldPCDemo,

        [GameMode(MajorEngineVersion.Rayman1, EngineVersion.R1_PC_Kit, Game.R1_Designer, "Rayman Designer (PC)", typeof(R1_Kit_Manager))]
        RaymanDesignerPC,

        [GameMode(MajorEngineVersion.Rayman1, EngineVersion.R1_PC_Kit, Game.R1_Mapper, "Rayman Mapper (PC)", typeof(R1_Mapper_Manager))]
        MapperPC,

        [GameMode(MajorEngineVersion.Rayman1, EngineVersion.R1_PC_Kit, Game.R1_ByHisFans, "Rayman by his Fans (PC)", typeof(R1_Kit_Manager))]
        RaymanByHisFansPC,

        [GameMode(MajorEngineVersion.Rayman1, EngineVersion.R1_PC_Kit, Game.R1_60Levels, "Rayman 60 Levels (PC)", typeof(R1_Kit_Manager))]
        Rayman60LevelsPC,

        [GameMode(MajorEngineVersion.Rayman1, EngineVersion.R1_PC_Edu, Game.R1_Educational, "Rayman Educational (PC)", typeof(R1_PCEdu_Manager))]
        RaymanEducationalPC,

        [GameMode(MajorEngineVersion.Rayman1, EngineVersion.R1_PS1_Edu, Game.R1_Educational, "Rayman Educational (PS1)", typeof(R1_PS1Edu_Manager))]
        RaymanEducationalPS1,

        [GameMode(MajorEngineVersion.Rayman1, EngineVersion.R1_PC_Edu, Game.R1_Quiz, "Rayman Quiz (PC)", typeof(R1_PCEdu_Manager))]
        RaymanQuizPC,

        [GameMode(MajorEngineVersion.Rayman1, EngineVersion.R1_PS1_Edu, Game.R1_Quiz, "Rayman Quiz (PS1)", typeof(R1_PS1Edu_Manager))]
        RaymanQuizPS1,

        [GameMode(MajorEngineVersion.Rayman1, EngineVersion.R1_PocketPC, Game.R1_Rayman1, "Rayman Ultimate (Pocket PC)", typeof(R1_PocketPC_Manager))]
        RaymanPocketPC,

        [GameMode(MajorEngineVersion.Rayman1, EngineVersion.R1_PocketPC, Game.R1_Rayman1, "Rayman Classic (Mobile)", typeof(R1_Mobile_Manager))]
        RaymanClassicMobile,

        [GameMode(MajorEngineVersion.Rayman1, EngineVersion.R1_GBA, Game.R1_Rayman1, "Rayman Advance (GBA - EU)", typeof(R1_GBA_Manager))]
        RaymanAdvanceGBAEU,

        [GameMode(MajorEngineVersion.Rayman1, EngineVersion.R1_GBA, Game.R1_Rayman1, "Rayman Advance (GBA - US)", typeof(R1_GBA_Manager))]
        RaymanAdvanceGBAUS,

        [GameMode(MajorEngineVersion.Rayman1, EngineVersion.R1_GBA, Game.R1_Rayman1, "Rayman Advance (GBA - EU Beta)", typeof(R1_GBA_Manager))]
        RaymanAdvanceGBAEUBeta,

        [GameMode(MajorEngineVersion.Rayman1, EngineVersion.R1_DSi, Game.R1_Rayman1, "Rayman 1 (DSi)", typeof(R1_DSi_Manager))]
        RaymanDSi,

        [GameMode(MajorEngineVersion.Rayman1, EngineVersion.R2_PS1, Game.R1_Rayman2, "Rayman 2 (PS1 - Demo)", typeof(R1_PS1R2_Manager))]
        Rayman2PS1Demo,

        #endregion

        #region Rayman 1 Jaguar

        [GameMode(MajorEngineVersion.Rayman1_Jaguar, EngineVersion.R1Jaguar, Game.R1_Rayman1, "Rayman 1 (Jaguar)", typeof(R1Jaguar_Manager))]
        RaymanJaguar,

        [GameMode(MajorEngineVersion.Rayman1_Jaguar, EngineVersion.R1Jaguar_Proto, Game.R1_Rayman1, "Rayman 1 (Jaguar - Prototype)", typeof(R1Jaguar_Proto_Manager))]
        RaymanJaguarPrototype,

        [GameMode(MajorEngineVersion.Rayman1_Jaguar, EngineVersion.R1Jaguar_Demo, Game.R1_Rayman1, "Rayman 1 (Jaguar - Demo)", typeof(R1Jaguar_Demo_Manager))]
        RaymanJaguarDemo,

        #endregion

        #region SNES

        [GameMode(MajorEngineVersion.SNES, EngineVersion.SNES, Game.SNES_Prototype, "Rayman (SNES - Prototype)", typeof(SNES_Prototype_Manager))]
        RaymanSNES,

        #endregion

        #region GBA

        [GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_DonaldDuck, Game.GBA_DonaldDuckAdvance, "Donald Duck Advance (GBA - EU)", typeof(GBA_DonaldDuck_Manager))]
        DonaldDuckAdvanceGBAEU,

        [GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_DonaldDuck, Game.GBA_DonaldDuckAdvance, "Donald Duck Advance (GBA - US)", typeof(GBA_DonaldDuck_Manager))]
        DonaldDuckAdvanceGBAUS,

        [GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_CrouchingTiger, Game.GBA_CrouchingTigerHiddenDragon, "Crouching Tiger Hidden Dragon (GBA - EU)", typeof(GBA_CrouchingTiger_Manager))]
        CrouchingTigerHiddenDragonGBAEU,

        [GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_CrouchingTiger, Game.GBA_CrouchingTigerHiddenDragon, "Crouching Tiger Hidden Dragon (GBA - US)", typeof(GBA_CrouchingTiger_Manager))]
        CrouchingTigerHiddenDragonGBAUS,

        [GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_CrouchingTiger, Game.GBA_CrouchingTigerHiddenDragon, "Crouching Tiger Hidden Dragon (GBA - US Beta)", typeof(GBA_CrouchingTigerBeta_Manager))]
        CrouchingTigerHiddenDragonGBAUSBeta,

		[GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_CrouchingTigerPrototype, Game.GBA_CrouchingTigerHiddenDragon, "Crouching Tiger Hidden Dragon (GBA - Prototype)", typeof(GBA_CrouchingTigerProto_Manager))]
		CrouchingTigerHiddenDragonGBAPrototype,

		[GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_TomClancysRainbowSixRogueSpear, Game.GBA_TomClancysRainbowSixRogueSpear, "Tom Clancy's Rainbow Six: Rogue Spear (GBA - EU)", typeof(GBA_RainbowSixRogueSpear_Manager))]
        TomClancysRainbowSixRogueSpearEU,

        [GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_TomClancysRainbowSixRogueSpear, Game.GBA_TomClancysRainbowSixRogueSpear, "Tom Clancy's Rainbow Six: Rogue Spear (GBA - US)", typeof(GBA_RainbowSixRogueSpear_Manager))]
        TomClancysRainbowSixRogueSpearUS,

		[GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_TomClancysRainbowSixRogueSpear, Game.GBA_TomClancysRainbowSixRogueSpear, "Tom Clancy's Rainbow Six: Rogue Spear (GBA - Prototype)", typeof(GBA_RainbowSixRogueSpear_Prototype_Manager))]
		TomClancysRainbowSixRogueSpearPrototype,

		[GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_TheSumOfAllFears, Game.GBA_TheSumOfAllFears, "The Sum Of All Fears (GBA - EU)", typeof(GBA_TheSumOfAllFears_Manager))]
		TheSumOfAllFearsEU,

		[GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_TheSumOfAllFears, Game.GBA_TheSumOfAllFears, "The Sum Of All Fears (GBA - US)", typeof(GBA_TheSumOfAllFears_Manager))]
		TheSumOfAllFearsUS,

		[GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_TheMummy, Game.GBA_TheMummy, "The Mummy (GBA - EU)", typeof(GBA_TheMummy_Manager))]
        TheMummyEU,

        [GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_TheMummy, Game.GBA_TheMummy, "The Mummy (GBA - US)", typeof(GBA_TheMummy_Manager))]
        TheMummyUS,

        [GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_TombRaiderTheProphecy, Game.GBA_TombRaiderTheProphecy, "Tomb Raider: The Prophecy (GBA - EU)", typeof(GBA_TombRaider_Manager))]
        TombRaiderTheProphecyEU,

        [GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_TombRaiderTheProphecy, Game.GBA_TombRaiderTheProphecy, "Tomb Raider: The Prophecy (GBA - US)", typeof(GBA_TombRaider_Manager))]
        TombRaiderTheProphecyUS,

        [GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_BatmanVengeance, Game.GBA_BatmanVengeance, "Batman Vengeance (GBA - EU)", typeof(GBA_BatmanVengeance_Manager))]
        BatmanVengeanceGBAEU,

        [GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_BatmanVengeance, Game.GBA_BatmanVengeance, "Batman Vengeance (GBA - US)", typeof(GBA_BatmanVengeance_Manager))]
        BatmanVengeanceGBAUS,

        [GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_Sabrina, Game.GBA_SabrinaTheTeenageWitchPotionCommotion, "Sabrina the Teenage Witch - Potion Commotion (GBA - EU)", typeof(GBA_Sabrina_Manager))]
        SabrinaTheTeenageWitchPotionCommotionGBAEU,

        [GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_Sabrina, Game.GBA_SabrinaTheTeenageWitchPotionCommotion, "Sabrina the Teenage Witch - Potion Commotion (GBA - US)", typeof(GBA_Sabrina_Manager))]
        SabrinaTheTeenageWitchPotionCommotionGBAUS,

		[GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_R3_20020118_DemoRLE, Game.GBA_Rayman3, "Rayman 3 (GBA - Demo RLE - 2002/01/18)", typeof(GBA_R3_20020118_DemoRLE_Manager))]
		Rayman3GBA_20020118_DemoRLE,

		[GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_R3_20020301_PreAlpha, Game.GBA_Rayman3, "Rayman 3 (GBA - Pre-Alpha - 2002/03/01)", typeof(GBA_R3_20020301_PreAlpha_Manager))]
		Rayman3GBA_20020301_PreAlpha,

		[GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_R3_20020301_PreAlpha, Game.GBA_Rayman3, "Rayman 3 (GBA - Pre-Alpha (B) - 2002/03/08)", typeof(GBA_R3_20020308_PreAlphaB_Manager))]
		Rayman3GBA_20020308_PreAlphaB,

		[GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_R3_20020301_PreAlpha, Game.GBA_Rayman3, "Rayman 3 (GBA - Focus Group - 2002/03/18)", typeof(GBA_R3_20020318_FocusGroup_Manager))]
		Rayman3GBA_20020318_FocusGroup,

		[GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_R3_20020418_NintendoE3Approval, Game.GBA_Rayman3, "Rayman 3 (GBA - Nintendo E3 Approval - 2002/04/18)", typeof(GBA_R3_20020418_NintendoE3Approval_Manager))]
		Rayman3GBA_20020418_NintendoE3Approval,

		[GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_R3_20020513_E3GameCube, Game.GBA_Rayman3, "Rayman 3 (GBA - E3 GameCube - 2002/05/13)", typeof(GBA_R3_20020513_E3GameCube_Manager))]
		Rayman3GBA_20020513_E3GameCube,

		[GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_R3_20020513_E3GameCube, Game.GBA_Rayman3, "Rayman 3 (GBA - E3 - 2002/05/16)", typeof(GBA_R3_20020513_E3GameCube_Manager))]
		Rayman3GBA_20020516_E3,

		[GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_R3_Proto, Game.GBA_Rayman3, "Rayman 3 (GBA - La Ronde - 2002/07/19)", typeof(GBA_R3_20020719_LaRonde_Manager))]
		Rayman3GBA_20020719_LaRonde,

		[GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_R3_Proto, Game.GBA_Rayman3, "Rayman 3 (GBA - ECTS - 2002/08/09)", typeof(GBA_R3_20020809_ECTS_Manager))]
		Rayman3GBA_20020809_ECTS,

		[GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_R3, Game.GBA_Rayman3, "Rayman 3 (GBA - EU)", typeof(GBA_R3_Manager))]
        Rayman3GBAEU,

        [GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_R3, Game.GBA_Rayman3, "Rayman 3 (GBA - US)", typeof(GBA_R3_Manager))]
        Rayman3GBAUS,

        [GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_R3, Game.GBA_Rayman3, "Rayman 3 (GBA - EU Beta)", typeof(GBA_R3_Manager))]
        Rayman3GBAEUBeta,

        [GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_R3_Proto, Game.GBA_Rayman3, "Rayman 3 (GBA - US Prototype)", typeof(GBA_R3Proto_Manager))]
        Rayman3GBAUSPrototype,

        [GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_R3_MadTrax, Game.GBA_Rayman3_MadTrax, "Rayman 3 Mad Trax (EU)", typeof(GBA_R3MadTrax_Manager))]
        Rayman3GBAMadTraxEU,

        [GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_R3_MadTrax, Game.GBA_Rayman3_MadTrax, "Rayman 3 Mad Trax (US)", typeof(GBA_R3MadTrax_Manager))]
        Rayman3GBAMadTraxUS,

        [GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_R3_NGage, Game.GBA_Rayman3, "Rayman 3 (N-Gage)", typeof(GBA_R3NGage_Manager))]
        Rayman3NGage,

        [GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_R3, Game.GBA_Rayman3, "Rayman 3 (Digiblast)", typeof(GBA_R3Digiblast_Manager))]
        Rayman3Digiblast,

        [GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_SplinterCell, Game.GBA_SplinterCell, "Splinter Cell (GBA - EU)", typeof(GBA_SplinterCell_Manager))]
        SplinterCellGBAEU,

        [GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_SplinterCell, Game.GBA_SplinterCell, "Splinter Cell (GBA - US)", typeof(GBA_SplinterCell_Manager))]
        SplinterCellGBAUS,

        [GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_SplinterCell, Game.GBA_SplinterCell, "Splinter Cell (GBA - EU Beta)", typeof(GBA_SplinterCellProto_Manager))]
        SplinterCellGBAEUBeta,

        [GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_SplinterCell_NGage, Game.GBA_SplinterCell, "Splinter Cell (N-Gage)", typeof(GBA_SplinterCellNGage_Manager))]
        SplinterCellNGage,

        [GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_SplinterCellPandoraTomorrow, Game.GBA_SplinterCellPandoraTomorrow, "Splinter Cell: Pandora Tomorrow (GBA - EU)", typeof(GBA_SplinterCellPandoraTomorrow_Manager))]
        SplinterCellPandoraTomorrowGBAEU,

        [GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_SplinterCellPandoraTomorrow, Game.GBA_SplinterCellPandoraTomorrow, "Splinter Cell: Pandora Tomorrow (GBA - US)", typeof(GBA_SplinterCellPandoraTomorrow_Manager))]
        SplinterCellPandoraTomorrowGBAUS,

        [GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_BatmanRiseOfSinTzu, Game.GBA_BatmanRiseOfSinTzu, "Batman: Rise of Sin Tzu (GBA - US)", typeof(GBA_BatmanRiseOfSinTzu_Manager))]
        BatmanRiseOfSinTzuGBAUS,

        [GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_PrinceOfPersia, Game.GBA_PrinceOfPersiaTheSandsOfTime, "Prince of Persia: The Sands of Time (GBA - EU)", typeof(GBA_PoPSoT_Manager))]
        PrinceOfPersiaGBAEU,

        [GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_PrinceOfPersia, Game.GBA_PrinceOfPersiaTheSandsOfTime, "Prince of Persia: The Sands of Time (GBA - US)", typeof(GBA_PoPSoT_Manager))]
        PrinceOfPersiaGBAUS,

        [GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_StarWarsTrilogy, Game.GBA_StarWarsTrilogyApprenticeOfTheForce, "Star Wars Trilogy: Apprentice of the Force (GBA - EU)", typeof(GBA_StarWarsTrilogy_Manager))]
        StarWarsTrilogyApprenticeOfTheForceGBAEU,

        [GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_StarWarsTrilogy, Game.GBA_StarWarsTrilogyApprenticeOfTheForce, "Star Wars Trilogy: Apprentice of the Force (GBA - US)", typeof(GBA_StarWarsTrilogy_Manager))]
        StarWarsTrilogyApprenticeOfTheForceGBAUS,

        [GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_StarWarsEpisodeIII, Game.GBA_StarWarsEpisodeIII, "Star Wars Episode III (GBA - EU)", typeof(GBA_StarWarsEpisodeIII_Manager))]
        StarWarsEpisodeIIIGBAEU,

        [GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_StarWarsEpisodeIII, Game.GBA_StarWarsEpisodeIII, "Star Wars Episode III (GBA - US)", typeof(GBA_StarWarsEpisodeIII_Manager))]
        StarWarsEpisodeIIIGBAUS,

        [GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_KingKong, Game.GBA_KingKong, "King Kong (GBA - EU)", typeof(GBA_KingKong_Manager))]
        KingKongGBAEU,

        [GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_KingKong, Game.GBA_KingKong, "King Kong (GBA - US)", typeof(GBA_KingKong_Manager))]
        KingKongGBAUS,

        [GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_OpenSeason, Game.GBA_OpenSeason, "Open Season (GBA - EU 1)", typeof(GBA_OpenSeason_Manager))]
        OpenSeasonGBAEU1,

        [GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_OpenSeason, Game.GBA_OpenSeason, "Open Season (GBA - EU 2)", typeof(GBA_OpenSeason_Manager))]
        OpenSeasonGBAEU2,

        [GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_OpenSeason, Game.GBA_OpenSeason, "Open Season (GBA - US)", typeof(GBA_OpenSeason_Manager))]
        OpenSeasonGBAUS,

        [GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_TMNT, Game.GBA_TMNT, "TMNT (GBA - EU)", typeof(GBA_TMNT_Manager))]
        TMNTGBAEU,

        [GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_TMNT, Game.GBA_TMNT, "TMNT (GBA - US)", typeof(GBA_TMNT_Manager))]
        TMNTGBAUS,

        [GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_SurfsUp, Game.GBA_SurfsUp, "Surf's Up (GBA - EU 1)", typeof(GBA_SurfsUp_Manager))]
        SurfsUpEU1,

        [GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_SurfsUp, Game.GBA_SurfsUp, "Surf's Up (GBA - EU 2)", typeof(GBA_SurfsUp_Manager))]
        SurfsUpEU2,

        [GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_SurfsUp, Game.GBA_SurfsUp, "Surf's Up (GBA - US)", typeof(GBA_SurfsUp_Manager))]
        SurfsUpUS,

        #endregion

        #region Onyx DS

        [GameMode(MajorEngineVersion.OnyxDS, EngineVersion.OnyxDS_RaymanRavingRabbids2, Game.OnyxDS_RaymanRavingRabbids2, "Rayman Raving Rabbids 2 (NDS)", typeof(Onyx_NDS_Manager))]
        RaymanRavingRabbids2NDS,

        #endregion

        #region GBA RRR

        [GameMode(MajorEngineVersion.GBARRR, EngineVersion.GBARRR, Game.GBARRR_RavingRabbids, "Rayman Raving Rabbids (GBA - EU)", typeof(GBA_RRR_Manager))]
        RaymanRavingRabbidsGBAEU,

        [GameMode(MajorEngineVersion.GBARRR, EngineVersion.GBARRR, Game.GBARRR_RavingRabbids, "Rayman Raving Rabbids (GBA - US)", typeof(GBA_RRR_Manager))]
        RaymanRavingRabbidsGBAUS,

        #endregion

        #region GBA Isometric

        [GameMode(MajorEngineVersion.GBAIsometric, EngineVersion.GBAIsometric_Spyro1, Game.GBAIsometric_Spyro1, "Spyro: Season of Ice (GBA - EU)", typeof(GBAIsometric_Spyro1_Manager))]
        SpyroSeasonIceEU,

        [GameMode(MajorEngineVersion.GBAIsometric, EngineVersion.GBAIsometric_Spyro1, Game.GBAIsometric_Spyro1, "Spyro: Season of Ice (GBA - US)", typeof(GBAIsometric_Spyro1_Manager))]
        SpyroSeasonIceUS,

        [GameMode(MajorEngineVersion.GBAIsometric, EngineVersion.GBAIsometric_Spyro1, Game.GBAIsometric_Spyro1, "Spyro Advance (GBA - JP)", typeof(GBAIsometric_Spyro1_Manager))]
        SpyroSeasonIceJP,

        [GameMode(MajorEngineVersion.GBAIsometric, EngineVersion.GBAIsometric_Spyro2, Game.GBAIsometric_Spyro2, "Spyro 2: Season of Flame (GBA - EU)", typeof(GBAIsometric_Spyro2_Manager))]
        SpyroSeasonFlameEU,

        [GameMode(MajorEngineVersion.GBAIsometric, EngineVersion.GBAIsometric_Spyro2, Game.GBAIsometric_Spyro2, "Spyro 2: Season of Flame (GBA - US)", typeof(GBAIsometric_Spyro2_Manager))]
        SpyroSeasonFlameUS,

        [GameMode(MajorEngineVersion.GBAIsometric, EngineVersion.GBAIsometric_Spyro3, Game.GBAIsometric_Spyro3, "Spyro Adventure (GBA - EU)", typeof(GBAIsometric_Spyro3_Manager))]
        SpyroAdventureEU,

        [GameMode(MajorEngineVersion.GBAIsometric, EngineVersion.GBAIsometric_Spyro3, Game.GBAIsometric_Spyro3, "Spyro: Attack of the Rhynocs (GBA - US)", typeof(GBAIsometric_Spyro3_Manager))]
        SpyroAdventureUS,

        [GameMode(MajorEngineVersion.GBAIsometric, EngineVersion.GBAIsometric_Tron2, Game.GBAIsometric_Tron2, "TRON 2.0: Killer App (GBA - EU)", typeof(GBAIsometric_Tron2_Manager))]
        Tron2KillerAppEU,

        [GameMode(MajorEngineVersion.GBAIsometric, EngineVersion.GBAIsometric_Tron2, Game.GBAIsometric_Tron2, "TRON 2.0: Killer App (GBA - US)", typeof(GBAIsometric_Tron2_Manager))]
        Tron2KillerAppUS,

        [GameMode(MajorEngineVersion.GBAIsometric, EngineVersion.GBAIsometric_RHR, Game.GBAIsometric_RHR, "Rayman Hoodlum's Revenge (GBA - EU)", typeof(GBAIsometric_RHR_Manager))]
        RaymanHoodlumsRevengeEU,

        [GameMode(MajorEngineVersion.GBAIsometric, EngineVersion.GBAIsometric_RHR, Game.GBAIsometric_RHR, "Rayman Hoodlum's Revenge (GBA - US)", typeof(GBAIsometric_RHR_Manager))]
        RaymanHoodlumsRevengeUS,

        #endregion

        #region GBC

        [GameMode(MajorEngineVersion.GBC, EngineVersion.GBC_R1_Palm, Game.GBC_R1, "Rayman (PalmOS - Color)", typeof(GBC_R1PalmOS_Manager))]
        RaymanGBCPalmOSColor,

        [GameMode(MajorEngineVersion.GBC, EngineVersion.GBC_R1_Palm, Game.GBC_R1, "Rayman (PalmOS - Greyscale)", typeof(GBC_R1PalmOS_Manager))]
        RaymanGBCPalmOSGreyscale,

        [GameMode(MajorEngineVersion.GBC, EngineVersion.GBC_R1_Palm, Game.GBC_R1, "Rayman (PalmOS - Color Demo)", typeof(GBC_R1PalmOS_Manager))]
        RaymanGBCPalmOSColorDemo,

        [GameMode(MajorEngineVersion.GBC, EngineVersion.GBC_R1_Palm, Game.GBC_R1, "Rayman (PalmOS - Greyscale Demo)", typeof(GBC_R1PalmOS_Manager))]
        RaymanGBCPalmOSGreyscaleDemo,

        [GameMode(MajorEngineVersion.GBC, EngineVersion.GBC_R1_PocketPC, Game.GBC_R1, "Rayman (PocketPC - Portrait)", typeof(GBC_R1PocketPC_Manager))]
        RaymanGBCPocketPC_Portrait,

        [GameMode(MajorEngineVersion.GBC, EngineVersion.GBC_R1_PocketPC, Game.GBC_R1, "Rayman (PocketPC - Landscape)", typeof(GBC_R1PocketPC_Manager))]
        RaymanGBCPocketPC_Landscape,

        [GameMode(MajorEngineVersion.GBC, EngineVersion.GBC_R1_PocketPC, Game.GBC_R1, "Rayman (PocketPC - IPAQ)", typeof(GBC_R1PocketPC_Manager))]
        RaymanGBCPocketPC_LandscapeIPAQ,

        [GameMode(MajorEngineVersion.GBC, EngineVersion.GBC_R1_PocketPC, Game.GBC_R1, "Rayman (PocketPC - Portrait Demo)", typeof(GBC_R1PocketPC_Manager))]
        RaymanGBCPocketPC_PortraitDemo,

        [GameMode(MajorEngineVersion.GBC, EngineVersion.GBC_R1_PocketPC, Game.GBC_R1, "Rayman (PocketPC - Landscape Demo)", typeof(GBC_R1PocketPC_Manager))]
        RaymanGBCPocketPC_LandscapeDemo,

        [GameMode(MajorEngineVersion.GBC, EngineVersion.GBC_R1_PocketPC, Game.GBC_R1, "Rayman (PocketPC - IPAQ Demo)", typeof(GBC_R1PocketPC_Manager))]
        RaymanGBCPocketPC_LandscapeIPAQDemo,

        [GameMode(MajorEngineVersion.GBC, EngineVersion.GBC_R1_PocketPC, Game.GBC_R1, "Rayman (Symbian)", typeof(GBC_R1PocketPC_Manager))]
        RaymanGBCSymbian,

        [GameMode(MajorEngineVersion.GBC, EngineVersion.GBC_R1, Game.GBC_R1, "Rayman (GBC - EU/US)", typeof(GBC_R1_Manager))]
        RaymanGBC,

        [GameMode(MajorEngineVersion.GBC, EngineVersion.GBC_R1, Game.GBC_R1, "Rayman (GBC - JP)", typeof(GBC_R1_Manager))]
        RaymanGBCJP,

        [GameMode(MajorEngineVersion.GBC, EngineVersion.GBC_R1, Game.GBC_R2, "Rayman 2 (GBC - EU)", typeof(GBC_R2_Manager))]
        Rayman2GBCEU,

        [GameMode(MajorEngineVersion.GBC, EngineVersion.GBC_R1, Game.GBC_R2, "Rayman 2 (GBC - US)", typeof(GBC_R2_Manager))]
        Rayman2GBCUS,

        [GameMode(MajorEngineVersion.GBC, EngineVersion.GBC_R1, Game.GBC_DD, "Donald Duck Quack Attack (GBC - EU)", typeof(GBC_DD_Manager))]
        DonaldDuckGBCEU,

        [GameMode(MajorEngineVersion.GBC, EngineVersion.GBC_R1, Game.GBC_DD, "Donald Duck Goin' Quackers (GBC - US)", typeof(GBC_DD_Manager))]
        DonaldDuckGBCUS,

        [GameMode(MajorEngineVersion.GBC, EngineVersion.GBC_R1, Game.GBC_Mowgli, "Mowgli's Wild Adventure (GBC - EU/US)", typeof(GBC_Mowgli_Manager))]
        MowgliGBC,

        #endregion

        #region GBA Vicarious Visions

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_Crash1, Game.GBAVV_Crash1, "Crash Bandicoot XS (GBA - EU)", typeof(GBAVV_Crash1EU_Manager))]
        Crash1GBAEU,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_Crash1, Game.GBAVV_Crash1, "Crash Bandicoot The Huge Adventure (GBA - US)", typeof(GBAVV_Crash1US_Manager))]
        Crash1GBAUS,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_Crash1, Game.GBAVV_Crash1, "Crash Bandicoot Advance (GBA - JP)", typeof(GBAVV_Crash1JP_Manager))]
        Crash1GBAJP,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_ThePowerpuffGirlsHimAndSeek, Game.GBAVV_ThePowerpuffGirls, "The Powerpuff Girls Him and Seek (GBA - EU)", typeof(GBAVV_PowerpuffGirlsEU_Manager))]
        ThePowerpuffGirlsHimAndSeekGBAEU,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_ThePowerpuffGirlsHimAndSeek, Game.GBAVV_ThePowerpuffGirls, "The Powerpuff Girls Him and Seek (GBA - US)", typeof(GBAVV_PowerpuffGirlsUS_Manager))]
        ThePowerpuffGirlsHimAndSeekGBAUS,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_FroggerAdvance, Game.GBAVV_FroggerAdvance, "Frogger Advance The Great Quest (GBA - EU)", typeof(GBAVV_FroggerEU_Manager))]
        FroggerAdvanceGBAEU,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_FroggerAdvance, Game.GBAVV_FroggerAdvance, "Frogger Advance The Great Quest (GBA - US)", typeof(GBAVV_FroggerUS_Manager))]
        FroggerAdvanceGBAUS,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_SpongeBobRevengeOfTheFlyingDutchman, Game.GBAVV_SpongeBobRevengeOfTheFlyingDutchman, "SpongeBob SquarePants - Revenge of the Flying Dutchman (GBA - EU/US)", typeof(GBAVV_SpongeBobRevengeOfTheFlyingDutchmanEUUS_Manager))]
        GBAVV_SpongeBobRevengeOfTheFlyingDutchmanGBA,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_SpongeBobRevengeOfTheFlyingDutchman, Game.GBAVV_SpongeBobRevengeOfTheFlyingDutchman, "SpongeBob SquarePants - Revenge of the Flying Dutchman (GBA - US Beta)", typeof(GBAVV_SpongeBobRevengeOfTheFlyingDutchmanUSBeta_Manager))]
        GBAVV_SpongeBobRevengeOfTheFlyingDutchmanGBAUSBeta,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_TheMuppetsOnWithTheShow, Game.GBAVV_TheMuppetsOnWithTheShow, "The Muppets On with the Show! (GBA - EU/US)", typeof(GBAVV_TheMuppetsOnWithTheShow_Manager))]
        MuppetsOnWithTheShowGBA,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_SpyMuppetsLicenseToCroak, Game.GBAVV_SpyMuppetsLicenseToCroak, "Spy Muppets License to Croak (GBA - EU/US)", typeof(GBAVV_SpyMuppetsLicenseToCroak_Manager))]
        SpyMuppetsLicenseToCroakGBA,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_Crash2, Game.GBAVV_Crash2, "Crash Bandicoot 2 N-Tranced (GBA - EU)", typeof(GBAVV_Crash2EU_Manager))]
        Crash2GBAEU,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_Crash2, Game.GBAVV_Crash2, "Crash Bandicoot 2 N-Tranced (GBA - US)", typeof(GBAVV_Crash2US_Manager))]
        Crash2GBAUS,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_Crash2, Game.GBAVV_Crash2, "Crash Bandicoot Advance 2 (GBA - JP)", typeof(GBAVV_Crash2JP_Manager))]
        Crash2GBAJP,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_BruceLeeReturnOfTheLegend, Game.GBAVV_BruceLeeReturnOfTheLegend, "Bruce Lee Return of the Legend (GBA - EU)", typeof(GBAVV_BruceLeeReturnOfTheLegendEU_Manager))]
        BruceLeeReturnOfTheLegendGBAEU,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_BruceLeeReturnOfTheLegend, Game.GBAVV_BruceLeeReturnOfTheLegend, "Bruce Lee Return of the Legend (GBA - US)", typeof(GBAVV_BruceLeeReturnOfTheLegendUS_Manager))]
        BruceLeeReturnOfTheLegendGBAUS,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_X2WolverinesRevenge, Game.GBAVV_X2WolverinesRevenge, "X2 Wolverine's Revenge (GBA - EU/US)", typeof(GBAVV_X2WolverinesRevenge_Manager))]
        X2WolverinesRevengeGBA,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_FindingNemo, Game.GBAVV_FindingNemo, "Finding Nemo (GBA - EU/US)", typeof(GBAVV_FindingNemoEUUS_Manager))]
        FindingNemoGBA,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_FindingNemo, Game.GBAVV_FindingNemo, "Finding Nemo (GBA - JP)", typeof(GBAVV_FindingNemoJP_Manager))]
        FindingNemoGBAJP,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_TheLionKing, Game.GBAVV_TheLionKing, "The Lion King 1 ½ (GBA - EU)", typeof(GBAVV_TheLionKingEU_Manager))]
        TheLionKing112GBAEU,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_TheLionKing, Game.GBAVV_TheLionKing, "The Lion King 1 ½ (GBA - US)", typeof(GBAVV_TheLionKingUS_Manager))]
        TheLionKing112GBAUS,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_BrotherBear, Game.GBAVV_BrotherBear, "Brother Bear (GBA - EU)", typeof(GBAVV_BrotherBearEU_Manager))]
        BrotherBearGBAEU,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_BrotherBear, Game.GBAVV_BrotherBear, "Brother Bear (GBA - US)", typeof(GBAVV_BrotherBearUS_Manager))]
        BrotherBearGBAUS,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_SpongeBobBattleForBikiniBottom, Game.GBAVV_SpongeBobBattleForBikiniBottom, "SpongeBob SquarePants - Battle for Bikini Bottom (GBA - EU)", typeof(GBAVV_SpongeBobBattleForBikiniBottomEU_Manager))]
        SpongeBobBattleForBikiniBottomGBAEU,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_SpongeBobBattleForBikiniBottom, Game.GBAVV_SpongeBobBattleForBikiniBottom, "SpongeBob SquarePants - Battle for Bikini Bottom (GBA - US)", typeof(GBAVV_SpongeBobBattleForBikiniBottomUS_Manager))]
        SpongeBobBattleForBikiniBottomGBAUS,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_CrashNitroKart, Game.GBAVV_CrashNitroKart, "Crash Nitro Kart (GBA - EU)", typeof(GBAVV_NitroKartEU_Manager))]
        CrashNitroKartEU,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_CrashNitroKart, Game.GBAVV_CrashNitroKart, "Crash Nitro Kart (GBA - US)", typeof(GBAVV_NitroKartUS_Manager))]
        CrashNitroKartUS,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_CrashNitroKart, Game.GBAVV_CrashNitroKart, "Crash Bandicoot Bakusou! Nitro Cart (GBA - JP)", typeof(GBAVV_NitroKartJP_Manager))]
        CrashNitroKartJP,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_CrashNitroKart_NGage, Game.GBAVV_CrashNitroKart, "Crash Nitro Kart (N-Gage)", typeof(GBAVV_NitroKart_NGage_Manager))]
        CrashNitroKartNGage,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_CrashFusion, Game.GBAVV_CrashFusion, "Crash Bandicoot Fusion (GBA - EU)", typeof(GBAVV_CrashFusionEU_Manager))]
        CrashFusionGBAEU,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_CrashFusion, Game.GBAVV_CrashFusion, "Crash Bandicoot Purple Ripto's Rampage (GBA - US)", typeof(GBAVV_CrashFusionUS_Manager))]
        CrashFusionGBAUS,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_CrashFusion, Game.GBAVV_CrashFusion, "Crash Bandicoot Advance Wakuwaku Tomodachi Daisakusen! (GBA - JP)", typeof(GBAVV_CrashFusionJP_Manager))]
        CrashFusionGBAJP,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_SpyroFusion, Game.GBAVV_SpyroFusion, "Spyro Fusion (GBA - EU)", typeof(GBAVV_SpyroFusionEU_Manager))]
        SpyroFusionGBAEU,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_SpyroFusion, Game.GBAVV_SpyroFusion, "Spyro Orange The Cortex Conspiracy (GBA - US 1)", typeof(GBAVV_SpyroFusionUS_Manager))]
        SpyroFusionGBAUS,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_SpyroFusion, Game.GBAVV_SpyroFusion, "Spyro Orange The Cortex Conspiracy (GBA - US 2)", typeof(GBAVV_SpyroFusionUS2_Manager))]
        SpyroFusionGBAUS2,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_SpyroFusion, Game.GBAVV_SpyroFusion, "Spyro Advance Wakuwaku Tomodachi Daisakusen! (GBA - JP)", typeof(GBAVV_SpyroFusionJP_Manager))]
        SpyroFusionGBAJP,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_SharkTale, Game.GBAVV_SharkTale, "Shark Tale (GBA - EU/US)", typeof(GBAVV_SharkTaleEUUS_Manager))]
        SharkTaleGBA,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_SharkTale, Game.GBAVV_SharkTale, "Shark Tale (GBA - JP)", typeof(GBAVV_SharkTaleJP_Manager))]
        SharkTaleGBAJP,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_ThatsSoRaven, Game.GBAVV_ThatsSoRaven, "That's so Raven (GBA - EU/US)", typeof(GBAVV_ThatsSoRaven_Manager))]
        ThatsSoRavenGBA,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_Shrek2, Game.GBAVV_Shrek2, "Shrek 2 (GBA - EU/US)", typeof(GBAVV_Shrek2_Manager))]
        Shrek2GBA,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_Shrek2BegForMercy, Game.GBAVV_Shrek2BegForMercy, "Shrek 2 Beg for Mercy (GBA - EU/US)", typeof(GBAVV_Shrek2BegForMercy_Manager))]
        Shrek2BegForMercyGBA,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_KidsNextDoorOperationSODA, Game.GBAVV_KidsNextDoorOperationSODA, "Codename - Kids Next Door - Operation S.O.D.A. (GBA - US)", typeof(GBAVV_KidsNextDoorOperationSODA_Manager))]
        KidsNextDoorOperationSODAGBAUS,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_Madagascar, Game.GBAVV_Madagascar, "Madagascar (GBA - EU/US)", typeof(GBAVV_MadagascarEUUS_Manager))]
        MadagascarGBA,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_Madagascar, Game.GBAVV_Madagascar, "Madagascar (GBA - JP)", typeof(GBAVV_MadagascarJP_Manager))]
        MadagascarGBAJP,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_BatmanBegins, Game.GBAVV_BatmanBegins, "Batman Begins (GBA - EU/US)", typeof(GBAVV_BatmanBegins_Manager))]
        BatmanBeginsGBA,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_UltimateSpiderMan, Game.GBAVV_UltimateSpiderMan, "Ultimate Spider-Man (GBA - EU)", typeof(GBAVV_UltimateSpiderManEU_Manager))]
        UltimateSpiderManGBAEU,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_UltimateSpiderMan, Game.GBAVV_UltimateSpiderMan, "Ultimate Spider-Man (GBA - US)", typeof(GBAVV_UltimateSpiderManUS_Manager))]
        UltimateSpiderManGBAUS,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_MadagascarOperationPenguin, Game.GBAVV_MadagascarOperationPenguin, "Madagascar Operation Penguin (GBA - EU/US)", typeof(GBAVV_MadagascarOperationPenguinManager))]
        MadagascarOperationPenguinGBA,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_OverTheHedge, Game.GBAVV_OverTheHedge, "Over the Hedge (GBA - EU)", typeof(GBAVV_OverTheHedgeEU_Manager))]
        OverTheHedgeGBAEU,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_OverTheHedge, Game.GBAVV_OverTheHedge, "Over the Hedge (GBA - US)", typeof(GBAVV_OverTheHedgeUS_Manager))]
        OverTheHedgeGBAUS,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_OverTheHedgeHammyGoesNuts, Game.GBAVV_OverTheHedgeHammyGoesNuts, "Over the Hedge - Hammy Goes Nuts! (GBA - EU)", typeof(GBAVV_OverTheHedgeHammyGoesNutsEU_Manager))]
        OverTheHedgeHammyGoesNutsGBAEU,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_OverTheHedgeHammyGoesNuts, Game.GBAVV_OverTheHedgeHammyGoesNuts, "Over the Hedge - Hammy Goes Nuts! (GBA - US)", typeof(GBAVV_OverTheHedgeHammyGoesNutsUS_Manager))]
        OverTheHedgeHammyGoesNutsGBAUS,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_SpiderMan3, Game.GBAVV_SpiderMan3, "Spider-Man 3 (GBA - EU)", typeof(GBAVV_SpiderMan3EU_Manager))]
        SpiderMan3GBAEU,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_SpiderMan3, Game.GBAVV_SpiderMan3, "Spider-Man 3 (GBA - US)", typeof(GBAVV_SpiderMan3US_Manager))]
        SpiderMan3GBAUS,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_ShrekTheThird, Game.GBAVV_ShrekTheThird, "Shrek the Third (GBA - EU)", typeof(GBAVV_ShrekTheThirdEU_Manager))]
        GBAVV_ShrekTheThirdGBAEU,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_ShrekTheThird, Game.GBAVV_ShrekTheThird, "Shrek the Third (GBA - US)", typeof(GBAVV_ShrekTheThirdUS_Manager))]
        GBAVV_ShrekTheThirdGBAUS,

        #endregion

        #region Gameloft

        [GameMode(MajorEngineVersion.Gameloft, EngineVersion.Gameloft_RRR, Game.Gameloft_RRR, "Rayman Raving Rabbids (Mobile, 128x128, s40v2)", typeof(Gameloft_RRR_128x128_s40v2_Manager))]
        RaymanRavingRabbidsMobile_128x128_s40v2,

        [GameMode(MajorEngineVersion.Gameloft, EngineVersion.Gameloft_RRR, Game.Gameloft_RRR, "Rayman Raving Rabbids (Mobile, 128x128, CZ)", typeof(Gameloft_RRR_128x128_cz_Manager))]
        RaymanRavingRabbidsMobile_128x128_CZ,

        [GameMode(MajorEngineVersion.Gameloft, EngineVersion.Gameloft_RRR, Game.Gameloft_RRR, "Rayman Raving Rabbids (Mobile, 128x128, Motorola)", typeof(Gameloft_RRR_Manager))]
        RaymanRavingRabbidsMobile_128x128_Motorola,

        [GameMode(MajorEngineVersion.Gameloft, EngineVersion.Gameloft_RRR, Game.Gameloft_RRR, "Rayman Raving Rabbids (Mobile, 128x160, s40v2a)", typeof(Gameloft_RRR_128x128_s40v2_Manager))]
        RaymanRavingRabbidsMobile_128x160_s40v2a,

        [GameMode(MajorEngineVersion.Gameloft, EngineVersion.Gameloft_RRR, Game.Gameloft_RRR, "Rayman Raving Rabbids (Mobile, 128x160, Samsung SGH-J700/X660)", typeof(Gameloft_RRR_128x128_s40v2_Manager))]
        RaymanRavingRabbidsMobile_128x160_SamsungX660,

        [GameMode(MajorEngineVersion.Gameloft, EngineVersion.Gameloft_RRR, Game.Gameloft_RRR, "Rayman Raving Rabbids (Mobile, 128x160, Samsung E360/E370)", typeof(Gameloft_RRR_128x128_s40v2_Manager))]
        RaymanRavingRabbidsMobile_128x160_SamsungE360,

        [GameMode(MajorEngineVersion.Gameloft, EngineVersion.Gameloft_RRR, Game.Gameloft_RRR, "Rayman Raving Rabbids (Mobile, 128x160, Sony Ericsson)", typeof(Gameloft_RRR_Manager))]
        RaymanRavingRabbidsMobile_128x160_SonyEricsson,

        [GameMode(MajorEngineVersion.Gameloft, EngineVersion.Gameloft_RRR, Game.Gameloft_RRR, "Rayman Raving Rabbids (Mobile, 176x208, s60v1)", typeof(Gameloft_RRR_Manager))]
        RaymanRavingRabbidsMobile_176x208_s60v1,

        [GameMode(MajorEngineVersion.Gameloft, EngineVersion.Gameloft_RRR, Game.Gameloft_RRR, "Rayman Raving Rabbids (Mobile, 176x208, s60v2)", typeof(Gameloft_RRR_Manager))]
        RaymanRavingRabbidsMobile_176x208_s60v2,

        [GameMode(MajorEngineVersion.Gameloft, EngineVersion.Gameloft_RRR, Game.Gameloft_RRR, "Rayman Raving Rabbids (Mobile, 208x208, s40v3)", typeof(Gameloft_RRR_Manager))]
        RaymanRavingRabbidsMobile_208x208_s40v3,

        [GameMode(MajorEngineVersion.Gameloft, EngineVersion.Gameloft_RRR, Game.Gameloft_RRR, "Rayman Raving Rabbids (Mobile, 240x320, s40v3a)", typeof(Gameloft_RRRHighRes_Manager))]
        RaymanRavingRabbidsMobile_240x320_s40v3a,

        [GameMode(MajorEngineVersion.Gameloft, EngineVersion.Gameloft_RRR, Game.Gameloft_RRR, "Rayman Raving Rabbids (Mobile, 240x320, Samsung SGH-F400)", typeof(Gameloft_RRRHighRes_Manager))]
        RaymanRavingRabbidsMobile_240x320_SamsungF400,

        [GameMode(MajorEngineVersion.Gameloft, EngineVersion.Gameloft_RRR, Game.Gameloft_RRR, "Rayman Raving Rabbids (Mobile, 240x320, Samsung SGH-D900)", typeof(Gameloft_RRRHighRes_Manager))]
        RaymanRavingRabbidsMobile_240x320_SamsungD900,

        [GameMode(MajorEngineVersion.Gameloft, EngineVersion.Gameloft_RRR, Game.Gameloft_RRR, "Rayman Raving Rabbids (Mobile, 240x320, Samsung SGH-F480V)", typeof(Gameloft_RRRHighRes_Manager))]
        RaymanRavingRabbidsMobile_240x320_SamsungF480V,


        [GameMode(MajorEngineVersion.Gameloft, EngineVersion.Gameloft_RK, Game.Gameloft_RK_LowRes, "Rayman Kart (Mobile, 128x128)", typeof(Gameloft_RK_128x128_Manager))]
        RaymanKartMobile_128x128,

        [GameMode(MajorEngineVersion.Gameloft, EngineVersion.Gameloft_RK, Game.Gameloft_RK_LowRes, "Rayman Kart (Mobile, 128x128, s40v2)", typeof(Gameloft_RK_128x128_s40v2_Manager))]
        RaymanKartMobile_128x128_s40v2,

        [GameMode(MajorEngineVersion.Gameloft, EngineVersion.Gameloft_RK, Game.Gameloft_RK, "Rayman Kart (Mobile, 128x160, s40v2a/N6101)", typeof(Gameloft_RK_128x128_s40v2_Manager))]
        RaymanKartMobile_128x160_s40v2a_N6101,

        [GameMode(MajorEngineVersion.Gameloft, EngineVersion.Gameloft_RK, Game.Gameloft_RK, "Rayman Kart (Mobile, 128x160, s40v2a/N6151)", typeof(Gameloft_RK_128x160_Manager))]
        RaymanKartMobile_128x160_s40v2a_N6151,

        [GameMode(MajorEngineVersion.Gameloft, EngineVersion.Gameloft_RK, Game.Gameloft_RK, "Rayman Kart (Mobile, 128x160, Sony Ericsson K500i)", typeof(Gameloft_RK_128x160_Manager))]
        RaymanKartMobile_128x160_K500i,

        [GameMode(MajorEngineVersion.Gameloft, EngineVersion.Gameloft_RK, Game.Gameloft_RK, "Rayman Kart (Mobile, 176x208, s60v1/N3650)", typeof(Gameloft_RK_128x160_Manager))]
        RaymanKartMobile_176x208_s60v1_N3650,

        [GameMode(MajorEngineVersion.Gameloft, EngineVersion.Gameloft_RK, Game.Gameloft_RK, "Rayman Kart (Mobile, 176x208, s60v1/N7650)", typeof(Gameloft_RK_128x160_Manager))]
        RaymanKartMobile_176x208_s60v1_N7650,

        [GameMode(MajorEngineVersion.Gameloft, EngineVersion.Gameloft_RK, Game.Gameloft_RK, "Rayman Kart (Mobile, 176x208, s60v2/N70)", typeof(Gameloft_RK_128x160_Manager))]
        RaymanKartMobile_176x208_s60v2_N70,

        [GameMode(MajorEngineVersion.Gameloft, EngineVersion.Gameloft_RK, Game.Gameloft_RK, "Rayman Kart (Mobile, 208x208, s40v3)", typeof(Gameloft_RK_Manager))]
        RaymanKartMobile_208x208_s40v3,

        [GameMode(MajorEngineVersion.Gameloft, EngineVersion.Gameloft_RK, Game.Gameloft_RK, "Rayman Kart (Mobile, 176x220, Sony Ericsson K700i)", typeof(Gameloft_RK_Manager))]
        RaymanKartMobile_176x220_K700i,

        [GameMode(MajorEngineVersion.Gameloft, EngineVersion.Gameloft_RK, Game.Gameloft_RK, "Rayman Kart (Mobile, 176x220, Samsung SGH-ZV60)", typeof(Gameloft_RK_176x220_Manager))]
        RaymanKartMobile_176x220_SamsungZV60,

        [GameMode(MajorEngineVersion.Gameloft, EngineVersion.Gameloft_RK, Game.Gameloft_RK, "Rayman Kart (Mobile, 176x220, Samsung SGH-ZV10)", typeof(Gameloft_RK_Manager))]
        RaymanKartMobile_176x220_SamsungZV10,

        [GameMode(MajorEngineVersion.Gameloft, EngineVersion.Gameloft_RK, Game.Gameloft_RK, "Rayman Kart (Mobile, 176x220, RAZR V3/KRZR K1)", typeof(Gameloft_RK_128x160_Manager))]
        RaymanKartMobile_176x220_RAZRV3,

        [GameMode(MajorEngineVersion.Gameloft, EngineVersion.Gameloft_RK, Game.Gameloft_RK, "Rayman Kart (Mobile, 176x220, U8110/U8360)", typeof(Gameloft_RK_128x160_Manager))]
        RaymanKartMobile_176x220_U8110,

        [GameMode(MajorEngineVersion.Gameloft, EngineVersion.Gameloft_RK, Game.Gameloft_RK_HighResHalf, "Rayman Kart (Mobile, 176x220, KG800)", typeof(Gameloft_RK_Manager))]
        RaymanKartMobile_176x220_KG800,

        [GameMode(MajorEngineVersion.Gameloft, EngineVersion.Gameloft_RK, Game.Gameloft_RK, "Rayman Kart (Mobile, 240x320, s40v3a)", typeof(Gameloft_RK_240x320_s40v3a_Manager))]
        RaymanKartMobile_240x320_s40v3a,

        [GameMode(MajorEngineVersion.Gameloft, EngineVersion.Gameloft_RK, Game.Gameloft_RK, "Rayman Kart (Mobile, 240x320, s40v5)", typeof(Gameloft_RK_240x320_s40v3a_Manager))]
        RaymanKartMobile_240x320_s40v5,

        [GameMode(MajorEngineVersion.Gameloft, EngineVersion.Gameloft_RK, Game.Gameloft_RK, "Rayman Kart (Mobile, 240x320, Sony Ericsson W910i)", typeof(Gameloft_RK_Manager))]
        RaymanKartMobile_240x320_W910i,

        [GameMode(MajorEngineVersion.Gameloft, EngineVersion.Gameloft_RK, Game.Gameloft_RK, "Rayman Kart (Mobile, 240x320, Sony Ericsson C905)", typeof(Gameloft_RK_Manager))]
        RaymanKartMobile_240x320_C905,

        [GameMode(MajorEngineVersion.Gameloft, EngineVersion.Gameloft_RK, Game.Gameloft_RK, "Rayman Kart (Mobile, 240x320, Samsung SGH-F400)", typeof(Gameloft_RK_Manager))]
        RaymanKartMobile_240x320_SamsungF400,

        [GameMode(MajorEngineVersion.Gameloft, EngineVersion.Gameloft_RK, Game.Gameloft_RK, "Rayman Kart (Mobile, 240x320, Samsung SGH-F480)", typeof(Gameloft_RK_Manager))]
        RaymanKartMobile_240x320_SamsungF480,

        [GameMode(MajorEngineVersion.Gameloft, EngineVersion.Gameloft_RK, Game.Gameloft_RK, "Rayman Kart (Mobile, 320x240, LG KS360)", typeof(Gameloft_RK_Manager))]
        RaymanKartMobile_320x240_KS360,

        [GameMode(MajorEngineVersion.Gameloft, EngineVersion.Gameloft_RK, Game.Gameloft_RK, "Rayman Kart (Mobile, 320x240, Broken)", typeof(Gameloft_RK_Manager))]
        RaymanKartMobile_320x240_Broken,

        #endregion

        #region Jade

        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_BGE, Game.Jade_BGE, "Beyond Good & Evil (PC)", typeof(Jade_BGE_PC_Manager), Platform.PC)]
        BeyondGoodAndEvilPC,

        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_BGE, Game.Jade_BGE, "Beyond Good & Evil (PC - Demo)", typeof(Jade_BGE_PCDemo_Manager), Platform.PC)]
        BeyondGoodAndEvilPCDemo,

        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_BGE, Game.Jade_BGE, "Beyond Good & Evil (GC)", typeof(Jade_BGE_GC_Manager), Platform.GC)]
        BeyondGoodAndEvilGC,

        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_BGE, Game.Jade_BGE, "Beyond Good & Evil (PS2 - Demo 2003/08/05)", typeof(Jade_BGE_PS2_20030805_Manager), Platform.PS2)]
        BeyondGoodAndEvilPS2_20030805,

        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_BGE, Game.Jade_BGE, "Beyond Good & Evil (PS2 - Prototype 2003/08/14)", typeof(Jade_BGE_PS2_20030814_Manager), Platform.PS2)]
        BeyondGoodAndEvilPS2_20030814,

        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_BGE, Game.Jade_BGE, "Beyond Good & Evil (PS2)", typeof(Jade_BGE_PS2_Manager), Platform.PS2)]
        BeyondGoodAndEvilPS2,

        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_BGE, Game.Jade_BGE, "Beyond Good & Evil (Xbox)", typeof(Jade_BGE_Xbox_Manager), Platform.Xbox)]
        BeyondGoodAndEvilXbox,

        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_BGE_HD, Game.Jade_BGE, "Beyond Good & Evil HD (PS3)", typeof(Jade_BGE_HD_Manager), Platform.PS3)]
        BeyondGoodAndEvilPS3,

        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_BGE_HD, Game.Jade_BGE, "Beyond Good & Evil HD (Xbox 360)", typeof(Jade_BGE_HD_Manager), Platform.Xbox360)]
        BeyondGoodAndEvilXbox360,

		[GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_BGE_Anniversary, Game.Jade_BGE, "Beyond Good & Evil 20th Anniversary (PC)", typeof(Jade_BGE_Anniversary_Manager), Platform.PC)]
		BeyondGoodAndEvilAnniversaryPC,

		[GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_BGE_Anniversary, Game.Jade_BGE, "Beyond Good & Evil 20th Anniversary (Switch)", typeof(Jade_BGE_Anniversary_Manager), Platform.Switch)]
		BeyondGoodAndEvilAnniversarySwitch,


		[GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_KingKong, Game.Jade_KingKong, "King Kong (PC)", typeof(Jade_KingKong_PC_Manager), Platform.PC)]
        KingKongPC,

        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_KingKong, Game.Jade_KingKong, "King Kong (GC)", typeof(Jade_KingKong_GC_Manager), Platform.GC)]
        KingKongGC,

        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_KingKong, Game.Jade_KingKong, "King Kong (PS2)", typeof(Jade_KingKong_PS2_Manager), Platform.PS2)]
        KingKongPS2,

        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_KingKong, Game.Jade_KingKong, "King Kong (Xbox)", typeof(Jade_KingKong_Xbox_Manager), Platform.Xbox)]
        KingKongXbox,

        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_KingKong, Game.Jade_KingKong, "King Kong (Xbox - Demo 2005/07/28)", typeof(Jade_KingKong_Xbox_20050728_Manager), Platform.Xbox)]
        KingKongXbox_20050728,

        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_KingKong, Game.Jade_KingKong, "King Kong (PSP)", typeof(Jade_KingKong_PSP_Manager), Platform.PSP)]
        KingKongPSP,

        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_KingKong_Xenon, Game.Jade_KingKong, "King Kong (PC - Gamer's Edition)", typeof(Jade_KingKong_PCGamersEdition_Manager), Platform.PC, flags: EngineFlags.Jade_Xenon)]
        KingKongPCGamersEdition,

        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_KingKong_Xenon, Game.Jade_KingKong, "King Kong (Xbox 360)", typeof(Jade_KingKong_Xbox360_Manager), Platform.Xbox360, flags: EngineFlags.Jade_Xenon)]
        KingKongXbox360,

        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_KingKong_Xenon, Game.Jade_KingKong, "King Kong (Xbox 360 - Demo 2005/09/26)", typeof(Jade_KingKong_Xbox360_20050926_Manager), Platform.Xbox360, flags: EngineFlags.Jade_Xenon)]
        KingKongXbox360_20050926,


        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_RRR, Game.Jade_RRR, "Rayman Raving Rabbids (PC)", typeof(Jade_RRR_PC_Manager), Platform.PC)]
        RaymanRavingRabbidsPC,

        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_RRR, Game.Jade_RRR, "Rayman Raving Rabbids (PC - Demo)", typeof(Jade_RRR_PCDemo_Manager), Platform.PC)]
        RaymanRavingRabbidsPCDemo,

		[GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_RRRPrototype, Game.Jade_RRR, "Rayman Raving Rabbids (PC - Prototype)", typeof(Jade_RRR_PC_Prototype_Manager), Platform.PC)]
		RaymanRavingRabbidsPCPrototype,

		[GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_RRR, Game.Jade_RRR, "Rayman Raving Rabbids (PC - Unbinarized)", typeof(Jade_RRR_PC_Unbinarized_Modded_Manager), Platform.PC)]
        RaymanRavingRabbidsPCUnbinarized,

        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_RRR, Game.Jade_RRR, "Rayman Raving Rabbids (Wii)", typeof(Jade_RRR_Wii_Manager), Platform.Wii)]
        RaymanRavingRabbidsWii,

        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_RRR, Game.Jade_RRR, "Rayman Raving Rabbids (Wii - JP)", typeof(Jade_RRR_Wii_Manager), Platform.Wii)]
        RaymanRavingRabbidsWiiJP,

        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_RRR, Game.Jade_RRR, "Rayman Raving Rabbids (Xbox 360)", typeof(Jade_RRR_Xbox360_Manager), Platform.Xbox360, flags: EngineFlags.Jade_Xenon)]
        RaymanRavingRabbidsXbox360,

		[GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_RRR, Game.Jade_RRR, "Rayman Raving Rabbids (Xbox 360 - 2007/02/13)", typeof(Jade_RRR_Xbox360_20070213_Manager), Platform.Xbox360, flags: EngineFlags.Jade_Xenon)]
		RaymanRavingRabbidsXbox360_20070213,

		[GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_RRR, Game.Jade_RRR, "Rayman Raving Rabbids (PS2)", typeof(Jade_RRR_PS2_Manager), Platform.PS2)]
        RaymanRavingRabbidsPS2,

		[GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_RRR, Game.Jade_RRR, "Rayman Raving Rabbids (PS2 - 2006/10/13)", typeof(Jade_RRR_PS2_Manager), Platform.PS2)]
		RaymanRavingRabbidsPS2_20061013,

		[GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_Horsez, Game.Jade_Horsez, "Horsez (PS2)", typeof(Jade_Horsez_PS2_Manager), Platform.PS2)]
        HorsezPS2,

        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_Horsez2, Game.Jade_Horsez2, "Horsez 2: Ranch Rescue (PS2)", typeof(Jade_Horsez2_PS2_Manager), Platform.PS2)]
        Horsez2PS2,

        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_Horsez2, Game.Jade_Horsez2, "Horsez 2: Ranch Rescue (PSP)", typeof(Jade_Horsez2_PSP_Manager), Platform.PSP)]
        Horsez2PSP,

        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_Horsez2, Game.Jade_Horsez2, "Horsez 2: Ranch Rescue (PSP - Demo)", typeof(Jade_Horsez2_PSPDemo_Manager), Platform.PSP)]
        Horsez2PSPDemo,

        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_Horsez2, Game.Jade_Horsez2, "Horsez 2: Ranch Rescue (Wii)", typeof(Jade_Horsez2_Wii_Manager), Platform.Wii)]
        Horsez2Wii,

        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_Horsez2, Game.Jade_Horsez2, "Horsez 2: Ranch Rescue (PC)", typeof(Jade_Horsez2_PC_Manager), Platform.PC)]
        Horsez2PC,

        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_Horsez2, Game.Jade_Horsez2, "Horsez 2: Ranch Rescue (PC - HD)", typeof(Jade_Horsez2_PC_HD_Manager), Platform.PC)]
        Horsez2PCHD,

        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_PetzHorseClub, Game.Jade_PetzHorseClub, "Petz: Horse Club (Wii)", typeof(Jade_PetzHorseClub_Wii_Manager), Platform.Wii)]
        PetzHorseClubWii,

        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_PetzHorseClub, Game.Jade_PetzHorseClub, "Petz: Horse Club (PC)", typeof(Jade_PetzHorseClub_PC_Manager), Platform.PC)]
        PetzHorseClubPC,

        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_PetzHorseClub, Game.Jade_PetzHorseClub, "Petz: Horse Club (PC - HD)", typeof(Jade_PetzHorseClub_PC_HD_Manager), Platform.PC)]
        PetzHorseClubPCHD,

        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_MovieGames, Game.Jade_MovieGames, "Movie Games (Wii)", typeof(Jade_MovieGames_Wii_Manager), Platform.Wii)]
        MovieGamesWii,


        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_RRR2, Game.Jade_RRR2, "Rayman Raving Rabbids 2 (PC)", typeof(Jade_RRR2_PC_Manager), Platform.PC)]
        RaymanRavingRabbids2PC,

        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_RRR2, Game.Jade_RRR2, "Rayman Raving Rabbids 2 (Wii)", typeof(Jade_RRR2_Wii_Manager), Platform.Wii)]
        RaymanRavingRabbids2Wii,

		[GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_RRR2, Game.Jade_RRR2, "Rayman Raving Rabbids 2 (Wii - 2007/09/01)", typeof(Jade_RRR2_Wii_20070901_Manager), Platform.Wii)]
		RaymanRavingRabbids2Wii_20070901,

		[GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_PoP_SoT, Game.Jade_PoP_SoT, "Prince of Persia: The Sands of Time (PS2)", typeof(Jade_PoP_SoT_PS2_Manager), Platform.PS2)]
        PrinceOfPersiaTheSandsOfTimePS2,

		[GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_PoP_SoT_20030723, Game.Jade_PoP_SoT, "Prince of Persia: The Sands of Time (PS2 - Demo 2003/07/10)", typeof(Jade_PoP_SoT_PS2_20030723_Manager), Platform.PS2)]
		PrinceOfPersiaTheSandsOfTimePS2_20030710, // Earlier version of PrinceOfPersiaTheSandsOfTimePS2_20030723. Same levels, so same manager

		[GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_PoP_SoT_20030723, Game.Jade_PoP_SoT, "Prince of Persia: The Sands of Time (PS2 - Demo 2003/07/23)", typeof(Jade_PoP_SoT_PS2_20030723_Manager), Platform.PS2)]
        PrinceOfPersiaTheSandsOfTimePS2_20030723,

        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_PoP_SoT_20030819, Game.Jade_PoP_SoT, "Prince of Persia: The Sands of Time (PS2 - Prototype 2003/08/19)", typeof(Jade_PoP_SoT_PS2_20030819_Manager), Platform.PS2)]
        PrinceOfPersiaTheSandsOfTimePS2_20030819,

        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_PoP_SoT, Game.Jade_PoP_SoT, "Prince of Persia: The Sands of Time (GC)", typeof(Jade_PoP_SoT_GC_Manager), Platform.GC)]
        PrinceOfPersiaTheSandsOfTimeGC,

        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_PoP_SoT, Game.Jade_PoP_SoT, "Prince of Persia: The Sands of Time (PC)", typeof(Jade_PoP_SoT_PC_Manager), Platform.PC)]
        PrinceOfPersiaTheSandsOfTimePC,

		[GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_PoP_SoT, Game.Jade_PoP_SoT, "Prince of Persia: The Sands of Time (PC - Demo 2003/12/17)", typeof(Jade_PoP_SoT_PC_20031217_Manager), Platform.PC)]
		PrinceOfPersiaTheSandsOfTimePC_20031217, // OEM Demo

		[GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_PoP_SoT, Game.Jade_PoP_SoT, "Prince of Persia: The Sands of Time (PC - Demo 2004/02/27)", typeof(Jade_PoP_SoT_PC_20040227_Manager), Platform.PC)]
		PrinceOfPersiaTheSandsOfTimePC_20040227, // Limited Demo

		[GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_PoP_SoT, Game.Jade_PoP_SoT, "Prince of Persia: The Sands of Time (Xbox)", typeof(Jade_PoP_SoT_Xbox_Manager), Platform.Xbox)]
        PrinceOfPersiaTheSandsOfTimeXbox,

        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_PoP_SoT_20030723, Game.Jade_PoP_SoT, "Prince of Persia: The Sands of Time (Xbox - Demo 2003/07/23)", typeof(Jade_PoP_SoT_Xbox_20030723_Manager), Platform.Xbox)]
        PrinceOfPersiaTheSandsOfTimeXbox_20030723,

        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_PoP_SoT, Game.Jade_PoP_SoT, "Prince of Persia: The Sands of Time (PS3)", typeof(Jade_PoP_SoT_PS3_Manager), Platform.PS3)]
        PrinceOfPersiaTheSandsOfTimePS3,


        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_PoP_WW, Game.Jade_PoP_WW, "Prince of Persia: Warrior Within (PC)", typeof(Jade_PoP_WW_PC_Manager), Platform.PC)]
        PrinceOfPersiaWarriorWithinPC,

        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_PoP_WW, Game.Jade_PoP_WW, "Prince of Persia: Warrior Within (PC - Demo)", typeof(Jade_PoP_WW_PC_Demo_Manager), Platform.PC)]
        PrinceOfPersiaWarriorWithinPCDemo,

        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_PoP_WW, Game.Jade_PoP_WW, "Prince of Persia: Warrior Within (GC)", typeof(Jade_PoP_WW_GC_Manager), Platform.GC)]
        PrinceOfPersiaWarriorWithinGC,

        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_PoP_WW, Game.Jade_PoP_WW, "Prince of Persia: Warrior Within (PS2)", typeof(Jade_PoP_WW_PS2_Manager), Platform.PS2)]
        PrinceOfPersiaWarriorWithinPS2,

        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_PoP_WW_20040920, Game.Jade_PoP_WW, "Prince of Persia: Warrior Within (PS2 - Demo 2004/09/20)", typeof(Jade_PoP_WW_PS2_20040920_Manager), Platform.PS2)]
        PrinceOfPersiaWarriorWithinPS2_20040920,

        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_PoP_WW_20041024, Game.Jade_PoP_WW, "Prince of Persia: Warrior Within (PS2 - Prototype 2004/10/24)", typeof(Jade_PoP_WW_PS2_20041024_Manager), Platform.PS2)]
        PrinceOfPersiaWarriorWithinPS2_20041024,

        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_PoP_WW, Game.Jade_PoP_WW, "Prince of Persia: Warrior Within (PSP)", typeof(Jade_PoP_WW_PSP_Manager), Platform.PSP)]
        PrinceOfPersiaWarriorWithinPSP,

        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_PoP_WW, Game.Jade_PoP_WW, "Prince of Persia: Warrior Within (Xbox)", typeof(Jade_PoP_WW_Xbox_Manager), Platform.Xbox)]
        PrinceOfPersiaWarriorWithinXbox,

        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_PoP_WW, Game.Jade_PoP_WW, "Prince of Persia: Warrior Within (iOS)", typeof(Jade_PoP_WW_iOS_Manager), Platform.iOS)]
        PrinceOfPersiaWarriorWithinIOS,

		[GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_PoP_WW, Game.Jade_PoP_WW, "Prince of Persia: Warrior Within (iOS - HD)", typeof(Jade_PoP_WW_iOS_HD_Manager), Platform.iOS)]
		PrinceOfPersiaWarriorWithinIOSHD,

		[GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_PoP_WW, Game.Jade_PoP_WW, "Prince of Persia: Warrior Within (iOS - Demo)", typeof(Jade_PoP_WW_iOS_Demo_Manager), Platform.iOS)]
		PrinceOfPersiaWarriorWithinIOSDemo,

		[GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_PoP_WW, Game.Jade_PoP_WW, "Prince of Persia: Warrior Within (iOS 3G OS4 - Demo)", typeof(Jade_PoP_WW_iOS_3GOS4_Demo_Manager), Platform.iOS)]
		PrinceOfPersiaWarriorWithinIOS3GOS4Demo,

		[GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_PoP_WW, Game.Jade_PoP_WW, "Prince of Persia: Warrior Within (PS3)", typeof(Jade_PoP_WW_PS3_Manager), Platform.PS3)]
        PrinceOfPersiaWarriorWithinPS3,


        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_PoP_T2T, Game.Jade_PoP_T2T, "Prince of Persia: The Two Thrones (PC)", typeof(Jade_PoP_T2T_PC_Manager), Platform.PC)]
        PrinceOfPersiaTheTwoThronesPC,

        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_PoP_T2T, Game.Jade_PoP_T2T, "Prince of Persia: The Two Thrones (GC)", typeof(Jade_PoP_T2T_GC_Manager), Platform.GC)]
        PrinceOfPersiaTheTwoThronesGC,

        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_PoP_T2T, Game.Jade_PoP_T2T, "Prince of Persia: The Two Thrones (Wii)", typeof(Jade_PoP_T2T_Wii_Manager), Platform.Wii)]
        PrinceOfPersiaTheTwoThronesWii,

        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_PoP_T2T, Game.Jade_PoP_T2T, "Prince of Persia: The Two Thrones (PS2)", typeof(Jade_PoP_T2T_PS2_Manager), Platform.PS2)]
        PrinceOfPersiaTheTwoThronesPS2,

		[GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_PoP_T2T_20051002, Game.Jade_PoP_T2T, "Prince of Persia: The Two Thrones (PS2 - Prototype 2005/09/30)", typeof(Jade_PoP_T2T_PS2_20050930_Manager), Platform.PS2)]
		PrinceOfPersiaTheTwoThronesPS2_20050930,

		[GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_PoP_T2T, Game.Jade_PoP_T2T, "Prince of Persia: The Two Thrones (PSP)", typeof(Jade_PoP_T2T_PSP_Manager), Platform.PSP)]
        PrinceOfPersiaTheTwoThronesPSP,

        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_PoP_T2T, Game.Jade_PoP_T2T, "Prince of Persia: The Two Thrones (Xbox)", typeof(Jade_PoP_T2T_Xbox_Manager), Platform.Xbox)]
        PrinceOfPersiaTheTwoThronesXbox,

        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_PoP_T2T_20051002, Game.Jade_PoP_T2T, "Prince of Persia: The Two Thrones (Xbox - Prototype 2005/10/02)", typeof(Jade_PoP_T2T_Xbox_20051002_Manager), Platform.Xbox)]
        PrinceOfPersiaTheTwoThronesXbox_20051002,

        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_PoP_T2T, Game.Jade_PoP_T2T, "Prince of Persia: The Two Thrones (PS3)", typeof(Jade_PoP_T2T_PS3_Manager), Platform.PS3)]
        PrinceOfPersiaTheTwoThronesPS3,


		[GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_PoP_TFS, Game.Jade_PoP_TFS, "Prince of Persia: The Forgotten Sands (Wii)", typeof(Jade_PoP_TFS_Wii_Manager), Platform.Wii)]
		PrinceOfPersiaTheForgottenSandsWii,

		[GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_PoP_TFS_PSP, Game.Jade_PoP_TFS, "Prince of Persia: The Forgotten Sands (PSP)", typeof(Jade_PoP_TFS_PSP_Manager), Platform.PSP)]
		PrinceOfPersiaTheForgottenSandsPSP,


		[GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_Beowulf, Game.Jade_Beowulf, "Beowulf (PSP)", typeof(Jade_Beowulf_PSP_Manager), Platform.PSP)]
		BeowulfPSP,


		[GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_MyWordCoach, Game.Jade_MyWordCoach, "My Word Coach (Wii)", typeof(Jade_MyWordCoach_Wii_Manager), Platform.Wii)]
        MyWordCoachWii,

        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_MyWordCoach, Game.Jade_MyFrenchCoach, "My French Coach (Wii)", typeof(Jade_MyFrenchCoach_Wii_Manager), Platform.Wii)]
        MyFrenchCoachWii,

        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_MyWordCoach, Game.Jade_MySpanishCoach, "My Spanish Coach (Wii)", typeof(Jade_MySpanishCoach_Wii_Manager), Platform.Wii)]
        MySpanishCoachWii,


		[GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_TMNT, Game.Jade_TMNT, "TMNT (PC)", typeof(Jade_TMNT_PC_Manager), Platform.PC)]
		TMNTPC,

		[GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_TMNT, Game.Jade_TMNT, "TMNT (PS2)", typeof(Jade_TMNT_PS2_Manager), Platform.PS2)]
		TMNTPS2,

		[GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_TMNT, Game.Jade_TMNT, "TMNT (GC)", typeof(Jade_TMNT_GC_Manager), Platform.GC)]
		TMNTGC,

		[GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_TMNT, Game.Jade_TMNT, "TMNT (Wii)", typeof(Jade_TMNT_Wii_Manager), Platform.Wii)]
		TMNTWii,


		[GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_Avatar, Game.Jade_Avatar, "James Cameron's Avatar: The Game (Wii)", typeof(Jade_Avatar_Wii_Manager), Platform.Wii)]
		AvatarWii,

		[GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_Avatar, Game.Jade_Avatar, "James Cameron's Avatar: The Game (PSP)", typeof(Jade_Avatar_PSP_Manager), Platform.PSP)]
		AvatarPSP,


		[GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_RRRTVParty, Game.Jade_RRRTVParty, "Rayman Raving Rabbids: TV Party (Wii)", typeof(Jade_RRRTVParty_Wii_Manager), Platform.Wii)]
        RaymanRavingRabbidsTVPartyWii,

		[GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_JustDance, Game.Jade_JustDance, "Just Dance (Wii)", typeof(Jade_JustDance_Wii_Manager), Platform.Wii)]
		JustDanceWii,

        [GameMode(MajorEngineVersion.Jade, EngineVersion.Jade_Naruto1RiseOfANinja, Game.Jade_Naruto1RiseOfANinja, "Naruto: Rise of a Ninja (Xbox 360)", typeof(Jade_NarutoRiseOfANinja_X360_Manager), Platform.Xbox360)]
        Naruto1RiseOfANinjaXbox360,

		#endregion

		#region GEN

		[GameMode(MajorEngineVersion.GEN, EngineVersion.GEN, Game.GEN_RaymanEveil, "Rayman Eveil (PC)", typeof(GEN_BaseManager), Platform.PC)]
        RaymanEveilPC,

        #endregion

        #region GBA Klonoa

        [GameMode(MajorEngineVersion.GBAKlonoa, EngineVersion.KlonoaGBA_EOD, Game.KlonoaGBA_EmpireOfDreams, "Klonoa Empire of Dreams (GBA - US)", typeof(GBAKlonoa_EOD_Manager), Platform.GBA)]
        KlonoaEmpireOfDreamsGBAUS,

        [GameMode(MajorEngineVersion.GBAKlonoa, EngineVersion.KlonoaGBA_DCT, Game.KlonoaGBA_DreamChampTournament, "Klonoa 2 Dream Champ Tournament (GBA - US)", typeof(GBAKlonoa_DCT_Manager), Platform.GBA)]
        KlonoaDreamChampTournamentGBAUS,

        #endregion

        #region Klonoa Heroes

        [GameMode(MajorEngineVersion.KlonoaHeroes, EngineVersion.KlonoaHeroes, Game.KlonoaHeroes_Heroes, "Klonoa Heroes (GBA - JP)", typeof(KlonoaHeroes_Manager), Platform.GBA)]
        KlonoaHeroesGBAJP,

        #endregion

        #region PS1 Klonoa

        [GameMode(MajorEngineVersion.PSKlonoa, EngineVersion.PSKlonoa_DTP, Game.PS1Klonoa_DoorToPhantomile, "Klonoa Door to Phantomile (PS1 - Prototype 1997/07/17)", typeof(PSKlonoa_DTP_Manager_Prototype_19970717), Platform.PS1)]
        KlonoaDoorToPhantomilePS1USPrototype_19970717,

        [GameMode(MajorEngineVersion.PSKlonoa, EngineVersion.PSKlonoa_DTP, Game.PS1Klonoa_DoorToPhantomile, "Klonoa Door to Phantomile (PS1 - US)", typeof(PSKlonoa_DTP_Manager_US), Platform.PS1)]
        KlonoaDoorToPhantomilePS1US,

        [GameMode(MajorEngineVersion.PSKlonoa, EngineVersion.PSKlonoa_DTP, Game.PS1Klonoa_DoorToPhantomile, "Klonoa Door to Phantomile (PS2)", typeof(PSKlonoa_DTP_Manager_PS2), Platform.PS2)]
        KlonoaDoorToPhantomilePS2,

        #endregion

        #region Psychonauts

        [GameMode(MajorEngineVersion.Psychonauts, EngineVersion.Psychonauts, Game.Psychonauts_Psychonauts, "Psychonauts (Xbox - Prototype 2004/12/17)", typeof(Psychonauts_Manager), Platform.Xbox)]
        Psychonauts_Xbox_Proto_20041217,

        // TODO: Xbox 2005 prototype
        // TODO: Xbox
        // TODO: PC retail
        // TODO: PS4

        [GameMode(MajorEngineVersion.Psychonauts, EngineVersion.Psychonauts, Game.Psychonauts_Psychonauts, "Psychonauts (PC - Digital)", typeof(Psychonauts_Manager), Platform.PC)]
        Psychonauts_PC_Digital,

        [GameMode(MajorEngineVersion.Psychonauts, EngineVersion.Psychonauts, Game.Psychonauts_Psychonauts, "Psychonauts (PS2 - EU)", typeof(Psychonauts_Manager_PS2), Platform.PS2)]
        Psychonauts_PS2_EU,

        [GameMode(MajorEngineVersion.Psychonauts, EngineVersion.Psychonauts, Game.Psychonauts_Psychonauts, "Psychonauts (PS2 - US)", typeof(Psychonauts_Manager_PS2), Platform.PS2)]
        Psychonauts_PS2_US,

        [GameMode(MajorEngineVersion.Psychonauts, EngineVersion.Psychonauts, Game.Psychonauts_Psychonauts, "Psychonauts (PS2 - US Demo)", typeof(Psychonauts_Manager_PS2), Platform.PS2)]
        Psychonauts_PS2_US_Demo,

        #endregion
    }
}