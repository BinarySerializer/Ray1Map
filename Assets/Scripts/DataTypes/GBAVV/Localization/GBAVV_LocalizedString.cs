namespace R1Engine
{
    public class GBAVV_LocalizedString : R1Serializable
    {
        public int Int_00 { get; set; }
        public Pointer[] LocalizationPointers { get; set; }

        public GBAVV_LocalizedStringItem[] Items { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if (s.GameSettings.EngineVersion == EngineVersion.GBAVV_CrashNitroKart)
                Int_00 = s.Serialize<int>(Int_00, name: nameof(Int_00));

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