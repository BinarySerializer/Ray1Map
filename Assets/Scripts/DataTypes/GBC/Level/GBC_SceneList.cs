namespace R1Engine
{
    public class GBC_SceneList : GBC_BaseBlock
    {
        public byte IndexMusic { get; set; }
        public byte IndexUnk0 { get; set; }
        public byte IndexUnk1 { get; set; }
        public byte IndexFirstLevel { get; set; }
        public byte IndexFinalBoss { get; set; }
        public byte IndexWorldMap { get; set; }
        public byte IndexUbiCliff { get; set; }
        public byte Unk { get; set; }

        // Parsed
        public GBC_Scene Scene { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            IndexMusic = s.Serialize<byte>(IndexMusic, name: nameof(IndexMusic));
            IndexUnk0 = s.Serialize<byte>(IndexUnk0, name: nameof(IndexUnk0));
            IndexUnk1 = s.Serialize<byte>(IndexUnk1, name: nameof(IndexUnk1));
            IndexFirstLevel = s.Serialize<byte>(IndexFirstLevel, name: nameof(IndexFirstLevel));
            IndexFinalBoss = s.Serialize<byte>(IndexFinalBoss, name: nameof(IndexFinalBoss));
            IndexWorldMap = s.Serialize<byte>(IndexWorldMap, name: nameof(IndexWorldMap));
            IndexUbiCliff = s.Serialize<byte>(IndexUbiCliff, name: nameof(IndexUbiCliff));
            Unk = s.Serialize<byte>(Unk, name: nameof(Unk));

            // Parse data from pointers
            Scene = s.DoAt(OffsetTable.GetPointer(s.GameSettings.Level), () => s.SerializeObject<GBC_Scene>(Scene, name: nameof(Scene)));
        }
    }
}