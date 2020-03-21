namespace R1Engine
{
    /// <summary>
    /// Background later data for Rayman 1 (PS1)
    /// </summary>
    public class PS1_R1_BackgroundLayerInfo : R1Serializable
    {
        public uint Unknown1 { get; set; }

        /// <summary>
        /// The layer the background appears on
        /// </summary>
        public byte Layer { get; set; }

        /// <summary>
        /// The background width
        /// </summary>
        public byte Width { get; set; }

        /// <summary>
        /// The background height
        /// </summary>
        public byte Height { get; set; }

        public byte[] Unknown2 { get; set; }

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public override void SerializeImpl(SerializerObject s) {
            Unknown1 = s.Serialize(Unknown1, name: "Unknown1");
            Layer = s.Serialize(Layer, name: "Layer");
            Width = s.Serialize(Width, name: "Width");
            Height = s.Serialize(Height, name: "Height");
            Unknown2 = s.SerializeArray<byte>(Unknown2, 13, name: "Unknown2");
        }
    }
}