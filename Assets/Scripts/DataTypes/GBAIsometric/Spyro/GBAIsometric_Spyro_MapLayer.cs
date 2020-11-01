using System;

namespace R1Engine
{
    public class GBAIsometric_Spyro_MapLayer : R1Serializable
    {
        public GBAIsometric_Spyro_DataBlockIndex MapIndex { get; set; } // 2D map of 1 byte structs (sometimes 2 byte structs) - each index here indexes the UnkMapData groups
        public GBAIsometric_Spyro_DataBlockIndex TileAssembleIndex { get; set; }
        public GBAIsometric_Spyro_DataBlockIndex TilesetIndex { get; set; }

        public byte Byte_0C { get; set; }
        public byte Byte_0D { get; set; }
        public byte Byte_0E { get; set; }
        public byte Byte_0F { get; set; }

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
            Byte_0C = s.Serialize<byte>(Byte_0C, name: nameof(Byte_0C));
            Byte_0D = s.Serialize<byte>(Byte_0D, name: nameof(Byte_0D));
            Byte_0E = s.Serialize<byte>(Byte_0E, name: nameof(Byte_0E));
            Byte_0F = s.Serialize<byte>(Byte_0F, name: nameof(Byte_0F));
            Int_10 = s.Serialize<int>(Int_10, name: nameof(Int_10));
            Int_14 = s.Serialize<int>(Int_14, name: nameof(Int_14));

            Map = MapIndex.DoAtBlock(size => s.SerializeObject<GBAIsometric_Spyro_MapData>(Map, x => x.BlockSize = size, name: nameof(Map)));

            TileAssemble = TileAssembleIndex.DoAtBlock(size => s.SerializeObject<GBAIsometric_Spyro_TileAssemble>(TileAssemble, x => x.BlockSize = size, name: nameof(TileAssemble)));
            TileSet = TilesetIndex.DoAtBlock(size => s.SerializeObject<GBAIsometric_Spyro_TileSet>(TileSet, x => x.BlockSize = size, name: nameof(TileSet)));
        }
    }
}