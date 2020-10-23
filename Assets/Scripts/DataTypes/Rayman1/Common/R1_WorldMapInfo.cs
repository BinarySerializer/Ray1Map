using System.Linq;

namespace R1Engine
{
    /// <summary>
    /// World map info
    /// </summary>
    public class R1_WorldMapInfo : R1Serializable
    {
        public short XPosition { get; set; }
        public short YPosition { get; set; }
        public byte UpIndex { get; set; }
        public byte DownIndex { get; set; }
        public byte LeftIndex { get; set; }
        public byte RightIndex { get; set; }
        public ushort RuntimeData { get; set; } // Has info for if it's unlocked, cage count etc.
        public R1_World World { get; set; }
        public byte Level { get; set; }
        public uint Uint_0C { get; set; }
        public Pointer Pointer_10 { get; set; }

        // EDU
        public uint Unk1 { get; set; }
        public byte[] Unk2 { get; set; }
        public ushort LevelName { get; set; }
        public string LoadingVig { get; set; }
        public R1_PC_WorldMapLevelMapEntry[] MapEntries { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if (s.GameSettings.EngineVersion == EngineVersion.R1_PC_Edu || s.GameSettings.EngineVersion == EngineVersion.R1_PS1_Edu || s.GameSettings.EngineVersion == EngineVersion.R1_PC_Kit)
            {
                Unk1 = s.Serialize<uint>(Unk1, name: nameof(Unk1));
                XPosition = s.Serialize<short>(XPosition, name: nameof(XPosition));
                YPosition = s.Serialize<short>(YPosition, name: nameof(YPosition));
                UpIndex = s.Serialize<byte>(UpIndex, name: nameof(UpIndex));
                DownIndex = s.Serialize<byte>(DownIndex, name: nameof(DownIndex));
                LeftIndex = s.Serialize<byte>(LeftIndex, name: nameof(LeftIndex));
                RightIndex = s.Serialize<byte>(RightIndex, name: nameof(RightIndex));
                Unk2 = s.SerializeArray<byte>(Unk2, 8, name: nameof(Unk2));
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
            else
            {
                XPosition = s.Serialize<short>(XPosition, name: nameof(XPosition));
                YPosition = s.Serialize<short>(YPosition, name: nameof(YPosition));
                UpIndex = s.Serialize<byte>(UpIndex, name: nameof(UpIndex));
                DownIndex = s.Serialize<byte>(DownIndex, name: nameof(DownIndex));
                LeftIndex = s.Serialize<byte>(LeftIndex, name: nameof(LeftIndex));
                RightIndex = s.Serialize<byte>(RightIndex, name: nameof(RightIndex));
                RuntimeData = s.Serialize<ushort>(RuntimeData, name: nameof(RuntimeData));
                World = s.Serialize<R1_World>(World, name: nameof(World));
                Level = s.Serialize<byte>(Level, name: nameof(Level));
                Uint_0C = s.Serialize<uint>(Uint_0C, name: nameof(Uint_0C));
                Pointer_10 = s.SerializePointer(Pointer_10, allowInvalid: true, name: nameof(Pointer_10));
            }
        }
    }
}