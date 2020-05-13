using System;
using System.Collections.Generic;

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
        /// <returns>The pointer table</returns>
        public static Dictionary<GBA_R1_ROMPointer, uint> GetPointerTable(GameModeSelection gameMode)
        {
            if (gameMode == GameModeSelection.RaymanAdvanceGBAEU)
            {
                return new Dictionary<GBA_R1_ROMPointer, uint>()
                {
                    [GBA_R1_ROMPointer.Levels] = 0x085485B4,
                    [GBA_R1_ROMPointer.BackgroundVignette] = 0x086D4D60,
                    [GBA_R1_ROMPointer.IntroVignette] = 0x080F7968,
                    [GBA_R1_ROMPointer.SpritePalettes] = 0x08548F56,
                    [GBA_R1_ROMPointer.UnkLevelPointerArray1] = 0x081A62E4,
                    [GBA_R1_ROMPointer.UnkLevelPointerArray2] = 0x081A6444,
                    [GBA_R1_ROMPointer.UnkLevelPointerArray3] = 0x081A65A4,
                    [GBA_R1_ROMPointer.UnkLevelDwordArray] = 0x081A6704,
                };
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(gameMode), gameMode, null);
            }
        }
    }
}