using System;

namespace R1Engine
{
    /// <summary>
    /// A standard ARGB color wrapper with serializing support for the encoding BGR-1555
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
        /// Indicates if fully black should be treated as transparent
        /// </summary>
        public bool IsBlackTransparent { get; set; } = true;

        ushort Color555 {
            get {
                if (Alpha == 0) return 0;
                ushort red16 = (ushort)((Red / 255f) * 31);
                ushort green16 = (ushort)((Green / 255f) * 31);
                ushort blue16 = (ushort)((Blue / 255f) * 31);
                ushort col = (ushort)(red16 | (green16 << 5) | (blue16 << 10));
                return col;
            }
            set {
                ushort color16 = value;
                // Extract the bits
                Alpha = 255;
                Red = (byte)((BitHelpers.ExtractBits(color16, 5, 0) / 31f) * 255);
                Green = (byte)((BitHelpers.ExtractBits(color16, 5, 5) / 31f) * 255);
                Blue = (byte)((BitHelpers.ExtractBits(color16, 5, 10) / 31f) * 255);

                // Check if transparent
                if (IsBlackTransparent && Red == 0 && Green == 0 && Blue == 0)
                    Alpha = 0;
            }
        }

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public override void SerializeImpl(SerializerObject s) {
            Color555 = s.Serialize<ushort>(Color555, name: "Color555");
        }
    }
}