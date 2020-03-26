namespace R1Engine
{
    /// <summary>
    /// Background block data for Rayman 1 (PS1)
    /// </summary>
    public class PS1_R1_BackgroundBlock : R1Serializable
    {
        /// <summary>
        /// The background layer positions
        /// </summary>
        public PS1_R1_BackgroundLayerPosition[] BackgroundLayerPositions { get; set; }

        public byte[] Unknown3 { get; set; }

        /// <summary>
        /// The background layer info items
        /// </summary>
        public PS1_R1_BackgroundLayerInfo[] BackgroundLayerInfos { get; set; }

        public byte[] Unknown4 { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize the background layer information (always 12)
            BackgroundLayerPositions = s.SerializeObjectArray<PS1_R1_BackgroundLayerPosition>(BackgroundLayerPositions, 12, name: nameof(BackgroundLayerPositions));

            Unknown3 = s.SerializeArray<byte>(Unknown3, 16, name: nameof(Unknown3));

            BackgroundLayerInfos = s.SerializeObjectArray<PS1_R1_BackgroundLayerInfo>(BackgroundLayerInfos, 12, name: nameof(BackgroundLayerInfos));

            Unknown4 = s.SerializeArray<byte>(Unknown4, 80, name: nameof(Unknown4));

            // TODO: NTSC-J has more values here!
        }
    }
}