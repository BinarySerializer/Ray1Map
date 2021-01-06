namespace R1Engine
{
    public class GBA_Milan_Action : GBA_BaseBlock
    {
        public ushort Ushort_00 { get; set; }
        public ushort AnimIndex { get; set; }
        public uint Uint_04 { get; set; } // Seems to determine the length of the remaining data

        public byte[] RemainingData { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            Ushort_00 = s.Serialize<ushort>(Ushort_00, name: nameof(Ushort_00));
            AnimIndex = s.Serialize<ushort>(AnimIndex, name: nameof(AnimIndex));
            Uint_04 = s.Serialize<uint>(Uint_04, name: nameof(Uint_04));

            RemainingData = s.SerializeArray<byte>(RemainingData, (Offset + BlockSize) - s.CurrentPointer, name: nameof(RemainingData));
        }
    }
}