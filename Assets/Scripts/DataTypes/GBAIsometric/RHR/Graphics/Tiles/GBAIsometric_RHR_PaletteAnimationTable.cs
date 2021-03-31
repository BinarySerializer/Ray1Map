using System.Collections.Generic;
using BinarySerializer;

namespace R1Engine
{
    public class GBAIsometric_RHR_PaletteAnimationTable : BinarySerializable
    {
        public Entry[] Entries { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if (Entries == null) {
                List<Entry> entries = new List<Entry>();
                while (true)
                {
                    var e = s.SerializeObject<Entry>(default, name: $"{nameof(Entries)}[{entries.Count}]");

                    if (e.StartIndex != e.EndIndex)
                        entries.Add(e);
                    else
                        break;
                }
                Entries = entries.ToArray();
            } else {
                s.SerializeObjectArray<Entry>(Entries, Entries.Length, name: nameof(Entries));
            }
        }
        public class Entry : BinarySerializable {
            public byte PaletteIndex { get; set; }
            public sbyte Speed { get; set; }
            public byte StartIndex { get; set; }
            public byte EndIndex { get; set; }
            public byte Byte_04 { get; set; }
            public byte Byte_05 { get; set; }
            public byte Byte_06 { get; set; }
            public byte Byte_07 { get; set; }

            public override void SerializeImpl(SerializerObject s) {
                PaletteIndex = s.Serialize<byte>(PaletteIndex, name: nameof(PaletteIndex));
                Speed = s.Serialize<sbyte>(Speed, name: nameof(Speed));
                StartIndex = s.Serialize<byte>(StartIndex, name: nameof(StartIndex));
                EndIndex = s.Serialize<byte>(EndIndex, name: nameof(EndIndex));
                Byte_04 = s.Serialize<byte>(Byte_04, name: nameof(Byte_04));
                Byte_05 = s.Serialize<byte>(Byte_05, name: nameof(Byte_05));
                Byte_06 = s.Serialize<byte>(Byte_06, name: nameof(Byte_06));
                Byte_07 = s.Serialize<byte>(Byte_07, name: nameof(Byte_07));
            }
        }
    }
}