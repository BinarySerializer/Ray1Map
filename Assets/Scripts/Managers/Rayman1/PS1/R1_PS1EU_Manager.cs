using System.Collections.Generic;
using BinarySerializer;
using BinarySerializer.Ray1;
using Cysharp.Threading.Tasks;

namespace R1Engine
{
    public class R1_PS1EU_Manager : R1_PS1_Manager
    {
        public override string ExeFilePath => "SLES_000.49";
        public override uint? ExeBaseAddress => 0x80123000 - 0x800;
        protected override PS1_ExecutableConfig GetExecutableConfig => PS1_ExecutableConfig.PS1_EU;

        public string GetLanguageFilePath(string langCode) => GetDataPath() + $"IMA/CRD/RAY{langCode}.TXT";

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

        public override void AddContextPointers(Context context)
        {
            context.AddPreDefinedPointers(PS1_DefinedPointers.PS1_EU);
        }
    }
}