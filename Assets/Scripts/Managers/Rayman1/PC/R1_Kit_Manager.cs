using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace R1Engine
{
    /// <summary>
    /// The game manager for Rayman Designer (PC)
    /// </summary>
    public class R1_Kit_Manager : R1_PCBaseManager
    {
        /// <summary>
        /// Gets the file path for the specified level
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The level file path</returns>
        public override string GetLevelFilePath(GameSettings settings) => GetDataPath() + $"{GetShortWorldName(settings.R1_World)}{settings.Level:00}.LEV";

        /// <summary>
        /// Gets the file path for the specified world file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The world file path</returns>
        public override string GetWorldFilePath(GameSettings settings) => GetDataPath() + $"RAY{settings.World:00}.WLD";

        /// <summary>
        /// Gets the file path for the vignette file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The vignette file path</returns>
        public override string GetVignetteFilePath(GameSettings settings) => GetDataPath() + $"VIGNET.DAT";

        /// <summary>
        /// Gets the file path for the primary sound file
        /// </summary>
        /// <returns>The primary sound file path</returns>
        public override string GetSoundFilePath() => GetDataPath() + $"SNDD8B.DAT";

        /// <summary>
        /// Gets the file path for the primary sound manifest file
        /// </summary>
        /// <returns>The primary sound manifest file path</returns>
        public override string GetSoundManifestFilePath() => GetDataPath() + $"SNDH8B.DAT";

        public virtual string GetVolumeDirectory(string volume) => GetDataPath() + $"{volume}/";
        public virtual string GetSamplesArchiveFilePath(string volume) => GetVolumeDirectory(volume) + "SNDSMP.DAT";
        public virtual string GetSpecialArchiveFilePath(string volume) => GetVolumeDirectory(volume) + "SPECIAL.DAT";
        public virtual string GetCommonArchiveFilePath() => GetDataPath() + "COMMON.DAT";
        public virtual string GetEventLocFilePath(string volume, int index) => GetVolumeDirectory(volume) + $"EVNAME{index:00}.WLD";

        /// <summary>
        /// Gets the levels for each world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The levels</returns>
        public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(WorldHelpers.GetR1Worlds().Select(w => new GameInfo_World((int)w, Directory.EnumerateFiles(settings.GameDirectory + GetDataPath(), $"{GetShortWorldName(w)}??.LEV", SearchOption.TopDirectoryOnly)
            .Select(FileSystem.GetFileNameWithoutExtensions)
            .Select(x => Int32.Parse(x.Substring(3)))
            .ToArray())).ToArray());

        /// <summary>
        /// Gets the DES file name for the current index in the current context
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="desIndex">The DES index</param>
        /// <returns>The file name</returns>
        public string GetDESFileName(Context context, int desIndex)
        {
            // Read the world data
            var worldData = FileFactory.Read<R1_PC_WorldFile>(GetWorldFilePath(context.Settings), context);

            // Get file names
            var desNames = worldData.DESFileNames ?? new string[0];

            // Return the name
            return desNames.ElementAtOrDefault(desIndex) ?? $"DES_{desIndex}";
        }

        /// <summary>
        /// Gets the ETA file name for the current index in the current context
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="etaIndex">The ETA index</param>
        /// <returns>The file name</returns>
        public string GetETAFileName(Context context, int etaIndex)
        {
            // Read the world data
            var worldData = FileFactory.Read<R1_PC_WorldFile>(GetWorldFilePath(context.Settings), context);

            // Get file names
            var etaNames = worldData.ETAFileNames ?? new string[0];

            // Return the name
            return etaNames.ElementAtOrDefault(etaIndex) ?? $"ETA_{etaIndex}";
        }

        /// <summary>
        /// Gets the archive files which can be extracted
        /// </summary>
        public override ArchiveFile[] GetArchiveFiles(GameSettings settings)
        {
            return new ArchiveFile[]
            {
                new ArchiveFile(GetCommonArchiveFilePath()),
                new ArchiveFile($"PCMAP/SNDD8B.DAT"),
                new ArchiveFile($"PCMAP/SNDH8B.DAT"),
                new ArchiveFile($"PCMAP/VIGNET.DAT", ".pcx"),
            }.Concat(Directory.GetDirectories(settings.GameDirectory + "PCMAP").Select(Path.GetFileName).SelectMany(x => new ArchiveFile[]
            {
                new ArchiveFile(GetSamplesArchiveFilePath(x), volume: x),
                new ArchiveFile(GetSpecialArchiveFilePath(x), volume: x),
            })).ToArray();
        }

        /// <summary>
        /// Gets additional sound archives
        /// </summary>
        /// <param name="settings">The game settings</param>
        public override AdditionalSoundArchive[] GetAdditionalSoundArchives(GameSettings settings) => 
            Directory.GetDirectories(settings.GameDirectory + "PCMAP").
                Select(Path.GetFileName).
                Select(x => new AdditionalSoundArchive($"SMP ({x})", new ArchiveFile($"PCMAP/{x}/SNDSMP.DAT"))).ToArray();

        public override bool IsDESMultiColored(Context context, int desIndex, GeneralEventInfoData[] generalEvents)
        {
            var name = GetDESFileName(context, desIndex);

            var nameWithoutExt = name.Length > 4 ? name.Substring(0, name.Length - 4) : name;

            return generalEvents.Any(x => x.DesKit[context.Settings.R1_World] == nameWithoutExt && ((R1_EventType)x.Type).IsMultiColored());
        }

        protected override async UniTask<IReadOnlyDictionary<string, string[]>> LoadLocalizationAsync(Context context)
        {
            // Create the dictionary
            var localization = new Dictionary<string, string[]>();

            // Enumerate each language
            foreach (var lang in FileFactory.Read<R1_PC_VersionFile>("VERSION", context).VersionCodes)
            {
                // Read the text data
                var loc = ReadArchiveFile<R1_PC_LocFile>(context, R1_PC_ArchiveFileName.TEXT, lang);

                // Save the localized name
                var locName = loc?.LanguageNames[loc.LanguageUtilized];

                if (String.IsNullOrWhiteSpace(locName))
                    locName = lang;

                // Add the localization
                if (loc != null)
                    localization.Add($"TEXT ({locName})", loc.TextDefine.Select(x => x.Value).ToArray());

                // Read the general data
                var general = ReadArchiveFile<R1_PC_GeneralFile>(context, R1_PC_ArchiveFileName.GENERAL, lang);

                // Add the localization
                if (general != null)
                    localization.Add($"GENERAL ({locName})", general.CreditsStringItems.Select(x => x.String.Value).ToArray());

                // Add the event localizations (allfix + 6 worlds)
                for (int i = 0; i < 7; i++)
                {
                    // Get the file path
                    var evLocPath = GetEventLocFilePath(lang, i);

                    // Add the file to the context
                    await AddFile(context, evLocPath);

                    if (!FileSystem.FileExists(context.BasePath + evLocPath))
                        continue;

                    // Read the file
                    var evLoc = FileFactory.Read<R1_Mapper_EventLocFile>(evLocPath, context);

                    // Add the localization
                    localization.Add($"EVNAME{i:00} ({locName})", evLoc.LocItems.Select(x => $"{x.LocKey}: {x.Name} - {x.Description}").ToArray());
                }
            }

            return localization;
        }

        public override IList<ARGBColor> GetBigRayPalette(Context context) => ReadArchiveFile<PCX>(context, "FND00")?.VGAPalette;

        public override async UniTask LoadArchivesAsync(Context context)
        {
            // Load the vignette archive
            await LoadArchiveAsync(context, GetVignetteFilePath(context.Settings), null);

            // Load the common archive
            await LoadArchiveAsync(context, GetCommonArchiveFilePath(), null);

            // Get the available languages (volumes)
            var version = ReadArchiveFile<R1_PC_VersionFile>(context, R1_PC_ArchiveFileName.VERSION);

            // Load the special archive for every version
            foreach (var versionCode in version.VersionCodes)
                await LoadArchiveAsync(context, GetSpecialArchiveFilePath(versionCode), versionCode);
        }
    }
}