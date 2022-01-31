using BinarySerializer;
using BinarySerializer.GBA;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_Ice_Sparx_LevelData : BinarySerializable
    {
        public bool Pre_Resolve { get; set; }

        public Pointer<GBAIsometric_Ice_Sparx_MapLayer>[] Maps { get; set; } // 1, 0, 2 (3 is hard-coded as FLASH)
        public Pointer<GBAIsometric_Ice_Sparx_MapLayer> ObjectMap { get; set; } // Objects and collision
        public Pointer<GBAIsometric_Ice_Sparx_TileSet> TileSet { get; set; }
        public Pointer<GBAIsometric_Ice_Sparx_TileSetMap> TileSetMap { get; set; } // Each tile defines in maps consists of 2x2 tiles
        public Pointer<Palette> Palette { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // The padding is unused pointers (this format seems to be taken from another game, not sure which one)
            Maps = s.SerializePointerArray<GBAIsometric_Ice_Sparx_MapLayer>(Maps, 3, resolve: Pre_Resolve, name: nameof(Maps));
            s.SerializePadding(4);
            ObjectMap = s.SerializePointer<GBAIsometric_Ice_Sparx_MapLayer>(ObjectMap, resolve: Pre_Resolve, name: nameof(ObjectMap));
            TileSet = s.SerializePointer<GBAIsometric_Ice_Sparx_TileSet>(TileSet, resolve: Pre_Resolve, name: nameof(TileSet));
            s.SerializePadding(4);
            TileSetMap = s.SerializePointer<GBAIsometric_Ice_Sparx_TileSetMap>(TileSetMap, resolve: Pre_Resolve, name: nameof(TileSetMap));
            s.SerializePadding(4);
            Palette = s.SerializePointer<Palette>(Palette, onPreSerialize: x => x.Pre_Is8Bit = true, resolve: Pre_Resolve, name: nameof(Palette));
        }
    }
}