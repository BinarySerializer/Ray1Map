using BinarySerializer;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Ray1Map.Jade;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Ray1Map {
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
		public virtual bool CanBeModded => false;

		// Game actions
		public override GameAction[] GetGameActions(GameSettings settings) {
			GameAction[] actions = new GameAction[] {
				new GameAction("Extract BF file(s)", false, true, (input, output) => ExtractFilesAsync(settings, output, false, true, true)),
				new GameAction("Extract BF file(s) - BIN decompression", false, true, (input, output) => ExtractFilesAsync(settings, output, true, true, true)),
				new GameAction("Create level list", false, false, (input, output) => CreateLevelListAsync(settings)),
				new GameAction("Export localization", false, true, (input, output) => ExportLocalizationAsync(settings, output, false)),
				new GameAction("Export textures", false, true, (input, output) => ExportTexturesAsync(settings, output, true)),
				new GameAction("Export models", false, true, (input, output) => ExportModelsAsync(settings, output)),
				new GameAction("Export sounds", false, true, (input, output) => ExportSoundsAsync(settings, output, true)),
				new GameAction("Export unbinarized assets", false, true, (input, output) => ExportUnbinarizedAsync(settings, null, output, true, false)),
				new GameAction("Export unbinarized into RRR format", true, true, (input, output) => ExportUnbinarizedAsync(settings, input, output, true, true)),
				new GameAction("Create new BF using unbinarized files", true, true, (input, output) => CreateBFAsync(settings, input, output, true)),
			};
			if (CanBeModded) {
				actions = actions.Concat(new GameAction[] {
					new GameAction("Add modded GameObjects", true, true, (input, output) => AddModdedGameObjects(settings, input, output)),
				}).ToArray();
			}
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

		#region Extract assets
		public async UniTask ExtractFilesAsync(GameSettings settings, string outputDir, bool decompressBIN = false, bool exportKeyList = false, bool exportTimeline = false) {
            using (var context = new Ray1MapContext(settings)) {
				var s = context.Deserializer;
                await LoadFilesAsync(context);
				Dictionary<uint, string> fileKeys = new Dictionary<uint, string>();
				List<KeyValuePair<DateTime, string>> timelineList = new List<KeyValuePair<DateTime, string>>();
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
										if(exportTimeline) timelineList.Add(new KeyValuePair<DateTime, string>(fi.DateLastModified, Path.Combine(directories[fi.ParentDirectory], fileName)));
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
				if (exportTimeline) {
					StringBuilder b = new StringBuilder();
					timelineList.Sort((x, y) => x.Key.CompareTo(y.Key));
					foreach (var kv in timelineList) {
						b.AppendLine($"{kv.Key:ddd, dd/MM/yyyy - HH:mm:ss}\t{kv.Value}");
					}
					File.WriteAllText(Path.Combine(outputDir, "timeline.txt"), b.ToString());
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
					using (var context = new Ray1MapContext(settings)) {
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
								if (allText?.Text == null) continue;
								foreach (var g in allText.Text) {
									if (g.IsNull || g.Value == null) continue;
									var group = (TEXT_TextGroup)g.Value;
									var usedRef = group?.GetUsedReference(langID);
									if(usedRef == null || usedRef.IsNull || usedRef.TextList == null) continue;
									var txl = usedRef.TextList;
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
				if (lev?.Type.HasValue ?? false) {
					if (lev.Type.Value.HasFlag(LevelInfo.FileType.WOL) || lev.Type.Value.HasFlag(LevelInfo.FileType.Unbinarized)) {
						levIndex++;
						continue;
					}
				}

				Debug.Log($"Exporting for level {levIndex++ + 1}/{LevelInfos.Length}: {lev.MapName}");

                try
                {
                    using (var context = new Ray1MapContext(settings)) {
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

									for (int i = 0; i < dds.Faces.Length; i++) {
										Util.ByteArrayToFile(Path.Combine(outputDir, "Cubemaps", $"{t.Key.Key:X8}_{i}.png"), dds.Faces[i].Surfaces[0].ToTexture2D().EncodeToPNG());
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
		public async UniTask ExportModelsAsync(GameSettings settings, string outputDir) {
			var parsedTexs = new HashSet<uint>();

			var levIndex = 0;
			uint currentKey = 0;
			HashSet<uint> exportedObjects = new HashSet<uint>();
			HashSet<string> exportedObjectIDs = new HashSet<string>();

			foreach (var lev in LevelInfos) {
				// If there are WOL files, there are also raw WOW files. It's better to process those one by one.
				if (lev?.Type.HasValue ?? false) {
					if (lev.Type.Value.HasFlag(LevelInfo.FileType.WOL) || lev.Type.Value.HasFlag(LevelInfo.FileType.Unbinarized)) {
						levIndex++;
						continue;
					}
				}

				Debug.Log($"Exporting for level {levIndex++ + 1}/{LevelInfos.Length}: {lev.MapName}");

				try {
					using (var context = new Ray1MapContext(settings)) {
						currentKey = 0;
						await LoadFilesAsync(context);
						await LoadJadeAsync(context, new Jade_Key(context, lev.Key), LoadFlags.Maps | LoadFlags.Textures);

						string worldName = null;

						LOA_Loader loader = context.GetStoredObject<LOA_Loader>(LoaderKey);
						foreach (var w in loader.LoadedWorlds) {
							worldName = w?.Name;
							var gaos = w.GameObjects?.Value?.GameObjects;
							if(gaos == null) continue;
							foreach (var o in gaos) {
								if(o.IsNull || exportedObjects.Contains(o.Key.Key)) continue;
								exportedObjects.Add(o.Key.Key);
								string objectID = "";
								if (o?.Value?.Base?.Visual != null) {
									var visu = o?.Value?.Base?.Visual;
									objectID += $"G{visu.GeometricObject.Key.Key:X8}-M{visu.Material.Key.Key:X8}";
								}
								if (o?.Value?.Base?.ActionData?.SkeletonGroup != null) {
									objectID += $"-S{o.Value.Base.ActionData.SkeletonGroup.Key.Key:X8}";
								}
								if (o?.Value?.Extended?.Group != null) {
									objectID += $"-Gr{o?.Value.Extended?.Group.Key.Key:X8}";
								}
								if(exportedObjectIDs.Contains(objectID)) continue;
								exportedObjectIDs.Add(objectID);
								if (o.Value != null) await FBXExporter.ExportFBXAsync(o.Value, outputDir);
							}
						}
					}
				} catch (Exception ex) {
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
		public async UniTask ExportSoundsAsync(GameSettings settings, string outputDir, bool useComplexNames) {
			var parsedSounds = new HashSet<uint>();

			var levIndex = 0;
			uint currentKey = 0;

			foreach (var lev in LevelInfos) {
				// If there are WOL files, there are also raw WOW files. It's better to process those one by one.
				if (lev?.Type.HasValue ?? false) {
					if (lev.Type.Value.HasFlag(LevelInfo.FileType.WOL) || lev.Type.Value.HasFlag(LevelInfo.FileType.Unbinarized)) {
						levIndex++;
						continue;
					}
				}

				Debug.Log($"Exporting for level {levIndex++ + 1}/{LevelInfos.Length}: {lev.MapName}");

				try {
					using (var context = new Ray1MapContext(settings)) {
						currentKey = 0;
						await LoadFilesAsync(context);
						await LoadJadeAsync(context, new Jade_Key(context, lev.Key), LoadFlags.Maps | LoadFlags.TextNoSound | LoadFlags.TextSound | LoadFlags.Sounds);

						SND_GlobalList sndList = context.GetStoredObject<SND_GlobalList>(SoundListKey);

						//string worldName = null;
						if (context.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal)) {
							throw new NotImplementedException("Not yet implemented for Montreal");
						} else {
							await ExportSndList(sndList);
						}

						async UniTask ExportSndList(SND_GlobalList sndList) {
							if (sndList.Waves != null && sndList.Waves.Any()) {
								Debug.Log($"Loaded level. Exporting {sndList?.Waves?.Count} sounds...");
								await Controller.WaitIfNecessary();

								LOA_Loader actualLoader = context.GetStoredObject<LOA_Loader>(LoaderKey);

								using (var writeContext = new Ray1MapContext(outputDir, settings)) {
									// Set up loader
									LOA_Loader loader = new LOA_Loader(actualLoader.BigFiles, writeContext);
									writeContext.StoreObject<LOA_Loader>(LoaderKey, loader);
									var sndListWrite = new SND_GlobalList();
									writeContext.StoreObject<SND_GlobalList>(SoundListKey, sndListWrite);

									foreach (var t in sndList.Waves) {
										if (parsedSounds.Contains(t.Key.Key))
											continue;

										parsedSounds.Add(t.Key.Key);

										Jade_Reference<SND_Wave> wave = new Jade_Reference<SND_Wave>(writeContext, t.Key) {
											Value = t
										};
										wave.Resolve();
									}
									var s = writeContext.Serializer;
									await loader.LoadLoop(s);
								}
							}
						}
					}
				} catch (Exception ex) {
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
		public async UniTask ExportUnbinarizedAsync(GameSettings settings, string inputDir, string outputDir, bool useComplexNames, bool exportForRRRPC) {
			var parsedSounds = new HashSet<uint>();

			var levIndex = 0;
			uint currentKey = 0;
			Dictionary<uint, string> writtenFileKeys = new Dictionary<uint, string>();

			// Key relocation (for writing as RRR mod)
			Dictionary<uint, uint> keysToRelocate = new Dictionary<uint, uint>();
			HashSet<uint> keysToAvoid = new HashSet<uint>();

			int[] rrrPC_supportedModifiers = new int[] { 
				-1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 18, 19,
				20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 36, 37, 38, 49,
				50, 51, 52, 53, 54, 55
			};

			if (inputDir != null) {
				DirectoryInfo dinfo = new DirectoryInfo(inputDir);
				var keyFiles = dinfo.GetFiles("*.txt", SearchOption.TopDirectoryOnly);
				foreach (var f in keyFiles) {
					string[] lines = File.ReadAllLines(f.FullName);
					foreach (var l in lines) {
						var lineSplit = l.Split(',');
						if (lineSplit.Length < 1) continue;
						uint k;
						if (uint.TryParse(lineSplit[0], System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.CurrentCulture, out k)) {
							if (!keysToAvoid.Contains(k)) keysToAvoid.Add(k);
						}
					}
				}
			}


			foreach (var lev in LevelInfos) {
				// If there are WOL files, there are also raw WOW files. It's better to process those one by one.
				if (lev?.Type.HasValue ?? false) {
					if (lev.Type.Value.HasFlag(LevelInfo.FileType.WOL)) {
						levIndex++;
						continue;
					}
				}
				/*if (!(lev?.MapName.Contains("Finger_Guess") ?? false)) {
					levIndex++;
					continue;
				}*/

				Debug.Log($"Exporting for level {levIndex++ + 1}/{LevelInfos.Length}: {lev.MapName}");

				try {
					using (var context = new Ray1MapContext(settings)) {
						currentKey = 0;
						await LoadFilesAsync(context);
						if (exportForRRRPC) {
							await LoadJadeAsync(context, new Jade_Key(context, lev.Key), LoadFlags.Maps | LoadFlags.Textures);
						} else {
							await LoadJadeAsync(context, new Jade_Key(context, lev.Key), LoadFlags.All);
						}

						LOA_Loader actualLoader = context.GetStoredObject<LOA_Loader>(LoaderKey);
						var worlds = actualLoader.LoadedWorlds;

						if (exportForRRRPC) {
							foreach (var w in worlds) {
								w.Text = new Jade_TextReference(context, new Jade_Key(context, 0xFFFFFFFF));
								foreach (var gao in w.SerializedGameObjects) {
									gao.FlagsIdentity &= ~OBJ_GameObject_IdentityFlags.Sound;
									gao.FlagsIdentity &= ~OBJ_GameObject_IdentityFlags.AI;

									if (gao.Extended?.Modifiers != null) {
										if (context.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal)) {
											foreach (var m in gao.Extended.Modifiers) {
												if ((int)m.Type_Montreal >= 15) {
													m.Type = (MDF_ModifierType)100; // Unsupported
												} else {
													m.Type = (MDF_ModifierType)(int)m.Type_Montreal;
												}
											}
										}
										gao.Extended.Modifiers = gao.Extended.Modifiers
											.Where(m =>
												m.Type != MDF_ModifierType.GEN_ModifierSound && m.Type != MDF_ModifierType.MDF_LoadingSound
												&& rrrPC_supportedModifiers.Contains((int)m.Type))
											.ToArray();
										if(gao.Extended.Modifiers.Length == 0
											|| (gao.Extended.Modifiers.Length == 1 && gao.Extended.Modifiers[0].Type == MDF_ModifierType.None)) gao.Extended.HasModifiers = 0;
									}

									if (!context.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_KingKong)) {
										if (gao?.Base?.Visual?.VertexColors != null) {
											gao.Base.Visual.VertexColors = gao.Base.Visual.VertexColors.Select(c => new Jade_Color(c.Blue, c.Green, c.Red, c.Alpha)).ToArray();
										}
									}
								}
							}
						}

						//string worldName = null;
						if (context.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal) && !exportForRRRPC) {
							throw new NotImplementedException("Not yet implemented for Montreal");
						} else {
							foreach (var w in worlds) {
								await ExportUnbinarized(w);
							}
							if(!exportForRRRPC) await ExportRest();
						}

						async UniTask ExportUnbinarized(WOR_World world) {
							if(world != null) {
								Debug.Log($"Loaded level. Exporting unbinarized assets...");
								await Controller.WaitIfNecessary();

								LOA_Loader actualLoader = context.GetStoredObject<LOA_Loader>(LoaderKey);

								var newSettings = new GameSettings(GameModeSelection.RaymanRavingRabbidsPC, settings.GameDirectory, settings.World, settings.Level);

								using (var writeContext = new Ray1MapContext(outputDir, newSettings)) {
									// Set up loader
									LOA_Loader loader = new LOA_Loader(actualLoader.BigFiles, writeContext);

									loader.Raw_WriteFilesAlreadyInBF = HasUnbinarizedData || exportForRRRPC;
									if (exportForRRRPC) {
										loader.Raw_RelocateKeys = true;
										loader.Raw_KeysToAvoid = keysToAvoid;
										loader.Raw_KeysToRelocate = keysToRelocate;
									}

									loader.WrittenFileKeys = writtenFileKeys;
									writeContext.StoreObject<LOA_Loader>(LoaderKey, loader);
									var sndListWrite = new SND_GlobalList();
									writeContext.StoreObject<SND_GlobalList>(SoundListKey, sndListWrite);
									var texListWrite = new TEX_GlobalList();
									writeContext.StoreObject<TEX_GlobalList>(TextureListKey, texListWrite);
									var aiLinks = context.GetStoredObject<AI_Links>(AIKey);
									writeContext.StoreObject<AI_Links>(AIKey, aiLinks);

									Jade_Reference<WOR_World> worldRef = new Jade_Reference<WOR_World>(writeContext, new Jade_Key(writeContext, world.Key.Key)) {
										Value = world
									};
									worldRef?.Resolve();
									var s = writeContext.Serializer;
									await loader.LoadLoop(s);
								}
							}
						}


						async UniTask ExportRest() {
							Debug.Log($"Checking for unexported unbinarized assets...");
							await Controller.WaitIfNecessary();

							LOA_Loader actualLoader = context.GetStoredObject<LOA_Loader>(LoaderKey);

							var newSettings = new GameSettings(GameModeSelection.RaymanRavingRabbidsPC, settings.GameDirectory, settings.World, settings.Level);

							using (var writeContext = new Ray1MapContext(outputDir, newSettings)) {
								// Set up loader
								LOA_Loader loader = new LOA_Loader(actualLoader.BigFiles, writeContext);

								loader.Raw_WriteFilesAlreadyInBF = HasUnbinarizedData || exportForRRRPC;
								if (exportForRRRPC) {
									loader.Raw_RelocateKeys = true;
									loader.Raw_KeysToAvoid = keysToAvoid;
									loader.Raw_KeysToRelocate = keysToRelocate;
								}

								loader.WrittenFileKeys = writtenFileKeys;
								writeContext.StoreObject<LOA_Loader>(LoaderKey, loader);
								var sndListWrite = new SND_GlobalList();
								writeContext.StoreObject<SND_GlobalList>(SoundListKey, sndListWrite);
								var texListWrite = new TEX_GlobalList();
								writeContext.StoreObject<TEX_GlobalList>(TextureListKey, texListWrite);
								var aiLinks = context.GetStoredObject<AI_Links>(AIKey);
								writeContext.StoreObject<AI_Links>(AIKey, aiLinks);
									
								bool foundUnserializedElement = false;
								foreach (var containedInTotalCache in actualLoader.TotalCache) {
									if (!writtenFileKeys.ContainsKey(containedInTotalCache.Key) && !loader.FileInfos.ContainsKey(containedInTotalCache.Key)) {
										foundUnserializedElement = true;
										var element = containedInTotalCache.Value;
										switch (element) {
											case TEXT_TextList txl:
												Jade_Reference<TEXT_TextList> txlRef = new Jade_Reference<TEXT_TextList>(writeContext, new Jade_Key(writeContext, txl.Key.Key)) {
													Value = txl
												};
												txlRef?.Resolve();
												break;
											default:
												throw new Exception($"New unserialized element type: {element.GetType()}");
										}

									}
								}
								if (foundUnserializedElement) {
									var s = writeContext.Serializer;
									await loader.LoadLoop(s);
								}
							}
						}
					}
				} catch (Exception ex) {
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

			StringBuilder b = new StringBuilder();
			foreach (var fk in writtenFileKeys) {
				b.AppendLine($"{fk.Key:X8},{fk.Value}");
			}
			File.WriteAllText(Path.Combine(outputDir, "filekeys.txt"), b.ToString());

			Debug.Log($"Finished export");
		}
		public async UniTask ExportTexturesUnbinarized(GameSettings settings, string outputDir) {
			using (var context = new Ray1MapContext(settings)) {
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

				// Set up sound list
				SND_GlobalList sndList = new SND_GlobalList();
				context.StoreObject<SND_GlobalList>(SoundListKey, sndList);

				foreach (var kvp in loader.FileInfos) {
					var fileInfo = kvp.Value;
					if (fileInfo.FileName != null && (fileInfo.FileName.EndsWith(".tex") || fileInfo.FileName.EndsWith(".jtx"))) {
						try {
							Jade_TextureReference texRef = new Jade_TextureReference(context, fileInfo.Key);
							texRef.Resolve();
							await loader.LoadLoop(context.Deserializer);

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
		#endregion

		#region Gear BF
		public async UniTask ExportGearBF(GameSettings settings, string bfPath, string outputDir, string extension) {
			if (bfPath == null) return;
			using (var context = new Ray1MapContext(settings)) {
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
			using (var context = new Ray1MapContext(settings)) {
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
		#endregion

		public async UniTask CreateBFAsync(GameSettings settings, string inputDir, string outputDir, bool keepAllUnbinarizedFiles) {
			BIG_BigFile originalBF = null;
			LOA_Loader originalLoader = null;
			BIG_BigFile.FileInfoForCreate[] FilesToPack = null;
			BIG_BigFile.DirectoryInfoForCreate[] DirectoriesToPack = null;
			using (var readContext = new Ray1MapContext(settings)) {
				await LoadFilesAsync(readContext);
				originalLoader = await InitJadeAsync(readContext, initAI: false);
				originalBF = originalLoader.BigFiles[0];

				// Read file keys (unbinarized)
				string keyListPath = Path.Combine(inputDir, "original/filekeys.txt");
				Dictionary<uint, string> fileKeysUnbinarized = new Dictionary<uint, string>();
				{
					string[] lines = File.ReadAllLines(keyListPath);
					foreach (var l in lines) {
						var lineSplit = l.Split(',');
						if (lineSplit.Length != 2) continue;
						uint k;
						if (uint.TryParse(lineSplit[0], System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.CurrentCulture, out k)) {
							var path = lineSplit[1].Replace('\\', '/');
							var absolutePath = Path.Combine(inputDir, $"original/files/{path}");
							if (File.Exists(absolutePath)) {
								fileKeysUnbinarized[k] = path;
							}
						}
					}
				}

				// Get file keys (existing in BF)
				Dictionary<uint, string> fileKeysExisting = new Dictionary<uint, string>();
				if (keepAllUnbinarizedFiles) {
					foreach (var fi in originalLoader.FileInfos) {
						if (fi.Key.Type == Jade_Key.KeyType.Unknown) {
							fileKeysExisting.Add(fi.Key.Key, fi.Value.FilePath);
							if (fileKeysUnbinarized.ContainsKey(fi.Key.Key)) fileKeysUnbinarized.Remove(fi.Key.Key);
						}
					}
				}

				// Read file keys (modded)
				List<KeyValuePair<string, Dictionary<uint, string>>> mods = new List<KeyValuePair<string, Dictionary<uint, string>>>();
				var modDirectory = Path.Combine(inputDir, "mod");
				foreach (var modDir in Directory.GetDirectories(modDirectory).OrderBy(dirName => dirName)) {
					string configPath = Path.Combine(modDir, "config.txt");
					bool overwrite = true;
					if (File.Exists(configPath)) {
						string[] lines = File.ReadAllLines(configPath);
						foreach (var l in lines) {
							if (l.Trim() == "overwrite=false") {
								overwrite = false;
							}
						}
					}
					string modKeyListPath = Path.Combine(modDir, "filekeys.txt");
					if (File.Exists(modKeyListPath)) {
						var mod = new KeyValuePair<string, Dictionary<uint, string>>(modDir, new Dictionary<uint, string>());
						mods.Add(mod);
						string[] lines = File.ReadAllLines(modKeyListPath);
						foreach (var l in lines) {
							var lineSplit = l.Split(',');
							if (lineSplit.Length != 2) continue;
							uint k;
							if (uint.TryParse(lineSplit[0], System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.CurrentCulture, out k)) {
								var path = lineSplit[1].Replace('\\', '/');
								var absolutePath = Path.Combine(modDir, $"files/{path}");
								if (File.Exists(absolutePath)) {
									bool addModFile = true;
									if (fileKeysUnbinarized.ContainsKey(k)) {
										if(overwrite) fileKeysUnbinarized.Remove(k);
										else addModFile = false;
									}
									if (fileKeysExisting.ContainsKey(k)) {
										if (overwrite) fileKeysExisting.Remove(k);
										else addModFile = false;
									}
									foreach (var otherMod in mods) {
										if (otherMod.Key != mod.Key && otherMod.Value.ContainsKey(k)) {
											if(overwrite) otherMod.Value.Remove(k);
											else addModFile = false;
										}
									}
									if(addModFile) mod.Value[k] = path;
								}
							}
						}
					}
				}

				// Read all files
				FilesToPack =
					fileKeysUnbinarized.Select(fk => new BIG_BigFile.FileInfoForCreate() {
						FullPath = fk.Value ?? $"{fk.Key:X8}",
						Key = new Jade_Key(readContext, fk.Key),
						Source = BIG_BigFile.FileInfoForCreate.FileSource.Unbinarized,
						DirectoryIndex = -1,
					}).Concat(
					fileKeysExisting.Select(fk => new BIG_BigFile.FileInfoForCreate() {
						FullPath = fk.Value ?? $"{fk.Key:X8}",
						Key = new Jade_Key(readContext, fk.Key),
						Source = BIG_BigFile.FileInfoForCreate.FileSource.Existing,
						DirectoryIndex = -1,
					})).ToArray();

				foreach (var mod in mods) {
					FilesToPack = FilesToPack.Concat(mod.Value.Select(fk => new BIG_BigFile.FileInfoForCreate() {
						FullPath = fk.Value ?? $"{fk.Key:X8}",
						Key = new Jade_Key(readContext, fk.Key),
						Source = BIG_BigFile.FileInfoForCreate.FileSource.Mod,
						ModDirectory = mod.Key,
						DirectoryIndex = -1,
					})
					).ToArray();
				}

				// Create directories
				List<BIG_BigFile.DirectoryInfoForCreate> directories = new List<BIG_BigFile.DirectoryInfoForCreate>();
				int curDirectoriesCount = 0;
				foreach (var fileToPack in FilesToPack) {
					var originalFullPath = fileToPack.FullPath;
					var currentFullPath = fileToPack.FullPath;
					Stack<string> pathStrings = new Stack<string>();
					int currentPathIndexInOriginalFullPath = 0;
					while (!string.IsNullOrEmpty(currentFullPath) && currentFullPath.Contains('/')) {
						var firstIndex = currentFullPath.IndexOf('/');
						var firstFolder = currentFullPath.Substring(0, firstIndex);
						var dirIndex = directories.FindItemIndex(dir => dir.ParentIndex == fileToPack.DirectoryIndex);
						if (dirIndex == -1) {
							directories.Add(new BIG_BigFile.DirectoryInfoForCreate() {
								DirectoryName = firstFolder,
								FullDirectoryString = originalFullPath.Substring(0, currentPathIndexInOriginalFullPath + firstIndex),
								ParentIndex = fileToPack.DirectoryIndex,
							});
							fileToPack.DirectoryIndex = curDirectoriesCount;
							curDirectoriesCount++;
						} else {
							fileToPack.DirectoryIndex = dirIndex;
						}
						currentFullPath = currentFullPath.Substring(firstIndex + 1, currentFullPath.Length - firstIndex - 1);
					}
					fileToPack.Filename = currentFullPath;
				}
				DirectoriesToPack = directories.ToArray();

				foreach (var file in FilesToPack) {
					switch (file.Source) {
						case BIG_BigFile.FileInfoForCreate.FileSource.Unbinarized:
							file.Bytes = File.ReadAllBytes(Path.Combine(inputDir, $"original/files/{file.FullPath}"));
							break;
						case BIG_BigFile.FileInfoForCreate.FileSource.Mod:
							file.Bytes = File.ReadAllBytes(Path.Combine(file.ModDirectory, $"files/{file.FullPath}"));
							break;
						case BIG_BigFile.FileInfoForCreate.FileSource.Existing:
							var s = readContext.Deserializer;
							var reference = new Jade_Reference<Jade_ByteArrayFile>(readContext, file.Key);
							reference.Resolve(flags: LOA_Loader.ReferenceFlags.DontCache | LOA_Loader.ReferenceFlags.DontUseCachedFile);
							await originalLoader.LoadLoop(s);
							file.Bytes = reference.Value.Bytes;

							/*if (file.Bytes.Length >= 0x3966C0 - 2
								&& file.Bytes[0] == 0x52 && file.Bytes[1] == 0x49 && file.Bytes[2] == 0x46 && file.Bytes[3] == 0x46 // RIFF
								&& file.Bytes[8] == 0x57 && file.Bytes[9] == 0x41 && file.Bytes[10] == 0x56 && file.Bytes[11] == 0x45) { // WAVE

								// TODO: Keep the music files intact, but just replace them now
								if (loader.FileInfos[file.Key].FileName != null) {
									var extension = Path.GetExtension(loader.FileInfos[file.Key].FileName);
									var smaller = loader.FileInfos.FirstOrDefault(smollerfile => smollerfile.Value.FileName != null
									&& Path.GetExtension(smollerfile.Value.FileName) == extension
									&& smollerfile.Value.FatFileInfo.FileSize > 8
									&& smollerfile.Value.FatFileInfo.FileSize < 0x3966C0 - 0x1000);
									reference = new Jade_Reference<Jade_ByteArrayFile>(readContext, smaller.Key);
									reference.Resolve();
									await loader.LoadLoop(s);
									file.Bytes = reference.Value.Bytes;
								}
							}*/

							/*if (file.Bytes.Length >= 12
								&& file.Bytes[0] == 0x52 && file.Bytes[1] == 0x49 && file.Bytes[2] == 0x46 && file.Bytes[3] == 0x46 // RIFF
								&& file.Bytes[8] == 0x57 && file.Bytes[9] == 0x41 && file.Bytes[10] == 0x56 && file.Bytes[11] == 0x45) { // WAVE
								var actualSize = file.Bytes[4] | file.Bytes[5] << 8 | file.Bytes[6] << 16 | file.Bytes[7] << 24;
								var bytes = file.Bytes;
								Array.Resize<byte>(ref bytes, actualSize + 8);
								file.Bytes = bytes;
							}*/

							/*if (loader.FileInfos.ContainsKey(file.Key)) {
								if (loader.FileInfos[file.Key].FatFileInfo != null && loader.FileInfos[file.Key].FatFileInfo.Name != null) {
									var fileSize = loader.FileInfos[file.Key].FatFileInfo.FileSize;
									if (fileSize != 0 && file.Bytes.Length > fileSize) {
										var bytes = file.Bytes;
										Array.Resize<byte>(ref bytes, (int)fileSize);
										file.Bytes = bytes;
									}

								}
							}*/
							break;
					}
					file.FileSize = (uint)file.Bytes.Length;
				}
			}

			using (Context writeContext = new Ray1MapContext(outputDir, settings)) {
				var targetFilePath = "out.bf";
				var bfFile = new LinearFile(writeContext, targetFilePath, Endian.Little);
				writeContext.AddFile(bfFile);

				BIG_BigFile newBF = BIG_BigFile.Create(originalBF, bfFile.StartPointer, FilesToPack, DirectoriesToPack);
				var s = writeContext.Serializer;
				s.Goto(bfFile.StartPointer);
				newBF = s.SerializeObject<BIG_BigFile>(newBF, name: nameof(newBF));
				newBF.SerializeFatFiles(s);

				foreach (var file in FilesToPack) {
					s.Goto(file.Offset);
					s.Serialize<uint>(file.FileSize, name: nameof(file.FileSize));
					s.SerializeArray<byte>(file.Bytes, file.Bytes.Length, name: nameof(file.Bytes));
					s.Goto(file.NameOffset);
					s.SerializeString($"#{file.Filename}#", encoding: Jade_BaseManager.Encoding, name: nameof(file.Filename));
				}
			}
		}

		public class ModdedGameObject {
			public uint PrefabKey { get; set; }
			public Jade_Reference<OBJ_GameObject> ReadReference { get; set; }
			public string Name { get; set; }
			public Transform Transform { get; set; }
			public uint NewKey { get; set; }
			public bool CreatePrefab { get; set; }
			public Jade_Reference<OBJ_GameObject> WriteReference { get; set; }
		}
		public async UniTask AddModdedGameObjects(GameSettings settings, string inputDir, string outputDir) {
			// Input: Keys to avoid
			// Output: folder where raw files will be saved
			var modBehaviour = GameObject.FindObjectOfType<JadeModBehaviour>();
			var customModels = GameObject.FindObjectsOfType<JadeModelImportBehaviour>();
			if (modBehaviour != null || customModels.Length > 0) {
				List<ModdedGameObject> ModdedGameObjects = new List<ModdedGameObject>();
				Jade_Reference<WOR_World> ModWorld = null;
				string ModWorldName = null;
				var moddedObjCount = modBehaviour?.gameObject.transform.childCount ?? 0;

				if (moddedObjCount > 0 || customModels.Length > 0) {
					bool saveModWorld = false;
					LOA_Loader readLoader = null;

					using (var readContext = new Ray1MapContext(settings)) {
						await LoadFilesAsync(readContext);
						readLoader = await InitJadeAsync(readContext, initAI: true, initTextures: true, initSound: true);

						for (int moddedObjIndex = 0; moddedObjIndex < moddedObjCount; moddedObjIndex++) {
							var transform = modBehaviour.gameObject.transform.GetChild(moddedObjIndex);
							var transformName = transform.gameObject.name;
							bool createPrefab = false;
							Debug.Log($"Processing GameObject: {transformName}");
							string parsedName = null;

							// Parse name to get key + desired GameObject name
							const string GaoNamePattern = @"^(?<prefab>Prefab - )?\[(?<key>........)\] (?<name>.*)$";
							Match m = Regex.Match(transformName, GaoNamePattern, RegexOptions.IgnoreCase);
							uint key = 0;
							if (m.Success) {
								string keyString = m.Groups["key"].Value;
								parsedName = m.Groups["name"].Value;
								createPrefab = m.Groups["prefab"].Success;
								UInt32.TryParse(keyString,
									System.Globalization.NumberStyles.HexNumber,
									System.Globalization.CultureInfo.CurrentCulture,
									out key);
							} else {
								Debug.Log("GameObject name regex failed!");
							}
							if (key != 0) {
								var file = new Jade_Reference<OBJ_GameObject>(readContext, new Jade_Key(readContext, key));
								file.Resolve();
								// Save transform & name & file in struct for later
								ModdedGameObjects.Add(new ModdedGameObject() {
									ReadReference = file,
									Name = parsedName,
									PrefabKey = key,
									Transform = transform,
									CreatePrefab = createPrefab
								});
								if(!createPrefab) saveModWorld = true;
							}
						}
						if (saveModWorld) {
							const string WorldNamePattern = @"^\[(?<key>........)\] (?<name>.*)$";
							Match m = Regex.Match(modBehaviour.gameObject.name, WorldNamePattern, RegexOptions.IgnoreCase);
							uint key = 0;
							if (m.Success) {
								string keyString = m.Groups["key"].Value;
								ModWorldName = m.Groups["name"].Value;
								UInt32.TryParse(keyString,
									System.Globalization.NumberStyles.HexNumber,
									System.Globalization.CultureInfo.CurrentCulture,
									out key);
							} else {
								Debug.Log("GameObject name regex failed!");
							}
							if (key != 0) {
								ModWorld = new Jade_Reference<WOR_World>(readContext, new Jade_Key(readContext, key));
								ModWorld.Resolve();
							}
						}
						await readLoader.LoadLoop(readContext.Deserializer);
					}

					if(ModdedGameObjects.Count == 0 && customModels.Length == 0) return;

					// Read keys to avoid
					HashSet<uint> keysToAvoid = new HashSet<uint>();
					if (inputDir != null) {
						DirectoryInfo dinfo = new DirectoryInfo(inputDir);
						var keyFiles = dinfo.GetFiles("*.txt", SearchOption.TopDirectoryOnly);
						foreach (var f in keyFiles) {
							string[] lines = File.ReadAllLines(f.FullName);
							foreach (var l in lines) {
								var lineSplit = l.Split(',');
								if (lineSplit.Length < 1) continue;
								uint k;
								if (uint.TryParse(lineSplit[0], System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.CurrentCulture, out k)) {
									if (!keysToAvoid.Contains(k)) keysToAvoid.Add(k);
								}
							}
						}
					}
					Dictionary<uint, string> writtenFileKeys = new Dictionary<uint, string>();

					Debug.Log("Opening write context");
					// Open write context with outputDir and configure KeysToAvoid. Make sure it doesn't write files already in the BF.
					using (var writeContext = new Ray1MapContext(outputDir, settings)) {
						// Set up loader
						LOA_Loader loader = new LOA_Loader(readLoader.BigFiles, writeContext);
						loader.Raw_RelocateKeys = false; // Don't relocate keys by default. We'll determine which ones to relocate and which to keep
						loader.Raw_KeysToAvoid = keysToAvoid;
						loader.Raw_WriteFilesAlreadyInBF = false;
						uint currentUnusedKeyInstance = 0x88000000 - 1;
						uint currentUnusedKeyPrefab   = 0x11000000 - 1;
						loader.Raw_CurrentUnusedKey = currentUnusedKeyInstance; // Start key will be this one
						loader.WrittenFileKeys = writtenFileKeys;

						writeContext.StoreObject<LOA_Loader>(LoaderKey, loader);
						var sndListWrite = new SND_GlobalList();
						writeContext.StoreObject<SND_GlobalList>(SoundListKey, sndListWrite);
						var texListWrite = new TEX_GlobalList();
						writeContext.StoreObject<TEX_GlobalList>(TextureListKey, texListWrite);
						var aiLinks = readLoader.Context.GetStoredObject<AI_Links>(AIKey);
						writeContext.StoreObject<AI_Links>(AIKey, aiLinks);
						// Process modded objects
						Jade_Key newKey() => new Jade_Key(writeContext, loader.Raw_RelocateKey(loader.Raw_CurrentUnusedKey));
						foreach (var moddedObject in ModdedGameObjects) {
							if (moddedObject.CreatePrefab) {
								loader.Raw_CurrentUnusedKey = currentUnusedKeyPrefab;
							} else {
								loader.Raw_CurrentUnusedKey = currentUnusedKeyInstance;
							}
							// Apply new transform, set name and key
							var obj = moddedObject.ReadReference.Value;
							var transform = moddedObject.Transform;
							obj.Matrix.SetTRS(
								transform.localPosition,
								transform.localRotation,
								transform.localScale,
								newType: TypeFlags.Translation | TypeFlags.Rotation | TypeFlags.Scale,
								convertAxes: true);
							obj.Name = $"{(moddedObject.CreatePrefab ? "PREFAB_" : "")}{moddedObject.Name}.gao";
							obj.NameLength = (uint)obj.Name.Length + 1;
							obj.Key = newKey();

							// Rewrite AI. Two objects with the same AI instance & same instance vars buffer cannot exist.
							if (obj.Extended?.AI?.Value != null) {
								var ai = obj.Extended.AI;
								ai.Value.Key = newKey();
								ai.Key = ai.Value.Key;
								if (ai.Value.Vars?.Value != null) {
									ai.Value.Vars.Value.Key = newKey();
									ai.Value.Vars.Key = ai.Value.Vars.Value.Key;
								}
							}
							if (!moddedObject.CreatePrefab) {
								// Same for COL Instance & COLMap
								if (obj.COL_Instance?.Value != null) {
									var cin = obj.COL_Instance;
									cin.Value.Key = newKey();
									cin.Key = cin.Value.Key;
								}
								if (obj.COL_ColMap?.Value != null) {
									var colmap = obj.COL_ColMap;
									colmap.Value.Key = newKey();
									colmap.Key = colmap.Value.Key;
								}
							}

							Jade_Reference<OBJ_GameObject> newRef = new Jade_Reference<OBJ_GameObject>(writeContext, obj.Key) {
								Value = obj
							};
							Debug.Log($"Processing Modded Object: {moddedObject.Name} [Assigned Key: {obj.Key}]");

							// Resolve this new reference with NoCache
							newRef.Resolve(flags: LOA_Loader.ReferenceFlags.DontUseCachedFile | LOA_Loader.ReferenceFlags.Log);
							moddedObject.WriteReference = newRef;
							await loader.LoadLoop(writeContext.Serializer);
							writeContext.Serializer.ClearWrittenObjects();
							if (moddedObject.CreatePrefab) {
								currentUnusedKeyPrefab = loader.Raw_CurrentUnusedKey;
							} else {
								currentUnusedKeyInstance = loader.Raw_CurrentUnusedKey;
							}
						}
						if (saveModWorld && ModWorld?.Value != null) {
							Debug.Log("Saving mod world");
							var w = ModWorld.Value;
							w.Name = ModWorldName;
							w.Key = newKey();
							ModWorld.Key = ModWorld.Value.Key;
							w.AmbientColor = new Jade_Color(0);
							w.BackgroundColor = new Jade_Color(0);
							//w.SerializedGameObjects.Clear();
							var gol = w.GameObjects.Value;
							if (gol != null) {
								w.GameObjects.Key = newKey();
								gol.Key = w.GameObjects.Key;
								var moddedGaos = ModdedGameObjects.Where(g => g.WriteReference != null && !g.CreatePrefab).ToArray();
								gol.GameObjects = new WOR_GameObjectGroup.GameObjectRef[moddedGaos.Length];
								gol.FileSize = (uint)gol.GameObjects.Length * 8;
								for (int i = 0; i < moddedGaos.Length; i++) {
									gol.GameObjects[i] = new WOR_GameObjectGroup.GameObjectRef();
									var gao = gol.GameObjects[i];
									var mod = moddedGaos[i];
									gao.Reference = mod.WriteReference;
									gao.Type = new Jade_FileType() { Extension = ".gao" };
								}
								Jade_Reference<WOR_World> newRef = new Jade_Reference<WOR_World>(writeContext, w.Key) {
									Value = w
								};
								newRef.Resolve();
								await loader.LoadLoop(writeContext.Serializer);
							}
						}

						// Export models
						foreach (var cMod in customModels) {
							var geos = cMod.GetJadeModel(loader, newKey);
							foreach (var geo in geos) {
								Jade_Reference<GEO_Object> newRef = new Jade_Reference<GEO_Object>(writeContext, geo.Key) {
									Value = geo
								};
								newRef.Resolve();
								await loader.LoadLoop(writeContext.Serializer);
							}
						}

					}

					// Save written file keys
					StringBuilder b = new StringBuilder();
					foreach (var fk in writtenFileKeys) {
						b.AppendLine($"{fk.Key:X8},{fk.Value}");
					}
					File.WriteAllText(Path.Combine(outputDir, "filekeys.txt"), b.ToString());
				}
			}
		}

		public async UniTask CreateLevelListAsync(GameSettings settings) {

			using (var context = new Ray1MapContext(settings)) {
				await LoadFilesAsync(context);
				var loader = await InitJadeAsync(context, initAI: false);

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
			s.Goto(context.GetRequiredFile(bfPath).StartPointer);
			await s.FillCacheForReadAsync((int)BIG_BigFile.HeaderLength);
			var bfFile = FileFactory.Read<BIG_BigFile>(context, bfPath);
			await s.FillCacheForReadAsync((int)bfFile.TotalFatFilesLength);
			bfFile.SerializeFatFiles(s);
			return bfFile;
		}
		public async UniTask<GEAR_BigFile> LoadGearBF(Context context, string bfPath) {
			var s = context.Deserializer;
			s.Goto(context.GetRequiredFile(bfPath).StartPointer);
			await s.FillCacheForReadAsync(GEAR_BigFile.HeaderSize);
			var bfFile = FileFactory.Read<GEAR_BigFile>(context, bfPath);
			await s.FillCacheForReadAsync((int)bfFile.ArraysSize);
			bfFile.SerializeArrays(s);
			return bfFile;
		}

		public void LoadJadeSPE(Context context) {
			if(JadeSpePath == null) return;
			var s = context.Deserializer;
			LOA_Loader loader = context.GetStoredObject<LOA_Loader>(LoaderKey);
			loader.SpecialArray = FileFactory.Read<LOA_SpecialArray>(context, JadeSpePath);
		}
		public async UniTask<Jade_Reference<WOR_WorldList>> LoadWorldList(Context context, Jade_Key worldKey, LoadFlags loadFlags, bool isFix = false, bool isEditor = false, bool isPrefabs = false) {
			LOA_Loader loader = context.GetStoredObject<LOA_Loader>(LoaderKey);
			loader.IsLoadingFix = isFix;

			Jade_Reference<WOR_WorldList> worldList = new Jade_Reference<WOR_WorldList>(context, worldKey);

			// Set up texture list
			TEX_GlobalList texList = new TEX_GlobalList();
			context.StoreObject<TEX_GlobalList>(TextureListKey, texList);
			loader.Caches[LOA_Loader.CacheType.TextureInfo].Clear();
			loader.Caches[LOA_Loader.CacheType.TextureContent].Clear();

			// Set up sound list
			SND_GlobalList sndList = new SND_GlobalList();
			context.StoreObject<SND_GlobalList>(SoundListKey, sndList);

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
							await worldList.Value.ResolveReferences(s, isPrefabs);
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
								texList.Textures[i].Content.Height != texList.Textures[i].Info.Height/* ||
								texList.Textures[i].Content.Color != texList.Textures[i].Info.Color*/) {
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

			if (loader.Context.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_KingKong)) {
				if (loadFlags.HasFlag(LoadFlags.TextNoSound)) {
					Controller.DetailedState = $"Loading text";
					for (int languageID = 0; languageID < 32; languageID++) {
						var binKey = new Jade_Key(context, worldKey.GetBinary(Jade_Key.KeyType.TextNoSound, languageID: languageID));
						if (!loader.FileInfos.ContainsKey(binKey)) continue;
						bool hasSound = false;
						loader.BeginSpeedMode(binKey, serializeAction: async s => {
							Controller.DetailedState = $"Loading text: Language {languageID} - No sound";
							for (int i = 0; i < (worldList?.Value?.Worlds?.Length ?? 0); i++) {
								var w = worldList?.Value?.Worlds[i]?.Value;
								if (w != null) {
									var world = (WOR_World)w;
									world.Text?.LoadText(languageID, hasSound);
									await loader.LoadLoopBINAsync();
									if (world.Text?.GetTextForLanguage(languageID, hasSound) != null) {
										var text = world.Text.GetTextForLanguage(languageID, hasSound);
										if (text.Text != null) {
											foreach (var txg in text.Text) {
												txg.Resolve(onPreSerialize: (_, f) => {
													((TEXT_TextGroup)f).LanguageID = languageID;
												},
												cache: LOA_Loader.CacheType.Text,
												flags: LOA_Loader.ReferenceFlags.DontCache | LOA_Loader.ReferenceFlags.DontUseCachedFile | LOA_Loader.ReferenceFlags.Log);
												await loader.LoadLoopBINAsync();
												var group = (TEXT_TextGroup)txg.Value;
												if (group == null) continue;
												var usedRef = group.GetUsedReference(languageID);
												usedRef?.ResolveBinarizedNoSound();
												await loader.LoadLoopBINAsync();
											}
										}
									}
								}
							}
							//await loader.LoadLoopBINAsync();
						});
						await loader.LoadLoop(context.Deserializer);
						loader.Caches[LOA_Loader.CacheType.Text]?.Clear();

						if (loadFlags.HasFlag(LoadFlags.TextSound)) {
							hasSound = true;
							binKey = new Jade_Key(context, worldKey.GetBinary(Jade_Key.KeyType.TextSound, languageID: languageID));
							if (!loader.FileInfos.ContainsKey(binKey)) continue;
							loader.BeginSpeedMode(binKey, serializeAction: async s => {
								Controller.DetailedState = $"Loading text: Language {languageID} - Sound";
								List<TEXT_TextList> soundTexts = new List<TEXT_TextList>();
								for (int i = 0; i < (worldList?.Value?.Worlds?.Length ?? 0); i++) {
									var w = worldList?.Value?.Worlds[i]?.Value;
									if (w != null) {
										var world = (WOR_World)w;
										world.Text?.LoadText(languageID, true);
										await loader.LoadLoopBINAsync();
										if (world.Text?.GetTextForLanguage(languageID, hasSound) != null) {
											var textNoSound = world.Text.GetTextForLanguage(languageID, false);
											var textSound = world.Text.GetTextForLanguage(languageID, hasSound);
											if (textSound.Text != null) {
												for (int j = 0; j < textSound.Text.Length; j++) {
													var txg = textSound.Text[j];
													var txgNoSound = textNoSound.Text[j];
													txg.Resolve(onPreSerialize: (_, f) => {
														((TEXT_TextGroup)f).LanguageID = languageID;
													}, flags: LOA_Loader.ReferenceFlags.DontCache | LOA_Loader.ReferenceFlags.DontUseCachedFile | LOA_Loader.ReferenceFlags.Log);
													await loader.LoadLoopBINAsync();
													var group = (TEXT_TextGroup)txg.Value;
													if (group == null) continue;
													var usedRef = group.GetUsedReference(languageID)?.TextListRef;

													var groupNoSound = (TEXT_TextGroup)txgNoSound.Value;
													if (groupNoSound == null) continue;
													if ((group?.Key?.Key ?? 0) != (groupNoSound?.Key?.Key ?? 0))
														throw new Exception($"TXG keys do not match between text bins: NS{groupNoSound?.Key} : S{group?.Key}");
													var usedRefNoSound = groupNoSound.GetUsedReference(languageID)?.TextListRef;
													if ((usedRef?.Key?.Key ?? 0) != (usedRefNoSound?.Key?.Key ?? 0))
														throw new Exception($"TXL keys do not match between text bins: NS{usedRefNoSound?.Key} : S{usedRef?.Key}");
													if (usedRefNoSound.Value == null) continue;
													var txlNoSound = (TEXT_TextList)usedRefNoSound.Value;

													usedRef?.Resolve(onPreSerialize: (_, txl) => {
														((TEXT_TextList)txl).HasSound = true;
														((TEXT_TextList)txl).TXLNoSound = txlNoSound;
													},
													cache: LOA_Loader.CacheType.TextSound,
													flags: LOA_Loader.ReferenceFlags.DontCache | LOA_Loader.ReferenceFlags.DontUseCachedFile | LOA_Loader.ReferenceFlags.Log);
													await loader.LoadLoopBINAsync();

													if (usedRef?.Value != null && usedRef?.Value is TEXT_TextList soundTxl) {
														soundTexts.Add(soundTxl);
													}

												}
											}
										}
									}
								}
								foreach (var soundTxl in soundTexts) {
									soundTxl.TXLNoSound.ResolveSounds(s);
								}
								await loader.LoadLoopBINAsync();
								//await loader.LoadLoopBINAsync();
							});
							await loader.LoadLoop(context.Deserializer);
							loader.Caches[LOA_Loader.CacheType.TextSound]?.Clear();
						}
					}
					// Fill in missing text references
					/*for (int languageID_main = 0; languageID_main < 32; languageID_main++) {
						for (int i = 0; i < (worldList?.Value?.Worlds?.Length ?? 0); i++) {
							var w = worldList?.Value?.Worlds[i]?.Value;
							if (w != null) {
								var world = (WOR_World)w;
								var alltext = world.Text?.GetTextForLanguage(languageID_main, false);
								if (alltext != null) {
									foreach (var txg_generic in alltext.Text) {
										if(txg_generic == null || txg_generic.Value == null) continue;
										var txg = (TEXT_TextGroup)txg_generic.Value;
										foreach (var txl_generic in txg.Text) {
											if(txl_generic == null || txl_generic.Value == null) continue;

										}
									}
								}
							}
						}
					}*/
				}
			} else if (loader.Context.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montpellier)) {
				if (loadFlags.HasFlag(LoadFlags.TextNoSound)) {
					Controller.DetailedState = $"Loading text";
					for (int languageID = 0; languageID < 32; languageID++) {
						var binKey = new Jade_Key(context, worldKey.GetBinary(Jade_Key.KeyType.TextNoSound, languageID: languageID));
						if (!loader.FileInfos.ContainsKey(binKey)) continue;
						bool hasSound = false;
						loader.BeginSpeedMode(binKey, serializeAction: async s => {
							Controller.DetailedState = $"Loading text: Language {languageID} - No sound";
							Jade_TextReference worldTextRef = new Jade_TextReference(context, new Jade_Key(context, 0)) { ForceResolve = true };
							Jade_TextReference fixTextRef = new Jade_TextReference(context, new Jade_Key(context, 1)) { ForceResolve = true };
							for (int i = 0; i < (worldList?.Value?.Worlds?.Length ?? 0); i++) {
								var w = worldList?.Value?.Worlds[i]?.Value;
								if (w != null) {
									var world = (WOR_World)w;
									if (world.Text != null && !world.Text.IsNull) worldTextRef = world.Text;
								}
							}
							var textRef = worldTextRef;
							textRef?.LoadText(languageID, false);
							await loader.LoadLoopBINAsync();
							var text = textRef?.GetTextForLanguage(languageID, hasSound);
							if (text?.Text != null) {
								foreach (var txg in text.Text) {
									txg.Resolve(onPreSerialize: (_, f) => {
										((TEXT_TextGroup)f).LanguageID = languageID;
									},
									cache: LOA_Loader.CacheType.Text,
									flags: LOA_Loader.ReferenceFlags.DontCache | LOA_Loader.ReferenceFlags.DontUseCachedFile | LOA_Loader.ReferenceFlags.Log);
								}
								await loader.LoadLoopBINAsync();
							}
							textRef = fixTextRef;
							textRef?.LoadText(languageID, false);
							await loader.LoadLoopBINAsync();
							text = textRef?.GetTextForLanguage(languageID, hasSound);
							if (text?.Text != null) {
								foreach (var txg in text.Text) {
									txg.Resolve(onPreSerialize: (_, f) => {
										((TEXT_TextGroup)f).LanguageID = languageID;
									},
									cache: LOA_Loader.CacheType.Text,
									flags: LOA_Loader.ReferenceFlags.DontCache | LOA_Loader.ReferenceFlags.DontUseCachedFile | LOA_Loader.ReferenceFlags.Log);
								}
								await loader.LoadLoopBINAsync();
							}
							//await loader.LoadLoopBINAsync();
						});
						await loader.LoadLoop(context.Deserializer);
						loader.Caches[LOA_Loader.CacheType.Text]?.Clear();

						if (loadFlags.HasFlag(LoadFlags.TextSound)) {
							UnityEngine.Debug.Log($"TextSound not yet implemented");
						}
					}
					// Fill in missing text references
					/*for (int languageID_main = 0; languageID_main < 32; languageID_main++) {
						for (int i = 0; i < (worldList?.Value?.Worlds?.Length ?? 0); i++) {
							var w = worldList?.Value?.Worlds[i]?.Value;
							if (w != null) {
								var world = (WOR_World)w;
								var alltext = world.Text?.GetTextForLanguage(languageID_main, false);
								if (alltext != null) {
									foreach (var txg_generic in alltext.Text) {
										if(txg_generic == null || txg_generic.Value == null) continue;
										var txg = (TEXT_TextGroup)txg_generic.Value;
										foreach (var txl_generic in txg.Text) {
											if(txl_generic == null || txl_generic.Value == null) continue;

										}
									}
								}
							}
						}
					}*/
				}
			}
			if (loadFlags.HasFlag(LoadFlags.Sounds) && context.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montpellier)) {
				Controller.DetailedState = $"Loading sounds";
				loader.BeginSpeedMode(new Jade_Key(context, Jade_Key.KeyTypeSounds), serializeAction: async s => {
					for (int i = 0; i < (sndList.Waves?.Count ?? 0); i++) {
						if (!loader.FileInfos.ContainsKey(sndList.Waves[i].Key)) {
							if (sndList.Waves[i].IsSerializeDataSupported(s)) {
								sndList.Waves[i].SerializeData(s);
							}
						}
					}
					await UniTask.CompletedTask;
				});
				await loader.LoadLoop(context.Deserializer);
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

			// Set up sound list
			SND_GlobalList sndList = new SND_GlobalList();
			context.StoreObject<SND_GlobalList>(SoundListKey, sndList);

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

			var loadFlags = LoadFlags.Maps | LoadFlags.Textures | LoadFlags.TextNoSound | LoadFlags.Universe;
			//var loadFlags = LoadFlags.All;
			var loader = await LoadJadeAsync(context, new Jade_Key(context, (uint)context.GetR1Settings().Level), loadFlags);

			stopWatch.Stop();

			Debug.Log($"Loaded BINs in {stopWatch.ElapsedMilliseconds}ms");

			await CreateTestVisualization(loader);
			if(CanBeModded) await CreateModWorld(loader);

			Debug.LogWarning("BINs serialized. Time to do something with this data :)");
			return new Unity_Level()
            {
                ObjManager = new Unity_ObjectManager(context), 
                IsometricData = new Unity_IsometricData()
                {

                },
			};
		}

		public async UniTask CreateTestVisualization(LOA_Loader loader) {
			await UniTask.CompletedTask;
			Dictionary<uint, Texture2D> textureKeyDict = new Dictionary<uint, Texture2D>();
			var worlds = loader.LoadedWorlds;
			foreach (var world in worlds) {
				GameObject w_gao = new GameObject($"({world.Key}) {world.Name}");
				foreach (var gao in world.SerializedGameObjects) {
					GameObject g_gao = new GameObject(); // GameObject.CreatePrimitive(PrimitiveType.Sphere);
					g_gao.name = $"({gao.Key}) {gao.Name}";
					g_gao.transform.SetParent(w_gao.transform);
					g_gao.transform.localPosition = gao.Matrix.GetPosition(convertAxes: true);
					g_gao.transform.localRotation = gao.Matrix.GetRotation(convertAxes: true);
					g_gao.transform.localScale = gao.Matrix.GetScale(convertAxes: true);
					var gro = gao.Base?.Visual?.GeometricObject?.Value;
					if (gro != null) {
						if (gro.RenderObject.Type == GRO_Type.GEO_StaticLOD) {
							gro = (gro.RenderObject?.Value as GEO_StaticLOD).LODLevels[0]?.Value;
						}
						if (gro.RenderObject.Type == GRO_Type.GEO) {
							Texture2D[] tex = null;
							var gro_m = gao.Base.Visual.Material?.Value;
							if (gro_m != null) {
								Texture2D GetTexture2D(Jade_TextureReference texRef) {
									if(texRef == null) return null;
									var texContent = (texRef.Content ?? texRef.Info);
									if (texContent != null) {
										if (!textureKeyDict.ContainsKey(texRef.Key)) {
											textureKeyDict[texRef.Key] = texContent.ToTexture2D();
											if (textureKeyDict[texRef.Key] != null) {
												textureKeyDict[texRef.Key].wrapMode = TextureWrapMode.Repeat;
												textureKeyDict[texRef.Key].filterMode = FilterMode.Bilinear;
											}
										}
										return textureKeyDict[texRef.Key];
									}
									return null;
								}
								Texture2D GetTextureFromRenderObject(GRO_Struct renderObject) {
									if(renderObject == null) return null;
									if (renderObject.Type == GRO_Type.MAT_MTT) {
										var mat_mtt = (MAT_MTT_MultiTextureMaterial)renderObject.Value;
										
										if ((mat_mtt.Levels?.Length ?? 0) > 0) {
											for (int i = 0; i < mat_mtt.Levels.Length; i++) {
												var texRef = mat_mtt.Levels[i].Texture;
												var tex = GetTexture2D(texRef);
												if(tex != null) return tex;
											}
										}
									} else if (renderObject.Type == GRO_Type.MAT_SIN) {
										var mat_sin = (MAT_SIN_SingleMaterial)renderObject.Value;
										var texRef = mat_sin.Texture;
										var tex = GetTexture2D(texRef);
										if (tex != null) return tex;
									}
									return null;
								}
								if (gro_m.RenderObject.Type == GRO_Type.MAT_MTT || gro_m.RenderObject.Type == GRO_Type.MAT_SIN) {
									tex = new Texture2D[1];
									tex[0] = GetTextureFromRenderObject(gro_m.RenderObject);
								} else if (gro_m.RenderObject.Type == GRO_Type.MAT_MSM) {
									var mat = (MAT_MSM_MultiSingleMaterial)gro_m.RenderObject.Value;
									tex = new Texture2D[mat.Materials.Length];
									for (int i = 0; i < mat.Materials.Length; i++) {
										var gro_m_sub = mat.Materials[i]?.Value?.RenderObject;
										tex[i] = GetTextureFromRenderObject(gro_m_sub);
									}
								}
							}
							GameObject g_geo = new GameObject($"Geo {gro.Key}");
							g_geo.transform.SetParent(g_gao.transform, false);
							var geo = (GEO_GeometricObject)gro.RenderObject.Value;

							if (geo.OptimizedGeoObject_PS2 != null
								//&& geo.Context.GetR1Settings().Platform == Platform.PS2
								&& !geo.Montreal_FilledUnoptimizedData) {
								var ps2 = geo.OptimizedGeoObject_PS2;
								if (geo.Context.GetR1Settings().Platform == Platform.PS2) {
									geo.Montreal_FilledUnoptimizedData = true;
									gao?.Base?.Visual?.VisuPS2?.ExecuteChainPrograms(gao, geo, ps2);
								} else if (geo.Context.GetR1Settings().Platform == Platform.PSP) {
									geo.Montreal_FilledUnoptimizedData = true;
									gao?.Base?.Visual?.VisuPS2?.ExecuteGEPrograms(gao, geo, ps2);
								}
							} else if (geo.OptimizedGeoObject_PC != null
								&& !geo.Montreal_FilledUnoptimizedData) {
								geo.OptimizedGeoObject_PC.Unoptimize();
							}
							if (geo.Montreal_WasOptimized && geo.OptimizedGeoObject_PC != null) {
								gao?.Base?.Visual?.VisuPC?.Unoptimize(gao?.Base?.Visual, geo, geo?.OptimizedGeoObject_PC);
							}
							if (geo.Elements != null) {
								var verts = geo.Vertices.Select(v => new Vector3(v.X, v.Z, v.Y)).ToArray();
								var uvs = geo.UVs.Select(uv => new Vector2(uv.U, uv.V)).ToArray();
								Color[] colors = null;
								Color ComputeColor(Color c) {
									return Color.Lerp(Color.white, new Color(c.r, c.g, c.b, 1f), c.a);
								}
								if (gao.Base?.Visual?.VertexColors != null) {
									if (gao.Base?.Visual?.VertexColors.Length == verts.Length) {
										colors = gao.Base?.Visual?.VertexColors?.Select(c => ComputeColor(c.GetColor())).ToArray();
									}
								} else if (gao.Base?.Visual?.RLI?.Value != null) {
									if (gao.Base?.Visual?.RLI?.Value?.VertexRLI.Length == verts.Length) {
										colors = gao.Base?.Visual?.RLI?.Value?.VertexRLI?.Select(c => ComputeColor(c.GetColor())).ToArray();
									}
								} else if (geo.Colors != null) {
									if (geo.Colors?.Length == verts.Length) {
										colors = geo.Colors?.Select(c => ComputeColor(c.GetColor())).ToArray();
									}
								}
								foreach (var e in geo.Elements) {
									Mesh m = new Mesh();
									var vertIndices = e.Triangles.SelectMany(t => new int[] { t.Vertex1, t.Vertex0, t.Vertex2 });
									m.vertices = vertIndices.Select(v => verts[v]).ToArray();
									if (uvs.Length > 0) {
										var uvIndices = e.Triangles.SelectMany(t => new int[] { t.UV1, t.UV0, t.UV2 });
										m.uv = uvIndices.Select(uv => uvs[uv]).ToArray();
									}
									if (colors != null) {
										m.colors = vertIndices.Select(v => colors[v]).ToArray();
									}
									m.triangles = Enumerable.Range(0, m.vertices.Length).ToArray();
									m.RecalculateNormals();
									GameObject g_geo_e = new GameObject($"Element {e.Offset}");
									g_geo_e.transform.SetParent(g_geo.transform, false);
									g_geo_e.layer = LayerMask.NameToLayer("3D Collision");
									MeshFilter mf = g_geo_e.AddComponent<MeshFilter>();
									mf.mesh = m;
									MeshRenderer mr = g_geo_e.AddComponent<MeshRenderer>();
									mr.material = Controller.obj.levelController.controllerTilemap.unlitMaterial;
									var texturesLength = tex?.Length ?? 0;
									if (texturesLength > 0) {
										if (texturesLength == 1) {
											mr.material.SetTexture("_MainTex", tex[0]);
										} else {
											if (e.MaterialID < texturesLength) {
												mr.material.SetTexture("_MainTex", tex[e.MaterialID]);
											}
										}
									}
								}
							}
						}
					}


				}
			}
			Controller.obj.levelController.editor.cam.camera3D.farClipPlane = 10000f;
		}
		public async UniTask CreateModWorld(LOA_Loader loader) {
			await UniTask.CompletedTask;
			GameObject gao = new GameObject($"MOD_{loader.LoadedWorlds.Last().Name}");
			JadeModBehaviour modBehaviour = gao.AddComponent<JadeModBehaviour>();
			gao.transform.localPosition = Vector3.zero;
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
			if(loader.ShouldExportVars) {
				string worldName = "Univers";
				string name = "univers";
				univers?.Value?.Vars?.Value?.ExportVarsOverview(worldName, $"{name}_instance");
				univers?.Value?.Vars?.Value?.ExportStruct(worldName, $"{name}_instance");
				univers?.Value?.Model?.Value?.Vars?.ExportVarsOverview(worldName, $"{name}_model");
				univers?.Value?.Model?.Value?.Vars?.ExportStruct(worldName, $"{name}_model");
				univers?.Value?.Model?.Value?.Vars?.ExportStruct(worldName, $"{name}_save", save: true);
				univers?.Value?.Model?.Value?.Vars?.ExportStruct(worldName, $"{name}_save_serializable", save: true, mode: AI_Vars.ExportStructMode.BinarySerializable);
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

		public async UniTask<LOA_Loader> InitJadeAsync(Context context, bool initAI = true, bool initTextures = false, bool initSound = false) {
			List<BIG_BigFile> bfs = new List<BIG_BigFile>();
			foreach (var bfPath in BFFiles) {
				var bf = await LoadBF(context, bfPath);
				bfs.Add(bf);
			}

			// Set up loader
			LOA_Loader loader = new LOA_Loader(bfs.ToArray(), context);
			context.StoreObject<LOA_Loader>(LoaderKey, loader);

			if (initAI) {
				// Set up AI types
				AI_Links aiLinks = AI_Links.GetAILinks(context.GetR1Settings());
				context.StoreObject<AI_Links>(AIKey, aiLinks);
			}

			if (initTextures) {
				// Set up texture list
				TEX_GlobalList texList = new TEX_GlobalList();
				context.StoreObject<TEX_GlobalList>(TextureListKey, texList);
			}

			if (initSound) {
				// Set up sound list
				SND_GlobalList sndList = new SND_GlobalList();
				context.StoreObject<SND_GlobalList>(SoundListKey, sndList);
			}

			return loader;
		}

		public async UniTask<LOA_Loader> LoadJadeAsync(Context context, Jade_Key worldKey, LoadFlags loadFlags) 
        {
            LOA_Loader loader = await InitJadeAsync(context);

			// Load jade.spe
			LoadJadeSPE(context);

			// Create level list if null
			var levelInfos = LevelInfos;
			bool isWOW = false;
			bool isEditor = false;
			bool isPrefabs = false;
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
				isPrefabs = levInfo != null && levInfo.IsPrefabs;
			}

			if (loadFlags.HasFlag(LoadFlags.Universe) && !isPrefabs) {
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
					var worldList = await LoadWorldList(context, worldKey, loadFlags, isEditor: isEditor, isPrefabs: isPrefabs);
				}
			}

			return loader;
        }

        public override async UniTask LoadFilesAsync(Context context) {
			foreach (var bfPath in BFFiles) {
				await context.AddLinearFileAsync(bfPath, bigFileCacheLength: 8);
			}
			if (JadeSpePath != null) await context.AddLinearFileAsync(JadeSpePath);
			if (TexturesGearBFPath != null) await context.AddLinearFileAsync(TexturesGearBFPath, bigFileCacheLength: 8);
			if (SoundGearBFPath != null) await context.AddLinearFileAsync(SoundGearBFPath, bigFileCacheLength: 8);
		}

		// Constants
		public static readonly Encoding Encoding = Encoding.GetEncoding(1252);
		public const string LoaderKey = "loader";
		public const string TextureListKey = "textureList";
		public const string SoundListKey = "soundList";
		public const string AIKey = "ai";

		public class LevelInfo
        {
            public LevelInfo(uint key, string directoryPath, string filePath, string worldName = null, string mapName = null, bool isPrefabs = false, FileType? type = null)
            {
				OriginalMapName = mapName;
				OriginalWorldName = worldName;
				OriginalType = type;
                Key = key;
                DirectoryPath = directoryPath;
                FilePath = filePath;
				IsPrefabs = isPrefabs;
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

			public bool IsPrefabs { get; }
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
