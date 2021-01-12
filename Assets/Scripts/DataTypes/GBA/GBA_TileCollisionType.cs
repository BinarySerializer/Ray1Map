namespace R1Engine
{
    /// <summary>
    /// The collision types for GBA
    /// </summary>
    public enum GBA_TileCollisionType : byte
    {
        Empty = 0xFF,

        Solid = 0,

        // Slippery solid platform
        Slippery = 1,

        // Indicates that you can hang off of the ledge
        Ledge = 2,

        // At edges of slippery platforms
        Slippery_Ledge = 3,

        // Seems to be tiles which are only solid when you stand on them?
        Passthrough = 15,
        
        // Solid angles, like in R1
        Solid_Left_1 = 18,
        Solid_Left_2 = 19,
        Solid_Right_2 = 20,
        Solid_Right_1 = 21,

        // Slippery angles
        Slippery_Left_1 = 22,
        Slippery_Left_2 = 23,
        Slippery_Right_2 = 24,
        Slippery_Right_1 = 25,

        // Instantly kills you
        InstaKill = 32,

        // Mainly used in the sanctuary levels for spikes on the walls
        Damage = 33,

        // Acts as triggers for enemies
        EnemyTrigger_Left = 34,
        EnemyTrigger_Right = 35,
        EnemyTrigger_Up = 36,
        EnemyTrigger_Down = 37,

        // Acts as triggers for moving platforms
        Direction_Left = 38,
        Direction_Right = 39,
        Direction_Up = 40,
        Direction_Down = 41,
        Direction_DownLeft = 42,
        Direction_DownRight = 43,
        Direction_UpRight = 44,
        Direction_UpLeft = 45,

        // Hang from top
        Hang = 46,

        // Climb in all 4 directions
        Climb = 47,

        // Normal water, appears in early levels
        Water = 48,
        
        ClimbableWalls = 49,

        // Makes you jump - used on the bottom of slippery surfaces and for the shell to start a loop
        AutoJump = 50,

        // Same as Climb, but spiders can move on it too
        Climb_Spider_51 = 51,
        Climb_Spider_52 = 52,
        Climb_Spider_53 = 53,
        Climb_Spider_54 = 54,

        // Plums can move on this
        Lava = 74,

        // See Wicked Flow
        Reactionary_Turn_45CounterClockwise = 81,
        Reactionary_Turn_90CounterClockwise = 82,
        Reactionary_Turn_90Clockwise = 86,
        Reactionary_Turn_45Clockwise = 87,

        // Endless pits
        Cliff = 90,
    }
}