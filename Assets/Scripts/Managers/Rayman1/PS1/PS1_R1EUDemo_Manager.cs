using System;
using System.Collections.Generic;
using R1Engine.Serialize;

namespace R1Engine
{
    /// <summary>
    /// The game manager for Rayman 1 (PS1 - EU Demo)
    /// </summary>
    public class PS1_R1EUDemo_Manager : PS1_R1_Manager
    {
        /// <summary>
        /// Gets the folder path for the specified world
        /// </summary>
        /// <param name="world">The world</param>
        /// <returns>The world folder path</returns>
        public override string GetWorldFolderPath(World world) => String.Empty;

        public override string GetLanguageFilePath(string langCode) => $"IMA/RAY{langCode}.TXT";

        /// <summary>
        /// Gets the base path for the game data
        /// </summary>
        /// <returns>The data path</returns>
        public override string GetDataPath() => String.Empty;

        /// <summary>
        /// Gets the file info to use
        /// </summary>
        /// <param name="settings">The game settings</param>
        protected override Dictionary<string, PS1FileInfo> GetFileInfo(GameSettings settings) => PS1FileInfo.fileInfoPALDemo;

        protected override void LoadLocalization(Context context, Common_Lev level)
        {
            // Create the dictionary
            level.Localization = new Dictionary<string, string[]>()
            {
                ["English"] = FileFactory.ReadText<R1_TextLocFile>(GetLanguageFilePath("US"), context).Strings
            };
        }
    }
}