using System;
using System.Collections.Generic;
using BinarySerializer;
using BinarySerializer.Ray1;
using Cysharp.Threading.Tasks;

namespace R1Engine
{
    /// <summary>
    /// The game manager for Rayman 1 (PS1 - EU Demo)
    /// </summary>
    public class R1_PS1EUDemo_Manager : R1_PS1_Manager
    {
        /// <summary>
        /// Gets the folder path for the specified world
        /// </summary>
        /// <param name="world">The world</param>
        /// <returns>The world folder path</returns>
        public override string GetWorldFolderPath(World world) => String.Empty;
        protected override PS1_ExecutableConfig GetExecutableConfig => PS1_ExecutableConfig.PS1_EUDemo;

        public string GetLanguageFilePath(string langCode) => $"IMA/RAY{langCode}.TXT";

        /// <summary>
        /// Gets the base path for the game data
        /// </summary>
        /// <returns>The data path</returns>
        public override string GetDataPath() => String.Empty;

        public override string ExeFilePath => "RAY.EXE";
        public override uint? ExeBaseAddress => 0x8012F000 - 0x800;

        protected override async UniTask<KeyValuePair<string, string[]>[]> LoadLocalizationAsync(Context context)
        {
            var filePath = GetLanguageFilePath("US");

            await FileSystem.PrepareFile(context.GetAbsoluteFilePath(filePath));

            // Create the dictionary
            return new KeyValuePair<string, string[]>[]
            {
                new KeyValuePair<string, string[]>("English", Ray1TextFileFactory.ReadText<TextLocFile>(filePath, context).Strings)
            };
        }

        public override void AddContextPointers(Context context)
        {
            context.AddPreDefinedPointers(PS1_DefinedPointers.PS1_EUDemo);
        }
    }
}