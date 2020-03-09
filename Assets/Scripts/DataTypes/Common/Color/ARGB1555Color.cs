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
        /// Deserializes the file contents
        /// </summary>
        /// <param name="deserializer">The deserializer</param>
        public void Deserialize(BinaryDeserializer deserializer)
        {
            // Read the color value
            ushort colour16 = deserializer.Read<ushort>();

            // Extract the bits
            Alpha = 255;
            Red = (byte)((colour16 & 0x1F) << 3);
            Green = (byte)(((colour16 & 0x3E0) >> 5) << 3);
            Blue = (byte)(((colour16 & 0x7C00) >> 10) << 3);

            // Check if transparent
            if (Red == 0 && Green == 0 && Blue == 0)
                Alpha = 0;
        }

        /// <summary>
        /// Serializes the file contents
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public void Serialize(BinarySerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}