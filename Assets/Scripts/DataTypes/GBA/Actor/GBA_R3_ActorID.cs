namespace R1Engine
{
    // Event types have to be ushorts right now even though this is a byte
    public enum GBA_R3_ActorID : ushort
    {
        Rayman = 0,
        RaymanBody = 1,
        Pirate_Red = 2,
        Piranha = 3,
        WaterSplash = 4,
        Explosion = 5,

        Platform_Bouncy = 7,
        Platform_Moving = 8,
        Platform_Falling = 9,
        Switch = 10,
        LockedDoor = 11,
        Lum = 12,
        Cage = 13,
        LevelCurtain = 14,

        Keg = 18,
        FloatingBarrel = 19,
        SphereBase = 20,
        Sphere = 21,

        Pirate_Green = 23,
        Pirate_Blue = 24,
        Pirate_Unknown = 25,
        HelicopterBomb = 26,
        ZombieChicken = 27,

        FloatingBarrelSplash = 29,
        LevelExit_Back = 30,

        Rayman_WaterSki = 33,
        Lum_Mode7 = 34,
        Caterpillar = 35,
        Butterfly = 37,

        Boss_Jano = 39,

        Spider = 41,
        Ssssam = 42,

        Ring = 47,

        Effect_49 = 49,
        Breakable_Ground = 50,

        Platform_Moving_Burn = 53,
        Plum = 54,

        DarkLum = 57,

        SpikyBall_Hovering = 60,

        HelicopterBomb_Mode7 = 62,

        Murfy = 64,

        Effect_67 = 67,

        Shell_Walking = 76,

        Ly = 78,

        Breakable_Wall = 81,
        FallingBarrelEffect = 82,

        Boss_Machine = 84,

        balloon = 85,

        Rayman_FlyingShell = 88,
        MovingSkull = 89,

        SpikyBall_Swinging = 91,
        MurfyStone = 92,
        Boss_Grolgoth = 93,

        Teensies = 95,

        Platform_UpOnPunch = 99,
        Urchin = 100,
        LevelExit_Next = 101,

        Rayman_WorldMap = 103,

        Effect_JanoShot = 109,
    }
}