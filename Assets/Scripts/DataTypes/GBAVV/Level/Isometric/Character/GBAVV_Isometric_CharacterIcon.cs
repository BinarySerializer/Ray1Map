using BinarySerializer;

namespace R1Engine
{
    public class GBAVV_Isometric_CharacterIcon : BinarySerializable
    {
        public Pointer TileSetPointer { get; set; }
        public uint PaletteIndex { get; set; }

        // Serialized from pointers

        public GBAVV_Map2D_TileSetBlock TileSet { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            TileSetPointer = s.SerializePointer(TileSetPointer, name: nameof(TileSetPointer));
            PaletteIndex = s.Serialize<uint>(PaletteIndex, name: nameof(PaletteIndex));

            TileSet = s.DoAt(TileSetPointer, () => s.SerializeObject<GBAVV_Map2D_TileSetBlock>(TileSet, name: nameof(TileSet)));
        }
    }
}