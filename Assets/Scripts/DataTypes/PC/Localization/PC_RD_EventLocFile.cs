using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace R1Engine
{
    /// <summary>
    /// Event localization data for Rayman Designer (PC)
    /// </summary>
    [Description("Rayman Designer (PC) Event Localization File")]
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
        /// <param name="deserializer">The deserializer</param>
        public override void Deserialize(BinaryDeserializer deserializer)
        {
            base.Deserialize(deserializer);

            Unknown1 = deserializer.Read<ushort>();
            LocCount = deserializer.Read<uint>();

            // TODO: Find way to avoid this
            // Since we don't know the length we go on until we hit the bytes for the localization items (they always start with MS)
            byte[] values;
            List<ushort> tempList = new List<ushort>();

            while (Settings.StringEncoding.GetString(values = deserializer.ReadBytes(2)) != "MS")
                tempList.Add(BitConverter.ToUInt16(values, 0));

            Unknown2 = tempList.ToArray();

            // Go back two steps...
            deserializer.BaseStream.Position -= 2;

            LocItems = deserializer.Read<PC_RD_EventLocItem>(LocCount);
        }

        /// <summary>
        /// Serializes the file contents
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public override void Serialize(BinarySerializer serializer)
        {
            base.Serialize(serializer);

            serializer.Write(Unknown1);
            serializer.Write(LocCount);
            serializer.Write(Unknown2);
            serializer.Write(LocItems);
        }
    }
}