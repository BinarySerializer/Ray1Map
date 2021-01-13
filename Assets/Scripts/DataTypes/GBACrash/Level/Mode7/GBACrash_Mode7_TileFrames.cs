using System.Collections.Generic;

namespace R1Engine
{
    public class GBACrash_Mode7_TileFrames : R1Serializable
    {
        public uint TileSetFramesBlockLength { get; set; } // Set before serializing

        public ushort Width { get; set; }
        public ushort Height { get; set; }

        public TileFrame[] TileFrames { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Width = s.Serialize<ushort>(Width, name: nameof(Width));
            Height = s.Serialize<ushort>(Height, name: nameof(Height));

            if (TileFrames == null)
            {
                var datas = new List<TileFrame>();
                var index = 0;
                long length = 4;

                do
                {
                    var frame = s.SerializeObject<TileFrame>(default, name: $"{nameof(TileFrames)}[{index++}]");
                    datas.Add(frame);
                    length += frame.DataLength + 4;
                } while (length < TileSetFramesBlockLength);

                TileFrames = datas.ToArray();
            }
            else
            {
                TileFrames = s.SerializeObjectArray<TileFrame>(TileFrames, TileFrames.Length, name: nameof(TileFrames));
            }
        }

        public class TileFrame : R1Serializable
        {
            public uint DataLength { get; set; }
            public byte[] Header { get; set; }
            public byte[] TileSet { get; set; } // Width * Height, 4bpp

            public override void SerializeImpl(SerializerObject s)
            {
                DataLength = s.Serialize<uint>(DataLength, name: nameof(DataLength));
                Header = s.SerializeArray<byte>(Header, 4, name: nameof(Header));
                TileSet = s.SerializeArray<byte>(TileSet, DataLength - 4, name: nameof(TileSet));
            }
        }
    }
}