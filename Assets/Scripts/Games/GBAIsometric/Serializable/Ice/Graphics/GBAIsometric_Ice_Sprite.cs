using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_Ice_Sprite : BinarySerializable
    {
        public ushort TileIndex { get; set; } // Index to start reading tiles from

        public byte Height { get; set; } // Half the height in tiles, rounded down
        public byte Width { get; set; } // // Half the width in tiles, rounded down
        public byte SpriteSize { get; set; }

        public sbyte XPos { get; set; }
        public sbyte YPos { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            TileIndex = s.Serialize<ushort>(TileIndex, name: nameof(TileIndex));
            s.DoBits<byte>(b =>
            {
                Height = b.SerializeBits<byte>(Height, 3, name: nameof(Height));
                Width = b.SerializeBits<byte>(Width, 3, name: nameof(Width));
                SpriteSize = b.SerializeBits<byte>(SpriteSize, 2, name: nameof(SpriteSize));
            });
            XPos = s.Serialize<sbyte>(XPos, name: nameof(XPos));
            YPos = s.Serialize<sbyte>(YPos, name: nameof(YPos));
        }
    }
}