using lzo.net;
using System;
using System.IO;
using System.IO.Compression;

namespace R1Engine {
    /// <summary>
    /// Compresses/decompresses data using Gzip
    /// </summary>
    public class Lzo1xEncoder : IStreamEncoder
    {
        public uint CompressedSize { get; }
        public uint DecompressedSize { get; }
        public Lzo1xEncoder(uint compressedSize, uint decompressedSize) {
            CompressedSize = compressedSize;
            DecompressedSize = decompressedSize;
        }

        /// <summary>
        /// Decodes the data and returns it in a stream
        /// </summary>
        /// <param name="s">The encoded stream</param>
        /// <returns>The stream with the decoded data</returns>
        public Stream DecodeStream(Stream s) {

            Reader reader = new Reader(s, isLittleEndian: false);
            byte[] compressedData = reader.ReadBytes((int)CompressedSize);
            byte[] decompressedData;
            using (var compressedStream = new MemoryStream(compressedData))
            using (var lzo = new LzoStream(compressedStream, CompressionMode.Decompress))
            using (Reader lzoReader = new Reader(lzo, isLittleEndian: false)) {
                lzo.SetLength(DecompressedSize);
                decompressedData = lzoReader.ReadBytes((int)DecompressedSize);
            }

            var memStream = new MemoryStream(decompressedData);
            // Set the position to the beginning
            memStream.Position = 0;

            // Return the compressed data stream
            return memStream;
        }

        public Stream EncodeStream(Stream s) {
            throw new NotImplementedException();
        }
    }
}