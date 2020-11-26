namespace R1Engine
{
    public class LUDI_BlockIdentifier : R1Serializable
    {
        public ushort BlockID { get; set; }
        public ushort Padding { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            BlockID = s.Serialize<ushort>(BlockID, name: nameof(BlockID));
            Padding = s.Serialize<ushort>(Padding, name: nameof(Padding));
        }
    }
}