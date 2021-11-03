using System;
using System.Collections.Generic;
using System.IO;
using BinarySerializer;

namespace Ray1Map.GEN
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

        private void Decompress_1(byte[] compressed, byte[] decompressed, ref int inPos, ref int outPos, ref int toDecompress) {
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
                    int literalCount = curByte + 1;
                    for (int i = 0; i < literalCount; i++) {
                        decompressed[outPos + i] = compressed[inPos++];
                    }
                    outPos += literalCount;
                    toDecompress -=  literalCount;
                }
            }
        }

        private void Decompress_2(byte[] compressed, byte[] decompressed, ref int inPos, ref int outPos, ref int toDecompress) {
            while (toDecompress > 0) {
                byte curByte = compressed[inPos++];
                toDecompress--;
                if ((curByte & 0x80) != 0) {
                    int compressedCount = BitHelpers.ExtractBits(curByte, 7, 0) + 1;
                    byte repeatByte = compressed[inPos++];
                    if(RLX.LookupTable != null && repeatByte < RLX.LookupTableCount) repeatByte = RLX.LookupTable[repeatByte];
                    for (int i = 0; i < compressedCount; i++) {
                        decompressed[outPos + i] = repeatByte;
                    }
                    outPos += compressedCount;
                    toDecompress--;
                } else {
                    if (RLX.LookupTable != null && curByte < RLX.LookupTableCount) curByte = RLX.LookupTable[curByte];
                    decompressed[outPos++] = curByte;
                }
            }
        }

        private void Decompress_3(byte[] compressed, byte[] decompressed, ref int inPos, ref int outPos, ref int toDecompress) {
            while (toDecompress > 0) {
                byte curByte = compressed[inPos++];
                toDecompress--;
                if ((curByte & 0x80) != 0) {
                    int compressedCount = ((BitHelpers.ExtractBits(curByte, 7, 0) + 1) * 2 + 1) >> 1;
                    byte bytesToWrite = compressed[inPos++];
                    byte byte1 = (byte)BitHelpers.ExtractBits(bytesToWrite, 4, 4);
                    byte byte2 = (byte)BitHelpers.ExtractBits(bytesToWrite, 4, 0);
                    if (RLX.LookupTable != null && byte1 < RLX.LookupTableCount) byte1 = RLX.LookupTable[byte1];
                    if (RLX.LookupTable != null && byte2 < RLX.LookupTableCount) byte2 = RLX.LookupTable[byte2];
                    toDecompress--;
                    for (int i = 0; i < compressedCount; i++) {
                        decompressed[outPos++] = byte1;
                        decompressed[outPos++] = byte2;
                    }
                } else {
                    int compressedCount = ((curByte + 1) * 2 + 1) >> 1;
                    for (int i = 0; i < compressedCount; i++) {
                        byte bytesToWrite = compressed[inPos++];
                        byte byte1 = (byte)BitHelpers.ExtractBits(bytesToWrite, 4, 4);
                        byte byte2 = (byte)BitHelpers.ExtractBits(bytesToWrite, 4, 0);
                        if (RLX.LookupTable != null && byte1 < RLX.LookupTableCount) byte1 = RLX.LookupTable[byte1];
                        if (RLX.LookupTable != null && byte2 < RLX.LookupTableCount) byte2 = RLX.LookupTable[byte2];
                        decompressed[outPos++] = byte1;
                        decompressed[outPos++] = byte2;
                        toDecompress--;
                    }
                }
            }
        }

        private void Decompress_4(byte[] compressed, byte[] decompressed, ref int inPos, ref int outPos, ref int toDecompress) {
            while (toDecompress > 0) {
                byte curByte = compressed[inPos++];
                toDecompress--;

                for (int i = 0; i < 8; i++) {
                    byte toWrite = (byte)BitHelpers.ExtractBits(curByte, 1, 7-i);
                    if (RLX.LookupTable != null && toWrite < RLX.LookupTableCount) toWrite = RLX.LookupTable[toWrite];
                    decompressed[outPos++] = toWrite;
                    if(outPos >= decompressed.Length) break;
                }
            }
        }
        
        private void Decompress_5(byte[] compressed, byte[] decompressed, ref int inPos, ref int outPos, ref int toDecompress) {
            while (toDecompress > 0) {
                byte curByte = compressed[inPos++];
                toDecompress--;

                byte toWrite = (byte)BitHelpers.ExtractBits(curByte, 1, 0);
                if (RLX.LookupTable != null && toWrite < RLX.LookupTableCount) toWrite = RLX.LookupTable[toWrite];

                int count = BitHelpers.ExtractBits(curByte, 7, 1) + 1;

                for (int i = 0; i < count; i++) {
                    decompressed[outPos++] = toWrite;
                }
            }
        }

        private void Decompress_6(byte[] compressed, byte[] decompressed, ref int inPos, ref int outPos, ref int toDecompress) {
            while (toDecompress > 0) {
                byte curByte = compressed[inPos++];
                toDecompress--;

                for (int i = 0; i < 4; i++) {
                    byte toWrite = (byte)BitHelpers.ExtractBits(curByte, 1, 6 - (i * 2));
                    if (RLX.LookupTable != null && toWrite < RLX.LookupTableCount) toWrite = RLX.LookupTable[toWrite];
                    decompressed[outPos++] = toWrite;
                    if (outPos >= decompressed.Length) break;
                }
            }
        }

        private byte[] Decompress(byte[] compressed) {
            byte[] decompressed = new byte[RLX.Width * RLX.Height];
            int inPos = 0, outPos = 0;
            int toDecompress = compressed.Length;
            switch (RLX.RLXType) {
                case 1:
                    Decompress_1(compressed, decompressed, ref inPos, ref outPos, ref toDecompress);
                    break;
                case 2:
                    Decompress_2(compressed, decompressed, ref inPos, ref outPos, ref toDecompress);
                    break;
                case 3:
                    Decompress_3(compressed, decompressed, ref inPos, ref outPos, ref toDecompress);
                    break;
                case 4:
                    Decompress_4(compressed, decompressed, ref inPos, ref outPos, ref toDecompress);
                    break;
                case 5:
                    Decompress_5(compressed, decompressed, ref inPos, ref outPos, ref toDecompress);
                    break;
                case 6:
                    Decompress_6(compressed, decompressed, ref inPos, ref outPos, ref toDecompress);
                    break;
                default:
                    return compressed;
            }
            if (outPos < decompressed.Length) {
                UnityEngine.Debug.LogWarning($"{RLX.Offset}: RLX Type {RLX.RLXType} did not fully decompress");
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