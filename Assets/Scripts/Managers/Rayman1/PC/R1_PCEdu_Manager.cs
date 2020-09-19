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

        public override string GetVignetteFilePath(GameSettings settings) => GetVignetteFilePath(settings.EduVolume);
        public string GetVignetteFilePath(string volume) => GetVolumePath(volume) + $"VIGNET.DAT";

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
        public virtual string GetCommonArchiveFilePath(GameSettings settings) => GetDataPath() + "COMMON.DAT";

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
        public override Archive[] GetArchiveFiles(GameSettings settings)
        {
            return new Archive[]
            {
                new Archive(GetCommonArchiveFilePath(settings)),
                new Archive(GetSoundFilePath()),
                new Archive(GetSoundManifestFilePath()),
            }.Concat(GetLevels(settings).SelectMany(x => new Archive[]
            {
                new Archive(GetSamplesArchiveFilePath(x.Name), x.Name),
                new Archive(GetSpecialArchiveFilePath(x.Name), x.Name),
                new Archive(GetVignetteFilePath(x.Name), x.Name),
            })).ToArray();
        }

        /// <summary>
        /// Gets additional sound archives
        /// </summary>
        /// <param name="settings">The game settings</param>
        public override AdditionalSoundArchive[] GetAdditionalSoundArchives(GameSettings settings) => GetLevels(settings).Select(x => new AdditionalSoundArchive($"SMP ({x.Name})", GetSamplesArchiveFilePath(x.Name))).ToArray();

        public override bool IsDESMultiColored(Context context, int desIndex, GeneralEventInfoData[] generalEvents) => generalEvents.Any(x => x.DesEdu[context.Settings.R1_World] == desIndex && ((R1_EventType)x.Type).IsMultiColored());

        #endregion

        #region Manager Methods

        public override byte[] GetTypeZDCBytes => R1_PC_ZDCTables.EduPC_Type_ZDC;
        public override byte[] GetZDCTableBytes => R1_PC_ZDCTables.EduPC_ZDCTable;
        public override byte[] GetEventFlagsBytes => R1_PC_EventFlagTables.EduPC_Flags;

        public override UniTask<Texture2D> LoadBackgroundVignetteAsync(Context context, R1_PC_WorldFile world, R1_PC_LevFile level, bool parallax) =>
            UniTask.FromResult(parallax ? null : LoadArchiveFile<PCX>(context, GetVignetteFilePath(context.Settings), world.Plan0NumPcxFiles[level.KitLevelDefines.BG_0])?.ToTexture(true));

        protected override UniTask<IReadOnlyDictionary<string, string[]>> LoadLocalizationAsync(Context context)
        {
            // Create the dictionary
            var localization = new Dictionary<string, string[]>();

            // Read the text data
            var loc = LoadArchiveFile<R1_PC_LocFile>(context, GetSpecialArchiveFilePath(context.Settings.EduVolume), R1_PC_ArchiveFileName.TEXT);

            // Save the localized name
            var locName = loc.LanguageNames[loc.LanguageUtilized];

            if (String.IsNullOrWhiteSpace(locName))
                locName = context.Settings.EduVolume;

            // Add the localization
            localization.Add($"TEXT ({locName})", loc.TextDefine.Select(x => x.Value).ToArray());

            // Read the general data
            var general = LoadArchiveFile<R1_PC_GeneralFile>(context, GetSpecialArchiveFilePath(context.Settings.EduVolume), R1_PC_ArchiveFileName.GENERAL);

            // Add the localization
            localization.Add($"GENERAL ({locName})", general.CreditsStringItems.Select(x => x.String.Value).ToArray());

            // Read the MOT data
            var mot = LoadArchiveFile<R1_PCEdu_MOTFile>(context, GetSpecialArchiveFilePath(context.Settings.EduVolume), R1_PC_ArchiveFileName.MOT); 

            // Add the localization
            localization.Add($"MOT ({locName})", mot.TextDefine.Select(x => x.Value).ToArray());

            return UniTask.FromResult<IReadOnlyDictionary<string, string[]>>(localization);
        }

        public override IList<ARGBColor> GetBigRayPalette(Context context) => LoadArchiveFile<PCX>(context, GetVignetteFilePath(context.Settings), "FND04")?.VGAPalette;

        public override async UniTask LoadFilesAsync(Context context)
        {
            // Base
            await base.LoadFilesAsync(context);

            // Common
            await AddFile(context, GetCommonArchiveFilePath(context.Settings));

            // Special
            await AddFile(context, GetSpecialArchiveFilePath(context.Settings.EduVolume));
        }

        #endregion
    }
}