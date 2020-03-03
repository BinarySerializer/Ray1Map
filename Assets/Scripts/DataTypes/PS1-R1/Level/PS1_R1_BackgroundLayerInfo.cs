using System.IO;

namespace R1Engine
{
    /// <summary>
    /// Background later data for Rayman 1 (PS1)
    /// </summary>
    public class PS1_R1_BackgroundLayerInfo : ISerializableFile
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
        /// Deserializes the file contents
        /// </summary>
        /// <param name="stream">The stream to read from</param>
        public void Deserialize(Stream stream)
        {
            Unknown1 = stream.Read<uint>();
            Layer = stream.Read<byte>();
            Width = stream.Read<byte>();
            Height = stream.Read<byte>();
            Unknown2 = stream.ReadBytes(13);
        }

        /// <summary>
        /// Serializes the file contents
        /// </summary>
        /// <param name="stream">The stream to write to</param>
        public void Serialize(Stream stream)
        {
            stream.Write(Unknown1);
            stream.Write(Layer);
            stream.Write(Width);
            stream.Write(Height);
            stream.Write(Unknown2);
        }
    }
}