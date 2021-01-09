using System;

namespace R1Engine
{
    public abstract class GBACrash_BaseBlock : R1Serializable
    {
        public BlockCompressionType Compression { get; set; }
        public int BlockLength { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize the header
            s.DoAt(s.CurrentPointer, () => s.SerializeBitValues<uint>(bitFunc =>
            {
                bitFunc(default, 4, name: "Padding");
                Compression = (BlockCompressionType)bitFunc((byte)Compression, 4, name: nameof(Compression));
                BlockLength = bitFunc(BlockLength, 24, name: nameof(BlockLength));
            }));

            IStreamEncoder encoder;

            switch (Compression)
            {
                case BlockCompressionType.None:
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