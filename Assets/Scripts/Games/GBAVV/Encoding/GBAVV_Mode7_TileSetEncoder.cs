using System;
using System.IO;
using BinarySerializer;

namespace Ray1Map.GBAVV
{
    public class GBAVV_Mode7_TileSetEncoder : IStreamEncoder
    {
        public GBAVV_Mode7_TileSetEncoder(long decodedLength)
        {
            DecodedLength = decodedLength;
        }

        public string Name => "GBAVV_Mode7TileSetEncoding";

        public long DecodedLength { get; }

        public void DecodeStream(Stream input, Stream output)
        {
            using var reader = new Reader(input, leaveOpen: true);

            var initialPaddingSize = reader.ReadUInt16();

            // Skip padding
            output.Position += initialPaddingSize * 2;

            while (output.Position < DecodedLength)
            {
                // Read the data size
                var blockSize = reader.ReadUInt16();

                // Read the data and write to the decoded stream
                output.Write(reader.ReadBytes(blockSize * 2), 0, blockSize * 2);

                if (output.Position >= DecodedLength)
                    break;

                // Read padding
                var paddingSize = reader.ReadUInt16();

                // Skip padding
                output.Position += paddingSize * 2;
            }
        }

        public void EncodeStream(Stream input, Stream output) => throw new NotImplementedException();
    }
}