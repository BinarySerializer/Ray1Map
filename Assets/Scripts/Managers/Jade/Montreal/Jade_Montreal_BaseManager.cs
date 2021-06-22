using R1Engine.Jade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace R1Engine
{
    public abstract class Jade_Montreal_BaseManager : Jade_BaseManager {
        // Levels
        public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(LevelInfos?.GroupBy(x => x.WorldName).Select((x, i) => {
            return new GameInfo_World(
                index: i,
                worldName: x.Key.ReplaceFirst(CommonLevelBasePath, String.Empty),
                maps: x.Select(m => (int)m.Key).ToArray(),
                mapNames: x.Select(m => m.MapName).ToArray());
        }).ToArray() ?? new GameInfo_World[0]);

        public virtual string CommonLevelBasePath => @"ROOT/Bin/";

		public override void CreateLevelList(LOA_Loader l) {
			var groups = l.FileInfos.GroupBy(f => Jade_Key.UncomposeBinKey(l.Context, f.Key)).OrderBy(f => f.Key);
			//List<KeyValuePair<uint, LOA_Loader.FileInfo>> levels = new List<KeyValuePair<uint, LOA_Loader.FileInfo>>();
			List<KeyValuePair<uint, LevelInfo>> levels = new List<KeyValuePair<uint, LevelInfo>>();
			foreach (var g in groups) {
				if (!g.Any(f => f.Key.Type == Jade_Key.KeyType.Map)) continue;
				var kvpair = g.FirstOrDefault(f => f.Value.FileName != null && f.Value.FileName.EndsWith(".wol"));
				string mapName = null;
				string worldName = null;
				LevelInfo.FileType? fileType = null;
				if (kvpair.Value == null) {
					kvpair = g.FirstOrDefault(f => f.Value.FileName != null && f.Key.Type == Jade_Key.KeyType.Map);

					if (kvpair.Value != null) {
						if(kvpair.Value.DirectoryName == "ROOT/Bin") {
							string FilenamePattern = @"^(?<name>.*)_(?<type>(wow|wol|oin))_(?<key>[0-9a-f]{1,8}).bin";
							Match m = Regex.Match(kvpair.Value.FileName, FilenamePattern, RegexOptions.IgnoreCase);
							if (m.Success) {
								var name = m.Groups["name"].Value;
								var keyStr = m.Groups["key"].Value;
								var type = m.Groups["type"].Value;
								if (type.ToLower() == "oin") {
									continue;
								} else {
									mapName = name;
									worldName = type.ToUpper();
									if (worldName == "WOW") {
										fileType = LevelInfo.FileType.WOW;
									} else if(worldName == "WOL") fileType = LevelInfo.FileType.WOL;
								}
							}
						}
					}
				}
				//if (kvpair.Value != null) {
				//	Debug.Log($"{g.Key:X8} - {kvpair.Value.FilePath }");
				//}
				levels.Add(new KeyValuePair<uint, LevelInfo>(g.Key, new LevelInfo(
					g.Key,
					kvpair.Value?.DirectoryName ?? "null",
					kvpair.Value?.FileName ?? "null",
					worldName: worldName,
					mapName: mapName,
					type: fileType)));
			}

			var str = new StringBuilder();

			foreach (var kv in levels.OrderBy(l => l.Value?.DirectoryPath).ThenBy(l => l.Value?.FilePath)) {
				str.AppendLine($"new LevelInfo(0x{kv.Key:X8}, \"{kv.Value?.DirectoryPath}\", \"{kv.Value?.FilePath}\"" +
					$"{(kv.Value?.OriginalWorldName != null ? $", worldName: \"{kv.Value.OriginalWorldName}\"" : "")}" +
					$"{(kv.Value?.OriginalMapName != null ? $", mapName: \"{kv.Value.OriginalMapName}\"" : "")}" +
					$"{(kv.Value?.OriginalType != null ? $", type: LevelInfo.FileType.{kv.Value.OriginalType}" : "")}" +
					$"),");
				//Debug.Log($"{kv.Key:X8} - {kv.Value }");
			}

			str.ToString().CopyToClipboard();
		}
	}
}
