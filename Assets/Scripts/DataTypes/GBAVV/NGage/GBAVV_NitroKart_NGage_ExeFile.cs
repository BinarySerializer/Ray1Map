namespace R1Engine
{
    public class GBAVV_NitroKart_NGage_ExeFile : R1Serializable
    {
        public bool SerializeAllData { get; set; } // Set before serializing

        public uint[] CRCPolynomialData { get; set; }
        public GBAVV_NitroKart_NGage_LevelInfo[] LevelInfos { get; set; }
        public GBAVV_NitroKart_ObjTypeData[] NitroKart_ObjTypeData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // TODO: Move pointers to pointer table so we can support the JP version

            CRCPolynomialData = s.DoAt(new Pointer(0x10068A00, Offset.file), () => s.SerializeArray<uint>(CRCPolynomialData, 256, name: nameof(CRCPolynomialData)));

            s.DoAt(new Pointer(0x1006961c, Offset.file), () =>
            {
                if (LevelInfos == null)
                    LevelInfos = new GBAVV_NitroKart_NGage_LevelInfo[26];

                for (int i = 0; i < LevelInfos.Length; i++)
                    LevelInfos[i] = s.SerializeObject<GBAVV_NitroKart_NGage_LevelInfo>(LevelInfos[i], x => x.SerializeData = i == s.GameSettings.Level || SerializeAllData, name: $"{nameof(LevelInfos)}[{i}]");
            });

            var pointers = ((GBAVV_NitroKart_NGage_Manager)s.GameSettings.GetGameManager).ObjTypesDataPointers;

            if (NitroKart_ObjTypeData == null)
                NitroKart_ObjTypeData = new GBAVV_NitroKart_ObjTypeData[pointers.Length];

            for (int i = 0; i < pointers.Length; i++)
                NitroKart_ObjTypeData[i] = s.DoAt(pointers[i] == null ? null : new Pointer(pointers[i].Value, Offset.file), () => s.SerializeObject<GBAVV_NitroKart_ObjTypeData>(NitroKart_ObjTypeData[i], name: $"{nameof(NitroKart_ObjTypeData)}[{i}]"));
        }
    }
}