using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;
using BinarySerializer.Ray1;
using UnityEngine;

namespace R1Engine
{
    public static class WebJSONHelpers
    {
        public static void OutputJSONForWeb(string outputDir)
        {
            foreach (var mode in EnumHelpers.GetValues<GameModeSelection>().Where(x => Settings.GameDirectories.ContainsKey(x) && Directory.Exists(Settings.GameDirectories[x])))
            {
                var s = new GameSettings(mode, Settings.GameDirectories[mode], 0, 0);
                var m = mode.GetManager();

                foreach (var vol in m.GetLevels(s))
                {
                    s.EduVolume = vol.Name;
                    OutputJSONForWeb(Path.Combine(outputDir, $"{mode}{vol.Name}.json"), s);
                }
            }
        }

        public static void OutputJSONForWeb(string outputPath, GameSettings s)
        {
            var manager = s.GetGameManager;
            var attr = s.GameModeSelection.GetAttribute<GameModeAttribute>();
            var settings = s;
            var worlds = manager.GetLevels(settings).First(x => x.Name == null || x.Name == s.EduVolume).Worlds.ToArray();
            var names = MapNames.GetMapNames(attr.Game);

            var lvlWorldIndex = 0;

            var jsonObj = new
            {
                name = attr.DisplayName,
                mode = s.GameModeSelection.ToString(),
                folder = (string)null,
                icons = worlds.Select(x =>
                {
                    var icon = new
                    {
                        image = (string)null,
                        level = lvlWorldIndex
                    };

                    lvlWorldIndex += x.Maps.Length;

                    return icon;
                }),
                levels = worlds.Select(w => w.Maps.OrderBy(x => x).Select(lvl => new
                {
                    world = w.Index,
                    level = lvl,
                    nameInternal = s.MajorEngineVersion == MajorEngineVersion.GBA ? lvl.ToString() : (string)null,
                    name = names?.TryGetItem(w.Index)?.TryGetItem(lvl) ?? (s.MajorEngineVersion == MajorEngineVersion.GBA ? $"Map {lvl}" : $"Map {w.Index}-{lvl}")
                })).SelectMany(x => x)
            };

            JsonHelpers.SerializeToFile(jsonObj, outputPath);
        }

        public static void OutputEDUJSONForWeb(string dir, GameModeSelection mode, bool isPC)
        {
            var modeName = mode == GameModeSelection.RaymanQuizPC || mode == GameModeSelection.RaymanQuizPS1 ? "quiz" : "edu";
            var platformName = isPC ? "PC" : "PS1";
            var m = isPC ? new R1_PCEdu_Manager() : new R1_PS1Edu_Manager();

            foreach (var subDir in Directory.GetDirectories(dir, "*", SearchOption.TopDirectoryOnly))
            {
                var settings = new GameSettings(mode, subDir, 1, 1);

                using (var context = new R1Context(settings))
                {
                    foreach (var v in m.GetLevels(settings))
                    {
                        var vol = v.Name;
                        settings.EduVolume = vol;
                        context.GetSettings<Ray1Settings>().Volume = vol;
                        var specialPath = m.GetSpecialArchiveFilePath(vol);

                        context.AddFile(new LinearFile(context, specialPath));

                        var wldMap = m.LoadArchiveFile<PC_WorldMap>(context, specialPath, R1_PCBaseManager.R1_PC_ArchiveFileName.WLDMAP01);
                        var text = m.LoadArchiveFile<PC_LocFile>(context, specialPath, R1_PCBaseManager.R1_PC_ArchiveFileName.TEXT);

                        var worlds = v.Worlds;

                        var lvlWorldIndex = 0;

                        var jsonObj = new
                        {
                            name = $"NAME ({platformName} - {vol})",
                            mode = mode.ToString(),
                            folder = $"r1/{modeName}/{Path.GetFileName(subDir)}",
                            volume = vol,
                            icons = worlds.Select(x =>
                            {
                                var icon = new
                                {
                                    image = $"./img/icon/R1/R1-W{x.Index}.png",
                                    level = lvlWorldIndex
                                };

                                lvlWorldIndex += x.Maps.Length;

                                return icon;
                            }),
                            levels = worlds.Where(x => x.Index < 7).Select(w => w.Maps.OrderBy(x => x).Select(lvl => new
                            {
                                world = w.Index,
                                level = lvl,
                                nameInternal = $"{m.GetShortWorldName((World)w.Index)}{lvl:00}",
                                name = getLevelName(w.Index, lvl)
                            })).SelectMany(x => x)
                        };

                        JsonHelpers.SerializeToFile(jsonObj, Path.Combine(dir, $"{platformName.ToLower()}_{vol.ToLower()}.json"));

                        string getLevelName(int world, int level)
                        {
                            foreach (var lvl in wldMap.Levels.Take(wldMap.LevelsCount))
                            {
                                sbyte currentWorld = -1;
                                var levelIndex = 0;
                                var groupIndex = 1;

                                for (int i = 0; i < lvl.MapEntries.Length; i++)
                                {
                                    var entry = lvl.MapEntries[i];

                                    if (entry.Level == -1)
                                    {
                                        levelIndex = 0;
                                        groupIndex++;
                                        continue;
                                    }
                                    else
                                    {
                                        levelIndex++;
                                    }

                                    if (entry.World != -1)
                                        currentWorld = entry.World;

                                    if (currentWorld == world && entry.Level == level)
                                    {
                                        if (text.TextDefine.Length <= lvl.LevelName)
                                            return $"{(World)world} {level}";

                                        return $"{text.TextDefine[lvl.LevelName].Value.Trim('/')} {groupIndex}-{levelIndex}";
                                    }
                                }
                            }

                            return $"{(World)world} {level}";
                        }
                    }
                }
            }
        }

        public static void OutputGBAJSONForWeb(string dir, GameModeSelection mode)
        {
            var dirName = Path.GetFileName(dir);
            var name = dirName.Substring(0, dirName.LastIndexOf('_'));
            var settings = new GameSettings(mode, dir, 1, 1);

            var m = settings.GetGameManager;

            var worlds = m.GetLevels(settings).First().Worlds;

            var jsonObj = new
            {
                name = $"{mode.GetAttribute<GameModeAttribute>().DisplayName.Replace(" - EU", "").Replace(" - US", "").Replace(" - EU 1", "").Replace(" - EU 2", "")}",
                mode = mode.ToString(),
                folder = $"gba/{dirName}",
                levels = worlds.Select(w => w.Maps.OrderBy(x => x).Select(lvl => new
                {
                    world = w.Index,
                    level = lvl,
                    nameInternal = $"{lvl}",
                    name = $"Map {lvl}"
                })).SelectMany(x => x)
            };

            var outDir = Path.Combine(Path.GetDirectoryName(dir), "JSON", name);

            Directory.CreateDirectory(outDir);

            JsonHelpers.SerializeToFile(jsonObj, Path.Combine(outDir, $"{dirName.Substring(name.Length + 1)}.json"));
        }

        /*
        public static void OutputGBAVVJSONLevelListForWeb(GameModeSelection mode, bool includeInternalName)
        {
            // Helper for getting a line
            string getLine(int world, int level, string nameInternal, string name) => $"    {{ \"world\": {world}, \"level\": {level}, \"nameInternal\": \"{(includeInternalName ? nameInternal : null)}\", \"name\": \"{name}\" }},";

            StringBuilder str = new StringBuilder();

            var manager = (GBAVV_BaseManager)new GameSettings(mode, "", 0, 0).GetGameManager;

            for (var i = 0; i < manager.LevInfos.Length; i++)
            {
                var lev = manager.LevInfos[i];
                str.AppendLine(getLine(0, i, lev.LevelIndex != -1 ? lev.LevelIndex.ToString() : null, lev.DisplayName));
            }

            str.ToString().CopyToClipboard();
        }*/

        public static void GenerateWebData_GBAVV(GameModeSelection mode, string outputDir, string name, GBAVV_Variant variant, bool fullContent)
        {
            var variantDisplayName = variant.ToString().ToUpper().Replace("__", " ").Replace("_", "/");

            var gameFolderName = name.Replace(" ", "_").Replace(":", "").Replace("'", "").Replace(".", "").Replace("-", "").Replace("!", "").ToLower();
            var variantName = $"GBA - {variantDisplayName}";
            var gameVersionFileName = $"{variant.ToString().Replace("__", "_").ToLower()}";
            var versionName = $"GBA ({variantDisplayName})";

            GenerateJSON<GBAVV_BaseManager>(mode, outputDir, name, variantName, "gba_crash", gameFolderName, gameVersionFileName, "gba.png", versionName, fullContent, (str, manager) =>
            {
                if (manager is GBAVV_Volume_BaseManager vol)
                {
                    bool isFirst = true;

                    for (var i = 0; i < vol.LevInfos.Length; i++)
                    {
                        var lev = vol.LevInfos[i];
                        if (!isFirst)
                            str.AppendLine(",");

                        isFirst = false;

                        str.Append(GetLevelLine(0, i, $"{lev.InternalLevelName}-{lev.Map}", $"{lev.DisplayName}", !String.IsNullOrWhiteSpace(lev.InternalLevelName)));
                    }

                    str.AppendLine();
                }
                else
                {
                    bool multiWorlds = manager.GetLevels(null)[0].Worlds.Length > 1;
                    bool isFirst = true;

                    foreach (var world in manager.GetLevels(null)[0].Worlds)
                    {
                        foreach (var map in world.Maps)
                        {
                            if (!isFirst)
                                str.AppendLine(",");

                            isFirst = false;

                            str.Append(GetLevelLine(world.Index, map, "", $"Map {(multiWorlds ? $"{world.Index}-{map}" : $"{map}")}", false));
                        }
                    }

                    str.AppendLine();
                }
            });
        }
        public enum GBAVV_Variant
        {
            EU,
            US,
            US__Beta,
            JP,
            EU_US,
        }

        public static async Task OutputGameloftRRRJSONForWebAsync(string dir)
        {
            var worlds = new string[]
            {
                "Jungle",
                "Desert",
                "Ship"
            };

            var str = new StringBuilder();

            foreach (var mode in EnumHelpers.GetValues<GameModeSelection>().Where(x => x.ToString().StartsWith("RaymanRavingRabbidsMobile_")))
            {
                var values = mode.ToString().Split('_');

                var worldCounts = new int[worlds.Length];

                using (var context = new R1Context(new GameSettings(mode, Settings.GameDirectories[mode], 0, 0)))
                {
                    var m = (Gameloft_RRR_Manager)context.GetR1Settings().GetGameManager;
                    await m.LoadFilesAsync(context);
                    var levels = m.LoadLevelList(context);

                    var attr = mode.GetAttribute<GameModeAttribute>();

                    var folder = $"gameloft/rrr_{values[1]}_{values[2]}";

                    var jsonObj = new
                    {
                        name = attr.DisplayName.ReplaceFirst(", ", " - "),
                        mode = mode.ToString(),
                        folder = folder,
                        levels = levels.Levels.Select((lvl, i) => new
                        {
                            world = 0,
                            level = i,
                            nameInternal = m.GetLevelPath(i),
                            name = $"{worlds[lvl.World]} {++worldCounts[lvl.World]}"
                        }).ToArray()
                    };

                    var fileName = $"gameloft_{values[1]}_{values[2]}";
                    JsonHelpers.SerializeToFile(jsonObj, Path.Combine(dir, fileName + ".json"));

                    str.AppendLine($"{{ \"json\": \"{fileName}\", \"image\": \".\\/img\\/version\\/mobile.png\", \"name\": \"Mobile ({values[1]}, {values[2]})\", \"folder\": \"{folder}\" }},");
                }
            }

            str.ToString().CopyToClipboard();
        }

        public static async Task OutputGameloftKartJSONForWebAsync(string dir)
        {
            var str = new StringBuilder();

            foreach (var mode in EnumHelpers.GetValues<GameModeSelection>().Where(x => x.ToString().StartsWith("RaymanKartMobile_")))
            {
                var values = mode.ToString().Split('_');

                using (var context = new R1Context(new GameSettings(mode, Settings.GameDirectories[mode], 0, 0)))
                {
                    var m = (Gameloft_RK_Manager)context.GetR1Settings().GetGameManager;
                    await m.LoadFilesAsync(context);
                    var levelNames = MapNames.GetMapNames(mode.GetAttribute<GameModeAttribute>().Game);

                    var attr = mode.GetAttribute<GameModeAttribute>();

                    var gameNameInfo = String.Join("_", values.Skip(2));
                    var name = $"{values[1]}{(gameNameInfo.Length > 0 ? $"_{gameNameInfo}" : "")}";

                    var folder = $"gameloft/kart_{name}";

                    var jsonObj = new
                    {
                        name = attr.DisplayName.ReplaceFirst(", ", " - "),
                        mode = mode.ToString(),
                        folder = folder,
                        levels = levelNames[0].OrderBy(x => x.Key).Select(x => new
                        {
                            world = 0,
                            level = x.Key,
                            nameInternal = $"{m.GetLevelPath(x.Key)}-{m.GetLevelResourceIndex(x.Key)}",
                            name = $"{x.Value}"
                        }).ToArray()
                    };

                    var fileName = $"gameloft_{name}";
                    JsonHelpers.SerializeToFile(jsonObj, Path.Combine(dir, fileName + ".json"));

                    var gameNameInfo2 = String.Join("/", values.Skip(2));
                    str.AppendLine($"{{ \"json\": \"{fileName}\", \"image\": \".\\/img\\/version\\/mobile.png\", \"name\": \"Mobile ({values[1]}{(gameNameInfo2.Length > 0 ? $", {gameNameInfo2}" : "")})\", \"folder\": \"{folder}\" }},");

                    // Copy files
                    var inputDir = Settings.GameDirectories[mode];
                    var outputDir = Path.Combine(dir, folder);

                    Util.CopyDir(inputDir, outputDir);
                }
            }

            str.ToString().CopyToClipboard();
        }

        private static void GenerateJSON<T>(GameModeSelection mode, string outputDir, string name, string variantName, string engineFolder, string gameFolderName, string gameVersionFileName, string versionImg, string versionName, bool fullContent, Action<StringBuilder, T> generateAction)
            where T : BaseGameManager
        {
            StringBuilder str = new StringBuilder();

            str.AppendLine(GetHeader(name, variantName, mode, engineFolder, $"{gameFolderName}_{gameVersionFileName}"));

            var manager = new GameSettings(mode, "", 0, 0).GetGameManagerOfType<T>();

            generateAction(str, manager);

            str.AppendLine(GetFooter());

            // Output JSON
            var jsonPath = Path.Combine(outputDir, "JSON", gameFolderName, $"{gameVersionFileName}.json");
            Directory.CreateDirectory(Path.GetDirectoryName(jsonPath));
            File.WriteAllText(jsonPath, str.ToString());

            // Copy the game data
            var dataPath = Path.Combine(outputDir, "DATA", engineFolder, $"{gameFolderName}_{gameVersionFileName}");
            Directory.CreateDirectory(dataPath);
            Util.CopyDir(Settings.GameDirectories[mode], dataPath);

            // Copy content to clipboard
            var contentLine = $"{{ \"json\": \"{gameVersionFileName}\", \"image\": \".\\/img\\/version\\/{versionImg}\", \"name\": \"{versionName}\", \"folder\": \"{engineFolder}/{gameFolderName}_{gameVersionFileName}\" }},";
            if (fullContent)
            {
                var s = $"{{{Environment.NewLine}" +
                        $"\"title\": \"{name}\",{Environment.NewLine}" +
                        $"\"viewer\": \"ray1map\",{Environment.NewLine}" +
                        $"\"json\": \"{engineFolder}\\/{gameFolderName}\",{Environment.NewLine}" +
                        $"\"image\": \".\\/img\\/logo\\/{gameFolderName}.png\",{Environment.NewLine}" +
                        $"\"versions\": [{Environment.NewLine}" +
                        $"{contentLine}{Environment.NewLine}" +
                        $"]{Environment.NewLine}" +
                        $"}}";
                s.CopyToClipboard();
                Debug.Log("Copied content");
            }
            else
            {
                contentLine.CopyToClipboard();
                Debug.Log("Copied line");
            }
        }
        private static string GetHeader(string name, string variantName, GameModeSelection mode, string engineFolder, string gameFolderName) => $"{{{Environment.NewLine}  \"name\": \"{name} ({variantName})\",{Environment.NewLine}" +
                                                                                                                                                $"  \"mode\": \"{mode}\",{Environment.NewLine}" +
                                                                                                                                                $"  \"folder\": \"{engineFolder}/{gameFolderName}\",{Environment.NewLine}" +
                                                                                                                                                $"  \"levels\": [";
        private static string GetFooter() => $"   ]{Environment.NewLine}}}";
        private static string GetLevelLine(int world, int level, string nameInternal, string name, bool includeInternalName) => $"    {{ \"world\": {world}, \"level\": {level}, \"nameInternal\": \"{(includeInternalName ? nameInternal : "")}\", \"name\": \"{name}\" }}";
    }
}