using System;
using System.IO;

namespace R1Engine
{
    public class BriefLZEncoder : IStreamEncoder
    {
        public Stream DecodeStream(Stream s)
        {
            // Create a reader for the input
            var reader = new Reader(s, isLittleEndian: true);

            // First four bytes have the decompressed size
            var decompressedSize = reader.ReadUInt32();

            // Create a buffer and stream to store the decompressed data
            var decompressedBuffer = new byte[decompressedSize];
            var decompressedStream = new MemoryStream(decompressedBuffer);
            
            // Create a state object
            State state = new State();

            // Keep track of the amount of data we have decompressed
            ulong currentlyDecompressedSize = 0;

            state.BitsLeft = 1;
            state.Tag = 0x4000;

            // Decompress until we've decompressed everything
            while (currentlyDecompressedSize < decompressedSize)
            {
                if (GetBit(state, reader)) 
                {
                    // Get match length and offset
                    ulong len = GetGamma(state, reader) + 2;
                    ulong off = GetGamma(state, reader) - 2;

                    off = (off << 8) + (ulong)reader.ReadByte() + 1;

                    // Get the initial offset to read from
                    var p = decompressedStream.Position - (long)off;

                    // Copy bytes
                    for (ulong i = len; i > 0; --i)
                        decompressedStream.WriteByte(decompressedBuffer[p++]);

                    currentlyDecompressedSize += len;
                }
                else
                {
                    // Copy byte
                    decompressedStream.WriteByte(reader.ReadByte());

                    currentlyDecompressedSize++;
                }
            }

            // Set position back to 0
            decompressedStream.Position = 0;

            // Return the compressed data stream
            return decompressedStream;
        }

        public Stream EncodeStream(Stream s) => throw new NotImplementedException();

        protected bool GetBit(State state, Reader reader)
        {
            // Check if the tag is empty
            if (state.BitsLeft-- == 0)
            {
                // Read the next tag
                state.Tag = (uint)reader.ReadByte() | ((uint)reader.ReadByte() << 8);
                state.BitsLeft = 15;
            }

            // Shift bit out of tag
            var bit = state.Tag & 0x8000;
            state.Tag <<= 1;

            return bit != 0;
        }
        protected ulong GetGamma(State state, Reader reader)
        {
            ulong result = 1;

            // Gamma2-encoded bits
            do
            {
                result = (result << 1) + (GetBit(state, reader) ? 1u : 0);
            } while (GetBit(state, reader));

            return result;
        }

        protected class State
        {
            public uint Tag { get; set; }
            public uint BitsLeft { get; set; }
        };
    }
}