namespace R1Engine
{
    public class GBA_Milan_Puppet : GBA_BaseBlock
    {
        #region Data

        public ushort Index_TileKit { get; set; }
        public ushort Index_Palette { get; set; }
        
        public ushort Ushort_04 { get; set; }
        public ushort AnimCount { get; set; }

        public byte[] Bytes_08 { get; set; }

        public ushort[] AnimationIndexTable { get; set; }

        #endregion

        #region Parsed

        public GBA_SpritePalette Palette { get; set; }
        public GBA_TileKit TileSet { get; set; }
        public GBA_Animation[] Animations { get; set; }

        #endregion

        #region Public Methods

        public override void SerializeBlock(SerializerObject s)
        {
            Index_TileKit = s.Serialize<ushort>(Index_TileKit, name: nameof(Index_TileKit));
            Index_Palette = s.Serialize<ushort>(Index_Palette, name: nameof(Index_Palette));

            Ushort_04 = s.Serialize<ushort>(Ushort_04, name: nameof(Ushort_04));
            AnimCount = s.Serialize<ushort>(AnimCount, name: nameof(AnimCount));

            Bytes_08 = s.SerializeArray<byte>(Bytes_08, 8, name: nameof(Bytes_08));

            AnimationIndexTable = s.SerializeArray<ushort>(AnimationIndexTable, AnimCount, name: nameof(AnimationIndexTable));
        }

        public override void SerializeOffsetData(SerializerObject s)
        {
            Palette = s.DoAt(OffsetTable.GetPointer(Index_Palette), () => s.SerializeObject<GBA_SpritePalette>(Palette, name: nameof(Palette)));
            TileSet = s.DoAt(OffsetTable.GetPointer(Index_TileKit), () => s.SerializeObject<GBA_TileKit>(TileSet, name: nameof(TileSet)));
            return;
            if (Animations == null)
                Animations = new GBA_Animation[AnimationIndexTable.Length];

            for (int i = 0; i < Animations.Length; i++)
                Animations[i] = s.DoAt(OffsetTable.GetPointer(AnimationIndexTable[i]), () => s.SerializeObject<GBA_Animation>(Animations[i], name: $"{nameof(Animations)}[{i}]"));
        }

        #endregion
    }
}