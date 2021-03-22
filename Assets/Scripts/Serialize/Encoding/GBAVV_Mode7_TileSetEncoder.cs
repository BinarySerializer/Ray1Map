using System;
using System.IO;

namespace R1Engine
{
    public class GBAVV_Mode7_TileSetEncoder : IStreamEncoder
    {
        public GBAVV_Mode7_TileSetEncoder(long decodedLength)
        {
            DecodedLength = decodedLength;
        }

        public long DecodedLength { get; }

        public Stream DecodeStream(Stream s)
        {
            var decodedStream = new MemoryStream(new byte[DecodedLength]);
            var reader = new Reader(s);

            var initialPaddingSize = reader.ReadUInt16();

            // Skip padding
            decodedStream.Position += initialPaddingSize * 2;

            while (decodedStream.Position < DecodedLength)
            {
                // Read the data size
                var blockSize = reader.ReadUInt16();

                // Read the data and write to the decoded stream
                decodedStream.Write(reader.ReadBytes(blockSize * 2), 0, blockSize * 2);

                if (decodedStream.Position >= DecodedLength)
                    break;

                // Read padding
                var paddingSize = reader.ReadUInt16();

                // Skip padding
                decodedStream.Position += paddingSize * 2;
            }

            decodedStream.Position = 0;
            return decodedStream;
        }

        public Stream EncodeStream(Stream s) => throw new NotImplementedException();
    }
}