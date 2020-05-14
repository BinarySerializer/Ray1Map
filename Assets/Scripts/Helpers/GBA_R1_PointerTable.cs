using System;
using System.Collections.Generic;
using System.Linq;
using R1Engine.Serialize;

namespace R1Engine
{
    /// <summary>
    /// Pointer table data for GBA
    /// </summary>
    public static class GBA_R1_PointerTable
    {
        /// <summary>
        /// Gets the pointer table for the specified GBA version
        /// </summary>
        /// <param name="gameMode">The GBA game mode</param>
        /// <param name="romFile">The ROM file</param>
        /// <returns>The pointer table</returns>
        public static Dictionary<GBA_R1_ROMPointer, Pointer> GetPointerTable(GameModeSelection gameMode, BinaryFile romFile)
        {
            if (gameMode == GameModeSelection.RaymanAdvanceGBAEU)
            {
                return new Dictionary<GBA_R1_ROMPointer, uint>()
                {
                    [GBA_R1_ROMPointer.Levels] = 0x085485B4,
                    [GBA_R1_ROMPointer.BackgroundVignette] = 0x086D4D60,
                    [GBA_R1_ROMPointer.IntroVignette] = 0x080F7968,
                    [GBA_R1_ROMPointer.SpritePalettes] = 0x08548F56,
                    [GBA_R1_ROMPointer.EventGraphicsPointers] = 0x081A62E4,
                    [GBA_R1_ROMPointer.EventDataPointers] = 0x081A6444,
                    [GBA_R1_ROMPointer.EventGraphicsGroupCountTablePointers] = 0x081A65A4,
                    [GBA_R1_ROMPointer.LevelEventGraphicsGroupCounts] = 0x081A6704,
                    [GBA_R1_ROMPointer.WorldLevelOffsetTable] = 0x081539A4,
                }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(gameMode), gameMode, null);
            }
        }
    }
}