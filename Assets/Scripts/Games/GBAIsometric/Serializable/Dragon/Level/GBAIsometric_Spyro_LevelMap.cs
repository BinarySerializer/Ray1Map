using BinarySerializer;
using BinarySerializer.Nintendo.GBA;
using UnityEngine;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_Spyro_LevelMap : BinarySerializable
    {
        public GBAIsometric_IceDragon_ResourceRef TileSetIndex { get; set; }
        public GBAIsometric_IceDragon_ResourceRef MapIndex { get; set; }
        public GBAIsometric_IceDragon_ResourceRef PaletteIndex { get; set; }
        public GBAIsometric_IceDragon_ResourceRef Index3 { get; set; }
        public GBAIsometric_IceDragon_ResourceRef Index4 { get; set; } // Map data
        public ushort LevelID { get; set; }

        // Parsed
        public byte[] TileSet { get; set; }
        public GBAIsometric_IceDragon_SpriteMap Map { get; set; }
        public Palette256 Palette { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            TileSetIndex = s.SerializeObject<GBAIsometric_IceDragon_ResourceRef>(TileSetIndex, name: nameof(TileSetIndex));
            MapIndex = s.SerializeObject<GBAIsometric_IceDragon_ResourceRef>(MapIndex, name: nameof(MapIndex));
            PaletteIndex = s.SerializeObject<GBAIsometric_IceDragon_ResourceRef>(PaletteIndex, name: nameof(PaletteIndex));

            if (s.GetR1Settings().EngineVersion == EngineVersion.GBAIsometric_Spyro2)
            {
                Index3 = s.SerializeObject<GBAIsometric_IceDragon_ResourceRef>(Index3, name: nameof(Index3));
                Index4 = s.SerializeObject<GBAIsometric_IceDragon_ResourceRef>(Index4, name: nameof(Index4));
            }
            else
            {
                s.Serialize<uint>(default, "Padding");
            }

            LevelID = s.Serialize<ushort>(LevelID, name: nameof(LevelID));

            TileSetIndex.DoAt(size => TileSet = s.SerializeArray<byte>(TileSet, size, name: nameof(TileSet)));
            MapIndex.DoAt(size => Map = s.SerializeObject<GBAIsometric_IceDragon_SpriteMap>(Map, name: nameof(Map)));
            PaletteIndex.DoAt(size => Palette = s.SerializeObject<Palette256>(Palette, name: nameof(Palette)));
        }

        public Texture2D ToTexture2D()
        {
            const int cellSize = 8;
            const int tileSize = (cellSize * cellSize) / 2;

            var tex = TextureHelpers.CreateTexture2D(Map.Width * cellSize, Map.Height * cellSize);
            var pal = Util.ConvertGBAPalette(Palette.Colors);

            for (int y = 0; y < Map.Height; y++)
            {
                for (int x = 0; x < Map.Width; x++)
                {
                    var tile = Map.MapData[y * Map.Width + x];
                    tex.FillInTile(TileSet, tile.TileMapY * tileSize, pal, Util.TileEncoding.Linear_4bpp, cellSize, true, x * cellSize, y * cellSize, tile.HorizontalFlip, tile.VerticalFlip);
                }
            }

            tex.Apply();

            return tex;
        }
    }
}