using lzo.net;

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using BinarySerializer;

namespace R1Engine {
    /// <summary>
    /// Compresses/decompresses data using Gzip
    /// </summary>
    public class Jade_Lzo1xEncoder : IStreamEncoder
    {
        public uint TotalCompressedSize { get; }
        public bool Xbox360Version { get; }
        public Endian Endianness { get; }
        public Jade_Lzo1xEncoder(uint compressedSize, bool xbox360Version, Endian endianness = Endian.Little) {
            TotalCompressedSize = compressedSize;
            Xbox360Version = xbox360Version;
            Endianness = endianness;
        }

        /// <summary>
        /// Decodes the data and returns it in a stream
        /// </summary>
        /// <param name="s">The encoded stream</param>
        /// <returns>The stream with the decoded data</returns>
        public Stream DecodeStream(Stream s) {

            Reader reader = new Reader(s, isLittleEndian: Endianness == Endian.Little);
            var currentPos = reader.BaseStream.Position;
            var lastSizeRead = 0xFFFFFFFF;
            List<byte[]> decompressedBlocks = new List<byte[]>();
            while (reader.BaseStream.Position + 8 < currentPos + TotalCompressedSize && lastSizeRead > 0) {
                var size = reader.ReadUInt32();
                lastSizeRead = size;
                if (size != 0) {
                    var zSize = reader.ReadUInt32();
                    byte[] compressedBlock = reader.ReadBytes((int)zSize);
                    if (zSize == size) {
                        decompressedBlocks.Add(compressedBlock);
                    } else {
                        byte[] decompressedBlock;
                        using (var compressedStream = new MemoryStream(compressedBlock))
                        using (var lzo = new LzoStream(compressedStream, CompressionMode.Decompress))
                        using (Reader lzoReader = new Reader(lzo, isLittleEndian: Endianness == Endian.Little)) {
                            lzo.SetLength(size);
                            decompressedBlock = lzoReader.ReadBytes((int)size);
                        }
                        decompressedBlocks.Add(decompressedBlock);
                    }
                    if (Xbox360Version) {
                        reader.ReadUInt32();
                    }
                }
            }

            byte[] decompressedData = new byte[decompressedBlocks.Sum(d => d.Length)];
            int curPos = 0;
            foreach (var d in decompressedBlocks) {
                Array.Copy(d, 0, decompressedData, curPos, d.Length);
                curPos += d.Length;
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