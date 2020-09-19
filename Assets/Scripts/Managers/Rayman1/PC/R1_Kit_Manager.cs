using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

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
        public override Archive[] GetArchiveFiles(GameSettings settings)
        {
            return new Archive[]
            {
               new Archive(GetCommonArchiveFilePath()),
               new Archive(GetSoundFilePath()),
               new Archive(GetSoundManifestFilePath()),
               new Archive(GetVignetteFilePath(settings)),
            }.Concat(Directory.GetDirectories(settings.GameDirectory + "PCMAP").Select(Path.GetFileName).SelectMany(x => new Archive[]
            {
                new Archive(GetSamplesArchiveFilePath(x)),
                new Archive(GetSpecialArchiveFilePath(x)),
            })).ToArray();
        }

        /// <summary>
        /// Gets additional sound archives
        /// </summary>
        /// <param name="settings">The game settings</param>
        public override AdditionalSoundArchive[] GetAdditionalSoundArchives(GameSettings settings) => 
            Directory.GetDirectories(settings.GameDirectory + GetDataPath()).
                Select(Path.GetFileName).
                Select(x => new AdditionalSoundArchive($"SMP ({x})", GetSamplesArchiveFilePath(x))).ToArray();

        public override bool IsDESMultiColored(Context context, int desIndex, GeneralEventInfoData[] generalEvents)
        {
            var name = GetDESFileName(context, desIndex);

            var nameWithoutExt = name.Length > 4 ? name.Substring(0, name.Length - 4) : name;

            return generalEvents.Any(x => x.DesKit[context.Settings.R1_World] == nameWithoutExt && ((R1_EventType)x.Type).IsMultiColored());
        }

        public override byte[] GetTypeZDCBytes => R1_PC_ZDCTables.KitPC_Type_ZDC;
        public override byte[] GetZDCTableBytes => R1_PC_ZDCTables.KitPC_ZDCTable;
        public override byte[] GetEventFlagsBytes => R1_PC_EventFlagTables.KitPC_Flags;

        public override UniTask<Texture2D> LoadBackgroundVignetteAsync(Context context, R1_PC_WorldFile world, R1_PC_LevFile level, bool parallax) => 
            UniTask.FromResult(parallax ? null : LoadArchiveFile<PCX>(context, GetVignetteFilePath(context.Settings), world.Plan0NumPcxFiles[level.KitLevelDefines.BG_0])?.ToTexture(true));

        protected override async UniTask<IReadOnlyDictionary<string, string[]>> LoadLocalizationAsync(Context context)
        {
            // Create the dictionary
            var localization = new Dictionary<string, string[]>();

            // Enumerate each language
            foreach (var lang in LoadArchiveFile<R1_PC_VersionFile>(context, GetCommonArchiveFilePath(), R1_PC_ArchiveFileName.VERSION).VersionCodes)
            {
                // Read the text data
                var loc = LoadArchiveFile<R1_PC_LocFile>(context, GetSpecialArchiveFilePath(lang), R1_PC_ArchiveFileName.TEXT);

                // Save the localized name
                var locName = loc?.LanguageNames[loc.LanguageUtilized];

                if (String.IsNullOrWhiteSpace(locName))
                    locName = lang;

                // Add the localization
                if (loc != null)
                    localization.Add($"TEXT ({locName})", loc.TextDefine.Select(x => x.Value).ToArray());

                // Read the general data
                var general = LoadArchiveFile<R1_PC_GeneralFile>(context, GetSpecialArchiveFilePath(lang), R1_PC_ArchiveFileName.GENERAL);

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

        public override IList<ARGBColor> GetBigRayPalette(Context context) => LoadArchiveFile<PCX>(context, GetVignetteFilePath(context.Settings), "FND00")?.VGAPalette;

        public override async UniTask LoadFilesAsync(Context context)
        {
            // Base
            await base.LoadFilesAsync(context);

            // Common
            await AddFile(context, GetCommonArchiveFilePath());
            
            // Special
            foreach (var version in LoadArchiveFile<R1_PC_VersionFile>(context, GetCommonArchiveFilePath(), R1_PC_ArchiveFileName.VERSION).VersionCodes)
                await AddFile(context, GetSpecialArchiveFilePath(version));
        }
    }
}