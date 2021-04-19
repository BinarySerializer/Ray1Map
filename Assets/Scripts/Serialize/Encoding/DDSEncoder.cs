using BinarySerializer;
using System;
using System.IO;

namespace R1Engine
{
    public class DDSEncoder : IStreamEncoder
    {
        public DDSEncoder(DDS_Header header)
        {
            Header = header;
        }

        public string Name => $"{FourCC}Encoding";
        public DDS_Header Header { get; }
        public string FourCC => Header.PixelFormat.FourCC;

        public Stream DecodeStream(Stream s)
        {
            return new MemoryStream(DDSParser.DecompressData(new Reader(s), Header));
        }

        public Stream EncodeStream(Stream s)
        {
            throw new NotImplementedException();
        }
    }
}