namespace R1Engine
{
    // TODO: Support multi-language versions
    public class GBAVV_Fusion_LocalizedString : R1Serializable
    {
        public Pointer LocalizationPointer { get; set; }

        public GBAVV_LocalizedStringItem Item { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            LocalizationPointer = s.SerializePointer(LocalizationPointer, name: nameof(LocalizationPointer));

            Item = s.DoAt(LocalizationPointer, () => s.SerializeObject<GBAVV_LocalizedStringItem>(Item, name: nameof(Item)));
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