using BinarySerializer;
using Cysharp.Threading.Tasks;
using Ray1Map.Jade;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using static Ray1Map.Jade_BaseManager;

namespace Ray1Map {
	public class Jade_GameActions_ConvertUnbinarize : Jade_GameActions {
		public Jade_GameActions_ConvertUnbinarize(Jade_BaseManager manager) : base(manager) { }

		public async UniTask ExportUnbinarizedAsync(
			GameSettings settings,
			string inputDir, string outputDir,
			bool useComplexNames,
			GameModeSelection? targetMode = null,
			bool exportWOLfiles = true) {
			var parsedSounds = new HashSet<uint>();

			var levIndex = 0;
			uint currentKey = 0;
			Dictionary<uint, string> writtenFileKeys = new Dictionary<uint, string>();

			// Key relocation (for writing as RRR mod)
			Dictionary<uint, uint> keysToRelocate = new Dictionary<uint, uint>();
			Dictionary<uint, uint> keysToRelocateReverse = new Dictionary<uint, uint>();
			HashSet<uint> keysToAvoid = new HashSet<uint>();
			LOA_Loader.ExportFilenameGuessData namingData = new LOA_Loader.ExportFilenameGuessData();

			bool exportForDifferentGameMode = targetMode.HasValue;// && settings.GameModeSelection != GameModeSelection.KingKongPC;

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

			// First, loop over all wol files to export if necessary, and try to get directory for the wow
			try {
				List<WOR_WorldList> wols = new List<WOR_WorldList>();
				using (var context = new Ray1MapContext(settings)) {
					await LoadFilesAsync(context);
					List<BIG_BigFile> bfs = new List<BIG_BigFile>();
					foreach (var bfPath in BFFiles) {
						var bf = await LoadBF(context, bfPath);
						bfs.Add(bf);
					}
					// Set up loader
					LOA_Loader loader = new LOA_Loader(bfs.ToArray(), context) {
						LoadSingle = true
					};
					context.StoreObject<LOA_Loader>(LoaderKey, loader);

					foreach (var kvp in loader.FileInfos) {
						var fileInfo = kvp.Value;
						if (fileInfo.FileName != null && (fileInfo.FileName.EndsWith(".wol"))) {
							try {
								Jade_Reference<WOR_WorldList> wolRef = new Jade_Reference<WOR_WorldList>(context, fileInfo.Key);
								wolRef.Resolve();
								await loader.LoadLoop(context.Deserializer);

								if (wolRef?.Value == null) continue;
								var wol = wolRef.Value;

								wols.Add(wol);
								var wolFileInfo = loader.FileInfos[wolRef.Key];
								if (wolFileInfo.FileName != null) {
									namingData.AddFact(wolRef.Key, Path.GetFileNameWithoutExtension(wolFileInfo.FileNameValidCharacters), wolFileInfo.DirectoryName);
								}

							} catch (Exception ex) {
								UnityEngine.Debug.LogError(ex);
							}
							await Controller.WaitIfNecessary();
						}
					}
					foreach (var wol in wols) {
						var validworlds = wol?.Worlds?.Where(w => !w.IsNull);
						var wolInfo = loader.FileInfos[wol.Key];
						if (wolInfo.FileName != null) {
							var directory = wolInfo.DirectoryName;
							int count = validworlds.Count();
							int i = 0;
							foreach (var w in validworlds) {
								namingData.AddGuess(w.Key, null, directory, 100 * (i + 1) / count);
								i++;
							}
						}
					}

					if (exportWOLfiles) {
						using (var writeContext = new Ray1MapContext(outputDir, settings)) {
							// Set up loader
							LOA_Loader writeloader = new LOA_Loader(loader.BigFiles, writeContext) {
								Raw_WriteFilesAlreadyInBF = true,
								Raw_UseOriginalFileNames = true,
								LoadSingle = true
							};
							writeloader.Raw_FilenameGuesses = namingData;
							if (exportForDifferentGameMode) {
								writeloader.Raw_RelocateKeys = true;
								writeloader.Raw_KeysToAvoid = keysToAvoid;
								writeloader.Raw_KeysToRelocate = keysToRelocate;
								writeloader.Raw_KeysToRelocateReverse = keysToRelocateReverse;
							}
							writeloader.WrittenFileKeys = writtenFileKeys;
							writeContext.StoreObject<LOA_Loader>(LoaderKey, writeloader);

							foreach (var wol in wols) {
								var wkey = writeloader.Raw_RelocateKeyIfNecessary(wol.Key);
								Jade_Reference<WOR_WorldList> writeWol = new Jade_Reference<WOR_WorldList>(writeContext, new Jade_Key(context, wkey)) {
									Value = wol
								};
								writeWol.Resolve();

								var s = writeContext.Serializer;
								await writeloader.LoadLoop(s);
							}
						}
					}
				}
			} catch (Exception ex) {
				UnityEngine.Debug.LogError(ex);
			}


			foreach (var lev in LevelInfos) {
				// If there are WOL files, there are also raw WOW files. It's better to process those one by one.
				if (lev?.Type.HasValue ?? false) {
					if (lev.Type.Value.HasFlag(LevelInfo.FileType.WOL)) {
						levIndex++;
						continue;
					}
				}
				if (levIndex <= 15) {
					levIndex++;
					continue;
				}
				if (levIndex >= 30) {
					levIndex++;
					continue;
				}
				/*if (!(lev?.MapName.Contains("ilot") ?? false)) {
					levIndex++;
					continue;
				}*/

				/*if (!(lev?.MapName.Contains("FPS") ?? false)) {
					levIndex++;
					continue;
				}*/
				/*if (!(lev?.MapName.Contains("Finger_Guess") ?? false)) {
					levIndex++;
					continue;
				}*/

				Debug.Log($"Exporting for level {levIndex++ + 1}/{LevelInfos.Length}: {lev.MapName}");

				try {
					using (var context = new Ray1MapContext(settings)) {
						currentKey = 0;
						await LoadFilesAsync(context);
						if (exportForDifferentGameMode) {
							await LoadJadeAsync(context, new Jade_Key(context, lev.Key), LoadFlags.Maps | LoadFlags.Textures | LoadFlags.TextNoSound);
						} else {
							await LoadJadeAsync(context, new Jade_Key(context, lev.Key), LoadFlags.All);
						}

						LOA_Loader actualLoader = context.GetStoredObject<LOA_Loader>(LoaderKey);
						var worlds = actualLoader.LoadedWorlds;

						if (exportForDifferentGameMode) {
							foreach (var w in worlds) {
								//w.Text = new Jade_TextReference(context, new Jade_Key(context, 0xFFFFFFFF));
								foreach (var gao in w.SerializedGameObjects) {
									gao.FlagsIdentity &= ~OBJ_GameObject_IdentityFlags.Sound;
									gao.FlagsIdentity &= ~OBJ_GameObject_IdentityFlags.AI;

									if (gao.Extended?.Modifiers != null) {
										//if(targetMode.Value == GameModeSelection.RaymanRavingRabbidsPCPrototype && !context.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_RRR))
										//	FixGameObjectMorphChannels(context, gao);

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
												m.Type != MDF_ModifierType.GEN_ModifierSound && m.Type != MDF_ModifierType.MDF_LoadingSound && m.Type != MDF_ModifierType.GEN_ModifierSoundFx
												&& rrrPC_supportedModifiers.Contains((int)m.Type))
											.ToArray();
										if (gao.Extended.Modifiers.Length == 0
											|| (gao.Extended.Modifiers.Length == 1 && gao.Extended.Modifiers[0].Type == MDF_ModifierType.None)) gao.Extended.HasModifiers = 0;
									}

									if (!context.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_KingKong) && targetMode != GameModeSelection.RaymanRavingRabbidsPCPrototype) {
										if (gao?.Base?.Visual?.VertexColors != null) {
											gao.Base.Visual.VertexColors = gao.Base.Visual.VertexColors.Select(c => new Jade_Color(c.Blue, c.Green, c.Red, c.Alpha)).ToArray();
										}
									}
								}
							}
						}

						//string worldName = null;
						if (context.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal) && !exportForDifferentGameMode) {
							throw new NotImplementedException("Not yet implemented for Montreal");
						} else {
							foreach (var w in worlds) {
								await ExportUnbinarized(w);
							}
							if (!exportForDifferentGameMode) await ExportRest();
						}

						async UniTask ExportUnbinarized(WOR_World world) {
							if (world != null) {
								Debug.Log($"Loaded level. Exporting unbinarized assets...");
								await Controller.WaitIfNecessary();

								// First, try to determine names
								var bestName = namingData.GetMostLikelyFilename(world.Key);
								if (bestName != null) {
									var wowDir = bestName.Directory;
									namingData.AddGuess(world.Key, world.Name, wowDir, 100);
									namingData.AddGuess(world.GameObjects?.Key, world.Name, wowDir, 100);
									namingData.AddGuess(world.Text?.Key, world.Name, wowDir, 100);
									namingData.AddGuess(world.AllNetworks?.Key, world.Name, wowDir, 100);
									if (world.SerializedGameObjects.Any()) {
										var gaoDir = $"{wowDir}/Game Objects";
										var cobDir = $"{wowDir}/Collision Objects";
										var cinDir = $"{wowDir}/COL Instances";
										var lnkDir = $"{wowDir}/Links";
										var rliDir = $"{wowDir}/Game Objects RLI";
										var gaoPrio = 10000 / world.SerializedGameObjects.Count;
										foreach (var obj in world?.SerializedGameObjects) {
											var objname = obj.Export_FileBasename;
											namingData.AddGuess(obj.Key, objname, gaoDir, gaoPrio);
											namingData.AddGuess(obj.Base?.Visual?.RLI?.Key, objname, rliDir, gaoPrio);
											namingData.AddGuess(obj.COL_ColMap?.Key, objname, cinDir, gaoPrio);
											if (obj.COL_ColMap?.Value?.Cobs != null) {
												foreach (var cob in obj.COL_ColMap?.Value?.Cobs) {
													namingData.AddGuess(cob.Key, $"{cob.Key.Key:X8}_{objname}", cobDir, gaoPrio);
												}
											}
											namingData.AddGuess(obj.Extended?.Links?.Key, objname, lnkDir, gaoPrio);
											namingData.AddGuess(obj.COL_Instance?.Key, objname, cinDir, gaoPrio);
										}
									}
								}


								LOA_Loader actualLoader = context.GetStoredObject<LOA_Loader>(LoaderKey);

								var newSettings = new GameSettings(targetMode ?? settings.GameModeSelection, settings.GameDirectory, settings.World, settings.Level);

								using (var writeContext = new Ray1MapContext(outputDir, newSettings)) {
									// Set up loader
									LOA_Loader loader = new LOA_Loader(actualLoader.BigFiles, writeContext);

									loader.Raw_WriteFilesAlreadyInBF = HasUnbinarizedData || exportForDifferentGameMode;
									if (exportForDifferentGameMode) {
										loader.Raw_RelocateKeys = true;
										loader.Raw_KeysToAvoid = keysToAvoid;
										loader.Raw_KeysToRelocate = keysToRelocate;
										loader.Raw_KeysToRelocateReverse = keysToRelocateReverse;
									}
									loader.Raw_FilenameGuesses = namingData;

									loader.WrittenFileKeys = writtenFileKeys;
									writeContext.StoreObject<LOA_Loader>(LoaderKey, loader);
									var sndListWrite = new SND_GlobalList();
									writeContext.StoreObject<SND_GlobalList>(SoundListKey, sndListWrite);
									var texListWrite = new TEX_GlobalList();
									writeContext.StoreObject<TEX_GlobalList>(TextureListKey, texListWrite);
									var aiLinks = context.GetStoredObject<AI_Links>(AIKey);
									writeContext.StoreObject<AI_Links>(AIKey, aiLinks);

									var wkey = loader.Raw_RelocateKeyIfNecessary(world.Key.Key);

									Jade_Reference<WOR_World> worldRef = new Jade_Reference<WOR_World>(writeContext, new Jade_Key(writeContext, wkey)) {
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

								loader.Raw_WriteFilesAlreadyInBF = HasUnbinarizedData || exportForDifferentGameMode;
								if (exportForDifferentGameMode) {
									loader.Raw_RelocateKeys = true;
									loader.Raw_KeysToAvoid = keysToAvoid;
									loader.Raw_KeysToRelocate = keysToRelocate;
									loader.Raw_KeysToRelocateReverse = keysToRelocateReverse;
								}
								loader.Raw_FilenameGuesses = namingData;

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
	}
}
