using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class GBAIsometric_TileMapData : R1Serializable
    {
        public Pointer<GBAIsometric_GraphicsData> GraphicsDataPointer { get; set; }
        public Pointer Pointer1 { get; set; } // level 0: pointer to byte array with length = lookup buffer  (0xfd5), pointer to 0x55F bytes, pointer to 0x55F ushorts
        public Pointer Pointer2 { get; set; } // ushorts
        public Pointer Pointer3 { get; set; } // ushorts. max ushort in this array = pointer2.length. not length-1, so probably either 0 or length has some special meaning

        public GBAIsometric_TileAssemble[] AssembleData { get; set; } = new GBAIsometric_TileAssemble[4];
        private Dictionary<ushort, ushort[]> AssembleCache { get; set; } = new Dictionary<ushort, ushort[]>();

        public Pointer PalettesPointer { get; set; }

        // Parsed

        public Pointer PaletteIndexTablePointer { get; set; }
        public Pointer[] Pointer1_Pointers { get; set; }
        public byte[] PaletteIndexTable { get; set; }
        public ARGB1555Color[] Palettes { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            GraphicsDataPointer = s.SerializePointer<GBAIsometric_GraphicsData>(GraphicsDataPointer, resolve: true, name: nameof(GraphicsDataPointer));
            Pointer1 = s.SerializePointer(Pointer1, name: nameof(Pointer1));
            Pointer2 = s.SerializePointer(Pointer2, name: nameof(Pointer2));
            Pointer3 = s.SerializePointer(Pointer3, name: nameof(Pointer3));
            for (int i = 0; i < 4; i++) {
                AssembleData[i] = s.SerializeObject<GBAIsometric_TileAssemble>(AssembleData[i], onPreSerialize: ad => ad.TileCompression = (GBAIsometric_TileAssemble.Compression)i, name: $"{nameof(AssembleData)}[{i}]");
            }
            PalettesPointer = s.SerializePointer(PalettesPointer, name: nameof(PalettesPointer));

            s.DoAt(Pointer1, () => {
                PaletteIndexTablePointer = s.SerializePointer(PaletteIndexTablePointer, name: nameof(PaletteIndexTablePointer));
                Pointer1_Pointers = s.SerializePointerArray(Pointer1_Pointers, 2, name: nameof(Pointer1_Pointers));
            });
            Palettes = s.DoAt(PalettesPointer, () => s.SerializeObjectArray<ARGB1555Color>(Palettes, 16 * 45, name: nameof(Palettes)));

            s.DoEncoded(new RHR_SpriteEncoder(false, GraphicsDataPointer.Value.CompressionLookupBuffer, GraphicsDataPointer.Value.CompressedDataPointer), () => {
                byte[] fullSheet = s.SerializeArray<byte>(default, s.CurrentLength, name: nameof(fullSheet));
                //Color[] cols = AnimatedPalettes.Select(c => c.GetColor());
                //Util.ByteArrayToFile(Context.BasePath + $"tiles/Full_4Bit_{Offset.StringAbsoluteOffset}.bin", fullSheet);
            });
            s.DoAt(PaletteIndexTablePointer, () => {
                PaletteIndexTable = s.SerializeArray<byte>(PaletteIndexTable, GraphicsDataPointer.Value.CompressionLookupBufferLength, name: nameof(PaletteIndexTable));
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
    }
}