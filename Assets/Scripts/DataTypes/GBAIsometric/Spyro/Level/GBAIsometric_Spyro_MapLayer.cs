namespace R1Engine
{
    public class GBAIsometric_Spyro_MapLayer : R1Serializable
    {
        public GBAIsometric_Spyro_DataBlockIndex MapIndex { get; set; }
        public GBAIsometric_Spyro_DataBlockIndex TileAssembleIndex { get; set; }
        public GBAIsometric_Spyro_DataBlockIndex TilesetIndex { get; set; }

        public ushort Flags { get; set; }

        public int Int_10 { get; set; }
        public int Int_14 { get; set; }

        // Parsed
        
        public GBAIsometric_Spyro_MapData Map { get; set; }
        public GBAIsometric_Spyro_TileAssemble TileAssemble { get; set; }
        public GBAIsometric_Spyro_TileSet TileSet { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            MapIndex = s.SerializeObject<GBAIsometric_Spyro_DataBlockIndex>(MapIndex, x => x.HasPadding = true, name: nameof(MapIndex));
            TileAssembleIndex = s.SerializeObject<GBAIsometric_Spyro_DataBlockIndex>(TileAssembleIndex, x => x.HasPadding = true, name: nameof(TileAssembleIndex));
            TilesetIndex = s.SerializeObject<GBAIsometric_Spyro_DataBlockIndex>(TilesetIndex, x => x.HasPadding = true, name: nameof(TilesetIndex));
            Flags = s.Serialize<ushort>(Flags, name: nameof(Flags));
            s.Serialize<ushort>(default, name: "Padding"); // Always 0 and not referenced by the code
            Int_10 = s.Serialize<int>(Int_10, name: nameof(Int_10));
            s.Log($"Layer: {BitHelpers.ExtractBits(Int_10, 2, 0)}");
            Int_14 = s.Serialize<int>(Int_14, name: nameof(Int_14));

            // Parse data
            Map = MapIndex.DoAtBlock(size => s.SerializeObject<GBAIsometric_Spyro_MapData>(Map, x => { x.BlockSize = size; }, name: nameof(Map)));
            TileAssemble = TileAssembleIndex.DoAtBlock(size => s.SerializeObject<GBAIsometric_Spyro_TileAssemble>(TileAssemble, x => x.BlockSize = size, name: nameof(TileAssemble)));
            TileSet = TilesetIndex.DoAtBlock(size => s.SerializeObject<GBAIsometric_Spyro_TileSet>(TileSet, x => x.BlockSize = size, name: nameof(TileSet)));
        }
    }
}