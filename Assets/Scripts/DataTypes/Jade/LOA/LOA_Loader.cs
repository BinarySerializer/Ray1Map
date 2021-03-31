using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace R1Engine.Jade {
	public class LOA_Loader {
		public BIG_BigFile[] BigFiles { get; set; }
		public Dictionary<QueueType, Queue<FileReference>> LoadQueues = new Dictionary<QueueType, Queue<FileReference>>();// = new Queue<FileReference>();
		public Queue<FileReference> LoadQueue => LoadQueues[CurrentQueueType];
		public Dictionary<Jade_Key, FileInfo> FileInfos { get; private set; }
		public Dictionary<Jade_Key, Jade_File> LoadedFiles { get; private set; }
		public bool IsBinaryData => SpeedMode && ReadMode == Read.Binary;
		public bool SpeedMode { get; set; } = true;
		public Read ReadMode { get; set; } = Read.Full;
		public bool IsCompressed { get; set; } = false;
		public bool ReadSizes { get; set; } = false;
		public BinData Bin { get; set; }
		public QueueType CurrentQueueType { get; set; } = QueueType.BigFat;


		public class BinData {
			public Jade_Key Key { get; set; }
			public Pointer CurrentPosition { get; set; }
			public uint TotalSize { get; set; }
			public SerializerObject Serializer { get; set; }
			public QueueType QueueType { get; set; }
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

		public enum Read {
			Full = 0,
			Binary = 2,
		}

		[Flags]
		public enum ReferenceFlags {
			None = 0,
			Log = 1 << 0,
			Flag1 = 1 << 1,
			DontCache = 1 << 2,
			Flag3 = 1 << 3,
			KeepReferencesCount = 1 << 4,
			DontUseAlreadyLoadedCallback = 1 << 5,
			Flag6 = 1 << 6,
			Flag7 = 1 << 7,
		}


		public LOA_Loader(BIG_BigFile[] bigFiles) {
			BigFiles = bigFiles;
			CreateFileDictionaries();
			foreach (QueueType queue in (QueueType[])Enum.GetValues(typeof(QueueType))) {
				LoadQueues[queue] = new Queue<FileReference>();
			}
		}

		private void CreateFileDictionaries() {
			FileInfos = new Dictionary<Jade_Key, FileInfo>();
			LoadedFiles = new Dictionary<Jade_Key, Jade_File>();
			for (int b = 0; b < BigFiles.Length; b++) {
				var big = BigFiles[b];
				for (int f = 0; f < big.FatFiles.Length; f++) {
					var fat = big.FatFiles[f];

					// Create directories list
					string[] directories = new string[fat.DirectoryInfos.Length];
					for (int i = 0; i < directories.Length; i++) {
						var dir = fat.DirectoryInfos[i];
						var dirName = dir.Name;
						var curDir = dir;
						while (curDir.ParentDirectory != -1) {
							curDir = fat.DirectoryInfos[curDir.ParentDirectory];
							dirName = $"{curDir.Name}/{dirName}";
						}
						directories[i] = dirName;
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
							fileInfo.DirectoryName = directories[fileInfo.FatFileInfo.ParentDirectory];
							fileInfo.FileName = fileInfo.FatFileInfo.Name;
						}
						FileInfos[file.Key] = fileInfo;
					}
				}
			}
		}

		public async UniTask LoadLoop(SerializerObject s) {
			CurrentQueueType = QueueType.BigFat;

			while (LoadQueue.Count > 0) {
				FileReference currentRef = LoadQueue.Dequeue();
				if (currentRef.Key != null && FileInfos.ContainsKey(currentRef.Key)) {
					if (!currentRef.IsBin && LoadedFiles.ContainsKey(currentRef.Key)) {
						var f = LoadedFiles[currentRef.Key];
						if (currentRef.Flags.HasFlag(ReferenceFlags.KeepReferencesCount) && f != null) f.ReferencesCount++;
						if (!currentRef.Flags.HasFlag(ReferenceFlags.DontUseAlreadyLoadedCallback)) currentRef.AlreadyLoadedCallback(f);
					} else {
						Pointer off_current = s.CurrentPointer;
						FileInfo f = FileInfos[currentRef.Key];
						Pointer off_target = f.FileOffset;
						s.Goto(off_target);
						s.Log($"LOA: Loading file: {f}");
						await s.FillCacheForReadAsync(4);
						var fileSize = s.Serialize<uint>(default, name: "FileSize");
						if (fileSize != 0) {
							await s.FillCacheForReadAsync((int)fileSize);

							// Add region
							off_target.File.AddRegion(off_target.FileOffset + 4, fileSize, f.FileRegionName ?? $"{currentRef.Name}_{currentRef.Key:X8}");

							if (currentRef.IsBin && Bin != null) {
								if (IsCompressed) {
									s.DoEncoded(new Jade_Lzo1xEncoder(fileSize, xbox360Version: s.GetR1Settings().EngineVersion == EngineVersion.Jade_RRR_Xbox360), () => {
										uint decompressedLength = s.CurrentLength;
										Bin.Serializer = s;
										Bin.TotalSize = decompressedLength;
										LoadLoopBIN();
									});
								} else {
									Bin.Serializer = s;
									Bin.TotalSize = fileSize;
									LoadLoopBIN();
								}
							} else {
								currentRef.LoadCallback(s, (f) => {
									f.Key = currentRef.Key;
									f.FileSize = fileSize;
									f.Loader = this;
									if (!LoadedFiles.ContainsKey(f.Key) && !currentRef.Flags.HasFlag(ReferenceFlags.DontCache)) LoadedFiles[f.Key] = f;
								});
							}
						} else {
							if (!LoadedFiles.ContainsKey(f.Key) && !currentRef.Flags.HasFlag(ReferenceFlags.DontCache)) LoadedFiles[f.Key] = null;
						}
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
			SerializerObject s = Bin.Serializer;
			var curPointer = s.CurrentPointer;
			if (Bin.CurrentPosition == null) Bin.CurrentPosition = curPointer;
			var startPointer = Bin.CurrentPosition;
			while (LoadQueue.Count > 0) {
				uint FileSize = 0;
				s.Goto(Bin.CurrentPosition);
				FileReference currentRef = LoadQueue.Dequeue();
				if (LoadedFiles.ContainsKey(currentRef.Key)) {
					var f = LoadedFiles[currentRef.Key];
					if (currentRef.Flags.HasFlag(ReferenceFlags.KeepReferencesCount) && f != null) f.ReferencesCount++;
					if (!currentRef.Flags.HasFlag(ReferenceFlags.DontUseAlreadyLoadedCallback)) currentRef.AlreadyLoadedCallback(f);
				} else {
					if (ReadSizes) {
						FileSize = s.Serialize<uint>(FileSize, name: nameof(FileSize));

						// Add region
						Bin.CurrentPosition.File.AddRegion(Bin.CurrentPosition.FileOffset + 4, FileSize, $"{currentRef.Name}_{currentRef.Key:X8}");

						Bin.CurrentPosition = Bin.CurrentPosition + 4 + FileSize;
					} else {
						FileSize = Bin.TotalSize - (uint)(Bin.CurrentPosition - startPointer);
					}
					if (FileSize != 0) {
						currentRef.LoadCallback(s, (f) => {
							f.Key = currentRef.Key;
							f.FileSize = FileSize;
							f.Loader = this;
							if (!LoadedFiles.ContainsKey(f.Key) && !currentRef.Flags.HasFlag(ReferenceFlags.DontCache)) LoadedFiles[f.Key] = f;
						});
					} else {
						if (!LoadedFiles.ContainsKey(currentRef.Key) && !currentRef.Flags.HasFlag(ReferenceFlags.DontCache)) LoadedFiles[currentRef.Key] = null;
					}
					if (ReadSizes) {
						s.Goto(Bin.CurrentPosition); // count size uint and actual file
					} else {
						Bin.CurrentPosition = s.CurrentPointer;
					}
				}
			}
			s.Goto(curPointer);
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
		}

		public void RequestFile(Jade_Key key, ResolveAction loadCallback, ResolvedAction alreadyLoadedCallback, bool immediate = false, QueueType queue = QueueType.Current, string name = "", ReferenceFlags flags = ReferenceFlags.Log) {
			if (queue == QueueType.Current) {
				queue = Bin?.QueueType ?? QueueType.BigFat;
			}
			LoadQueues[queue].Enqueue(new FileReference() {
				Name = name,
				Key = key,
				LoadCallback = loadCallback,
				AlreadyLoadedCallback = alreadyLoadedCallback,
				Flags = flags
			});
			if (immediate && Bin != null && queue == Bin.QueueType) {
				LoadLoopBIN();
			}
		}
		public void BeginSpeedMode(Jade_Key key) {
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
						Bin = new BinData() { Key = key };
						switch (key.Type) {
							case Jade_Key.KeyType.Map: Bin.QueueType = QueueType.Maps; break;
							case Jade_Key.KeyType.Sounds: Bin.QueueType = QueueType.Sound; break;
							case Jade_Key.KeyType.TextNoSound: Bin.QueueType = QueueType.TextNoSound; break;
							case Jade_Key.KeyType.TextSound: Bin.QueueType = QueueType.TextSound; break;
							case Jade_Key.KeyType.Textures: Bin.QueueType = QueueType.Textures; break;
						}
						LoadQueues[QueueType.BigFat].Enqueue(new FileReference() {
							Name = "BIN",
							Key = key,
							IsBin = true,
							Flags = ReferenceFlags.Log
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
