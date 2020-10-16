namespace R1Engine
{
    public enum GBARRR_ActorType : byte
    {
        Special = 0,
        Collectible = 1, // Lum, life

        DoorTrigger = 3,
        MovingPlatform1 = 4,
        Keg = 5,

        RabbidGuard = 7,
        Scenery1 = 8,
        WoodenFloor = 9,
        MetalFoor = 10,
        Switch = 11,
        Gate = 12,
        MovingPlatform2 = 13,
        MovingPlatform3 = 14,
        MovingPlatform4 = 15,
        BlockingDoor1 = 16,
        BreakableDoor = 17,
        MovingMachine = 18,
        PrisonBoss = 19,
        Platform = 20,
        Enemy1 = 21, // Pen, gray rabbid etc.
        Scenery2 = 22,
        BouncyPlatform = 23,
        Ring = 24,
        FloatingObject1 = 25,
        DroppingObject = 26,
        ThrowableObject = 27,
        Scenery3 = 28,
        MurfyTrigger = 29,

        SizeTrigger = 31,
        Cage1 = 32,
        Cage2 = 33,
        Friend = 34, // Ly, Murfy
        EnemyBookVertical = 35,
        EnemyBookHorizontal = 36,
        Hunter = 37,
        StompablePencil = 38,
        FlyingRabbid = 39,
        ShootingTarget = 40,
        BlockingDoor2 = 41,
        TimedSwitch = 42,
        SwitchPunch = 44,
        SwitchStand = 43,
        DrippingLiquid = 45,
        KnockableObject = 46,
        Scenery4 = 47,
        FloatingObject2 = 48,
        WorldPortal = 49,

        MechanicalEnemy = 51,
        SuperheroRabbid = 52,
        EnemyBackpack = 53,
        EnemyLarva = 54,

        Livingstone = 56,
        UnusedRabbid = 57,
        Enemy2 = 58,
        EnemyMuffin = 59,
        RabbidRobot = 60,
        EnemyPiranha = 61,
        EnemyFish = 62,
        Boss = 63,
        DancingRabbid = 64,
        Enemy3 = 65,
        EnemyBread = 66,
        Enemy4 = 67,
        EnemyChef = 68,

        Stump = 70,
        MinigameRabbid = 71,
        FinalBossPart = 72
    }
}