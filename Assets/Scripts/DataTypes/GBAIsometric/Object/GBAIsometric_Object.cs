namespace R1Engine
{
    public class GBAIsometric_Object : R1Serializable
    {
        public short ObjectType { get; set; }
        public short XPosition { get; set; }
        public short YPosition { get; set; }
        public short Height { get; set; }
        
        public short WaypointIndex { get; set; }
        public byte WaypointCount { get; set; }

        public byte LinkIndex { get; set; } // 0xFF if not linked

        public byte[] Data { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ObjectType = s.Serialize<short>(ObjectType, name: nameof(ObjectType));
            XPosition = s.Serialize<short>(XPosition, name: nameof(XPosition));
            YPosition = s.Serialize<short>(YPosition, name: nameof(YPosition));
            Height = s.Serialize<short>(Height, name: nameof(Height));
            WaypointIndex = s.Serialize<short>(WaypointIndex, name: nameof(WaypointIndex));
            WaypointCount = s.Serialize<byte>(WaypointCount, name: nameof(WaypointCount));
            LinkIndex = s.Serialize<byte>(LinkIndex, name: nameof(LinkIndex));
            Data = s.SerializeArray<byte>(Data, 4, name: nameof(Data));
        }
    }
}