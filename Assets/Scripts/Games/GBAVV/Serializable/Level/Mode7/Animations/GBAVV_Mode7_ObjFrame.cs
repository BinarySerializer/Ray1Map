using System;
using BinarySerializer;

namespace Ray1Map.GBAVV
{
    public class GBAVV_Mode7_ObjFrame : BinarySerializable
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

            s.DoEncodedIf(new GBAVV_Mode7_TileSetEncoder(Width * Height * (Flags.HasFlag(FrameFlags.Is8bit) ? 0x40 : 0x20)), Flags.HasFlag(FrameFlags.IsCompressed), () => TileSet = s.SerializeArray<byte>(TileSet, Width * Height * 0x20, name: nameof(TileSet)));
        }

        [Flags]
        public enum FrameFlags : ushort
        {
            None = 0,
            Is8bit = 1 << 0, // Unused
            Flag_04 = 1 << 4,
            IsCompressed = 1 << 5,
        }
    }
}