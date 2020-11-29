namespace R1Engine
{
    /// <summary>
    /// A standard ARGB color wrapper with serializing support for the encoding RGB-556
    /// </summary>
    public class RGB556Color : ARGBColor
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public RGB556Color() : base(255, 0, 0, 0)
        { }

        public RGB556Color(byte red, byte green, byte blue) : base(red, green, blue)
        { }

        public RGB556Color(byte alpha, byte red, byte green, byte blue) : base(alpha, red, green, blue)
        { }

        /// <summary>
        /// The color as a raw 16-bit value
        /// </summary>
        public ushort Color565 {
            get {
                ushort val = 0;
                val = (ushort)BitHelpers.SetBits(val, (ushort)((Red / 255f) * 31), 5, 11);
                val = (ushort)BitHelpers.SetBits(val, (ushort)((Green / 255f) * 63), 6, 0);
                val = (ushort)BitHelpers.SetBits(val, (ushort)((Blue / 255f) * 31), 5, 6);
                return val;
            }
            set {
                ushort color16 = value;
                // Extract the bits
                Red = (byte)((BitHelpers.ExtractBits(color16, 5, 11) / 31f) * 255);
                Green = (byte)((BitHelpers.ExtractBits(color16, 6, 0) / 63f) * 255);
                Blue = (byte)((BitHelpers.ExtractBits(color16, 5, 6) / 31f) * 255);
                Alpha = 255;
            }
        }

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) => Color565 = s.Serialize<ushort>(Color565, name: nameof(Color565));
    }
}