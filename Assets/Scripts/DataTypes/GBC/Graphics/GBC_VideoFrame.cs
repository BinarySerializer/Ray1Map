using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class GBC_VideoFrame : GBC_BaseBlock
    {
        public byte Height { get; set; }
        public byte Width { get; set; }
        public byte[] UnkData { get; set; }
        public byte[] TileSet { get; set; }
        public byte[] PalIndices { get; set; }
        public RGBA5551Color[] Palette { get; set; }

        public Texture2D ToTexture2D()
        {
            var pal = Util.CreateDummyPalette(4, firstTransparent: false);
            return Util.ToTileSetTexture(TileSet, pal.Select(x => x.GetColor()).ToArray(), Util.TileEncoding.Planar_2bpp, 8, true, wrap: Width);
        }

        public override void SerializeBlock(SerializerObject s)
        {
            // Serialize data
            Height = s.Serialize<byte>(Height, name: nameof(Height));
            Width = s.Serialize<byte>(Width, name: nameof(Width));
            UnkData = s.SerializeArray<byte>(UnkData, 14, name: nameof(UnkData));
            TileSet = s.SerializeArray<byte>(TileSet, Width * Height * 0x10, name: nameof(TileSet));
            PalIndices = s.SerializeArray<byte>(PalIndices, Width * Height, name: nameof(PalIndices));
            Palette = s.SerializeObjectArray<RGBA5551Color>(Palette, Width * Height, name: nameof(Palette)); 
        }
    }
}