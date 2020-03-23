using R1Engine.Serialize;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace R1Engine
{
    /// <summary>
    /// The game manager for Rayman Designer (PC)
    /// </summary>
    public class PC_RD_Manager : PC_Manager
    {
        #region Values and paths

        /// <summary>
        /// Gets the file path for the specified level
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The level file path</returns>
        public override string GetLevelFilePath(GameSettings settings) => GetDataPath() + $"{GetShortWorldName(settings.World)}{settings.Level:00}.LEV";

        /// <summary>
        /// Gets the file path for the specified world file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The world file path</returns>
        public override string GetWorldFilePath(GameSettings settings) => GetDataPath() + $"RAY{((int)settings.World + 1):00}.WLD";

        /// <summary>
        /// Gets the file path for the vignette file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The vignette file path</returns>
        public override string GetVignetteFilePath(GameSettings settings) => GetDataPath() + $"VIGNET.DAT";

        /// <summary>
        /// Indicates if the game has 3 palettes it swaps between
        /// </summary>
        public override bool Has3Palettes => false;

        /// <summary>
        /// Gets the levels for the specified world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The levels</returns>
        public override int[] GetLevels(GameSettings settings) => Enumerable.Range(1, Directory.EnumerateFiles(settings.GameDirectory + "/" + GetDataPath(), $"{GetShortWorldName(settings.World)}??.LEV", SearchOption.TopDirectoryOnly).Count<string>()).ToArray<int>();

        /// <summary>
        /// Gets the DES file names, in order, for the world
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The DES file names</returns>
        public override IEnumerable<string> GetDESNames(Context context)
        {
            return EnumerateWLDManifest(context).Where<string>(str => str.Contains("DES"));
        }

        /// <summary>
        /// Gets the ETA file names, in order, for the world
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The ETA file names</returns>
        public override IEnumerable<string> GetETANames(Context context)
        {
            return EnumerateWLDManifest(context).Where<string>(str => str.Contains("ETA"));
        }

        /// <summary>
        /// Enumerates the strings in a .wld manifest
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The found strings</returns>
        protected IEnumerable<string> EnumerateWLDManifest(Context context)
        {
            // Get the encoding
            var e = Settings.StringEncoding;

            // TODO: Find better way to parse this
            // Read the world file and get the last data group
            var wld = FileFactory.Read<PC_WorldFile>(GetWorldFilePath(context.Settings), context,
                data => data.FileType = PC_WorldFile.Type.World).Unknown5;

            // Get the DES file names
            for (int i = 1; i < wld.Length; i += 13)
            {
                // Read the bytes until we reach NULL
                var length = 0;

                for (int j = 0; j < 13; j++, length++)
                {
                    if (wld[i + j] == 0x00)
                        break;
                }

                // Get the string
                var str = e.GetString(wld, i, length);

                // Return it
                yield return str;
            }
        }

        #endregion
    }
}