using System;

namespace BinarySerializer.Ubisoft.Onyx.NDS
{
    [Flags]
    public enum PuppetFlags : byte
    {
        None = 0,
        UseTileBuffer = 1 << 0, // For reusing tiles when allocating?
        Flag_1 = 1 << 1,
        Flag_2 = 1 << 2,
        Flag_3 = 1 << 3,
        Is8Bit = 1 << 4,
        Flag_5 = 1 << 5,
        Flag_6 = 1 << 6,
        Flag_7 = 1 << 7,
    }
}