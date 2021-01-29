using System;
using System.IO;

namespace R1Engine
{
    public class GBAVV_Mode7_ObjFrame : R1Serializable
    {
        public byte Width { get; set; }
        public byte Height { get; set; }
        public FrameFlags Flags { get; set; }
        public byte[] TileSet { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Width = s.Serialize<byte>(Width, name: nameof(Width));
            Height = s.Serialize<byte>(Height, name: nameof(Height));
            Flags = s.Serialize<FrameFlags>(Flags, name: nameof(Flags));

            s.DoEncodedIf(new FrameTileSetEncoder(Width * Height * (Flags.HasFlag(FrameFlags.Is8bit) ? 0x40 : 0x20)), Flags.HasFlag(FrameFlags.IsCompressed), () => TileSet = s.SerializeArray<byte>(TileSet, Width * Height * 0x20, name: nameof(TileSet)));
        }

        [Flags]
        public enum FrameFlags : ushort
        {
            None = 0,
            Is8bit = 1 << 0, // Unused
            Flag_04 = 1 << 4,
            IsCompressed = 1 << 5,
        }

        public class FrameTileSetEncoder : IStreamEncoder
        {
            public FrameTileSetEncoder(long decodedLength)
            {
                DecodedLength = decodedLength;
            }

            public long DecodedLength { get; }

            public Stream DecodeStream(Stream s)
            {
                var decodedStream = new MemoryStream(new byte[DecodedLength]);
                var reader = new Reader(s);

                var initialPaddingSize = reader.ReadInt16();

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
}