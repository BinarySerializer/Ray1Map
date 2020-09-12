namespace R1Engine
{
    public class R1_PC_WorldMapLevel : R1Serializable
    {
        public uint Unk1 { get; set; }
        public ushort XPos { get; set; }
        public ushort YPos { get; set; }
        public byte[] Unk2 { get; set; }
        public ushort LevelName { get; set; }
        public string LoadingVig { get; set; }
        public R1_PC_WorldMapLevelMapEntry[] MapEntries { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Unk1 = s.Serialize<uint>(Unk1, name: nameof(Unk1));
            XPos = s.Serialize<ushort>(XPos, name: nameof(XPos));
            YPos = s.Serialize<ushort>(YPos, name: nameof(YPos));
            Unk2 = s.SerializeArray<byte>(Unk2, 12, name: nameof(Unk2));
            LevelName = s.Serialize<ushort>(LevelName, name: nameof(LevelName));
            s.Log($"Name: {((R1_PCBaseManager)s.GameSettings.GetGameManager).ReadArchiveFile<R1_PC_LocFile>(s.Context, R1_PCBaseManager.R1_PC_ArchiveFileName.TEXT, s.GameSettings.EngineVersion == EngineVersion.R1_PC_Kit ? "USA" : null)?.TextDefine[LevelName].Value}");
            LoadingVig = s.SerializeString(LoadingVig, 9, name: nameof(LoadingVig));
            MapEntries = s.SerializeObjectArray(MapEntries, 46, name: nameof(MapEntries));
        }
    }
}