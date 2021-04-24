using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace R1Engine.Jade {
	public class LOA_Loader {
		public bool IsLoadingFix { get; set; }
		public LOA_SpecialArray SpecialArray { get; set; }

		public BIG_BigFile[] BigFiles { get; set; }
		public Dictionary<QueueType, LinkedList<FileReference>> LoadQueues = new Dictionary<QueueType, LinkedList<FileReference>>();// = new Queue<FileReference>();
		public LinkedList<FileReference> LoadQueue => LoadQueues[CurrentQueueType];
		public Dictionary<Jade_Key, FileInfo> FileInfos { get; private set; }
		public Dictionary<CacheType, Dictionary<Jade_Key, Jade_File>> Caches { get; private set; } = new Dictionary<CacheType, Dictionary<Jade_Key, Jade_File>>();
		public Dictionary<Jade_Key, Jade_File> Cache => Caches[CurrentCacheType];
		public bool IsBinaryData => SpeedMode && ReadMode == Read.Binary;
		public bool SpeedMode { get; set; } = true;
		public Read ReadMode { get; set; } = Read.Full;
		public bool IsCompressed { get; set; } = false;
		public bool ReadSizes { get; set; } = false;
		public BinData Bin { get; set; }
		public QueueType CurrentQueueType { get; set; } = QueueType.BigFat;
		public CacheType CurrentCacheType { get; set; } = CacheType.Main;

		public WOR_World WorldToLoadIn { get; set; }
		public WOR_World CurWorldForGrids { get; set; }
		public List<WOR_World> LoadedWorlds { get; set; } = new List<WOR_World>();
		public List<OBJ_GameObject> AttachedGameObjects { get; set; } = new List<OBJ_GameObject>();
		public bool IsGameObjectAttached(OBJ_GameObject gao) => AttachedGameObjects.Any(obj => obj.Key == gao.Key);


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
		}

		public enum Read {
			Full = 0,
			Binary = 2,
		}

		[Flags]
		public enum ReferenceFlags : uint {
			None = 0,
			Log = 1 << 0,
			Flag1 = 1 << 1,
			DontCache = 1 << 2,
			Flag3 = 1 << 3,
			KeepReferencesCount = 1 << 4,
			DontUseAlreadyLoadedCallback = 1 << 5,
			Flag6 = 1 << 6,
			Flag7 = 1 << 7,
			
			// Custom
			IsIrregularFileFormat = 1 << 16,
			DontUseCachedFile = 1 << 17
		}


		public LOA_Loader(BIG_BigFile[] bigFiles) {
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
							while (curDir.ParentDirectory != -1) {
								curDir = fatForDirectories.DirectoryInfos[curDir.ParentDirectory];
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

		public async UniTask LoadLoop(SerializerObject s) {
			CurrentQueueType = QueueType.BigFat;
			CurrentCacheType = IsLoadingFix ? CacheType.Fix : CacheType.Main;

			while (LoadQueue.First?.Value != null) {
				FileReference currentRef = LoadQueue.First.Value;
				LoadQueue.RemoveFirst();
				if (currentRef.Key != null && FileInfos.ContainsKey(currentRef.Key)) {
					CurrentCacheType = currentRef.Cache;
					if (!currentRef.IsBin && Cache.ContainsKey(currentRef.Key) && !currentRef.Flags.HasFlag(ReferenceFlags.DontUseCachedFile)) {
						var f = Cache[currentRef.Key];
						if (f != null) f.CachedCount++;
						if (currentRef.Flags.HasFlag(ReferenceFlags.KeepReferencesCount) && f != null) f.ReferencesCount++;
						if (!currentRef.Flags.HasFlag(ReferenceFlags.DontUseAlreadyLoadedCallback)) currentRef.AlreadyLoadedCallback(f);
					} else {
						Pointer off_current = s.CurrentPointer;
						FileInfo f = FileInfos[currentRef.Key];
						Pointer off_target = f.FileOffset;
						s.Goto(off_target);
						s.Log($"LOA: Loading file: {f}");
						string previousState = Controller.DetailedState;
						Controller.DetailedState = $"{previousState}\n{f}";
						await s.FillCacheForReadAsync(4);
						var fileSize = s.Serialize<uint>(default, name: "FileSize");
						if (fileSize != 0) {
							await s.FillCacheForReadAsync((int)fileSize);

							// Add region
							string regionName = f.FileRegionName ?? $"{currentRef.Name}_{currentRef.Key:X8}";
							off_target.File.AddRegion(off_target.FileOffset + 4, fileSize, regionName);

							if (currentRef.IsBin && Bin != null) {
								if (IsCompressed) {
									Bin.StartPosition = s.BeginEncoded(new Jade_Lzo1xEncoder(fileSize, xbox360Version: s.GetR1Settings().Jade_Version == Jade_Version.Xenon),
										filename: regionName);
									Bin.CurrentPosition = Bin.StartPosition;
									s.Goto(Bin.StartPosition);
									uint decompressedLength = s.CurrentLength;
									Bin.Serializer = s;
									Bin.TotalSize = decompressedLength;
									if (Bin?.SerializeAction != null) {
										await Bin.SerializeAction(s);
									} else {
										await LoadLoopBINAsync();
									}
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
									if (!Cache.ContainsKey(f.Key) && !currentRef.Flags.HasFlag(ReferenceFlags.DontCache)) Cache[f.Key] = f;
								});
							}
						} else {
							if (!Cache.ContainsKey(f.Key) && !currentRef.Flags.HasFlag(ReferenceFlags.DontCache)) Cache[f.Key] = null;
						}
						await Controller.WaitIfNecessary();
						Controller.DetailedState = previousState;
						s.Goto(off_current);
					}
				} else if (currentRef.Flags.HasFlag(ReferenceFlags.Log)) {
					UnityEngine.Debug.LogWarning($"File {currentRef.Name}_{currentRef.Key:X8} was not found");
				}
			}
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
			uint FileSize = 0;
			s.Goto(Bin.CurrentPosition);
			CurrentCacheType = currentRef.Cache;

			bool hasLoadedFile = GetLoadedFile(currentRef.Key, out Jade_File loadedFile);
			if (hasLoadedFile && !currentRef.Flags.HasFlag(ReferenceFlags.DontUseCachedFile)) {
				var f = loadedFile;
				if (f != null) f.CachedCount++;
				if (currentRef.Flags.HasFlag(ReferenceFlags.KeepReferencesCount) && f != null) f.ReferencesCount++;
				if (!currentRef.Flags.HasFlag(ReferenceFlags.DontUseAlreadyLoadedCallback)) currentRef.AlreadyLoadedCallback(f);
			} else {
				/*if (previouslyCached.Contains(currentRef.Key)) {
					UnityEngine.Debug.Log($"Reserializing: {currentRef.Key}");
				}*/
				if (!currentRef.Flags.HasFlag(ReferenceFlags.IsIrregularFileFormat)) {
					if (ReadSizes) {
						FileSize = s.Serialize<uint>(FileSize, name: nameof(FileSize));

						// Add region
						Bin.CurrentPosition.File.AddRegion(Bin.CurrentPosition.FileOffset + 4, FileSize, $"{currentRef.Name}_{currentRef.Key:X8}");

						Bin.CurrentPosition = Bin.CurrentPosition + 4 + FileSize;
					} else {
						FileSize = Bin.TotalSize - (uint)(Bin.CurrentPosition - Bin.StartPosition);
					}
				}
				if (FileSize != 0 || currentRef.Flags.HasFlag(ReferenceFlags.IsIrregularFileFormat)) {
					currentRef.LoadCallback(s, (f) => {
						f.Key = currentRef.Key;
						f.FileSize = FileSize;
						f.Loader = this;
						if (!Cache.ContainsKey(f.Key) && !currentRef.Flags.HasFlag(ReferenceFlags.DontCache)) Cache[f.Key] = f;
					});
				} else {
					if (!Cache.ContainsKey(currentRef.Key) && !currentRef.Flags.HasFlag(ReferenceFlags.DontCache)) {
						Cache[currentRef.Key] = null;
					}
				}
				if (ReadSizes && !currentRef.Flags.HasFlag(ReferenceFlags.IsIrregularFileFormat)) {
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
		public class FileReference {
			public string Name { get; set; }
			public Jade_Key Key { get; set; }
			public ResolveAction LoadCallback { get; set; }
			public ResolvedAction AlreadyLoadedCallback { get; set; }
			public bool IsBin { get; set; }
			public ReferenceFlags Flags { get; set; }
			public CacheType Cache { get; set; }
		}

		public void RequestFile(Jade_Key key, ResolveAction loadCallback, ResolvedAction alreadyLoadedCallback, bool immediate = false,
			QueueType queue = QueueType.Current,
			CacheType cache = CacheType.Current,
			string name = "", ReferenceFlags flags = ReferenceFlags.Log) {
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
							Flags = ReferenceFlags.Log,
							Cache = IsLoadingFix ? CacheType.Fix : CacheType.Main,
						});
						IsCompressed = Bin.Key.IsCompressed;
						ReadSizes = IsCompressed;
						UnityEngine.Debug.Log($"[{key}] ({key.Type}) - Entering Speed Mode");
					} else {
						EndSpeedMode();
					}
				}
			}
		}
		public void EndSpeedMode() {
			ReadMode = Read.Full;
			IsCompressed = false;
			ReadSizes = false;
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
