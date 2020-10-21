using System;

namespace R1Engine
{
    [Flags]
    public enum R1_EventFlags
    {
        None = 0,

        IsAlways = 1 << 0, // If true the game sets the pos to (-32000, -32000) on init
        Flag_01 = 1 << 1,
        NoCollision = 1 << 2, // Indicates if the event has no collision - does not include follow
        HurtsRayman = 1 << 3, // Indicates if the event damages Rayman
        Flag_04 = 1 << 4,
        Flag_05 = 1 << 5,
        Flag_06 = 1 << 6,
        Flag_07 = 1 << 7,

        Flag_08 = 1 << 8,
        IsCollectible = 1 << 9, // Indicates if the event can be collected and thus not respawn again
        Flag_0A = 1 << 10,
        Flag_0B = 1 << 11,
        Flag_0C = 1 << 12,
        Flag_0D = 1 << 13,
        Flag_0E = 1 << 14,
        Flag_0F = 1 << 15,

        Flag_10 = 1 << 16,
        Flag_11 = 1 << 17,
        Flag_12 = 1 << 18,
        Flag_13 = 1 << 19,
        Flag_14 = 1 << 20,
        Flag_15 = 1 << 21,
        Flag_16 = 1 << 22,
        Flag_17 = 1 << 23,

        Flag_18 = 1 << 24,
        Flag_19 = 1 << 25,
        Flag_1A = 1 << 26,
        Flag_1B = 1 << 27,
        Flag_1C = 1 << 28,
        LinkRequiresGendoor = 1 << 29, // Indicates if the object requires a gendoor in the link group to be valid
        NoLink = 1 << 30, // Indicates that the event can't be linked
        Flag_1F = 1 << 31,
    }
}