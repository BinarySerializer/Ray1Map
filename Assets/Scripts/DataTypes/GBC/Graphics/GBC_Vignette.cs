using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class GBC_Vignette : GBC_BaseBlock
    {
        public byte Width { get; set; }
        public byte Height { get; set; }
        public byte[] UnkData { get; set; }
        public ARGB1555Color[] Palette { get; set; }
        public byte[] TileSet { get; set; }
        public byte[] PalIndices { get; set; }

        public Texture2D ToTexture2D()
        {
            var pal = Util.ConvertAndSplitGBCPalette(Palette, firstTransparent: false);
            return Util.ToTileSetTexture(TileSet, pal.First(), Util.TileEncoding.Planar_2bpp, 8, true, wrap: Width, getPalFunc: x => pal[PalIndices[x]]);
        }

        public override void SerializeBlock(SerializerObject s)
        {
            // Serialize data
            Width = s.Serialize<byte>(Width, name: nameof(Width));
            Height = s.Serialize<byte>(Height, name: nameof(Height));
            UnkData = s.SerializeArray<byte>(UnkData, 14, name: nameof(UnkData));
            Palette = s.SerializeObjectArray<ARGB1555Color>(Palette, 4 * 8, name: nameof(Palette));
            TileSet = s.SerializeArray<byte>(TileSet, Width * Height * 0x10, name: nameof(TileSet));
            PalIndices = s.SerializeArray<byte>(PalIndices, Width * Height, name: nameof(PalIndices));
        }
    }
}