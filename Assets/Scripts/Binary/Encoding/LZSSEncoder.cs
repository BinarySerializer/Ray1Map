using System;
using System.IO;
using BinarySerializer;

namespace Ray1Map
{
    /// <summary>
    /// Compresses/decompresses data using LZSS
    /// </summary>
    public class LZSSEncoder : IStreamEncoder
    {
        public LZSSEncoder(uint length, bool hasHeader = true)
        {
            Length = length;
            HasHeader = hasHeader;
        }
     
        public string Name => "LZSS";

        protected uint Length { get; }
        protected bool HasHeader { get; }

        public void DecodeStream(Stream input, Stream output) 
        {
            using Reader reader = new Reader(input, isLittleEndian: true, leaveOpen: true);
            
            if (HasHeader) 
            {
                uint magic = reader.ReadUInt32();

                if (magic != 0x01234567)
                    throw new InvalidDataException("The data is not LZSS compressed!");
            }

            byte[] bytes = reader.ReadBytes((int)Length);

            using MemoryStream ms = new MemoryStream(bytes);
            LzssAlgorithm.Lzss.Decode(ms, output);
        }

        public void EncodeStream(Stream input, Stream output) => throw new NotImplementedException();
    }
}