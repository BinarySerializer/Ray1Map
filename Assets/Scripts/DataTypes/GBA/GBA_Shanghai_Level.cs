namespace R1Engine
{
    public class GBA_Shanghai_Level : GBA_BaseBlock
    {
        public byte[] Bytes_00 { get; set; }

        public ushort Index_Scene { get; set; }
        public ushort Index_ObjPal { get; set; }
        public ushort Index_Unknown1 { get; set; }
        public ushort Index_Unknown2 { get; set; }
        public ushort Index_PlayField { get; set; }
        public ushort Index_TilePal { get; set; }

        // Parsed from offets

        public GBA_Shanghai_Scene Scene { get; set; }
        public GBA_SpritePalette ObjPal { get; set; }
        public GBA_PlayField PlayField { get; set; }
        public GBA_Palette TilePal { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            Bytes_00 = s.SerializeArray<byte>(Bytes_00, 16, name: nameof(Bytes_00));

            Index_Scene = s.Serialize<ushort>(Index_Scene, name: nameof(Index_Scene));
            Index_ObjPal = s.Serialize<ushort>(Index_ObjPal, name: nameof(Index_ObjPal));
            Index_Unknown1 = s.Serialize<ushort>(Index_Unknown1, name: nameof(Index_Unknown1));
            Index_Unknown2 = s.Serialize<ushort>(Index_Unknown2, name: nameof(Index_Unknown2));
            Index_PlayField = s.Serialize<ushort>(Index_PlayField, name: nameof(Index_PlayField));
            Index_TilePal = s.Serialize<ushort>(Index_TilePal, name: nameof(Index_TilePal));
        }

        public override void SerializeOffsetData(SerializerObject s)
        {
            Scene = s.DoAt(OffsetTable.GetPointer(Index_Scene), () => s.SerializeObject<GBA_Shanghai_Scene>(Scene, name: nameof(Scene)));
            ObjPal = s.DoAt(OffsetTable.GetPointer(Index_ObjPal), () => s.SerializeObject<GBA_SpritePalette>(ObjPal, name: nameof(ObjPal)));
            PlayField = s.DoAt(OffsetTable.GetPointer(Index_PlayField), () => s.SerializeObject<GBA_PlayField>(PlayField, name: nameof(PlayField)));
            TilePal = s.DoAt(OffsetTable.GetPointer(Index_TilePal), () => s.SerializeObject<GBA_Palette>(TilePal, name: nameof(TilePal)));
        }
    }
}