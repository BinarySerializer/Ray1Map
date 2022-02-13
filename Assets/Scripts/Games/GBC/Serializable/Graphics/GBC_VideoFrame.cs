using BinarySerializer;
using UnityEngine;

namespace Ray1Map.GBC
{
    public class GBC_VideoFrame : GBC_BaseBlock
    {
        public byte Height { get; set; }
        public byte Width { get; set; }
        public byte[] UnkData0 { get; set; }
        public byte PaletteCount { get; set; }
        public byte PaletteBlockCount { get; set; }
        public uint PaletteOffset { get; set; }
        public uint TileDataOffset { get; set; }
        public byte[] UnkData1 { get; set; }
        public byte[] TileSet { get; set; }
        public byte[] PalIndices { get; set; }
        public RGBA5551Color[][] Palette { get; set; }

        public Texture2D ToTexture2D() {
            if (Width == 0 || Height == 0) return null;
            Texture2D tex = TextureHelpers.CreateTexture2D(Width * 8, Height * 8);
            for (int y = 0; y < Height; y++) {
                var pal = Util.ConvertAndSplitGBCPalette(Palette[y], transparentIndex: null);
                for (int x = 0; x < Width; x++) {
                    var tileInd = (y * Width + x);
                   tex.FillInTile(TileSet, tileInd * 0x10, pal[PalIndices[tileInd]], Util.TileEncoding.Planar_2bpp, 8, true, x*8,y*8);
                }
            }
            tex.Apply();
            return tex;
        }

        public override void SerializeBlock(SerializerObject s)
        {
            // Serialize data
            Height = s.Serialize<byte>(Height, name: nameof(Height));
            Width = s.Serialize<byte>(Width, name: nameof(Width));
            UnkData0 = s.SerializeArray<byte>(UnkData0, 4, name: nameof(UnkData0));
            PaletteCount = s.Serialize<byte>(PaletteCount, name: nameof(PaletteCount));
            PaletteBlockCount = s.Serialize<byte>(PaletteBlockCount, name: nameof(PaletteBlockCount));
            PaletteOffset = s.Serialize<uint>(PaletteOffset, name: nameof(PaletteOffset));
            TileDataOffset = s.Serialize<uint>(TileDataOffset, name: nameof(TileDataOffset));

            var basePtr = s.CurrentPointer;
            s.Goto(basePtr + TileDataOffset);
            TileSet = s.SerializeArray<byte>(TileSet, Width * Height * 0x10, name: nameof(TileSet));
            PalIndices = s.SerializeArray<byte>(PalIndices, Width * Height, name: nameof(PalIndices));

            s.Goto(basePtr + PaletteOffset);
            if(Palette == null) Palette = new RGBA5551Color[PaletteBlockCount][];
            for (int i = 0; i < PaletteBlockCount; i++) {
                Palette[i] = s.SerializeObjectArray<RGBA5551Color>(Palette[i], PaletteCount * 4, name: $"{nameof(Palette)}[{i}]");
            }
        }
    }
}