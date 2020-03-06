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
        /// Deserializes the file contents
        /// </summary>
        /// <param name="deserializer">The deserializer</param>
        public virtual void Deserialize(BinaryDeserializer deserializer)
        {
            PointerCount = deserializer.Read<uint>();
            Pointers = deserializer.ReadArray<uint>(PointerCount);
            FileSize = deserializer.Read<uint>();
        }

        /// <summary>
        /// Serializes the file contents
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public virtual void Serialize(BinarySerializer serializer)
        {
            serializer.Write(PointerCount);
            serializer.Write(Pointers);
            serializer.Write(FileSize);
        }
    }
}