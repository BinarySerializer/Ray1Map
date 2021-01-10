namespace R1Engine
{
    // Note: The game actually has two bytes for the collision, to determine the shape and type. But since ray1map works with a type index we have to do this
    public enum GBACrash_Crash2_CollisionType
    {
        None = 0,
        Hill_Slight_Left_1 = 3,

        Hill_Slight_Left_2 = 5,

        Hill_Slight_Right_2 = 6,
        Hill_Slight_Right_1 = 7,
        Solid = 43,

        //Lava = 256,

        //HangMove = 2048,
    }
}