namespace R1Engine
{
    public class GBAVV_NitroKart_NGage_ExeFile : R1Serializable
    {
        public uint[] CRCPolynomialData { get; set; }
        public GBAVV_NitroKart_NGage_LevelInfo[] LevelInfos { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            CRCPolynomialData = s.DoAt(new Pointer(0x10068A00, Offset.file), () => s.SerializeArray<uint>(CRCPolynomialData, 256, name: nameof(CRCPolynomialData)));
            LevelInfos = s.DoAt(new Pointer(0x1006961c, Offset.file), () => s.SerializeObjectArray<GBAVV_NitroKart_NGage_LevelInfo>(LevelInfos, 26, name: nameof(LevelInfos)));
        }
    }
}