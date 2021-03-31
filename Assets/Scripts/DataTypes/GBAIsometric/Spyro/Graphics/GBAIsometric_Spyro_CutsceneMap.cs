using System.Linq;
using BinarySerializer;
using UnityEngine;

namespace R1Engine
{
    public class GBAIsometric_Spyro_CutsceneMap : BinarySerializable
    {
        public GBAIsometric_Spyro_DataBlockIndex[] TileSetIndices { get; set; }
        public GBAIsometric_Spyro_DataBlockIndex MapIndex { get; set; }
        public GBAIsometric_Spyro_DataBlockIndex PaletteIndex { get; set; }

        // Parsed
        public byte[][] TileSets { get; set; }
        public GBAIsometric_Spyro_SpriteMap Map { get; set; }
        public RGBA5551Color[] Palette { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            TileSetIndices = s.SerializeObjectArray<GBAIsometric_Spyro_DataBlockIndex>(TileSetIndices, 4, name: nameof(TileSetIndices));
            MapIndex = s.SerializeObject<GBAIsometric_Spyro_DataBlockIndex>(MapIndex, name: nameof(MapIndex));
            PaletteIndex = s.SerializeObject<GBAIsometric_Spyro_DataBlockIndex>(PaletteIndex, name: nameof(PaletteIndex));

            if (TileSets == null)
                TileSets = new byte[TileSetIndices.Length][];

            for (int i = 0; i < TileSets.Length; i++)
                TileSets[i] = TileSetIndices[i].DoAtBlock(size => s.SerializeArray<byte>(TileSets[i], size, name: $"{nameof(TileSets)}[{i}]"));

            Map = MapIndex.DoAtBlock(size => s.SerializeObject<GBAIsometric_Spyro_SpriteMap>(Map, name: nameof(Map)));
            Palette = PaletteIndex.DoAtBlock(size => s.SerializeObjectArray<RGBA5551Color>(Palette, 256, name: nameof(Palette)));

            s.Log($"Min: {Map.MapData.Where(x => x.TileMapY > 1).Min(x => x.TileMapY)}");
        }

        public Texture2D ToTexture2D()
        {
            const int cellSize = 8;
            const int tileSize = cellSize * cellSize;

            var tex = TextureHelpers.CreateTexture2D(Map.Width * cellSize, Map.Height * cellSize);
            var pal = Util.ConvertGBAPalette(Palette);

            var fullTileSet = TileSets.SelectMany(x => x).ToArray();

            for (int y = 0; y < Map.Height; y++)
            {
                for (int x = 0; x < Map.Width; x++)
                {
                    var tile = Map.MapData[y * Map.Width + x];
                    tex.FillInTile(fullTileSet, tile.TileMapY * tileSize, pal, Util.TileEncoding.Linear_8bpp, cellSize, true, x * cellSize, y * cellSize, tile.HorizontalFlip, tile.VerticalFlip);
                }
            }

            tex.Apply();

            return tex;
        }
    }
}