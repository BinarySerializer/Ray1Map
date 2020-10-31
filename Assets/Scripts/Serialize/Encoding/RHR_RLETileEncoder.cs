using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.UI;
using UnityEngine;

namespace R1Engine {
    /// <summary>
    /// Compresses/decompresses data with RHR's sprite compression algorithm
    /// </summary>
    public class RHR_RLETileEncoder : IStreamEncoder
    {
        public enum RLEMode {
            RLE1,
            RLE2
        }
        public RLEMode Mode { get; }

        public RHR_RLETileEncoder(RLEMode mode) {
            Mode = mode;
        }

        public void RLE1_ReadHalf(Reader reader, byte[] outArray, ref int outPos, uint bits, ref ushort currentShort) {
            void Write(ushort toWrite, ref int curOutPos) {
                outArray[curOutPos] = (byte)(toWrite & 0xFF);
                outArray[curOutPos + 1] = (byte)((toWrite >> 8) & 0xFF);
                curOutPos += 2;
            }
            bool IsNegative(uint uintToCheck) {
                return (uintToCheck & 0x80000000) != 0;
            }
            uint curBits = bits;
            for (int i = 0; i < 0x20; i++) {
                if (!IsNegative(curBits)) {
                    currentShort = reader.ReadUInt16();
                }
                Write(currentShort, ref outPos);
                curBits <<= 1;
            }
        }
        public void RLE2_ReadHalf(Reader reader, byte[] outArray, ref int outPos, uint bits, ref ushort currentShort) {
            void Write(ushort toWrite, ref int curOutPos) {
                outArray[curOutPos] = (byte)(toWrite & 0xFF);
                outArray[curOutPos + 1] = (byte)((toWrite >> 8) & 0xFF);
                curOutPos += 16;
            }
            bool IsNegative(uint uintToCheck) {
                return (uintToCheck & 0x80000000) != 0;
            }
            uint curBits = bits;
            for (int i = 0; i < 0x20; i++) {
                if (i % 8 == 0 && i != 0) {
                    outPos -= 0x7e;
                }
                if (!IsNegative(curBits)) {
                    currentShort = reader.ReadUInt16();
                }
                Write(currentShort, ref outPos);
                curBits <<= 1;
            }
        }

        public void RLE1_Read(Reader reader, byte[] outArray, int outPos) {
            uint bits1 = reader.ReadUInt32();
            uint bits2 = reader.ReadUInt32();
            ushort currentShort = 0;
            int curOutPos = outPos;
            RLE1_ReadHalf(reader, outArray, ref curOutPos, bits1, ref currentShort);
            RLE1_ReadHalf(reader, outArray, ref curOutPos, bits2, ref currentShort);
        }

        public void RLE2_Read(Reader reader, byte[] outArray, int outPos) {
            uint bits1 = reader.ReadUInt32();
            uint bits2 = reader.ReadUInt32();
            ushort currentShort = 0;
            int curOutPos = outPos;
            RLE2_ReadHalf(reader, outArray, ref curOutPos, bits1, ref currentShort);
            curOutPos -= 0x7e;
            RLE2_ReadHalf(reader, outArray, ref curOutPos, bits2, ref currentShort);
        }

        /// <summary>
        /// Decodes the data and returns it in a stream
        /// </summary>
        /// <param name="s">The encoded stream</param>
        /// <returns>The stream with the decoded data</returns>
        public Stream DecodeStream(Stream s) {
            long streamPos = s.Position;
            byte[] decompressed = new byte[0x80];
            Reader reader = new Reader(s, isLittleEndian: true);
            if (Mode == RLEMode.RLE1) {
                RLE1_Read(reader, decompressed, 0);
            } else if(Mode == RLEMode.RLE2) {
                RLE2_Read(reader, decompressed, 0);
            }
            
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