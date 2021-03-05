namespace R1Engine
{
    public class GBAVV_NitroKart_NGage_BackgroundLayer : R1Serializable
    {
        public Pointer DataPointer { get; set; }
        public int DataLength { get; set; }

        // Serialized from pointers
        public byte[] Data { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            DataPointer = s.SerializePointer(DataPointer, name: nameof(DataPointer));
            DataLength = s.Serialize<int>(DataLength, name: nameof(DataLength));

            Data = s.DoAt(DataPointer, () => s.SerializeArray<byte>(Data, DataLength * 4, name: nameof(Data)));
        }
    }
}