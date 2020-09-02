using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
        public override string GetLevelFilePath(GameSettings settings) => GetVolumePath(settings.EduVolume) + $"{GetShortWorldName(settings.R1_World)}{settings.Level:00}.lev";

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

        protected override IReadOnlyDictionary<string, string[]> LoadLocalization(Context context)
        {
            // Create the dictionary
            var localization = new Dictionary<string, string[]>();

            // Enumerate each language
            foreach (var vol in GetLevels(context.Settings).Select(x => x.Name))
            {
                // Get the file path
                var specialFilePath = GetSpecialArchiveFilePath(vol);

                // Add the file to the context
                context.AddFile(new LinearSerializedFile(context)
                {
                    filePath = specialFilePath
                });

                // Read the archive
                var specialData = FileFactory.Read<R1_PC_EncryptedFileArchive>(specialFilePath, context);

                // Save the localized name
                string locName;

                // Create a stream for the text data
                using (var stream = new MemoryStream(specialData.DecodedFiles[specialData.Entries.FindItemIndex(x => x.FileName == "TEXT")]))
                {
                    var key = $"TEXT{vol}";

                    context.AddFile(new StreamFile(key, stream, context));

                    var loc = FileFactory.Read<R1_PC_LocFile>(key, context);

                    locName = $"{loc.LanguageNames[loc.LanguageUtilized]} ({vol})";

                    // Add the localization
                    localization.Add($"TEXT ({locName})", loc.TextDefine.Select(x => x.Value).ToArray());
                }

                // Create a stream for the general data
                using (var stream = new MemoryStream(specialData.DecodedFiles[specialData.Entries.FindItemIndex(x => x.FileName == "GENERAL")]))
                {
                    var key = $"GENERAL{vol}";

                    context.AddFile(new StreamFile(key, stream, context));

                    var general = FileFactory.Read<R1_PC_GeneralFile>(key, context);

                    // Add the localization
                    localization.Add($"GENERAL ({locName})", general.CreditsStringItems.Select(x => x.String.Value).ToArray());
                }

                // Create a stream for the MOT data
                using (var stream = new MemoryStream(specialData.DecodedFiles[specialData.Entries.FindItemIndex(x => x.FileName == "MOT")]))
                {
                    var key = $"MOT{vol}";

                    context.AddFile(new StreamFile(key, stream, context));

                    var loc = FileFactory.Read<R1_PCEdu_MOTFile>(key, context);

                    // Add the localization
                    localization.Add($"MOT ({locName})", loc.TextDefine.Select(x => x.Value).ToArray());
                }
            }

            return localization;
        }

        #endregion
    }
}