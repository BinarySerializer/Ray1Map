namespace R1Engine
{
    public class GBC_PalmOS_UncompressedBlock<T> : R1Serializable
        where T : R1Serializable, new()
    {
        public LUDI_BlockHeader Header { get; set; }
        public T Value { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            Header = s.SerializeObject<LUDI_BlockHeader>(Header, name: nameof(Header));
            Value = s.SerializeObject<T>(Value, name: nameof(Value));
        }
    }
}