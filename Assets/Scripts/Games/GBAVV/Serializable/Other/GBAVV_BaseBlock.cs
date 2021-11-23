using System;
using BinarySerializer;
using BinarySerializer.GBA;


namespace Ray1Map.GBAVV
{
    public abstract class GBAVV_BaseBlock : BinarySerializable
    {
        public bool HasHeader { get; set; } = true; // Set before serializing

        public BlockCompressionType Compression { get; set; }
        public int BlockLength { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize the header
            if (HasHeader)
            {
                s.DoAt(s.CurrentPointer, () => s.DoBits<uint>(b =>
                {
                    b.SerializeBits<int>(default, 4, name: "Padding");
                    Compression = (BlockCompressionType)b.SerializeBits<int>((byte)Compression, 4, name: nameof(Compression));
                    BlockLength = b.SerializeBits<int>(BlockLength, 24, name: nameof(BlockLength));
                }));
            }

            IStreamEncoder encoder;

            switch (Compression)
            {
                case BlockCompressionType.None:
                    if (HasHeader)
                        s.Goto(s.CurrentPointer + 4);
                    encoder = null;
                    break;

                case BlockCompressionType.LZSS:
                    encoder = new GBA_LZSSEncoder();
                    break;

                case BlockCompressionType.Huffman:
                    encoder = new GBA_Huffman4Encoder();
                    break;

                case BlockCompressionType.RL:
                    encoder = new GBA_RLEEncoder();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(Compression), Compression, null);
            }

            s.DoEncodedIf(encoder, encoder != null, () => SerializeBlock(s));
        }

        public abstract void SerializeBlock(SerializerObject s);

        public enum BlockCompressionType : byte
        {
            None = 0,
            LZSS = 1,
            Huffman = 2,
            RL = 3,
        }
    }
}