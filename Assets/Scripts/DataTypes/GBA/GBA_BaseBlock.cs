namespace R1Engine
{
    /// <summary>
    /// A base GBA block
    /// </summary>
    public abstract class GBA_BaseBlock : R1Serializable
    {
        /// <summary>
        /// The size of the block in bytes, not counting this value
        /// </summary>
        public uint BlockSize { get; set; }

        /// <summary>
        /// The block offset table
        /// </summary>
        public GBA_OffsetTable OffsetTable { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize the size
            BlockSize = s.Serialize<uint>(BlockSize, name: nameof(BlockSize));

            // Serialize the block
            SerializeBlock(s);

            // Align
            s.Align();

            // Serialize the offset table
            OffsetTable = s.SerializeObject<GBA_OffsetTable>(OffsetTable, name: nameof(OffsetTable));

            // Serialize data from the offset table
            SerializeOffsetData(s);
        }

        public abstract void SerializeBlock(SerializerObject s);
        public virtual void SerializeOffsetData(SerializerObject s) { }
    }
}