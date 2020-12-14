namespace R1Engine
{
    public class SNES_Proto_AnimatedTileEntry : R1Serializable
    {
        public SNES_Pointer GraphicsPointer { get; set; }
        public ushort VRAMAddress { get; set; }
        public ushort BytesToCopy { get; set; }
        public byte[] Unknown { get; set; }

        public byte[] GraphicsBuffer { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            GraphicsPointer = s.SerializeObject<SNES_Pointer>(GraphicsPointer, onPreSerialize: p => p.HasMemoryBankValue = true, name: nameof(GraphicsPointer));
            VRAMAddress = s.Serialize<ushort>(VRAMAddress, name: nameof(VRAMAddress));
            BytesToCopy = s.Serialize<ushort>(BytesToCopy, name: nameof(BytesToCopy));
            Unknown = s.SerializeArray<byte>(Unknown, BytesToCopy >> 8, name: nameof(Unknown));

            GraphicsBuffer = s.DoAt(GraphicsPointer.GetPointer(), () => {
                return s.SerializeArray<byte>(GraphicsBuffer, BytesToCopy, name: nameof(GraphicsBuffer));
            });
        }
    }
}