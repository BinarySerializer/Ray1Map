using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_RHR_AnimPattern : BinarySerializable
    {
        public byte WidthPower { get; set; }
        public byte HeightPower { get; set; }
        public byte XPosition { get; set; }
        public byte YPosition { get; set; }
        public byte Unknown { get; set; }
        public bool IsLastPattern { get; set; }

        public int NumTiles => Width * Height;
        public int Width => 1 << WidthPower;
        public int Height => 1 << HeightPower;

        public override void SerializeImpl(SerializerObject s)
        {
            s.DoBits<ushort>(b => {
                WidthPower = (byte)b.SerializeBits<int>(WidthPower, 2, name: nameof(WidthPower));
                HeightPower = (byte)b.SerializeBits<int>(HeightPower, 2, name: nameof(HeightPower));
                XPosition = (byte)b.SerializeBits<int>(XPosition, 5, name: nameof(XPosition));
                YPosition = (byte)b.SerializeBits<int>(YPosition, 5, name: nameof(YPosition));
                Unknown = (byte)b.SerializeBits<int>(Unknown, 1, name: nameof(Unknown));
                IsLastPattern = b.SerializeBits<int>(IsLastPattern ? 1 : 0, 1, name: nameof(IsLastPattern)) == 1;
            });
        }
    }
}