using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using R1Engine.Serialize;

namespace R1Engine
{
    public class R1_SaturnEU_Manager : R1_Saturn_Manager
    {
        public override uint GetPalOffset => 0x78D14;

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

        public override uint? TypeZDCOffset => 0x7EB22;
        public override uint? ZDCDataOffset => 0x7DB22;
        public override uint? EventFlagsOffset => 0x7D320;
    }
}