using BinarySerializer;
using Cysharp.Threading.Tasks;
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
				new GameAction("Extract BF file(s)", false, true, (input, output) => new Jade_GameActions_ExtractBF(this).ExtractFilesAsync(settings, output, decompressBIN: false)),
				new GameAction("Extract BF file(s) - BIN decompression", false, true, (input, output) => new Jade_GameActions_ExtractBF(this).ExtractFilesAsync(settings, output, decompressBIN: true)),
				new GameAction("Create level list", false, false, (input, output) => CreateLevelListAsync(settings)),
				new GameAction("Export localization", false, true, (input, output) => new Jade_GameActions_ExportLocalization(this).ExportLocalizationAsync(settings, output, false)),
				new GameAction("Export textures", false, true, (input, output) => new Jade_GameActions_ExportTextures(this).ExportTexturesAsync(settings, output, true)),
				new GameAction("Export models", false, true, (input, output) => new Jade_GameActions_ExportModels(this).ExportModelsAsync(settings, output)),
				new GameAction("Export sounds", false, true, (input, output) => new Jade_GameActions_ExportSounds(this).ExportSoundsAsync(settings, output, true)),
				new GameAction("Export AI Lists", false, true, (input, output) => new Jade_GameActions_ExportAIModels(this).ExportAI(settings, output, exportInstances: true)),
				new GameAction("Export unbinarized assets", false, true, (input, output) => new Jade_GameActions_ConvertUnbinarize(this).ExportUnbinarizedAsync(settings, null, output, true, null)),
				new GameAction("Export unbinarized into RRR format", true, true, (input, output) => new Jade_GameActions_ConvertUnbinarize(this).ExportUnbinarizedAsync(settings, input, output, true, targetMode: GameModeSelection.RaymanRavingRabbidsPC)),
				new GameAction("Export unbinarized into RRR Prototype format", true, true, (input, output) => new Jade_GameActions_ConvertUnbinarize(this).ExportUnbinarizedAsync(settings, input, output, true, targetMode: GameModeSelection.RaymanRavingRabbidsPCPrototype)),
				new GameAction("Create new BF using unbinarized files", true, true, (input, output) => new Jade_GameActions_CreateBF(this).CreateBFAsync(settings, input, output)),
				new GameAction("Temp Tools", false, true, (input, output) => new Jade_GameActions_ModToolsRRRPrototype(this).TempTools(settings, input, output)),
			};
			if (CanBeModded) {
				actions = actions.Concat(new GameAction[] {
					new GameAction("Add modded GameObjects", true, true, (input, output) => new Jade_GameActions_ModdingUnityScene(this).AddModdedGameObjects(settings, input, output)),
				}).ToArray();
			}
			if (HasUnbinarizedData) {
				actions = actions.Concat(new GameAction[] {
					new GameAction("Extract BF file(s) - Comparison", true, true, (input, output) => new Jade_GameActions_ExtractBF(this).ExtractFilesAsync(settings, output, decompressBIN: false, createAllDirectories: false, onlyRecent: true, compareEquality: input)),
					new GameAction("Export textures (unbinarized)", false, true, (input, output) => new Jade_GameActions_ExportTextures(this).ExportTexturesUnbinarized(settings, output)),
					new GameAction("Export models (unbinarized)", false, true, (input, output) => new Jade_GameActions_ExportModels(this).ExportModelsUnbinarizedAsync(settings, output)),
					new GameAction("Export localization (unbinarized)", false, true, (input, output) => new Jade_GameActions_ExportLocalization(this).ExportLocalizationUnbinarizedAsync(settings, output)),
					new GameAction("Export AI Lists (unbinarized)", false, true, (input, output) => new Jade_GameActions_ExportAIModels(this).ExportAIUnbinarized(settings, output)),
				}).ToArray();
			}
			if (TexturesGearBFPath != null) {
				actions = actions.Concat(new GameAction[] {
					new GameAction("Export textures (Gear BF)", false, true, (input, output) => new Jade_GameActions_ExportGearBF(this).ExportTexturesGearBF(settings, output))
				}).ToArray();
			}
			if (SoundGearBFPath != null) {
				actions = actions.Concat(new GameAction[] {
					new GameAction("Export sound (Gear BF)", false, true, (input, output) => new Jade_GameActions_ExportGearBF(this).ExportGearBF(settings, SoundGearBFPath, output, "wav"))
				}).ToArray();
			}
			return actions;
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
							LOA_Loader.ReferenceFlags.MustExist | LOA_Loader.ReferenceFlags.Montreal_NoKeyChecks |
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
												flags: LOA_Loader.ReferenceFlags.DontCache | LOA_Loader.ReferenceFlags.DontUseCachedFile | LOA_Loader.ReferenceFlags.MustExist);
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
													}, flags: LOA_Loader.ReferenceFlags.DontCache | LOA_Loader.ReferenceFlags.DontUseCachedFile | LOA_Loader.ReferenceFlags.MustExist);
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
													flags: LOA_Loader.ReferenceFlags.DontCache | LOA_Loader.ReferenceFlags.DontUseCachedFile | LOA_Loader.ReferenceFlags.MustExist);
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
									flags: LOA_Loader.ReferenceFlags.DontCache | LOA_Loader.ReferenceFlags.DontUseCachedFile | LOA_Loader.ReferenceFlags.MustExist);
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
									flags: LOA_Loader.ReferenceFlags.DontCache | LOA_Loader.ReferenceFlags.DontUseCachedFile | LOA_Loader.ReferenceFlags.MustExist);
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

				loader.EndSpeedMode();
				loader.IsLoadingFix = false;

				// Also load waves outside of bin
				List<Jade_Reference<SND_Wave>> wavesOutsideBin = new List<Jade_Reference<SND_Wave>>();
				for (int i = 0; i < (sndList.Waves?.Count ?? 0); i++) {
					if (loader.FileInfos.ContainsKey(sndList.Waves[i].Key)) {
						Jade_Reference<SND_Wave> wave = new Jade_Reference<SND_Wave>(context, sndList.Waves[i].Key);
						wave?.Resolve(flags: LOA_Loader.ReferenceFlags.DontUseCachedFile | LOA_Loader.ReferenceFlags.DontCache);
						wavesOutsideBin.Add(wave);
					}
				}
				await loader.LoadLoop(context.Deserializer);

				// Merge them, overwriting the data inside 
				foreach (var wave in wavesOutsideBin) {
					var toMerge = sndList?.Waves?.FindItem(w => w.Key == wave.Key);
					toMerge.Merge(wave?.Value);
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


			Texture2D GetTexture2D(Jade_TextureReference texRef) {
				if (texRef == null) return null;
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

			var worlds = loader.LoadedWorlds;
			foreach (var world in worlds) {
				GameObject w_gao = new GameObject($"({world.Key}) {world.Name}");
				foreach (var gao in world.SerializedGameObjects) {
					gao.UnoptimizeGeometry();

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

							if (geo.Elements != null) {
								Vector3[] verts = null;
								Vector2[] uvs = null;
								Vector2[] uvs1 = null;
								Color[] colors = null;
								if (geo.CPP_VertexBuffer != null) {
									var vertsJade = geo.CPP_VertexBuffer.Vertices.Select(v => v.ToVector(GEO_CompressedFloat.FloatType.Vertex)).ToArray();
									var uvsJade = geo.CPP_VertexBuffer.Tex0.Select(v => v.ToUV(GEO_CompressedFloat.FloatType.TexCoord0)).ToArray();
									verts = vertsJade.Select(v => new Vector3(v.X, v.Z, v.Y)).ToArray();
									uvs = uvsJade.Select(uv => new Vector2(uv.U, uv.V)).ToArray();
									colors = geo.CPP_VertexBuffer?.Colors?.Select(c => ComputeColor(c.GetColor()))?.ToArray();
									var uvs1Jade = geo.CPP_VertexBuffer.Tex1.Select(v => v.ToUV(GEO_CompressedFloat.FloatType.TexCoord1)).ToArray();
									uvs1 = uvs1Jade.Select(uv => new Vector2(uv.U, uv.V)).ToArray();
								} else {
									verts = geo.Vertices.Select(v => new Vector3(v.X, v.Z, v.Y)).ToArray();
									uvs = geo.UVs.Select(uv => new Vector2(uv.U, uv.V)).ToArray();

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
								}
								Color ComputeColor(Color c) {
									return Color.Lerp(Color.white, new Color(c.r, c.g, c.b, 1f), c.a);
								}
								foreach (var e in geo.Elements) {
									Mesh m = new Mesh();
									IEnumerable<int> vertIndices = null;
									IEnumerable<int> uvIndices = null;
									IEnumerable<int> uv1Indices = null;
									IEnumerable<int> colIndices = null;
									List<int> FillList(ushort[] indices, bool isStrip) {
										List<int> list = new List<int>();
										if (!isStrip) {
											for (int vi = 0; vi < indices.Length / 3; vi++) {
												list.Add(indices[vi * 3 + 1]);
												list.Add(indices[vi * 3 + 0]);
												list.Add(indices[vi * 3 + 2]);
											}
										} else {
											for (int vi = 0; vi < indices.Length - 2; vi++) {
												if (vi % 2 == 0) {
													list.Add(indices[vi + 1]);
													list.Add(indices[vi + 0]);
													list.Add(indices[vi + 2]);
												} else {
													list.Add(indices[vi + 0]);
													list.Add(indices[vi + 1]);
													list.Add(indices[vi + 2]);
												}
											}
										}
										return list;
									}
									if (e.IndexBuffer != null) {
										vertIndices = FillList(e.IndexBuffer.IndicesPos, e.IndexBuffer.UseStrips != 0);
										if (uvs != null && uvs.Length > 0) {
											uvIndices = FillList(e.IndexBuffer.IndicesTex0, e.IndexBuffer.UseStrips != 0);
										}
										if (uvs1 != null && uvs1.Length > 0) {
											uv1Indices = FillList(e.IndexBuffer.IndicesTex1, e.IndexBuffer.UseStrips != 0);
										}
										if (colors != null && e.IndexBuffer.IndicesCol != null) {
											colIndices = FillList(e.IndexBuffer.IndicesCol, e.IndexBuffer.UseStrips != 0);
										}
										if (e.StitchBuckets != null) {
											foreach (var bucket in e.StitchBuckets) {
												if (bucket.IndexBuffer != null) {
													var indexBuffer = bucket.IndexBuffer;
													var vertIndicesList = FillList(indexBuffer.IndicesPos, indexBuffer.UseStrips != 0);
													vertIndices = vertIndices.Concat(vertIndicesList).ToArray();
													if (uvs != null && uvs.Length > 0) {
														var uvIndicesList = FillList(indexBuffer.IndicesTex0, indexBuffer.UseStrips != 0);
														uvIndices = uvIndices.Concat(uvIndicesList).ToArray();
													}
													if (uvs1 != null && uvs1.Length > 0) {
														var uvIndicesList = FillList(indexBuffer.IndicesTex1, indexBuffer.UseStrips != 0);
														uv1Indices = uv1Indices.Concat(uvIndicesList).ToArray();
													}

													if (colors != null && indexBuffer.IndicesCol != null) {
														var colIndicesList = FillList(indexBuffer.IndicesCol, indexBuffer.UseStrips != 0);
														if (colIndices != null)
															colIndices = colIndices.Concat(colIndicesList).ToArray();
													}
												}
											}
										}
									} else {
										vertIndices = e.Triangles.SelectMany(t => new int[] { t.Vertex1, t.Vertex0, t.Vertex2 });
										if(uvs.Length > 0) uvIndices = e.Triangles.SelectMany(t => new int[] { t.UV1, t.UV0, t.UV2 });
										colIndices = vertIndices;
									}
									m.vertices = vertIndices.Select(v => verts[v]).ToArray();
									if (uvs != null && uvs.Length > 0) {
										m.uv = uvIndices.Select(uv => uvs[uv]).ToArray();
									}
									if (uvs1 != null && uvs1.Length > 0) {
										m.SetUVs(3, uv1Indices.Select(uv => uvs1[uv]).ToArray());
									}
									if (colors != null && colIndices != null && colors.Length > 0) {
										m.colors = colIndices.Select(v => colors[v]).ToArray();
									}
									m.triangles = Enumerable.Range(0, m.vertices.Length).ToArray();
									m.RecalculateNormals();
									GameObject g_geo_e = new GameObject($"Element {e.Offset}");
									g_geo_e.transform.SetParent(g_geo.transform, false);
									g_geo_e.layer = LayerMask.NameToLayer("3D Collision");
									MeshFilter mf = g_geo_e.AddComponent<MeshFilter>();
									mf.mesh = m;
									MeshRenderer mr = g_geo_e.AddComponent<MeshRenderer>();
									var matSrc = Controller.obj.levelController.controllerTilemap.MaterialPsychonautsAlphaTest;

									Material mat = new Material(matSrc);
									//mat.SetVector("_MaterialColor", meshFrag.MaterialColor.ToVector4());
									//mat.SetFloat("_IsSelfIllumination", isSelfIllum ? 1 : 0);
									mr.sharedMaterial = mat;

									var texturesLength = tex?.Length ?? 0;
									Vector4 texturesInUse = new Vector4();
									if (texturesLength > 0) {
										Texture2D mainTex = null;
										if (texturesLength == 1) {
											mainTex = tex[0];
											mat.SetTexture("_Tex0", tex[0]);
										} else {
											if (e.MaterialID < texturesLength) {
												mainTex = tex[e.MaterialID];
												mat.SetTexture("_Tex0", tex[e.MaterialID]);
											}
										}
										if (mainTex != null) {
											texturesInUse[0] = 1;
										}
										if (e.IndexBuffer != null && mainTex != null && uvIndices != null) {
											//var dimensions = new Vector2(0x10000f / mainTex.width, 0x10000f / mainTex.height);
											var dimensions = Vector2.one;
											m.uv = uvIndices.Select(uv => uvs[uv] * dimensions).ToArray();
										}
									}
									if (gao.Base?.Visual?.LightmapTexture != null) {
										var lmTex = GetTexture2D(gao.Base.Visual.LightmapTexture);
										if (lmTex != null) {
											mat.SetTexture("_TexLightMap", lmTex);
											texturesInUse[3] = 1;
										}
									}
									mat.SetVector("_TexturesInUse", texturesInUse);
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
			loader.Universe = univers;
			if (context.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal)) {
				Jade_Reference<Jade_BinTerminator> terminator = new Jade_Reference<Jade_BinTerminator>(context, new Jade_Key(context, 0)) { ForceResolve = true };
				loader.BeginSpeedMode(univers.Key, async s => { // Univers is bin compressed in Montreal version
					univers.Resolve();
					await loader.LoadLoopBINAsync();
					if (!s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PhoenixRayman4) && !s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_CPP)) {
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

			if (loadFlags != LoadFlags.Universe) { // If we're loading more than just the universe
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
