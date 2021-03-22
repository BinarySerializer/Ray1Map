namespace R1Engine
{
    public class GBAVV_Generic_PaletteShifts : R1Serializable
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
                var memOffset = s.GameSettings.GetGameManagerOfType<GBAVV_SpongeBobRevengeOfTheFlyingDutchman_Manager>().ROMMemPointersOffset;
                Shifts = s.DoAt(new Pointer(ShiftsPointer + memOffset, Offset.file), () => s.SerializeObjectArray<GBAVV_Generic_PaletteShift>(Shifts, ShiftsCount, name: nameof(Shifts)));
            }
        }
    }
}