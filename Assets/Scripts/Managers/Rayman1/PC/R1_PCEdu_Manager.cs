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
    /// The game manager for Rayman Educational (PC)
    /// </summary>
    public class R1_PCEdu_Manager : R1_PCBaseManager
    {
        #region Values and paths

        /// <summary>
        /// Gets the file path for the specified level
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The level file path</returns>
        public override string GetLevelFilePath(GameSettings settings) => GetVolumePath(settings.EduVolume) + $"{GetShortWorldName(settings.R1_World)}{settings.Level:00}.LEV";

        /// <summary>
        /// Gets the file path for the specified world file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The world file path</returns>
        public override string GetWorldFilePath(GameSettings settings) => GetVolumePath(settings.EduVolume) + $"RAY{settings.World:00}.WLD";

        /// <summary>
        /// Gets the file path for the vignette file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The vignette file path</returns>
        public override string GetVignetteFilePath(GameSettings settings) => GetVolumePath(settings.EduVolume) + $"VIGNET.DAT";

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

        /// <summary>
        /// Gets the volume data path
        /// </summary>
        /// <param name="volume">The volume name</param>
        /// <returns>The volume data path</returns>
        public string GetVolumePath(string volume) => GetDataPath() + volume + "/";

        public virtual string GetSamplesArchiveFilePath(string volume) => GetVolumePath(volume) + "SNDSMP.DAT";
        public virtual string GetSpecialArchiveFilePath(string volume) => GetVolumePath(volume) + "SPECIAL.DAT";
        public virtual string GetCommonArchiveFilePath() => GetDataPath() + "COMMON.DAT";

        /// <summary>
        /// Gets the levels for each world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The levels</returns>
        public override GameInfo_Volume[] GetLevels(GameSettings settings) => Directory.GetDirectories(settings.GameDirectory + "/" + GetDataPath(), "???", SearchOption.TopDirectoryOnly).Select(Path.GetFileName).Select(vol => new GameInfo_Volume(vol, WorldHelpers.GetR1Worlds().Select(w => new GameInfo_World((int)w, Directory.EnumerateFiles(settings.GameDirectory + GetVolumePath(vol), $"{GetShortWorldName(w)}??.LEV", SearchOption.TopDirectoryOnly)
            .Select(FileSystem.GetFileNameWithoutExtensions)
            .Select(x => Int32.Parse(x.Substring(3)))
            .ToArray())).ToArray())).ToArray();

        /// <summary>
        /// Gets the archive files which can be extracted
        /// </summary>
        public override ArchiveFile[] GetArchiveFiles(GameSettings settings)
        {
            return new ArchiveFile[]
            {
                new ArchiveFile($"PCMAP/COMMON.DAT"),
                new ArchiveFile($"PCMAP/SNDD8B.DAT"),
                new ArchiveFile($"PCMAP/SNDH8B.DAT"),
            }.Concat(GetLevels(settings).SelectMany(x => new ArchiveFile[]
            {
                new ArchiveFile($"PCMAP/{x.Name}/sndsmp.dat"),
                new ArchiveFile($"PCMAP/{x.Name}/SPECIAL.DAT"),
                new ArchiveFile($"PCMAP/{x.Name}/VIGNET.DAT", ".pcx"),
            })).ToArray();
        }

        /// <summary>
        /// Gets additional sound archives
        /// </summary>
        /// <param name="settings">The game settings</param>
        public override AdditionalSoundArchive[] GetAdditionalSoundArchives(GameSettings settings) => GetLevels(settings).Select(x => new AdditionalSoundArchive($"SMP ({x.Name})", new ArchiveFile(GetSamplesArchiveFilePath(x.Name)))).ToArray();

        public override bool IsDESMultiColored(Context context, int desIndex, GeneralEventInfoData[] generalEvents) => generalEvents.Any(x => x.DesEdu[context.Settings.R1_World] == desIndex && ((R1_EventType)x.Type).IsMultiColored());

        #endregion

        #region Manager Methods

        public override Texture2D LoadBackgroundVignette(Context context, R1_PC_WorldFile world, R1_PC_LevFile level, bool parallax) => parallax ? null : ReadArchiveFile<PCX>(context, world.Plan0NumPcxFiles[level.KitLevelDefines.BG_0])?.ToTexture(true);

        protected override UniTask<IReadOnlyDictionary<string, string[]>> LoadLocalizationAsync(Context context)
        {
            // Create the dictionary
            var localization = new Dictionary<string, string[]>();

            // Read the text data
            var loc = ReadArchiveFile<R1_PC_LocFile>(context, R1_PC_ArchiveFileName.TEXT);

            // Save the localized name
            var locName = loc.LanguageNames[loc.LanguageUtilized];

            if (String.IsNullOrWhiteSpace(locName))
                locName = context.Settings.EduVolume;

            // Add the localization
            localization.Add($"TEXT ({locName})", loc.TextDefine.Select(x => x.Value).ToArray());

            // Read the general data
            var general = ReadArchiveFile<R1_PC_GeneralFile>(context, R1_PC_ArchiveFileName.GENERAL);

            // Add the localization
            localization.Add($"GENERAL ({locName})", general.CreditsStringItems.Select(x => x.String.Value).ToArray());

            // Read the MOT data
            var mot = ReadArchiveFile<R1_PCEdu_MOTFile>(context, R1_PC_ArchiveFileName.MOT);

            // Add the localization
            localization.Add($"MOT ({locName})", mot.TextDefine.Select(x => x.Value).ToArray());

            return UniTask.FromResult<IReadOnlyDictionary<string, string[]>>(localization);
        }

        public override IList<ARGBColor> GetBigRayPalette(Context context) => ReadArchiveFile<PCX>(context, "FND04")?.VGAPalette;

        public override async UniTask LoadArchivesAsync(Context context)
        {
            // Load the vignette archive
            await LoadArchiveAsync(context, GetVignetteFilePath(context.Settings), null);

            // Load the common archive
            await LoadArchiveAsync(context, GetCommonArchiveFilePath(), null);

            // Load the special archive
            await LoadArchiveAsync(context, GetSpecialArchiveFilePath(context.Settings.EduVolume), null);
        }

        #endregion
    }
}