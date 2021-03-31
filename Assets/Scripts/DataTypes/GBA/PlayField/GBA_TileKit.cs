
using System;
using BinarySerializer;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// A map block for GBA
    /// </summary>
    public class GBA_TileKit : GBA_BaseBlock {
        public ushort TileSet4bppSize { get; set; }
        public ushort TileSet8bppSize { get; set; }
        public byte Byte_04 { get; set; } // Not used. Always 0 in R3GBA, but not in N-Gage (but tile block is always offset 0).
        public byte AnimatedTileKitManagerIndex { get; set; }
        public byte PaletteCount { get; set; }
        public byte Byte_07 { get; set; }

        public byte[] PaletteIndices { get; set; }
        public byte[] TileSet4bpp { get; set; }
        public byte[] TileSet8bpp { get; set; }

        // Batman
        public bool Is8bpp { get; set; }
        public bool IsCompressed { get; set; }

        public Milan_CompressionType CompressionType { get; set; }

        #region Parsed
        public GBA_Palette[] Palettes { get; set; }
        public GBA_AnimatedTileKitManager AnimatedTileKitManager { get; set; }
        public GBA_AnimatedTileKit[] AnimatedTileKits { get; set; }
        #endregion

        public override void SerializeBlock(SerializerObject s) {
            if (s.GetR1Settings().EngineVersion <= EngineVersion.GBA_BatmanVengeance) {
                Is8bpp = s.Serialize<bool>(Is8bpp, name: nameof(Is8bpp));

                if (s.GetR1Settings().GBA_IsMilan)
                {
                    CompressionType = s.Serialize<Milan_CompressionType>(CompressionType, name: nameof(CompressionType));
                }
                else
                {
                    IsCompressed = s.Serialize<bool>(IsCompressed, name: nameof(IsCompressed));
                    CompressionType = IsCompressed ? Milan_CompressionType.LZSS : Milan_CompressionType.None;
                }

                if (Is8bpp) {
                    TileSet8bppSize = s.Serialize<ushort>(TileSet8bppSize, name: nameof(TileSet8bppSize));
                } else {
                    TileSet4bppSize = s.Serialize<ushort>(TileSet4bppSize, name: nameof(TileSet4bppSize));
                }
            } else {
                TileSet4bppSize = s.Serialize<ushort>(TileSet4bppSize, name: nameof(TileSet4bppSize));
                TileSet8bppSize = s.Serialize<ushort>(TileSet8bppSize, name: nameof(TileSet8bppSize));
                IsCompressed = s.Serialize<bool>(IsCompressed, name: nameof(IsCompressed));
                AnimatedTileKitManagerIndex = s.Serialize<byte>(AnimatedTileKitManagerIndex, name: nameof(AnimatedTileKitManagerIndex)); // Can be 0xFF which means this block doesn't exist
                PaletteCount = s.Serialize<byte>(PaletteCount, name: nameof(PaletteCount));
                Byte_07 = s.Serialize<byte>(Byte_07, name: nameof(Byte_07));
                PaletteIndices = s.SerializeArray<byte>(PaletteIndices, PaletteCount, name: nameof(PaletteIndices));

                CompressionType = IsCompressed && s.GetR1Settings().EngineVersion != EngineVersion.GBA_R3_NGage ? Milan_CompressionType.LZSS : Milan_CompressionType.None;
            }

            // Serialize tilemap data
            switch (CompressionType)
            {
                case Milan_CompressionType.None:
                default:
                    TileSet4bpp = s.SerializeArray<byte>(TileSet4bpp, TileSet4bppSize * 0x20, name: nameof(TileSet4bpp));
                    TileSet8bpp = s.SerializeArray<byte>(TileSet8bpp, TileSet8bppSize * 0x40, name: nameof(TileSet8bpp));
                    break;

                case Milan_CompressionType.RL:
                    throw new NotImplementedException();

                case Milan_CompressionType.LZSS:
                    s.DoEncoded(new GBA_LZSSEncoder(), () => {
                        TileSet4bpp = s.SerializeArray<byte>(TileSet4bpp, TileSet4bppSize * 0x20, name: nameof(TileSet4bpp));
                        TileSet8bpp = s.SerializeArray<byte>(TileSet8bpp, TileSet8bppSize * 0x40, name: nameof(TileSet8bpp));
                    });
                    s.Align();
                    break;
            }
        }

        public override void SerializeOffsetData(SerializerObject s)
        {
            if (s.GetR1Settings().EngineVersion > EngineVersion.GBA_BatmanVengeance) {
                // Serialize tile palette
                if (Palettes == null) Palettes = new GBA_Palette[PaletteCount];
                for (int p = 0; p < Palettes.Length; p++) {
                    Palettes[p] = s.DoAt(OffsetTable.GetPointer(PaletteIndices[p]), () => s.SerializeObject<GBA_Palette>(Palettes[p], name: $"{nameof(Palettes)}[{p}]"));
                }

                // Serialize tile animations
                if (AnimatedTileKitManagerIndex != 0xFF) {
                    AnimatedTileKitManager = s.DoAt(OffsetTable.GetPointer(AnimatedTileKitManagerIndex), () => s.SerializeObject<GBA_AnimatedTileKitManager>(AnimatedTileKitManager, name: nameof(AnimatedTileKitManager)));
                    if (AnimatedTileKits == null) AnimatedTileKits = new GBA_AnimatedTileKit[AnimatedTileKitManager.Length];
                    for (int i = 0; i < AnimatedTileKits.Length; i++) {
                        AnimatedTileKits[i] = s.DoAt(OffsetTable.GetPointer(AnimatedTileKitManager.TileKitBlocks[i]), () => s.SerializeObject<GBA_AnimatedTileKit>(AnimatedTileKits[i], name: $"{nameof(AnimatedTileKits)}[{i}]"));
                    }
                    Debug.Log("Level " + s.GetR1Settings().Level + " (" + s.GetR1Settings().World + ") has " + AnimatedTileKits.Length + " animated tilekits.");
                }
            }
            else if (s.GetR1Settings().GBA_IsMilan && OffsetTable.OffsetsCount > 0)
            {
                if (Palettes == null)
                    Palettes = new GBA_Palette[1];

                Palettes[0] = s.DoAt(OffsetTable.GetPointer(0), () => s.SerializeObject<GBA_Palette>(Palettes[0], name: nameof(Palettes)));
            }
        }

        public enum Milan_CompressionType : byte
        {
            None = 0,
            RL = 1,
            LZSS = 2,
        }
    }
}