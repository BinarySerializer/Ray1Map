namespace R1Engine
{
    public class GBC_Pointer : R1Serializable
    {
        public bool HasMemoryBankValue { get; set; } = true; // Set before serializing

        public ushort Pointer { get; set; }
        public ushort MemoryBank { get; set; }

        public Pointer GetPointer()
        {
            // The ROM is split into memory banks, with the size 0x4000 which get loaded at 0x4000 in RAM.
            var baseOffset = Offset.file.StartPointer;

            long bank = MemoryBank;

            // If we don't have a bank value the pointer is in the current bank
            if (!HasMemoryBankValue)
                bank = Offset.FileOffset / 0x4000;

            return baseOffset + (0x4000 * bank) + (Pointer - 0x4000);
        }

        public override void SerializeImpl(SerializerObject s)
        {
            Pointer = s.Serialize<ushort>(Pointer, name: nameof(Pointer));

            if (HasMemoryBankValue)
                MemoryBank = s.Serialize<ushort>(MemoryBank, name: nameof(MemoryBank));

            s.Log($"Pointer: {GetPointer()}");
        }
    }
}