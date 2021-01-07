namespace R1Engine
{
    public class GBA_BatmanVengeance_Puppet : GBA_BaseBlock {
        #region Data

        public bool IsObjAnimation { get; set; } // False if it's a tile animation, thus using the tile palette
        public byte Batman_Byte00 { get; set; }
        public byte Index_Palette { get; set; }
        public byte AnimationsCount { get; set; }
        public byte Index_TileSet { get; set; }

        public byte TilemapWidth { get; set; }
        public byte TilemapHeight { get; set; }
        public byte[] Padding { get; set; }

        public uint[] AnimationOffsets { get; set; }

        #endregion

        #region Parsed
        public Pointer BlockEndPointer { get; set; }
        public GBA_SpritePalette Palette { get; set; }
        public GBA_SpriteTileSet TileSet { get; set; }
        public GBA_BatmanVengeance_Animation[] Animations { get; set; }

        #endregion

        #region Public Methods

        public override void SerializeBlock(SerializerObject s)
        {
            s.SerializeBitValues<byte>(bitFunc => {
                IsObjAnimation = bitFunc(IsObjAnimation ? 1 : 0, 1, name: nameof(IsObjAnimation)) == 1;
                Batman_Byte00 = (byte)bitFunc(Batman_Byte00, 7, name: nameof(Batman_Byte00));
            });

            Index_Palette = s.Serialize<byte>(Index_Palette, name: nameof(Index_Palette));
            AnimationsCount = s.Serialize<byte>(AnimationsCount, name: nameof(AnimationsCount));
            Index_TileSet = s.Serialize<byte>(Index_TileSet, name: nameof(Index_TileSet));

            if (s.GameSettings.EngineVersion < EngineVersion.GBA_BatmanVengeance) {
                TilemapWidth = s.Serialize<byte>(TilemapWidth, name: nameof(TilemapWidth));
                TilemapHeight = s.Serialize<byte>(TilemapHeight, name: nameof(TilemapHeight));
                Padding = s.SerializeArray<byte>(Padding, 2, name: nameof(Padding));
            }
            var animOffsetsBase = s.CurrentPointer;

            AnimationOffsets = s.SerializeArray<uint>(AnimationOffsets, AnimationsCount, name: nameof(AnimationOffsets));

            if (Animations == null) 
                Animations = new GBA_BatmanVengeance_Animation[AnimationOffsets.Length];
            BlockEndPointer = Offset + BlockSize;
            for (int i = 0; i < AnimationOffsets.Length; i++)
                Animations[i] = s.DoAt(animOffsetsBase + AnimationOffsets[i], () => s.SerializeObject<GBA_BatmanVengeance_Animation>(Animations[i], onPreSerialize: a => a.Puppet = this, name: $"{nameof(Animations)}[{i}]"));

            s.Goto(Offset + BlockSize);
        }

        public override void SerializeOffsetData(SerializerObject s)
        {
            if (s.GameSettings.GBA_IsShanghai)
            {
                TileSet = s.DoAt(OffsetTable.GetPointer(0), () => s.SerializeObject<GBA_SpriteTileSet>(TileSet, name: nameof(TileSet)));
            }
            else
            {
                Palette = s.DoAt(OffsetTable.GetPointer(Index_Palette), () => s.SerializeObject<GBA_SpritePalette>(Palette, name: nameof(Palette)));
                TileSet = s.DoAt(OffsetTable.GetPointer(Index_TileSet), () => s.SerializeObject<GBA_SpriteTileSet>(TileSet, name: nameof(TileSet)));
            }
        }

        #endregion
    }
}