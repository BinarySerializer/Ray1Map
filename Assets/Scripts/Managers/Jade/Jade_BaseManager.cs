using BinarySerializer;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using R1Engine.Jade;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace R1Engine {
	public abstract class Jade_BaseManager : BaseGameManager {
		// Levels
		public override GameInfo_Volume[] GetLevels(GameSettings settings) {
			return GameInfo_Volume.SingleVolume(LevelInfos?.GroupBy(x => x.WorldName).Select((x, i) => {
				return new GameInfo_World(
					index: i,
					worldName: x.Key.ReplaceFirst(CommonLevelBasePath, String.Empty),
					maps: x.Select(m => (int)m.Key).ToArray(),
					mapNames: x.Select(m => m.MapName).ToArray());
			}).ToArray() ?? new GameInfo_World[0]);
		}

		public virtual string CommonLevelBasePath => @"ROOT/EngineDatas/06 Levels/";

		public abstract LevelInfo[] LevelInfos { get; }

		public virtual bool HasUnbinarizedData => false;

		// Game actions
		public override GameAction[] GetGameActions(GameSettings settings) {
			GameAction[] actions = new GameAction[] {
				new GameAction("Extract BF file(s)", false, true, (input, output) => ExtractFilesAsync(settings, output, false, true)),
				new GameAction("Extract BF file(s) - BIN decompression", false, true, (input, output) => ExtractFilesAsync(settings, output, true, true)),
				new GameAction("Create level list", false, false, (input, output) => CreateLevelListAsync(settings)),
				new GameAction("Export localization", false, true, (input, output) => ExportLocalizationAsync(settings, output, false)),
				new GameAction("Export textures", false, true, (input, output) => ExportTexturesAsync(settings, output, true)),
			};
			if (HasUnbinarizedData) {
				actions = actions.Concat(new GameAction[] {
					new GameAction("Export textures (unbinarized)", false, true, (input, output) => ExportTexturesUnbinarized(settings, output))
				}).ToArray();
			}
			if (TexturesGearBFPath != null) {
				actions = actions.Concat(new GameAction[] {
					new GameAction("Export textures (Gear BF)", false, true, (input, output) => ExportTexturesGearBF(settings, output))
				}).ToArray();
			}
			if (SoundGearBFPath != null) {
				actions = actions.Concat(new GameAction[] {
					new GameAction("Export sound (Gear BF)", false, true, (input, output) => ExportGearBF(settings, SoundGearBFPath, output, "wav"))
				}).ToArray();
			}
			return actions;
		}
        public async UniTask ExtractFilesAsync(GameSettings settings, string outputDir, bool decompressBIN = false, bool exportKeyList = false) {
            using (var context = new R1Context(settings)) {
				var s = context.Deserializer;
                await LoadFilesAsync(context);
				Dictionary<uint, string> fileKeys = new Dictionary<uint, string>();
				foreach (var bfPath in BFFiles) {
					var bf = await LoadBF(context, bfPath);
					List<KeyValuePair<long, long>> fileSizes = new List<KeyValuePair<long, long>>();
					try {
						string[] directories = new string[0];
						for (int fatIndex = 0; fatIndex < bf.FatFiles.Length; fatIndex++) {
							var fat = bf.FatFiles[fatIndex];
							if (fat.DirectoryInfos?.Length > 0) {
								directories = new string[fat.DirectoryInfos.Length];
								for (int i = 0; i < directories.Length; i++) {
									var dir = fat.DirectoryInfos[i];
									var dirName = dir.Name;
									var curDir = dir;
									while (curDir.ParentDirectory != -1) {
										curDir = fat.DirectoryInfos[curDir.ParentDirectory];
										dirName = Path.Combine(curDir.Name, dirName);
									}
									directories[i] = dirName;
									Directory.CreateDirectory(Path.Combine(outputDir, dirName));
								}
							}
							for (int i = 0; i < fat.Files.Length; i++) {
								var f = fat.Files[i];
								var fi = fat.FileInfos[i];
								bool fileIsCompressed = decompressBIN && f.IsCompressed;
								if (fileIsCompressed && fi.Name != null && !fi.Name.EndsWith(".bin")) {
									// Hack. Really whether it's compressed or not also depends on whether speed mode is enabled when loading this specific key
									fileIsCompressed = false;
								}
								//UnityEngine.Debug.Log($"{bf.Offset.file.AbsolutePath} - {i} - {f.Key} - {(fi.Name != null ? Path.Combine(directories[fi.ParentDirectory], fi.Name) : fi.Name)}");
								byte[] fileBytes = null;
								await bf.SerializeFile(s, fatIndex, i, (fileSize) => {
									fileSizes.Add(new KeyValuePair<long, long>(f.FileOffset.AbsoluteOffset, fileSize + 4));
									if (fileIsCompressed) {
										try {
											s.DoEncoded(new Jade_Lzo1xEncoder(fileSize, xbox360Version: settings.EngineFlags.HasFlag(EngineFlags.Jade_Xenon)), () => {
												fileBytes = s.SerializeArray<byte>(fileBytes, s.CurrentLength, name: "FileBytes");
											});
										} catch (Exception) {
											UnityEngine.Debug.LogWarning($"File with key {f.Key} is not compressed, trying uncompressed version");
											fileIsCompressed = false;
											s.Goto(f.FileOffset + 4);
											fileBytes = s.SerializeArray<byte>(fileBytes, fileSize, name: "FileBytes");
										}
									} else {
										fileBytes = s.SerializeArray<byte>(fileBytes, fileSize, name: "FileBytes");
									}
								});
								string fileName = null;
								if (fi.Name != null) {
									fileName = fi.Name;
									if (fileIsCompressed) {
										fileName += ".dec";
									}
									if (fi.ParentDirectory >= 0) {
										Util.ByteArrayToFile(Path.Combine(outputDir, directories[fi.ParentDirectory], fileName), fileBytes);
										if(exportKeyList) fileKeys[f.Key.Key] = Path.Combine(directories[fi.ParentDirectory], fileName);
									}
								} else {
									fileName = $"no_name_{fat.Files[i].Key:X8}.dat";
									if (fileIsCompressed) {
										fileName += ".dec";
									}
									Util.ByteArrayToFile(Path.Combine(outputDir, fileName), fileBytes);
									if (exportKeyList) fileKeys[f.Key.Key] = fileName;
								}
							}
						}
						// Extract hidden files
						{
							s.Goto(bf.Offset);
							var sortedFileSizes = fileSizes.OrderBy(f => f.Key).ToArray();
							for (int i = 0; i < sortedFileSizes.Length; i++) {
								var nextOffset = i == sortedFileSizes.Length - 1 ? s.CurrentLength : sortedFileSizes[i + 1].Key;
								var curOffset = sortedFileSizes[i].Key;
								var curSize = sortedFileSizes[i].Value;
								while (curOffset + curSize < nextOffset) {
									curOffset = curOffset + curSize;
									Pointer curPtr = bf.Offset + curOffset;
									byte[] fileBytes = null;
									Debug.Log($"Reading hidden file @ {curPtr}");
									await bf.SerializeAt(s, curPtr, (fileSize) => {
										fileBytes = s.SerializeArray<byte>(fileBytes, fileSize, name: "FileBytes");
									});
									if (fileBytes.Length == 0) {
										s.DoAt(curPtr, () => {
											fileBytes = s.SerializeArray<byte>(fileBytes, nextOffset - curOffset, name: "FileBytes");
										});
										curSize = fileBytes.Length;
									} else {
										curSize = fileBytes.Length + 4;
									}
									string fileName = $"hidden_file_{curPtr.StringFileOffset}.dat";
									Util.ByteArrayToFile(Path.Combine(outputDir, fileName), fileBytes);
								}
								if (curOffset + curSize > nextOffset) {
									UnityEngine.Debug.Log($"error @ {(bf.Offset+curOffset)}");
								}
							}
							/*{
								s.Goto(bf.Offset);
								byte[] fileBytes = File.ReadAllBytes(Path.Combine(context.BasePath, BFFile));
								foreach (var c in fileSizes) {
									for (int i = 0; i < c.Value; i++) {
										fileBytes[c.Key + i] = 0;
									}
								}
								Util.ByteArrayToFile(Path.Combine(outputDir, "unread.bf"), fileBytes);
							}*/
						}
					} catch (Exception ex) {
						UnityEngine.Debug.LogError(ex);
					}
				}
				if (exportKeyList) {
					StringBuilder b = new StringBuilder();
					foreach (var kv in fileKeys) {
						b.AppendLine($"{kv.Key:X8},{kv.Value}");
					}
					File.WriteAllText(Path.Combine(outputDir, "keylist.txt"), b.ToString());
				}
            }
        }

		public async UniTask ExportLocalizationAsync(GameSettings settings, string outputDir, bool perWorld) {
			var parsedTexs = new HashSet<uint>();

			var levIndex = 0;
			if(settings.EngineVersionTree.HasParent(EngineVersion.Jade_Montreal))
				throw new NotImplementedException($"Not yet implemented for Montreal version");

			Dictionary<int, Dictionary<string, string>> langTables = null;
			if(!perWorld) langTables = new Dictionary<int, Dictionary<string, string>>();


			void ExportLangTable(string name) {
				var output = langTables.Select(langTable => new
				{
					Language = ((TEXT_Language)langTable.Key).ToString(),
					Text = langTable.Value
					/*Text = langTable.Value.Select(ltv => new {
						Key = ltv.Key,
						Value = ltv.Value
					}).ToArray()*/
				});
				string json = JsonConvert.SerializeObject(output, Formatting.Indented);
				Util.ByteArrayToFile(Path.Combine(outputDir, $"{name}.json"), Encoding.UTF8.GetBytes(json));
			}

			foreach (var lev in LevelInfos) {
				if (lev?.Type.HasValue ?? false) {
					// If there are WOL files, there are also raw WOW files. It's better to process those one by one.
					if (lev.Type.Value.HasFlag(LevelInfo.FileType.WOL) || lev.Type.Value.HasFlag(LevelInfo.FileType.Unbinarized)) {
						levIndex++;
						continue;
					}
				}

				Debug.Log($"Exporting for level {levIndex++ + 1}/{LevelInfos.Length}: {lev.MapName}");

				try {
					using (var context = new R1Context(settings)) {
						await LoadFilesAsync(context);
						await LoadJadeAsync(context, new Jade_Key(context, lev.Key), LoadFlags.Maps | LoadFlags.TextNoSound);

						Debug.Log($"Loaded level. Exporting text...");
						LOA_Loader loader = context.GetStoredObject<LOA_Loader>(LoaderKey);
						foreach (var w in loader.LoadedWorlds) {
							if (perWorld) langTables = new Dictionary<int, Dictionary<string, string>>();
							var text = w?.Text;
							await ExportTextList(text, w?.Name);
						}

						async UniTask ExportTextList(Jade_TextReference text, string worldName) {
							await UniTask.CompletedTask;
							if(text.IsNull) return;
							foreach (var kv in text.Text) {
								var langID = kv.Key;
								var allText = kv.Value;
								if(!langTables.ContainsKey(langID)) langTables[langID] = new Dictionary<string, string>();
								foreach (var g in allText.Text) {
									if (g.IsNull || g.Value == null) continue;
									var group = (TEXT_TextGroup)g.Value;
									var usedRef = group?.GetUsedReference(langID);
									if(usedRef == null || usedRef.IsNull || usedRef.Value == null) continue;
									var txl = (TEXT_TextList)usedRef.Value;
									foreach (var t in txl.Text) {
										var id = t.IDString ?? $"{txl.Key}-{t.IdKey}";
										var content = t.Text;
										if (langTables[langID].ContainsKey(id) && langTables[langID][id] != content) {
											Debug.LogWarning($"Different content for same IdKey {id}: {langTables[langID][id]} - {content}");
										}
										langTables[langID][id] = content;
									}
								}
							}
							if(perWorld) ExportLangTable(worldName);
						}
					}
				} catch (Exception ex) {
					Debug.LogError($"Failed to export for level {lev.MapName}: {ex.ToString()}");
				}


				// Unload textures
				await Controller.WaitIfNecessary();
				await Resources.UnloadUnusedAssets();

				GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true);
				GC.WaitForPendingFinalizers();
				/*if (settings.EngineVersionTree.HasParent(EngineVersion.Jade_Montpellier)) {
					await UniTask.Delay(500);
				}*/
			}
			if(!perWorld) ExportLangTable("localization");

			Debug.Log($"Finished export");
		}
		public async UniTask ExportTexturesAsync(GameSettings settings, string outputDir, bool useComplexNames)
        {
			var parsedTexs = new HashSet<uint>();

			var levIndex = 0;
			uint currentKey = 0;

            foreach (var lev in LevelInfos)
            {
				// If there are WOL files, there are also raw WOW files. It's better to process those one by one.
				if (lev.Type.Value.HasFlag(LevelInfo.FileType.WOL) || lev.Type.Value.HasFlag(LevelInfo.FileType.Unbinarized)) {
					levIndex++;
					continue;
				}

				Debug.Log($"Exporting for level {levIndex++ + 1}/{LevelInfos.Length}: {lev.MapName}");

                try
                {
                    using (var context = new R1Context(settings)) {
						currentKey = 0;
						await LoadFilesAsync(context);
                        await LoadJadeAsync(context, new Jade_Key(context, lev.Key), LoadFlags.Maps | LoadFlags.Textures);

						TEX_GlobalList texList = context.GetStoredObject<TEX_GlobalList>(TextureListKey);

						string worldName = null;
						if (context.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal)) {
							LOA_Loader loader = context.GetStoredObject<LOA_Loader>(LoaderKey);
							foreach (var w in loader.LoadedWorlds) {
								texList = w?.TextureList_Montreal;
								worldName = w?.Name;
								await ExportTexList(texList);
							}
						} else {
							await ExportTexList(texList);
						}

						async UniTask ExportTexList(TEX_GlobalList texList) {
							if (texList.Textures != null && texList.Textures.Any()) {
								Debug.Log($"Loaded level. Exporting {texList?.Textures?.Count} textures...");
								await Controller.WaitIfNecessary();

								foreach (var t in texList.Textures) {
									if (parsedTexs.Contains(t.Key.Key))
										continue;

									if (context.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal)
										&& t.Content == null && t.Info == null) continue;

									parsedTexs.Add(t.Key.Key);

									Texture2D tex = null;
									currentKey = t.Key;
									tex = (t.Content ?? t.Info).ToTexture2D();

									if (tex == null)
										continue;

									string name = $"{t.Key.Key:X8}";
									if (useComplexNames) {
										if (worldName != null) name = $"{worldName}/{name}";
										var file = (t.Content ?? t.Info);
										if (file != null) {
											name += $"_{file.Type}";
											if (file.Content_Xenon != null) name += $"_{file.Content_Xenon.Format}";
											if (file.Content_JTX != null) name += $"_{file.Content_JTX.Format}";
											if (file.Content_TGA != null) name += $"_{file.Format}";
										}
									}

									Util.ByteArrayToFile(Path.Combine(outputDir, $"{name}.png"), tex.EncodeToPNG());
								}
							}
							if (texList.CubeMaps != null && texList.CubeMaps.Any()) {
								foreach (var t in texList.CubeMaps) {
									if (parsedTexs.Contains(t.Key.Key))
										continue;
									parsedTexs.Add(t.Key.Key);
									var dds = t.Value.DDS;

									for (int i = 0; i < dds.Textures.Length; i++) {
										Util.ByteArrayToFile(Path.Combine(outputDir, "Cubemaps", $"{t.Key.Key:X8}_{i}.png"), dds.Textures[i].Items[0].ToTexture2D().EncodeToPNG());
									}
								}
							}
						}
                    }
                }
				catch (Exception ex)
                {
					if (currentKey == 0) {
						Debug.LogError($"Failed to export for level {lev.MapName}: {ex.ToString()}");
					} else {
						Debug.LogError($"Failed to export for level {lev.MapName}: [{currentKey:X8}] {ex.ToString()}");
					}
                }


				// Unload textures
				await Controller.WaitIfNecessary();
				await Resources.UnloadUnusedAssets();

				GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true);
				GC.WaitForPendingFinalizers();
				if (settings.EngineVersionTree.HasParent(EngineVersion.Jade_Montpellier)) {
					await UniTask.Delay(2000);
				}
			}

            Debug.Log($"Finished export");
		}
		public async UniTask ExportTexturesUnbinarized(GameSettings settings, string outputDir) {
			using (var context = new R1Context(settings)) {
				await LoadFilesAsync(context);
				List<BIG_BigFile> bfs = new List<BIG_BigFile>();
				foreach (var bfPath in BFFiles) {
					var bf = await LoadBF(context, bfPath);
					bfs.Add(bf);
				}
				// Set up loader
				LOA_Loader loader = new LOA_Loader(bfs.ToArray(), context);
				context.StoreObject<LOA_Loader>(LoaderKey, loader);

				// Set up texture list
				TEX_GlobalList texList = new TEX_GlobalList();
				context.StoreObject<TEX_GlobalList>(TextureListKey, texList);

				foreach (var kvp in loader.FileInfos) {
					var fileInfo = kvp.Value;
					if (fileInfo.FileName != null && (fileInfo.FileName.EndsWith(".tex") || fileInfo.FileName.EndsWith(".jtx"))) {
						try {
							Jade_TextureReference texRef = new Jade_TextureReference(context, fileInfo.Key);
							texRef.Resolve();

							var t = texList.Textures[0];

							Texture2D tex = null;
							uint currentKey = t.Key;
							tex = (t.Content ?? t.Info).ToTexture2D();

							if (tex == null)
								continue;

							string name = fileInfo.FilePath;
							/*if ((t.Content ?? t.Info)?.Content_Xenon != null) {
								name += "_" + (t.Content ?? t.Info).Content_Xenon.Format.ToString();
							}*/
							Util.ByteArrayToFile(Path.Combine(outputDir, $"{name}.png"), tex.EncodeToPNG());
						} catch (Exception ex) {
							UnityEngine.Debug.LogError(ex);
						} finally {
							texList.Textures?.Clear();
							texList.Palettes?.Clear();
						}
						await Controller.WaitIfNecessary();
					}
				}
			}
		}
		
		public async UniTask ExportGearBF(GameSettings settings, string bfPath, string outputDir, string extension) {
			if (bfPath == null) return;
			using (var context = new R1Context(settings)) {
				await LoadFilesAsync(context);
				GEAR_BigFile bf = await LoadGearBF(context, bfPath);

				var s = context.Deserializer;
				foreach (var crc in bf.FileCRC32) {
					await bf.SerializeFile(s, crc, (filesize) => {
						var bytes = s.SerializeArray<byte>(default, filesize, name: "bytes");
						Util.ByteArrayToFile(Path.Combine(outputDir, $"{crc:X8}.{extension}"), bytes);
					});
				}
			}
			// TODO: "texture_map.txt" is the key<->filename map for the textures.bf file. The file ID is CRC32 of filename.
			// TODO: Figure out the correct converted filename (the .bra is replaced, but with what?)
		}

		public async UniTask ExportTexturesGearBF(GameSettings settings, string outputDir) {
			string bfPath = TexturesGearBFPath;
			if (bfPath == null) return;
			using (var context = new R1Context(settings)) {
				await LoadFilesAsync(context);
				GEAR_BigFile bf = await LoadGearBF(context, bfPath);
				var s = context.Deserializer;
				HashSet<int> exportedCRC = new HashSet<int>();

				int getCrc(string str) {
					var crcObj = new Ionic.Crc.CRC32();
					var bytes = Encoding.GetBytes(str);
					int crc32 = 0;
					using (var ms = new MemoryStream(bytes)) {
						crc32 = crcObj.GetCrc32(ms);
					}
					return crc32;
				}
				async UniTask<byte[]> readBytes(int crc) {
					byte[] bytes = null;
					await bf.SerializeFile(s, crc, (filesize) => {
						bytes = s.SerializeArray<byte>(default, filesize, name: "bytes");
					});
					return bytes;
				}
				async UniTask<byte[]> readAndExport(string filename) {
					var crc = getCrc(filename);
					byte[] bytes = await readBytes(getCrc(filename));
					if (bytes != null) {
						if(!exportedCRC.Contains(crc)) exportedCRC.Add(crc);
						Util.ByteArrayToFile(Path.Combine(outputDir, filename), bytes);
					}
					return bytes;
				}
				byte[] textureMapBytes = await readAndExport("texture_map.txt");
				if (textureMapBytes != null) {
					using (var ms = new MemoryStream(textureMapBytes)) {
						using (var reader = new StreamReader(ms, Encoding)) {
							string line;
							while ((line = reader.ReadLine()) != null) {
								var split = line.Split(';');
								if (split.Length == 2) {
									var filename = split[0];
									var key = split[1];

									if (filename.Length > 4) {
										var basename = filename.Substring(0, filename.Length - 4);
										await readAndExport($"{basename}_ps3.dds");
										await readAndExport($"{basename}_nm_ps3.dds");
									}
								}
							}
						}
					}
				}
				foreach (var crc in bf.FileCRC32) {
					if(exportedCRC.Contains(crc)) continue;
					byte[] bytes = await readBytes(crc);
					if (bytes != null) {
						Util.ByteArrayToFile(Path.Combine(outputDir, "unnamed", $"{crc:X8}.dds"), bytes);
					}
				}
			}
		}
		public async UniTask CreateLevelListAsync(GameSettings settings) {

			using (var context = new R1Context(settings)) {
				await LoadFilesAsync(context);
				List<BIG_BigFile> bfs = new List<BIG_BigFile>();
				foreach (var bfPath in BFFiles) {
					var bf = await LoadBF(context, bfPath);
					bfs.Add(bf);
				}
				// Set up loader
				LOA_Loader loader = new LOA_Loader(bfs.ToArray(), context);
				context.StoreObject<LOA_Loader>(LoaderKey, loader);

				// Load jade.spe
				LoadJadeSPE(context);
				
				// Create level list				
				await CreateLevelList(loader);
				Debug.Log($"Copied to clipboard.");
			}
		}

		// Version properties
		public abstract string[] BFFiles { get; }
		public virtual string[] FixWorlds => null;
		public virtual string JadeSpePath => null;
		public virtual string TexturesGearBFPath => null;
		public virtual string GeometryBFPath => null;
		public virtual string SoundGearBFPath => null;

		// Helpers
        public virtual async UniTask CreateLevelList(LOA_Loader l) {
			await Task.CompletedTask;
			var groups = l.FileInfos.GroupBy(f => Jade_Key.UncomposeBinKey(l.Context, f.Key)).OrderBy(f => f.Key);
			List<KeyValuePair<uint, LOA_Loader.FileInfo>> levels = new List<KeyValuePair<uint, LOA_Loader.FileInfo>>();
			foreach (var g in groups) {
				if(!g.Any(f => f.Key.Type == Jade_Key.KeyType.Map)) continue;
				var kvpair = g.FirstOrDefault(f => f.Value.FileName != null && f.Value.FileName.EndsWith(".wol"));
				if (kvpair.Value == null) {
					kvpair = g.FirstOrDefault(f => f.Value.FileName != null && f.Key.Type == Jade_Key.KeyType.Map);
				}
				//if (kvpair.Value != null) {
				//	Debug.Log($"{g.Key:X8} - {kvpair.Value.FilePath }");
				//}
				levels.Add(new KeyValuePair<uint, LOA_Loader.FileInfo>(g.Key, kvpair.Value));
			}

			var str = new StringBuilder();

			foreach (var kv in levels.OrderBy(l => l.Value?.DirectoryName).ThenBy(l => l.Value?.FileName)) 
            {
				str.AppendLine($"\t\t\tnew LevelInfo(0x{kv.Key:X8}, \"{kv.Value?.DirectoryName}\", \"{kv.Value?.FileName}\"),");
				//Debug.Log($"{kv.Key:X8} - {kv.Value }");
			}
			if (HasUnbinarizedData) {
				string unbinarizedStr = await CreateLevelListUnbinarized(l);
				str.AppendLine(unbinarizedStr);
			}

			str.ToString().CopyToClipboard();
		}
		public virtual async UniTask<string> CreateLevelListUnbinarized(LOA_Loader l) {
			await Task.CompletedTask;
			var groups = l.FileInfos
				.Where(f => !string.IsNullOrEmpty(f.Value.FileName) && (f.Value.FileName.EndsWith(".wow") || f.Value.FileName.EndsWith(".wol")))
				.OrderBy(f => f.Key.Key);
			List<KeyValuePair<uint, LevelInfo>> levels = new List<KeyValuePair<uint, LevelInfo>>();
			foreach (var g in groups) {
				string extension = g.Value.FileName.Substring(g.Value.FileName.Length-3, 3);
				string filePath = g.Value.FilePath.Substring(0, g.Value.FilePath.Length - 4);
				string fileName = g.Value.FileName.Substring(0, g.Value.FileName.Length - 4);
				if (filePath.StartsWith("ROOT/EngineDatas/")) filePath = filePath.Remove(0, "ROOT/EngineDatas/".Length);
				if (filePath.StartsWith("06 Levels/")) filePath = filePath.Remove(0, "06 Levels/".Length);
				if (filePath.EndsWith($"{fileName}/{fileName}")) filePath = filePath.Substring(0, filePath.Length - fileName.Length - 1);
				levels.Add(new KeyValuePair<uint, LevelInfo>(g.Key, new LevelInfo(
					g.Key,
					g.Value?.DirectoryName ?? "null",
					g.Value?.FileName ?? "null",
					worldName: $"Editor {extension.ToUpper()}",
					mapName: filePath,
					type: extension == "wol" ? LevelInfo.FileType.WOLUnbinarized : LevelInfo.FileType.WOWUnbinarized)));
				//levels.Add(new KeyValuePair<uint, LOA_Loader.FileInfo>(g.Key, g.Value));
			}

			var str = new StringBuilder();
			str.AppendLine($"\t\t\t// Unbinarized");
			foreach (var kv in levels.OrderBy(l => l.Value?.DirectoryPath).ThenBy(l => l.Value?.FilePath)) {
				str.AppendLine($"\t\t\tnew LevelInfo(0x{kv.Key:X8}, \"{kv.Value?.DirectoryPath}\", \"{kv.Value?.FilePath}\"" +
					$"{(kv.Value?.OriginalWorldName != null ? $", worldName: \"{kv.Value.OriginalWorldName}\"" : "")}" +
					$"{(kv.Value?.OriginalMapName != null ? $", mapName: \"{kv.Value.OriginalMapName}\"" : "")}" +
					$"{(kv.Value?.OriginalType != null ? $", type: LevelInfo.FileType.{kv.Value.OriginalType}" : "")}" +
					$"),");
				//Debug.Log($"{kv.Key:X8} - {kv.Value }");
			}

			return str.ToString();
		}
		public async UniTask<BIG_BigFile> LoadBF(Context context, string bfPath) {
			var s = context.Deserializer;
			s.Goto(context.GetFile(bfPath).StartPointer);
			await s.FillCacheForReadAsync((int)BIG_BigFile.HeaderLength);
			var bfFile = FileFactory.Read<BIG_BigFile>(bfPath, context);
			await s.FillCacheForReadAsync((int)bfFile.TotalFatFilesLength);
			bfFile.SerializeFatFiles(s);
			return bfFile;
		}
		public async UniTask<GEAR_BigFile> LoadGearBF(Context context, string bfPath) {
			var s = context.Deserializer;
			s.Goto(context.GetFile(bfPath).StartPointer);
			await s.FillCacheForReadAsync(GEAR_BigFile.HeaderSize);
			var bfFile = FileFactory.Read<GEAR_BigFile>(bfPath, context);
			await s.FillCacheForReadAsync((int)bfFile.ArraysSize);
			bfFile.SerializeArrays(s);
			return bfFile;
		}

		public void LoadJadeSPE(Context context) {
			if(JadeSpePath == null) return;
			var s = context.Deserializer;
			LOA_Loader loader = context.GetStoredObject<LOA_Loader>(LoaderKey);
			loader.SpecialArray = FileFactory.Read<LOA_SpecialArray>(JadeSpePath, context);
		}
		public async UniTask<Jade_Reference<WOR_WorldList>> LoadWorldList(Context context, Jade_Key worldKey, LoadFlags loadFlags, bool isFix = false, bool isEditor = false) {
			LOA_Loader loader = context.GetStoredObject<LOA_Loader>(LoaderKey);
			loader.IsLoadingFix = isFix;

			Jade_Reference<WOR_WorldList> worldList = new Jade_Reference<WOR_WorldList>(context, worldKey);

			// Set up texture list
			TEX_GlobalList texList = new TEX_GlobalList();
			context.StoreObject<TEX_GlobalList>(TextureListKey, texList);
			loader.Caches[LOA_Loader.CacheType.TextureInfo].Clear();
			loader.Caches[LOA_Loader.CacheType.TextureContent].Clear();

			if (!isEditor) {
				loader.BeginSpeedMode(worldKey, serializeAction: async s => {
					if (context.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal)) {
						worldList.Resolve(queue: LOA_Loader.QueueType.Maps, flags:
							LOA_Loader.ReferenceFlags.Log | LOA_Loader.ReferenceFlags.Montreal_NoKeyChecks |
							LOA_Loader.ReferenceFlags.DontCache | LOA_Loader.ReferenceFlags.DontUseCachedFile);
					} else {
						worldList.Resolve(queue: LOA_Loader.QueueType.Maps);
					}
					await loader.LoadLoopBINAsync();
					if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montpellier)) {
						if (worldList?.Value != null) {
							await worldList.Value.ResolveReferences(s);
						}
					}
				});
				await loader.LoadLoop(context.Deserializer);

				if (context.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal)) {
					loader.EndSpeedMode();
					await worldList.Value.ResolveReferences_Montreal(context.Deserializer, isEditor);
					loader.BeginSpeedMode(worldKey);
				}
			} else {
				SerializerObject s = context.Deserializer;
				// Unbinarized
				worldList.Resolve(queue: LOA_Loader.QueueType.Current);
				await loader.LoadLoop(s);
				if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montpellier)) {
					if (worldList?.Value != null) {
						await worldList.Value.ResolveReferences(s);
						await loader.LoadLoop(s);
					}
				} else {
					await worldList.Value.ResolveReferences_Montreal(context.Deserializer, isEditor);
				}
			}

			if (!isEditor && loadFlags.HasFlag(LoadFlags.Textures) && texList.Textures != null && texList.Textures.Any()
				&& context.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montpellier)) {
				Controller.DetailedState = $"Loading textures";
				loader.BeginSpeedMode(new Jade_Key(context, Jade_Key.KeyTypeTextures), serializeAction: async s => {
					Controller.DetailedState = $"Loading textures: Info";
					for (int i = 0; i < texList.Textures.Count; i++) {
						texList.Textures[i].LoadInfo();
						await loader.LoadLoopBINAsync();
					}
					Controller.DetailedState = $"Loading textures: Palettes";
					if (texList.Palettes != null) {
						for (int i = 0; i < (texList.Palettes?.Count ?? 0); i++) {
							texList.Palettes[i].Load();
						}
						await loader.LoadLoopBINAsync();
					}
					Controller.DetailedState = $"Loading textures: Content";
					for (int i = 0; i < texList.Textures.Count; i++) {
						texList.Textures[i].LoadContent();
						await loader.LoadLoopBINAsync();
						if (texList.Textures[i].Content != null && texList.Textures[i].Info.Type != TEX_File.TexFileType.RawPal) {
							if (texList.Textures[i].Content.Width != texList.Textures[i].Info.Width ||
								texList.Textures[i].Content.Height != texList.Textures[i].Info.Height ||
								texList.Textures[i].Content.Color != texList.Textures[i].Info.Color) {
								throw new Exception($"Info & Content width/height mismatch for texture with key {texList.Textures[i].Key}");
							}
						}
					}
					Controller.DetailedState = $"Loading textures: CubeMaps";
					for (int i = 0; i < (texList.CubeMaps?.Count ?? 0); i++) {
						texList.CubeMaps[i].Load();
						await loader.LoadLoopBINAsync();
					}
					Controller.DetailedState = $"Loading textures: End";
					texList.FillInReferences();
				});
				await loader.LoadLoop(context.Deserializer);
			}

			if (loadFlags.HasFlag(LoadFlags.TextNoSound) && context.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montpellier)) {
				Controller.DetailedState = $"Loading text";
				for (int languageID = 0; languageID < 32; languageID++) {
					var binKey = new Jade_Key(context, worldKey.GetBinary(Jade_Key.KeyType.TextNoSound, languageID: languageID));
					if(!loader.FileInfos.ContainsKey(binKey)) continue;
					loader.BeginSpeedMode(binKey, serializeAction: async s => {
						Controller.DetailedState = $"Loading text: Language {languageID} - No sound";
						for (int i = 0; i < (worldList?.Value?.Worlds?.Length ?? 0); i++) {
							var w = worldList?.Value?.Worlds[i]?.Value;
							if (w != null) {
								var world = (WOR_World)w;
								world.Text?.LoadText(languageID);
								await loader.LoadLoopBINAsync();
								if (world.Text?.GetTextForLanguage(languageID) != null) {
									var text = world.Text.GetTextForLanguage(languageID);
									foreach (var txg in text.Text) {
										txg.Resolve(flags: LOA_Loader.ReferenceFlags.DontCache | LOA_Loader.ReferenceFlags.DontUseCachedFile | LOA_Loader.ReferenceFlags.Log);
										await loader.LoadLoopBINAsync();
										var group = (TEXT_TextGroup)txg.Value;
										if(group == null) continue;
										var usedRef = group.GetUsedReference(languageID);
										usedRef?.Resolve(onPreSerialize: (_, txl) => {
											((TEXT_TextList)txl).HasSound = false;
										},
										cache: LOA_Loader.CacheType.Text);
										await loader.LoadLoopBINAsync();
									}
								}
							}
						}
						//await loader.LoadLoopBINAsync();
					});
					await loader.LoadLoop(context.Deserializer);
					loader.Caches[LOA_Loader.CacheType.Text]?.Clear();
				}
			}
			loader.EndSpeedMode();
			loader.IsLoadingFix = false;

			return worldList;
		}

		public async UniTask<Jade_GenericReference> LoadWorld(Context context, Jade_Key worldKey, bool isFix = false, bool isEditor = false) {
			LOA_Loader loader = context.GetStoredObject<LOA_Loader>(LoaderKey);
			loader.IsLoadingFix = isFix;

			// Set up texture list
			TEX_GlobalList texList = new TEX_GlobalList();
			context.StoreObject<TEX_GlobalList>(TextureListKey, texList);
			loader.Caches[LOA_Loader.CacheType.TextureInfo].Clear();
			loader.Caches[LOA_Loader.CacheType.TextureContent].Clear();

			Jade_GenericReference world = new Jade_GenericReference(context, worldKey, new Jade_FileType() { Extension = ".wow" });
			if (isEditor && context.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montpellier)) {
				var s = context.Deserializer;
				world.Resolve();
				await loader.LoadLoop(s);
				if (world?.Value != null && world.Value is WOR_World w) {
					Controller.DetailedState = $"Loading world: {w.Name}";
					await w.JustAfterLoad(s, false);
				}
				await loader.LoadLoop(context.Deserializer);
			} else {
				await Jade_Montreal_BaseManager.LoadWorld_Montreal(context.Deserializer, world, 0, 1, isEditor);
			}

			loader.IsLoadingFix = false;

			return world;
		}

		// Load
		public override async UniTask<Unity_Level> LoadAsync(Context context) 
        {
			var stopWatch = new Stopwatch();
			stopWatch.Start();

			var loader = await LoadJadeAsync(context, new Jade_Key(context, (uint)context.GetR1Settings().Level), LoadFlags.All);

			stopWatch.Stop();

			Debug.Log($"Loaded BINs in {stopWatch.ElapsedMilliseconds}ms");

			throw new NotImplementedException("BINs serialized. Time to do something with this data :)");
		}

		public async UniTask<Jade_Reference<AI_Instance>> LoadUniverse(Context context) {
			LOA_Loader loader = context.GetStoredObject<LOA_Loader>(LoaderKey);

			Controller.DetailedState = $"Loading universe";
			await Controller.WaitIfNecessary();

			Jade_Reference<AI_Instance> univers = new Jade_Reference<AI_Instance>(context, loader.BigFiles[0].UniverseKey);
			if (context.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal)) {
				Jade_Reference<Jade_BinTerminator> terminator = new Jade_Reference<Jade_BinTerminator>(context, new Jade_Key(context, 0)) { ForceResolve = true };
				loader.BeginSpeedMode(univers.Key, async s => { // Univers is bin compressed in Montreal version
					univers.Resolve();
					await loader.LoadLoopBINAsync();
					if (!s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PhoenixRayman4)) {
						terminator.Resolve();
						await loader.LoadLoopBINAsync();
					}
				});
				await loader.LoadLoop(context.Deserializer); // First resolve universe
				loader.EndSpeedMode();
			} else {
				univers.Resolve();
				await loader.LoadLoop(context.Deserializer); // First resolve universe
			}
			// Make export
			{
				string worldName = "Univers";
				string name = "univers";
				univers?.Value?.Vars?.Value?.ExportVarsOverview(worldName, $"{name}_instance");
				univers?.Value?.Vars?.Value?.ExportIDAStruct(worldName, $"{name}_instance");
				univers?.Value?.Model?.Value?.Vars?.ExportVarsOverview(worldName, $"{name}_model");
				univers?.Value?.Model?.Value?.Vars?.ExportIDAStruct(worldName, $"{name}_model");
			}

			return univers;
		}

		public async UniTask LoadFix(Context context, Jade_Key worldKey, LoadFlags loadFlags) {
			if (FixWorlds != null && LevelInfos != null) {
				var levelInfos = LevelInfos;
				foreach (var world in FixWorlds) {
					var fixInfo = levelInfos.FindItem(li => li.MapName.Contains(world));
					if (fixInfo != null) {
						Jade_Key fixKey = new Jade_Key(context, fixInfo.Key);
						if (fixKey == worldKey) {
							UnityEngine.Debug.LogWarning($"Loading fix world with name {world} as a regular map world");
							break;
						}
						var fixWorldList = await LoadWorldList(context, fixKey, loadFlags, isFix: true);
					} else {
						UnityEngine.Debug.LogWarning($"Fix world with name {world} could not be found");
					}
				}
			}
		}

		public async UniTask<LOA_Loader> LoadJadeAsync(Context context, Jade_Key worldKey, LoadFlags loadFlags) 
        {
            List<BIG_BigFile> bfs = new List<BIG_BigFile>();
			foreach (var bfPath in BFFiles) {
				var bf = await LoadBF(context, bfPath);
				bfs.Add(bf);
			}
			// Set up loader
			LOA_Loader loader = new LOA_Loader(bfs.ToArray(), context);
			context.StoreObject<LOA_Loader>(LoaderKey, loader);

			// Load jade.spe
			LoadJadeSPE(context);

			// Create level list if null
			var levelInfos = LevelInfos;
			bool isWOW = false;
			bool isEditor = false;
			if (levelInfos == null) {
				throw new Exception($"Before loading, add the level list using the Create Level List game action.");
			} else {
				var levInfos = levelInfos.Where(l => l.Key == worldKey.Key);
				if (levInfos.Count() > 1) {
					var levels = levelInfos.GroupBy(x => x.WorldName).ToArray();
					levInfos = levels[context.GetR1Settings().World].Where(l => l.Key == worldKey.Key);
				}
				var levInfo = levInfos.FirstOrDefault();
				isWOW = levInfo != null && (levInfo.Type.HasValue && levInfo.Type.Value.HasFlag(LevelInfo.FileType.WOW));
				isEditor = levInfo != null && (levInfo.Type.HasValue && levInfo.Type.Value.HasFlag(LevelInfo.FileType.Unbinarized));
			}

			// Set up AI types
			AI_Links aiLinks = AI_Links.GetAILinks(context.GetR1Settings());
			context.StoreObject<AI_Links>(AIKey, aiLinks);

			if (loadFlags.HasFlag(LoadFlags.Universe)) {
				// Load universe
				var univers = await LoadUniverse(context);
			}

			if (loadFlags.HasFlag(LoadFlags.Maps) || loadFlags.HasFlag(LoadFlags.Textures) || loadFlags.HasFlag(LoadFlags.TextNoSound)) {
				// Load world
				Controller.DetailedState = $"Loading worlds";
				await Controller.WaitIfNecessary();

				if (!isEditor) await LoadFix(context, worldKey, loadFlags);
				if (isWOW) {
					var world = await LoadWorld(context, worldKey, isEditor: isEditor);
				} else {
					var worldList = await LoadWorldList(context, worldKey, loadFlags, isEditor: isEditor);
				}
			}

			return loader;
        }

        public override async UniTask LoadFilesAsync(Context context) {
			foreach (var bfPath in BFFiles) {
				await context.AddLinearSerializedFileAsync(bfPath, bigFileCacheLength: 8);
			}
			if (JadeSpePath != null) await context.AddLinearSerializedFileAsync(JadeSpePath);
			if (TexturesGearBFPath != null) await context.AddLinearSerializedFileAsync(TexturesGearBFPath, bigFileCacheLength: 8);
			if (SoundGearBFPath != null) await context.AddLinearSerializedFileAsync(SoundGearBFPath, bigFileCacheLength: 8);
		}

		// Constants
		public static readonly Encoding Encoding = Encoding.GetEncoding(1252);
		public const string LoaderKey = "loader";
		public const string TextureListKey = "textureList";
		public const string AIKey = "ai";

		public class LevelInfo
        {
            public LevelInfo(uint key, string directoryPath, string filePath, string worldName = null, string mapName = null, FileType? type = null)
            {
				OriginalMapName = mapName;
				OriginalWorldName = worldName;
				OriginalType = type;
                Key = key;
                DirectoryPath = directoryPath;
                FilePath = filePath;
				MapName = mapName ?? Path.GetFileNameWithoutExtension(FilePath);
				WorldName = worldName ?? (DirectoryPath.Contains('/') ? DirectoryPath.Substring(0, DirectoryPath.LastIndexOf('/')) : DirectoryPath);
				Type = type;
            }

			[Flags]
			public enum FileType {
				WOL = 1 << 0,
				WOW = 1 << 1,
				Unbinarized = 1 << 2,
				WOLUnbinarized = WOL | Unbinarized,
				WOWUnbinarized = WOW | Unbinarized,
			}

			public string OriginalWorldName { get; }
			public string OriginalMapName { get; }
			public FileType? OriginalType { get; }

			public FileType? Type { get; }
            public uint Key { get; }
			public string DirectoryPath { get; }
			public string FilePath { get; }

			public string WorldName { get; }
			public string MapName { get; }
		}

		[Flags]
		public enum LoadFlags : uint {
			None = 0,
			Universe = 1 << 0,
			Maps = 1 << 1,
			Textures = 1 << 2,
			TextNoSound = 1 << 3,
			TextSound = 1 << 4,
			Sounds = 1 << 5,
			All = (uint)0xFFFFFFFF
		}
    }
}
