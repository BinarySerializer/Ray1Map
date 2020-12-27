namespace R1Engine
{
    /// <summary>
    /// The event types for Rayman 2
    /// </summary>
    public enum R1_R2EventType : ushort
    {
        Invalid = 0, // Slots for always events
        Rayman = 1,
        Fist = 2,
        Oneup = 3,
        BigPower = 4,
        Flash = 5,
        Gendoor = 6,
        Killdoor = 7, // Unused
        HitEffect = 8,
        FloatingMine = 9,
        Explosion = 10,
        FistReflector = 11,

        MetalPart = 13,

        WaterLily = 18,
        FlyingRing = 19,

        Teleporter = 24,

        ExitSign = 27,

        ScaredPlatform = 34,

        TrapCube = 36,

        Trampoline = 38,

        Trigger = 48,
        RaymanPosition = 49,

        Bolts = 51,
        Potion = 52,

        RotatingCube = 56,

        WaterSplash = 76,

        Cannon = 81,
        CannonTarget = 82,
        Cannonball = 83,

        Enemy = 91,
        MovingGunshot = 92,

        DestructableGround = 95,
        SmackBX003 = 96,
        Ting = 97,
        Dino = 98,
        DinoBreath = 99,
        SmackBX003Part = 100,

        Unk_102 = 102,
        GunshotExplosion = 103,
        WaterFall = 104

        // No more event types after this are defined in the exe
    }
}