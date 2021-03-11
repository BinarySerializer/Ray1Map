namespace R1Engine
{
    public class GBAVV_NitroKart_NGage_ExeFile : R1Serializable
    {
        public bool SerializeAllData { get; set; } // Set before serializing
        public bool SerializeGAX { get; set; } // Set before serializing

        public uint[] CRCPolynomialData { get; set; }
        public GBAVV_NitroKart_NGage_LevelInfo[] LevelInfos { get; set; }
        public GBAVV_NitroKart_ObjTypeData[] NitroKart_ObjTypeData { get; set; }

        public GBAVV_NitroKart_NGage_S3D S3D_Podium { get; set; }
        public GBAVV_NitroKart_NGage_S3D S3D_Warp { get; set; }

        public GBAVV_Script[] Scripts { get; set; }

        public GBAVV_NitroKart_NGage_GAX GAX_Music { get; set; }
        public GBAVV_NitroKart_NGage_GAX GAX_FX { get; set; }

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

            S3D_Podium = new GBAVV_NitroKart_NGage_FilePath(s.Context, @"podium.s3d").DoAtFile(() => s.SerializeObject<GBAVV_NitroKart_NGage_S3D>(S3D_Podium, name: nameof(S3D_Podium)));
            S3D_Warp = new GBAVV_NitroKart_NGage_FilePath(s.Context, @"warp.s3d").DoAtFile(() => s.SerializeObject<GBAVV_NitroKart_NGage_S3D>(S3D_Warp, name: nameof(S3D_Warp)));

            // Serialize scripts
            var scriptPointers = ((GBAVV_BaseManager)s.GameSettings.GetGameManager).ScriptPointers;

            if (scriptPointers != null)
            {
                if (Scripts == null)
                    Scripts = new GBAVV_Script[scriptPointers.Length];

                for (int i = 0; i < scriptPointers.Length; i++)
                    Scripts[i] = s.DoAt(new Pointer(scriptPointers[i], Offset.file), () => s.SerializeObject<GBAVV_Script>(Scripts[i], x =>
                    {
                        x.SerializeFLC = false;
                        x.BaseFile = Offset.file;
                    }, name: $"{nameof(Scripts)}[{i}]"));
            }

            if (SerializeGAX)
            {
                GAX_Music = new GBAVV_NitroKart_NGage_FilePath(s.Context, @"snd\music.gax").DoAtFile(() => s.SerializeObject<GBAVV_NitroKart_NGage_GAX>(GAX_Music, x => x.SongsCount = 24, name: nameof(GAX_Music)));
                GAX_FX = new GBAVV_NitroKart_NGage_FilePath(s.Context, @"snd\fx.gax").DoAtFile(() => s.SerializeObject<GBAVV_NitroKart_NGage_GAX>(GAX_FX, x => {
                    x.SongsCount = 1; x.SamplesCount = 68;
                }, name: nameof(GAX_FX)));
            }
        }
    }
}