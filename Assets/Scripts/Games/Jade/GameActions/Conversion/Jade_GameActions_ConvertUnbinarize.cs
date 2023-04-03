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

		private class AIModelInfo {
			public uint Key { get; set; }
			public string Name { get; set; }
			public Var[] Vars { get; set; }

			public class Var {
				public string Name { get; set; }
				public AI_VarType Type { get; set; }
				public int ArrayLength { get; set; }
				public int ArrayDimensionsCount { get; set; }
			}
		}

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
			HashSet<uint> textKeys = new HashSet<uint>();

			// Key relocation (for writing as RRR mod)
			Dictionary<uint, uint> keysToRelocate = new Dictionary<uint, uint>();
			Dictionary<uint, uint> keysToRelocateReverse = new Dictionary<uint, uint>();
			HashSet<uint> keysToAvoid = new HashSet<uint>();
			LOA_Loader.ExportFilenameGuessData namingData = new LOA_Loader.ExportFilenameGuessData();
			Dictionary<uint, AIModelInfo> allowedAI = new Dictionary<uint, AIModelInfo>();
			Dictionary<uint, string[]> allowedProcs = new Dictionary<uint, string[]>();
			HashSet<uint> dontRelocateKeys = new HashSet<uint>();
			HashSet<uint> dontWriteKeys = new HashSet<uint>();

			bool exportForDifferentGameMode = targetMode.HasValue;// && settings.GameModeSelection != GameModeSelection.KingKongPC;


			void DontRelocateKey(uint key) {
				if(!dontRelocateKeys.Contains(key))
					dontRelocateKeys.Add(key);
			}
			void DontWriteKey(uint key) {
				if(!dontWriteKeys.Contains(key))
					dontWriteKeys.Add(key);
			}

			#region Key Relocation outside loader
			bool Raw_RelocateKeys = exportForDifferentGameMode;
			uint Raw_CurrentUnusedKey = 0xBB000000;
			uint Raw_GetNextUnusedKey() {
				while (true) {
					uint curKey = Raw_CurrentUnusedKey;
					if (!keysToAvoid.Contains(curKey)) {
						return curKey;
					}
					if (Raw_CurrentUnusedKey >= 0xF3FFFFFF) {
						Raw_CurrentUnusedKey = 0x01000000;
					} else {
						Raw_CurrentUnusedKey++;
					}
				}
			}
			uint Raw_RelocateKey(uint keyToRelocate) {
				if (keyToRelocate == 0 || keyToRelocate == 0xFFFFFFFF) return keyToRelocate;
				if (keysToRelocate.ContainsKey(keyToRelocate)) {
					return keysToRelocate[keyToRelocate];
				}
				var curKey = Raw_GetNextUnusedKey();

				keysToAvoid.Add(curKey);
				keysToRelocate[keyToRelocate] = curKey;
				keysToRelocateReverse[curKey] = keyToRelocate;
				return curKey;
			}
			uint Raw_RelocateKeyIfNecessary(uint key) {
				if (key == 0 || key == 0xFFFFFFFF) return key;
				if (Raw_RelocateKeys) {
					if (dontRelocateKeys.Contains(key)) {
						return key;
					} else if (keysToRelocate.ContainsKey(key)) {
						return keysToRelocate[key];
					} else if (keysToAvoid.Contains(key)) {
						return Raw_RelocateKey(key);
					}
					return Raw_RelocateKey(key);
				}
				return key;
			}
			#endregion

			int[] rrrPC_supportedModifiers = new int[] {
				-1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 18, 19,
				20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 36, 37, 38, 49,
				50, 51, 52, 53, 54, 55
			};

			if (inputDir != null) {
				DirectoryInfo keysDir = new DirectoryInfo(Path.Combine(inputDir,"Keys"));
				if (keysDir.Exists) {
					var keyFiles = keysDir.GetFiles("*.txt", SearchOption.TopDirectoryOnly);
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

				DirectoryInfo aimodelsDir = new DirectoryInfo(Path.Combine(inputDir, "AI", "Models"));
				if (aimodelsDir.Exists) {
					var aimodelsFiles = aimodelsDir.GetFiles("*.txt", SearchOption.TopDirectoryOnly);
					foreach (var f in aimodelsFiles) {
						var filename = Path.GetFileNameWithoutExtension(f.Name);

						var keystr = f.Name.Substring(0, f.Name.IndexOf("_"));
						uint key = uint.Parse(keystr, System.Globalization.NumberStyles.HexNumber);

						var name = f.Name.Substring(f.Name.IndexOf("_")+1);

						var aiModelValue = new AIModelInfo() {
							Name = name,
							Key = key,
						};
						allowedAI.Add(aiModelValue.Key, aiModelValue);

						List<AIModelInfo.Var> vars = new List<AIModelInfo.Var>();

						string[] lines = File.ReadAllLines(f.FullName);
						foreach (var l in lines) {
							var lineSplit = l.Split(',');
							if (lineSplit.Length < 4) continue;

							AIModelInfo.Var variable = new AIModelInfo.Var() {
								Name = lineSplit[0],
								Type = (AI_VarType)Enum.Parse(typeof(AI_VarType), lineSplit[1]),
								ArrayDimensionsCount = int.Parse(lineSplit[2]),
								ArrayLength = int.Parse(lineSplit[3])
							};
							vars.Add(variable);
						}
						aiModelValue.Vars = vars.ToArray();
					}
				}

				DirectoryInfo aiprocsDir = new DirectoryInfo(Path.Combine(inputDir, "AI", "Procs"));
				if (aiprocsDir.Exists) {
					var aiprocsFiles = aiprocsDir.GetFiles("*.txt", SearchOption.TopDirectoryOnly);
					foreach (var f in aiprocsFiles) {
						var filename = Path.GetFileNameWithoutExtension(f.Name);

						var keystr = f.Name.Substring(0, f.Name.IndexOf("_"));
						uint key = uint.Parse(keystr, System.Globalization.NumberStyles.HexNumber);

						var name = f.Name.Substring(f.Name.IndexOf("_") + 1);

						List<string> procs = new List<string>();

						string[] lines = File.ReadAllLines(f.FullName);
						foreach (var l in lines) {
							string triml = l.Trim();
							if(!string.IsNullOrEmpty(triml))
								procs.Add(triml);
						}
						allowedProcs.Add(key, procs.ToArray());
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
					// Also load filenames
					string filenamesPath = context.GetAbsoluteFilePath("filenames.txt");
					if (context.FileManager?.FileExists(filenamesPath) ?? false) {
						string[] lines = File.ReadAllLines(filenamesPath);
						foreach (var l in lines) {
							var lineSplit = l.Split(',');
							if (lineSplit.Length != 2) continue;

							if (uint.TryParse(lineSplit[0], System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.CurrentCulture, out uint k)) {
								var key = k;
								var path = lineSplit[1].Replace('\\', '/');
								var dir = "";
								var filename = path;
								if (path.Contains('/')) {
									var lastIndex = path.LastIndexOf('/');
									dir = path.Substring(0, lastIndex);
									filename = path.Substring(lastIndex + 1);
								}
								if (filename.Contains('.')) {
									filename = filename.Substring(0, filename.LastIndexOf('.'));
								}
								namingData.AddGuess(key, filename, dir, 0);
							}
						}
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
								namingData.AddGuess(w.Key, null, directory, 100f * (i + 1) / count);
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
								/*writeloader.Raw_RelocateKeys = true;
								writeloader.Raw_KeysToAvoid = keysToAvoid;
								writeloader.Raw_DontRelocateKeys = dontRelocateKeys;
								writeloader.Raw_DontWriteKeys = dontWriteKeys;
								writeloader.Raw_KeysToRelocate = keysToRelocate;*/
								writeloader.Raw_KeysToRelocateReverse = keysToRelocateReverse;
							}
							writeloader.WrittenFileKeys = writtenFileKeys;
							writeloader.TextKeys = textKeys;
							writeContext.StoreObject<LOA_Loader>(LoaderKey, writeloader);

							foreach (var wol in wols) {
								uint originalWolKey = wol.Key;
								var wkey = Raw_RelocateKeyIfNecessary(wol.Key);
								if (wkey != originalWolKey) {
									var originalFilename = namingData.GetMostLikelyFilename(originalWolKey);
									namingData.AddFact(wkey, originalFilename.Filename, originalFilename.Directory);
								}

								if (exportForDifferentGameMode) {
									// Relocate keys in wol...
									foreach (var wow in wol.Worlds) {
										if (!wow.Key.IsNull)
											wow.Key = new Jade_Key(writeContext, Raw_RelocateKeyIfNecessary(wow.Key));
									}
									// ... and add existing wows (_basic_global, _basic_Rayman)
									if (targetMode == GameModeSelection.RaymanRavingRabbidsPCPrototype) {
										const uint basicRaymanKey = 0x9E00DCD2;
										const uint basicGlobalKey = 0x9E007AEA;
										bool addBasicGlobal = false, addBasicRayman = false;

										if (context.GetR1Settings().EngineVersion != EngineVersion.Jade_RRR && context.GetR1Settings().EngineVersion != EngineVersion.Jade_RRRPrototype) {
											addBasicGlobal = true;
											addBasicRayman = true;
										} else {
											foreach (var wow in wol.Worlds) {
												if (!wow.Key.IsNull && keysToRelocateReverse.ContainsKey(wow.Key)) {
													switch (keysToRelocateReverse[wow.Key]) {
														case basicGlobalKey: addBasicGlobal = true; break;
														case basicRaymanKey: addBasicRayman = true; break;
													}
												}
											}
										}
										if (addBasicGlobal || addBasicRayman) {
											List<Jade_GenericReference> worlds = new List<Jade_GenericReference>();
											var validworlds = wol?.Worlds?.Where(w => !w.IsNull);
											if (addBasicGlobal) {
												worlds.Add(
													new Jade_GenericReference(writeContext,
													new Jade_Key(writeContext, basicGlobalKey),
													new Jade_FileType() { Extension = ".wow" }));
											}
											if (addBasicRayman) {
												worlds.Add(
													new Jade_GenericReference(writeContext,
													new Jade_Key(writeContext, basicRaymanKey),
													new Jade_FileType() { Extension = ".wow" }));

											}
											worlds.AddRange(validworlds);
											wol.Worlds = worlds.ToArray();
											wol.FileSize = (uint)wol.Worlds.Length * 8;
										}
									}
								}

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
				/*if (levIndex <= 15) {
					levIndex++;
					continue;
				}
				if (levIndex >= 30) {
					levIndex++;
					continue;
				}*/
				/*if (!(lev?.MapName.Contains("ilot") ?? false)) {
					levIndex++;
					continue;
				}*/

				/*if (!(lev?.MapName?.ToLower()?.Contains("fps") ?? false) && !(lev?.MapName?.ToLower()?.Contains("gladiator") ?? false)) {
					levIndex++;
					continue;
				}*/
				/*if (!(lev?.MapName?.ToLower()?.Contains("gladiator") ?? false)) {
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
						bool removeSound = (!context.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_KingKong)
										|| (targetMode != GameModeSelection.RaymanRavingRabbidsPCPrototype && targetMode != GameModeSelection.RaymanRavingRabbidsPC));


						currentKey = 0;
						await LoadFilesAsync(context);
						if (exportForDifferentGameMode) {
							var lf = LoadFlags.Maps | LoadFlags.Textures;
							lf |= LoadFlags.TextNoSound;
							if (!removeSound) {
								lf |= LoadFlags.TextSound | LoadFlags.Sounds;
							}
							await LoadJadeAsync(context, new Jade_Key(context, lev.Key), lf);
						} else {
							await LoadJadeAsync(context, new Jade_Key(context, lev.Key), LoadFlags.All);
						}

						LOA_Loader actualLoader = context.GetStoredObject<LOA_Loader>(LoaderKey);

						// RRR: Don't export audio files that already exist in the proto, as the proto's are sourced from the final anyway
						if (!removeSound && exportForDifferentGameMode && targetMode.Value == GameModeSelection.RaymanRavingRabbidsPCPrototype && context.GetR1Settings().EngineVersion == EngineVersion.Jade_RRR) {
							foreach (var cache in actualLoader.Caches) {
								foreach (var cachedObj in cache.Value) {
									var key = cachedObj.Key;
									var obj = cachedObj.Value;
									if (obj != null && obj.GetType() == typeof(SND_Wave) && keysToAvoid.Contains(key)) {
										DontRelocateKey(key);
										DontWriteKey(key);
									}
								}
							}
						}

						var worlds = actualLoader.LoadedWorlds;

						if (exportForDifferentGameMode) {
							if (targetMode.Value == GameModeSelection.RaymanRavingRabbidsPCPrototype
								&& context.GetR1Settings().EngineVersion == EngineVersion.Jade_RRR) {
								foreach (var w in worlds) {
									if (w.Name == "_basic_global" || w.Name == "_basic_Rayman") {
										w.Name += "_imported";
										var gaos = w.GameObjects?.Value?.GameObjects;
										if (gaos != null) {
											foreach (var gaoRef in gaos) {
												if (gaoRef?.Value != null) {
													if (keysToAvoid.Contains(gaoRef?.Value?.Key)) {
														DontRelocateKey(gaoRef?.Value?.Key);
														DontWriteKey(gaoRef?.Value?.Key);
													}
												}
											}
										}
									}
								}
							}
							foreach (var w in worlds) {
								//w.Text = new Jade_TextReference(context, new Jade_Key(context, 0xFFFFFFFF));
								foreach (var gao in w.SerializedGameObjects) {
									if (removeSound) {
										gao.FlagsIdentity &= ~OBJ_GameObject_IdentityFlags.Sound;
									}
									if (allowedAI.Any()) {
										if (gao.Extended?.AI?.Value != null) {
											// Check if AI is supported by target mode
											var aiInstance = gao.Extended?.AI?.Value;
											var aiModel = aiInstance.Model?.Value;
											if (aiModel != null) {
												if (allowedAI.ContainsKey(aiModel.Key)) {
													DontRelocateKey(aiModel.Key);
													DontWriteKey(aiModel.Key);
													// Supported, now check vars
													if (aiInstance?.Vars?.Value != null) {
														var vars = aiInstance?.Vars?.Value;
														var modelVars = allowedAI[aiModel.Key].Vars;
														foreach (var variable in vars.Vars) {
															void DoForAllValues(AI_Var variable, Action<AI_VarValue> action) {
																if (variable.Value.ValueArray != null) {
																	foreach (var val in variable.Value.ValueArray) {
																		action(val);
																	}
																}
																action(variable.Value);
															}

															var modelVar = modelVars.FirstOrDefault(mvar => variable.Name == mvar.Name);
															bool remove = false;
															if (modelVar != null) {
																if (modelVar.Type != variable.Type) {
																	if (modelVar.Type == AI_VarType.GAO && variable.Type == AI_VarType.Int) {
																		variable.Type = AI_VarType.GAO;
																		variable.Info.Type = 40; // TODO: Get this index from Links directly
																		DoForAllValues(variable, v => {
																			v.ValueKey = new Jade_Key(context, v.ValueUInt);
																		});
																	} else if (modelVar.Type == AI_VarType.Int && variable.Type == AI_VarType.GAO) {
																		variable.Type = AI_VarType.Int;
																		variable.Info.Type = 33; // TODO: Get this index from Links directly
																		DoForAllValues(variable, v => {
																			v.ValueUInt = v.ValueKey.Key;
																		});
																	} else {
																		remove = true;
																		Debug.Log($"Removing variable due to type mismatch: {variable.Name} - {modelVar.Type} - {variable.Type}");
																	}
																	// TODO: Change var types
																}
																if (modelVar.ArrayLength != variable.Info.ArrayLength || variable.Info.ArrayDimensionsCount != modelVar.ArrayDimensionsCount) {
																	// Resize if required
																	var varInfo = variable.Info;
																	var value = variable.Value;

																	// Resize dimensions
																	var dimensionsCountDiff = modelVar.ArrayDimensionsCount - variable.Info.ArrayDimensionsCount;
																	vars.VarValueBufferSize += (uint)(dimensionsCountDiff * 4);
																	if (modelVar.ArrayDimensionsCount != 0) {
																		if (value.Dimensions == null)
																			value.Dimensions = new uint[modelVar.ArrayDimensionsCount];
																		else {
																			var dims = value.Dimensions;
																			Array.Resize(ref dims, modelVar.ArrayDimensionsCount);
																			value.Dimensions = dims;
																		}
																		value.Dimensions[value.Dimensions.Length - 1] = (uint)modelVar.ArrayLength;
																		for (int i = 0; i < value.Dimensions.Length - 1; i++) {
																			value.Dimensions[i] = 1;
																		}
																		if (value.ValueArray == null) {
																			// Create array container for value
																			var dims = value.Dimensions;
																			value.Dimensions = null;
																			value.IsArrayElement = true;
																			value = new AI_VarValue() {
																				Var = variable,
																				IsArrayElement = false,
																				ValueArray = new AI_VarValue[1] {
																					value
																				},
																				Dimensions = dims
																			};
																			variable.Value = value;
																		}

																		// Actually resize var array
																		if (modelVar.ArrayLength != 1) {
																			var arr = value.ValueArray;
																			Array.Resize(ref arr, modelVar.ArrayLength);
																			for (int i = 0; i < arr.Length; i++) {
																				if (arr[i] != null) {
																					arr[i].IsArrayElement = true;
																				} else {
																					arr[i] = new AI_VarValue() {
																						Var = variable,
																						IsArrayElement = true
																					};
																				}
																			}
																			value.ValueArray = arr;
																		}
																	} else {
																		value.Dimensions = null;
																		// Remove array container for value
																		if (value.ValueArray != null) {
																			value = value.ValueArray[0];
																			value.Dimensions = null;
																			variable.Value = value;
																		}
																	}
																	var valueSizeDiff = modelVar.ArrayLength - variable.Info.ArrayLength;
																	vars.VarValueBufferSize += (uint)(valueSizeDiff * variable.Link.Size);
																	// Go over rest of infos
																	foreach (var otherVarInfo in vars.VarInfos) {
																		if (otherVarInfo.BufferOffset >= varInfo.BufferOffset) {
																			otherVarInfo.BufferOffset += valueSizeDiff;
																		}
																	}
																}
															} else {
																remove = true;
															}
															if (remove) {
																// Remove name
																var nameObject = vars.Names.FirstOrDefault(n => n.Name == variable.Name);
																vars.NameBufferSize -= (uint)nameObject.SerializedSize;
																var namesList = vars.Names.ToList();
																namesList.Remove(nameObject);
																vars.Names = namesList.ToArray();

																// Remove value
																var value = variable.Value;
																vars.VarValueBufferSize -= (uint)value.SerializedSize;
																var valuesList = vars.Values.ToList();
																valuesList.Remove(value);
																vars.Values = valuesList.ToArray();

																// Remove info
																var varInfo = variable.Info;
																vars.VarInfosBufferSize -= (uint)varInfo.SerializedSize;
																var varInfosList = vars.VarInfos.ToList();
																varInfosList.Remove(varInfo);
																vars.VarInfos = varInfosList.ToArray();
																// Go over rest of infos
																foreach (var otherVarInfo in vars.VarInfos) {
																	if (otherVarInfo.BufferOffset >= varInfo.BufferOffset) {
																		otherVarInfo.BufferOffset -= (int)value.SerializedSize;
																	}
																}

																// TODO: Editor infos
																continue;
															}

															// Go over int values (in arrays too!)
															// If the int matches any loaded key, relocate it in advance
															if (variable.Type == AI_VarType.Int) {
																void CheckInt(AI_VarValue val) {
																	if (val.ValueInt == -1 || val.ValueInt == 0) return;
																	// Key always has a user in the top byte
																	if ((val.ValueInt & 0xFF000000) == 0) return;

																	var keyUint = val.ValueUInt;
																	var key = new Jade_Key(context, val.ValueUInt);
																	if(key.Type != Jade_Key.KeyType.Unknown) return;

																	if (actualLoader.TotalCache.ContainsKey(key)) {
																		// Relocate key
																		uint newKey = Raw_RelocateKeyIfNecessary(keyUint);
																		val.ValueUInt = newKey;
																	}
																}
																DoForAllValues(variable, v => CheckInt(v));
															}

															// TODO: Relocate Text too

															// Make sure trigger values are not relocated!
															if (variable.Type == AI_VarType.Trigger) {
																void CheckTrigger(AI_VarValue val) {
																	var trg = val.ValueTrigger;
																	if (trg == null) return;
																	if (!trg.KeyFile.IsNull) {
																		if (allowedProcs.ContainsKey(trg.KeyFile) && allowedProcs[trg.KeyFile].Contains(trg.Name)) {
																			DontRelocateKey(trg.KeyFile);
																			DontWriteKey(trg.KeyFile);
																		} else {
																			trg.KeyFile = new Jade_Key(context, 0);
																			trg.CFunctionPointer = 0;
																			trg.Name = null;
																		}
																	}
																}
																DoForAllValues(variable, v => CheckTrigger(v));
															}
														}
													}
												} else {
													// Not supported
													gao.FlagsIdentity &= ~OBJ_GameObject_IdentityFlags.AI;
												}
											}
										}
									} else {
										gao.FlagsIdentity &= ~OBJ_GameObject_IdentityFlags.AI;
									}

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
										if (removeSound) {
											gao.Extended.Modifiers = gao.Extended.Modifiers
												.Where(m =>
													m.Type != MDF_ModifierType.GEN_ModifierSound
													&& m.Type != MDF_ModifierType.MDF_LoadingSound
													&& m.Type != MDF_ModifierType.GEN_ModifierSoundFx
													&& rrrPC_supportedModifiers.Contains((int)m.Type))
												.ToArray();
										} else {
											gao.Extended.Modifiers = gao.Extended.Modifiers
												.Where(m => rrrPC_supportedModifiers.Contains((int)m.Type))
												.ToArray();
										}
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
									var objects = world?.GameObjects?.Value?.GameObjects;
									if (objects != null && objects.Any()) {
										var gaoDir = $"{wowDir}/Game Objects";
										var cobDir = $"{wowDir}/Collision Objects";
										var cinDir = $"{wowDir}/COL Instances";
										var lnkDir = $"{wowDir}/Links";
										var aiDir  = $"{wowDir}/AI Instances";
										var rliDir = $"{wowDir}/Game Objects RLI";
										var grpDir = $"{wowDir}/Groups";
										var gaoPrio = 10000 / objects.Length;
										foreach (var objRef in objects) {
											var obj = objRef?.Value;
											if(obj == null) continue;
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
											if (obj.FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.AI)) {
												var aiModelName = obj?.Extended?.AI?.Value?.Model?.Value?.FunctionDef?.Name;
												if (aiModelName != null) {
													namingData.AddGuess(obj.Extended?.AI?.Key, objname, $"{aiDir}/{aiModelName}", gaoPrio);
													namingData.AddGuess(obj.Extended?.AI?.Value?.Vars?.Key, objname, $"{aiDir}/{aiModelName}", gaoPrio);
												} else {
													namingData.AddGuess(obj.Extended?.AI?.Key, objname, aiDir, gaoPrio);
													namingData.AddGuess(obj.Extended?.AI?.Value?.Vars?.Key, objname, aiDir, gaoPrio);
												}
											}
											namingData.AddGuess(obj.Extended?.Group?.Key, objname, grpDir, gaoPrio);
											namingData.AddGuess(obj.Extended?.Group?.Value?.GroupObjectList?.Key, objname, grpDir, gaoPrio);
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
										loader.Raw_DontRelocateKeys = dontRelocateKeys;
										loader.Raw_DontWriteKeys = dontWriteKeys;
										loader.Raw_KeysToRelocate = keysToRelocate;
										loader.Raw_KeysToRelocateReverse = keysToRelocateReverse;
									}
									loader.Raw_FilenameGuesses = namingData;

									loader.WrittenFileKeys = writtenFileKeys;
									loader.TextKeys = textKeys;
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
									loader.Raw_DontRelocateKeys = dontRelocateKeys;
									loader.Raw_DontWriteKeys = dontWriteKeys;
									loader.Raw_KeysToRelocate = keysToRelocate;
									loader.Raw_KeysToRelocateReverse = keysToRelocateReverse;
								}
								loader.Raw_FilenameGuesses = namingData;

								loader.WrittenFileKeys = writtenFileKeys;
								loader.TextKeys = textKeys;
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

			b = new StringBuilder();
			foreach (var tk in textKeys) {
				b.AppendLine($"{tk:X8}");
			}
			File.WriteAllText(Path.Combine(outputDir, "textkeys.txt"), b.ToString());

			Debug.Log($"Finished export");
		}
	}
}
