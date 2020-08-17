using System;

namespace R1Engine
{
    /// <summary>
    /// GRX bundle data for EDU on PS1
    /// </summary>
    public class R1_PS1Edu_GRX : R1Serializable
    {
        /// <summary>
        /// The magic header
        /// </summary>
        public byte[] Magic { get; set; }

        /// <summary>
        /// The base offset
        /// </summary>
        public Pointer BaseOffset { get; set; }

        /// <summary>
        /// The amount of files in the bundle
        /// </summary>
        public uint FileCount { get; set; }

        /// <summary>
        /// The files
        /// </summary>
        public R1_PS1Edu_GRXFile[] Files { get; set; }

        /// <summary>
        /// Gets the file with the specified file name
        /// </summary>
        /// <param name="fileName">The file name</param>
        /// <returns>The file</returns>
        public R1_PS1Edu_GRXFile GetFile(string fileName) => Files?.FindItem(x => x.FileName.Equals(fileName, StringComparison.InvariantCultureIgnoreCase)) ?? throw new Exception($"No matching file was found for name {fileName}");

        /// <summary>
        /// Gets the file bytes based on name
        /// </summary>
        /// <param name="s">The serializer object</param>
        /// <param name="fileName">The file name</param>
        /// <returns>The file bytes</returns>
        public byte[] GetFileBytes(SerializerObject s, string fileName)
        {
            // Attempt to find the file
            var file = GetFile(fileName);

            // Create the buffer
            byte[] buffer = null;

            // Read the bytes
            s.DoAt(BaseOffset + file.FileOffset, () => buffer = s.SerializeArray<byte>(buffer, file.FileSize, name: nameof(buffer)));

            // Return the buffer
            return buffer;
        }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            Magic = s.SerializeArray<byte>(Magic, 4, name: nameof(Magic));
            BaseOffset = s.SerializePointer(BaseOffset, this.BaseOffset, name: nameof(BaseOffset));
            FileCount = s.Serialize<uint>(FileCount, name: nameof(FileCount));
            Files = s.SerializeObjectArray<R1_PS1Edu_GRXFile>(Files, FileCount, name: nameof(Files));
        }
    }
}