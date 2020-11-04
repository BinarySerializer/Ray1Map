namespace R1Engine
{
    public class GBAIsometric_Spyro_UnkStruct2 : R1Serializable
    {
        public ushort ID { get; set; }
        public GBAIsometric_Spyro_DataBlockIndex Index_02 { get; set; }
        public GBAIsometric_Spyro_DataBlockIndex Index_04 { get; set; }
        public uint Uint_08 { get; set; } // Some count?

        public override void SerializeImpl(SerializerObject s)
        {
            ID = s.Serialize<ushort>(ID, name: nameof(ID));
            Index_02 = s.SerializeObject<GBAIsometric_Spyro_DataBlockIndex>(Index_02, name: nameof(Index_02));
            Index_04 = s.SerializeObject<GBAIsometric_Spyro_DataBlockIndex>(Index_04, x => x.HasPadding = true, name: nameof(Index_04));
            Uint_08 = s.Serialize<uint>(Uint_08, name: nameof(Uint_08));
        }
    }
}