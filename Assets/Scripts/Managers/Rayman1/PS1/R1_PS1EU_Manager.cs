using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using R1Engine.Serialize;

namespace R1Engine
{
    public class R1_PS1EU_Manager : R1_PS1_Manager
    {
        public override string ExeFilePath => "SLES_000.49";
        public override uint? ExeBaseAddress => 0x80123000 - 0x800;

        public string GetLanguageFilePath(string langCode) => GetDataPath() + $"IMA/CRD/RAY{langCode}.TXT";

        protected override async UniTask<IReadOnlyDictionary<string, string[]>> LoadLocalizationAsync(Context context)
        {
            var langs = new[]
            {
                new
                {
                    LangCode = "US",
                    Language = "English"
                },
                new
                {
                    LangCode = "FR",
                    Language = "French"
                },
                new
                {
                    LangCode = "GR",
                    Language = "German"
                },
            };

            // Create the dictionary
            var loc = new Dictionary<string, string[]>();

            // Add each language
            foreach (var lang in langs)
            {
                var filePath = GetLanguageFilePath(lang.LangCode);
                await FileSystem.PrepareFile(context.BasePath + filePath);
                var langFile = FileFactory.ReadText<R1_TextLocFile>(filePath, context);
                loc.Add(lang.Language, langFile.Strings);
            }

            return loc;
        }

        public override uint? TypeZDCOffset => ExeBaseAddress + 0x9F754;
        public override uint? ZDCDataOffset => ExeBaseAddress + 0x9E754;
        public override uint? EventFlagsOffset => ExeBaseAddress + 0x9DF54;
        public override uint? LevelBackgroundIndexTableOffset => ExeBaseAddress + 0xA0DC8;

        public override FileTableInfo[] FileTableInfos => new FileTableInfo[]
        {
            new FileTableInfo(0x801c3d5c,3,R1_PS1_FileType.img_file),
            new FileTableInfo(0x801c3dc8,2,R1_PS1_FileType.ldr_file),
            new FileTableInfo(0x801c3e10,2,R1_PS1_FileType.div_file),
            new FileTableInfo(0x801c3e58,0x12,R1_PS1_FileType.vdo_file),
            new FileTableInfo(0x801c40e0,0x35,R1_PS1_FileType.trk_file),
            new FileTableInfo(0x801c4854,3,R1_PS1_FileType.lang_file),
            new FileTableInfo(0x801c48c0,5,R1_PS1_FileType.pre_file),
            new FileTableInfo(0x801c4974,6,R1_PS1_FileType.crd_file),
            new FileTableInfo(0x801c4a4c,6,R1_PS1_FileType.gam_file),
            new FileTableInfo(0x801c4b24,6,R1_PS1_FileType.vig_wld_file),
            new FileTableInfo(0x801c4bfc,6,R1_PS1_FileType.wld_file),
            new FileTableInfo(0x801c4cd4,0x7e,R1_PS1_FileType.map_file),
            new FileTableInfo(0x801c5e8c,0x1f,R1_PS1_FileType.fnd_file),
            new FileTableInfo(0x801c62e8,7,R1_PS1_FileType.vab_file),
            new FileTableInfo(0x801c63e4,7,R1_PS1_FileType.big_file),
            new FileTableInfo(0x801c64e0,7,R1_PS1_FileType.vab4sep_file),
            new FileTableInfo(0x801c65dc,2,R1_PS1_FileType.filefxs),
            new FileTableInfo(0x801c6624,1,R1_PS1_FileType.ini_file),
        };
    }
}