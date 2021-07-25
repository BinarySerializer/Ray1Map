using Ionic.Zlib;
using System;
using System.IO;
using BinarySerializer;
using System.Collections.Generic;
using System.Linq;

namespace R1Engine {
    /// <summary>
    /// Compresses/decompresses data using Zlib
    /// </summary>
    public class Jade_ZlibEncoder : IStreamEncoder {
        public string Name => "Jade_Zlib";
        public Jade_ZlibEncoder(uint decompressedLength) {
            DecompressedLength = decompressedLength;
        }
        protected uint DecompressedLength { get; }

        /// <summary>
        /// Decodes the data and returns it in a stream
        /// </summary>
        /// <param name="s">The encoded stream</param>
        /// <returns>The stream with the decoded data</returns>
        public Stream DecodeStream(Stream s) {
            Reader reader = new Reader(s, isLittleEndian: true);
            byte decompressedChunkSizeBits = reader.ReadByte();
            uint decompressedChunkSize = (uint)1 << decompressedChunkSizeBits;
            uint chunksCount = DecompressedLength / decompressedChunkSize;
            if(DecompressedLength % decompressedChunkSize != 0) chunksCount++;
            uint[] chunkEndOffsets = new uint[chunksCount];
            uint[] chunkSizes = new uint[chunksCount];
            for (int i = 0; i < chunksCount; i++) chunkEndOffsets[i] = reader.ReadUInt32();
            chunkSizes[0] = chunkEndOffsets[0];
            for(int i = 1; i < chunksCount; i++) chunkSizes[i] = chunkEndOffsets[i] - chunkEndOffsets[i-1];

            List<byte[]> decompressedBlocks = new List<byte[]>();
            uint totalSizeLeft = DecompressedLength;
            for (int i = 0; i < chunkSizes.Length; i++) {
                byte[] bytes = reader.ReadBytes((int)chunkSizes[i]);
                byte[] decompressedBlock;
                uint bytesToRead = Math.Min(totalSizeLeft, decompressedChunkSize);
                if (bytesToRead == bytes.Length) {
                    decompressedBlock = bytes;
                } else {
                    using (var zlibStream = new ZlibStream(new MemoryStream(bytes), CompressionMode.Decompress))
                    using (Reader zlibReader = new Reader(zlibStream, isLittleEndian: true)) {
                        decompressedBlock = zlibReader.ReadBytes((int)bytesToRead);
                    }
                    totalSizeLeft -= bytesToRead;
                }
                decompressedBlocks.Add(decompressedBlock);
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