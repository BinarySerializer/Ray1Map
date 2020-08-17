namespace R1Engine
{
    /// <summary>
    /// The collision types for GBA
    /// </summary>
    public enum GBA_TileCollisionType : sbyte
    {
        Empty = -1,

        Solid = 0,

        // Indicates that you can hang off of the ledge
        Ledge = 2,

        Solid2 = 15,
        
        Hill_Slight_Left_1 = 18,
        Hill_Slight_Left_2 = 19,
        Hill_Slight_Right_2 = 20,
        Hill_Slight_Right_1 = 21,

        InstaKill = 32,
        Damage = 33,

        // 34 and 35 might be enemy borders
        // 36 might be vertical camera lock
        // 38 and 39 might be horizontal camera locks

        // Acts as triggers for moving platforms
        Reactionary_Left = 38,
        Reactionary_Right = 39,
        Reactionary_Up = 40,
        Reactionary_Down = 41,

        Reactionary_DownLeft = 42,
        Reactionary_DownRight = 43,
        Reactionary_UpRight = 44,
        Reactionary_UpLeft = 45,

        Hang = 46,
        Climb = 47,

        Water = 48,
        
        ClimbableWalls = 49,

        ShellLoop = 50,

        Lava = 74,

        // 81, 86 and 87 seems to change speed of moving platform

        InstaKill2 = 90,
    }
}