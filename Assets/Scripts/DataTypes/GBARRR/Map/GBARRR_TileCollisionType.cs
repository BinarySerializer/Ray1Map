namespace R1Engine
{
    public enum GBARRR_TileCollisionType : byte
    {
        Empty = 0,

        Solid = 2,
        Climb = 3,
        Hang = 4,
        ClimbableWalls = 5,

        Damage = 7,

        Solid_Left_1 = 23,
        Solid_Left_2 = 24,
        Solid_Right_2 = 25,
        Solid_Right_1 = 26,
        Solid_Left = 27,
        Solid_Right = 28,
        InstaKill = 29
    }
}