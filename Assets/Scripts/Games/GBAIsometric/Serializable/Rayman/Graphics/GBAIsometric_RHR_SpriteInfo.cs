﻿using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_RHR_SpriteInfo : BinarySerializable
    {
        public int Width { get; set; } // In 8x8 tiles
        public int Height { get; set; }
        public byte Byte_01 { get; set; }
        public byte Byte_02 { get; set; }

        public uint CanvasWidth => Util.NextPowerOfTwo((uint)Width);
        public uint CanvasHeight => Util.NextPowerOfTwo((uint)Height);

        public override void SerializeImpl(SerializerObject s)
        {
            s.DoBits<byte>(b => {
                Width = b.SerializeBits<int>(Width, 4, name: nameof(Width));
                Height = b.SerializeBits<int>(Height, 4, name: nameof(Height));
            });
            Byte_01 = s.Serialize<byte>(Byte_01, name: nameof(Byte_01));
            Byte_02 = s.Serialize<byte>(Byte_02, name: nameof(Byte_02));
        }
    }
}