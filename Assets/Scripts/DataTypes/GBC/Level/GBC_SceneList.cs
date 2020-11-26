namespace R1Engine
{
    public class GBC_SceneList : GBC_BaseBlock
    {
        public byte Byte_00 { get; set; }
        public byte Byte_01 { get; set; }
        public byte Byte_02 { get; set; }
        public byte Index_FirstLevel { get; set; }
        public byte Index_FinalBoss { get; set; }
        public byte Index_WorldMap { get; set; }
        public byte Index_UbiCliff { get; set; }
        public byte Index_Unknown { get; set; }

        // Parsed
        public GBC_Scene Scene { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            Byte_00 = s.Serialize<byte>(Byte_00, name: nameof(Byte_00));
            Byte_01 = s.Serialize<byte>(Byte_01, name: nameof(Byte_01));
            Byte_02 = s.Serialize<byte>(Byte_02, name: nameof(Byte_02));
            Index_FirstLevel = s.Serialize<byte>(Index_FirstLevel, name: nameof(Index_FirstLevel));
            Index_FinalBoss = s.Serialize<byte>(Index_FinalBoss, name: nameof(Index_FinalBoss));
            Index_WorldMap = s.Serialize<byte>(Index_WorldMap, name: nameof(Index_WorldMap));
            Index_UbiCliff = s.Serialize<byte>(Index_UbiCliff, name: nameof(Index_UbiCliff));
            Index_Unknown = s.Serialize<byte>(Index_Unknown, name: nameof(Index_Unknown));

            // Parse data from pointers
            Scene = s.DoAt(OffsetTable.GetPointer(s.GameSettings.Level), () => s.SerializeObject<GBC_Scene>(Scene, name: nameof(Scene)));
        }
    }
}