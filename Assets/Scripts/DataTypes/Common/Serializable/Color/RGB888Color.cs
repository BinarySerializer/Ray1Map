namespace R1Engine
{
    /// <summary>
    /// A standard ARGB color wrapper with serializing support for the encoding RGB-888
    /// </summary>
    public class RGB888Color : ARGBColor
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public RGB888Color() : base(255, 0, 0, 0)
        { }

        public RGB888Color(byte red, byte green, byte blue) : base(red, green, blue)
        { }

        public RGB888Color(byte alpha, byte red, byte green, byte blue) : base(alpha, red, green, blue)
        { }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) 
        {
            Red = s.Serialize<byte>(Red, name: nameof(Red));
            Green = s.Serialize<byte>(Green, name: nameof(Green));
            Blue = s.Serialize<byte>(Blue, name: nameof(Blue));
        }
    }
}