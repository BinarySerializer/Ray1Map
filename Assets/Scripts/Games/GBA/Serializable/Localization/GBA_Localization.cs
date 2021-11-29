using BinarySerializer;

namespace Ray1Map.GBA
{
    public class GBA_Localization : BinarySerializable
    {
        public Pointer[] LanguagePointers { get; set; }
        
        public GBA_LocalizationStrings[] LanguageGroups { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            LanguagePointers = s.SerializePointerArray(LanguagePointers, s.GetR1Settings().GetGameManagerOfType<GBA_Manager>().Languages?.Length ?? 0, name: nameof(LanguagePointers));

            LanguageGroups ??= new GBA_LocalizationStrings[LanguagePointers.Length];

            for (int i = 0; i < LanguageGroups.Length; i++)
                LanguageGroups[i] = s.DoAt(LanguagePointers[i], () => s.SerializeObject<GBA_LocalizationStrings>(LanguageGroups[i], name:  $"{nameof(LanguageGroups)}[{i}]"));
        }
    }
}