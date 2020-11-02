namespace R1Engine
{
    public class GBAIsometric_RHR_Waypoint : R1Serializable
    {
        public int XPos { get; set; }
        public int YPos { get; set; }
        public int Height { get; set; }

        public float XPosValue
        {
            get => XPos / 256f;
            set => XPos = (int)(value * 256);
        }

        public float YPosValue
        {
            get => YPos / 256f;
            set => YPos = (int)(value * 256);
        }

        public float HeightValue
        {
            get => Height / 256f;
            set => Height = (int)(value * 256);
        }

        public override void SerializeImpl(SerializerObject s)
        {
            XPos = s.Serialize<int>(XPos, name: nameof(XPos));
            s.Log($"XPos value: {XPosValue}");
            YPos = s.Serialize<int>(YPos, name: nameof(YPos));
            s.Log($"YPos value: {YPosValue}");
            Height = s.Serialize<int>(Height, name: nameof(Height));
            s.Log($"Height value: {HeightValue}");
        }
    }
}