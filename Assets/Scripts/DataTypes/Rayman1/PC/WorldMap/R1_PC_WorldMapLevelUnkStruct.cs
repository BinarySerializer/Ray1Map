namespace R1Engine
{
    public class R1_PC_WorldMapLevelUnkStruct : R1Serializable
    {
        public uint DataLength { get; set; }
        public byte[] Unk2 { get; set; }
        public byte[] Data { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            DataLength = s.Serialize<uint>(DataLength, name: nameof(DataLength));

            if (DataLength > 0)
            {
                Unk2 = s.SerializeArray<byte>(Unk2, 24, name: nameof(Unk2));
                Data = s.SerializeArray<byte>(Data, DataLength, name: nameof(Data));
            }
        }
    }
}