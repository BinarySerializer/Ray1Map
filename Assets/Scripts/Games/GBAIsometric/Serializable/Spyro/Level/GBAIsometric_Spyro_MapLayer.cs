using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_Spyro_MapLayer : BinarySerializable
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
            MapIndex = s.SerializeObject<GBAIsometric_Spyro_DataBlockIndex>(MapIndex, x => x.Pre_HasPadding = true, name: nameof(MapIndex));
            TileAssembleIndex = s.SerializeObject<GBAIsometric_Spyro_DataBlockIndex>(TileAssembleIndex, x => x.Pre_HasPadding = true, name: nameof(TileAssembleIndex));
            TilesetIndex = s.SerializeObject<GBAIsometric_Spyro_DataBlockIndex>(TilesetIndex, x => x.Pre_HasPadding = true, name: nameof(TilesetIndex));
            Flags = s.Serialize<ushort>(Flags, name: nameof(Flags));
            s.Serialize<ushort>(default, name: "Padding"); // Always 0 and not referenced by the code
            Int_10 = s.Serialize<int>(Int_10, name: nameof(Int_10));
            s.Log($"Layer: {BitHelpers.ExtractBits(Int_10, 2, 0)}");
            Int_14 = s.Serialize<int>(Int_14, name: nameof(Int_14));

            // Parse data
            MapIndex.DoAt(size => Map = s.SerializeObject<GBAIsometric_Spyro_MapData>(Map, name: nameof(Map)));
            TileAssembleIndex.DoAt(size => TileAssemble = s.SerializeObject<GBAIsometric_Spyro_TileAssemble>(TileAssemble, x => x.BlockSize = size, name: nameof(TileAssemble)));
            TilesetIndex.DoAt(size => TileSet = s.SerializeObject<GBAIsometric_Spyro_TileSet>(TileSet, x => x.BlockSize = size, name: nameof(TileSet)));
        }
    }
}