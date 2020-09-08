namespace R1Engine
{
    /// <summary>
    /// The event types for Rayman 2
    /// </summary>
    public enum R1_R2EventType : ushort
    {
        None = 0,
        Rayman = 1,
        Unk_2 = 2,

        Oneup = 3,

        BigPower = 4,

        Flash = 5,

        Unk_6 = 6,

        HitEffect = 8,

        FloatingMine = 9,

        Explosion = 10,

        FistReflector = 11,

        MetalThing1 = 13,

        WaterLily = 18,
        FlyingRing = 19,
        Teleporter = 24,
        ExitSign = 27,
        ScaredPlatform = 34,
        TrapCube = 36,
        Trampoline = 38,

        Unk_48 = 48,

        RaymanPosition = 49,

        Bolts = 51,

        Potion = 52,
        RotatingCube = 56,

        WaterSplash = 76,

        // NOTE: This type is also used for an always event!
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

        SmackBX003Thing = 100,

        Unk_102 = 102,

        GunshotExplosion = 103,

        WaterFall = 104,
    }
}