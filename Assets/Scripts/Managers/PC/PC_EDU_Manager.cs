using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using R1Engine.Serialize;

namespace R1Engine
{
    /// <summary>
    /// The game manager for Rayman Educational (PC)
    /// </summary>
    public class PC_EDU_Manager : PC_Manager
    {
        #region Values and paths

        /// <summary>
        /// Gets the file path for the specified level
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The level file path</returns>
        public override string GetLevelFilePath(GameSettings settings) => GetVolumePath(settings.EduVolume) + $"{GetShortWorldName(settings.World)}{settings.Level:00}.lev";

        /// <summary>
        /// Gets the file path for the specified world file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The world file path</returns>
        public override string GetWorldFilePath(GameSettings settings) => GetVolumePath(settings.EduVolume) + $"RAY{((int)settings.World + 1):00}.WLD";

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
        public override KeyValuePair<World, int[]>[] GetLevels(GameSettings settings) => EnumHelpers.GetValues<World>().Select(w => new KeyValuePair<World, int[]>(w, Directory.EnumerateFiles(settings.GameDirectory + GetVolumePath(settings.EduVolume), $"{GetShortWorldName(w)}??.LEV", SearchOption.TopDirectoryOnly)
            .Select(FileSystem.GetFileNameWithoutExtensions)
            .Select(x => Int32.Parse(x.Substring(3)))
            .ToArray())).ToArray();

        /// <summary>
        /// Gets the available educational volumes
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The available educational volumes</returns>
        public override string[] GetEduVolumes(GameSettings settings) => Directory.GetDirectories(settings.GameDirectory + "/" + GetDataPath(), "???", SearchOption.TopDirectoryOnly).Select(Path.GetFileName).ToArray();

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
            }.Concat(GetEduVolumes(settings).SelectMany(x => new ArchiveFile[]
            {
                new ArchiveFile($"PCMAP/{x}/sndsmp.dat"),
                new ArchiveFile($"PCMAP/{x}/SPECIAL.DAT"),
                new ArchiveFile($"PCMAP/{x}/VIGNET.DAT", ".pcx"),
            })).ToArray();
        }

        /// <summary>
        /// Gets additional sound archives
        /// </summary>
        /// <param name="settings">The game settings</param>
        public override AdditionalSoundArchive[] GetAdditionalSoundArchives(GameSettings settings) => GetEduVolumes(settings).Select(x => new AdditionalSoundArchive($"SMP ({x})", new ArchiveFile(GetSamplesArchiveFilePath(x)))).ToArray();

        #endregion

        #region Manager Methods

        protected override void LoadLocalization(Context context, Common_Lev level)
        {
            // Create the dictionary
            level.Localization = new Dictionary<string, string[]>();

            // Enumerate each language
            foreach (var vol in GetEduVolumes(context.Settings))
            {
                // Get the file path
                var specialFilePath = GetSpecialArchiveFilePath(vol);

                // Add the file to the context
                context.AddFile(new LinearSerializedFile(context)
                {
                    filePath = specialFilePath
                });

                // Read the archive
                var specialData = FileFactory.Read<PC_EncryptedFileArchive>(specialFilePath, context);

                // Save the localized name
                string locName;

                // Create a stream for the text data
                using (var stream = new MemoryStream(specialData.DecodedFiles[specialData.Entries.FindItemIndex(x => x.FileNameString == "TEXT")]))
                {
                    var key = $"TEXT{vol}";

                    context.AddFile(new StreamFile(key, stream, context));

                    var loc = FileFactory.Read<PC_LocFile>(key, context);

                    locName = $"{loc.LanguageNames[loc.LanguageUtilized]} ({vol})";

                    // Add the localization
                    level.Localization.Add($"TEXT ({locName})", loc.TextDefine.Select(x => x.Value).ToArray());
                }

                // Create a stream for the general data
                using (var stream = new MemoryStream(specialData.DecodedFiles[specialData.Entries.FindItemIndex(x => x.FileNameString == "GENERAL")]))
                {
                    var key = $"GENERAL{vol}";

                    context.AddFile(new StreamFile(key, stream, context));

                    var general = FileFactory.Read<PC_GeneralFile>(key, context);

                    // Add the localization
                    level.Localization.Add($"GENERAL ({locName})", general.CreditsStringItems.Select(x => x.String.Value).ToArray());
                }

                // Create a stream for the MOT data
                using (var stream = new MemoryStream(specialData.DecodedFiles[specialData.Entries.FindItemIndex(x => x.FileNameString == "MOT")]))
                {
                    var key = $"MOT{vol}";

                    context.AddFile(new StreamFile(key, stream, context));

                    var loc = FileFactory.Read<PC_EDU_MOTFile>(key, context);

                    // Add the localization
                    level.Localization.Add($"MOT ({locName})", loc.TextDefine.Select(x => x.Value).ToArray());
                }
            }
        }

        /// <summary>
        /// Gets an editor manager from the specified objects
        /// </summary>
        /// <param name="level">The common level</param>
        /// <param name="context">The context</param>
        /// <param name="designs">The common design</param>
        /// <returns>The editor manager</returns>
        public override BaseEditorManager GetEditorManager(Common_Lev level, Context context, Common_Design[] designs) => new PC_EDU_EditorManager(level, context, this, designs);

        #endregion
    }
}