﻿using BinarySerializer;
using UnityEngine;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_Spyro_PortraitSprite : BinarySerializable
    {
        public uint ID { get; set; }
        public Pointer Pointer_04 { get; set; } // Uint array
        public GBAIsometric_IceDragon_ResourceRef MapIndex { get; set; }
        public GBAIsometric_IceDragon_ResourceRef TileSetIndex { get; set; }
        public GBAIsometric_IceDragon_ResourceRef PaletteIndex { get; set; }

        // Parsed
        public GBAIsometric_IceDragon_SpriteMap Map { get; set; }
        public byte[] TileSet { get; set; }
        public RGBA5551Color[] Palette { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ID = s.Serialize<uint>(ID, name: nameof(ID));
            Pointer_04 = s.SerializePointer(Pointer_04, name: nameof(Pointer_04));
            MapIndex = s.SerializeObject<GBAIsometric_IceDragon_ResourceRef>(MapIndex, name: nameof(MapIndex));
            TileSetIndex = s.SerializeObject<GBAIsometric_IceDragon_ResourceRef>(TileSetIndex, name: nameof(TileSetIndex));
            PaletteIndex = s.SerializeObject<GBAIsometric_IceDragon_ResourceRef>(PaletteIndex, name: nameof(PaletteIndex));
            s.Serialize<ushort>(default, name: "Padding");

            MapIndex.DoAt(size => Map = s.SerializeObject<GBAIsometric_IceDragon_SpriteMap>(Map, name: nameof(Map)));
            TileSetIndex.DoAt(size => TileSet = s.SerializeArray<byte>(TileSet, size, name: nameof(TileSet)));
            PaletteIndex.DoAt(size => Palette = s.SerializeObjectArray<RGBA5551Color>(Palette, size / 2, name: nameof(Palette)));
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
                    tex.FillInTile(TileSet, tile.TileMapY * tileSize, pal, Util.TileEncoding.Linear_4bpp, cellSize, true, x * cellSize, y * cellSize, tile.HorizontalFlip, tile.VerticalFlip);
                }
            }

            tex.Apply();

            return tex;
        }
    }
}