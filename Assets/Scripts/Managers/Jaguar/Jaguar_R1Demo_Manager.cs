using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using R1Engine.Serialize;

namespace R1Engine
{
    /// <summary>
    /// Game manager for Jaguar
    /// </summary>
    public class Jaguar_R1Demo_Manager : Jaguar_R1_Manager
    {
        #region Values and paths

        /// <summary>
        /// Gets the levels for each world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The levels</returns>
        public override KeyValuePair<World, int[]>[] GetLevels(GameSettings settings) => new KeyValuePair<World, int[]>[]
        {
            // Hard-code this since most levels don't have maps we can load
            new KeyValuePair<World, int[]>(World.Jungle, Enumerable.Range(1, 14).ToArray()), 
            new KeyValuePair<World, int[]>(World.Music, Enumerable.Range(1, 5).ToArray()), 
        };

        /// <summary>
        /// Gets the file path to the ROM file
        /// </summary>
        public override string GetROMFilePath => $"ROM.rom";

        /// <summary>
        /// Gets the base address for the ROM file
        /// </summary>
        protected override uint GetROMBaseAddress => 0x00802000;

        /// <summary>
        /// Gets the available levels ordered based on the global level array
        /// </summary>
        public override KeyValuePair<World, int>[] GetNumLevels => new KeyValuePair<World, int>[]
        {
            new KeyValuePair<World, int>(World.Jungle, 14),
            new KeyValuePair<World, int>(World.Mountain, 4),
            new KeyValuePair<World, int>(World.Cave, 2),
            new KeyValuePair<World, int>(World.Music, 5),
            new KeyValuePair<World, int>(World.Image, 2),
            new KeyValuePair<World, int>(World.Cake, 2)
        };

        public override int[] ExtraMapCommands => new int[] {
            0, 1, 2
        };

        /// <summary>
        /// Gets the vignette addresses and widths
        /// </summary>
        protected override KeyValuePair<uint, int>[] GetVignette => new KeyValuePair<uint, int>[]
        {
            // World map
            new KeyValuePair<uint, int>(0x875BC0, 320), 
            
            // Breakout
            new KeyValuePair<uint, int>(0x8855F8, 320), 

            // Jungle
            new KeyValuePair<uint, int>(0x8B083E, 192), 
            new KeyValuePair<uint, int>(0x8D735E, 144), 
            new KeyValuePair<uint, int>(0x8D8F8A, 48), 

            // Music
            new KeyValuePair<uint, int>(0x8F64E6, 384), 
        };

        #endregion
    }
}