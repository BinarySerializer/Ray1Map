using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace R1Engine
{
    /// <summary>
    /// Event localization data for Rayman Mapper (PC)
    /// </summary>
    [Description("Rayman Mapper (PC) Event Localization File")]
    public class PC_Mapper_EventLocFile : PC_BaseFile
    {
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
        public PC_Mapper_EventLocItem[] LocItems { get; set; }

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public override void Serialize(BinarySerializer serializer)
        {
            base.Serialize(serializer);

            serializer.Serialize(nameof(LocCount));

            if (serializer.Mode == SerializerMode.Read)
            {
                // TODO: Find way to avoid this
                // Since we don't know the length we go on until we hit the bytes for the localization items (they always start with MS)
                byte[] values;
                List<ushort> tempList = new List<ushort>();

                while (Settings.StringEncoding.GetString(values = serializer.ReadArray<byte>(2)) != "MS")
                    tempList.Add(BitConverter.ToUInt16(values, 0));

                Unknown2 = tempList.ToArray();

                // Go back two steps...
                serializer.BaseStream.Position -= 2;
            }
            else
            {
                serializer.Write(Unknown2);
            }

            serializer.SerializeArray<PC_Mapper_EventLocItem>(nameof(LocItems), LocCount);
        }
    }
}