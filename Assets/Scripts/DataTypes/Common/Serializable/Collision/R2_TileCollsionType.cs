namespace R1Engine
{
    public enum R2_TileCollsionType : byte
    {
        None = 0,

        // It appears that the reactionary tiles work in a way of changing a moving platform's position in directions like "up-left", like how the UFO works in R1
        Reactionary0 = 1,
        Reactionary1 = 2,
        Reactionary2 = 3,
        Reactionary3 = 4,
        Reactionary4 = 5,

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

        // Used for enemy movements
        ReactionaryEnemy = 47,
        
        ReactionaryUnk = 49,
    }
}