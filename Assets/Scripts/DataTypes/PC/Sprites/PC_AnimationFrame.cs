namespace R1Engine
{
    /// <summary>
    /// Animation frame data for PC
    /// </summary>
    public class PC_AnimationFrame : R1Serializable
    {
        // TODO: Verify the values are correct

        /// <summary>
        /// The frame x position
        /// </summary>
        public byte XPosition { get; set; }

        /// <summary>
        /// The frame y position
        /// </summary>
        public byte YPosition { get; set; }

        /// <summary>
        /// The frame width
        /// </summary>
        public byte Width { get; set; }

        /// <summary>
        /// The frame height
        /// </summary>
        public byte Height { get; set; }

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public override void SerializeImpl(SerializerObject s) {
            XPosition = s.Serialize<byte>(XPosition, name: "XPosition");
            YPosition = s.Serialize<byte>(YPosition, name: "YPosition");
            Width = s.Serialize<byte>(Width, name: "Width");
            Height = s.Serialize<byte>(Height, name: "Height");
        }
    }
}