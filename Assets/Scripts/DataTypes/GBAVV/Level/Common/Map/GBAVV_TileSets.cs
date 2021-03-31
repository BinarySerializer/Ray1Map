using BinarySerializer;

namespace R1Engine
{
    public class GBAVV_TileSets : BinarySerializable
    {
        public Pointer TileSet8bppPointer { get; set; }
        public Pointer TileSet4bppPointer { get; set; }

        // Serialized from pointers
        public GBAVV_TileSet TileSet8bpp { get; set; }
        public GBAVV_TileSet TileSet4bpp { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            TileSet8bppPointer = s.SerializePointer(TileSet8bppPointer, name: nameof(TileSet8bppPointer));
            TileSet4bppPointer = s.SerializePointer(TileSet4bppPointer, name: nameof(TileSet4bppPointer));

            TileSet8bpp = s.DoAt(TileSet8bppPointer, () => s.SerializeObject<GBAVV_TileSet>(TileSet8bpp, name: nameof(TileSet8bpp)));
            TileSet4bpp = s.DoAt(TileSet4bppPointer, () => s.SerializeObject<GBAVV_TileSet>(TileSet4bpp, name: nameof(TileSet4bpp)));
        }
    }
}