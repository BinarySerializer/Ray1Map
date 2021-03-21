namespace R1Engine
{
    public class GBAVV_AnimationRect : R1Serializable
    {
        public short X { get; set; }
        public short Y { get; set; }
        public short Width { get; set; }
        public short Height { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            X = s.Serialize<short>(X, name: nameof(X));
            Y = s.Serialize<short>(Y, name: nameof(Y));

            if (s.GameSettings.EngineVersion == EngineVersion.GBAVV_Crash2 || 
                s.GameSettings.EngineVersion == EngineVersion.GBAVV_CrashNitroKart_NGage ||
                s.GameSettings.EngineVersion == EngineVersion.GBAVV_Madagascar ||
                s.GameSettings.EngineVersion >= EngineVersion.GBAVV_UltimateSpiderMan)
            {
                Width = s.Serialize<short>(Width, name: nameof(Width));
                Height = s.Serialize<short>(Height, name: nameof(Height));
            }
            else
            {
                Width = s.Serialize<byte>((byte)Width, name: nameof(Width));
                Height = s.Serialize<byte>((byte)Height, name: nameof(Height));
                s.Serialize<ushort>(default, name: "Padding");
            }
        }
    }
}