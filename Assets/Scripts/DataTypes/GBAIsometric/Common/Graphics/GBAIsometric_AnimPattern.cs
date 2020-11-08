namespace R1Engine
{
    public class GBAIsometric_AnimPattern : R1Serializable
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

        public byte[] Spyro_Bytes { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if (s.GameSettings.EngineVersion == EngineVersion.GBAIsometric_RHR)
            {
                s.SerializeBitValues<ushort>(bitFunc => {
                    WidthPower = (byte)bitFunc(WidthPower, 2, name: nameof(WidthPower));
                    HeightPower = (byte)bitFunc(HeightPower, 2, name: nameof(HeightPower));
                    XPosition = (byte)bitFunc(XPosition, 5, name: nameof(XPosition));
                    YPosition = (byte)bitFunc(YPosition, 5, name: nameof(YPosition));
                    Unknown = (byte)bitFunc(Unknown, 1, name: nameof(Unknown));
                    IsLastPattern = bitFunc(IsLastPattern ? 1 : 0, 1, name: nameof(IsLastPattern)) == 1;
                });
            }
            else
            {
                Spyro_Bytes = s.SerializeArray<byte>(Spyro_Bytes, 16, name: nameof(Spyro_Bytes));
            }
        }
    }
}