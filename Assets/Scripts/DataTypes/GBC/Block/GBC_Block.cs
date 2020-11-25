namespace R1Engine
{
    public class GBC_Block : R1Serializable {
        public ushort BlockID { get; set; }
        public ushort Padding { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            BlockID = s.Serialize<ushort>(BlockID, name: nameof(BlockID));
            Padding = s.Serialize<ushort>(Padding, name: nameof(Padding));
        }
    }
}