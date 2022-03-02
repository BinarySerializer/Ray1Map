using System.Linq;
using BinarySerializer;
using UnityEngine;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_IceDragon_CutsceneMap : BinarySerializable
    {
        public GBAIsometric_IceDragon_DataRef[] TileSetIndices { get; set; }
        public GBAIsometric_IceDragon_DataRef MapIndex { get; set; }
        public GBAIsometric_IceDragon_DataRef PaletteIndex { get; set; }

        // Parsed
        public byte[][] TileSets { get; set; }
        public GBAIsometric_IceDragon_SpriteMap Map { get; set; }
        public RGB555Color[] Palette { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if (s.GetR1Settings().EngineVersion == EngineVersion.GBAIsometric_Spyro1)
            {
                const GBAIsometric_IceDragon_CompressionType c = GBAIsometric_IceDragon_CompressionType.LZSS;
                TileSetIndices = s.SerializeObjectArray((GBAIsometric_IceDragon_DataPointer[])TileSetIndices, 4, x => x.Pre_CompressionType = c, name: nameof(TileSetIndices));
                MapIndex = s.SerializeObject((GBAIsometric_IceDragon_DataPointer)MapIndex, x => x.Pre_CompressionType = c, name: nameof(MapIndex));
                PaletteIndex = s.SerializeObject((GBAIsometric_IceDragon_DataPointer)PaletteIndex, name: nameof(PaletteIndex));
            }
            else
            {
                TileSetIndices = s.SerializeObjectArray((GBAIsometric_IceDragon_ResourceRef[])TileSetIndices, 4, name: nameof(TileSetIndices));
                MapIndex = s.SerializeObject((GBAIsometric_IceDragon_ResourceRef)MapIndex, name: nameof(MapIndex));
                PaletteIndex = s.SerializeObject((GBAIsometric_IceDragon_ResourceRef)PaletteIndex, name: nameof(PaletteIndex));
            }

            TileSets ??= new byte[TileSetIndices.Length][];

            for (int i = 0; i < TileSets.Length; i++)
                TileSetIndices[i].DoAt(size => TileSets[i] = s.SerializeArray<byte>(TileSets[i], size, name: $"{nameof(TileSets)}[{i}]"));

            MapIndex.DoAt(size => Map = s.SerializeObject<GBAIsometric_IceDragon_SpriteMap>(Map, name: nameof(Map)));
            PaletteIndex.DoAt(size => Palette = s.SerializeObjectArray<RGB555Color>(Palette, 256, name: nameof(Palette)));

            s.Log("Min: {0}", Map.MapData.Where(x => x.TileMapY > 1).Min(x => x.TileMapY));
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