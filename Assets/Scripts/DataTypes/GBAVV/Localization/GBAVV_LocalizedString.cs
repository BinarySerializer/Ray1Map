namespace R1Engine
{
    public class GBAVV_LocalizedString : R1Serializable
    {
        public Pointer[] LocalizationPointers { get; set; }

        public GBAVV_LocalizedStringItem[] Items { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            LocalizationPointers = s.SerializePointerArray(LocalizationPointers, ((GBAVV_BaseManager)s.GameSettings.GetGameManager).LanguagesCount, name: nameof(LocalizationPointers));

            if (Items == null)
                Items = new GBAVV_LocalizedStringItem[LocalizationPointers.Length];

            for (int i = 0; i < Items.Length; i++)
                Items[i] = s.DoAt(LocalizationPointers[i], () => s.SerializeObject<GBAVV_LocalizedStringItem>(Items[i], name: $"{nameof(Items)}[{i}]"));
        }

        public class GBAVV_LocalizedStringItem : R1Serializable
        {
            public Pointer TextPointer { get; set; }

            public string Text { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                TextPointer = s.SerializePointer(TextPointer, name: nameof(TextPointer));

                Text = s.DoAt(TextPointer, () => s.SerializeString(Text, name: nameof(Text)));
            }
        }
    }
}