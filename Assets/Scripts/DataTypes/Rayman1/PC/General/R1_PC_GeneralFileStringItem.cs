using BinarySerializer;

namespace R1Engine
{
    public class R1_PC_GeneralFileStringItem : BinarySerializable
    {
        public uint StringPointer { get; set; } // Gets overwritten by a pointer in memory
        public uint Uint_04 { get; set; }
        public ushort TextNum { get; set; }
        public byte FontSize { get; set; } // 1 = force uppercase, 2 = normal
        public byte Byte_0B { get; set; }
        public uint TextColor { get; set; } // 0-5

        public R1_PC_LocFileString String { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            StringPointer = s.Serialize<uint>(StringPointer, name: nameof(StringPointer));
            Uint_04 = s.Serialize<uint>(Uint_04, name: nameof(Uint_04));
            TextNum = s.Serialize<ushort>(TextNum, name: nameof(TextNum));
            FontSize = s.Serialize<byte>(FontSize, name: nameof(FontSize));
            Byte_0B = s.Serialize<byte>(Byte_0B, name: nameof(Byte_0B));
            TextColor = s.Serialize<uint>(TextColor, name: nameof(TextColor));
            String = s.SerializeObject<R1_PC_LocFileString>(String, name: nameof(String));
        }
    }
}