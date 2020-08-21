namespace R1Engine
{
    /// <summary>
    /// A standard ARGB color wrapper with serializing support for the encoding BGR-444
    /// </summary>
    public class ARGB1444Color : ARGBColor
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public ARGB1444Color() : base(255, 0, 0, 0)
        { }

        public ARGB1444Color(byte red, byte green, byte blue) : base(red, green, blue)
        { }

        public ARGB1444Color(byte alpha, byte red, byte green, byte blue) : base(alpha, red, green, blue)
        { }

        /// <summary>
        /// The color as a raw 16-bit value
        /// </summary>
        public ushort Color1444 {
            get {
                ushort alpha16 = (ushort)(Alpha / 255f);
                ushort red16 = (ushort)((Red / 255f) * 15);
                ushort green16 = (ushort)((Green / 255f) * 15);
                ushort blue16 = (ushort)((Blue / 255f) * 15);
                ushort col = (ushort)(blue16 | (green16 << 4) | (red16 << 8) | (alpha16 << 15));
                return col;
            }
            set {
                ushort color16 = value;
                // Extract the bits
                Blue = (byte)((BitHelpers.ExtractBits(color16, 4, 0) / 15f) * 255);
                Green = (byte)((BitHelpers.ExtractBits(color16, 4, 4) / 15f) * 255);
                Red = (byte)((BitHelpers.ExtractBits(color16, 4, 8) / 15f) * 255);
                Alpha = (byte)((BitHelpers.ExtractBits(color16, 1, 15)) * 255);
            }
        }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) {
            Color1444 = s.Serialize<ushort>(Color1444, name: nameof(Color1444));
        }

        public static ARGB1444Color From1444(ushort argb1444) {
            ARGB1444Color col = new ARGB1444Color();
            col.Color1444 = argb1444;
            return col;
        }
    }
}