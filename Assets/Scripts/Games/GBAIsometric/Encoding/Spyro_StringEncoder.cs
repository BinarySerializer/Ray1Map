using System;
using System.IO;
using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    /// <summary>
    /// Compresses/decompresses data with Spyro's string compression algorithm
    /// </summary>
    public class Spyro_StringEncoder : IStreamEncoder
    {
        public Spyro_StringEncoder(GBAIsometric_Spyro_LocDecompress[] helpers)
        {
            Helpers = helpers;
        }

        public string Name => "Spyro_StringEncoding";
        public GBAIsometric_Spyro_LocDecompress[] Helpers { get; }

        public void DecompressString(Reader reader, byte[] outBuffer)
        {
            byte bits = 0;
            int curBit = -1;
            int curHelper = 0;
            int outPos = 0;

            while (outPos < outBuffer.Length)
            {
                if (curBit < 0)
                {
                    bits = reader.ReadByte();
                    curBit += 8;
                }

                if (BitHelpers.ExtractBits(bits, 1, curBit) == 0)
                {
                    if (BitHelpers.ExtractBits(Helpers[curHelper].b0, 1, 0) == 1)
                    {
                        outBuffer[outPos++] = (byte)(Helpers[curHelper].b1);
                        curHelper = 0;
                    }
                    else
                    {
                        curHelper = Helpers[curHelper].b1;
                    }
                }
                else
                {
                    if (BitHelpers.ExtractBits(Helpers[curHelper].b0, 1, 1) == 1)
                    {
                        outBuffer[outPos++] = (byte)(Helpers[curHelper].b2);
                        curHelper = 0;
                    }
                    else
                    {
                        curHelper = Helpers[curHelper].b2;
                    }
                }
                curBit--;
            }
        }

        public void DecodeStream(Stream input, Stream output)
        {
            using Reader reader = new Reader(input, isLittleEndian: true, leaveOpen: true);
            byte length = reader.ReadByte();
            byte[] decompressed = new byte[length];

            if (length > 0)
                DecompressString(reader, decompressed);

            output.Write(decompressed, 0, decompressed.Length);
        }

        public void EncodeStream(Stream input, Stream output)
        {
            throw new NotImplementedException();
        }
    }
}