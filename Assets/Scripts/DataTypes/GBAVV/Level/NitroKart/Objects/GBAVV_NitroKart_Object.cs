namespace R1Engine
{
    public class GBAVV_NitroKart_Object : R1Serializable
    {
        public int XPos { get; set; }
        public int YPos { get; set; }
        public int Height { get; set; }
        public int ObjType { get; set; }
        public Pointer ParamsPointer { get; set; }
        public byte[] NGage_Params { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            XPos = s.Serialize<int>(XPos, name: nameof(XPos));
            YPos = s.Serialize<int>(YPos, name: nameof(YPos));
            Height = s.Serialize<int>(Height, name: nameof(Height));
            ObjType = s.Serialize<int>(ObjType, name: nameof(ObjType));

            if (s.GameSettings.EngineVersion == EngineVersion.GBAVV_CrashNitroKart_NGage)
                NGage_Params = s.SerializeArray<byte>(NGage_Params, 20, name: nameof(NGage_Params));
            else
                ParamsPointer = s.SerializePointer(ParamsPointer, name: nameof(ParamsPointer));
        }
    }
}