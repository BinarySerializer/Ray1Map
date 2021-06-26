using System.IO;
using BinarySerializer;
using BinarySerializer.GBA;
using UnityEngine;

namespace R1Engine
{
    public class GBAKlonoa_Encoder : IStreamEncoder
    {
        public string Name => $"GBAKlonoa_Encoder";

        public Stream DecodeStream(Stream s)
        {
            using var reader = new Reader(s, leaveOpen: true);

            var lengthValue = reader.ReadInt32();
            var length = BitHelpers.ExtractBits(lengthValue, 31, 0);

            Stream decompStream = null;

            try
            {
                if (lengthValue < 0)
                {
                    var huff = new GBA_Huffman4Encoder();
                    var lzss = new GBA_LZSSEncoder();

                    var decodedHuff = huff.DecodeStream(s);
                    decompStream = lzss.DecodeStream(decodedHuff);
                }
                else
                {
                    var lzss = new GBA_LZSSEncoder();

                    decompStream = lzss.DecodeStream(s);
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