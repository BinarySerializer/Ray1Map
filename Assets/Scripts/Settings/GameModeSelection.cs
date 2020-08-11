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
        [GameMode(MajorEngineVersion.PS1, EngineVersion.RayPS1, Game.Rayman1, "Rayman 1 (PS1 - US)", typeof(PS1_R1_Manager))]
        RaymanPS1US,

        [GameMode(MajorEngineVersion.PS1, EngineVersion.RayPS1, Game.Rayman1, "Rayman 1 (PS1 - EU)", typeof(PS1_R1_Manager))]
        RaymanPS1EU,

        [GameMode(MajorEngineVersion.PS1, EngineVersion.RayPS1, Game.Rayman1, "Rayman 1 (PS1 - EU Demo)", typeof(PS1_R1EUDemo_Manager))]
        RaymanPS1EUDemo,

        [GameMode(MajorEngineVersion.PS1, EngineVersion.RayPS1JP, Game.Rayman1, "Rayman 1 (PS1 - JP)", typeof(PS1_R1JP_Manager))]
        RaymanPS1Japan,

        [GameMode(MajorEngineVersion.PS1, EngineVersion.RayPS1JPDemoVol3, Game.Rayman1DemoPrototype, "Rayman 1 (PS1 - JP Demo Vol3)", typeof(PS1_R1JPDemoVol3_Manager))]
        RaymanPS1DemoVol3Japan,

        [GameMode(MajorEngineVersion.PS1, EngineVersion.RayPS1JPDemoVol6, Game.Rayman1DemoPrototype, "Rayman 1 (PS1 - JP Demo Vol6)", typeof(PS1_R1JPDemoVol6_Manager))]
        RaymanPS1DemoVol6Japan,

        [GameMode(MajorEngineVersion.PS1, EngineVersion.RaySaturn, Game.Rayman1, "Rayman 1 (Saturn - US)", typeof(Satun_R1_Manager))]
        RaymanSaturnUS,

        [GameMode(MajorEngineVersion.PS1, EngineVersion.RaySaturn, Game.Rayman1, "Rayman 1 (Saturn - Prototype)", typeof(Satun_R1_Manager))]
        RaymanSaturnProto,

        [GameMode(MajorEngineVersion.PS1, EngineVersion.RaySaturn, Game.Rayman1, "Rayman 1 (Saturn - EU)", typeof(Satun_R1_Manager))]
        RaymanSaturnEU,

        [GameMode(MajorEngineVersion.PS1, EngineVersion.RaySaturn, Game.Rayman1, "Rayman 1 (Saturn - JP)", typeof(Satun_R1_Manager))]
        RaymanSaturnJP,

        [GameMode(MajorEngineVersion.PC, EngineVersion.RayPC, Game.Rayman1, "Rayman 1 (PC - 1.00)", typeof(PC_R1_Manager))]
        RaymanPC_1_00,

        [GameMode(MajorEngineVersion.PC, EngineVersion.RayPC, Game.Rayman1, "Rayman 1 (PC - 1.10)", typeof(PC_R1_Manager))]
        RaymanPC_1_10,

        [GameMode(MajorEngineVersion.PC, EngineVersion.RayPC, Game.Rayman1, "Rayman 1 (PC - 1.12)", typeof(PC_R1_Manager))]
        RaymanPC_1_12,

        [GameMode(MajorEngineVersion.PC, EngineVersion.RayPC, Game.Rayman1, "Rayman 1 (PC - 1.20)", typeof(PC_R1_Manager))]
        RaymanPC_1_20,

        [GameMode(MajorEngineVersion.PC, EngineVersion.RayPC, Game.Rayman1, "Rayman 1 (PC - 1.21 JP)", typeof(PC_R1_Manager))]
        RaymanPC_1_21_JP,

        [GameMode(MajorEngineVersion.PC, EngineVersion.RayPC, Game.Rayman1, "Rayman 1 (PC - 1.21)", typeof(PC_R1_Manager))]
        RaymanPC_1_21,

        [GameMode(MajorEngineVersion.PC, EngineVersion.RayKitPC, Game.RaymanDesigner, "Rayman Gold (PC - Demo)", typeof(PC_RD_Manager))]
        RaymanGoldPCDemo,

        [GameMode(MajorEngineVersion.PC, EngineVersion.RayKitPC, Game.RaymanDesigner, "Rayman Designer (PC)", typeof(PC_RD_Manager))]
        RaymanDesignerPC,

        [GameMode(MajorEngineVersion.PC, EngineVersion.RayKitPC, Game.RaymanMapper, "Rayman Mapper (PC)", typeof(PC_Mapper_Manager))]
        MapperPC,

        [GameMode(MajorEngineVersion.PC, EngineVersion.RayKitPC, Game.RaymanByHisFans, "Rayman by his Fans (PC)", typeof(PC_RD_Manager))]
        RaymanByHisFansPC,

        [GameMode(MajorEngineVersion.PC, EngineVersion.RayKitPC, Game.Rayman60Levels, "Rayman 60 Levels (PC)", typeof(PC_RD_Manager))]
        Rayman60LevelsPC,

        [GameMode(MajorEngineVersion.PC, EngineVersion.RayEduPC, Game.RaymanEducational, "Rayman Educational (PC)", typeof(PC_EDU_Manager))]
        RaymanEducationalPC,

        [GameMode(MajorEngineVersion.PC, EngineVersion.RayEduPS1, Game.RaymanEducational, "Rayman Educational (PS1)", typeof(PS1_EDU_Manager))]
        RaymanEducationalPS1,

        [GameMode(MajorEngineVersion.PC, EngineVersion.RayEduPC, Game.RaymanQuiz, "Rayman Quiz (PC)", typeof(PC_EDU_Manager))]
        RaymanQuizPC,

        [GameMode(MajorEngineVersion.PC, EngineVersion.RayPocketPC, Game.Rayman1, "Rayman Ultimate (Pocket PC)", typeof(PocketPC_R1_Manager))]
        RaymanPocketPC,

        [GameMode(MajorEngineVersion.PC, EngineVersion.RayPocketPC, Game.Rayman1, "Rayman Classic (Mobile)", typeof(Mobile_R1_Manager))]
        RaymanClassicMobile,

        [GameMode(MajorEngineVersion.Jaguar, EngineVersion.RayJaguar, Game.Rayman1, "Rayman 1 (Jaguar)", typeof(Jaguar_R1_Manager))]
        RaymanJaguar,

        [GameMode(MajorEngineVersion.Jaguar, EngineVersion.RayJaguarProto, Game.Rayman1, "Rayman 1 (Jaguar - Prototype)", typeof(Jaguar_R1Proto_Manager))]
        RaymanJaguarPrototype,

        [GameMode(MajorEngineVersion.Jaguar, EngineVersion.RayJaguarDemo, Game.Rayman1, "Rayman 1 (Jaguar - Demo)", typeof(Jaguar_R1Demo_Manager))]
        RaymanJaguarDemo,

        [GameMode(MajorEngineVersion.PC, EngineVersion.RayGBA, Game.Rayman1, "Rayman Advance (GBA - EU)", typeof(GBA_R1_Manager))]
        RaymanAdvanceGBAEU,

        [GameMode(MajorEngineVersion.PC, EngineVersion.RayGBA, Game.Rayman1, "Rayman Advance (GBA - US)", typeof(GBA_R1_Manager))]
        RaymanAdvanceGBAUS,

        [GameMode(MajorEngineVersion.PC, EngineVersion.RayGBA, Game.Rayman1, "Rayman Advance (GBA - EU Beta)", typeof(GBA_R1_Manager))]
        RaymanAdvanceGBAEUBeta,

        [GameMode(MajorEngineVersion.PC, EngineVersion.RayDSi, Game.Rayman1, "Rayman 1 (DSi)", typeof(DSi_R1_Manager))]
        RaymanDSi,

        [GameMode(MajorEngineVersion.PS1, EngineVersion.Ray2PS1, Game.Rayman2, "Rayman 2 (PS1 - Demo)", typeof(PS1_R2Demo_Manager))]
        Rayman2PS1Demo,

        [GameMode(MajorEngineVersion.SNES, EngineVersion.RaySNES, Game.RaymanSNESPrototype, "Rayman (SNES - Prototype)", typeof(SNES_Prototype_Manager))]
        RaymanSNES,

        [GameMode(MajorEngineVersion.GBA, EngineVersion.Ray3GBA, Game.Rayman3GBA, "Rayman 3 (GBA - EU)", typeof(GBA_R3_Manager))]
        Rayman3GBAEU,

        [GameMode(MajorEngineVersion.GBA, EngineVersion.Ray3GBA, Game.Rayman3GBA, "Rayman 3 (GBA - US)", typeof(GBA_R3_Manager))]
        Rayman3GBAUS,

        [GameMode(MajorEngineVersion.GBA, EngineVersion.Ray3GBA, Game.Rayman3GBA, "Rayman 3 (N-Gage)", typeof(NGage_R3_Manager))]
        Rayman3NGage,

        //[GameMode(MajorEngineVersion.GBA, EngineVersion.PrinceOfPersiaGBA, Game.PrinceOfPersiaTheSandsOfTimeGBA, "Prince of Persia: The Sands of Time (GBA - EU)", typeof(GBA_PoPSoT_Manager))]
        //PrinceOfPersiaGBAEU,

        [GameMode(MajorEngineVersion.GBA, EngineVersion.PrinceOfPersiaGBA, Game.PrinceOfPersiaTheSandsOfTimeGBA, "Prince of Persia: The Sands of Time (GBA - US)", typeof(GBA_PoPSoT_Manager))]
        PrinceOfPersiaGBAUS,

        //[GameMode(MajorEngineVersion.GBA, EngineVersion.SabrinaGBA, Game.SabrinaTheTeenageWitchPotionCommotionGBA, "Sabrina - The Teenage Witch - Potion Commotion (GBA - EU)", typeof(GBA_Sabrina_Manager))]
        //SabrinaTheTeenageWitchPotionCommotionGBAEU,

        [GameMode(MajorEngineVersion.GBA, EngineVersion.SabrinaGBA, Game.SabrinaTheTeenageWitchPotionCommotionGBA, "Sabrina - The Teenage Witch - Potion Commotion (GBA - US)", typeof(GBA_Sabrina_Manager))]
        SabrinaTheTeenageWitchPotionCommotionGBAUS,

        //[GameMode(MajorEngineVersion.GBA, EngineVersion.SabrinaGBA, Game.SabrinaTheTeenageWitchPotionCommotionGBA, "Star Wars Trilogy - Apprentice of the Force (GBA - EU)", typeof(GBA_StarWarsApprentice_Manager))]
        //StarWarsTrilogyApprenticeOfTheForceGBAEU,

        [GameMode(MajorEngineVersion.GBA, EngineVersion.SabrinaGBA, Game.SabrinaTheTeenageWitchPotionCommotionGBA, "Star Wars Trilogy - Apprentice of the Force (GBA - US)", typeof(GBA_StarWarsApprentice_Manager))]
        StarWarsTrilogyApprenticeOfTheForceGBAUS,
    }
}