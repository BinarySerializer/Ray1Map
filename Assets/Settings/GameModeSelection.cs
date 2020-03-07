namespace R1Engine
{
    /// <summary>
    /// The available game modes to select from
    /// </summary>
    public enum GameModeSelection
    {
        [GameMode(GameMode.RayPS1, "Rayman 1 (PS1)", typeof(PS1_R1_Manager))]
        RaymanPS1,

        [GameMode(GameMode.RayPC, "Rayman 1 (PC)", typeof(PC_R1_Manager))]
        RaymanPC,

        [GameMode(GameMode.RayKit, "Rayman Designer (PC)", typeof(PC_RD_Manager))]
        RaymanDesignerPC,

        [GameMode(GameMode.RayKit, "Rayman by his Fans (PC)", typeof(PC_RD_Manager))]
        RaymanByHisFansPC,

        [GameMode(GameMode.RayKit, "Rayman 60 Levels (PC)", typeof(PC_RD_Manager))]
        Rayman60LevelsPC,
    }
}