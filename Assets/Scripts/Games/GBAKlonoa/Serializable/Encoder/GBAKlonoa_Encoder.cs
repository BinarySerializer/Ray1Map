using System.IO;
using BinarySerializer;
using BinarySerializer.Nintendo.GBA;

namespace Ray1Map.GBAKlonoa
{
    public class GBAKlonoa_Encoder : IStreamEncoder
    {
        public GBAKlonoa_Encoder(bool isDct)
        {
            IsDCT = isDct;
        }

        public bool IsDCT { get; }

        public string Name => $"GBAKlonoa_Encoding";

        public void DecodeStream(Stream input, Stream output)
        {
            using var reader = new Reader(input, leaveOpen: true);

            int lengthValue = reader.ReadInt32();
            int length = BitHelpers.ExtractBits(lengthValue, IsDCT ? 30 : 31, 0);

            Stream decompStream = null;

            try
            {
                // Huffman + LZSS
                if (lengthValue < 0)
                {
                    var huff = new HuffmanEncoder();
                    var lzss = new BinarySerializer.Nintendo.GBA.LZSSEncoder();

                    Stream decodedHuff = new MemoryStream();
                    huff.DecodeStream(input, decodedHuff);
                    decodedHuff.Position = 0;
                    
                    decompStream = new MemoryStream();
                    lzss.DecodeStream(decodedHuff, decompStream);
                    decompStream.Position = 0;
                }
                else
                {
                    // Not compressed
                    if (IsDCT && (lengthValue & 0x40000000) == 0)
                    {
                        var buffer = new byte[length];

                        input.Read(buffer, 0, length);
                        output.Write(buffer, 0, buffer.Length);

                        return;
                    }
                    // LZSS
                    else
                    {
                        var lzss = new BinarySerializer.Nintendo.GBA.LZSSEncoder();

                        decompStream = new MemoryStream();
                        lzss.DecodeStream(input, decompStream);
                        decompStream.Position = 0;
                    }
                }

                //if (decompStream.Length != length)
                //    Debug.LogWarning($"Incorrect size for decompressed data! {decompStream.Length} != {length}");

                // First 4 bytes are irrelevant
                decompStream.Position += 4;
                decompStream.CopyTo(output);
            }
            finally
            {
                decompStream?.Dispose();
            }
        }

        public void EncodeStream(Stream input, Stream output)
        {
            throw new System.NotImplementedException();
        }
    }
}