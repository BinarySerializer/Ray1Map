using System.Text;

namespace R1Engine
{
    public class GBA_LocStringTable : R1Serializable
    {
        public uint StringCount { get; set; }
        public Pointer StringPointersPointer { get; set; }

        public Pointer[] Pointers { get; set; }
        public string[] Strings { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            StringCount = s.Serialize<uint>(StringCount, name: nameof(StringCount));
            StringPointersPointer = s.SerializePointer(StringPointersPointer, name: nameof(StringPointersPointer));

            Pointers = s.DoAt(StringPointersPointer, () => s.SerializePointerArray(Pointers, StringCount, name: nameof(Pointers)));

            if (Strings == null)
                Strings = new string[Pointers.Length];

            for (int i = 0; i < Strings.Length; i++)
                // TODO: Encoding is wrong!
                Strings[i] = s.DoAt(Pointers[i], () => s.SerializeString(Strings[i], encoding: Encoding.ASCII, name: $"{nameof(Strings)}[{i}]"));
        }
    }
}