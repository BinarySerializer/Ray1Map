using BinarySerializer;

namespace Ray1Map.GBAKlonoa
{
    public class GBAKlonoa_EOD_TileSets : BinarySerializable
    {
        public bool Pre_SerializeData { get; set; }

        public Pointer[] TileSetPointers { get; set; }
        public byte[][] TileSets { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            TileSetPointers = s.SerializePointerArray(TileSetPointers, 3, name: nameof(TileSetPointers));

            if (!Pre_SerializeData)
                return;

            TileSets ??= new byte[TileSetPointers.Length][];

            for (int i = 0; i < TileSets.Length; i++)
            {
                s.DoAt(TileSetPointers[i], () =>
                {
                    s.DoEncoded(new GBAKlonoa_EOD_Encoder(), () =>
                    {
                        TileSets[i] = s.SerializeArray<byte>(TileSets[i], s.CurrentLength, name: $"{nameof(TileSets)}[{i}]");
                    });
                });
            }
        }
    }
}