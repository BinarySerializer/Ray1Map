using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    // A line, except for the first 3 values?
    public class GBAIsometric_Ice_Level3D_MapCollisionItem : BinarySerializable
    {
        public UInt24 Height { get; set; } // Shift by 14
        public byte ShapeLength { get; set; }
        public short MinX { get; set; }
        public short MinY { get; set; }
        public short MaxX { get; set; }
        public short MaxY { get; set; }

        // Parsed later
        public GBAIsometric_Ice_Level3D_MapCollisionShapeItem[] Shape { get; set; } // Optional 4 lines

        public override void SerializeImpl(SerializerObject s)
        {
            Height = s.Serialize<UInt24>(Height, name: nameof(Height));
            ShapeLength = s.Serialize<byte>(ShapeLength, name: nameof(ShapeLength));
            MinX = s.Serialize<short>(MinX, name: nameof(MinX));
            MinY = s.Serialize<short>(MinY, name: nameof(MinY));
            MaxX = s.Serialize<short>(MaxX, name: nameof(MaxX));
            MaxY = s.Serialize<short>(MaxY, name: nameof(MaxY));

            s.Log($"Size: {MaxX - MinX}x{MaxY - MinY}");
        }
    }
}