using System;

namespace R1Engine
{
    /// <summary>
    /// A standard ARGB color wrapper with serializing support for the encoding RGB-565
    /// </summary>
    public class RGB565Color : ARGBColor
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public RGB565Color() : base(255, 0, 0, 0)
        { }

        public RGB565Color(byte red, byte green, byte blue) : base(red, green, blue)
        { }

        public RGB565Color(byte alpha, byte red, byte green, byte blue) : base(alpha, red, green, blue)
        { }

        /// <summary>
        /// The color as a raw 16-bit value
        /// </summary>
        public ushort Color565 {
            get {
                ushort val = 0;
                val = (ushort)BitHelpers.SetBits(val, (ushort)((Red / 255f) * 31), 5, 0);
                val = (ushort)BitHelpers.SetBits(val, (ushort)((Green / 255f) * 63), 6, 5);
                val = (ushort)BitHelpers.SetBits(val, (ushort)((Blue / 255f) * 31), 5, 11);
                return val;
            }
            set {
                ushort color16 = value;
                // Extract the bits
                Red = (byte)((BitHelpers.ExtractBits(color16, 5, 0) / 31f) * 255);
                Green = (byte)((BitHelpers.ExtractBits(color16, 6, 5) / 63f) * 255);
                Blue = (byte)((BitHelpers.ExtractBits(color16, 5, 11) / 31f) * 255);
                Alpha = 255;
            }
        }

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public override void SerializeImpl(SerializerObject s) {
            Color565 = s.Serialize<ushort>(Color565, name: nameof(Color565));
        }
    }
}