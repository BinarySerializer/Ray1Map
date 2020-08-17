using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace R1Engine
{
    /// <summary>
    /// The game manager for Rayman Ultimate (Pocket PC)
    /// </summary>
    public class R1_PocketPC_Manager : R1_PC_Manager
    {
        #region Values and paths

        /// <summary>
        /// Gets the levels for each world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The levels</returns>
        public override KeyValuePair<int, int[]>[] GetLevels(GameSettings settings) => WorldHelpers.GetR1Worlds().Select(w => new KeyValuePair<int, int[]>((int)w, Directory.EnumerateFiles(settings.GameDirectory + GetWorldFolderPath(w), $"{GetShortWorldName(w)}??.lev.gz", SearchOption.TopDirectoryOnly)
            .Select(FileSystem.GetFileNameWithoutExtensions)
            .Select(x => Int32.Parse(x.Substring(3)))
            .ToArray())).ToArray();

        /// <summary>
        /// Gets the file path for the big ray file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The big ray file path</returns>
        public override string GetBigRayFilePath(GameSettings settings) => GetDataPath() + $"bray.dat.gz";

        /// <summary>
        /// Gets the file path for the vignette file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The vignette file path</returns>
        public override string GetVignetteFilePath(GameSettings settings) => throw new NotImplementedException();

        /// <summary>
        /// Gets the file path for the allfix file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The allfix file path</returns>
        public override string GetAllfixFilePath(GameSettings settings) => GetDataPath() + $"allfix.dat.gz";

        /// <summary>
        /// Gets the file path for the specified level
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The level file path</returns>
        public override string GetLevelFilePath(GameSettings settings) => GetWorldFolderPath(settings.R1_World) + $"{GetShortWorldName(settings.R1_World)}{settings.Level}.lev.gz";

        /// <summary>
        /// Gets the file path for the specified world file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The world file path</returns>
        public override string GetWorldFilePath(GameSettings settings) => GetDataPath() + $"ray{settings.World}.wld.gz";

        /// <summary>
        /// Gets the short name for the world
        /// </summary>
        /// <returns>The short world name</returns>
        public override string GetShortWorldName(R1_World world)
        {
            switch (world)
            {
                case R1_World.Jungle:
                    return "JUN";
                case R1_World.Music:
                    return "MUS";
                case R1_World.Mountain:
                    return "MON";
                case R1_World.Image:
                    return "IMG";
                case R1_World.Cave:
                    return "CAV";
                case R1_World.Cake:
                    return "CAK";
                default:
                    throw new ArgumentOutOfRangeException(nameof(world), world, null);
            }
        }

        #endregion

        #region Manager Methods

        /// <summary>
        /// Gets a binary file to add to the context
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="filePath">The file path</param>
        /// <returns>The binary file</returns>
        protected override BinaryFile GetFile(Context context, string filePath) => new GzipCompressedFile(context)
        {
            filePath = filePath
        };

        protected override void LoadLocalization(Context context, Unity_Level level)
        {
            // Read the language file
            var lng = FileFactory.ReadText<R1_PC_LNGFile>(GetLanguageFilePath(), context);

            // Set the common localization
            level.Localization = new Dictionary<string, string[]>()
            {
                ["English1"] = lng.Strings[0],
                ["English2"] = lng.Strings[1],
                ["English3"] = lng.Strings[2],
                ["French"] = lng.Strings[3],
                ["German"] = lng.Strings[4],
            };
        }

        #endregion
    }
}