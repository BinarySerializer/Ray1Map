namespace R1Engine
{
    public class Jaguar_ReferenceEntry : R1Serializable
    {
        public Pointer StringBase { get; set; }

        public Pointer StringPointer { get; set; }
        public EntryType Type { get; set; }
        public Pointer DataPointer { get; set; }
        public uint DataValue { get; set; }

        public string String { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            StringPointer = s.SerializePointer(StringPointer, anchor: StringBase, name: nameof(StringPointer));
            Type = s.Serialize<EntryType>(Type, name: nameof(Type));
            s.SerializeArray<byte>(new byte[3], 3, name: "Padding");

            if (Type == EntryType.DataBlock)
                DataPointer = s.SerializePointer(DataPointer, name: nameof(DataPointer));
            else
                DataValue = s.Serialize<uint>(DataValue, name: nameof(DataValue));

            s.DoAt(StringPointer, () => String = s.SerializeString(String, name: nameof(String)));
        }

        public enum EntryType : byte
        {
            Unk_2 = 2,

            /// <summary>
            /// Pointer to a data block or function
            /// </summary>
            DataBlock = 6,

            Unk_8 = 8
        }
    }
}