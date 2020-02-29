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
            LocKey = stream.ReadNullTerminatedString(PC_RD_Manager.GetStringEncoding);
            Name = stream.ReadNullTerminatedString(PC_RD_Manager.GetStringEncoding);
            Description = stream.ReadNullTerminatedString(PC_RD_Manager.GetStringEncoding);
        }

        /// <summary>
        /// Serializes the file contents
        /// </summary>
        /// <param name="stream">The stream to write to</param>
        public void Serialize(Stream stream)
        {
            stream.WriteNullTerminatedString(PC_RD_Manager.GetStringEncoding, LocKey);
            stream.WriteNullTerminatedString(PC_RD_Manager.GetStringEncoding, Name);
            stream.WriteNullTerminatedString(PC_RD_Manager.GetStringEncoding, Description);
        }
    }
}