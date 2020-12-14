namespace R1Engine
{
    public class SNES_Proto_AnimatedTileEntry : R1Serializable
    {
        public SNES_Pointer GraphicsPointer { get; set; }
        public ushort VRAMAddress { get; set; }
        public byte Byte_05 { get; set; }
        public byte UnknownCount { get; set; }
        public byte[] Unknown { get; set; }

        public byte[] GraphicsBuffer { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            GraphicsPointer = s.SerializeObject<SNES_Pointer>(GraphicsPointer, onPreSerialize: p => p.HasMemoryBankValue = true, name: nameof(GraphicsPointer));
            VRAMAddress = s.Serialize<ushort>(VRAMAddress, name: nameof(VRAMAddress));
            Byte_05 = s.Serialize<byte>(Byte_05, name: nameof(Byte_05));
            UnknownCount = s.Serialize<byte>(UnknownCount, name: nameof(UnknownCount));
            Unknown = s.SerializeArray<byte>(Unknown, UnknownCount, name: nameof(Unknown));

            GraphicsBuffer = s.DoAt(GraphicsPointer.GetPointer(), () => {
                return s.SerializeArray<byte>(GraphicsBuffer, 0x20 * (Byte_05 == 0x60 ? 19 : 32), name: nameof(GraphicsBuffer));
            });
        }
    }
}