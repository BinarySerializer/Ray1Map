using System.IO;

namespace R1Engine
{
    /// <summary>
    /// Base file for Rayman 1 (PS1)
    /// </summary>
    public class PS1_R1_BaseFile : ISerializableFile
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
        /// <param name="stream">The stream to read from</param>
        public virtual void Deserialize(Stream stream)
        {
            PointerCount = stream.Read<uint>();
            Pointers = stream.Read<uint>(PointerCount);
            FileSize = stream.Read<uint>();
        }

        /// <summary>
        /// Serializes the file contents
        /// </summary>
        /// <param name="stream">The stream to write to</param>
        public virtual void Serialize(Stream stream)
        {
            stream.Write(PointerCount);
            stream.Write(Pointers);
            stream.Write(FileSize);
        }
    }
}