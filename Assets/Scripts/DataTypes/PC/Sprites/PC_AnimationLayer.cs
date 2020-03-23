namespace R1Engine
{
    /// <summary>
    /// Animation layer data for PC
    /// </summary>
    public class PC_AnimationLayer : R1Serializable
    {
        /// <summary>
        /// Indicates if the layer is flipped horizontally
        /// </summary>
        public bool IsFlipped { get; set; }

        /// <summary>
        /// The x position
        /// </summary>
        public byte XPosition { get; set; }

        /// <summary>
        /// The y position
        /// </summary>
        public byte YPosition { get; set; }

        /// <summary>
        /// The image index as it appears in the image block
        /// </summary>
        public byte ImageIndex { get; set; }

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public override void SerializeImpl(SerializerObject s) {
            IsFlipped = s.Serialize<bool>(IsFlipped, name: "IsFlipped");
            XPosition = s.Serialize<byte>(XPosition, name: "XPosition");
            YPosition = s.Serialize<byte>(YPosition, name: "YPosition");
            ImageIndex = s.Serialize<byte>(ImageIndex, name: "ImageIndex");
        }
    }
}