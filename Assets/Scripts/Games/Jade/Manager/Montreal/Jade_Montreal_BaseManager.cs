using BinarySerializer;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Ray1Map.Jade;

namespace Ray1Map
{
    public abstract class Jade_Montreal_BaseManager : Jade_BaseManager {
		public override async UniTask CreateLevelList(LOA_Loader l) {
			await UniTask.CompletedTask;

			// TODO: Read WOLInfo if it exists
			List<KeyValuePair<uint, LevelInfo>> levels = new List<KeyValuePair<uint, LevelInfo>>();
			bool keysAreComposed = !l.Context.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_RRRTVParty);

			if (keysAreComposed) {
				var groups = l.FileInfos.GroupBy(f => Jade_Key.UncomposeBinKey(l.Context, f.Key)).OrderBy(f => f.Key);
				//List<KeyValuePair<uint, LOA_Loader.FileInfo>> levels = new List<KeyValuePair<uint, LOA_Loader.FileInfo>>();

				foreach (var g in groups) {
					if (!g.Any(f => f.Key.Type == Jade_Key.KeyType.Map)) continue;
					var kvpair = g.FirstOrDefault(f => f.Value.FileName != null && f.Value.FileName.EndsWith(".wol"));
					string mapName = null;
					string worldName = null;
					uint? overrideKey = null;
					LevelInfo.FileType? fileType = null;
					if (kvpair.Value == null) {
						kvpair = g.FirstOrDefault(f => f.Value.FileName != null && f.Key.Type == Jade_Key.KeyType.Map);

						if (kvpair.Value != null) {
							if (kvpair.Value.DirectoryName == "ROOT/Bin") {
								string FilenamePattern = @"^(?<name>.*)_(?<type>(wow|wol|oin))(?<optionalKey> \[0X(?<actualKey>[0-9a-f]{1,8})\])?_(?<key>[0-9a-f]{1,8}).bin";
								Match m = Regex.Match(kvpair.Value.FileName, FilenamePattern, RegexOptions.IgnoreCase);
								if (m.Success) {
									var name = m.Groups["name"].Value;
									var keyStr = m.Groups["key"].Value;
									var type = m.Groups["type"].Value;
									var optionalKey = m.Groups["actualKey"];
									if (optionalKey.Success) {
										var hex = optionalKey.Value;
										uint actualKey = 0;
										bool parsedSuccessfully = uint.TryParse(hex, System.Globalization.NumberStyles.HexNumber, CultureInfo.CurrentCulture, out actualKey);
										if (Jade_Key.GetBinaryForKey(l.Context, actualKey, Jade_Key.KeyType.Map) == Jade_Key.GetBinaryForKey(l.Context, g.Key, Jade_Key.KeyType.Map)) {
											overrideKey = actualKey;
										}
									}
									if (type.ToLower() == "oin") {
										continue;
									} else {
										mapName = name;
										worldName = type.ToUpper();
										if (worldName == "WOW") {
											fileType = LevelInfo.FileType.WOW;
										} else if (worldName == "WOL") fileType = LevelInfo.FileType.WOL;
									}
								}
							}
						}
					}
					if (!overrideKey.HasValue) {
						try {
							Jade_Reference<Jade_DummyFile> dummy = new Jade_Reference<Jade_DummyFile>(l.Context, new Jade_Key(l.Context, g.Key));
							l.BeginSpeedMode(dummy.Key, serializeAction: async s => {
								dummy.Resolve(flags: LOA_Loader.ReferenceFlags.MustExist | LOA_Loader.ReferenceFlags.DontUseCachedFile);
								await l.LoadLoopBINAsync();
							});
							await l.LoadLoop(l.Context.Deserializer);
							l.EndSpeedMode();
							var newKey = dummy?.Value?.BinFileHeader?.Key;
							if (newKey != null) {
								if (Jade_Key.GetBinaryForKey(l.Context, newKey, Jade_Key.KeyType.Map) == Jade_Key.GetBinaryForKey(l.Context, g.Key, Jade_Key.KeyType.Map)) {
									overrideKey = newKey;
								}
							}
						} catch (Exception) {
							l.EndSpeedMode();
						}
					}
					//if (kvpair.Value != null) {
					//	Debug.Log($"{g.Key:X8} - {kvpair.Value.FilePath }");
					//}
					levels.Add(new KeyValuePair<uint, LevelInfo>(overrideKey ?? g.Key, new LevelInfo(
						overrideKey ?? g.Key,
						kvpair.Value?.DirectoryName ?? "null",
						kvpair.Value?.FileName ?? "null",
						worldName: worldName,
						mapName: mapName,
						type: fileType)));
				}
			} else {
				var groups = l.FileInfos.OrderBy(f => f.Key?.Key);
				//List<KeyValuePair<uint, LOA_Loader.FileInfo>> levels = new List<KeyValuePair<uint, LOA_Loader.FileInfo>>();

				foreach (var f in groups) {
					string mapName = null;
					string worldName = null;
					uint? overrideKey = null;
					LevelInfo.FileType? fileType = null;
					if (f.Value.FileName != null && f.Value.FileName.EndsWith(".wol")) {
					} else if (f.Value.FileName != null && f.Value.FileName.EndsWith(".bin") && (f.Value.FileName.Contains("_wow_") || f.Value.FileName.Contains("_wol_"))) {

						if (f.Value.DirectoryName == "ROOT/Bin") {
							string FilenamePattern = @"^(?<name>.*)_(?<type>(wow|wol|oin))(?<optionalKey> \[0X(?<actualKey>[0-9a-f]{1,8})\])?_(?<key>[0-9a-f]{1,8}).bin";
							Match m = Regex.Match(f.Value.FileName, FilenamePattern, RegexOptions.IgnoreCase);
							if (m.Success) {
								var name = m.Groups["name"].Value;
								var keyStr = m.Groups["key"].Value;
								var type = m.Groups["type"].Value;
								var optionalKey = m.Groups["actualKey"];
								if (optionalKey.Success) {
									var hex = optionalKey.Value;
									uint actualKey = 0;
									bool parsedSuccessfully = uint.TryParse(hex, System.Globalization.NumberStyles.HexNumber, CultureInfo.CurrentCulture, out actualKey);
									if (Jade_Key.GetBinaryForKey(l.Context, actualKey, Jade_Key.KeyType.Map) == Jade_Key.GetBinaryForKey(l.Context, f.Key, Jade_Key.KeyType.Map)) {
										overrideKey = actualKey;
									}
								}
								if (type.ToLower() == "oin") {
									continue;
								} else {
									mapName = name;
									worldName = type.ToUpper();
									if (worldName == "WOW") {
										fileType = LevelInfo.FileType.WOW;
									} else if (worldName == "WOL") fileType = LevelInfo.FileType.WOL;
								}
							}
						}
					} else {
						continue;
					}
					if (!overrideKey.HasValue) {
						try {
							Jade_Reference<Jade_DummyFile> dummy = new Jade_Reference<Jade_DummyFile>(l.Context, new Jade_Key(l.Context, f.Key));
							l.BeginSpeedMode(dummy.Key, serializeAction: async s => {
								dummy.Resolve(flags: LOA_Loader.ReferenceFlags.MustExist | LOA_Loader.ReferenceFlags.DontUseCachedFile);
								await l.LoadLoopBINAsync();
							});
							await l.LoadLoop(l.Context.Deserializer);
							l.EndSpeedMode();
							var newKey = dummy?.Value?.BinFileHeader?.Key;
							if (newKey != null) {
								if (Jade_Key.GetBinaryForKey(l.Context, newKey, Jade_Key.KeyType.Map) == Jade_Key.GetBinaryForKey(l.Context, f.Key, Jade_Key.KeyType.Map)) {
									overrideKey = newKey;
								}
							}
						} catch (Exception) {
							l.EndSpeedMode();
						}
					}
					//if (kvpair.Value != null) {
					//	Debug.Log($"{g.Key:X8} - {kvpair.Value.FilePath }");
					//}
					levels.Add(new KeyValuePair<uint, LevelInfo>(overrideKey ?? f.Key, new LevelInfo(
						overrideKey ?? f.Key,
						f.Value?.DirectoryName ?? "null",
						f.Value?.FileName ?? "null",
						worldName: worldName,
						mapName: mapName,
						type: fileType)));
				}
			}

			var str = new StringBuilder();

			foreach (var kv in levels.OrderBy(l => l.Value?.DirectoryPath).ThenBy(l => l.Value?.FilePath)) {
				str.AppendLine($"\t\t\tnew LevelInfo(0x{kv.Key:X8}, \"{kv.Value?.DirectoryPath}\", \"{kv.Value?.FilePath}\"" +
					$"{(kv.Value?.OriginalWorldName != null ? $", worldName: \"{kv.Value.OriginalWorldName}\"" : "")}" +
					$"{(kv.Value?.OriginalMapName != null ? $", mapName: \"{kv.Value.OriginalMapName}\"" : "")}" +
					$"{(kv.Value?.OriginalType != null ? $", type: LevelInfo.FileType.{kv.Value.OriginalType}" : "")}" +
					$"),");
				//Debug.Log($"{kv.Key:X8} - {kv.Value }");
			}
			if (HasUnbinarizedData) {
				string unbinarizedStr = await CreateLevelListUnbinarized(l);
				str.AppendLine(unbinarizedStr);
			}

			str.ToString().CopyToClipboard();
		}

		public static async UniTask LoadTextures_Montreal(SerializerObject s, WOR_World w) {
			LOA_Loader Loader = s.Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
			if (!Loader.IsBinaryData) return;

			TEX_GlobalList texList = s.Context.GetStoredObject<TEX_GlobalList>(Jade_BaseManager.TextureListKey);

			// Hack so we can load WOWs separately in Montreal version
			Controller.DetailedState = $"Loading textures: Checks";
			var curPos = Loader.Bin.CurrentPosition;
			Jade_Key currentBinHeaderKey = null;
			HashSet<Jade_Key> dontAdd = new HashSet<Jade_Key>();
			while (currentBinHeaderKey == null || currentBinHeaderKey != (uint)Jade_Code.OffsetCode) {
				Jade_Reference<TEX_File_Montreal_Dummy> dummyFile = new Jade_Reference<TEX_File_Montreal_Dummy>(s.Context, new Jade_Key(s.Context, 0)) { ForceResolve = true };
				dummyFile?.Resolve(flags: LOA_Loader.ReferenceFlags.MustExist | LOA_Loader.ReferenceFlags.DontUseCachedFile | LOA_Loader.ReferenceFlags.DontCache | LOA_Loader.ReferenceFlags.Montreal_NoKeyChecks);
				await Loader.LoadLoopBINAsync();
				currentBinHeaderKey = dummyFile?.Value?.Key ?? null;
				if (dummyFile?.Value != null && dummyFile.Value.IsTexture && dummyFile.Value.Type != TEX_File.TexFileType.Raw) { // Raw textures are never referenced outside of RawPal textures
					var key = dummyFile.Value.BinFileHeader.Key;
					if (!texList.ContainsTextureKey(key) && !dontAdd.Contains(key)) new Jade_TextureReference(s.Context, key)?.Resolve();
					if (dummyFile.Value.Content_Animated != null) {
						foreach (var f in dummyFile.Value.Content_Animated.Frames) {
							if(!f.Texture.Key.IsNull && !dontAdd.Contains(f.Texture.Key)) dontAdd.Add(f.Texture.Key);
						}
					}
				}
			}
			Loader.Bin.CurrentPosition = curPos;
			// End of hack

			/*texList.SortTexturesList_Montreal();
			for (int i = 0; i < (texList.Textures?.Count ?? 0); i++) {
				texList.Textures[i].LoadInfo();
				await Loader.LoadLoopBINAsync();
			}*/

			Controller.DetailedState = $"Loading textures: Info";
			texList.SortTexturesList_Montreal();
			for (int i = 0; i < (texList.Textures?.Count ?? 0); i++) {
				texList.Textures[i].LoadInfo();
				await Loader.LoadLoopBINAsync();
			}

			Controller.DetailedState = $"Loading textures: Palettes";
			texList.SortPalettesList_Montreal();
			if (texList.Palettes != null) {
				for (int i = 0; i < (texList.Palettes?.Count ?? 0); i++) {
					texList.Palettes[i].Load();
				}
				await Loader.LoadLoopBINAsync();
			}
			Controller.DetailedState = $"Loading textures: Content";
			for (int i = 0; i < (texList.Textures?.Count ?? 0); i++) {
				texList.Textures[i].LoadContent();
				await Loader.LoadLoopBINAsync();
				if (texList.Textures[i].Content != null && texList.Textures[i].Info.Type != TEX_File.TexFileType.RawPal) {
					if (texList.Textures[i].Content.Width != texList.Textures[i].Info.Width ||
						texList.Textures[i].Content.Height != texList.Textures[i].Info.Height/* ||
						texList.Textures[i].Content.Color != texList.Textures[i].Info.Color*/) {
						throw new Exception($"Info & Content width/height mismatch for texture with key {texList.Textures[i].Key}");
					}
				}
			}
			Controller.DetailedState = $"Loading textures: CubeMaps";
			for (int i = 0; i < (texList.CubeMaps?.Count ?? 0); i++) {
				texList.CubeMaps[i].Load();
				await Loader.LoadLoopBINAsync();
			}
			Controller.DetailedState = $"Loading textures: End";
			texList.FillInReferences();

			w.TextureList_Montreal = texList;

			// New texture list
			texList = new TEX_GlobalList();
			s.Context.StoreObject<TEX_GlobalList>(Jade_BaseManager.TextureListKey, texList);

			// New sound list
			SND_GlobalList sndList = new SND_GlobalList();
			s.Context.StoreObject<SND_GlobalList>(SoundListKey, sndList);

			Loader.Caches[LOA_Loader.CacheType.TextureInfo].Clear();
			Loader.Caches[LOA_Loader.CacheType.TextureContent].Clear();
			for (int i = 0; i < (w.TextureList_Montreal.Textures?.Count ?? 0); i++) {
				var t = w.TextureList_Montreal.Textures[i];
				Jade_TextureReference tref = new Jade_TextureReference(s.Context, t.Key);
				tref.Resolve();
			}
		}

		public static async UniTask LoadWorld_Montreal(SerializerObject s, Jade_GenericReference world, int index, int count, bool isEditor) {
			LOA_Loader Loader = s.Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
			if (isEditor) {
				world.Resolve();
				await Loader.LoadLoop(s);
				if (world?.Value != null && world.Value is WOR_World w) {
					Controller.DetailedState = $"Loading world: {w.Name}";
					await w.JustAfterLoad_Montreal(s, false);
					await Jade_Montreal_BaseManager.LoadTextures_Montreal(s, w);
				}
				await Loader.LoadLoop(s);
			} else {
				Jade_Reference<Jade_BinTerminator> terminator = new Jade_Reference<Jade_BinTerminator>(s.Context, new Jade_Key(s.Context, (uint)Jade_Code.OffsetCode));
				Loader.BeginSpeedMode(world.Key, serializeAction: async s => {
					world.Resolve(flags: LOA_Loader.ReferenceFlags.MustExist | LOA_Loader.ReferenceFlags.DontUseCachedFile);
					await Loader.LoadLoopBINAsync();

					if (world?.Value != null && world.Value is WOR_World w) {
						if (count == 1) {
							Controller.DetailedState = $"Loading world: {w.Name}";
						} else {
							Controller.DetailedState = $"Loading world {index + 1}/{count}: {w.Name}";
						}
						await w.JustAfterLoad_Montreal(s, false);
						await Jade_Montreal_BaseManager.LoadTextures_Montreal(s, w);
					}
					terminator.Resolve();
					await Loader.LoadLoopBINAsync();

				});
				await Loader.LoadLoop(s);
				Loader.CurrentCacheType = LOA_Loader.CacheType.Main;
				Loader.Cache.Clear();
				Loader.EndSpeedMode();
			}
		}
	}
}
