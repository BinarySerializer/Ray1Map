using BinarySerializer;
using System.IO;
using System.IO.Compression;

namespace Ray1Map
{
    /// <summary>
    /// Compresses/decompresses data using Gzip
    /// </summary>
    public class GzipEncoder : IStreamEncoder
    {
        public string Name => "Gzip";

        public void DecodeStream(Stream input, Stream output)
        {
            using var gZipStream = new GZipStream(input, CompressionMode.Decompress, leaveOpen: true);
            gZipStream.CopyTo(output);
        }

        public void EncodeStream(Stream input, Stream output)
        {
            using var gZipStream = new GZipStream(input, CompressionMode.Compress, leaveOpen: true);
            gZipStream.CopyTo(output);
        }
    }
}