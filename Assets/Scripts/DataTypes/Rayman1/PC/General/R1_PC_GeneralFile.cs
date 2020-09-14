namespace R1Engine
{
    public class R1_PC_GeneralFile : R1Serializable
    {
        // Always 1
        public uint Unk1 { get; set; }
        
        // Last 21 are indexes
        public byte[] Unk2 { get; set; }

        // Has a header of file names
        public byte[] Unk3 { get; set; }

        public string[] SampleNames { get; set; }

        // Only has items on EDU
        public byte VignetteItemsCount { get; set; }
        public uint Unk5 { get; set; }
        public uint Unk6 { get; set; }
        public R1_PCEdu_GeneralFileVignetteItem[] VignetteItems { get; set; }

        // The game credits
        public uint CreditsStringItemsCount { get; set; }
        public R1_PC_GeneralFileStringItem[] CreditsStringItems { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            Unk1 = s.Serialize<uint>(Unk1, name: nameof(Unk1));

            var unk2Length = 27;

            // The PS1 version hard-codes a different length for this version
            if (s.GameSettings.EngineVersion == EngineVersion.R1_PS1_Edu && s.GameSettings.EduVolume.StartsWith("CS"))
                unk2Length = 29;

            Unk2 = s.SerializeArray<byte>(Unk2, unk2Length, name: nameof(Unk2));
            Unk3 = s.SerializeArray<byte>(Unk3, 3130, name: nameof(Unk3));
            SampleNames = s.SerializeStringArray(SampleNames, 7, 9, name: nameof(SampleNames));

            VignetteItemsCount = s.Serialize<byte>(VignetteItemsCount, name: nameof(VignetteItemsCount));
            Unk5 = s.Serialize<uint>(Unk5, name: nameof(Unk5));
            Unk6 = s.Serialize<uint>(Unk6, name: nameof(Unk6));

            VignetteItems = s.SerializeObjectArray<R1_PCEdu_GeneralFileVignetteItem>(VignetteItems, VignetteItemsCount, name: nameof(VignetteItems));

            CreditsStringItemsCount = s.Serialize<uint>(CreditsStringItemsCount, name: nameof(CreditsStringItemsCount));
            CreditsStringItems = s.SerializeObjectArray<R1_PC_GeneralFileStringItem>(CreditsStringItems, CreditsStringItemsCount, name: nameof(CreditsStringItems));
        }
    }
}