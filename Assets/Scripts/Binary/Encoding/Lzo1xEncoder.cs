using lzo.net;
using System;
using System.IO;
using System.IO.Compression;
using BinarySerializer;

namespace Ray1Map 
{
    public class Lzo1xEncoder : IStreamEncoder
    {
        public Lzo1xEncoder(uint compressedSize, uint decompressedSize) 
        {
            CompressedSize = compressedSize;
            DecompressedSize = decompressedSize;
        }

        public string Name => "Lzo1x";

        public uint CompressedSize { get; }
        public uint DecompressedSize { get; }

        public void DecodeStream(Stream input, Stream output) 
        {
            using Reader reader = new Reader(input, isLittleEndian: false, leaveOpen: true);

            byte[] compressedData = reader.ReadBytes((int)CompressedSize);
            byte[] decompressedData;

            using (var compressedStream = new MemoryStream(compressedData))
            using (var lzo = new LzoStream(compressedStream, CompressionMode.Decompress))
            using (Reader lzoReader = new Reader(lzo, isLittleEndian: false)) 
            {
                lzo.SetLength(DecompressedSize);
                decompressedData = lzoReader.ReadBytes((int)DecompressedSize);
            }

            output.Write(decompressedData, 0, decompressedData.Length);
        }

        public void EncodeStream(Stream input, Stream output) 
        {
            throw new NotImplementedException();
        }
    }
}