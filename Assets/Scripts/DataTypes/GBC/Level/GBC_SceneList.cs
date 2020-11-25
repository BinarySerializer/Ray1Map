namespace R1Engine
{
    public class GBC_SceneList : GBC_BaseBlock
    {
        public byte IndexMusic { get; set; }
        public byte IndexUnk0 { get; set; }
        public byte IndexUnk1 { get; set; }
        public byte IndexUnk2 { get; set; }
        public byte IndexFinalBoss { get; set; }
        public byte IndexWorldMap { get; set; }
        public byte IndexUnk3 { get; set; }
        public byte Unk { get; set; }

        // Parsed
        public GBC_Scene Scene { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize header
            base.SerializeImpl(s);
            SerializeOffsetTable(s);

            IndexMusic = s.Serialize<byte>(IndexMusic, name: nameof(IndexMusic));
            IndexUnk0 = s.Serialize<byte>(IndexUnk0, name: nameof(IndexUnk0));
            IndexUnk1 = s.Serialize<byte>(IndexUnk1, name: nameof(IndexUnk1));
            IndexUnk2 = s.Serialize<byte>(IndexUnk2, name: nameof(IndexUnk2));
            IndexFinalBoss = s.Serialize<byte>(IndexFinalBoss, name: nameof(IndexFinalBoss));
            IndexWorldMap = s.Serialize<byte>(IndexWorldMap, name: nameof(IndexWorldMap));
            IndexUnk3 = s.Serialize<byte>(IndexUnk3, name: nameof(IndexUnk3));
            Unk = s.Serialize<byte>(Unk, name: nameof(Unk));

            // Parse data from pointers
            Scene = s.DoAt(OffsetTable.GetPointer(s.GameSettings.Level), () => s.SerializeObject<GBC_Scene>(Scene, name: nameof(Scene)));
        }
    }
}