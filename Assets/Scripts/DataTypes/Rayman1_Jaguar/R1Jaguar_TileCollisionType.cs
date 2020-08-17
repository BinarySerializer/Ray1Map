namespace R1Engine
{
    /// <summary>
    /// The tile collision types for Rayman 1 on Jaguar
    /// </summary>
    public enum R1Jaguar_TileCollisionType : byte
    {
        None = 0,
        Reactionary = 1,
        Hill_Steep_Left = 2,
        Hill_Steep_Right = 3,
        Hill_Slight_Left_1 = 4,
        Hill_Slight_Left_2 = 5,
        Hill_Slight_Right_2 = 6,
        Hill_Slight_Right_1 = 7,
        Damage = 8,
        Bounce = 9,
        Water = 10,
        Spikes = 11,
        Climb = 12,
        PassthroughProto = 13,
        Passthrough = 14,
        Solid = 15,
    }
}