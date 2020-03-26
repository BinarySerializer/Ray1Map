using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace R1Engine
{
    /// <summary>
    /// Event localization data for Rayman Mapper (PC)
    /// </summary>
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
        public override void SerializeImpl(SerializerObject s) {
            base.SerializeImpl(s);

            LocCount = s.Serialize<uint>(LocCount, name: nameof(LocCount));

            if (s is BinaryDeserializer)
            {
                // TODO: Find way to avoid this
                // Since we don't know the length we go on until we hit the bytes for the localization items (they always start with MS)
                byte[] values;
                List<ushort> tempList = new List<ushort>();

                while (Settings.StringEncoding.GetString(values = s.SerializeArray<byte>(null,2)) != "MS")
                    tempList.Add(BitConverter.ToUInt16(values, 0));

                Unknown2 = tempList.ToArray();

                // Go back two steps...
                s.Goto(s.CurrentPointer - 2);
            }
            else
            {
                s.SerializeArray<ushort>(Unknown2, Unknown2.Length, name: nameof(Unknown2));
            }

            LocItems = s.SerializeObjectArray<PC_Mapper_EventLocItem>(LocItems, LocCount, name: nameof(LocItems));
        }
    }
}