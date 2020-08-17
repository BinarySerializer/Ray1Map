using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace R1Engine
{
    /// <summary>
    /// The game manager for Rayman Designer (PC)
    /// </summary>
    public class R1_Kit_Manager : R1_PCBaseManager
    {
        #region Values and paths

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
        public virtual string GetEventLocFilePath(string volume, int index) => GetVolumeDirectory(volume) + $"EVNAME{index:00}.WLD";

        // These are actually treated like the volumes in the EDU games, but since the levels are global we don't treat them as that
        public string[] GetLanguages(GameSettings settings) => Directory.GetDirectories(settings.GameDirectory + "PCMAP").Select(Path.GetFileName).ToArray();

        /// <summary>
        /// Gets the levels for each world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The levels</returns>
        public override KeyValuePair<int, int[]>[] GetLevels(GameSettings settings) => WorldHelpers.GetR1Worlds().Select(w => new KeyValuePair<int, int[]>((int)w, Directory.EnumerateFiles(settings.GameDirectory + GetDataPath(), $"{GetShortWorldName(w)}??.LEV", SearchOption.TopDirectoryOnly)
            .Select(FileSystem.GetFileNameWithoutExtensions)
            .Select(x => Int32.Parse(x.Substring(3)))
            .ToArray())).ToArray();

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
                new ArchiveFile($"PCMAP/COMMON.DAT"),
                new ArchiveFile($"PCMAP/SNDD8B.DAT"),
                new ArchiveFile($"PCMAP/SNDH8B.DAT"),
                new ArchiveFile($"PCMAP/VIGNET.DAT", ".pcx"),
            }.Concat(GetLanguages(settings).SelectMany(x => new ArchiveFile[]
            {
                new ArchiveFile(GetSamplesArchiveFilePath(x)),
                new ArchiveFile(GetSpecialArchiveFilePath(x)),
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

        #endregion

        #region Manager Methods

        protected override void LoadLocalization(Context context, Unity_Level level)
        {
            // Create the dictionary
            level.Localization = new Dictionary<string, string[]>();

            // Enumerate each language
            foreach (var lang in GetLanguages(context.Settings))
            {
                // Get the file path
                var specialFilePath = GetSpecialArchiveFilePath(lang);

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
                    var key = $"TEXT{lang}";

                    context.AddFile(new StreamFile(key, stream, context));

                    var loc = FileFactory.Read<R1_PC_LocFile>(key, context);

                    locName = loc.LanguageNames[loc.LanguageUtilized];

                    if (String.IsNullOrWhiteSpace(locName))
                        locName = lang;

                    // Add the localization
                    level.Localization.Add($"TEXT ({locName})", loc.TextDefine.Select(x => x.Value).ToArray());
                }

                // Create a stream for the general data
                using (var stream = new MemoryStream(specialData.DecodedFiles[specialData.Entries.FindItemIndex(x => x.FileName == "GENERAL")]))
                {
                    var key = $"GENERAL{lang}";

                    context.AddFile(new StreamFile(key, stream, context));

                    var general = FileFactory.Read<R1_PC_GeneralFile>(key, context);

                    // Add the localization
                    level.Localization.Add($"GENERAL ({locName})", general.CreditsStringItems.Select(x => x.String.Value).ToArray());
                }

                // Add the event localizations (allfix + 6 worlds)
                for (int i = 0; i < 7; i++)
                {
                    // Get the file path
                    var evLocPath = GetEventLocFilePath(lang, i);

                    if (!FileSystem.FileExists(context.BasePath + evLocPath))
                        continue;

                    // Add the file to the context
                    context.AddFile(new LinearSerializedFile(context)
                    {
                        filePath = evLocPath
                    });

                    // Read the file
                    var evLoc = FileFactory.Read<R1_Mapper_EventLocFile>(evLocPath, context);

                    // Add the localization
                    level.Localization.Add($"EVNAME{i:00} ({locName})", evLoc.LocItems.Select(x => $"{x.LocKey}: {x.Name} - {x.Description}").ToArray());
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
        public override BaseEditorManager GetEditorManager(Unity_Level level, Context context, Unity_ObjGraphics[] designs) => new R1_Kit_EditorManager(level, context, this, designs);

        #endregion
    }
}