using System.Collections.Generic;
using System.Linq;
using BinarySerializer;

namespace Ray1Map.GBA
{
    public class GBA_Puppet : GBA_BaseBlock {
        #region Data

        public ushort ID { get; set; }
        public ushort Index_TileSet { get; set; }
        public ushort Index_Palette { get; set; }
        public byte Index_Unknown { get; set; }
        public ushort Byte_04 { get; set; }
        public ushort AnimationsCount { get; set; }
        public byte Byte_06 { get; set; }

        public byte[] Milan_Bytes_08 { get; set; }

        public ushort[] AnimationIndexTable { get; set; }

        #endregion

        #region Parsed

        public GBA_SpritePalette Palette { get; set; }
        public GBA_SpriteTileSet TileSet { get; set; }
        public GBA_TileKit Milan_TileKit { get; set; } // We could use the SpriteTileSet class here too, but it'd require duplicating all the compression code
        public GBA_Animation[] Animations { get; set; }
        public Dictionary<int, GBA_AffineMatrixList> Matrices { get; set; } = new Dictionary<int, GBA_AffineMatrixList>();

        #endregion

        #region Public Methods

        public override void SerializeBlock(SerializerObject s)
        {
            if (!s.GetR1Settings().GBA_IsMilan)
            {
                if (s.GetR1Settings().EngineVersion != EngineVersion.GBA_Sabrina)
                    ID = s.Serialize<ushort>(ID, name: nameof(ID));

                Index_TileSet = s.Serialize<byte>((byte)Index_TileSet, name: nameof(Index_TileSet));
                Index_Palette = s.Serialize<byte>((byte)Index_Palette, name: nameof(Index_Palette));

                if (s.GetR1Settings().EngineVersion >= EngineVersion.GBA_PrinceOfPersia)
                    Index_Unknown = s.Serialize<byte>(Index_Unknown, name: nameof(Index_Unknown));

                Byte_04 = s.Serialize<byte>((byte)Byte_04, name: nameof(Byte_04)); // Byte_04 & 0xF == palette count
                AnimationsCount = s.Serialize<byte>((byte)AnimationsCount, name: nameof(AnimationsCount));
                Byte_06 = s.Serialize<byte>(Byte_06, name: nameof(Byte_06)); // This multiplied by 2 is some length

                AnimationIndexTable = s.SerializeArray<byte>(AnimationIndexTable?.Select(x => (byte)x).ToArray(), AnimationsCount, name: nameof(AnimationIndexTable)).Select(x => (ushort)x).ToArray();
            }
            else
            {
                Index_TileSet = s.Serialize<ushort>(Index_TileSet, name: nameof(Index_TileSet));
                Index_Palette = s.Serialize<ushort>(Index_Palette, name: nameof(Index_Palette));
                Byte_04 = s.Serialize<ushort>(Byte_04, name: nameof(Byte_04));
                AnimationsCount = s.Serialize<ushort>(AnimationsCount, name: nameof(AnimationsCount));

                Milan_Bytes_08 = s.SerializeArray<byte>(Milan_Bytes_08, 8, name: nameof(Milan_Bytes_08));

                AnimationIndexTable = s.SerializeArray<ushort>(AnimationIndexTable, AnimationsCount, name: nameof(AnimationIndexTable));
            }
        }

        public override void SerializeOffsetData(SerializerObject s)
        {
            Palette = s.DoAt(OffsetTable.GetPointer(Index_Palette), () => s.SerializeObject<GBA_SpritePalette>(Palette, name: nameof(Palette)));

            if (!s.GetR1Settings().GBA_IsMilan)
                TileSet = s.DoAt(OffsetTable.GetPointer(Index_TileSet), () => s.SerializeObject<GBA_SpriteTileSet>(TileSet, onPreSerialize: x =>
                {
                    if (s.GetR1Settings().EngineVersion == EngineVersion.GBA_Sabrina)
                        x.IsDataCompressed = BitHelpers.ExtractBits(Byte_04, 1, 5) == 0;
                }, name: nameof(TileSet)));
            else
                Milan_TileKit = s.DoAt(OffsetTable.GetPointer(Index_TileSet), () => s.SerializeObject<GBA_TileKit>(Milan_TileKit, name: nameof(Milan_TileKit)));

            if (Animations == null)
                Animations = new GBA_Animation[AnimationsCount];

            for (int i = 0; i < Animations.Length; i++)
                Animations[i] = s.DoAt(OffsetTable.GetPointer(AnimationIndexTable[i]), () => s.SerializeObject<GBA_Animation>(Animations[i], name: $"{nameof(Animations)}[{i}]"));

            for (int i = 0; i < Animations.Length; i++) {
                if (Animations[i] == null) continue;
                int matrixIndex = Animations[i].Index_AffineMatrices;
                if (matrixIndex != 0) {

                    Matrices[matrixIndex] = s.DoAt(OffsetTable.GetPointer(matrixIndex),
                        () => s.SerializeObject<GBA_AffineMatrixList>(
                            Matrices.ContainsKey(matrixIndex) ? Matrices[matrixIndex] : null,
                            onPreSerialize: ml => ml.FrameCount = Animations[i].FrameCount,
                            name: $"{nameof(Matrices)}[{matrixIndex}]"));
                }
            }
        }

        #endregion
    }
}