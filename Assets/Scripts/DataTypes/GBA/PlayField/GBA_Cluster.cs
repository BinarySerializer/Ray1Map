namespace R1Engine
{
    public class GBA_Cluster : GBA_BaseBlock
    {
        public byte[] UnknownData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            UnknownData = s.SerializeArray<byte>(UnknownData, BlockSize, name: nameof(UnknownData));
        }
    }
}