using System;

namespace R1Engine
{
    [Flags]
    public enum RayEvtsFlags : ushort
    {
        None = 0,

        Fist = 1 << 0,
        Hang = 1 << 1,

        Helico = 1 << 2,
        SuperHelico = 1 << 3,

        Unk_4 = 1 << 4,
        Unk_5 = 1 << 5,

        Seed = 1 << 6,
        Grab = 1 << 7,

        Run = 1 << 8,

        // Rest are for temp stuff, like Mr Dark spells etc.
        Unk_9 = 1 << 9,
        Unk_10 = 1 << 10,
        Unk_11 = 1 << 11,
        Unk_12 = 1 << 12,
        Unk_13 = 1 << 13,
        Unk_14 = 1 << 14,
        Unk_15 = 1 << 15,
    }
}