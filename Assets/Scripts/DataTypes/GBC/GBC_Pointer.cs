using BinarySerializer;

namespace R1Engine
{
    public class GBC_Pointer : BinarySerializable
    {
        public bool HasMemoryBankValue { get; set; } = true; // Set before serializing

        public ushort Pointer { get; set; }
        public ushort MemoryBank { get; set; }

        private Pointer _cachedPointer;
        public Pointer GetPointer() {
            if (_cachedPointer == null) {
                // The ROM is split into memory banks, with the size 0x4000 which get loaded at 0x4000 in RAM.
                var baseOffset = Offset.File.StartPointer;

                long bank = MemoryBank;

                // If we don't have a bank value the pointer is in the current bank
                if (!HasMemoryBankValue)
                    bank = Offset.FileOffset / MemoryBankSize;

                _cachedPointer = GetPointer(baseOffset, Pointer, bank);
            }
            return _cachedPointer;
        }

        public static Pointer GetPointer(Pointer romBaseOffset, long pointer, long memoryBank) => romBaseOffset + (MemoryBankSize * memoryBank) + (pointer - MemoryBankBaseAddress);

        public const long MemoryBankBaseAddress = 0x4000;
        public const long MemoryBankSize = 0x4000;

        public override void SerializeImpl(SerializerObject s)
        {
            Pointer = s.Serialize<ushort>(Pointer, name: nameof(Pointer));

            if (HasMemoryBankValue)
                MemoryBank = s.Serialize<ushort>(MemoryBank, name: nameof(MemoryBank));

            s.Log($"Pointer: {GetPointer()}");
        }
    }
}