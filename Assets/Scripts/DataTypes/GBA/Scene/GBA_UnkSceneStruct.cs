namespace R1Engine
{
    public class GBA_UnkSceneStruct : R1Serializable
    {
        public byte Length { get; set; }

        public byte UnkDataLength { get; set; }

        public byte Unk_02 { get; set; }

        public byte[] UnkData { get; set; }

        public byte[] RemainingData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Length = s.Serialize<byte>(Length, name: nameof(Length));
            UnkDataLength = s.Serialize<byte>(UnkDataLength, name: nameof(UnkDataLength));
            Unk_02 = s.Serialize<byte>(Unk_02, name: nameof(Unk_02));
            UnkData = s.SerializeArray<byte>(UnkData, UnkDataLength, name: nameof(UnkData));
            RemainingData = s.SerializeArray<byte>(RemainingData, Length - (s.CurrentPointer - Offset), name: nameof(RemainingData));
        }
    }
}