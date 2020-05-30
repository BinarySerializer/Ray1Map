namespace R1Engine
{
    /// <summary>
    /// A standard ARGB color wrapper with serializing support for the encoding BGR-555
    /// </summary>
    public class ARGB1555Color : ARGBColor
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public ARGB1555Color() : base(255, 0, 0, 0)
        { }

        public ARGB1555Color(byte red, byte green, byte blue) : base(red, green, blue)
        { }

        public ARGB1555Color(byte alpha, byte red, byte green, byte blue) : base(alpha, red, green, blue)
        { }

        /// <summary>
        /// The color as a raw 16-bit value
        /// </summary>
        public ushort Color1555 {
            get {
                ushort alpha16 = (ushort)(Alpha / 255f);
                ushort red16 = (ushort)((Red / 255f) * 31);
                ushort green16 = (ushort)((Green / 255f) * 31);
                ushort blue16 = (ushort)((Blue / 255f) * 31);
                ushort col = (ushort)(red16 | (green16 << 5) | (blue16 << 10) | (alpha16 << 15));
                return col;
            }
            set {
                ushort color16 = value;
                // Extract the bits
                Red = (byte)((BitHelpers.ExtractBits(color16, 5, 0) / 31f) * 255);
                Green = (byte)((BitHelpers.ExtractBits(color16, 5, 5) / 31f) * 255);
                Blue = (byte)((BitHelpers.ExtractBits(color16, 5, 10) / 31f) * 255);
                Alpha = (byte)((BitHelpers.ExtractBits(color16, 1, 15)) * 255);
            }
        }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) {
            Color1555 = s.Serialize<ushort>(Color1555, name: nameof(Color1555));
        }

        public static ARGB1555Color From1555(ushort argb1555) {
            ARGB1555Color col = new ARGB1555Color();
            col.Color1555 = argb1555;
            return col;
        }
    }
}