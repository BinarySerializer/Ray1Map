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
    public class RHR_SpriteEncoder : IStreamEncoder
    {
        public enum CombineMode {
            UseSprite,
            UseLookupBufferDirectly,
            UseLookupBufferDirectly_SingleTile
        }
        public CombineMode Mode { get; }
        public GBAIsometric_RHR_Sprite Sprite { get; }
        public bool Is8Bit { get; }
        public byte[] LookupBuffer { get; }
        public Pointer CompressedDataPointer { get; }
        public int? TileIndex { get; set; }
        public RHR_SpriteEncoder(GBAIsometric_RHR_Sprite sprite) {
            Sprite = sprite;
            Mode = CombineMode.UseSprite;
        }
        public RHR_SpriteEncoder(bool is8bit, byte[] lookupBuffer, Pointer compressedData) {
            LookupBuffer = lookupBuffer;
            Is8Bit = is8bit;
            Mode = CombineMode.UseLookupBufferDirectly;
            CompressedDataPointer = compressedData;
        }
        public RHR_SpriteEncoder(bool is8bit, byte[] lookupBuffer, Pointer compressedData, int tileIndex) {
            LookupBuffer = lookupBuffer;
            Is8Bit = is8bit;
            Mode = CombineMode.UseLookupBufferDirectly_SingleTile;
            CompressedDataPointer = compressedData;
            TileIndex = tileIndex;
        }
        private uint Rotate(uint bits, int places) {
            while(places < 0) places += 32;
            places %= 32;
            return bits >> places | (bits << (32-places));
        }
        public void ReadTileLine_4Bit_4Bytes(Reader reader, byte[] outArray, int outPos, ref uint carryParam) {
            void Write(uint toWrite) {
                outArray[outPos] = (byte)(toWrite & 0xFF);
                outArray[outPos + 1] = (byte)((toWrite >> 8) & 0xFF);
                outArray[outPos + 2] = (byte)((toWrite >> 16) & 0xFF);
                outArray[outPos + 3] = (byte)((toWrite >> 24) & 0xFF);
            }
            bool IsNegative(uint uintToCheck) {
                return (uintToCheck & 0x80000000) != 0;
            }

            uint R12 = 0;
            uint R4 = 0;
            int bytesRead = 0;
            uint R3 = reader.ReadByte();
            R3 = Rotate(R3, 9);
            for (int i = 0; i < 8; i++) {
                R3 = Rotate(R3, -1);
                if (!IsNegative(R3)) {
                    if (bytesRead % 2 == 0) {
                        R4 = reader.ReadByte();
                        carryParam = R4 >> 4;
                    } else {
                        carryParam = R4 & 0xF;
                    }
                    bytesRead++;
                }
                R12 = carryParam | Rotate(R12, 4);
            }
            R12 = Rotate(R12, 4);
            Write(R12);
            if (bytesRead % 2 != 0) {
                carryParam = R4 & 0xF;
            }
            /*
             
            first step
            followed by 7x mid step
            followed by 1x last step

            ---- init
            R12 = 0
            R3 = ReadByte();
            R3 = Rotate(R3, 9);
            R3 = Rotate(R3, -1);
            if(R3 >= 0) {
	            R4 = ReadByte();
	            PARAM_3 = R4 >> 4;
	            switch to other path
            }

            ---- path 0: mid step


            R12 = PARAM_3 | Rotate(R12,4)
            R3 = Rotate(R3, -1);
            if(R3 >= 0) {
	            R4 = ReadByte();
	            PARAM_3 = R4 >> 4;
	            switch to other path
            }

            ---- path 0: last step
  

            R12 = PARAM_3 | Rotate(R12,4)
            R12 = Rotate(R12,4)
            Write(R12)



            ---- path 1: mid step 
            orr        r12 ,param_3 ,r12 , ror #0x4
            movs       r3,r3, ror #0x1f
            andpl      param_3 ,r4,#0xf
            bpl        LAB_03004d3c

            R12 = PARAM_3 | Rotate(R12,4)
            R3 = Rotate(R3, -1);
            if(R3 >= 0) {
	            PARAM_3 = R4 & 0xF;
	            switch to other path
            }

            ---- path 1: last step 
            orr        r12 ,param_3 ,r12 , ror #0x4
            mov        r12 ,r12 , ror #0x4
            str        r12 ,[outBuf ],#0x4
            and        param_3 ,r4,#0xf
            bx         lr

            R12 = PARAM_3 | Rotate(R12,4)
            R12 = Rotate(R12,4)
            Write(R12)
            PARAM_3 = R4 & 0xF;
             */
        }

        public void ReadTile4Bit(Reader reader, long position, byte[] outArray, int outPos) {
            reader.BaseStream.Position = position;
            uint carryParam = 0;
            for (int i = 0; i < 8; i++) {
                ReadTileLine_4Bit_4Bytes(reader, outArray, outPos, ref carryParam);
                outPos += 4;
            }

        }

        public void ReadTileLine_8Bit(Reader reader, byte[] outArray, int outPos, ref uint carryParam, ref uint carryParam2) {
            void Write(uint toWrite) {
                outArray[outPos] = (byte)(toWrite & 0xFF);
                outArray[outPos + 1] = (byte)((toWrite >> 8) & 0xFF);
                outArray[outPos + 2] = (byte)((toWrite >> 16) & 0xFF);
                outArray[outPos + 3] = (byte)((toWrite >> 24) & 0xFF);
            }
            bool IsNegative(uint uintToCheck) {
                return (uintToCheck & 0x80000000) != 0;
            }

            uint R12 = 0;
            carryParam2 = Rotate(carryParam2, -1);
            if (!IsNegative(carryParam2)) carryParam = reader.ReadByte();

            carryParam2 = Rotate(carryParam2, -1);
            R12 = R12 | carryParam;
            if (!IsNegative(carryParam2)) carryParam = reader.ReadByte();

            carryParam2 = Rotate(carryParam2, -1);
            R12 = R12 | (carryParam << 8);
            if (!IsNegative(carryParam2)) carryParam = reader.ReadByte();

            carryParam2 = Rotate(carryParam2, -1);
            R12 = R12 | (carryParam << 16);
            if (!IsNegative(carryParam2)) carryParam = reader.ReadByte();
            R12 = R12 | (carryParam << 24);
            Write(R12);
        }

        public void ReadTile8it(Reader reader, long position, byte[] outArray, int outPos) {
            reader.BaseStream.Position = position;
            uint carryParam = 0;
            for (int i = 0; i < 8; i++) {
                uint R3 = reader.ReadByte();
                R3 = Rotate(R3, 9);
                ReadTileLine_8Bit(reader, outArray, outPos, ref carryParam, ref R3);
                ReadTileLine_8Bit(reader, outArray, outPos + 4, ref carryParam, ref R3);
                outPos += 8;
            }
            //UnityEngine.Debug.Log(reader.BaseStream.Position);
        }

        /// <summary>
        /// Decodes the data and returns it in a stream
        /// </summary>
        /// <param name="s">The encoded stream</param>
        /// <returns>The stream with the decoded data</returns>
        public Stream DecodeStream(Stream s) {
            long streamPos = s.Position;
            byte[] decompressed = null;
            Reader reader = new Reader(s, isLittleEndian: true);
            if (Mode == CombineMode.UseSprite) {
                int tileSize = Sprite.Is8Bit ? 0x40 : 0x20;
                decompressed = new byte[tileSize * Sprite.LookupBufferPositions.Length];
                uint h = Sprite.CanvasHeight;
                uint w = Sprite.CanvasWidth;
                long compressedDataOffset = Sprite.GraphicsDataPointer?.Value?.CompressedDataPointer?.FileOffset ?? 0;
                for (int y = 0; y < h; y++) {
                    for (int x = 0; x < w; x++) {
                        ushort pos = Sprite.LookupBufferPositions[y * w + x];

                        if (pos == 0) {
                            // Empty tile
                        } else {
                            byte[] lookupBuffer = Sprite.GraphicsDataPointer.Value.CompressionLookupBuffer;
                            int tilePos = lookupBuffer[pos] + pos * 0x10;
                            if (Sprite.Is8Bit) {
                                ReadTile8it(reader, compressedDataOffset + tilePos, decompressed, (int)(y * w + x) * tileSize);
                            } else {
                                ReadTile4Bit(reader, compressedDataOffset + tilePos, decompressed, (int)(y * w + x) * tileSize);
                            }
                        }
                    }
                }
            } else if(Mode == CombineMode.UseLookupBufferDirectly) {
                int tileSize = Is8Bit ? 0x40 : 0x20;
                decompressed = new byte[tileSize * LookupBuffer.Length];
                long compressedDataOffset = CompressedDataPointer?.FileOffset ?? 0;
                for (int i = 0; i < LookupBuffer.Length; i++) {
                    int pos = i;
                    if (pos == 0) {
                        // Empty tile
                    } else {
                        int tilePos = LookupBuffer[pos] + pos * 0x10;
                        if (Is8Bit) {
                            ReadTile8it(reader, compressedDataOffset + tilePos, decompressed, i * tileSize);
                        } else {
                            ReadTile4Bit(reader, compressedDataOffset + tilePos, decompressed, i * tileSize);
                        }
                    }
                }
            } else if (Mode == CombineMode.UseLookupBufferDirectly_SingleTile) {
                int tileSize = Is8Bit ? 0x40 : 0x20;
                decompressed = new byte[tileSize];
                long compressedDataOffset = CompressedDataPointer?.FileOffset ?? 0;
                int pos = TileIndex.Value;
                if (pos == 0) {
                    // Empty tile
                } else {
                    int tilePos = LookupBuffer[pos] + pos * 0x10;
                    if (Is8Bit) {
                        ReadTile8it(reader, compressedDataOffset + tilePos, decompressed, 0);
                    } else {
                        ReadTile4Bit(reader, compressedDataOffset + tilePos, decompressed, 0);
                    }
                }
            }
            // Reset stream position
            reader.BaseStream.Position = streamPos;
            
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