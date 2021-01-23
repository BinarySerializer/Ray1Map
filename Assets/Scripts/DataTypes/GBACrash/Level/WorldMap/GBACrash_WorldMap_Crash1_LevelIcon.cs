using UnityEngine;

namespace R1Engine
{
    public class GBACrash_WorldMap_Crash1_LevelIcon : R1Serializable
    {
        public Pointer PalettePointer { get; set; }
        public Pointer TileSetPointer { get; set; }

        // Serialized from pointers

        public GBACrash_WorldMap_Palette Palette { get; set; }
        public GBACrash_TileSet TileSet { get; set; }

        public Texture2D ToTexture2D() => Util.ToTileSetTexture(TileSet.TileSet, Util.ConvertGBAPalette(Palette.Palette), Util.TileEncoding.Linear_8bpp, 8, true, wrap: 8);

        public override void SerializeImpl(SerializerObject s)
        {
            PalettePointer = s.SerializePointer(PalettePointer, name: nameof(PalettePointer));
            TileSetPointer = s.SerializePointer(TileSetPointer, name: nameof(TileSetPointer));

            Palette = s.DoAt(PalettePointer, () => s.SerializeObject<GBACrash_WorldMap_Palette>(Palette, name: nameof(Palette)));
            TileSet = s.DoAt(TileSetPointer, () => s.SerializeObject<GBACrash_TileSet>(TileSet, name: nameof(TileSet)));
        }
    }
}