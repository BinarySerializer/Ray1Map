namespace R1Engine
{
    public enum R2_TileCollisionType : byte
    {
        None = 0,

        Direction_Left = 1,
        Direction_Right = 2,
        Direction_Up = 3,
        Direction_Down = 4,

        Direction_UpLeft = 5,
        Direction_UpRight = 6, // Verify this
        Direction_DownLeft = 7, // Verify this
        Direction_DownRight = 8,

        Unknown_11 = 11,
        Unknown_14 = 14,

        Cliff = 18,
        Water = 19,

        Solid = 22,
        Passthrough = 23,

        Hill_Slight_Left_1 = 25,
        Hill_Slight_Left_2 = 26,
        Hill_Steep_Left = 27,

        Hill_Slight_Right_2 = 28,
        Hill_Slight_Right_1 = 29,
        Hill_Steep_Right = 30,

        ReactionaryEnemy = 47, // Used for enemy movements

        ReactionaryUnk = 49, // Appears to change the direction of a moving platform after a certain amount of hits

        ValidTarget = 50,
        InvalidTarget = 51,
    }
}