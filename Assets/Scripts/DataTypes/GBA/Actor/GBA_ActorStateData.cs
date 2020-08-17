namespace R1Engine
{
    public class GBA_ActorStateData : GBA_BaseBlock
    {
        public byte[] Data { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Data = s.SerializeArray<byte>(Data, BlockSize, name: nameof(Data));
        }
    }
}