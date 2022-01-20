using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_Ice_Level3D_MapCollisionBox : BinarySerializable
    {
        public UInt24 Height { get; set; } // Shift by 14
        public byte LinesDataLength { get; set; }
        public short MinX { get; set; }
        public short MinY { get; set; }
        public short MaxX { get; set; }
        public short MaxY { get; set; }

        // Parsed later and is only used if the data length is not 0. Always has 4 lines. If the box has this
        // then it seems it defined the layering rather than solid collision?
        public GBAIsometric_Ice_Level3D_MapCollisionLine[] Lines { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Height = s.Serialize<UInt24>(Height, name: nameof(Height));
            LinesDataLength = s.Serialize<byte>(LinesDataLength, name: nameof(LinesDataLength));
            MinX = s.Serialize<short>(MinX, name: nameof(MinX));
            MinY = s.Serialize<short>(MinY, name: nameof(MinY));
            MaxX = s.Serialize<short>(MaxX, name: nameof(MaxX));
            MaxY = s.Serialize<short>(MaxY, name: nameof(MaxY));

            s.Log($"Size: {MaxX - MinX}x{MaxY - MinY}");
        }
    }
}