namespace R1Engine
{
    public class GBAVV_NitroKart_NGage_LevelInfo : R1Serializable
    {
        public GBAVV_NitroKart_NGage_FilePathPointer File_PVS { get; set; }
        public GBAVV_NitroKart_NGage_FilePathPointer File_POP { get; set; }
        public int Int_08 { get; set; }
        public GBAVV_NitroKart_NGage_FilePathPointer Dir_GFX { get; set; }
        public int Int_10 { get; set; }
        public int Int_14 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            File_PVS = s.SerializeObject(File_PVS, name: nameof(File_PVS));
            File_POP = s.SerializeObject(File_POP, name: nameof(File_POP));
            Int_08 = s.Serialize<int>(Int_08, name: nameof(Int_08));
            Dir_GFX = s.SerializeObject(Dir_GFX, x => x.IsRelativePath = true, name: nameof(Dir_GFX));
            Int_10 = s.Serialize<int>(Int_10, name: nameof(Int_10));
            Int_14 = s.Serialize<int>(Int_14, name: nameof(Int_14));
        }
    }
}