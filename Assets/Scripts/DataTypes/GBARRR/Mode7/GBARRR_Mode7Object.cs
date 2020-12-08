namespace R1Engine
{
    public class GBARRR_Mode7Object : R1Serializable
    {
        public Mode7Type ObjectType { get; set; }
        public short XPosition { get; set; }
        public short YPosition { get; set; }
        public byte[] Data0 { get; set; }
        public short AnimFrame { get; set; }
        public byte[] Data1 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ObjectType = s.Serialize<Mode7Type>(ObjectType, name: nameof(ObjectType));
            if (ObjectType != Mode7Type.Invalid) {
                XPosition = s.Serialize<short>(XPosition, name: nameof(XPosition));
                YPosition = s.Serialize<short>(YPosition, name: nameof(YPosition));
                Data0 = s.SerializeArray<byte>(Data0, 10, name: nameof(Data0));
                AnimFrame = s.Serialize<short>(AnimFrame, name: nameof(AnimFrame));
                Data1 = s.SerializeArray<byte>(Data1, 14, name: nameof(Data1));
            }
        }

        public enum Mode7Type : short
        {
            Lum = 4,
            SoccerBall = 18,
            Scenery_Cube = 29,
            Scenery_Pencil = 30,
            
            Invalid = 0xFE,
            Hidden = 0xFF
        }
    }
}