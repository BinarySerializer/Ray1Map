namespace R1Engine
{
    /// <summary>
    /// Animation layer data for Rayman 2 (PS1 - Demo)
    /// </summary>
    public class PS1_R2Demo_AnimationLayer : R1Serializable
    {
        /// <summary>
        /// The global image index
        /// </summary>
        public ushort ImageIndex { get; set; }

        /// <summary>
        /// The x position
        /// </summary>
        public byte XPosition { get; set; }

        /// <summary>
        /// The y position
        /// </summary>
        public byte YPosition { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            ImageIndex = s.Serialize<ushort>(ImageIndex, name: nameof(ImageIndex));
            XPosition = s.Serialize<byte>(XPosition, name: nameof(XPosition));
            YPosition = s.Serialize<byte>(YPosition, name: nameof(YPosition));
        }
    }
}