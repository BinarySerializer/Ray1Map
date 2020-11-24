namespace R1Engine
{
    public enum GBC_TileCollisionType : byte
    {
        Empty = 0,
        Solid = 1,
        Passthrough = 2,
        Slippery = 3,

        Steep_Left = 4,
        Steep_Right = 5,

        Hill_Left1 = 6,
        Hill_Left2 = 7,
        Hill_Right2 = 8,
        Hill_Right1 = 9,

        Slippery_Steep_Left = 10,
        Slippery_Steep_Right = 11,

        Slippery_Hill_Left1 = 12,
        Slippery_Hill_Left2 = 13,
        Slippery_Hill_Right2 = 14,
        Slippery_Hill_Right1 = 15,

        Damage = 17,
        InstaKill = 18,
        Pit = 19,

        Trigger_Right = 20,
        Trigger_Left = 21,
        Trigger_Up = 22,
        Trigger_Down = 23,

        Water = 29
    }
}