using System.ComponentModel;

namespace R1Engine
{
    /// <summary>
    /// The available game modes
    /// </summary>
    public enum GameMode 
    {
        [Description("Rayman 1 (PS1)")]
        RaymanPS1,

        //[Description("Rayman 1 (PS1 - Japanese)")]
        //RaymanPS1JP,

        [Description("Rayman 1 (PC)")]
        RaymanPC,

        //[Description("Rayman Designer (PC)")]
        //RaymanDesignerPC,

        //[Description("Rayman by his Fans (PC)")]
        //RaymanByHisFansPC,

        //[Description("Rayman 60 Levels (PC)")]
        //Rayman60LevelsPC,
    }
}