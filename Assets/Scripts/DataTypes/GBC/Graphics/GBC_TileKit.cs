using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class GBC_TileKit : GBC_BaseBlock 
    {
        // Palm OS
        public uint TilesCount { get; set; }
        public byte[] TileData { get; set; }

        // GBC
        public byte PaletteCount { get; set; }
        public ushort PaletteOffset { get; set; }
        public ushort TileDataOffset { get; set; }
        public RGBA5551Color[] Palette { get; set; }

        // Pocket PC
        public BGR565Color[] TileDataPocketPC { get; set; }

        public Texture2D[] GetTileSetTex()
        {
            switch (Context.Settings.EngineVersion)
            {
                case EngineVersion.GBC_R1:
                    return Util.ConvertAndSplitGBCPalette(Palette).Select(p => Util.ToTileSetTexture(TileData, p, Util.TileEncoding.Planar_2bpp, GBC_BaseManager.CellSize, true)).ToArray();

                case EngineVersion.GBC_R1_Palm:

                    bool greyScale = Context.Settings.GameModeSelection == GameModeSelection.RaymanGBCPalmOSGreyscale;
                    if (greyScale) {
                        return new Texture2D[]
                        {
                        Util.ToTileSetTexture(TileData, GBC_R1PalmOS_Manager.GetPalmOS4BitPalette().Select(x => x.GetColor()).ToArray(), Util.TileEncoding.Linear_4bpp_ReverseOrder, GBC_BaseManager.CellSize, true)
                        };
                    } else {
                        return new Texture2D[]
                        {
                        Util.ToTileSetTexture(TileData, GBC_R1PalmOS_Manager.GetPalmOS8BitPalette().Select(x => x.GetColor()).ToArray(), Util.TileEncoding.Linear_8bpp, GBC_BaseManager.CellSize, true)
                        };
                    }

                case EngineVersion.GBC_R1_PocketPC:
                    return new Texture2D[]
                    {
                        Util.ToTileSetTexture(TileDataPocketPC.Select(c => c.Color565 == 0 ? BaseColor.clear : c).ToArray(), GBC_BaseManager.CellSize, true)
                    };

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void SerializeBlock(SerializerObject s)
        {
            // Serialize data
            if (s.GameSettings.EngineVersion == EngineVersion.GBC_R1_PocketPC) {
                TilesCount = s.Serialize<uint>(TilesCount, name: nameof(TilesCount));
                TileDataPocketPC = s.SerializeObjectArray<BGR565Color>(TileDataPocketPC, TilesCount * 8 * 8, name: nameof(TileDataPocketPC));
            } else if (s.GameSettings.EngineVersion == EngineVersion.GBC_R1_Palm) {
                bool greyScale = s.GameSettings.GameModeSelection == GameModeSelection.RaymanGBCPalmOSGreyscale;
                TilesCount = s.Serialize<uint>(TilesCount, name: nameof(TilesCount));
                TileData = s.SerializeArray<byte>(TileData, TilesCount * (greyScale ? 0x20 : 0x40), name: nameof(TileData));
            } else {
                PaletteCount = s.Serialize<byte>(PaletteCount, name: nameof(PaletteCount));
                PaletteOffset = s.Serialize<ushort>(PaletteOffset, name: nameof(PaletteOffset));
                TileDataOffset = s.Serialize<ushort>(TileDataOffset, name: nameof(TileDataOffset));
                TilesCount = s.Serialize<UInt24>((UInt24)TilesCount, name: nameof(TilesCount));
                s.DoAt(BlockStartPointer + PaletteOffset, () => {
                    Palette = s.SerializeObjectArray<RGBA5551Color>(Palette, PaletteCount * 4, name: nameof(Palette));
                });
                s.DoAt(BlockStartPointer + TileDataOffset, () => {
                    TileData = s.SerializeArray<byte>(TileData, TilesCount * 0x10, name: nameof(TileData));
                });
                // Go to end of block
                s.Goto(BlockStartPointer + TileDataOffset + TilesCount * 0x10);
            }
        }
    }
}