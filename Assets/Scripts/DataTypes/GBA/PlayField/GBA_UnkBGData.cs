namespace R1Engine
{
    public class GBA_UnkBGData : GBA_BaseBlock
    {
        public uint Count { get; set; }
        public ushort[] Data { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Count = s.Serialize<uint>(Count, name: nameof(Count));
            Data = s.SerializeArray<ushort>(Data, Count, name: nameof(Data));
        }
    }
}