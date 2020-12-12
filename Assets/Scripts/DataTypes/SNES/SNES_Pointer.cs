namespace R1Engine
{
    /// <summary>
    /// SNES Pointer class. See https://datacrystal.romhacking.net/wiki/Pointer#SNES_Pointers
    /// </summary>
    public class SNES_Pointer : R1Serializable
    {
        public bool HasMemoryBankValue { get; set; } = false; // Set before serializing

        public ushort Pointer { get; set; }
        public ushort MemoryBank { get; set; }

        private Pointer _cachedPointer;
        public Pointer GetPointer() {
            if (_cachedPointer == null) {
                // The ROM is split into memory banks, with the size 0x8000 which get loaded at 0x8000 in RAM.
                var baseOffset = Offset.file.StartPointer;

                long bank = MemoryBank;

                // If we don't have a bank value the pointer is in the current bank
                if (!HasMemoryBankValue)
                    bank = Offset.FileOffset / MemoryBankSize;

                _cachedPointer = GetPointer(baseOffset, Pointer, bank);
            }
            return _cachedPointer;
        }

        public static Pointer GetPointer(Pointer romBaseOffset, long pointer, long memoryBank) => romBaseOffset + (MemoryBankSize * memoryBank) + (pointer - MemoryBankBaseAddress);

        public const long MemoryBankBaseAddress = 0x8000;
        public const long MemoryBankSize = 0x8000;

        public override void SerializeImpl(SerializerObject s)
        {
            Pointer = s.Serialize<ushort>(Pointer, name: nameof(Pointer));

            if (HasMemoryBankValue)
                MemoryBank = s.Serialize<ushort>(MemoryBank, name: nameof(MemoryBank));

            s.Log($"Pointer: {GetPointer()}");
        }
    }
}