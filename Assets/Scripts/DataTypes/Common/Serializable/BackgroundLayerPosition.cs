namespace R1Engine
{
    /// <summary>
    /// Background later position data
    /// </summary>
    public class BackgroundLayerPosition : R1Serializable
    {
        /// <summary>
        /// The layer x position
        /// </summary>
        public ushort XPosition { get; set; }
        
        /// <summary>
        /// The later y position
        /// </summary>
        public ushort YPosition { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) {
            XPosition = s.Serialize<ushort>(XPosition, name: nameof(XPosition));
            YPosition = s.Serialize<ushort>(YPosition, name: nameof(YPosition));
        }
    }
}