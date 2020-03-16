namespace R1Engine
{
    /// <summary>
    /// Base file for Rayman 1 (PS1)
    /// </summary>
    public class PS1_R1_BaseFile : IBinarySerializable
    {
        /// <summary>
        /// The amount of pointers in the header
        /// </summary>
        public uint PointerCount { get; set; }

        /// <summary>
        /// The block pointers
        /// </summary>
        public uint[] Pointers { get; set; }
        
        /// <summary>
        /// The length of the file in bytes
        /// </summary>
        public uint FileSize { get; set; }

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public virtual void Serialize(BinarySerializer serializer)
        {
            serializer.Serialize(nameof(PointerCount));
            serializer.SerializeArray<uint>(nameof(Pointers), PointerCount);
            serializer.Serialize(nameof(FileSize));
        }
    }
}