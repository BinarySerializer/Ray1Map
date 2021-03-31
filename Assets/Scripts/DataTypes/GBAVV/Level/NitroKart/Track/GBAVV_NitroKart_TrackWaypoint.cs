using BinarySerializer;

namespace R1Engine
{
    public class GBAVV_NitroKart_TrackWaypoint : BinarySerializable
    {
        public short XPos { get; set; }
        public short YPos { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            XPos = s.Serialize<short>(XPos, name: nameof(XPos));
            YPos = s.Serialize<short>(YPos, name: nameof(YPos));
        }
    }
}