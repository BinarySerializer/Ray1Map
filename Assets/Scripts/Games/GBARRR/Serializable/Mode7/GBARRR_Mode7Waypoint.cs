using BinarySerializer;

namespace Ray1Map.GBARRR
{
    public class GBARRR_Mode7Waypoint : BinarySerializable
    {
        public short XPosition { get; set; }
        public short YPosition { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            XPosition = s.Serialize<short>(XPosition, name: nameof(XPosition));
            YPosition = s.Serialize<short>(YPosition, name: nameof(YPosition));
        }
    }
}