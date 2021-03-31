using System.Linq;
using BinarySerializer;
using UnityEngine;

namespace R1Engine
{
    public class GBC_Vignette : GBC_BaseBlock
    {
        public GBC_Vignette[] Vignettes { get; set; }

        public byte Width { get; set; }
        public byte Height { get; set; }
        public byte[] UnkData { get; set; }
        public RGBA5551Color[] Palette { get; set; }
        public byte[] TileSet { get; set; }
        public byte[] PalIndices { get; set; }

        public Texture2D ToTexture2D()
        {
            if(Width == 0 || Height == 0) return null;
            var pal = Util.ConvertAndSplitGBCPalette(Palette, transparentIndex: null);
            return Util.ToTileSetTexture(TileSet, pal.First(), Util.TileEncoding.Planar_2bpp, 8, true, wrap: Width, getPalFunc: x => pal[PalIndices[x]]);
        }

        public override void SerializeBlock(SerializerObject s)
        {
            if (DependencyTable.DependenciesCount > 0) {
                if (Vignettes == null) Vignettes = new GBC_Vignette[DependencyTable.DependenciesCount];

                for (int i = 0; i < Vignettes.Length; i++)
                    Vignettes[i] = s.DoAt(DependencyTable.GetPointer(i), () => s.SerializeObject<GBC_Vignette>(Vignettes[i], name: $"{nameof(Vignettes)}[{i}]"));
            } else {
                // Serialize data
                Width = s.Serialize<byte>(Width, name: nameof(Width));
                Height = s.Serialize<byte>(Height, name: nameof(Height));
                UnkData = s.SerializeArray<byte>(UnkData, 14, name: nameof(UnkData));
                Palette = s.SerializeObjectArray<RGBA5551Color>(Palette, 4 * 8, name: nameof(Palette));
                TileSet = s.SerializeArray<byte>(TileSet, Width * Height * 0x10, name: nameof(TileSet));
                PalIndices = s.SerializeArray<byte>(PalIndices, Width * Height, name: nameof(PalIndices));
            }
        }
    }
}