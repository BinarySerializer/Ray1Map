namespace R1Engine
{
    /// <summary>
    /// The collision types for GBA
    /// </summary>
    public enum GBA_TileCollisionType : byte
    {
        Solid = 0x00,

        // Indicates that you can hang off of the ledge
        Ledge = 0x02,
        
        Hill_Slight_Left_1 = 0x12,
        Hill_Slight_Left_2 = 0x13,
        Hill_Slight_Right_2 = 0x14,
        Hill_Slight_Right_1 = 0x15,

        // 38 and 39 might be camera locks

        // Acts as triggers for moving platforms
        Reactionary_Up = 0x28,
        Reactionary_Down = 0x29,
        
        Hang = 0x2E,
        Climb = 0x2F,
        
        Water = 0x30,
        
        Empty = 0xFF
    }
}