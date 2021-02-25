namespace R1Engine
{
    public class GBAVV_Map2D_AnimationRect : R1Serializable
    {
        public short X { get; set; }
        public short Y { get; set; }
        public short Width { get; set; }
        public short Height { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            X = s.Serialize<short>(X, name: nameof(X));
            Y = s.Serialize<short>(Y, name: nameof(Y));

            if (s.GameSettings.EngineVersion == EngineVersion.GBAVV_Crash1)
            {
                Width = s.Serialize<byte>((byte)Width, name: nameof(Width));
                Height = s.Serialize<byte>((byte)Height, name: nameof(Height));
                s.Serialize<ushort>(default, name: "Padding");
            }
            else if (s.GameSettings.EngineVersion == EngineVersion.GBAVV_Crash2)
            {
                Width = s.Serialize<short>(Width, name: nameof(Width));
                Height = s.Serialize<short>(Height, name: nameof(Height));
            }
            else if (s.GameSettings.GBAVV_IsFusion)
            {
                Width = s.Serialize<byte>((byte)Width, name: nameof(Width));
                Height = s.Serialize<byte>((byte)Height, name: nameof(Height));
            }
        }
    }
}