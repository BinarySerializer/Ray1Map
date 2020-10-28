namespace R1Engine
{
    public class GBARRR_Mode7Object : R1Serializable
    {
        public short ObjectType { get; set; }
        public short XPosition { get; set; }
        public short YPosition { get; set; }
        public byte[] Data { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ObjectType = s.Serialize<short>(ObjectType, name: nameof(ObjectType));
            XPosition = s.Serialize<short>(XPosition, name: nameof(XPosition));
            YPosition = s.Serialize<short>(YPosition, name: nameof(YPosition));
            Data = s.SerializeArray<byte>(Data, 32 - (2 * 3), name: nameof(Data));
        }

        public enum Mode7Type
        {
            Lum = 4,
            SoccerBall = 18,
            Scenery_Cube = 29,
            Scenery_Pencil = 30
        }
    }
}