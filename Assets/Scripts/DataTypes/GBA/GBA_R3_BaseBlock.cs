namespace R1Engine
{
    /// <summary>
    /// A base Rayman 3 (GBA) block
    /// </summary>
    public class GBA_R3_BaseBlock : R1Serializable
    {
        /// <summary>
        /// The size of the block in bytes, not counting this value
        /// </summary>
        public uint BlockSize { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            BlockSize = s.Serialize<uint>(BlockSize, name: nameof(BlockSize));
        }
    }
}