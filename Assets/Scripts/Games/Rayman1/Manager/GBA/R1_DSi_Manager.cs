using System;
using System.Collections.Generic;
using BinarySerializer;
using BinarySerializer.Ray1;

namespace Ray1Map.Rayman1
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

        public override KeyValuePair<World, int>[] GetLevelCounts => new KeyValuePair<World, int>[]
        {
            new KeyValuePair<World, int>(World.Jungle, 22),
            new KeyValuePair<World, int>(World.Music, 18),
            new KeyValuePair<World, int>(World.Mountain, 13),
            new KeyValuePair<World, int>(World.Image, 13),
            new KeyValuePair<World, int>(World.Cave, 12),
            new KeyValuePair<World, int>(World.Cake, 4),
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
        public override IGBAData LoadData(Context context) => FileFactory.Read<DSi_DataFile>(GetROMFilePath, context);

        public override KeyValuePair<string, string[]>[] LoadLocalization(IGBAData data)
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

        /// <summary>
        /// Creates a relocated 0.bin file, that is searchable with file offsets in big endian, prefixed with "DD".
        /// e.g.: the bytes 010F1E02 (a pointer to 0x01) become DD000001.
        /// </summary>
        /// <param name="s"></param>
        public void CreateRelocatedFile(SerializerObject s)
        {
            byte[] data = s.SerializeArray<byte>(null, s.CurrentLength, name: "fullfile");
            const uint addr = 0x021E0F00;
            for (int j = 0; j < data.Length; j++)
            {
                if (data[j] != 0x02)
                    continue;

                int off = j - 3;
                uint ptr = BitConverter.ToUInt32(data, off);

                if (ptr < addr || ptr >= addr + data.Length)
                    continue;

                ptr = (ptr - addr) + 0xDD000000;
                byte[] newData = BitConverter.GetBytes(ptr);

                for (int y = 0; y < 4; y++)
                    data[off + 3 - y] = newData[y];

                j += 3;
            }

            Util.ByteArrayToFile(s.Context.GetAbsoluteFilePath("relocated.bin"), data);
        }

        public override void AddContextPointers(Context context)
        {
            context.AddPreDefinedPointers(DSi_DefinedPointers.DSi);
        }

        #endregion
    }
}