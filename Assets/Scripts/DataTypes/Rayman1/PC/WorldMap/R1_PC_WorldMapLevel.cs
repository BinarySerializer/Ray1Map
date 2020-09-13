using System.Linq;

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

            string specialPath = null;
            var m = (R1_PCBaseManager)s.GameSettings.GetGameManager;

            if (m is R1_Kit_Manager kit)
                specialPath = kit.GetSpecialArchiveFilePath("USA");
            else if (m is R1_PCEdu_Manager edu)
                specialPath = edu.GetSpecialArchiveFilePath(s.GameSettings.EduVolume);

            if (specialPath != null)
                s.Log($"Name: {m.LoadArchiveFile<R1_PC_LocFile>(s.Context, specialPath, R1_PCBaseManager.R1_PC_ArchiveFileName.TEXT)?.TextDefine.ElementAtOrDefault(LevelName)?.Value ?? "(out of bounds)"}");

            LoadingVig = s.SerializeString(LoadingVig, 9, name: nameof(LoadingVig));
            MapEntries = s.SerializeObjectArray(MapEntries, 46, name: nameof(MapEntries));
        }
    }
}