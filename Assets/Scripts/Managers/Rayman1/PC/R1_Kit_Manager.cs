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

        /// <summary>
        /// Gets the available game actions
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The game actions</returns>
        public override GameAction[] GetGameActions(GameSettings settings)
        {
            return new GameAction[]
            {
                new GameAction("Export Sprites", false, true, (input, output) => ExportSpriteTexturesAsync(settings, output, false)),
                new GameAction("Export Animation Frames", false, true, (input, output) => ExportSpriteTexturesAsync(settings, output, true)),
                new GameAction("Export Vignette", false, true, (input, output) => ExtractVignette(settings, GetVignetteFilePath(settings), output)),
                new GameAction("Export Archives", false, true, (input, output) => ExtractArchives(output)),
                new GameAction("Export Sound", false, true, (input, output) => ExtractSoundAsync(settings, output)),
                new GameAction("Export Palettes", false, true, (input, output) => ExportPaletteImage(settings, output)),
                new GameAction("Log Archive Files", false, false, (input, output) => LogArchives(settings)),
                new GameAction("Export ETA Info", false, true, (input, output) => ExportETAInfo(settings, output, false)),
                new GameAction("Export ETA Info (extended)", false, true, (input, output) => ExportETAInfo(settings, output, true)),
		new GameAction("Import ETA/DES from Rayman 1", false, false, (input, output) => ImportDESETA(settings, false)),
		new GameAction("Import ETA/DES from EDU", false, false, (input, output) => ImportDESETA(settings, true)),
            };
        }

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
                    if (FileSystem.mode == FileSystem.Mode.Web) {
                        localization.Add($"EVNAME{i:00} ({locName})", evLoc.LocItems.Select(x => $"<key>{x.LocKey}</key><name>{x.Name}</name><description>{x.Description}</description>").ToArray());
                    } else {
                        localization.Add($"EVNAME{i:00} ({locName})", evLoc.LocItems.Select(x => $"{x.LocKey}: {x.Name} - {x.Description}").ToArray());
                    }
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

        public async UniTask ImportDESETA(GameSettings settings, bool edu)
        {
            // TODO: Not hard-code these?
            const int desAllfixCount = 9;
            const int etaAllfixCount = 5;
            int otherDesAllfixCount = edu ? 8 : 7;
            int otherEtaAllfixCount = edu ? 4 : 5;

            // Load in Events.csv to get our mappings
            const string file = "Events.csv";
            GeneralEventInfoData[] eventInfoData;
            await FileSystem.PrepareFile(file);
            if (FileSystem.FileExists(file))
            {
                // Load the event info data
                using (var csvFile = FileSystem.GetFileReadStream(file))
                    eventInfoData = GeneralEventInfoData.ReadCSV(csvFile);
                Debug.Log($"{file} has been loaded with {eventInfoData.Length} events");
            }
            else
            {
                eventInfoData = new GeneralEventInfoData[0];
                Debug.Log($"{file} has not been loaded");
            }

            using (var context = new Context(settings))
            {
                GameSettings otherSettings = new GameSettings(GameModeSelection.RaymanEducationalPC, Settings.GameDirectories[GameModeSelection.RaymanEducationalPC], settings.World, settings.Level);
                if (!edu) {
                    // Find the first Rayman 1 version that the user has actually set up.
                    foreach (var mode in EnumHelpers.GetValues<GameModeSelection>().Where(x => x.GetAttribute<GameModeAttribute>().EngineVersion == EngineVersion.R1_PC)) {
                        if (Settings.GameDirectories.ContainsKey(mode) && Directory.Exists(Settings.GameDirectories[mode])) {
                            otherSettings = new GameSettings(mode, Settings.GameDirectories[mode], settings.World, settings.Level);
                            break;
                        }
                    }
                }

                using (var otherContext = new Context(otherSettings)) {
                    // Create manager for the other game, and load its files.
                    R1_PCBaseManager otherGame;
                    if (edu) {
                        otherGame = new R1_PCEdu_Manager();
                        otherContext.Settings.EduVolume = otherGame.GetLevels(otherContext.Settings).First().Name;
                    } else
                        otherGame = new R1_PC_Manager();

                    // Loop through the worlds.
                    for (int w = 1; w < 7; w++)
                    {
                        context.Settings.World = otherContext.Settings.World = w;
                        var wldPath = GetWorldFilePath(context.Settings);

                        await LoadFilesAsync(context);
                        await otherGame.LoadFilesAsync(otherContext);

                        // Load our WLD file and the other game's.
                        var wld = FileFactory.Read<R1_PC_WorldFile>(wldPath, context);
                        var otherWld = FileFactory.Read<R1_PC_WorldFile>(otherGame.GetWorldFilePath(otherContext.Settings), otherContext);

                        // Get the list of existing ETA and DES files so we know what's missing.
                        var desNames = wld.DESFileNames.ToArray();
                        var etaNames = wld.ETAFileNames.ToArray();

                        // Use Events.csv to get mappings from the other game onto this one.
                        var desMappings = new Dictionary<int, string>() {};
                        var etaMappings = new Dictionary<int, string>() {};
                        var r1wld = otherContext.Settings.R1_World;
                        foreach (var eve in eventInfoData)
                        {
                            // First see if there's a DES specified for our source game.
                            var otherDes = edu ? eve.DesEdu.TryGetItem(r1wld) : eve.DesR1.TryGetItem(r1wld);
                            if (otherDes is int iOtherDes) {
                                // See if there's a DES also specified for KIT.
                                var desMapping = eve.DesKit.TryGetItem(r1wld);
                                if (desMapping != null && desMapping != "" && !desNames.Contains($"{desMapping}.DES")) {
                                    // The DES is specified in Events.csv, but doesn't exist in the WLD file.
                                    // Add it to the copy list!
                                    desMappings[iOtherDes] = $"{desMapping}.DES";

                                    Debug.Log($"Mapping DES {iOtherDes} to {desMapping} based on {eve.Name}");
                                }
                            }

                            // Do the same thing for the ETA.
                            var otherEta = edu ? eve.EtaEdu.TryGetItem(r1wld) : eve.EtaR1.TryGetItem(r1wld);
                            if (otherEta is int iOtherEta) {
                                // See if there's an ETA also specified for KIT.
                                var etaMapping = eve.EtaKit.TryGetItem(r1wld);
                                if (etaMapping != null && etaMapping != "" && !etaNames.Contains($"{etaMapping}.ETA")) {
                                    // The ETA is specified in Events.csv, but doesn't exist in the WLD file.
                                    // Add it to the copy list!
                                    etaMappings[iOtherEta] = $"{etaMapping}.ETA";
                                    Debug.Log($"Mapping ETA {iOtherEta} to {etaMapping} based on {eve.Name}");
                                }
                            }
                        }

                        // Now that we've set up the mappings, carry out the copies!
                        foreach (var mapping in desMappings) {
                            Debug.Log($"Attempting to port DES {mapping}");
                            wld.DesItems = wld.DesItems.Append(otherWld.DesItems[mapping.Key - otherDesAllfixCount - 1]).ToArray();
                            wld.DesItemCount = (ushort)wld.DesItems.Length;
                            wld.DESFileNames[wld.DesItemCount + desAllfixCount - 1] = mapping.Value;
                        }
                        foreach (var mapping in etaMappings) {
                            Debug.Log($"Attempting to port ETA {mapping}");
                            wld.Eta = wld.Eta.Append(otherWld.Eta[mapping.Key - otherEtaAllfixCount - 1]).ToArray();
                            wld.ETAFileNames[wld.Eta.Length + etaAllfixCount - 1] = mapping.Value;
                        }

                        // Save the WLD
                        FileFactory.Write<R1_PC_WorldFile>(wldPath, context);
                    }
                }

                /*// Beef up the memory allocation if necessary.
                const int newMemAlloc = 2048; // 2 MiB should be enough!
                var commonDat = FileFactory.Read<R1_PC_EncryptedFileArchive>(GetCommonArchiveFilePath(), context);
                var datInitialOffset = commonDat.Offset;
                var versionFileName = R1_PC_ArchiveFileName.VERSION.ToString();
                var versionFileIdx = commonDat.Entries.FindItemIndex(x => x.FileName == versionFileName);
                var versionFile = commonDat.ReadFile<R1_PC_VersionFile>(context, versionFileIdx);

                // Increase the memory allocated for each version.
                foreach (var verMemInfo in versionFile.VersionMemoryInfos) {
                    if (verMemInfo.TailleMainMemWorld < newMemAlloc)
                        verMemInfo.TailleMainMemWorld = newMemAlloc;
                    if (verMemInfo.TailleMainMemSprite < newMemAlloc)
                        verMemInfo.TailleMainMemSprite = newMemAlloc;
                }

                // Reserialize the object
                var s = context.Serializer;
                var versionEntry = commonDat.Entries[versionFileIdx];
                s.DoAt(datInitialOffset + versionEntry.FileOffset, () =>
                        {
                        s.DoXOR(versionEntry.XORKey, () => s.SerializeObject<R1_PC_VersionFile>(versionFile, null, name: versionFileName));
                        });

                // Save out the updated archive.
                FileFactory.Write<R1_PC_EncryptedFileArchive>(GetCommonArchiveFilePath(), context);*/
            }
        }
    }
}
