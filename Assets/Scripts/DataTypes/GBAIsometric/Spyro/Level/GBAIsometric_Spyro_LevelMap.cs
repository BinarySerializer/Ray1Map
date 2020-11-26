using UnityEngine;

namespace R1Engine
{
    public class GBAIsometric_Spyro_LevelMap : R1Serializable
    {
        public GBAIsometric_Spyro_DataBlockIndex TileSetIndex { get; set; }
        public GBAIsometric_Spyro_DataBlockIndex MapIndex { get; set; }
        public GBAIsometric_Spyro_DataBlockIndex PaletteIndex { get; set; }
        public GBAIsometric_Spyro_DataBlockIndex Index3 { get; set; }
        public GBAIsometric_Spyro_DataBlockIndex Index4 { get; set; } // Map data
        public ushort LevelID { get; set; }

        // Parsed
        public byte[] TileSet { get; set; }
        public GBAIsometric_Spyro_SpriteMap Map { get; set; }
        public ARGB1555Color[] Palette { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            TileSetIndex = s.SerializeObject<GBAIsometric_Spyro_DataBlockIndex>(TileSetIndex, name: nameof(TileSetIndex));
            MapIndex = s.SerializeObject<GBAIsometric_Spyro_DataBlockIndex>(MapIndex, name: nameof(MapIndex));
            PaletteIndex = s.SerializeObject<GBAIsometric_Spyro_DataBlockIndex>(PaletteIndex, name: nameof(PaletteIndex));

            if (s.GameSettings.EngineVersion == EngineVersion.GBAIsometric_Spyro2)
            {
                Index3 = s.SerializeObject<GBAIsometric_Spyro_DataBlockIndex>(Index3, name: nameof(Index3));
                Index4 = s.SerializeObject<GBAIsometric_Spyro_DataBlockIndex>(Index4, name: nameof(Index4));
            }
            else
            {
                s.Serialize<uint>(default, "Padding");
            }

            LevelID = s.Serialize<ushort>(LevelID, name: nameof(LevelID));

            TileSet = TileSetIndex.DoAtBlock(size => s.SerializeArray<byte>(TileSet, size, name: nameof(TileSet)));
            Map = MapIndex.DoAtBlock(size => s.SerializeObject<GBAIsometric_Spyro_SpriteMap>(Map, name: nameof(Map)));
            Palette = PaletteIndex.DoAtBlock(size => s.SerializeObjectArray<ARGB1555Color>(Palette, 256, name: nameof(Palette)));
        }

        public Texture2D ToTexture2D()
        {
            const int cellSize = 8;
            const int tileSize = (cellSize * cellSize) / 2;

            var tex = TextureHelpers.CreateTexture2D(Map.Width * cellSize, Map.Height * cellSize);
            var pal = Util.ConvertGBAPalette(Palette);

            for (int y = 0; y < Map.Height; y++)
            {
                for (int x = 0; x < Map.Width; x++)
                {
                    var tile = Map.MapData[y * Map.Width + x];
                    tex.FillInTile(TileSet, tile.TileMapY * tileSize, pal, 4, cellSize, true, x * cellSize, y * cellSize, tile.HorizontalFlip, tile.VerticalFlip);
                }
            }

            tex.Apply();

            return tex;
        }
    }
}