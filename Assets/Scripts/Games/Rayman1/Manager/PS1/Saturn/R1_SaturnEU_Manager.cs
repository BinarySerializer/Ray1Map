using System.Collections.Generic;
using BinarySerializer;
using BinarySerializer.Ray1;
using Cysharp.Threading.Tasks;

namespace Ray1Map.Rayman1
{
    public class R1_SaturnEU_Manager : R1_Saturn_Manager
    {
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
    }
}