﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinarySerializer;
using BinarySerializer.Image;
using BinarySerializer.Ray1;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Ray1Map.Rayman1
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
        public override GameInfo_Volume[] GetLevels(GameSettings settings) => Directory.GetDirectories(settings.GameDirectory + "/" + GetDataPath(), "???", SearchOption.TopDirectoryOnly).Select(Path.GetFileName).Select(vol => new GameInfo_Volume(vol, WorldHelpers.EnumerateWorlds().Select(w => new GameInfo_World((int)w, Directory.EnumerateFiles(settings.GameDirectory + GetVolumePath(vol), $"{GetShortWorldName(w)}??.LEV", SearchOption.TopDirectoryOnly)
            .Select(FileSystem.GetFileNameWithoutExtensions)
            .Select(x => Int32.Parse(x.Substring(3)))
            .ToArray())).Append(new GameInfo_World(7, new []
        {
            0
        })).ToArray())).ToArray();

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

        #endregion

        #region Manager Methods

        public override string[] GetDESNameTable(Context context) => LevelEditorData.NameTable_EDUDES[context.GetR1Settings().R1_World == World.Menu ? 0 : context.GetR1Settings().World - 1];
        public override string[] GetETANameTable(Context context) => LevelEditorData.NameTable_EDUETA[context.GetR1Settings().R1_World == World.Menu ? 0 : context.GetR1Settings().World - 1];

        public override byte[] GetTypeZDCBytes => PC_ZDCTables.EduPC_Type_ZDC;
        public override byte[] GetZDCTableBytes => PC_ZDCTables.EduPC_ZDCTable;
        public override byte[] GetEventFlagsBytes => PC_ObjTypeFlagTables.EduPC_Flags;

        public override WorldInfo[] GetWorldMapInfos(Context context)
        {
            var wld = LoadArchiveFile<PC_WorldMap>(context, GetSpecialArchiveFilePath(context.GetR1Settings().EduVolume), R1_PC_ArchiveFileName.WLDMAP01);
            return wld.Levels.Take(wld.LevelsCount + 1).Where(x => x.XPosition != 0 && x.YPosition != 0).ToArray();

        }

        public override UniTask<Texture2D> LoadBackgroundVignetteAsync(Context context, PC_WorldFile world, PC_LevFile level, bool parallax) =>
            UniTask.FromResult(parallax ? null : LoadArchiveFile<PCX>(context, GetVignetteFilePath(context.GetR1Settings()), world.Plan0NumPcxFiles[level.LevelDefines.FNDIndex])?.ToTexture(true));

        protected override UniTask<KeyValuePair<string, string[]>[]> LoadLocalizationAsync(Context context)
        {
            // Create the dictionary
            var localization = new KeyValuePair<string, string[]>[3];

            // Read the text data
            var loc = LoadArchiveFile<PC_LocFile>(context, GetSpecialArchiveFilePath(context.GetR1Settings().EduVolume), R1_PC_ArchiveFileName.TEXT);

            // Save the localized name
            var locName = loc.LanguageNames[loc.LanguageUtilized];

            if (String.IsNullOrWhiteSpace(locName))
                locName = context.GetR1Settings().EduVolume;

            // Add the localization
            localization[0] = new KeyValuePair<string, string[]>($"TEXT ({locName})", loc.TextDefine.Select(x => x.Value).ToArray());

            // Read the general data
            var general = LoadArchiveFile<PC_GeneralFile>(context, GetSpecialArchiveFilePath(context.GetR1Settings().EduVolume), R1_PC_ArchiveFileName.GENERAL);

            // Add the localization
            localization[1] = new KeyValuePair<string, string[]>($"GENERAL ({locName})", general.CreditsStringItems.Select(x => x.String.Value).ToArray());

            // Read the MOT data
            var mot = LoadArchiveFile<PC_MOTFile>(context, GetSpecialArchiveFilePath(context.GetR1Settings().EduVolume), R1_PC_ArchiveFileName.MOT);

            // Add the localization
            localization[2] = new KeyValuePair<string, string[]>($"MOT ({locName})", mot.TextDefine.Select(x => x.Value).ToArray());

            return UniTask.FromResult<KeyValuePair<string, string[]>[]>(localization);
        }

        public override IList<BaseColor> GetBigRayPalette(Context context) => LoadArchiveFile<PCX>(context, GetVignetteFilePath(context.GetR1Settings()), "FND04")?.VGAPalette;

        public override async UniTask LoadFilesAsync(Context context)
        {
            // Base
            await base.LoadFilesAsync(context);

            // Common
            await AddFile(context, GetCommonArchiveFilePath(context.GetR1Settings()));

            // Special
            await AddFile(context, GetSpecialArchiveFilePath(context.GetR1Settings().EduVolume));
        }

        public override UniTask<PCX> GetWorldMapVigAsync(Context context)
        {
            var worldVig = LoadArchiveFile<PC_WorldMap>(context, GetSpecialArchiveFilePath(context.GetR1Settings().EduVolume), R1_PC_ArchiveFileName.WLDMAP01).WorldMapVig;

            return UniTask.FromResult(LoadArchiveFile<PCX>(context, GetVignetteFilePath(context.GetR1Settings()), worldVig));
        }

        #endregion
    }
}