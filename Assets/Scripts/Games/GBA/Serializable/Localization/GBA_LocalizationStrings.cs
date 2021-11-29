using BinarySerializer;

namespace Ray1Map.GBA
{
    public class GBA_LocalizationStrings : BinarySerializable
    {
        public Pointer[] GroupPointers { get; set; }

        public GBA_LocalizationString[][] Groups { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            GroupPointers = s.SerializePointerArray(GroupPointers, 12, name: nameof(GroupPointers));

            Groups ??= new GBA_LocalizationString[GroupPointers.Length][];

            int[] lengths = s.GetR1Settings().GetGameManagerOfType<GBA_Manager>()?.LocalizationGroupLengths;

            if (lengths == null)
                return;

            for (int i = 0; i < Groups.Length; i++)
                Groups[i] = s.DoAt(GroupPointers[i], () => s.SerializeObjectArray<GBA_LocalizationString>(Groups[i], lengths[i], name: $"{nameof(Groups)}[{i}]"));
        }
    }
}