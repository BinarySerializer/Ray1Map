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
        
        Hill_Slight_Left_1 = 18,
        Hill_Slight_Left_2 = 19,
        Hill_Slight_Right_2 = 20,
        Hill_Slight_Right_1 = 21,

        InstaKill = 32,

        // 35 and 36 might be vertical camera locks
        // 38 and 39 might be horizontal camera locks

        // Acts as triggers for moving platforms
        Reactionary_Up = 40,
        Reactionary_Down = 41,

        Hang = 46,
        Climb = 47,

        Water = 48,
        
        ClimbableWalls = 49,

        Lava = 74,
    }
}