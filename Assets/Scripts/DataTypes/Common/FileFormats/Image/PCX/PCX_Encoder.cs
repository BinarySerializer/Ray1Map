using System;
using System.IO;
using BinarySerializer;

namespace R1Engine
{
    public class PCX_Encoder : IStreamEncoder
    {
        public PCX_Encoder(int decodedLength)
        {
            DecodedLength = decodedLength;
        }

        public int DecodedLength { get; }
        public string Name => $"PCX_RLE";

        public Stream DecodeStream(Stream s)
        {
            using var reader = new Reader(s, leaveOpen: true);

            // Keep track of the index
            int index = 0;

            // Create the buffer
            byte[] buffer = new byte[DecodedLength];

            do
            {
                // Read the byte
                var b = reader.ReadByte();

                int repeatCount;
                byte runValue;

                // Check if it should be repeated
                if ((b & 0xC0) == 0xC0)
                {
                    repeatCount = b & 0x3F;
                    runValue = reader.ReadByte();
                }
                else
                {
                    repeatCount = 1;
                    runValue = b;
                }

                // Write the specified number of bytes
                while (index < buffer.Length && repeatCount > 0)
                {
                    buffer[index] = runValue;
                    repeatCount--;
                    index++;
                }

            } while (index < buffer.Length);

            return new MemoryStream(buffer);
        }

        public Stream EncodeStream(Stream s)
        {
            throw new NotImplementedException();
        }
    }
}