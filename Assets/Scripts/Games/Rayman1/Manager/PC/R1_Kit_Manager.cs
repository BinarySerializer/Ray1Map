﻿
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinarySerializer;
using BinarySerializer.Image;
using BinarySerializer.Ray1;
using BinarySerializer.Ray1.PC;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Ray1Map.Rayman1
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
        public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(WorldHelpers.EnumerateWorlds().Select(w => new GameInfo_World((int)w, Directory.EnumerateFiles(settings.GameDirectory + GetDataPath(), $"{GetShortWorldName(w)}??.LEV", SearchOption.TopDirectoryOnly)
            .Select(FileSystem.GetFileNameWithoutExtensions)
            .Select(x => Int32.Parse(x.Substring(3)))
            .ToArray())).Append(new GameInfo_World(7, new []
        {
            0
        })).ToArray());

        /// <summary>
        /// Gets the DES file name for the current index in the current context
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="desIndex">The DES index</param>
        /// <returns>The file name</returns>
        public string GetDESFileName(Context context, int desIndex)
        {
            // Read the world data
            var worldData = FileFactory.Read<WorldFile>(context, GetWorldFilePath(context.GetR1Settings()));

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
            var worldData = FileFactory.Read<WorldFile>(context, GetWorldFilePath(context.GetR1Settings()));

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

        public override string[] GetDESNameTable(Context context) => context.GetR1Settings().R1_World == World.Menu ? new string[0] : FileFactory.Read<WorldFile>(context, GetWorldFilePath(context.GetR1Settings())).DESFileNames.Select(x => x.Length > 4 ? x.Substring(0, x.Length - 4) : x).ToArray();

        public override string[] GetETANameTable(Context context) => context.GetR1Settings().R1_World == World.Menu ? new string[0] : FileFactory.Read<WorldFile>(context, GetWorldFilePath(context.GetR1Settings())).ETAFileNames.Select(x => x.Length > 4 ? x.Substring(0, x.Length - 4) : x).ToArray();

        public override byte[] GetTypeZDCBytes => ZDCTables.KitPC_Type_ZDC;
        public override byte[] GetZDCTableBytes => ZDCTables.KitPC_ZDCTable;
        public override byte[] GetEventFlagsBytes => ObjTypeFlagTables.KitPC_Flags;

        public override WorldInfo[] GetWorldMapInfos(Context context)
        {
            var wld = LoadArchiveFile<WorldMapScript>(context, GetCommonArchiveFilePath(), R1_PC_ArchiveFileName.WLDMAP01);
            return wld.WorldMap.MapDefine.Take(wld.WorldMap.PelletsCount).ToArray();
        }

        public override UniTask<Texture2D> LoadBackgroundVignetteAsync(Context context, WorldFile world, LevelFile level, bool parallax) => 
            UniTask.FromResult(parallax ? null : LoadArchiveFile<PCX>(context, GetVignetteFilePath(context.GetR1Settings()), world.PcxFileNames[level.LevelDefine.Fnd])?.ToTexture(true));

        /// <summary>
        /// Gets the available game actions
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The game actions</returns>
        public override GameAction[] GetGameActions(GameSettings settings)
        {
            return base.GetGameActions(settings).Concat(new GameAction[]
            {
		        new GameAction("Import ETA/DES from Rayman 1", false, false, (input, output) => ImportDESETA(settings, false)),
		        new GameAction("Import ETA/DES from EDU", false, false, (input, output) => ImportDESETA(settings, true)),
		        new GameAction("Increase DES/WLD memory allocation", false, false, (input, output) => IncreaseMemAlloc(settings)),
            }).ToArray();
        }

        protected override async UniTask<KeyValuePair<string, string[]>[]> LoadLocalizationAsync(Context context)
        {
            // Create the dictionary
            var localization = new List<KeyValuePair<string, string[]>>();

            // Enumerate each language
            foreach (var lang in LoadArchiveFile<VersionScript>(context, GetCommonArchiveFilePath(), R1_PC_ArchiveFileName.VERSION).VersionCodes)
            {
                // Read the text data
                var loc = LoadArchiveFile<TextScript>(context, GetSpecialArchiveFilePath(lang), R1_PC_ArchiveFileName.TEXT);

                // Save the localized name
                var locName = loc?.LanguageNames[loc.LanguageUsed];

                if (String.IsNullOrWhiteSpace(locName))
                    locName = lang;

                // Add the localization
                if (loc != null)
                    localization.Add(new KeyValuePair<string, string[]>($"TEXT ({locName})", loc.PCPacked_TextDefine));

                // Read the general data
                var general = LoadArchiveFile<GeneralScript>(context, GetSpecialArchiveFilePath(lang), R1_PC_ArchiveFileName.GENERAL);

                // Add the localization
                if (general != null)
                    localization.Add(new KeyValuePair<string, string[]>($"GENERAL ({locName})", general.Credits.Select(x => x.Text).ToArray()));

                // Add the event localizations (allfix + 6 worlds)
                for (int i = 0; i < 7; i++)
                {
                    // Get the file path
                    var evLocPath = GetEventLocFilePath(lang, i);

                    // Add the file to the context
                    await AddFile(context, evLocPath);

                    if (!FileSystem.FileExists(context.GetAbsoluteFilePath(evLocPath)))
                        continue;

                    // Read the file
                    var evLoc = FileFactory.Read<EventNames>(context, evLocPath);

                    // Add the localization
                    if (FileSystem.mode == FileSystem.Mode.Web) {
                        localization.Add(new KeyValuePair<string, string[]>($"EVNAME{i:00} ({locName})", evLoc.LocItems.Select(x => $"<key>{x.Name}</key><name>{x.DisplayName}</name><description>{x.DisplayDescription}</description>").ToArray()));
                    } else {
                        localization.Add(new KeyValuePair<string, string[]>($"EVNAME{i:00} ({locName})", evLoc.LocItems.Select(x => $"{x.Name}: {x.DisplayName} - {x.DisplayDescription}").ToArray()));
                    }
                }
            }

            return localization.ToArray();
        }

        public override IList<BaseColor> GetBigRayPalette(Context context) => LoadArchiveFile<PCX>(context, GetVignetteFilePath(context.GetR1Settings()), "FND00")?.VGAPalette;

        public override async UniTask LoadFilesAsync(Context context)
        {
            // Base
            await base.LoadFilesAsync(context);

            // Common
            await AddFile(context, GetCommonArchiveFilePath());
            
            // Special
            foreach (var version in LoadArchiveFile<VersionScript>(context, GetCommonArchiveFilePath(), R1_PC_ArchiveFileName.VERSION).VersionCodes)
                await AddFile(context, GetSpecialArchiveFilePath(version));
        }

        public override UniTask<PCX> GetWorldMapVigAsync(Context context)
        {
            var worldVig = LoadArchiveFile<WorldMapScript>(context, GetCommonArchiveFilePath(), R1_PC_ArchiveFileName.WLDMAP01).WorldMap.Background;

            return UniTask.FromResult(LoadArchiveFile<PCX>(context, GetVignetteFilePath(context.GetR1Settings()), worldVig));
        }

        public async UniTask ImportDESETA(GameSettings settings, bool edu)
        {
            // TODO: Not hard-code these?
            const int desAllfixCount = 9;
            const int etaAllfixCount = 5;
            int otherDesAllfixCount = edu ? 8 : 7;
            int otherEtaAllfixCount = edu ? 5 : 4;

            // Load in the JSON files to get our mappings
            await LevelEditorData.InitAsync(settings, true);

            using (var context = new Ray1MapContext(settings))
            {
                GameSettings otherSettings;

                if (!edu) 
                {
                    // Find the first Rayman 1 version that the user has actually set up.
                    var mode = EnumHelpers.GetValues<GameModeSelection>().Where(x => x.GetAttribute<GameModeAttribute>().EngineVersion == EngineVersion.R1_PC).First(x => Settings.GameDirectories.ContainsKey(x) && Directory.Exists(Settings.GameDirectories[x]));

                    otherSettings = new GameSettings(mode, Settings.GameDirectories[mode], settings.World, settings.Level);
                }
                else
                {
                    otherSettings = new GameSettings(GameModeSelection.RaymanEducationalPC, Settings.GameDirectories[GameModeSelection.RaymanEducationalPC], settings.World, settings.Level);
                }

                using (var otherContext = new Ray1MapContext(otherSettings)) {
                    // Create manager for the other game, and load its files.
                    R1_PCBaseManager otherGame = (R1_PCBaseManager)otherSettings.GetGameManager;

                    if (edu)
                    {
                        otherContext.GetR1Settings().EduVolume = otherGame.GetLevels(otherContext.GetR1Settings()).First().Name;
                        otherContext.GetRequiredSettings<Ray1Settings>().Volume = otherContext.GetR1Settings().EduVolume;
                    }

                    // Loop through the worlds.
                    for (int w = 1; w < 7; w++)
                    {
                        context.GetR1Settings().World = otherContext.GetR1Settings().World = w;
                        context.GetRequiredSettings<Ray1Settings>().World = otherContext.GetRequiredSettings<Ray1Settings>().World = (World)w;
                        var wldPath = GetWorldFilePath(context.GetR1Settings());

                        await LoadFilesAsync(context);
                        await otherGame.LoadFilesAsync(otherContext);

                        // Load our WLD file and the other game's.
                        var wld = FileFactory.Read<WorldFile>(context, wldPath);
                        var otherWld = FileFactory.Read<WorldFile>(otherContext, otherGame.GetWorldFilePath(otherContext.GetR1Settings()));

                        // Get the list of existing ETA and DES files so we know what's missing.
                        var desNames = wld.DESFileNames.ToArray();
                        var etaNames = wld.ETAFileNames.ToArray();

                        // Use the tables to get mappings from the other game onto this one.
                        var desMappings = new Dictionary<int, string>();
                        var etaMappings = new Dictionary<int, string>();
                        var r1wld = otherContext.GetR1Settings().R1_World;

                        var desNameTable = otherGame.GetDESNameTable(otherContext);
                        var etaNameTable = otherGame.GetETANameTable(otherContext);

                        // Go through the other game's DES and ETA name tables, and see if any one
                        // is missing from Designer.
                        for (int iDes = 0; iDes < desNameTable.Length; iDes++) {
                            var desName = desNameTable[iDes];
                            if ((desName == null) || (desName == "N/A"))
                                continue;
                            if (!desNames.Contains($"{desName}.DES")) {
                                // The DES is specified in the JSON file, but doesn't exist in the WLD file.
                                // Add it to the copy list.
                                desMappings[iDes] = $"{desName}.DES";

                                Debug.Log($"Mapping DES {iDes} to {desName}");
                            }
                        }
                        for (int iEta = 0; iEta < etaNameTable.Length; iEta++) {
                            var etaName = etaNameTable[iEta];
                            if ((etaName == null) || (etaName == "N/A"))
                                continue;
                            if (!etaNames.Contains($"{etaName}.ETA")) {
                                // The ETA is specified in the JSON file, but doesn't exist in the WLD file.
                                // Add it to the copy list.
                                etaMappings[iEta] = $"{etaName}.ETA";

                                Debug.Log($"Mapping ETA {iEta} to {etaName}");
                            }
                        }

                        // Now that we've set up the mappings, carry out the copies!
                        var newDesItems = wld.DesItems.ToList();
                        foreach (var mapping in desMappings) {
                            Debug.Log($"Attempting to port DES {mapping.Key} -> {mapping.Value}");
                            newDesItems.Add(otherWld.DesItems[mapping.Key - otherDesAllfixCount - 1]);
                            wld.DesItemCount = (ushort)newDesItems.Count;
                            wld.DESFileNames[wld.DesItemCount + desAllfixCount - 1] = mapping.Value;
                        }
                        wld.DesItems = newDesItems.ToArray();

                        var newEtaItems = wld.Eta.ToList();
                        foreach (var mapping in etaMappings) {
                            Debug.Log($"Attempting to port ETA {mapping.Key} -> {mapping.Value}");
                            newEtaItems.Add(otherWld.Eta[mapping.Key - otherEtaAllfixCount]);
                            wld.ETAFileNames[newEtaItems.Count + etaAllfixCount - 1] = mapping.Value;
                        }
                        wld.Eta = newEtaItems.ToArray();

                        // Save the WLD
                        FileFactory.Write<WorldFile>(context, wldPath);
                    }
                }
            }

            // Beef up the memory allocation if necessary.
            await IncreaseMemAlloc(settings);
        }
	
        public async UniTask IncreaseMemAlloc(GameSettings settings)
        {
            const int newMemAlloc = 2048; // 2 MiB should be enough!
            using (var context = new Ray1MapContext(settings))
            {
                await LoadFilesAsync(context);

                Debug.Log("Opening version file...");
                var commonDat = FileFactory.Read<FileArchive>(context, GetCommonArchiveFilePath());
                var versionFileName = R1_PC_ArchiveFileName.VERSION.ToString();
                var versionFile = commonDat.ReadFile<VersionScript>(context, versionFileName);

                Debug.Log("Increasing memory allocation...");
                // Increase the memory allocated for each version.
                foreach (var verMemInfo in versionFile.MemorySizes) {
                    if (verMemInfo.World < newMemAlloc)
                        verMemInfo.World = newMemAlloc;
                    if (verMemInfo.Sprite < newMemAlloc)
                        verMemInfo.Sprite = newMemAlloc;
                }

                Debug.Log("Saving version file...");
                // Reserialize and save out the updated archive.
                commonDat.RepackArchive(context, new Dictionary<string, Action<SerializerObject>> {
                                {versionFileName, x => x.SerializeObject<VersionScript>(versionFile, name: versionFileName)}
                                });
                Debug.Log("Version file saved.");
            }
        }
    }
}
