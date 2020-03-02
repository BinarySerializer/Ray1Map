using System.IO;
using System.Text;

namespace R1Engine
{
    /// <summary>
    /// Event localization item for Rayman Designer (PC)
    /// </summary>
    public class PC_RD_EventLocItem : ISerializableFile
    {
        /// <summary>
        /// The localization key
        /// </summary>
        public string LocKey { get; set; }

        /// <summary>
        /// The localized name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The localized description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Deserializes the file contents
        /// </summary>
        /// <param name="stream">The stream to read from</param>
        public void Deserialize(Stream stream)
        {
            LocKey = stream.ReadNullTerminatedString();
            Name = stream.ReadNullTerminatedString();
            Description = stream.ReadNullTerminatedString();
        }

        /// <summary>
        /// Serializes the file contents
        /// </summary>
        /// <param name="stream">The stream to write to</param>
        public void Serialize(Stream stream)
        {
            stream.WriteNullTerminatedString(LocKey);
            stream.WriteNullTerminatedString(Name);
            stream.WriteNullTerminatedString(Description);
        }
    }
}