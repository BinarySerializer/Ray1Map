namespace R1Engine
{
    public class GBC_SoundBank : GBC_BaseBlock
    {
        public byte[] Data { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            // TODO: Parse data and data from referenced blocks
            Data = s.SerializeArray<byte>(Data, BlockSize, name: nameof(Data));
        }
    }
}