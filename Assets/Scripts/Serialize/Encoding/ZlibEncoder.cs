using Ionic.Zlib;
using System;
using System.IO;

namespace R1Engine {
    /// <summary>
    /// Compresses/decompresses data using Zlib
    /// </summary>
    public class ZlibEncoder : IStreamEncoder {
        public ZlibEncoder(uint length, uint decompressedLength) {
            Length = length;
            DecompressedLength = decompressedLength;
        }
        protected uint Length { get; }
        protected uint DecompressedLength { get; }

        /// <summary>
        /// Decodes the data and returns it in a stream
        /// </summary>
        /// <param name="s">The encoded stream</param>
        /// <returns>The stream with the decoded data</returns>
        public Stream DecodeStream(Stream s) {
            var memStream = new MemoryStream();

            Reader reader = new Reader(s, isLittleEndian: false);
            byte[] bytes = reader.ReadBytes((int)Length);

            using (var zlibStream = new ZlibStream(new MemoryStream(bytes), CompressionMode.Decompress))
                zlibStream.CopyTo(memStream);

            // Set the position to the beginning
            memStream.Position = 0;

            // Return the compressed data stream
            return memStream;
        }

        public Stream EncodeStream(Stream s) {
            var memStream = new MemoryStream();

            using (var compressionStream = new ZlibStream(memStream, CompressionMode.Compress)) {
                s.CopyTo(compressionStream);
            }
            memStream.Position = 0;
            return memStream;
        }
    }
}