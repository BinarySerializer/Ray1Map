using System;
using System.IO;

namespace BinarySerializer.KlonoaDTP
{
    public class ULZ_Encoder : IStreamEncoder
    {
        public const uint Header = 0x1A7A6C55;

        public string Name => $"ULZ_Encoder";

        // Implemented from https://subversion.assembla.com/svn/transprojects/psx/ace_combat_3/tools/code/ULZ.cs
        public Stream DecodeStream(Stream s)
        {
            // Create a reader for the input
            var reader = new Reader(s, isLittleEndian: true);

            // All offsets are relative to the initial position
            var initialPos = reader.BaseStream.Position;

            // Create a stream to store the decompressed data
            var decompressedStream = new MemoryStream();

            // Read the header
            var header = reader.ReadUInt32();

            // Validate the header
            if (header != Header) // ULZ
                throw new Exception($"Invalid ULZ header");

            // Read the first two values
            var v1 = reader.ReadInt32();
            var v2 = reader.ReadInt32();

            // Parse the values
            var decompressedSize = BitHelpers.ExtractBits(v1, 24, 0);
            var ulzType = BitHelpers.ExtractBits(v1, 8, 24);
            var decompressedOffset = BitHelpers.ExtractBits(v2, 24, 0);
            var numBits = BitHelpers.ExtractBits(v2, 8, 24);

            ushort unk3 = (ushort)((1 << numBits) + 0xFFFF);
            
            var compressedOffset = reader.ReadUInt32();
            var flagsPosition = reader.BaseStream.Position;

            int flags = reader.ReadInt32();
            flagsPosition += 4;

            var decompSize = decompressedSize;

            if (ulzType == 0)
            {
                bool is_comp = (0 < flags);

                while (true)
                {
                    flags <<= 1;
                    if (flags == 0)
                    {
                        reader.BaseStream.Seek(flagsPosition, SeekOrigin.Begin);
                        flags = reader.ReadInt32();

                        if (flags == 0)
                            break;

                        flagsPosition += 4;
                        is_comp = (0 < flags);
                        flags <<= 1;
                    }

                    if (is_comp)
                    {
                        reader.BaseStream.Seek(initialPos + compressedOffset, SeekOrigin.Begin);
                        ushort c = reader.ReadUInt16();
                        compressedOffset += 2;

                        uint pos = (uint)(c & unk3);
                        pos += 1;

                        uint run = (uint)(c >> numBits);
                        run += 3;

                        long t_pos = decompressedStream.Position;

                        for (int j = 0; j < run; j++)
                            decompressedStream.WriteByte(decompressedStream.GetBuffer()[(t_pos - pos) + j]);
                    }
                    else
                    {
                        reader.BaseStream.Seek(initialPos + decompressedOffset, SeekOrigin.Begin);
                        byte u = reader.ReadByte();
                        decompressedStream.WriteByte(u);
                        decompressedOffset++;
                    }

                    is_comp = (0 < flags);

                }
            }
            else if (ulzType == 2)
            {
                int flag_pos = 0x20;

                while (true)
                {
                    flag_pos -= 1;

                    if (flag_pos < 0)
                    {
                        reader.BaseStream.Seek(flagsPosition, SeekOrigin.Begin);
                        flags = reader.ReadInt32();
                        flagsPosition += 4;
                        flag_pos = 0x1F;
                    }

                    bool is_comp = (0 <= flags);
                    flags <<= 1;

                    if (is_comp)
                    {
                        reader.BaseStream.Seek(initialPos + compressedOffset, SeekOrigin.Begin);
                        ushort c = reader.ReadUInt16();
                        compressedOffset += 2;

                        uint pos = (uint)(c & unk3);
                        pos += 1;

                        uint run = (uint)(c >> numBits);
                        run += 3;
                        decompSize -= (int)run;

                        long t_pos = decompressedStream.Position;

                        for (int j = 0; j < run; j++)
                            decompressedStream.WriteByte(decompressedStream.GetBuffer()[(t_pos - pos) + j]);
                    }
                    else
                    {
                        reader.BaseStream.Seek(initialPos + decompressedOffset, SeekOrigin.Begin);
                        byte u = reader.ReadByte();
                        decompressedStream.WriteByte(u);
                        decompressedOffset++;

                        decompSize--;
                    }

                    if (decompSize <= 0)
                        break;
                }
            }
            else
            {
                throw new Exception($"ULZ type {ulzType} is not supported");
            }

            // Go to the end of the compressed data
            reader.BaseStream.Seek(initialPos + compressedOffset, SeekOrigin.Begin);

            // Set position back to 0
            decompressedStream.Position = 0;

            // Return the compressed data stream
            return decompressedStream;
        }

        public Stream EncodeStream(Stream s)
        {
            throw new System.NotImplementedException();
        }
    }
}