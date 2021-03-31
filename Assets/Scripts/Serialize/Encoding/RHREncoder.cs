using System;
using System.Collections.Generic;
using System.IO;
using BinarySerializer;

namespace R1Engine
{
    /// <summary>
    /// Compresses/decompresses data with RHR's hybrid compression algorithm
    /// </summary>
    public class RHREncoder : IStreamEncoder
    {
        public enum EncoderMode {
            Full,
            TileData
        }
        public EncoderMode Mode { get; set; } = EncoderMode.Full;
        public ushort TileCompressedSize { get; set; } = 0x80;
        public ushort TileDecompressedSize { get; set; } = 0x80;
        public ushort TileStep { get; set; } = 0x10; //00800010h
        public RHREncoder() {}

        public RHREncoder(EncoderMode mode, ushort comprSize = 0x80, ushort decomprSize = 0x80) {
            Mode = mode;
            TileCompressedSize = comprSize;
            TileDecompressedSize = decomprSize;
        }
        private byte[] TempBuffer { get; set; }
        private byte[] GetTempBuffer(int size) {
            if (TempBuffer == null || TempBuffer.Length < size) {
                return new byte[size];
            } else {
                return TempBuffer;
            }
        }
        private void SaveTempBuffer(byte[] buffer) {
            if(buffer != null) TempBuffer = buffer;
        }
        private ushort[] TempShortBuffer { get; set; }
        private ushort[] GetTempShortBuffer(int size) {
            if (TempShortBuffer == null || TempShortBuffer.Length < size) {
                return new ushort[size];
            } else {
                return TempShortBuffer;
            }
        }
        private void SaveTempShortBuffer(ushort[] buffer) {
            if (buffer != null) TempShortBuffer = buffer;
        }

        private void DecompressBlock_Copy(byte[] compressed, byte[] decompressed, ref int inPos, ref int outPos, ref int toDecompress) {
            Array.Copy(compressed, inPos, decompressed, outPos, toDecompress);
            inPos += toDecompress;
            outPos += toDecompress;
            toDecompress = 0;
        }

        private void DecompressBlock_RLE(byte[] compressed, byte[] decompressed, ref int inPos, ref int outPos, ref int toDecompress) {
            while (toDecompress > 0) {
                byte compressedCount = compressed[inPos++];
                byte uncompressedCount = compressed[inPos++];
                byte repeatByte = compressed[inPos++];
                for (int i = 0; i < compressedCount; i++) {
                    decompressed[outPos + i] = repeatByte;
                }
                outPos += compressedCount;
                for (int i = 0; i < uncompressedCount; i++) {
                    decompressed[outPos + i] = compressed[inPos++];
                }
                outPos += uncompressedCount;
                toDecompress -= compressedCount + uncompressedCount;
            }
        }

        private void DecompressBlock_Shorts(byte[] compressed, byte[] decompressed, ref int inPos, ref int outPos, ref int toDecompress) {
            using (Reader reader = new Reader(new MemoryStream(compressed))) {
                reader.BaseStream.Position = inPos;
                using (Writer writer = new Writer(new MemoryStream())) {
                    while (toDecompress > 0) {
                        ushort cmd = reader.ReadUInt16();
                        ushort toWrite = (ushort)BitHelpers.ExtractBits(cmd, 14, 0);
                        bool flag14 = BitHelpers.ExtractBits(cmd, 1, 14) == 1;
                        bool flag15 = BitHelpers.ExtractBits(cmd, 1, 15) == 1;
                        if (!flag15) {
                            if (flag14) {
                                writer.Write(toWrite);
                                toDecompress -= 2;
                            }
                            writer.Write(toWrite);
                            toDecompress -= 2;
                        } else {
                            if (!flag14) {
                                byte count = reader.ReadByte();
                                for (int i = 0; i < count; i++) {
                                    writer.Write(toWrite);
                                }
                                toDecompress -= 2*count;
                            } else {
                                writer.Write(toWrite);
                                writer.Write(toWrite);
                                writer.Write(toWrite);
                                toDecompress -= 6;
                            }
                        }
                    }
                    writer.BaseStream.Position = 0;
                    byte[] decompressed2 = (writer.BaseStream as MemoryStream).ToArray();
                    Array.Copy(decompressed2, 0, decompressed, outPos, decompressed2.Length);
                    outPos += decompressed2.Length;
                }
                inPos = (int)reader.BaseStream.Position;
            }
        }

        private void DecompressBlock_Bits(byte[] compressed, byte[] decompressed, ref int inPos, ref int outPos, ref int toDecompress) {
            uint commandOffset = (uint)(compressed[inPos] | (compressed[inPos+1] << 8));
            uint endOffset = (uint)(compressed[inPos+2] | (compressed[inPos+3] << 8));
            inPos += 4;
            int curCommandOffset = 0;
            byte curCommand = 0;
            uint startOffset = (uint)inPos;
            uint curOffset = 0;
            int curCommandBit = 0;
            void GetNewCommandBit(int bytesLeft) {
                if (curCommandBit == 0) {
                    curCommandBit = 7;
                    if (bytesLeft > 0) {
                        curCommand = compressed[startOffset + commandOffset + curCommandOffset++];
                    }
                } else {
                    curCommandBit--;
                }
            }
            GetNewCommandBit(toDecompress);
            while (toDecompress > 0) {
                if (BitHelpers.ExtractBits(curCommand,1,curCommandBit) == 1) {
                    curOffset += 2;
                }
                byte bVar1 = compressed[startOffset + curOffset + 1];
                if (bVar1 == 0) {
                    decompressed[outPos++] = compressed[startOffset + curOffset];
                    toDecompress--;
                    curOffset = 0;
                } else {
                    curOffset += (uint)bVar1 * 2;
                }
                GetNewCommandBit(toDecompress);
            }
            inPos = compressed.Length;
        }

        private void DecompressBlock_Window(byte[] compressed, byte[] decompressed, ref int inPos, ref int outPos, ref int toDecompress) {
            while (toDecompress > 0) {
                byte cmd = compressed[inPos++];
                if (BitHelpers.ExtractBits(cmd, 1, 7) == 0) {
                    if (cmd == 0 || 8 < cmd) {
                        //UnityEngine.Debug.Log($"1: td{toDecompress}: ip{inPos}, op{outPos}, cmd{cmd}");
                        decompressed[outPos++] = cmd;
                        toDecompress--;
                    } else {
                        //UnityEngine.Debug.Log($"2: td{toDecompress}: ip{inPos}, op{outPos}, cmd{cmd}");
                        for (int i = 0; i < cmd; i++) {
                            decompressed[outPos++] = compressed[inPos++];
                        }
                        toDecompress -= cmd;
                    }
                } else {
                    if (cmd < 0xc0) {
                        int bits = (cmd << 8) | compressed[inPos++];
                        int count = BitHelpers.ExtractBits(bits, 4, 0) + 3;
                        int lookBack = BitHelpers.ExtractBits(bits, 10, 4) + 1;
                        //UnityEngine.Debug.Log($"window lookback: td{toDecompress}: ip{inPos}, op{outPos}, lb{lookBack}, c{count}");
                        for (int i = 0; i < count; i++) {
                            decompressed[outPos] = decompressed[outPos-lookBack];
                            outPos++;
                        }
                        toDecompress-= count;
                    } else {
                        //UnityEngine.Debug.Log($"3: td{toDecompress}: ip{inPos}, op{outPos}, cmd{cmd}");
                        decompressed[outPos++] = 0x20;
                        decompressed[outPos++] = (byte)((cmd & 0x3f) + 0x41);
                        toDecompress -= 2;
                    }
                }
            }
        }

        private void DecompressBlock_Buffer(byte[] compressed, byte[] decompressed, ref int inPos, ref int outPos, ref int toDecompress) {
            while (toDecompress > 0) {
                int blockSz = Math.Min(0x800, toDecompress);
                // Fill temp buffer
                ushort[] buffer = new ushort[0x100];
                for (int i = 0; i < buffer.Length; i++) {
                    buffer[i] = (ushort)i;
                }
                byte fillBufferCount = compressed[inPos++];
                for (int i = 0; i < fillBufferCount; i++) {
                    byte location = compressed[inPos++];
                    byte msb = compressed[inPos++];
                    byte lsb = compressed[inPos++];
                    buffer[location] = (ushort)((msb << 8) | lsb);
                }
                int curSz = 0;
                Stack<byte> puVar6 = new Stack<byte>();
                //UnityEngine.Debug.Log($"1: td{toDecompress}: ip{inPos}, op{outPos}");
                while (curSz != blockSz) {
                    uint unk = compressed[inPos++];
                    //UnityEngine.Debug.Log($"2: td{toDecompress}: ip{inPos}, op{outPos}, unk{unk}");
                    while (true) {
                        while (buffer[unk] != unk) {
                            ushort buf = buffer[unk];
                            unk = (uint)BitHelpers.ExtractBits(buf, 8, 0);
                            puVar6.Push((byte)BitHelpers.ExtractBits(buf, 8, 8));
                        }
                        decompressed[outPos++] = (byte)unk;
                        //UnityEngine.Debug.Log($"3: td{toDecompress}: ip{inPos}, op{outPos}, unk{unk}");
                        curSz++;
                        if(puVar6.Count == 0) break;
                        unk = puVar6.Pop();
                    }
                }
                toDecompress -= blockSz;
            }
        }

        private void DecodeBlock_Sum_Byte(byte[] coded, int pos, int length) {
            uint sum = 0;
            for (int i = 0; i < length; i++) {
                sum = (sum + coded[pos + i]) % 0x100;
                coded[pos + i] = (byte)sum;
            }
        }
        private void DecodeBlock_Sum_Short(byte[] coded, int pos, int length) {
            uint sum = 0;
            for (int i = 0; i < length / 2; i++) {
                uint val = coded[pos + i * 2];
                if(pos + i * 2 + 1 < coded.Length) val += (uint)(coded[pos + i * 2 + 1] << 8);
                sum = (sum + val) % 0x10000;
                coded[pos + i*2] = (byte)(sum & 0xFF);
                if (pos + i * 2 + 1 < coded.Length) coded[pos + i*2 + 1] = (byte)(sum >> 8);
            }
        }
        private void DecodeBlock_Copy(byte[] coded, int pos, int length) {
            // Do nothing
        }
        private void DecodeBlock_Buffer(byte[] coded, int pos, int length) {
            // Fill temp buffer
            byte[] buffer = new byte[0x100];
            for (int i = 0; i < buffer.Length; i++) {
                buffer[i] = (byte)i;
            }
            int curByte = 0;
            for (int i = 0; i < length; i++) {
                byte inByte = coded[pos + i];
                int index = (inByte + curByte) & 0xFF;
                byte bufByte = buffer[index];
                coded[pos + i] = bufByte;
                if (inByte != 0) {
                    curByte = curByte > 0 ? curByte - 1 : 0xFF;
                    buffer[index] = buffer[curByte];
                    buffer[curByte] = bufByte;
                }
            }
        }

        private void DecodeBlock_TempBuffer_Short(byte[] coded, int pos, int length, uint startBufferIndex, byte[] previousBuffer, ushort[] previousShortBuffer) {
            // Create empty buffers
            short[] buffer = new short[0x100];
            ushort[] countBuffer = new ushort[0x100];

            for (int i = length - 1; i >= 0; i--) {
                short buf = buffer[previousBuffer[i]];
                // Loop around
                if (buf == short.MaxValue) {
                    buffer[previousBuffer[i]] = short.MinValue;
                } else {
                    buffer[previousBuffer[i]] = (short)(buffer[previousBuffer[i]] + 1);
                }
            }
            short curShort = 0;
            for (int i = 0; i < buffer.Length; i++) {
                short shortDelta = buffer[i];
                buffer[i] = curShort;
                int newShort = curShort + shortDelta;
                // Loop around
                if (newShort > short.MaxValue) {
                    newShort -= 0x10000;
                } else if (newShort < short.MinValue) {
                    newShort += 0x10000;
                }
                curShort = (short)newShort;
                //BitConverter.ToUInt16(BitConverter.GetBytes(buf), 0);
            }
            for (int i = 0; i < length; i++) {
                byte b = previousBuffer[i];
                ushort count = countBuffer[b];
                short s = buffer[b];
                countBuffer[b] = countBuffer[b] == 0xFFFF ? (ushort)0 : (ushort)(countBuffer[b] + 1);
                previousShortBuffer[count + (s >= 0 ? s : 0x10000+s)] = (ushort)i;
            }
            uint bufferIndex = startBufferIndex;
            for (int i = 0; i < length; i++) {
                coded[i] = previousBuffer[bufferIndex];
                bufferIndex = previousShortBuffer[bufferIndex];
            }
        }

        private void DecodeBlock_TempBuffer_Step(byte[] coded, int pos, int length, byte[] previousBuffer, ushort step) {
            if (step != length && step != 1) {
                int k = 0;
                for (int i = 0; i < step; i++) {
                    for (int j = i; j < length; j += step) {
                        coded[pos + j] = previousBuffer[k++];
                    }
                }
            }
        }

        private void DecodeBlock_TempBuffer_StepShort(byte[] coded, int pos, int length, byte[] previousBuffer, ushort step) {
            if (step != length && step != 2) {
                int k = 0;
                for (int i = 0; i < (step & 0xfffe); i+=2) {
                    for (int j = i; j < (length & 0xfffe); j += (step & 0xfffe)) {
                        coded[pos + j] = previousBuffer[k++];
                        coded[pos + j + 1] = previousBuffer[k++];
                    }
                }
            }
        }

        private byte[] ReadBlock(Reader reader, int decompressedBlockSize, ushort step) {
            reader.Align(4);
            bool log = false;
            //bool log = true;
            string logOffset = $"{reader.BaseStream.Position:X8}";
            ushort head = reader.ReadUInt16();
            byte unk0 = reader.ReadByte();
            byte unk1 = reader.ReadByte();
            int compressedSize = 0;
            if (Mode == EncoderMode.Full) {
                compressedSize = reader.ReadInt32();
            } else if (Mode == EncoderMode.TileData) {
                compressedSize = TileCompressedSize;
            }

            byte cmd = (byte)BitHelpers.ExtractBits(head, 7, 0);
            int byte2UpperNibble = BitHelpers.ExtractBits(head, 4, 12);
            int byte2LowerNibble = BitHelpers.ExtractBits(head, 4, 8);
            uint startBufferIndex = 0;
            /*if (byte2UpperNibble > 4) {
                throw new NotImplementedException();
            }*/
            if (byte2UpperNibble == 5) {
                startBufferIndex = reader.ReadUInt32();
                compressedSize -= 4;
            }
            byte[] compressed = reader.ReadBytes(compressedSize);
            int inPos = 0;
            int outPos = 0;
            int toDecompress = decompressedBlockSize;

            byte[] decompressed = new byte[decompressedBlockSize];
            byte[] tempBuffer = byte2UpperNibble > 4 ? GetTempBuffer(decompressed.Length) : null;
            byte[] targetBuffer = tempBuffer ?? decompressed;

            if (log) {
                Util.ByteArrayToFile(LevelEditorData.MainContext.BasePath + $"blocks/{logOffset}_{cmd}-{byte2LowerNibble}-{byte2UpperNibble}_compressed.bin", compressed);
            }
            try {
                switch (cmd) {
                    case 0:
                        // Uncompressed
                        DecompressBlock_Copy(compressed, targetBuffer, ref inPos, ref outPos, ref toDecompress);
                        break;
                    case 1:
                        // RLE
                        DecompressBlock_RLE(compressed, targetBuffer, ref inPos, ref outPos, ref toDecompress);
                        break;
                    case 2:
                        // Shorts
                        DecompressBlock_Shorts(compressed, targetBuffer, ref inPos, ref outPos, ref toDecompress);
                        break;
                    case 3:
                        // Bits
                        DecompressBlock_Bits(compressed, targetBuffer, ref inPos, ref outPos, ref toDecompress);
                        break;
                    case 4:
                        // Window
                        DecompressBlock_Window(compressed, targetBuffer, ref inPos, ref outPos, ref toDecompress);
                        break;
                    case 5:
                        // Block
                        DecompressBlock_Buffer(compressed, targetBuffer, ref inPos, ref outPos, ref toDecompress);
                        break;
                }
                if (log) {
                    Util.ByteArrayToFile(LevelEditorData.MainContext.BasePath + $"blocks/{logOffset}_{cmd}-{byte2LowerNibble}-{byte2UpperNibble}_after_1.bin", targetBuffer);
                }
                switch (byte2LowerNibble) {
                    case 1:
                        DecodeBlock_Sum_Byte(targetBuffer, 0, targetBuffer.Length);
                        break;
                    case 2:
                        DecodeBlock_Sum_Short(targetBuffer, 0, targetBuffer.Length);
                        break;
                    case 3:
                        DecodeBlock_Copy(targetBuffer, 0, targetBuffer.Length);
                        break;
                    case 4:
                        DecodeBlock_Buffer(targetBuffer, 0, targetBuffer.Length);
                        break;
                }
                if (log) {
                    Util.ByteArrayToFile(LevelEditorData.MainContext.BasePath + $"blocks/{logOffset}_{cmd}-{byte2LowerNibble}-{byte2UpperNibble}_after_2.bin", targetBuffer);
                }
                switch (byte2UpperNibble) {
                    case 1:
                        DecodeBlock_Sum_Byte(decompressed, 0, decompressed.Length);
                        break;
                    case 2:
                        DecodeBlock_Sum_Short(decompressed, 0, decompressed.Length);
                        break;
                    case 3:
                        DecodeBlock_Copy(decompressed, 0, decompressed.Length);
                        break;
                    case 4:
                        DecodeBlock_Buffer(decompressed, 0, decompressed.Length);
                        break;
                    case 5:
                        ushort[] tempShortBuffer = GetTempShortBuffer(decompressed.Length);
                        DecodeBlock_TempBuffer_Short(decompressed, 0, decompressed.Length, startBufferIndex, tempBuffer, tempShortBuffer);
                        SaveTempShortBuffer(tempShortBuffer);
                        break;
                    case 6:
                        DecodeBlock_TempBuffer_Step(decompressed, 0, decompressed.Length, tempBuffer, step);
                        break;
                    case 7:
                        DecodeBlock_TempBuffer_StepShort(decompressed, 0, decompressed.Length, tempBuffer, step);
                        break;
                }
                SaveTempBuffer(tempBuffer);
                if (log) {
                    Util.ByteArrayToFile(LevelEditorData.MainContext.BasePath + $"blocks/{logOffset}_{cmd}-{byte2LowerNibble}-{byte2UpperNibble}_after_3.bin", decompressed);
                }
            } catch (Exception ex) {
                if (log) {
                    Util.ByteArrayToFile(LevelEditorData.MainContext.BasePath + $"blocks/{logOffset}_{cmd}-{byte2LowerNibble}-{byte2UpperNibble}_crashedBlock.bin", decompressed);
                }
                throw new Exception($"{cmd}:{byte2LowerNibble}:{byte2UpperNibble} - {ex.Message}", ex);
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
            byte[] decompressed = null;
            if (Mode == EncoderMode.Full) {
                uint totalSize = reader.ReadUInt32();
                decompressed = new byte[totalSize];
                ushort step = reader.ReadUInt16();
                ushort blockSize = reader.ReadUInt16();
                uint bytesRead = 0;
                int curBlock = 0;
                while (bytesRead < totalSize) {
                    int toRead = (int)Math.Min(blockSize, totalSize - bytesRead);
                    byte[] decompressedBlock = ReadBlock(reader, toRead, step);
                    Array.Copy(decompressedBlock, 0, decompressed, bytesRead, decompressedBlock.Length);
                    bytesRead += (uint)decompressedBlock.Length;
                    curBlock++;
                }
            } else if (Mode == EncoderMode.TileData) {
                decompressed = ReadBlock(reader, TileDecompressedSize, TileStep);
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