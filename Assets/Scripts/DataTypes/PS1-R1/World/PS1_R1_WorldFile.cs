using System;
using System.IO;

namespace R1Engine
{
    /// <summary>
    /// World data for Rayman 1 (PS1)
    /// </summary>
    public class PS1_R1_WorldFile : PS1_R1_BaseFile
    {
        // TODO: Add properties for tile set

        /// <summary>
        /// Deserializes the file contents
        /// </summary>
        /// <param name="stream">The stream to read from</param>
        public override void Deserialize(Stream stream)
        {
            // HEADER
            base.Deserialize(stream);

            throw new NotImplementedException();
        }

        /// <summary>
        /// Serializes the file contents
        /// </summary>
        /// <param name="stream">The stream to write to</param>
        public override void Serialize(Stream stream)
        {
            // HEADER

            base.Serialize(stream);

            throw new NotImplementedException();
        }
    }
}