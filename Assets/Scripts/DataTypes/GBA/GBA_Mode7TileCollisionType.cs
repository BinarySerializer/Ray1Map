namespace R1Engine
{
    public enum GBA_Mode7TileCollisionType : sbyte
    {
        Empty = -1,

        Solid = 0,

        Damage = 33,

        // Track direction
        EnemyTrigger_Left = 34,
        EnemyTrigger_Right = 35,
        EnemyTrigger_Up = 36,
        EnemyTrigger_Down = 37,
        Direction_Left = 38,
        Direction_Right = 39,
        Direction_Up = 40,
        Direction_Down = 41,
        Direction_DownLeft = 42,
        Direction_DownRight = 43,
        Direction_UpRight = 44,
        Direction_UpLeft = 45,

        Bounce = 46,
        Bumper1 = 47,
        Bumper2 = 48,

        Damage_FinishLine = 49,
        FinishLine = 50,

        EnemyTrigger_UpLeft = 59,
        EnemyTrigger_DownLeft = 60,
        EnemyTrigger_DownRight = 61,
        EnemyTrigger_UpRight = 62,
    }
}