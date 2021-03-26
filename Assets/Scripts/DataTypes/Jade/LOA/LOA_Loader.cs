using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R1Engine.Jade {
	public class LOA_Loader {
		public BIG_BigFile[] BigFiles { get; set; }
		public Queue<FileReference> LoadQueue = new Queue<FileReference>();
		public Dictionary<Jade_Key, FileInfo> FileInfos { get; private set; }
		public Dictionary<Jade_Key, Jade_File> LoadedFiles { get; private set; }
		public bool IsBinaryData => SpeedMode && ReadMode == Read.Binary;
		public bool SpeedMode { get; set; } = true;
		public Read ReadMode { get; set; } = Read.Full;
		public bool IsCompressed { get; set; } = false;
		public bool ReadSizes { get; set; } = false;
		public BINData Bin { get; set; }

		public class BINData {
			public Jade_Key Key { get; set; }
			public Pointer CurrentPosition { get; set; }
			public uint TotalSize { get; set; }
			public SerializerObject Serializer { get; set; }
		}

		public enum Read {
			Full = 0,
			Binary = 2,
		}


		public LOA_Loader(BIG_BigFile[] bigFiles) {
			BigFiles = bigFiles;
			CreateFileDictionaries();

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
			while (LoadQueue.Count > 0) {
				FileReference currentRef = LoadQueue.Peek();
				if (currentRef.Key != null && FileInfos.ContainsKey(currentRef.Key)) {
					if (LoadedFiles.ContainsKey(currentRef.Key)) {
						currentRef.AlreadyLoadedCallback(LoadedFiles[currentRef.Key]);
					} else {
						Pointer off_current = s.CurrentPointer;
						FileInfo f = FileInfos[currentRef.Key];
						Pointer off_target = f.FileOffset;
						s.Goto(off_target);
						s.Log($"LOA: Loading file: {f}");
						await s.FillCacheForRead(4);
						var fileSize = s.Serialize<uint>(default, name: "FileSize");
						await s.FillCacheForRead((int)fileSize);

						if (Bin != null) {
							if (IsCompressed) {
								s.DoEncoded(new Jade_Lzo1xEncoder(fileSize, xbox360Version: s.GameSettings.EngineVersion == EngineVersion.Jade_RRR_Xbox360), () => {
									uint decompressedLength = s.CurrentLength;
									Bin.Serializer = s;
									LoadLoopBIN(decompressedLength);
								});
							} else {
								Bin.Serializer = s;
								LoadLoopBIN(fileSize);
							}
						} else {
							currentRef.LoadCallback(s, (f) => {
								f.Key = currentRef.Key;
								f.FileSize = fileSize;
								f.Loader = this;
								if (!LoadedFiles.ContainsKey(f.Key)) LoadedFiles[f.Key] = f;
							});
						}
						s.Goto(off_current);
					}
				}
				if(LoadQueue.Count > 0) LoadQueue.Dequeue();
			}
		}


		public void LoadLoopBIN(uint totalSize) {
			SerializerObject s = Bin.Serializer;
			var curPointer = s.CurrentPointer;
			if (Bin.CurrentPosition == null) Bin.CurrentPosition = curPointer;
			var startPointer = Bin.CurrentPosition;
			while (LoadQueue.Count > 0) {
				uint FileSize = 0;
				s.Goto(Bin.CurrentPosition);
				if (ReadSizes) {
					FileSize = s.Serialize<uint>(FileSize, name: nameof(FileSize));
					Bin.CurrentPosition = Bin.CurrentPosition + 4 + FileSize;
				} else {
					FileSize = totalSize - (uint)(Bin.CurrentPosition - startPointer);
				}
				FileReference currentRef = LoadQueue.Dequeue();
				currentRef.LoadCallback(s, (f) => {
					f.Key = currentRef.Key;
					f.FileSize = FileSize;
					f.Loader = this;
					if (!LoadedFiles.ContainsKey(f.Key)) LoadedFiles[f.Key] = f;
				});
				if (ReadSizes) {
					s.Goto(Bin.CurrentPosition); // count size uint and actual file
				} else {
					Bin.CurrentPosition = s.CurrentPointer;
				}
			}
			s.Goto(curPointer);
		}

		public delegate void ResolveAction(SerializerObject s, Action<Jade_File> configureAction);
		public delegate void ResolvedAction(Jade_File f);
		public class FileReference {
			public Jade_Key Key { get; set; }
			public ResolveAction LoadCallback { get; set; }
			public ResolvedAction AlreadyLoadedCallback { get; set; }
		}

		public void RequestFile(Jade_Key key, ResolveAction loadCallback, ResolvedAction alreadyLoadedCallback, bool immediate = false) {
			LoadQueue.Enqueue(new FileReference() {
				Key = key,
				LoadCallback = loadCallback,
				AlreadyLoadedCallback = alreadyLoadedCallback
			});
			if (immediate && Bin != null) {
				LoadLoopBIN(0);
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
						Bin = new BINData() { Key = key };
						IsCompressed = Bin.Key.IsCompressed;
						ReadSizes = IsCompressed;
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
		}
	}
}
