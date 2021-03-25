using System;

namespace R1Engine
{
    [Flags]
    public enum GBAVV_NitroKart_NGage_TriangleFlags : ushort
    {
        None = 0,
        Wall = 1 << 0,
        NotSolid = 1 << 1,
        
        TriggerArea = 1 << 2, // Used for hub world portal areas and water
        Flag_03 = 1 << 3, // Used for some walls, a bridge, animated fire, animated electric gate
        
        MoveSlow = 1 << 4,
        HorizontalScroll = 1 << 5,
        
        Flag_06 = 1 << 6, // Used for some decorations
        
        Water = 1 << 7,
        Pit = 1 << 8,
        
        Flag_09 = 1 << 9, // Unused
        
        Jump = 1 << 10,
        LongerJump = 1 << 11,
        Teleporter = 1 << 12,
        
        ElectricGate = 1 << 13,
        Flag_14 = 1 << 14, // Used for some decorations
        Flag_15 = 1 << 15, // Unused
    }
}