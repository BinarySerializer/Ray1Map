namespace R1Engine
{
    public class GBAIsometric_Spyro_UnkStruct2 : R1Serializable
    {
        public ushort ID { get; set; }
        public GBAIsometric_Spyro_DataBlockIndex MapDataIndex { get; set; }
        public GBAIsometric_Spyro_DataBlockIndex Index_04 { get; set; }
        public uint Uint_08 { get; set; } // Some count?

        // Parsed
        public GBAIsometric_Spyro_MapData MapData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ID = s.Serialize<ushort>(ID, name: nameof(ID));
            MapDataIndex = s.SerializeObject<GBAIsometric_Spyro_DataBlockIndex>(MapDataIndex, name: nameof(MapDataIndex));
            Index_04 = s.SerializeObject<GBAIsometric_Spyro_DataBlockIndex>(Index_04, x => x.HasPadding = true, name: nameof(Index_04));
            Uint_08 = s.Serialize<uint>(Uint_08, name: nameof(Uint_08));

            // Not always valid map data
            //MapData = MapDataIndex.DoAtBlock(size => s.SerializeObject<GBAIsometric_Spyro_MapData>(MapData, name: nameof(MapData)));
        }
    }
}