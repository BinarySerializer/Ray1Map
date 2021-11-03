namespace Ray1Map.GBC
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

        Climb = 16,

        Damage = 17,
        InstaKill = 18,
        Pit = 19,

        Trigger_Right = 20,
        Trigger_Left = 21,
        Trigger_Up = 22,
        Trigger_Down = 23,

        Trigger_UpRight = 24,
        Trigger_UpLeft = 25,
        Trigger_DownRight = 26,
        Trigger_DownLeft = 27,

        Water = 29,
        Climb_Full = 30
    }
}