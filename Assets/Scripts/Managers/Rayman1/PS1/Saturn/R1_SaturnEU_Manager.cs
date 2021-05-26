using System.Collections.Generic;
using BinarySerializer;
using BinarySerializer.Ray1;
using Cysharp.Threading.Tasks;

namespace R1Engine
{
    public class R1_SaturnEU_Manager : R1_Saturn_Manager
    {
        public override uint GetPalOffset => 0x78D14;
        public override uint GetFndFileTableOffset => 0x8142C;
        public override uint GetFndSPFileTableOffset => 0x81823;
        public override uint GetFndIndexTableOffset => 0x8175B;

        protected override async UniTask<KeyValuePair<string, string[]>[]> LoadLocalizationAsync(Context context)
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
            var loc = new List<KeyValuePair<string, string[]>>();

            // Add each language
            foreach (var lang in langs)
            {
                var filePath = GetLanguageFilePath(lang.LangCode);
                await FileSystem.PrepareFile(context.GetAbsoluteFilePath(filePath));
                var langFile = Ray1TextFileFactory.ReadText<TextLocFile>(filePath, context);
                loc.Add(new KeyValuePair<string, string[]>(lang.Language, langFile.Strings));
            }

            return loc.ToArray();
        }

        public override uint? TypeZDCOffset => 0x7EB22;
        public override uint? ZDCDataOffset => 0x7DB22;
        public override uint? EventFlagsOffset => 0x7D320;
        public override uint? WorldInfoOffset => 0x7F3F0;
    }
}