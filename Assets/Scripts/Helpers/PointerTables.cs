using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.Linq;

namespace R1Engine
{
    /// <summary>
    /// Pointer tables for rom games
    /// </summary>
    public static class PointerTables
    {
        /// <summary>
        /// Gets the pointer table for the specified GBA version
        /// </summary>
        /// <param name="gameMode">The GBA game mode</param>
        /// <param name="romFile">The ROM file</param>
        /// <returns>The pointer table</returns>
        public static Dictionary<GBA_R1_ROMPointer, Pointer> GetGBAPointerTable(GameModeSelection gameMode, BinaryFile romFile)
        {
            if (gameMode == GameModeSelection.RaymanAdvanceGBAUS)
            {
                return new Dictionary<GBA_R1_ROMPointer, uint>()
                {
                    [GBA_R1_ROMPointer.LevelMaps] = 0x08548688,
                    [GBA_R1_ROMPointer.BackgroundVignette] = 0x086D4E34,
                    [GBA_R1_ROMPointer.IntroVignette] = 0x080F7A04,
                    [GBA_R1_ROMPointer.SpritePalettes] = 0x0854902A,

                    [GBA_R1_ROMPointer.EventGraphicsPointers] = 0x081A63B8,
                    [GBA_R1_ROMPointer.EventDataPointers] = 0x081A6518,
                    [GBA_R1_ROMPointer.EventGraphicsGroupCountTablePointers] = 0x081A6678,
                    [GBA_R1_ROMPointer.LevelEventGraphicsGroupCounts] = 0x081A67D8,
                    
                    [GBA_R1_ROMPointer.WorldLevelOffsetTable] = 0x08153A40,

                    [GBA_R1_ROMPointer.WorldMapVignetteImageData] = 0x081452A4,
                    [GBA_R1_ROMPointer.WorldMapVignetteBlockIndices] = 0x08151504,
                    [GBA_R1_ROMPointer.WorldMapVignettePaletteIndices] = 0x08152284,
                    [GBA_R1_ROMPointer.WorldMapVignettePalettes] = 0x08152944,

                    [GBA_R1_ROMPointer.StringPointers] = 0x0854ADB4,

                    [GBA_R1_ROMPointer.DrumWalkerGraphics] = 0x082C6C5C,
                    [GBA_R1_ROMPointer.ClockGraphics] = 0x082C90C8,
                    [GBA_R1_ROMPointer.InkGraphics] = 0x082D33D0,
                    [GBA_R1_ROMPointer.FontSmallGraphics] = 0x082E74F4,
                    [GBA_R1_ROMPointer.FontLargeGraphics] = 0x082E7514,
                    [GBA_R1_ROMPointer.PinsGraphics] = 0x0832CBF4,
                }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));
            }
            else if (gameMode == GameModeSelection.RaymanAdvanceGBAEU || gameMode == GameModeSelection.RaymanAdvanceGBAEUBeta)
            {
                var offset = gameMode == GameModeSelection.RaymanAdvanceGBAEU ? 0 : 0xC;

                return new Dictionary<GBA_R1_ROMPointer, uint>()
                {
                    [GBA_R1_ROMPointer.LevelMaps] = 0x085485B4,
                    [GBA_R1_ROMPointer.BackgroundVignette] = 0x086D4D60,
                    [GBA_R1_ROMPointer.IntroVignette] = 0x080F7968,
                    [GBA_R1_ROMPointer.SpritePalettes] = 0x08548F56,
                    
                    [GBA_R1_ROMPointer.EventGraphicsPointers] = 0x081A62E4,
                    [GBA_R1_ROMPointer.EventDataPointers] = 0x081A6444,
                    [GBA_R1_ROMPointer.EventGraphicsGroupCountTablePointers] = 0x081A65A4,
                    [GBA_R1_ROMPointer.LevelEventGraphicsGroupCounts] = 0x081A6704,
                    
                    [GBA_R1_ROMPointer.WorldLevelOffsetTable] = 0x081539A4,

                    [GBA_R1_ROMPointer.WorldMapVignetteImageData] = 0x08145208,
                    [GBA_R1_ROMPointer.WorldMapVignetteBlockIndices] = 0x08151468,
                    [GBA_R1_ROMPointer.WorldMapVignettePaletteIndices] = 0x081521E8,
                    [GBA_R1_ROMPointer.WorldMapVignettePalettes] = 0x081528A8,

                    [GBA_R1_ROMPointer.StringPointers] = 0x0854ACE0,

                    [GBA_R1_ROMPointer.DrumWalkerGraphics] = 0x082C6B88,
                    [GBA_R1_ROMPointer.ClockGraphics] = 0x082C8FF4,
                    [GBA_R1_ROMPointer.InkGraphics] = 0x082D32FC,
                    [GBA_R1_ROMPointer.FontSmallGraphics] = 0x082E7420,
                    [GBA_R1_ROMPointer.FontLargeGraphics] = 0x082E7440,
                    [GBA_R1_ROMPointer.PinsGraphics] = 0x0832CB20,
                }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile) - offset);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(gameMode), gameMode, null);
            }
        }

        /// <summary>
        /// Gets the pointer table for the specified DSi version
        /// </summary>
        /// <param name="gameMode">The DSi game mode</param>
        /// <param name="dataFile">The data file</param>
        /// <returns>The pointer table</returns>
        public static Dictionary<DSi_R1_Pointer, Pointer> GetDSiPointerTable(GameModeSelection gameMode, BinaryFile dataFile)
        {
            return new Dictionary<DSi_R1_Pointer, uint>()
            {
                [DSi_R1_Pointer.JungleMaps] = 0x0226C6B4,
                [DSi_R1_Pointer.LevelMaps] = 0x02361968,
                [DSi_R1_Pointer.BackgroundVignette] = 0x025A1478,
                [DSi_R1_Pointer.SpecialPalettes] = 0x02268FEC,

                [DSi_R1_Pointer.StringPointers] = 0x022604D0,

                [DSi_R1_Pointer.EventGraphicsPointers] = 0x0284B5B0,
                [DSi_R1_Pointer.EventDataPointers] = 0x0284B6F8,
                [DSi_R1_Pointer.EventGraphicsGroupCountTablePointers] = 0x0284B988,
                [DSi_R1_Pointer.LevelEventGraphicsGroupCounts] = 0x0284B840,
            }.ToDictionary(x => x.Key, x => new Pointer(x.Value, dataFile));
        }
    }
}