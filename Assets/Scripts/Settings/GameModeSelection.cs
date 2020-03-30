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
        [GameMode(EngineVersion.RayPS1, Game.Rayman1, "Rayman 1 (PS1 - US)", typeof(PS1_R1_Manager))]
        RaymanPS1US,

        //[GameMode(EngineVersion.RayPS1, Game.Rayman1, "Rayman 1 (PS1 - EU)", typeof(PS1_R1_Manager))]
        //RaymanPS1EU,

        //[GameMode(EngineVersion.RayPS1, Game.Rayman1, "Rayman 1 (PS1 - EU Demo)", typeof(PS1_R1_Manager))]
        //RaymanPS1EUDemo,

        [GameMode(EngineVersion.RayPS1JP, Game.Rayman1, "Rayman 1 (PS1 - JP)", typeof(PS1_R1JP_Manager))]
        RaymanPS1Japan,

        [GameMode(EngineVersion.RayPS1JPDemo, Game.Rayman1DemoPrototype, "Rayman 1 (PS1 - JP Demo Vol3)", typeof(PS1_R1JPDemoVol3_Manager))]
        RaymanPS1DemoVol3Japan,

        //[GameMode(EngineVersion.RayPS1JPDemo, Game.Rayman1DemoPrototype, "Rayman 1 (PS1 - JP Demo Vol6)", typeof(PS1_R1JPDemoVol3_Manager))]
        //RaymanPS1DemoVol6Japan,

        [GameMode(EngineVersion.RayPC, Game.Rayman1, "Rayman 1 (PC)", typeof(PC_R1_Manager))]
        RaymanPC,

        [GameMode(EngineVersion.RayKitPC, Game.RaymanDesigner, "Rayman Gold (PC - Demo)", typeof(PC_RD_Manager))]
        RaymanGoldPCDemo,

        [GameMode(EngineVersion.RayKitPC, Game.RaymanDesigner, "Rayman Designer (PC)", typeof(PC_RD_Manager))]
        RaymanDesignerPC,

        [GameMode(EngineVersion.RayKitPC, Game.RaymanMapper, "Rayman Mapper (PC)", typeof(PC_Mapper_Manager))]
        MapperPC,

        [GameMode(EngineVersion.RayKitPC, Game.RaymanByHisFans, "Rayman by his Fans (PC)", typeof(PC_RD_Manager))]
        RaymanByHisFansPC,

        [GameMode(EngineVersion.RayKitPC, Game.Rayman60Levels, "Rayman 60 Levels (PC)", typeof(PC_RD_Manager))]
        Rayman60LevelsPC,

        [GameMode(EngineVersion.RayEduPC, Game.RaymanEducational, "Rayman Educational (PC)", typeof(PC_EDU_Manager))]
        RaymanEducationalPC,

        [GameMode(EngineVersion.RayEduPC, Game.RaymanQuiz, "Rayman Quiz (PC)", typeof(PC_EDU_Manager))]
        RaymanQuizPC,

        [GameMode(EngineVersion.RayPocketPC, Game.Rayman1, "Rayman Ultimate (Pocket PC)", typeof(PocketPC_R1_Manager))]
        RaymanPocketPC,

        [GameMode(EngineVersion.RayPocketPC, Game.Rayman1, "Rayman Classic (Mobile)", typeof(Mobile_R1_Manager))]
        RaymanClassicMobile,

        [GameMode(EngineVersion.RayPS1, Game.Rayman2, "Rayman 2 (PS1 - Demo)", typeof(PS1_R2Demo_Manager))]
        Rayman2PS1Demo,
    }
}