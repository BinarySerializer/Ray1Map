using System.Collections.Generic;

namespace R1Engine
{
    public class GBA_BatmanVengeance_SpriteGroup : GBA_BaseBlock {
        #region Data

        public byte Byte_00 { get; set; }
        public byte PaletteOffsetIndex { get; set; }
        public byte AnimationsCount { get; set; }
        public byte TileMapOffsetIndex { get; set; }

        public uint[] AnimationOffsets { get; set; }

        #endregion

        #region Parsed

        public GBA_SpritePalette Palette { get; set; }
        public GBA_SpriteTileMap TileMap { get; set; }
        public GBA_BatmanVengeance_Animation[] Animations { get; set; }
        public Dictionary<int, GBA_AffineMatrixList> Matrices { get; set; } = new Dictionary<int, GBA_AffineMatrixList>();

        #endregion

        #region Public Methods

        public override void SerializeBlock(SerializerObject s) {
            Byte_00 = s.Serialize<byte>(Byte_00, name: nameof(Byte_00));
            PaletteOffsetIndex = s.Serialize<byte>(PaletteOffsetIndex, name: nameof(PaletteOffsetIndex));
            AnimationsCount = s.Serialize<byte>(AnimationsCount, name: nameof(AnimationsCount));
            TileMapOffsetIndex = s.Serialize<byte>(TileMapOffsetIndex, name: nameof(TileMapOffsetIndex));

            AnimationOffsets = s.SerializeArray<uint>(AnimationOffsets, AnimationsCount, name: nameof(AnimationOffsets));
            if (Animations == null) Animations = new GBA_BatmanVengeance_Animation[AnimationOffsets.Length];
            for (int i = 0; i < AnimationOffsets.Length; i++) {
                s.DoAt(Offset + 4 + AnimationOffsets[i], () => {
                    Animations[i] = s.SerializeObject<GBA_BatmanVengeance_Animation>(Animations[i], name: $"{nameof(Animations)}[{i}]");
                });
            }
        }

        public override void SerializeOffsetData(SerializerObject s)
        {
            Palette = s.DoAt(OffsetTable.GetPointer(PaletteOffsetIndex), () => s.SerializeObject<GBA_SpritePalette>(Palette, name: nameof(Palette)));
            TileMap = s.DoAt(OffsetTable.GetPointer(TileMapOffsetIndex), () => s.SerializeObject<GBA_SpriteTileMap>(TileMap, name: nameof(TileMap)));
        }

        #endregion
    }
}