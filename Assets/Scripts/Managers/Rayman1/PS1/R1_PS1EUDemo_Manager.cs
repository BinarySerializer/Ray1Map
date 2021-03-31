using System;
using System.Collections.Generic;
using BinarySerializer;
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
        public override string GetWorldFolderPath(R1_World world) => String.Empty;

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

            await FileSystem.PrepareFile(context.BasePath + filePath);

            // Create the dictionary
            return new KeyValuePair<string, string[]>[]
            {
                new KeyValuePair<string, string[]>("English", R1FileFactory.ReadText<R1_TextLocFile>(filePath, context).Strings)
            };
        }

        public override uint? TypeZDCOffset => ExeBaseAddress + 0x93998;
        public override uint? ZDCDataOffset => ExeBaseAddress + 0x92998;
        public override uint? EventFlagsOffset => ExeBaseAddress + 0x92198;
        public override uint? LevelBackgroundIndexTableOffset => ExeBaseAddress + 0x9500C;
        //public override uint? WorldInfoOffset => ExeBaseAddress + 0x; // TODO: Find offset

        public override FileTableInfo[] FileTableInfos => new FileTableInfo[]
        {
            new FileTableInfo(0x801c42dc,3,R1_PS1_FileType.img_file),
            new FileTableInfo(0x801c4228,2,R1_PS1_FileType.ldr_file),
            new FileTableInfo(0x801c44d4,2,R1_PS1_FileType.div_file),
            new FileTableInfo(0x801c3fa0,0x12,R1_PS1_FileType.vdo_file),
            new FileTableInfo(0x801c6118,0x35,R1_PS1_FileType.trk_file),
            new FileTableInfo(0x801c451c,3,R1_PS1_FileType.lang_file),
            new FileTableInfo(0x801c4348,5,R1_PS1_FileType.pre_file),
            new FileTableInfo(0x801c43fc,6,R1_PS1_FileType.crd_file),
            new FileTableInfo(0x801c4588,6,R1_PS1_FileType.gam_file),
            new FileTableInfo(0x801c4660,6,R1_PS1_FileType.vig_wld_file),
            new FileTableInfo(0x801c4a2c,6,R1_PS1_FileType.wld_file),
            new FileTableInfo(0x801c4b04,0x7e,R1_PS1_FileType.map_file),
            new FileTableInfo(0x801c5cbc,0x1f,R1_PS1_FileType.fnd_file),
            new FileTableInfo(0x801c4834,7,R1_PS1_FileType.vab_file),
            new FileTableInfo(0x801c4738,7,R1_PS1_FileType.big_file),
            new FileTableInfo(0x801c4930,7,R1_PS1_FileType.vab4sep_file),
            new FileTableInfo(0x801c4294,2,R1_PS1_FileType.filefxs),
            new FileTableInfo(0x801c4270,1,R1_PS1_FileType.ini_file),
        };
    }
}