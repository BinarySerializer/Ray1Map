using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class GBAIsometric_TileSet : R1Serializable
    {
        public Pointer<GBAIsometric_GraphicsData> GraphicsDataPointer { get; set; }
        public Pointer<GBAIsometric_PaletteIndexTable> PaletteIndexTablePointer { get; set; } // level 0: pointer to byte array with length = lookup buffer  (0xfd5), pointer to 0x55F bytes, pointer to 0x55F ushorts
        public Pointer CombinedTileDataPointer { get; set; } // ushorts
        public Pointer CombinedTileOffsetsPointer { get; set; } // ushorts. max ushort in this array = pointer2.length. not length-1, so probably either 0 or length has some special meaning

        public GBAIsometric_TileAssemble[] AssembleData { get; set; } = new GBAIsometric_TileAssemble[4];
        private Dictionary<ushort, ushort[]> AssembleCache { get; set; } = new Dictionary<ushort, ushort[]>();

        public Pointer PalettesPointer { get; set; }

        // Parsed
        public ushort[] CombinedTileData { get; set; }
        public ushort[] CombinedTileOffsets { get; set; }
        public ARGB1555Color[][] Palettes { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            GraphicsDataPointer = s.SerializePointer<GBAIsometric_GraphicsData>(GraphicsDataPointer, resolve: true, name: nameof(GraphicsDataPointer));
            PaletteIndexTablePointer = s.SerializePointer<GBAIsometric_PaletteIndexTable>(PaletteIndexTablePointer, resolve: true, onPreSerialize: pit => pit.Length = GraphicsDataPointer.Value.CompressionLookupBufferLength, name: nameof(PaletteIndexTablePointer));
            CombinedTileDataPointer = s.SerializePointer(CombinedTileDataPointer, name: nameof(CombinedTileDataPointer));
            CombinedTileOffsetsPointer = s.SerializePointer(CombinedTileOffsetsPointer, name: nameof(CombinedTileOffsetsPointer));
            for (int i = 0; i < 4; i++) {
                AssembleData[i] = s.SerializeObject<GBAIsometric_TileAssemble>(AssembleData[i], onPreSerialize: ad => ad.TileCompression = (GBAIsometric_TileAssemble.Compression)i, name: $"{nameof(AssembleData)}[{i}]");
            }
            PalettesPointer = s.SerializePointer(PalettesPointer, name: nameof(PalettesPointer));

            // Todo: Read these in a less hacky way
            s.DoAt(CombinedTileDataPointer, () => {
                CombinedTileData = s.SerializeArray<ushort>(CombinedTileData, (CombinedTileOffsetsPointer - CombinedTileDataPointer) / 2, name: nameof(CombinedTileData));
            });
            s.DoAt(CombinedTileOffsetsPointer, () => {
                if (CombinedTileDataPointer == CombinedTileOffsetsPointer) {
                    CombinedTileOffsets = new ushort[0];
                } else {
                    uint length = 0;
                    s.DoAt(CombinedTileOffsetsPointer, () => {
                        ushort CombinedTileOffsetsLengthHack = 0;
                        while (CombinedTileOffsetsLengthHack < CombinedTileData.Length) {
                            CombinedTileOffsetsLengthHack = s.Serialize<ushort>(CombinedTileOffsetsLengthHack, name: nameof(CombinedTileOffsetsLengthHack));
                        }
                        length = (uint)((s.CurrentPointer - CombinedTileOffsetsPointer) / 2);
                    });
                    CombinedTileOffsets = s.SerializeArray<ushort>(CombinedTileOffsets, length, name: nameof(CombinedTileOffsets));
                }
            });

            s.DoAt(PalettesPointer, () =>
            {
                if (Palettes == null)
                    Palettes = new ARGB1555Color[PaletteIndexTablePointer.Value.GetMaxPaletteIndex() + 1][];

                for (int i = 0; i < Palettes.Length; i++)
                    Palettes[i] = s.SerializeObjectArray<ARGB1555Color>(Palettes[i], 16, name: $"{nameof(Palettes)}[i]");
            });

            s.DoEncoded(new RHR_SpriteEncoder(false, GraphicsDataPointer.Value.CompressionLookupBuffer, GraphicsDataPointer.Value.CompressedDataPointer), () => {
                byte[] fullSheet = s.SerializeArray<byte>(default, s.CurrentLength, name: nameof(fullSheet));
                //Color[] cols = AnimatedPalettes.Select(c => c.GetColor());
                //Util.ByteArrayToFile(Context.BasePath + $"tiles/Full_4Bit_{Offset.StringAbsoluteOffset}.bin", fullSheet);
            });
        }

        public ushort[] Get8x8Map(ushort mapEntry) {
            if (!AssembleCache.ContainsKey(mapEntry)) {
                int algo = BitHelpers.ExtractBits(mapEntry, 2, 14);
                int arrayIndex = BitHelpers.ExtractBits(mapEntry, 14, 0);
                GBAIsometric_TileAssemble.TileData data = AssembleData[algo].Data[arrayIndex];
                switch (data.TileCompression) {
                    case GBAIsometric_TileAssemble.Compression.BlockDiff:
                        ushort[] baseMap = null;
                        ushort[] filledMap = new ushort[data.TileIndices.Length];
                        Array.Copy(data.TileIndices, filledMap, data.TileIndices.Length);
                        for (int i = 0; i < data.TileIndices.Length; i++) {
                            if (data.TileIndices[i] == 0xFFFF) {
                                if (baseMap == null) {
                                    baseMap = Get8x8Map(data.BaseMapEntry);
                                }
                                filledMap[i] = baseMap[i];
                            } else {
                                filledMap[i] = data.TileIndices[i];
                            }
                        }
                        AssembleCache[mapEntry] = filledMap;
                        break;
                    default:
                        AssembleCache[mapEntry] = data.TileIndices;
                        break;
                }
            }
            return AssembleCache[mapEntry];
        }

        public int GetTileIndex(int tileIndex) {
            while (tileIndex >= GraphicsDataPointer.Value.CompressionLookupBufferLength) {
                int indexInSecondaryArray = (int)(GraphicsDataPointer.Value.TotalLength - 1 - tileIndex);
                tileIndex = PaletteIndexTablePointer.Value.SecondaryTileIndices[indexInSecondaryArray];
            }
            return tileIndex;
        }

        public int GetPaletteIndex(int tileIndex) {
            if (tileIndex >= GraphicsDataPointer.Value.CompressionLookupBufferLength) {
                int indexInSecondaryArray = (int)(GraphicsDataPointer.Value.TotalLength - 1 - tileIndex);
                return PaletteIndexTablePointer.Value.SecondaryPaletteIndices[indexInSecondaryArray];
            }
            return PaletteIndexTablePointer.Value.PaletteIndices[tileIndex];
        }
    }
}