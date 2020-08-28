
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

        #region Parsed
        public GBA_Palette[] Palettes { get; set; }
        public GBA_AnimatedTileKitManager AnimatedTileKitManager { get; set; }
        public GBA_AnimatedTileKit[] AnimatedTileKits { get; set; }
        #endregion

        public override void SerializeBlock(SerializerObject s) {
            if (s.GameSettings.EngineVersion == EngineVersion.GBA_BatmanVengeance) {
                Is8bpp = s.Serialize<bool>(Is8bpp, name: nameof(Is8bpp));
                IsCompressed = s.Serialize<bool>(IsCompressed, name: nameof(IsCompressed));
                if (Is8bpp) {
                    TileSet8bppSize = s.Serialize<ushort>(TileSet8bppSize, name: nameof(TileSet8bppSize));
                } else {
                    TileSet4bppSize = s.Serialize<ushort>(TileSet4bppSize, name: nameof(TileSet4bppSize));
                }
            } else {
                TileSet4bppSize = s.Serialize<ushort>(TileSet4bppSize, name: nameof(TileSet4bppSize));
                TileSet8bppSize = s.Serialize<ushort>(TileSet8bppSize, name: nameof(TileSet8bppSize));
                Byte_04 = s.Serialize<byte>(Byte_04, name: nameof(Byte_04));
                AnimatedTileKitManagerIndex = s.Serialize<byte>(AnimatedTileKitManagerIndex, name: nameof(AnimatedTileKitManagerIndex)); // Can be 0xFF which means this block doesn't exist
                PaletteCount = s.Serialize<byte>(PaletteCount, name: nameof(PaletteCount));
                Byte_07 = s.Serialize<byte>(Byte_07, name: nameof(Byte_07));
                PaletteIndices = s.SerializeArray<byte>(PaletteIndices, PaletteCount, name: nameof(PaletteIndices));
            }

            // Serialize tilemap data
            if (IsCompressed) {
                s.DoEncoded(new GBA_LZSSEncoder(), () => {
                    TileSet4bpp = s.SerializeArray<byte>(TileSet4bpp, TileSet4bppSize * 0x20, name: nameof(TileSet4bpp));
                    TileSet8bpp = s.SerializeArray<byte>(TileSet8bpp, TileSet8bppSize * 0x40, name: nameof(TileSet8bpp));
                });
                s.Align();
            } else {
                TileSet4bpp = s.SerializeArray<byte>(TileSet4bpp, TileSet4bppSize * 0x20, name: nameof(TileSet4bpp));
                TileSet8bpp = s.SerializeArray<byte>(TileSet8bpp, TileSet8bppSize * 0x40, name: nameof(TileSet8bpp));
            }
        }

        public override void SerializeOffsetData(SerializerObject s)
        {
            if (s.GameSettings.EngineVersion != EngineVersion.GBA_BatmanVengeance) {
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
                    Debug.Log("Level " + s.GameSettings.Level + " (" + s.GameSettings.World + ") has " + AnimatedTileKits.Length + " animated tilekits.");
                }
            }
        }
    }
}