using System;

namespace R1Engine
{
    /// <summary>
    /// A standard ARGB color wrapper with serializing support for the encoding RGB-666
    /// </summary>
    public class RGB666Color : ARGBColor
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public RGB666Color() : base(255, 0, 0, 0)
        { }

        public RGB666Color(byte red, byte green, byte blue) : base(red, green, blue)
        { }

        public RGB666Color(byte alpha, byte red, byte green, byte blue) : base(alpha, red, green, blue)
        { }

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public override void SerializeImpl(SerializerObject s) {
            Red = (byte)(s.Serialize<byte>((byte)(Red >> 2), name: nameof(Red)) << 2);
            Green = (byte)(s.Serialize<byte>((byte)(Green >> 2), name: nameof(Green)) << 2);
            Blue = (byte)(s.Serialize<byte>((byte)(Blue >> 2), name: nameof(Blue)) << 2);
        }
    }
}