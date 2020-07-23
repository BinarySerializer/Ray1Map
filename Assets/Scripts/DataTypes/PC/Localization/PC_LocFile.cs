namespace R1Engine
{
    public class PC_LocFile : R1Serializable
    {
        // NombreLangues
        public byte NumberOfLanguages { get; set; }

        // LangueUtilisee
        public byte LanguageUtilized { get; set; }

        public KeyboardTypes KeyboardType { get; set; }

        public string[] LanguageNames { get; set; }

        public uint TextDefineCount { get; set; }
        
        // Different for each language
        public ushort Unk1 { get; set; }

        public ushort Unk2 { get; set; }

        public PC_LocFileString[] TextDefine { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            NumberOfLanguages = s.Serialize<byte>(NumberOfLanguages, name: nameof(NumberOfLanguages));
            LanguageUtilized = s.Serialize<byte>(LanguageUtilized, name: nameof(LanguageUtilized));
            KeyboardType = s.Serialize<KeyboardTypes>(KeyboardType, name: nameof(KeyboardType));

            LanguageNames = s.SerializeStringArray(LanguageNames, 3, 11, name: nameof(LanguageNames));

            TextDefineCount = s.Serialize<uint>(TextDefineCount, name: nameof(TextDefineCount));
            Unk1 = s.Serialize<ushort>(Unk1, name: nameof(Unk1));
            Unk2 = s.Serialize<ushort>(Unk2, name: nameof(Unk2));

            TextDefine = s.SerializeObjectArray<PC_LocFileString>(TextDefine, TextDefineCount, name: nameof(TextDefine));
        }

        public enum KeyboardTypes : byte
        {
            QWERTY,
            AZERTY
        }
    }
}