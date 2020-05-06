namespace R1Engine
{
    /// <summary>
    /// The event types for Rayman 2
    /// </summary>
    public enum PS1_R2Demo_EventType : ushort
    {
        Unk_0 = 0,
        Unk_2 = 2,

        Oneup = 3,

        // NOTE: This type is also used for an always event!
        BigPower = 4,

        [EventTypeInfo(EventFlag.Always)]
        Flash = 5,

        Unk_6 = 6,

        [EventTypeInfo(EventFlag.Always)]
        HitEffect = 8,

        FloatingMine = 9,

        [EventTypeInfo(EventFlag.Always)]
        Explosion = 10,

        FistReflector = 11,

        [EventTypeInfo(EventFlag.Always)]
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

        [EventTypeInfo(EventFlag.Always)]
        Bolts = 51,

        Potion = 52,
        RotatingCube = 56,

        [EventTypeInfo(EventFlag.Always)]
        WaterSplash = 76,

        // NOTE: This type is also used for an always event!
        Cannon = 81,

        [EventTypeInfo(EventFlag.Always)]
        CannonTarget = 82,

        [EventTypeInfo(EventFlag.Always)]
        Cannonball = 83,

        Enemy = 91,

        [EventTypeInfo(EventFlag.Always)]
        MovingGunshot = 92,

        DestructableGround = 95,
        SmackBX003 = 96,
        Ting = 97,
        Dino = 98,

        [EventTypeInfo(EventFlag.Always)]
        DinoBreath = 99,

        [EventTypeInfo(EventFlag.Always)]
        SmackBX003Thing = 100,

        Unk_102 = 102,

        [EventTypeInfo(EventFlag.Always)]
        GunshotExplosion = 103,

        Unk_104 = 104,

    }
}