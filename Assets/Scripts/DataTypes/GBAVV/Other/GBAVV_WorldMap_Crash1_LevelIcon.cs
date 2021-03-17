using UnityEngine;

namespace R1Engine
{
    public class GBAVV_WorldMap_Crash1_LevelIcon : R1Serializable
    {
        public Pointer PalettePointer { get; set; }
        public Pointer TileSetPointer { get; set; }

        // Serialized from pointers

        public GBAVV_PaletteBlock Palette { get; set; }
        public GBAVV_Map2D_TileSetBlock TileSet { get; set; }

        public Texture2D ToTexture2D() => Util.ToTileSetTexture(TileSet.TileSet, Util.ConvertGBAPalette(Palette.Palette), Util.TileEncoding.Linear_8bpp, 8, true, wrap: 8);

        public override void SerializeImpl(SerializerObject s)
        {
            PalettePointer = s.SerializePointer(PalettePointer, name: nameof(PalettePointer));
            TileSetPointer = s.SerializePointer(TileSetPointer, name: nameof(TileSetPointer));

            Palette = s.DoAt(PalettePointer, () => s.SerializeObject<GBAVV_PaletteBlock>(Palette, name: nameof(Palette)));
            TileSet = s.DoAt(TileSetPointer, () => s.SerializeObject<GBAVV_Map2D_TileSetBlock>(TileSet, name: nameof(TileSet)));
        }
    }
}