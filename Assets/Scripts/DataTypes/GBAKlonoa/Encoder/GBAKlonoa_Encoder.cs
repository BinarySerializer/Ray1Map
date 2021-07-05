using System.IO;
using BinarySerializer;
using BinarySerializer.GBA;
using UnityEngine;

namespace R1Engine
{
    public class GBAKlonoa_Encoder : IStreamEncoder
    {
        public GBAKlonoa_Encoder(bool isDct)
        {
            IsDCT = isDct;
        }

        public bool IsDCT { get; }

        public string Name => $"GBAKlonoa_Encoder";

        public Stream DecodeStream(Stream s)
        {
            using var reader = new Reader(s, leaveOpen: true);

            var lengthValue = reader.ReadInt32();
            var length = BitHelpers.ExtractBits(lengthValue, IsDCT ? 30 : 31, 0);

            Stream decompStream = null;

            try
            {
                // Huffman + LZSS
                if (lengthValue < 0)
                {
                    var huff = new GBA_Huffman4Encoder();
                    var lzss = new GBA_LZSSEncoder();

                    var decodedHuff = huff.DecodeStream(s);
                    decompStream = lzss.DecodeStream(decodedHuff);
                }
                else
                {
                    // Not compressed
                    if (IsDCT && (lengthValue & 0x40000000) == 0)
                    {
                        var buffer = new byte[length];

                        s.Read(buffer, 0, length);

                        return new MemoryStream(buffer);
                    }
                    // LZSS
                    else
                    {
                        var lzss = new GBA_LZSSEncoder();

                        decompStream = lzss.DecodeStream(s);
                    }
                }

                if (decompStream.Length != length)
                    Debug.LogWarning($"Incorrect size for decompressed data! {decompStream.Length} != {length}");

                // First 4 bytes are irrelevant
                var returnStream = new MemoryStream((int)decompStream.Length - 4);

                decompStream.Position += 4;
                decompStream.CopyTo(returnStream);

                return returnStream;
            }
            finally
            {
                decompStream?.Dispose();
            }
        }

        public Stream EncodeStream(Stream s)
        {
            throw new System.NotImplementedException();
        }
    }
}