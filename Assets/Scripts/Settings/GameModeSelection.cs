using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace R1Engine
{
    /// <summary>
    /// The available game modes to select from
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum GameModeSelection
    {
        // Rayman 1

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

        [GameMode(MajorEngineVersion.Rayman1, EngineVersion.R1_Saturn, Game.R1_Rayman1, "Rayman 1 (Saturn - US)", typeof(R1_SaturnUS_Manager))]
        RaymanSaturnUS,

        [GameMode(MajorEngineVersion.Rayman1, EngineVersion.R1_Saturn, Game.R1_Rayman1, "Rayman 1 (Saturn - Prototype)", typeof(R1_SaturnProto_Manager))]
        RaymanSaturnProto,

        [GameMode(MajorEngineVersion.Rayman1, EngineVersion.R1_Saturn, Game.R1_Rayman1, "Rayman 1 (Saturn - EU)", typeof(R1_SaturnEU_Manager))]
        RaymanSaturnEU,

        [GameMode(MajorEngineVersion.Rayman1, EngineVersion.R1_Saturn, Game.R1_Rayman1, "Rayman 1 (Saturn - JP)", typeof(R1_SaturnJP_Manager))]
        RaymanSaturnJP,

        [GameMode(MajorEngineVersion.Rayman1, EngineVersion.R1_Saturn, Game.R1_Rayman1, "Rayman 1 (Saturn - US Demo)", typeof(R1_SaturnUSDemo_Manager))]
        RaymanSaturnUSDemo,

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

        // Rayman 1 Jaguar

        [GameMode(MajorEngineVersion.Rayman1_Jaguar, EngineVersion.R1Jaguar, Game.R1_Rayman1, "Rayman 1 (Jaguar)", typeof(R1Jaguar_Manager))]
        RaymanJaguar,

        [GameMode(MajorEngineVersion.Rayman1_Jaguar, EngineVersion.R1Jaguar_Proto, Game.R1_Rayman1, "Rayman 1 (Jaguar - Prototype)", typeof(R1Jaguar_Proto_Manager))]
        RaymanJaguarPrototype,

        [GameMode(MajorEngineVersion.Rayman1_Jaguar, EngineVersion.R1Jaguar_Demo, Game.R1_Rayman1, "Rayman 1 (Jaguar - Demo)", typeof(R1Jaguar_Demo_Manager))]
        RaymanJaguarDemo,

        // SNES

        [GameMode(MajorEngineVersion.SNES, EngineVersion.SNES, Game.SNES_Prototype, "Rayman (SNES - Prototype)", typeof(SNES_Prototype_Manager))]
        RaymanSNES,

        // GBA

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

        [GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_TomClancysRainbowSixRogueSpear, Game.GBA_TomClancysRainbowSixRogueSpear, "Tom Clancy's Rainbow Six: Rogue Spear (GBA - EU)", typeof(GBA_TomClancy_Manager))]
        TomClancysRainbowSixRogueSpearEU,

        [GameMode(MajorEngineVersion.GBA, EngineVersion.GBA_TomClancysRainbowSixRogueSpear, Game.GBA_TomClancysRainbowSixRogueSpear, "Tom Clancy's Rainbow Six: Rogue Spear (GBA - US)", typeof(GBA_TomClancy_Manager))]
        TomClancysRainbowSixRogueSpearUS,

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

        // GBA RRR

        [GameMode(MajorEngineVersion.GBARRR, EngineVersion.GBARRR, Game.GBARRR_RavingRabbids, "Rayman Raving Rabbids (GBA - EU)", typeof(GBA_RRR_Manager))]
        RaymanRavingRabbidsGBAEU,

        [GameMode(MajorEngineVersion.GBARRR, EngineVersion.GBARRR, Game.GBARRR_RavingRabbids, "Rayman Raving Rabbids (GBA - US)", typeof(GBA_RRR_Manager))]
        RaymanRavingRabbidsGBAUS,

        // GBA Isometric

        //[GameMode(MajorEngineVersion.GBAIsometric, EngineVersion.GBAIsometric_Spyro1, Game.GBAIsometric_Spyro1, "Spyro: Season of Ice (GBA - EU)", typeof(GBAIsometric_Spyro2_Manager))]
        //SpyroSeasonIceEU,

        [GameMode(MajorEngineVersion.GBAIsometric, EngineVersion.GBAIsometric_Spyro1, Game.GBAIsometric_Spyro1, "Spyro: Season of Ice (GBA - US)", typeof(GBAIsometric_Spyro1_Manager))]
        SpyroSeasonIceUS,

        [GameMode(MajorEngineVersion.GBAIsometric, EngineVersion.GBAIsometric_Spyro2, Game.GBAIsometric_Spyro2, "Spyro 2: Season of Flame (GBA - EU)", typeof(GBAIsometric_Spyro2EU_Manager))]
        SpyroSeasonFlameEU,

        [GameMode(MajorEngineVersion.GBAIsometric, EngineVersion.GBAIsometric_Spyro2, Game.GBAIsometric_Spyro2, "Spyro 2: Season of Flame (GBA - US)", typeof(GBAIsometric_Spyro2US_Manager))]
        SpyroSeasonFlameUS,

        [GameMode(MajorEngineVersion.GBAIsometric, EngineVersion.GBAIsometric_Spyro3, Game.GBAIsometric_Spyro3, "Spyro Adventure (GBA - EU)", typeof(GBAIsometric_Spyro3EU_Manager))]
        SpyroAdventureEU,

        [GameMode(MajorEngineVersion.GBAIsometric, EngineVersion.GBAIsometric_Spyro3, Game.GBAIsometric_Spyro3, "Spyro: Attack of the Rhynocs (GBA - US)", typeof(GBAIsometric_Spyro3US_Manager))]
        SpyroAdventureUS,

        [GameMode(MajorEngineVersion.GBAIsometric, EngineVersion.GBAIsometric_Tron2, Game.GBAIsometric_Tron2, "TRON 2.0: Killer App (GBA - EU)", typeof(GBAIsometric_Tron2_Manager))]
        Tron2KillerAppEU,

        [GameMode(MajorEngineVersion.GBAIsometric, EngineVersion.GBAIsometric_Tron2, Game.GBAIsometric_Tron2, "TRON 2.0: Killer App (GBA - US)", typeof(GBAIsometric_Tron2_Manager))]
        Tron2KillerAppUS,

        [GameMode(MajorEngineVersion.GBAIsometric, EngineVersion.GBAIsometric_RHR, Game.GBAIsometric_RHR, "Rayman Hoodlum's Revenge (GBA - EU)", typeof(GBAIsometric_RHR_Manager))]
        RaymanHoodlumsRevengeEU,

        [GameMode(MajorEngineVersion.GBAIsometric, EngineVersion.GBAIsometric_RHR, Game.GBAIsometric_RHR, "Rayman Hoodlum's Revenge (GBA - US)", typeof(GBAIsometric_RHR_Manager))]
        RaymanHoodlumsRevengeUS,

        // GBC

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

        // GBA Vicarious Visions

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_Crash1, Game.GBAVV_Crash1, "Crash Bandicoot XS (GBA - EU)", typeof(GBAVV_Crash1_Manager))]
        Crash1GBAEU,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_Crash1, Game.GBAVV_Crash1, "Crash Bandicoot The Huge Adventure (GBA - US)", typeof(GBAVV_Crash1_Manager))]
        Crash1GBAUS,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_Crash1, Game.GBAVV_Crash1, "Crash Bandicoot Advance (GBA - JP)", typeof(GBAVV_Crash1_Manager))]
        Crash1GBAJP,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_Crash2, Game.GBAVV_Crash2, "Crash Bandicoot 2 N-Tranced (GBA - EU)", typeof(GBAVV_Crash2_Manager))]
        Crash2GBAEU,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_Crash2, Game.GBAVV_Crash2, "Crash Bandicoot 2 N-Tranced (GBA - US)", typeof(GBAVV_Crash2_Manager))]
        Crash2GBAUS,

        [GameMode(MajorEngineVersion.GBAVV, EngineVersion.GBAVV_Crash2, Game.GBAVV_Crash2, "Crash Bandicoot Advance 2 (GBA - JP)", typeof(GBAVV_Crash2_Manager))]
        Crash2GBAJP,

        // Gameloft

        [GameMode(MajorEngineVersion.Gameloft, EngineVersion.Gameloft_RRR, Game.Gameloft_RRR, "Rayman Raving Rabbids (Mobile, low res)", typeof(Gameloft_RRR_Manager))]
        RaymanRavingRabbidsMobile_LowRes,

        [GameMode(MajorEngineVersion.Gameloft, EngineVersion.Gameloft_RRR, Game.Gameloft_RRR, "Rayman Raving Rabbids (Mobile, high res)", typeof(Gameloft_RRRHighRes_Manager))]
        RaymanRavingRabbidsMobile_HighRes,

        [GameMode(MajorEngineVersion.Gameloft, EngineVersion.Gameloft_RRR, Game.Gameloft_RRR, "Rayman Raving Rabbids (Mobile, 240x400)", typeof(Gameloft_RRRHighRes_Manager))]
        RaymanRavingRabbidsMobile_240x400,

        [GameMode(MajorEngineVersion.Gameloft, EngineVersion.Gameloft_RK, Game.Gameloft_RK, "Rayman Kart (Mobile, 128x160)", typeof(Gameloft_RK_128x160_Manager))]
        RaymanKartMobile_128x160,

        [GameMode(MajorEngineVersion.Gameloft, EngineVersion.Gameloft_RK, Game.Gameloft_RK, "Rayman Kart (Mobile, 176x208)", typeof(Gameloft_RK_128x160_Manager))]
        RaymanKartMobile_176x208,

        [GameMode(MajorEngineVersion.Gameloft, EngineVersion.Gameloft_RK, Game.Gameloft_RK, "Rayman Kart (Mobile, 240x320)", typeof(Gameloft_RK_Manager))]
        RaymanKartMobile_240x320,

        [GameMode(MajorEngineVersion.Gameloft, EngineVersion.Gameloft_RK, Game.Gameloft_RK, "Rayman Kart (Mobile, 320x240)", typeof(Gameloft_RK_Manager))]
        RaymanKartMobile_320x240,
    }
}