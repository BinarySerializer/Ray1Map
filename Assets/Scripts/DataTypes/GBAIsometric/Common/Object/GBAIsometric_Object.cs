namespace R1Engine
{
    public class GBAIsometric_Object : R1Serializable
    {
        public short XPosition { get; set; }
        public short YPosition { get; set; }
        public short Height { get; set; }

        // Spyro
        public bool Value1 { get; set; }
        public bool HorizontalFlip { get; set; }
        public bool Value2 { get; set; }
        public int Value3 { get; set; }
        public bool IsNormalObj { get; set; } // False if it's a waypoint

        // RHR
        public ushort ObjectType { get; set; }
        public short WaypointIndex { get; set; }
        public byte WaypointCount { get; set; }
        public byte LinkIndex { get; set; } // 0xFF if not linked
        public byte[] Data { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if (s.GameSettings.EngineVersion == EngineVersion.GBAIsometric_RHR)
            {
                ObjectType = s.Serialize<ushort>(ObjectType, name: nameof(ObjectType));
            }
            else
            {
                s.SerializeBitValues<ushort>(bitFunc =>
                {
                    ObjectType = (ushort)bitFunc(ObjectType, 10, name: nameof(ObjectType));
                    Value1 = bitFunc(Value1 ? 1 : 0, 1, name: nameof(Value1)) == 1;
                    HorizontalFlip = bitFunc(HorizontalFlip ? 1 : 0, 1, name: nameof(HorizontalFlip)) == 1;
                    Value2 = bitFunc(Value2 ? 1 : 0, 1, name: nameof(Value2)) == 1;
                    Value3 = bitFunc(Value3, 2, name: nameof(Value3));
                    IsNormalObj = bitFunc(IsNormalObj ? 1 : 0, 1, name: nameof(IsNormalObj)) == 1;
                });
            }

            XPosition = s.Serialize<short>(XPosition, name: nameof(XPosition));
            YPosition = s.Serialize<short>(YPosition, name: nameof(YPosition));
            Height = s.Serialize<short>(Height, name: nameof(Height));

            if (s.GameSettings.EngineVersion == EngineVersion.GBAIsometric_RHR)
            {
                WaypointIndex = s.Serialize<short>(WaypointIndex, name: nameof(WaypointIndex));
                WaypointCount = s.Serialize<byte>(WaypointCount, name: nameof(WaypointCount));
                LinkIndex = s.Serialize<byte>(LinkIndex, name: nameof(LinkIndex));
                Data = s.SerializeArray<byte>(Data, 4, name: nameof(Data));
            }
            else
            {
                LinkIndex = 0xFF;
            }
        }
    }
}