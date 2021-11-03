using BinarySerializer;

namespace Ray1Map.GBARRR
{
    public class GBARRR_Tileset : BinarySerializable
    {
        public uint BlockSize { get; set; }
        public int PalLength { get; set; } = 256;

        public RGBA5551Color[] Palette { get; set; }
        public byte[] Data { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Palette = s.SerializeObjectArray<RGBA5551Color>(Palette, PalLength, name: nameof(Palette));
            Data = s.SerializeArray<byte>(Data, BlockSize - (PalLength * 2), name: nameof(Data)); // Always 0x40040 for normal tilemaps
        }
    }
}