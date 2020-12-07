namespace R1Engine
{
    public enum GBARRR_Mode7TileCollisionType : byte
    {
        Empty = 0,
        Solid = 1,
        Speed = 2,
        Bounce = 3,
        Damage = 4,
        Slippery = 5,
        SlowDown = 7,
        FinishLine1 = 8,
        FinishLine2 = 9,
        FinishLine3 = 10,
    }
}