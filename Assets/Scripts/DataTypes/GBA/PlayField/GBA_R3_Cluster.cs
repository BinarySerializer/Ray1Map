namespace R1Engine
{
    public class GBA_R3_Cluster : GBA_R3_BaseBlock
    {
        public byte[] UnknownData { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            UnknownData = s.SerializeArray<byte>(UnknownData, BlockSize, name: nameof(UnknownData));
        }
    }
}