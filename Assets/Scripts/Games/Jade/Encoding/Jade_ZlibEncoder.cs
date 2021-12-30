using Ionic.Zlib;
using System;
using System.IO;
using BinarySerializer;
using System.Collections.Generic;
using System.Linq;

namespace Ray1Map.Jade
{
    /// <summary>
    /// Compresses/decompresses data using Zlib
    /// </summary>
    public class Jade_ZlibEncoder : IStreamEncoder 
    {
        public Jade_ZlibEncoder(uint decompressedLength) 
        {
            DecompressedLength = decompressedLength;
        }

        public string Name => "Jade_Zlib";
        protected uint DecompressedLength { get; }

        public void DecodeStream(Stream input, Stream output) 
        {
            using Reader reader = new Reader(input, isLittleEndian: true, leaveOpen: true);
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

            output.Write(decompressedData, 0, decompressedData.Length);
        }

        public void EncodeStream(Stream input, Stream output) => throw new NotImplementedException();
    }
}