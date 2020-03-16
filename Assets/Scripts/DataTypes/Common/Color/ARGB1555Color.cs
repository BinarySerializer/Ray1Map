using System;

namespace R1Engine
{
    /// <summary>
    /// A standard ARGB color wrapper with serializing support for the encoding BGR-1555
    /// </summary>
    public class ARGB1555Color : ARGBColor, IBinarySerializable
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
        /// Serializes the data
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public void Serialize(BinarySerializer serializer)
        {
            if (serializer.Mode == SerializerMode.Read)
            {
                // Read the color value
                ushort color16 = serializer.Read<ushort>();

                // Extract the bits
                Alpha = 255;
                Red = (byte)((BitHelpers.ExtractBits(color16, 5, 0) / 31f) * 255);
                Green = (byte)((BitHelpers.ExtractBits(color16, 5, 5) / 31f) * 255);
                Blue = (byte)((BitHelpers.ExtractBits(color16, 5, 10) / 31f) * 255);

                // Check if transparent
                if (Red == 0 && Green == 0 && Blue == 0)
                    Alpha = 0;
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}