namespace R1Engine
{
    public abstract class GBC_BaseBlock : R1Serializable {
        public ushort BlockID { get; set; }
        public ushort Padding { get; set; }
        public GBC_OffsetTable OffsetTable { get; set; } // Not always here

        public override void SerializeImpl(SerializerObject s) {
            BlockID = s.Serialize<ushort>(BlockID, name: nameof(BlockID));
            Padding = s.Serialize<ushort>(Padding, name: nameof(Padding));
        }

        protected void SerializeOffsetTable(SerializerObject s) {
            OffsetTable = s.SerializeObject<GBC_OffsetTable>(OffsetTable, name: nameof(OffsetTable));
        }
    }
}