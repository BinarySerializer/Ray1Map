using System.Linq;
using System.Text;

namespace R1Engine
{
    public class GBARRR_LocalizationBlock : R1Serializable
    {
        public int Count { get; set; } = 0x400;
        public int LanguageCount { get; set; } = 6;

        public uint[] StringOffsets { get; set; }
        public string[][] Strings { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            StringOffsets = s.SerializeArray<uint>(StringOffsets, Count * LanguageCount, name: nameof(StringOffsets));

            if (Strings == null)
                Strings = Enumerable.Range(0, LanguageCount).Select(x => new string[Count]).ToArray();

            for (int i = 0; i < Count; i++)
            {
                for (int j = 0; j < LanguageCount; j++)
                {
                    if (StringOffsets[i] != 0)
                        Strings[j][i] = s.DoAt(Offset + StringOffsets[i * LanguageCount + j] + 3, () => s.SerializeString(Strings[j][i], encoding: Encoding.GetEncoding(1252), name: $"{nameof(Strings)}[{j}][{i}]"));
                }
            }
        }
    }
}