using System.Linq;

namespace R1Engine
{
    public class GBAVV_LocalizedString : R1Serializable
    {
        public Pointer[] LocalizationPointers { get; set; }

        public GBAVV_LocalizedStringItem[] Items { get; set; }

        public string DefaultString { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            LocalizationPointers = s.SerializePointerArray(LocalizationPointers, ((GBAVV_BaseManager)s.GameSettings.GetGameManager).Languages.Length, name: nameof(LocalizationPointers));

            if (Items == null)
                Items = new GBAVV_LocalizedStringItem[LocalizationPointers.Length];

            for (int i = 0; i < Items.Length; i++)
                Items[i] = s.DoAt(LocalizationPointers[i], () => s.SerializeObject<GBAVV_LocalizedStringItem>(Items[i], name: $"{nameof(Items)}[{i}]"));

            DefaultString = Items?.ElementAtOrDefault(((GBAVV_BaseManager)s.GameSettings.GetGameManager).DefaultLanguage)?.Text;
        }
    }
}