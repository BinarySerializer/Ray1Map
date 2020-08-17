namespace R1Engine
{
    /// <summary>
    /// Common animation frame data
    /// </summary>
    public class R1_AnimationFrame : R1Serializable
    {
        /// <summary>
        /// The frame x position
        /// </summary>i
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
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) {
            XPosition = s.Serialize<byte>(XPosition, name: nameof(XPosition));
            YPosition = s.Serialize<byte>(YPosition, name: nameof(YPosition));
            Width = s.Serialize<byte>(Width, name: nameof(Width));
            Height = s.Serialize<byte>(Height, name: nameof(Height));
        }
    }
}