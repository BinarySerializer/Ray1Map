namespace R1Engine
{
    public class GBC_Actor : R1Serializable
    {
        public byte UnkIndex { get; set; } // Index to PlayField offset table, unless it's 0 or -1
        public byte Index { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            
        }
    }
}