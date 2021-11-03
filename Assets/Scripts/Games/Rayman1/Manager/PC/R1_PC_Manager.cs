using Cysharp.Threading.Tasks;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinarySerializer;
using BinarySerializer.Image;
using BinarySerializer.Ray1;
using UnityEngine;

namespace Ray1Map.Rayman1
{
    /// <summary>
    /// The game manager for Rayman 1 (PC)
    /// </summary>
    public class R1_PC_Manager : R1_PCBaseManager
    {
        #region Values and paths

        /// <summary>
        /// Gets the levels for each world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The levels</returns>
        public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(WorldHelpers.EnumerateWorlds().Where(x => Directory.Exists(settings.GameDirectory + GetWorldFolderPath(x))).Select(w => new GameInfo_World((int)w, Directory.EnumerateFiles(settings.GameDirectory + GetWorldFolderPath(w), $"RAY??.LEV", SearchOption.TopDirectoryOnly)
            .Select(FileSystem.GetFileNameWithoutExtensions)
            .Select(x => Int32.Parse(x.Substring(3)))
            .ToArray())).Append(new GameInfo_World(7, new []
        {
            0
        })).ToArray());

        /// <summary>
        /// Gets the folder path for the specified world
        /// </summary>
        /// <param name="world">The world</param>
        /// <returns>The world folder path</returns>
        public virtual string GetWorldFolderPath(World world) => GetDataPath() + GetWorldName(world) + "/";

        /// <summary>
        /// Gets the file path for the big ray file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The big ray file path</returns>
        public override string GetBigRayFilePath(GameSettings settings) => GetDataPath() + $"BRAY.DAT";

        /// <summary>
        /// Gets the file path for the vignette file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The vignette file path</returns>
        public override string GetVignetteFilePath(GameSettings settings) => $"VIGNET.DAT";

        /// <summary>
        /// Gets the file path for the specified level
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The level file path</returns>
        public override string GetLevelFilePath(GameSettings settings) => GetWorldFolderPath(settings.R1_World) + $"RAY{settings.Level}.LEV";

        /// <summary>
        /// Gets the file path for the specified world file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The world file path</returns>
        public override string GetWorldFilePath(GameSettings settings) => GetDataPath() + $"RAY{settings.World}.WLD";

        public virtual string GetLanguageFilePath() => "RAY.LNG";
        public virtual string GetSoundVignetteFilePath() => "SNDVIG.DAT";

        /// <summary>
        /// Gets the archive files which can be extracted
        /// </summary>
        public override Archive[] GetArchiveFiles(GameSettings settings) => new Archive[]
        {
            //GetLanguageFilePath(), // TODO: Serialize as an archive
            new Archive(GetVignetteFilePath(settings)),
            new Archive(GetSoundManifestFilePath()),
            new Archive(GetSoundFilePath()),
            new Archive(GetSoundVignetteFilePath())
        };

        /// <summary>
        /// Gets additional sound archives
        /// </summary>
        /// <param name="settings">The game settings</param>
        public override AdditionalSoundArchive[] GetAdditionalSoundArchives(GameSettings settings) => new AdditionalSoundArchive[]
        {
            new AdditionalSoundArchive("VIG", GetSoundVignetteFilePath(), 16),
        };

        /// <summary>
        /// Gets the available game actions
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The game actions</returns>
        public override GameAction[] GetGameActions(GameSettings settings)
        {
            return base.GetGameActions(settings).Concat(new GameAction[]
            {
                new GameAction("Decrypt Save Files", false, false, (input, output) => DecryptSaveFiles(settings)),
                new GameAction("Read Save Files", false, false, (input, output) => ReadSaveFiles(settings)),
                new GameAction("Export Vignette (brute force)", false, true, (input, output) => ExtractEncryptedPCX(settings.GameDirectory + GetVignetteFilePath(settings), output)),
            }).ToArray();
        }

        #endregion

        #region Manager Methods

        public void DecryptSaveFiles(GameSettings settings)
        {
            using (var context = new Ray1MapContext(settings))
            {
                foreach (var save in Directory.GetFiles(settings.GameDirectory, "*.sav", SearchOption.TopDirectoryOnly).Select(Path.GetFileName))
                {
                    LinearFile f = new LinearFile(context, save);
                    context.AddFile(f);
                    SerializerObject s = context.Deserializer;
                    byte[] saveData = null;
                    s.DoAt(f.StartPointer, () => {
                        s.DoEncoded(new PC_SaveEncoder(), () => {
                            saveData = s.SerializeArray<byte>(saveData, s.CurrentLength, name: "SaveData");
                            Util.ByteArrayToFile(context.GetAbsoluteFilePath($"{save}.dec"), saveData);
                        });
                    });
                    /*LinearSerializedFile f2 = new LinearSerializedFile(context) {
                        filePath = save + ".recompressed"
                    };
                    context.AddFile(f2);
                    s = context.Serializer;
                    s.DoAt(f2.StartPointer, () => {
                        s.DoEncoded(new R1PCSaveEncoder(), () => {
                            saveData = s.SerializeArray<byte>(saveData, saveData.Length, name: "SaveData");
                        });
                    });*/
                }
            }
        }

        public void ReadSaveFiles(GameSettings settings)
        {
            using (var context = new Ray1MapContext(settings))
            {
                foreach (var save in Directory.GetFiles(settings.GameDirectory, "*.sav", SearchOption.TopDirectoryOnly).Select(Path.GetFileName))
                {
                    LinearFile f = new LinearFile(context, save);
                    context.AddFile(f);
                    SerializerObject s = context.Deserializer;
                    s.DoAt(f.StartPointer, () => {
                        s.DoEncoded(new PC_SaveEncoder(), () => s.SerializeObject<PC_SaveFile>(default, name: "SaveFile"));
                    });
                }
            }
        }

        public override string[] GetDESNameTable(Context context) => LevelEditorData.NameTable_R1PCDES[context.GetR1Settings().R1_World == World.Menu ? 0 : context.GetR1Settings().World - 1];
        public override string[] GetETANameTable(Context context) => LevelEditorData.NameTable_R1PCETA[context.GetR1Settings().R1_World == World.Menu ? 0 : context.GetR1Settings().World - 1];

        public override byte[] GetTypeZDCBytes => PC_ZDCTables.R1PC_Type_ZDC;
        public override byte[] GetZDCTableBytes => PC_ZDCTables.R1PC_ZDCTable;
        public override byte[] GetEventFlagsBytes => PC_ObjTypeFlagTables.R1PC_Flags;
        public override WorldInfo[] GetWorldMapInfos(Context context) => context.Deserializer.SerializeFromBytes<ObjectArray<WorldInfo>>(PC_WorldInfoTables.R1PC_WorldInfo, "WorldInfo", x => x.Pre_Length = 24, name: "WorldInfos").Value;

        public override UniTask<Texture2D> LoadBackgroundVignetteAsync(Context context, PC_WorldFile world, PC_LevFile level, bool parallax)
        {
            // Return null if the parallax bg is the same as the normal one
            if (parallax && level.ScrollDiffFNDIndex == level.FNDIndex)
                return UniTask.FromResult<Texture2D>(null);

            var tex = LoadArchiveFile<PCX>(context, GetVignetteFilePath(context.GetR1Settings()),
                world.Plan0NumPcx[parallax ? level.ScrollDiffFNDIndex : level.FNDIndex])?.ToTexture(true);

            return UniTask.FromResult(tex);
        }

        protected override async UniTask<KeyValuePair<string, string[]>[]> LoadLocalizationAsync(Context context)
        {
            var lngPath = GetLanguageFilePath();

            await FileSystem.PrepareFile(context.GetAbsoluteFilePath(lngPath));

            // Read the language file
            var lng = Ray1TextFileFactory.ReadText<PC_LNGFile>(lngPath, context);

            var loc = new List<KeyValuePair<string, string[]>>();

            // Set localization if available
            if (lng.Strings.Length > 0)
                loc.Add(new KeyValuePair<string, string[]>("English", lng.Strings[0]));
            if (lng.Strings.Length > 1)
                loc.Add(new KeyValuePair<string, string[]>("French", lng.Strings[1]));
            if (lng.Strings.Length > 2)
                loc.Add(new KeyValuePair<string, string[]>("German", lng.Strings[2]));
            if (lng.Strings.Length > 3)
                loc.Add(new KeyValuePair<string, string[]>("Japanese", lng.Strings[3]));
            if (lng.Strings.Length > 4)
                loc.Add(new KeyValuePair<string, string[]>("Chinese", lng.Strings[4]));

            return loc.ToArray();
        }

        public override UniTask<PCX> GetWorldMapVigAsync(Context context)
        {
            int index;

            switch (context.GetR1Settings().GameModeSelection)
            {
                case GameModeSelection.RaymanPC_1_00:
                    index = 47;
                    break;

                case GameModeSelection.RaymanPC_1_10:
                case GameModeSelection.RaymanPC_1_12:
                case GameModeSelection.RaymanPC_Demo_2:
                case GameModeSelection.RaymanPC_1_20:
                case GameModeSelection.RaymanPC_1_21_JP:
                case GameModeSelection.RaymanClassicMobile:
                    index = 46;
                    break;

                case GameModeSelection.RaymanPC_1_21:
                    index = 48;
                    break;

                case GameModeSelection.RaymanPC_Demo_1:
                default:
                    throw new ArgumentOutOfRangeException(nameof(GameSettings.GameModeSelection), context.GetR1Settings().GameModeSelection, null);
            }

            return UniTask.FromResult(LoadArchiveFile<PCX>(context, GetVignetteFilePath(context.GetR1Settings()), index));
        }

        #endregion
    }
}