using System;
using System.ComponentModel;
using System.IO;

namespace R1Engine
{
    /// <summary>
    /// World data for Rayman 1 (PC)
    /// </summary>
    [Description("Rayman 1 (PC) World File")]
    public class PC_R1_WorldFile : ISerializableFile
    {
        #region Public Properties

        public ushort Unknown1 { get; set; }

        public ushort Unknown2 { get; set; }

        public ushort Unknown4Count { get; set; }

        public byte Unknown3 { get; set; }

        public byte[] Unknown4 { get; set; }

        /// <summary>
        /// The amount of sprite groups
        /// </summary>
        public ushort SpriteGroupCount { get; set; }

        /// <summary>
        /// The sprite groups
        /// </summary>
        public PC_R1_SpriteGroup[] SpriteGroups { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Deserializes the file contents
        /// </summary>
        /// <param name="stream">The stream to read from</param>
        public void Deserialize(Stream stream)
        {
            // Read header
            Unknown1 = stream.Read<ushort>();
            Unknown2 = stream.Read<ushort>();
            Unknown4Count = stream.Read<ushort>();
            Unknown3 = stream.Read<byte>();
            Unknown4 = stream.Read<byte>(Unknown4Count);

            // Read sprites
            SpriteGroupCount = stream.Read<ushort>();
            SpriteGroups = stream.Read<PC_R1_SpriteGroup>(SpriteGroupCount);

            // Read ETA data
            // TODO: Finish
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Serializes the file contents
        /// </summary>
        /// <param name="stream">The stream to write to</param>
        public void Serialize(Stream stream)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}