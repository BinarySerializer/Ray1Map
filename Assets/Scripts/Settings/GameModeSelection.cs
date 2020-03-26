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
        [GameMode(EngineVersion.RayPS1, Game.Rayman1, "Rayman 1 (PS1)", typeof(PS1_R1_Manager))]
        RaymanPS1,

        [GameMode(EngineVersion.RayPS1JP, Game.Rayman1, "Rayman 1 (PS1 - Japan)", typeof(PS1_R1JP_Manager))]
        RaymanPS1Japan,

        [GameMode(EngineVersion.RayPC, Game.Rayman1, "Rayman 1 (PC)", typeof(PC_R1_Manager))]
        RaymanPC,

        [GameMode(EngineVersion.RayKit, Game.RaymanDesigner, "Rayman Gold (PC - Demo)", typeof(PC_RD_Manager))]
        RaymanGoldPCDemo,

        [GameMode(EngineVersion.RayKit, Game.RaymanDesigner, "Rayman Designer (PC)", typeof(PC_RD_Manager))]
        RaymanDesignerPC,

        [GameMode(EngineVersion.RayKit, Game.RaymanMapper, "Rayman Mapper (PC)", typeof(PC_Mapper_Manager))]
        MapperPC,

        [GameMode(EngineVersion.RayKit, Game.RaymanByHisFans, "Rayman by his Fans (PC)", typeof(PC_RD_Manager))]
        RaymanByHisFansPC,

        [GameMode(EngineVersion.RayKit, Game.Rayman60Levels, "Rayman 60 Levels (PC)", typeof(PC_RD_Manager))]
        Rayman60LevelsPC,

        [GameMode(EngineVersion.RayEduPC, Game.RaymanEducational, "Rayman Educational (PC)", typeof(PC_EDU_Manager))]
        RaymanEducationalPC,

        [GameMode(EngineVersion.RayEduPC, Game.RaymanQuiz, "Rayman Quiz (PC)", typeof(PC_EDU_Manager))]
        RaymanQuizPC,

        [GameMode(EngineVersion.RayPocketPC, Game.Rayman1, "Rayman Ultimate (Pocket PC)", typeof(PocketPC_R1_Manager))]
        RaymanPocketPC,

        [GameMode(EngineVersion.RayPocketPC, Game.Rayman1, "Rayman Classic (Mobile)", typeof(Mobile_R1_Manager))]
        RaymanClassicMobile,
    }
}