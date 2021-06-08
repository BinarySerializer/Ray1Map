using System;
using System.Collections.Generic;
using System.IO;
using BinarySerializer;

namespace R1Engine
{
    /// <summary>
    /// Compresses/decompresses data with GEN's hybrid RLE-based sprite compression algorithm
    /// </summary>
    public class GEN_RLXEncoder : IStreamEncoder {
        public string Name => "GEN_RLXEncoding";
        public GEN_RLXData RLX { get; set; }

        public GEN_RLXEncoder() {}

        public GEN_RLXEncoder(GEN_RLXData rlx) {
            RLX = rlx;
        }

        private void Decompress_2(byte[] compressed, byte[] decompressed, ref int inPos, ref int outPos, ref int toDecompress) {
            while (toDecompress > 0) {
                byte curByte = compressed[inPos++];
                toDecompress--;
                if ((curByte & 0x80) != 0) {
                    int compressedCount = BitHelpers.ExtractBits(curByte, 7, 0) + 1;
                    byte repeatByte = compressed[inPos++];
                    for (int i = 0; i < compressedCount; i++) {
                        decompressed[outPos + i] = repeatByte;
                    }
                    outPos += compressedCount;
                    toDecompress--;
                } else {
                    decompressed[outPos++] = curByte;
                }
            }
        }

        private byte[] Decompress(byte[] compressed) {
            byte[] decompressed = new byte[RLX.Width * RLX.Height];
            int inPos = 0, outPos = 0;
            int toDecompress = compressed.Length;
            switch (RLX.RLXType) {
                case 2:
                    Decompress_2(compressed, decompressed, ref inPos, ref outPos, ref toDecompress);
                    break;
                default:
                    return compressed;
            }
            return decompressed;
        }

        /// <summary>
        /// Decodes the data and returns it in a stream
        /// </summary>
        /// <param name="s">The encoded stream</param>
        /// <returns>The stream with the decoded data</returns>
        public Stream DecodeStream(Stream s) {
            Reader reader = new Reader(s, isLittleEndian: true);
            byte[] compressed = reader.ReadBytes((int)RLX.DataLength);
            byte[] decompressed = Decompress(compressed);
            
            var decompressedStream = new MemoryStream(decompressed);

            // Set position back to 0
            decompressedStream.Position = 0;

            // Return the compressed data stream
            return decompressedStream;
        }

        public Stream EncodeStream(Stream s) {
            throw new NotImplementedException();
        }
    }
}