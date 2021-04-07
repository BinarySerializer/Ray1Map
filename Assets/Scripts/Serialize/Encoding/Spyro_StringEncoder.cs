using System;
using System.IO;
using BinarySerializer;

namespace R1Engine
{
    /// <summary>
    /// Compresses/decompresses data with Spyro's string compression algorithm
    /// </summary>
    public class Spyro_StringEncoder : IStreamEncoder {
        public string Name => "Spyro_StringEncoding";
        public GBAIsometric_Spyro_LocDecompress[] Helpers { get; set; }
        public Spyro_StringEncoder(GBAIsometric_Spyro_LocDecompress[] helpers) {
            Helpers = helpers;
        }

        public void DecompressString(Reader reader, byte[] outBuffer) {
            byte bits = 0;
            int curBit = -1;
            int curHelper = 0;
            int outPos = 0;
            while (outPos < outBuffer.Length) {
                if (curBit < 0) {
                    bits = reader.ReadByte();
                    curBit += 8;
                }
                if (BitHelpers.ExtractBits(bits, 1, curBit) == 0) {
                    if (BitHelpers.ExtractBits(Helpers[curHelper].b0, 1, 0) == 1) {
                        outBuffer[outPos++] = (byte)(Helpers[curHelper].b1);
                        curHelper = 0;
                    } else {
                        curHelper = Helpers[curHelper].b1;
                    }
                } else {
                    if (BitHelpers.ExtractBits(Helpers[curHelper].b0, 1, 1) == 1) {
                        outBuffer[outPos++] = (byte)(Helpers[curHelper].b2);
                        curHelper = 0;
                    } else {
                        curHelper = Helpers[curHelper].b2;
                    }
                }
                curBit--;
            }
        }

        /// <summary>
        /// Decodes the data and returns it in a stream
        /// </summary>
        /// <param name="s">The encoded stream</param>
        /// <returns>The stream with the decoded data</returns>
        public Stream DecodeStream(Stream s) {
            long streamPos = s.Position;
            Reader reader = new Reader(s, isLittleEndian: true);
            byte length = reader.ReadByte();
            byte[] decompressed = new byte[length];

            if (length > 0) {
                DecompressString(reader, decompressed);
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