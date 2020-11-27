namespace R1Engine
{
    public class GBC_ActorGraphicsData : GBC_BaseBlock
    {
        public byte[] Data { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            Data = s.SerializeArray<byte>(Data, BlockSize, name: nameof(Data));

            // TODO: Parse block at offset 0, which in turn references block which references tileKit + animations
        }
    }
}