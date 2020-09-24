using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using R1Engine.Serialize;

namespace R1Engine
{
    public class R1_PS1EU_Manager : R1_PS1_Manager
    {
        public override string GetExeFilePath => "SLES_000.49";

        public string GetLanguageFilePath(string langCode) => GetDataPath() + $"IMA/CRD/RAY{langCode}.TXT";

        protected override Dictionary<string, PS1FileInfo> GetFileInfo(GameSettings settings) => PS1FileInfo.fileInfoPAL;

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

        public override uint? TypeZDCOffset => 0x9F754;
        public override uint? ZDCDataOffset => 0x9E754;
        public override uint? EventFlagsOffset => 0x9DF54;
    }
}