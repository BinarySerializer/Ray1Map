using System.Collections.Generic;
using BinarySerializer;


namespace R1Engine
{
    /// <summary>
    /// Base game manager for DSi
    /// </summary>
    public class R1_DSi_Manager : R1_GBA_Manager
    {
        #region Values and paths

        /// <summary>
        /// The amount of levels in the game
        /// </summary>
        public new const int LevelCount = 22 + 18 + 13 + 13 + 12 + 4;

        public override KeyValuePair<R1_World, int>[] GetLevelCounts => new KeyValuePair<R1_World, int>[]
        {
            new KeyValuePair<R1_World, int>(R1_World.Jungle, 22),
            new KeyValuePair<R1_World, int>(R1_World.Music, 18),
            new KeyValuePair<R1_World, int>(R1_World.Mountain, 13),
            new KeyValuePair<R1_World, int>(R1_World.Image, 13),
            new KeyValuePair<R1_World, int>(R1_World.Cave, 12),
            new KeyValuePair<R1_World, int>(R1_World.Cake, 4),
        };

        /// <summary>
        /// Gets the file path to the ROM file
        /// </summary>
        public override string GetROMFilePath => $"0.bin";

        /// <summary>
        /// Gets the base address for the ROM file
        /// </summary>
        protected override uint GetROMBaseAddress => 0x021E0F00;

        /// <summary>
        /// True if colors are 4-bit, false if they're 8-bit
        /// </summary>
        public override bool Is4Bit => false;

        /// <summary>
        /// True if palette indexes are used, false if not
        /// </summary>
        public override bool UsesPaletteIndex => false;

        #endregion

        #region Manager Methods

        /// <summary>
        /// Loads the game data
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The game data</returns>
        public override IR1_GBAData LoadData(Context context) => FileFactory.Read<R1_DSi_DataFile>(GetROMFilePath, context);

        public override KeyValuePair<string, string[]>[] LoadLocalization(IR1_GBAData data)
        {
            return new KeyValuePair<string, string[]>[]
            {
                new KeyValuePair<string, string[]>("English", data.Strings[1]),
                new KeyValuePair<string, string[]>("French", data.Strings[2]),
                new KeyValuePair<string, string[]>("German", data.Strings[4]),
                new KeyValuePair<string, string[]>("Spanish", data.Strings[0]),
                new KeyValuePair<string, string[]>("Italian", data.Strings[3]),
            };
        }

        /// <summary>
        /// Gets the available game actions
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The game actions</returns>
        public override GameAction[] GetGameActions(GameSettings settings) {
            return new GameAction[]
            {
                new GameAction("Export Sprites", false, true, (input, output) => ExportAllSpritesAsync(settings, output)),
                new GameAction("Export Vignette", false, true, (input, output) => ExtractVignetteAsync(settings, output)),
                new GameAction("Export Palettes", false, true, (input, output) => ExportPaletteImage(settings, output)),
            };
        }

        #endregion
    }
}