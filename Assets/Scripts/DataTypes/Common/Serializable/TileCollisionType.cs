namespace R1Engine
{
    /// <summary>
    /// The tile collision types
    /// </summary>
    public enum TileCollisionType : byte
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
        Exit = 11,
        Climb = 12,
        WaterNoSplash = 13,
        Passthrough = 14,
        Solid = 15,
        Seed = 16,
        Slippery_Steep_Left = 18,
        Slippery_Steep_Right = 19,
        Slippery_Slight_Left_1 = 20,
        Slippery_Slight_Left_2 = 21,
        Slippery_Slight_Right_2 = 22,
        Slippery_Slight_Right_1 = 23,
        Spikes = 24,
        Cliff = 25,
        Slippery = 30
    }

    /*
    
    Rayman 2 collision (incomplete):

    Reactionary = 1,
    Reactionary = 2,
    Reactionary = 3,
    Reactionary = 4,
    Solid = 22,
    Passthrough = 23,
    Hill_Slight_Left_1 = 25, (?)
    Hill_Slight_Right_1 = 30, (?)
     */
}