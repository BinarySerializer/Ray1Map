using System;
using System.Collections.Generic;
using System.IO;

namespace R1Engine
{
    /// <summary>
    /// Event localization data for Rayman Designer (PC)
    /// </summary>
    public class PC_RD_EventLocFile : PC_RD_BaseFile
    {
        /// <summary>
        /// Unknown value, possibly a boolean
        /// </summary>
        public ushort Unknown1 { get; set; }

        /// <summary>
        /// The amount of localization items
        /// </summary>
        public uint LocCount { get; set; }

        /// <summary>
        /// Unknown header values
        /// </summary>
        public ushort[] Unknown2 { get; set; }

        /// <summary>
        /// The localization items
        /// </summary>
        public PC_RD_EventLocItem[] LocItems { get; set; }

        /// <summary>
        /// Deserializes the file contents
        /// </summary>
        /// <param name="stream">The stream to read from</param>
        public override void Deserialize(Stream stream)
        {
            base.Deserialize(stream);

            Unknown1 = stream.Read<ushort>();
            LocCount = stream.Read<uint>();

            // TODO: Find way to avoid this
            // Since we don't know the length we go on until we hit the bytes for the localization items (they always start with MS)
            byte[] values;
            List<ushort> tempList = new List<ushort>();

            while (PC_RD_Manager.GetStringEncoding.GetString(values = stream.ReadBytes(2)) != "MS")
                tempList.Add(BitConverter.ToUInt16(values, 0));

            Unknown2 = tempList.ToArray();

            // Go back two steps...
            stream.Position -= 2;

            LocItems = stream.Read<PC_RD_EventLocItem>(LocCount);
        }

        /// <summary>
        /// Serializes the file contents
        /// </summary>
        /// <param name="stream">The stream to write to</param>
        public override void Serialize(Stream stream)
        {
            base.Serialize(stream);

            stream.Write(Unknown1);
            stream.Write(LocCount);
            stream.Write(Unknown2);
            stream.Write(LocItems);
        }
    }
}