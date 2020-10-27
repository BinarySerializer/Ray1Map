namespace R1Engine
{
    public class GBAIsometric_GraphicsData : R1Serializable
    {
        public int Int_00 { get; set; }
        public int UnkDataLength { get; set; }
        public short Short_08 { get; set; }
        public short Short_0A { get; set; }

        public Pointer Pointer_0C { get; set; }
        public Pointer UnkDataPointer { get; set; }

        // Parsed
        public byte[] UnkData { get; set; } // Graphics data? Compressed?

        public override void SerializeImpl(SerializerObject s)
        {
            Int_00 = s.Serialize<int>(Int_00, name: nameof(Int_00));
            UnkDataLength = s.Serialize<int>(UnkDataLength, name: nameof(UnkDataLength));
            Short_08 = s.Serialize<short>(Short_08, name: nameof(Short_08));
            Short_0A = s.Serialize<short>(Short_0A, name: nameof(Short_0A));

            Pointer_0C = s.SerializePointer(Pointer_0C, name: nameof(Pointer_0C));
            UnkDataPointer = s.SerializePointer(UnkDataPointer, name: nameof(UnkDataPointer));

            // Serialize data from pointers
            UnkData = s.DoAt(UnkDataPointer, () => s.SerializeArray<byte>(UnkData, UnkDataLength, name: nameof(UnkData)));
        }
    }
}