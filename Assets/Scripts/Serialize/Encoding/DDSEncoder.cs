using BinarySerializer;
using System;
using System.IO;

namespace R1Engine
{
    public class DDSEncoder : IStreamEncoder
    {
        public DDSEncoder(DDS_Header header, uint width, uint height)
        {
            Header = header;
            Width = width;
            Height = height;
        }

        public string Name => $"{Header.PixelFormat.FourCC}Encoding";
        public DDS_Header Header { get; }
        public uint Width { get; }
        public uint Height { get; }

        public Stream DecodeStream(Stream s)
        {
            return new MemoryStream(DDSParser.DecompressData(new Reader(s), Header, Width, Height));
        }

        public Stream EncodeStream(Stream s)
        {
            throw new NotImplementedException();
        }
    }
}