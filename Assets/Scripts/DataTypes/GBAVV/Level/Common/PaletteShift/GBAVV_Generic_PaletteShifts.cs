using BinarySerializer;

namespace R1Engine
{
    public class GBAVV_Generic_PaletteShifts : BinarySerializable
    {
        public uint ShiftsPointer { get; set; } // Memory pointer
        public int ShiftsCount { get; set; }

        // Serialized from pointers
        public GBAVV_Generic_PaletteShift[] Shifts { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ShiftsPointer = s.Serialize<uint>(ShiftsPointer, name: nameof(ShiftsPointer));
            ShiftsCount = s.Serialize<int>(ShiftsCount, name: nameof(ShiftsCount));

            if (ShiftsPointer != 0)
            {
                var memOffset = s.GetR1Settings().GetGameManagerOfType<GBAVV_SpongeBobRevengeOfTheFlyingDutchman_Manager>().ROMMemPointersOffset;
                Shifts = s.DoAt(new Pointer(ShiftsPointer + memOffset, Offset.File), () => s.SerializeObjectArray<GBAVV_Generic_PaletteShift>(Shifts, ShiftsCount, name: nameof(Shifts)));
            }
        }
    }
}