using System.Text;
using BinarySerializer;

namespace Ray1Map.GBA
{
    public class GBA_LocalizationString : BinarySerializable
    {
        public uint LinesCount { get; set; }
        public Pointer LinesPointer { get; set; }

        public Pointer[] LinePointers { get; set; }
        public string[] Lines { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            LinesCount = s.Serialize<uint>(LinesCount, name: nameof(LinesCount));
            LinesPointer = s.SerializePointer(LinesPointer, name: nameof(LinesPointer));

            // The R3 prototype has some languages with fewer entries. Ignore the overflow entries we read.
            if (LinesCount > 1000)
                return;

            s.DoAt(LinesPointer, () => LinePointers = s.SerializePointerArray(LinePointers, LinesCount, name: nameof(LinePointers)));

            Lines ??= new string[LinePointers.Length];

            for (int i = 0; i < Lines.Length; i++)
                Lines[i] = s.DoAt(LinePointers[i], () => s.SerializeString(Lines[i], encoding: Encoding.GetEncoding(1252), name: $"{nameof(Lines)}[{i}]"));
        }
    }
}