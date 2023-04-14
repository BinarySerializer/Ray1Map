using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using System.IO;

namespace Ray1Map.Jade {
	public class LOA_Loader {
		public Context Context { get; set; }
		public bool IsLoadingFix { get; set; }
		public LOA_SpecialArray SpecialArray { get; set; }

		public BIG_BigFile[] BigFiles { get; set; }
		public Dictionary<QueueType, LinkedList<FileReference>> LoadQueues = new Dictionary<QueueType, LinkedList<FileReference>>();// = new Queue<FileReference>();
		public LinkedList<FileReference> LoadQueue => LoadQueues[CurrentQueueType];
		public Dictionary<Jade_Key, FileInfo> FileInfos { get; private set; }
		public Dictionary<CacheType, Dictionary<Jade_Key, Jade_File>> Caches { get; private set; } = new Dictionary<CacheType, Dictionary<Jade_Key, Jade_File>>();
		public Dictionary<Jade_Key, Jade_File> TotalCache { get; private set; } = new Dictionary<Jade_Key, Jade_File>();
		public Dictionary<Jade_Key, Jade_File> Cache => Caches[CurrentCacheType];
		public bool IsBinaryData => SpeedMode && ReadMode == Read.Binary;
		public bool SpeedMode { get; set; } = true;
		public Read ReadMode { get; set; } = Read.Full;
		public bool IsCompressed { get; set; } = false;
		public bool ReadBinFileHeader { get; set; } = false;
		public BinData Bin { get; set; }
		public QueueType CurrentQueueType { get; set; } = QueueType.BigFat;
		public CacheType CurrentCacheType { get; set; } = CacheType.Main;
		public bool LoadSingle { get; set; } = false;

		public Jade_Reference<AI_Instance> Universe { get; set; }

		// Writing
		public SerializeMode SerializerMode { get; set; } = LOA_Loader.SerializeMode.Read;
		public Dictionary<uint, string> WrittenFileKeys { get; set; } = new Dictionary<uint, string>();
		public HashSet<uint> TextKeys { get; set; } = new HashSet<uint>();
		public bool Raw_WriteFilesAlreadyInBF { get; set; } = false;
		public bool Raw_RelocateKeys { get; set; } = false;
		public bool Raw_UseOriginalFileNames { get; set; } = false;
		public ExportFilenameGuessData Raw_FilenameGuesses { get; set; }
		public HashSet<uint> Raw_DontRelocateKeys { get; set; } = new HashSet<uint>();
		public HashSet<uint> Raw_DontWriteKeys { get; set; } = new HashSet<uint>();
		public Dictionary<uint, uint> Raw_KeysToRelocate { get; set; } = new Dictionary<uint, uint>();
		public Dictionary<uint, uint> Raw_KeysToRelocateReverse { get; set; } = new Dictionary<uint, uint>();
		public HashSet<uint> Raw_KeysToAvoid { get; set; } = new HashSet<uint>();
		public uint Raw_CurrentUnusedKey { get; set; } = 0xBB000000;
		public uint Raw_GetNextUnusedKey() {
			while (true) {
				uint curKey = Raw_CurrentUnusedKey;
				if (!Raw_KeysToAvoid.Contains(curKey)) {
					return curKey;
				}
				if (Raw_CurrentUnusedKey >= 0xF3FFFFFF) {
					Raw_CurrentUnusedKey = 0x01000000;
				} else {
					Raw_CurrentUnusedKey++;
				}
			}
		}
		public uint Raw_RelocateKey(uint keyToRelocate) {
			if(keyToRelocate == 0 || keyToRelocate == 0xFFFFFFFF) return keyToRelocate;
			if (Raw_KeysToRelocate.ContainsKey(keyToRelocate)) {
				return Raw_KeysToRelocate[keyToRelocate];
			}
			var curKey = Raw_GetNextUnusedKey();
			
			Raw_KeysToAvoid.Add(curKey);
			Raw_KeysToRelocate[keyToRelocate] = curKey;
			Raw_KeysToRelocateReverse[curKey] = keyToRelocate;
			return curKey;
		}
		public uint Raw_RelocateKeyIfNecessary(uint key) {
			if (key == 0 || key == 0xFFFFFFFF) return key;
			if (Raw_RelocateKeys) {
				if (Raw_DontRelocateKeys.Contains(key)) {
					return key;
				} else if (Raw_KeysToRelocate.ContainsKey(key)) {
					return Raw_KeysToRelocate[key];
				} else if (Raw_KeysToAvoid.Contains(key)) {
					return Raw_RelocateKey(key);
				}
			}
			return key;
		}
		public uint Raw_GetNewKey() => Raw_RelocateKey(Raw_CurrentUnusedKey);
		public void AddTextKey(uint key) {
			if(!TextKeys.Contains(key)) TextKeys.Add(key);
		}

		// Loaded objects
		public WOR_World WorldToLoadIn { get; set; }
		public WOR_World CurWorldForGrids { get; set; }
		public List<WOR_World> LoadedWorlds { get; set; } = new List<WOR_World>();
		public List<OBJ_GameObject> AttachedGameObjects { get; set; } = new List<OBJ_GameObject>();
		public bool IsGameObjectAttached(OBJ_GameObject gao) => AttachedGameObjects.Any(obj => obj.Key == gao.Key);

		public class ExportFilenameGuessData {
			public Dictionary<uint, List<ExportFilenameGuess>> Guesses { get; set; } = new Dictionary<uint, List<ExportFilenameGuess>>();
			public Dictionary<uint, ExportFilenameGuess> Facts { get; set; } = new Dictionary<uint, ExportFilenameGuess>();

			public void AddGuess(uint key, string filename, string directory, float priority) {
				if (key == 0 || key == 0xFFFFFFFF) return;
				if (!Guesses.ContainsKey(key))
					Guesses[key] = new List<ExportFilenameGuess>();

				Guesses[key].Add(new ExportFilenameGuess() {
					Filename = filename,
					Directory = directory,
					Priority = priority,
				});
			}

			public bool HasGuess(uint key) {
				if (key == 0 || key == 0xFFFFFFFF) return true;
				if (!Guesses.ContainsKey(key)) return false;
				if(Guesses[key].Any()) return true;
				return false;
			}

			public void AddFact(uint key, string filename, string directory) {
				if(key == 0 || key == 0xFFFFFFFF) return; 
				Facts[key] = new ExportFilenameGuess() {
					Filename = filename,
					Directory = directory,
					Priority = float.MaxValue,
				};
			}

			public ExportFilenameGuess GetMostLikelyFilename(uint key) {
				if (key == 0 || key == 0xFFFFFFFF) return null;
				if (Facts.ContainsKey(key)) return Facts[key];
				if (Guesses.ContainsKey(key)) {
					var orderedGuesses = Guesses[key].OrderByDescending(g => g.Priority);
					var bestGuessFilename = orderedGuesses.FirstOrDefault(g => g.Filename != null);
					var bestGuessDirectory = orderedGuesses.FirstOrDefault(g => g.Directory != null);
					if (bestGuessFilename != bestGuessDirectory) {
						return new ExportFilenameGuess() {
							Directory = bestGuessDirectory?.Directory,
							Filename = bestGuessFilename?.Filename,
							Priority = bestGuessFilename?.Priority ?? bestGuessDirectory?.Priority ?? 0,
						};
					}
					return bestGuessFilename;
				}
				return null;
			}
		}
		public class ExportFilenameGuess {
			public string Filename { get; set; }
			public string Directory { get; set; }
			public float Priority { get; set; }
		}
		public class BinData {
			public Jade_Key Key { get; set; }
			public Pointer StartPosition { get; set; }
			public Pointer CurrentPosition { get; set; }
			public uint TotalSize { get; set; }
			public SerializerObject Serializer { get; set; }
			public QueueType QueueType { get; set; }
			public Func<SerializerObject, UniTask> SerializeAction { get; set; }
		}
		public enum QueueType {
			BigFat,
			Current,
			Maps,
			TextSound,
			TextNoSound,
			Sound,
			Textures
		}
		public enum CacheType {
			Main,
			Fix,
			Current,
			TextureInfo,
			TextureContent,
			Text,
			TextSound
		}

		public enum Read {
			Full = 0,
			Binary = 2,
		}

		public enum SerializeMode {
			Read,
			Write
		}

		[Flags]
		public enum ReferenceFlags : uint {
			None = 0,
			MustExist = 1 << 0,
			Flag1 = 1 << 1,
			DontCache = 1 << 2,
			TmpAlloc = 1 << 3,
			HasUserCounter = 1 << 4,
			OnlyOneRef = 1 << 5,
			Flag6 = 1 << 6,
			Flag7 = 1 << 7,
			
			// Custom
			IsIrregularFileFormat = 1 << 16,
			DontUseCachedFile = 1 << 17,
			Montreal_AllowSkip = 1 << 18,
			Montreal_NoKeyChecks = 1 << 19,
		}


		public LOA_Loader(BIG_BigFile[] bigFiles, Context context) {
			Context = context;
			BigFiles = bigFiles;
			CreateFileDictionaries();
			foreach (QueueType queue in (QueueType[])Enum.GetValues(typeof(QueueType))) {
				LoadQueues[queue] = new LinkedList<FileReference>();
			}
			foreach (CacheType cache in (CacheType[])Enum.GetValues(typeof(CacheType))) {
				Caches[cache] = new Dictionary<Jade_Key, Jade_File>();
			}
		}

		private void CreateFileDictionaries() {
			FileInfos = new Dictionary<Jade_Key, FileInfo>();
			for (int b = 0; b < BigFiles.Length; b++) {
				var big = BigFiles[b];
				string[] directories = new string[0];
				for (int f = 0; f < big.FatFiles.Length; f++) {
					var fat = big.FatFiles[f];

					// Create directories list
					var fatForDirectories = fat;
					if (fatForDirectories.DirectoryInfos?.Length > 0) {
						directories = new string[fatForDirectories.DirectoryInfos.Length];
						for (int i = 0; i < directories.Length; i++) {
							var dir = fatForDirectories.DirectoryInfos[i];
							var dirName = dir.Name;
							var curDir = dir;
							while (curDir.Parent != -1) {
								curDir = fatForDirectories.DirectoryInfos[curDir.Parent];
								dirName = $"{curDir.Name}/{dirName}";
							}
							directories[i] = dirName;
						}
					}


					for (int fi = 0; fi < fat.Files.Length; fi++) {
						var file = fat.Files[fi];
						var fileInfo = new FileInfo() {
							Key = file.Key,
							FileOffset = file.FileOffset,
							FileIndex = fi,
							FatFile = fat,
							FatFileInfo = fat.FileInfos[fi]
						};
						if (fileInfo.FatFileInfo.Name != null) {
							if (fileInfo.FatFileInfo.ParentDirectory < -1 || fileInfo.FatFileInfo.ParentDirectory >= directories.Length) {
								throw new Exception($"Parent directory index out of bounds: {fileInfo.FatFileInfo.ParentDirectory}. Directory count: {directories.Length}");
							}
							if (fileInfo.FatFileInfo.ParentDirectory != -1)
								fileInfo.DirectoryName = directories[fileInfo.FatFileInfo.ParentDirectory];
							else 
								fileInfo.DirectoryName = "";
							fileInfo.FileName = fileInfo.FatFileInfo.Name;
						}
						FileInfos[file.Key] = fileInfo;
					}
				}
			}
		}

		public void ReinitFileDictionaries() => CreateFileDictionaries();

		public async UniTask LoadLoop(SerializerObject s) {
			if (s is BinaryDeserializer) {
				SerializerMode = SerializeMode.Read;
			} else if (s is BinarySerializer.BinarySerializer) {
				SerializerMode = SerializeMode.Write;
				await LoadLoop_RawFilesWrite(s);
				return;
			}
			CurrentQueueType = QueueType.BigFat;
			CurrentCacheType = IsLoadingFix ? CacheType.Fix : CacheType.Main;

			while (LoadQueue.First?.Value != null) {
				FileReference currentRef = LoadQueue.First.Value;
				LoadQueue.RemoveFirst();
				if (currentRef.IsSizeRequest) {
					if (currentRef.Key != null && FileInfos.ContainsKey(currentRef.Key)) {
						uint fileSize = currentRef.Size_CurrentValue;
						if (!currentRef.IsBin && Cache.ContainsKey(currentRef.Key) && !currentRef.Flags.HasFlag(ReferenceFlags.DontUseCachedFile)) {
							var f = Cache[currentRef.Key];
							if (f != null) {
								fileSize = f.FileSize;
								currentRef.Size_ResolveAction(fileSize);
							}
						} else {
							Pointer off_current = s.CurrentPointer;
							FileInfo f = FileInfos[currentRef.Key];
							Pointer off_target = f.FileOffset;
							s.Goto(off_target);
							s.Log("LOA: Loading file: {0}", f);
							string previousState = Controller.DetailedState;
							Controller.DetailedState = $"{previousState}\n{f}";
							await s.FillCacheForReadAsync(4);
							fileSize = s.Serialize<uint>(default, name: "FileSize");
							s.Goto(off_current);
							currentRef.Size_ResolveAction(fileSize);
						}
					}
					continue;
				}
				if (currentRef.Key != null && FileInfos.ContainsKey(currentRef.Key)) {
					CurrentCacheType = currentRef.Cache;
					if (!currentRef.IsBin && Cache.ContainsKey(currentRef.Key) && !currentRef.Flags.HasFlag(ReferenceFlags.DontUseCachedFile)) {
						var f = Cache[currentRef.Key];
						if (f != null) f.CachedCount++;
						if (currentRef.Flags.HasFlag(ReferenceFlags.HasUserCounter) && f != null) f.ReferencesCount++;
						if (!currentRef.Flags.HasFlag(ReferenceFlags.OnlyOneRef)) currentRef.AlreadyLoadedCallback(f);
					} else {
						Pointer off_current = s.CurrentPointer;
						FileInfo f = FileInfos[currentRef.Key];
						Pointer off_target = f.FileOffset;
						s.Goto(off_target);
						s.Log("LOA: Loading file: {0}", f);
						string previousState = Controller.DetailedState;
						Controller.DetailedState = $"{previousState}\n{f}";
						await s.FillCacheForReadAsync(4);
						var fileSize = s.Serialize<uint>(default, name: "FileSize");

						string regionName = f.FileRegionName ?? $"{currentRef.Name}_{currentRef.Key:X8}";
						if (fileSize != 0) {
							await s.FillCacheForReadAsync(fileSize);

							// Add region
							off_target.File.AddRegion(off_target.FileOffset + 4, fileSize, regionName);
						}

						if (currentRef.IsBin && Bin != null) {
							if (IsCompressed) {
								Bin.StartPosition = s.BeginEncoded(new Jade_Lzo1xEncoder(fileSize, xbox360Version: s.GetR1Settings().EngineFlags.HasFlag(EngineFlags.Jade_Xenon)),
									filename: regionName);
								Bin.CurrentPosition = Bin.StartPosition;
								s.Goto(Bin.StartPosition);
								uint decompressedLength = s.CurrentLength32;
								Bin.Serializer = s;
								Bin.TotalSize = decompressedLength;
								if (Bin?.SerializeAction != null) {
									await Bin.SerializeAction(s);
								} else {
									await LoadLoopBINAsync();
								}
								//LoadLoopBIN_End();
								s.EndEncoded(Bin.CurrentPosition);
							} else {
								Bin.Serializer = s;
								Bin.TotalSize = fileSize;
								if (Bin?.SerializeAction != null) {
									await Bin.SerializeAction(s);
								} else {
									await LoadLoopBINAsync();
								}
							}
						} else {
							currentRef.LoadCallback(s, (f) => {
								f.Key = currentRef.Key;
								f.FileSize = fileSize;
								f.Loader = this;
								if (!(f is WOR_WorldList) && !TotalCache.ContainsKey(f.Key)) {
									TotalCache[f.Key] = f;
								}
								if (!Cache.ContainsKey(f.Key) && !currentRef.Flags.HasFlag(ReferenceFlags.DontCache)) Cache[f.Key] = f;
								f.SetIsBinaryData();
							});
						}

						await Controller.WaitIfNecessary();
						Controller.DetailedState = previousState;
						s.Goto(off_current);
					}
				} else if (currentRef.Flags.HasFlag(ReferenceFlags.MustExist)) {
					s.SystemLogger?.LogWarning($"File {currentRef.Name}_{currentRef.Key:X8} was not found");
				}
				if(LoadSingle)
					LoadQueue.Clear();
			}
		}

		public async UniTask LoadLoop_RawFiles(SerializerObject s, Dictionary<uint, string> keyList) {
			CurrentQueueType = QueueType.BigFat;
			CurrentCacheType = IsLoadingFix ? CacheType.Fix : CacheType.Main;

			while (LoadQueue.First?.Value != null) {
				FileReference currentRef = LoadQueue.First.Value;
				LoadQueue.RemoveFirst();
				if(currentRef.IsSizeRequest) continue;
				if (currentRef.Key != null && keyList.ContainsKey(currentRef.Key.Key)) {
					CurrentCacheType = currentRef.Cache;
					if (!currentRef.IsBin && Cache.ContainsKey(currentRef.Key) && !currentRef.Flags.HasFlag(ReferenceFlags.DontUseCachedFile)) {
						var f = Cache[currentRef.Key];
						if (f != null) f.CachedCount++;
						if (currentRef.Flags.HasFlag(ReferenceFlags.HasUserCounter) && f != null) f.ReferencesCount++;
						if (!currentRef.Flags.HasFlag(ReferenceFlags.OnlyOneRef)) currentRef.AlreadyLoadedCallback(f);
					} else {
						Pointer off_current = s.CurrentPointer;
						string filename = keyList[currentRef.Key.Key];
						var f = await Context.AddLinearFileAsync(filename);
						s.Log("LOA: Loading file: {0}", f);
						Pointer off_target = f.StartPointer;
						s.Goto(off_target);
						string previousState = Controller.DetailedState;
						Controller.DetailedState = $"{previousState}\n{f}";
						var fileSize = s.CurrentLength32;

						if (currentRef.IsBin && Bin != null) {
							throw new NotImplementedException($"Loading raw bin files is not supported");
						} else {
							currentRef.LoadCallback(s, (f) => {
								f.Key = currentRef.Key;
								f.FileSize = fileSize;
								f.Loader = this;
								f.Export_OriginalFilename = filename;
								if (!(f is WOR_WorldList) && !TotalCache.ContainsKey(f.Key)) {
									TotalCache[f.Key] = f;
								}
								if (!Cache.ContainsKey(f.Key) && !currentRef.Flags.HasFlag(ReferenceFlags.DontCache)) Cache[f.Key] = f;
								f.SetIsBinaryData();
							});
						}

						await Controller.WaitIfNecessary();
						Controller.DetailedState = previousState;
						s.Goto(off_current);
					}
				} else if (currentRef.Flags.HasFlag(ReferenceFlags.MustExist)) {
					s.SystemLogger?.LogWarning($"File {currentRef.Name}_{currentRef.Key:X8} was not found");
				}
				if (LoadSingle)
					LoadQueue.Clear();
			}
		}

		public async UniTask LoadLoop_RawFilesWrite(SerializerObject s) {
			CurrentQueueType = QueueType.BigFat;
			CurrentCacheType = IsLoadingFix ? CacheType.Fix : CacheType.Main;

			while (LoadQueue.First?.Value != null) {
				FileReference currentRef = LoadQueue.First.Value;
				LoadQueue.RemoveFirst();
				if (currentRef.IsSizeRequest) continue;
				if (currentRef.Key != null && !Raw_DontWriteKeys.Contains(currentRef.Key.Key) && !WrittenFileKeys.ContainsKey(currentRef.Key.Key) && currentRef.CurrentValue != null) {
					CurrentCacheType = currentRef.Cache;
					if (!currentRef.IsBin && Cache.ContainsKey(currentRef.Key)) {
						var f = Cache[currentRef.Key];
						if (f != null) f.CachedCount++;
					} else {
						//Pointer off_current = s.CurrentPointer;
						string filename = $"ROOT/Bin/{currentRef.Key.Key:X8}";
						string extension = null;
						string newFilename = null;
						string originalFilename = null;

						string previousState = Controller.DetailedState;
						Controller.DetailedState = $"{previousState}\n{filename}";

						using (MemoryStream memStream = new MemoryStream()) {
							// Stream key
							string key = filename;

							// Add the stream
							StreamFile streamFile = new StreamFile(
								context: Context,
								name: key,
								stream: memStream,
								endianness: Endian.Little);

							try {
								Context.AddFile(streamFile);

								Pointer off_target = streamFile.StartPointer;
								s.Goto(off_target);

								currentRef.LoadCallback(s, (f) => {
									f.Loader = this;
									extension = f.Export_Extension;
									newFilename = f.Export_FileBasename;
									originalFilename = f.Export_OriginalFilename;
									if (!Cache.ContainsKey(f.Key) && !currentRef.Flags.HasFlag(ReferenceFlags.DontCache)) Cache[f.Key] = f;
									f.SetIsBinaryData();
								});
							} finally {
								if (Raw_WriteFilesAlreadyInBF || !FileInfos.ContainsKey(currentRef.Key)) {
									var bytes = memStream.ToArray();
									if (!Raw_UseOriginalFileNames || !FileInfos.ContainsKey(currentRef.Key)) {
										if (originalFilename != null) {
											filename = originalFilename;
										} else {
											if (newFilename != null) filename += $"_{MakeValidFileName(newFilename)}";

											if (Raw_FilenameGuesses != null) {
												var usedKey = currentRef.Key.Key;

												// Names are assigned pre-relocation
												if (Raw_KeysToRelocateReverse.ContainsKey(usedKey))
													usedKey = Raw_KeysToRelocateReverse[usedKey];
												var bestGuess = Raw_FilenameGuesses.GetMostLikelyFilename(usedKey);
												if (bestGuess != null) {
													string dir = bestGuess?.Directory ?? $"ROOT/Bin";
													string fname = bestGuess ?.Filename ?? Path.GetFileName(filename);
													fname = string.Join("_", fname.Split(Path.GetInvalidFileNameChars()));

													filename = $"{dir}/{fname}";
												}
											}

											if (extension != null) filename += $".{extension}";
										}
									} else {
										filename = FileInfos[currentRef.Key].FilePathValidCharacters;
									}
									var outFilename = $"{Context.BasePath}files/{filename}";
									if (File.Exists(outFilename)) {
										Context.SystemLogger?.LogInfo($"File already exists: {outFilename}");

										// Append key to filename
										var filenameOnly = filename.Replace("\\","/");
										var dirname = "";
										var slashIndex = filenameOnly.LastIndexOf("/");
										if(slashIndex != -1) {
											dirname = filenameOnly.Substring(0, slashIndex + 1);
											filenameOnly = filenameOnly.Substring(slashIndex + 1);
										}
										filename = $"{dirname}{currentRef.Key.Key:X8}_{filenameOnly}";
										outFilename = $"{Context.BasePath}files/{filename}";
									}
									if (outFilename.Length >= 260) {
										Context.SystemLogger?.LogInfo($"Filename length exceeds MAX_PATH: {outFilename}");
									}
									Util.ByteArrayToFile(outFilename, bytes);
									if (FileInfos.ContainsKey(currentRef.Key)) {
										File.SetLastWriteTime(outFilename, FileInfos[currentRef.Key].FatFileInfo.DateLastModified);
									}
									WrittenFileKeys[currentRef.Key.Key] = filename;
									Context.RemoveFile(streamFile);
								}
							}
						}
						await Controller.WaitIfNecessary();
						Controller.DetailedState = previousState;
						//s.Goto(off_current);
					}
				} else if (currentRef.Flags.HasFlag(ReferenceFlags.MustExist)) {
					//s.LogWarning($"File {currentRef.Name}_{currentRef.Key:X8} was not found");
				}
				if (LoadSingle)
					LoadQueue.Clear();
			}
		}
		private static string MakeValidFileName(string name) {
			string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
			string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

			return System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "_");
		}

		public void LoadLoopBIN() {
			if (Bin != null) {
				CurrentQueueType = Bin.QueueType;
			} else {
				CurrentQueueType = QueueType.BigFat;
				return;
			}
			CurrentCacheType = IsLoadingFix ? CacheType.Fix : CacheType.Main;
			SerializerObject s = Bin.Serializer;
			var curPointer = s.CurrentPointer;
			if (Bin.CurrentPosition == null) Bin.CurrentPosition = curPointer;
			if (Bin.StartPosition == null) Bin.StartPosition = curPointer;
			while (LoadQueue.First?.Value != null) {
				FileReference currentRef = LoadQueue.First.Value;
				LoadQueue.RemoveFirst();
				LoadLoopBIN_ResolveReference(currentRef);
			}
			s.Goto(curPointer);
		}
		public async UniTask LoadBinOrNot(SerializerObject s) {
			if(IsBinaryData)
				await LoadLoopBINAsync();
			else
				await LoadLoop(s);
		}
		public async UniTask LoadLoopBINAsync() {
			if (Bin != null) {
				CurrentQueueType = Bin.QueueType;
			} else {
				CurrentQueueType = QueueType.BigFat;
				return;
			}
			SerializerObject s = Bin.Serializer;
			var curPointer = s.CurrentPointer;
			if (Bin.CurrentPosition == null) Bin.CurrentPosition = curPointer;
			if (Bin.StartPosition == null) Bin.StartPosition = curPointer;
			while (LoadQueue.First?.Value != null) {
				FileReference currentRef = LoadQueue.First.Value;
				LoadQueue.RemoveFirst();
				string previousState = Controller.DetailedState;
				Controller.DetailedState = $"{previousState}\n{currentRef.Name}_{currentRef.Key:X8}";
				await Controller.WaitIfNecessary();
				LoadLoopBIN_ResolveReference(currentRef);
				Controller.DetailedState = previousState;
			}
			s.Goto(curPointer);
		}

		private void LoadLoopBIN_End() {
			if (Context.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal)) {
				if (Bin != null) {
					CurrentQueueType = Bin.QueueType;
				} else {
					CurrentQueueType = QueueType.BigFat;
					return;
				}
				SerializerObject s = Bin.Serializer;
				var curPointer = s.CurrentPointer;
				if (Bin.CurrentPosition == null) Bin.CurrentPosition = curPointer;
				if (Bin.StartPosition == null) Bin.StartPosition = curPointer;
				s.Goto(Bin.CurrentPosition);
				LOA_BinFileHeader BinTerminator = s.SerializeObject<LOA_BinFileHeader>(default, name: nameof(BinTerminator));
				Bin.CurrentPosition = s.CurrentPointer;
				s.Goto(curPointer);
			}
		}

		private bool GetLoadedFile(Jade_Key key, out Jade_File loadedFile) {
			if (Cache.ContainsKey(key)) {
				loadedFile = Cache[key];
				return true;
			}
			if (!IsLoadingFix && SpecialArray != null) {
				if (SpecialArray.Lookup.Contains(key) && Caches[CacheType.Fix].ContainsKey(key)) {
					loadedFile = Caches[CacheType.Fix][key];
					return true;
				}
			}
			loadedFile = null;
			return false;
		}

		private void LoadLoopBIN_ResolveReference(FileReference currentRef) {
			SerializerObject s = Bin.Serializer;
			LOA_BinFileHeader BinFileHeader = null;
			uint FileSize = 0;
			s.Goto(Bin.CurrentPosition);
			CurrentCacheType = currentRef.Cache;

			bool hasLoadedFile = GetLoadedFile(currentRef.Key, out Jade_File loadedFile);

			if (currentRef.IsSizeRequest) {
				FileSize = currentRef.Size_CurrentValue;
				FileSize = s.Serialize<uint>(FileSize, name: "FileSize");
				currentRef.Size_ResolveAction(FileSize);
				Bin.CurrentPosition = s.CurrentPointer;
				return;
			}
			if (hasLoadedFile && !currentRef.Flags.HasFlag(ReferenceFlags.DontUseCachedFile)) {
				var f = loadedFile;
				if (f != null) f.CachedCount++;
				if (currentRef.Flags.HasFlag(ReferenceFlags.HasUserCounter) && f != null) f.ReferencesCount++;
				if (!currentRef.Flags.HasFlag(ReferenceFlags.OnlyOneRef)) currentRef.AlreadyLoadedCallback(f);
			} else {
				/*if (previouslyCached.Contains(currentRef.Key)) {
					Context.Logger?.Log($"Reserializing: {currentRef.Key}");
				}*/
				if (!currentRef.Flags.HasFlag(ReferenceFlags.IsIrregularFileFormat)) {
					if (ReadBinFileHeader) {
						BinFileHeader = s.SerializeObject<LOA_BinFileHeader>(BinFileHeader, name: nameof(BinFileHeader));

						if (BinFileHeader.Key != null) {
							if (BinFileHeader.Key != currentRef.Key) {
								if (currentRef.Flags.HasFlag(ReferenceFlags.Montreal_AllowSkip)) {
									// Activate this warning in case of weird texture problems
									//s.LogWarning($"BinFileHeader Key {BinFileHeader.Key} does not match Expected Key {currentRef.Key}. Skipping!");
									s.Goto(Bin.CurrentPosition);
									return;
								} else if (currentRef.Flags.HasFlag(ReferenceFlags.Montreal_NoKeyChecks)) {
									currentRef.Key = BinFileHeader.Key;
								} else {
									s.SystemLogger?.LogWarning($"BinFileHeader Key {BinFileHeader.Key} does not match Expected Key {currentRef.Key}");
								}
							}
						}

						Bin.CurrentPosition = s.CurrentPointer;
						FileSize = BinFileHeader.FileSize;

						// Add region
						Bin.CurrentPosition.File.AddRegion(Bin.CurrentPosition.FileOffset, BinFileHeader.FileSize, $"{currentRef.Name}_{currentRef.Key:X8}");

						Bin.CurrentPosition = Bin.CurrentPosition + BinFileHeader.FileSize;

					} else {
						FileSize = Bin.TotalSize - (uint)(Bin.CurrentPosition - Bin.StartPosition);
					}
				}
				currentRef.LoadCallback(s, (f) => {
					f.Key = currentRef.Key;
					f.FileSize = FileSize;
					f.Loader = this;
					f.BinFileHeader = BinFileHeader;
					if (!(f is WOR_WorldList) && !TotalCache.ContainsKey(f.Key)) {
						TotalCache[f.Key] = f;
					}
					if (!Cache.ContainsKey(f.Key) && !currentRef.Flags.HasFlag(ReferenceFlags.DontCache)) Cache[f.Key] = f;
					f.SetIsBinaryData();
				});

				if (ReadBinFileHeader && !currentRef.Flags.HasFlag(ReferenceFlags.IsIrregularFileFormat)) {
					s.Goto(Bin.CurrentPosition); // count size uint and actual file
				} else {
					Bin.CurrentPosition = s.CurrentPointer;
				}

			}
		}

		//public List<Jade_Key> previouslyCached = new List<Jade_Key>();
		public void RemoveCacheReference(Jade_Key key, bool all = false) {
			var cache = Cache;
			if (SpecialArray != null && SpecialArray.Lookup.Contains(key)) return;
			if (cache.ContainsKey(key)) {
				var file = cache[key];
				file.CachedCount--;
				if (all) file.CachedCount = 0;
				if (file.CachedCount == 0) cache.Remove(key);
				//if (file.CachedCount == 0) previouslyCached.Add(key);
			}
		}

		public delegate void ResolveAction(SerializerObject s, Action<Jade_File> configureAction);
		public delegate void ResolvedAction(Jade_File f);
		public delegate void FileSizeResolveAction(uint size);
		public class FileReference {
			public string Name { get; set; }
			public Jade_Key Key { get; set; }
			public Jade_File CurrentValue { get; set; }
			public ResolveAction LoadCallback { get; set; }
			public ResolvedAction AlreadyLoadedCallback { get; set; }
			public bool IsBin { get; set; }
			public ReferenceFlags Flags { get; set; }
			public CacheType Cache { get; set; }

			// Size
			public bool IsSizeRequest { get; set; }
			public FileSizeResolveAction Size_ResolveAction { get; set; }
			public uint Size_CurrentValue { get; set; }
		}

		public void RequestFile(Jade_Key key, Jade_File currentValue, ResolveAction loadCallback, ResolvedAction alreadyLoadedCallback, bool immediate = false,
			QueueType queue = QueueType.Current,
			CacheType cache = CacheType.Current,
			string name = "", ReferenceFlags flags = ReferenceFlags.MustExist) {
			if (queue == QueueType.Current) {
				queue = Bin?.QueueType ?? QueueType.BigFat;
				//if(Bin != null && FileInfos.ContainsKey(key)) queue = QueueType.BigFat;
			}
			if (cache == CacheType.Current) {
				cache = IsLoadingFix ? CacheType.Fix : CacheType.Main;
			}
			var fileRef = new FileReference() {
				Name = name,
				Key = key,
				CurrentValue = currentValue,
				LoadCallback = loadCallback,
				AlreadyLoadedCallback = alreadyLoadedCallback,
				Flags = flags,
				Cache = cache
			};
			if (immediate && Bin != null && queue == Bin.QueueType) {
				LoadQueues[queue].AddFirst(fileRef);
			} else {
				LoadQueues[queue].AddLast(fileRef);
			}
		}
		public void RequestFileSize(Jade_Key key, uint currentValue, FileSizeResolveAction loadCallback, bool immediate = true,
			QueueType queue = QueueType.Current,
			CacheType cache = CacheType.Current,
			string name = "", ReferenceFlags flags = ReferenceFlags.MustExist) {
			if (queue == QueueType.Current) {
				queue = Bin?.QueueType ?? QueueType.BigFat;
				//if(Bin != null && FileInfos.ContainsKey(key)) queue = QueueType.BigFat;
			}
			if (cache == CacheType.Current) {
				cache = IsLoadingFix ? CacheType.Fix : CacheType.Main;
			}
			var fileRef = new FileReference() {
				Name = name,
				Key = key,
				IsSizeRequest = true,
				Size_ResolveAction = loadCallback,
				Size_CurrentValue = currentValue,
				Flags = flags,
				Cache = cache
			};
			if (immediate && Bin != null && queue == Bin.QueueType) {
				LoadQueues[queue].AddFirst(fileRef);
			} else {
				LoadQueues[queue].AddLast(fileRef);
			}
		}
		public void BeginSpeedMode(Jade_Key key, Func<SerializerObject, UniTask> serializeAction = null) {
			if (SpeedMode) {
				if (Bin != null || (key != Jade_Key.KeyTypeSounds && key != Jade_Key.KeyTypeTextures && key.Type != Jade_Key.KeyType.TextSound)) {
					if (key == Jade_Key.KeyTypeTextures) {
						key = Bin.Key.GetBinary(Jade_Key.KeyType.Textures);
					} else if (key == Jade_Key.KeyTypeSounds) {
						key = Bin.Key.GetBinary(Jade_Key.KeyType.Sounds);
					} else {
						switch (key.Type) {
							case Jade_Key.KeyType.TextNoSound:
							case Jade_Key.KeyType.TextSound:
								break;
							default:
								key = key.GetBinary(Jade_Key.KeyType.Map);
								break;
						}
					}
					if (FileInfos.ContainsKey(key)) {
						ReadMode = Read.Binary;
						Bin = new BinData() { Key = key, SerializeAction = serializeAction };
						switch (key.Type) {
							case Jade_Key.KeyType.Map: Bin.QueueType = QueueType.Maps; break;
							case Jade_Key.KeyType.Sounds: Bin.QueueType = QueueType.Sound; break;
							case Jade_Key.KeyType.TextNoSound: Bin.QueueType = QueueType.TextNoSound; break;
							case Jade_Key.KeyType.TextSound: Bin.QueueType = QueueType.TextSound; break;
							case Jade_Key.KeyType.Textures: Bin.QueueType = QueueType.Textures; break;
						}
						LoadQueues[QueueType.BigFat].AddLast(new FileReference() {
							Name = "BIN",
							Key = key,
							IsBin = true,
							Flags = ReferenceFlags.MustExist,
							Cache = IsLoadingFix ? CacheType.Fix : CacheType.Main,
						});
						IsCompressed = Bin.Key.IsCompressed;
						ReadBinFileHeader = IsCompressed;
						Context.SystemLogger?.LogInfo($"[{key}] ({key.Type}) - Entering Speed Mode");
					} else {
						Context.SystemLogger?.LogWarning($"[{key}] ({key.Type} - File not found, could not enter Speed Mode");
						EndSpeedMode();
					}
				}
			}
		}
		public void EndSpeedMode() {
			ReadMode = Read.Full;
			IsCompressed = false;
			ReadBinFileHeader = false;
			Bin = null;
			CurrentQueueType = QueueType.BigFat;
		}

		public class FileInfo {
			public Jade_Key Key { get; set; }
			public Pointer FileOffset { get; set; }

			public BIG_FatFile FatFile { get; set; }
			public int FileIndex { get; set; }
			public BIG_FatFile.FileInfo FatFileInfo { get; set; }
			public string FileName { get; set; }
			public string DirectoryName { get; set; }
			public string FilePath => FileName != null ? $"{DirectoryName}/{FileName}" : null;
			public string FilePathValidCharacters => FileName != null ? $"{DirectoryName}/{FileNameValidCharacters}" : null;
			public string FileNameValidCharacters => FileName != null ? $"{string.Join("_", FileName.Split(Path.GetInvalidFileNameChars()))}" : null;

			public override string ToString() {
				var fp = FilePath;
				if (fp != null) {
					return $"[{Key}] {fp}";
				} else {
					return $"[{Key}] Unknown File";
				}
			}
			public string FileRegionName => FileName;
		}
	}
}
