using Cysharp.Threading.Tasks;
using System;
using BinarySerializer;
using System.Collections.Generic;

namespace Ray1Map.Jade {
	public class BIG_BigFile : BinarySerializable {
		public static readonly byte[] XORKey = new byte[] { 0xb3, 0x98, 0xcc, 0x66 };
		public static uint HeaderLength => 44;
		public uint TotalFatFilesLength => NumFat * (
			BIG_FatFile.HeaderLength
			+ SizeOfFat * BIG_FatFile.FileReference.StructSize
			+ SizeOfFat * BIG_FatFile.FileInfo.StructSize(this)
			+ SizeOfFat * BIG_FatFile.DirectoryInfo.StructSize(this));
		
		public string BIG_gst { get; set; }
		public uint Version { get; set; }
		public uint MaxFile { get; set; }
		public uint MaxDir { get; set; }
		public uint MaxKey { get; set; }
		public uint Root { get; set; }
		public int FirstFreeFile { get; set; }
		public int FirstFreeDirectory { get; set; }
		public uint SizeOfFat { get; set; }
		public uint NumFat { get; set; }
		public Jade_Key UniverseKey { get; set; } // First file it loads
		public BIG_BigFile_V43Data V43Data { get; set; }

		public Pointer FatFilesOffset { get; set; }
		public BIG_FatFile[] FatFiles { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			BIG_gst = s.SerializeString(BIG_gst, 4, name: nameof(BIG_gst));
			XORIfNecessary(s, () => {
				Version = s.Serialize<uint>(Version, name: nameof(Version));
				MaxFile = s.Serialize<uint>(MaxFile, name: nameof(MaxFile));
				MaxDir = s.Serialize<uint>(MaxDir, name: nameof(MaxDir));
				MaxKey = s.Serialize<uint>(MaxKey, name: nameof(MaxKey));
				Root = s.Serialize<uint>(Root, name: nameof(Root));
				FirstFreeFile = s.Serialize<int>(FirstFreeFile, name: nameof(FirstFreeFile));
				FirstFreeDirectory = s.Serialize<int>(FirstFreeDirectory, name: nameof(FirstFreeDirectory));
				SizeOfFat = s.Serialize<uint>(SizeOfFat, name: nameof(SizeOfFat));
				NumFat = s.Serialize<uint>(NumFat, name: nameof(NumFat));
			});
			UniverseKey = s.SerializeObject<Jade_Key>(UniverseKey, name: nameof(UniverseKey));
			if (Version >= 43) {
				V43Data = s.SerializeObject<BIG_BigFile_V43Data>(V43Data, name: nameof(V43Data));
			}
			FatFilesOffset = s.CurrentPointer;
		}

		public void SerializeFatFiles(SerializerObject s) {
			s.DoAt(FatFilesOffset, () => {
				XORIfNecessary(s, () => {
					FatFiles = s.SerializeObjectArray<BIG_FatFile>(FatFiles, NumFat, onPreSerialize: ff => {
						ff.Big = this;
					}, name: nameof(FatFiles));
				});
			});
		}

		public void XORIfNecessary(SerializerObject s, Action action) {
			if (BIG_gst == "BUG") {
				s.DoProcessed(new XorArrayProcessor(XORKey, byteIndex: (int)(s.CurrentPointer.AbsoluteOffset % 4)), action);
			} else {
				action();
			}
		}

		public async UniTask SerializeFile(SerializerObject s, int fatIndex, int fileIndex, Action<uint, bool> action) {
			var fat = FatFiles[fatIndex];
			Pointer off_current = s.CurrentPointer;
			Pointer off_target = fat.Files[fileIndex].FileOffset;
			/*var sortedFileList = fat.Files.OrderBy(f => f.FileOffset.AbsoluteOffset).ToArray();
			var indInSortedFileList = sortedFileList.FindItemIndex(f => f.FileOffset == off_target);*/
			s.Goto(off_target);
			uint FileSize = 0;
			bool IsBranch = false;
			await s.FillCacheForReadAsync(4);
			s.DoBits<uint>(b => {
				FileSize = b.SerializeBits<uint>(FileSize, 31, name: nameof(FileSize));
				IsBranch = b.SerializeBits<bool>(IsBranch, 1, name: nameof(IsBranch));
			});
		
			//var actualFileSize = (indInSortedFileList+1 >= sortedFileList.Length ? s.CurrentLength : sortedFileList[indInSortedFileList+1].FileOffset.AbsoluteOffset) - off_target.AbsoluteOffset - 4;
			/*if (s.GetR1Settings().EngineVersion == EngineVersion.Jade_BGE_HD) {
				FileSize = fat.FileInfos[fileIndex].FileSize;
			}*/
			await s.FillCacheForReadAsync(FileSize);
			action(FileSize, IsBranch);
			s.Goto(off_current);
		}

		public async UniTask SerializeAt(SerializerObject s, Pointer off_target, Action<uint> action) {
			Pointer off_current = s.CurrentPointer;
			s.Goto(off_target);
			await s.FillCacheForReadAsync(4);
			var fileSize = s.Serialize<uint>(default, name: "FileSize");
			//var actualFileSize = (indInSortedFileList+1 >= sortedFileList.Length ? s.CurrentLength : sortedFileList[indInSortedFileList+1].FileOffset.AbsoluteOffset) - off_target.AbsoluteOffset - 4;
			await s.FillCacheForReadAsync(fileSize);
			action(fileSize);
			s.Goto(off_current);
		}

		public static BIG_BigFile Create(BIG_BigFile og, Pointer startPointer,
			FileInfoForCreate[] files, DirectoryInfoForCreate[] directories, uint paddingBetweenFiles = 0x100,
			bool writeFilenameInPadding = true, bool increaseSizeOfFat = true) {
			BIG_BigFile bf = new BIG_BigFile();
			bf.BIG_gst = "BIG";
            bf.Init(startPointer);
			bf.Version = og.Version;
			bf.SizeOfFat = og.SizeOfFat;
			if (increaseSizeOfFat) {
				bf.SizeOfFat = (uint)Math.Max(directories.Length, files.Length);
			}
			bf.MaxFile = (uint)files.Length;
			bf.MaxDir = (uint)directories.Length;
			bf.MaxKey = 0;
			bf.Root = 0;
			bf.UniverseKey = og.UniverseKey;
			bf.FirstFreeFile = -1;
			bf.FirstFreeDirectory = -1;

			bf.FatFilesOffset = startPointer + HeaderLength;
			bf.NumFat = (uint)(bf.MaxFile / bf.SizeOfFat + (bf.MaxFile % bf.SizeOfFat > 0 ? 1 : 0));
			bf.FatFiles = new BIG_FatFile[bf.NumFat];

			uint curFileIndex = 0;
			uint curDirectoryIndex = 0;
			Pointer curFatOffset = bf.FatFilesOffset;
			Pointer filesStart = bf.FatFilesOffset + bf.TotalFatFilesLength;
			Pointer curFileStartOffset = filesStart;

			for (int fatIndex = 0; fatIndex < bf.NumFat; fatIndex++) {
				var fat = new BIG_FatFile(curFatOffset);
				bf.FatFiles[fatIndex] = fat;

				uint nextFileIndex = Math.Min(curFileIndex + bf.SizeOfFat, bf.MaxFile);
				uint nextDirectoryIndex = Math.Min(curDirectoryIndex + bf.SizeOfFat, bf.MaxDir);
				Pointer nextFatOffset = curFatOffset + BIG_FatFile.HeaderLength
					+ bf.SizeOfFat * BIG_FatFile.FileReference.StructSize
					+ bf.SizeOfFat * BIG_FatFile.FileInfo.StructSize(bf)
					+ bf.SizeOfFat * BIG_FatFile.DirectoryInfo.StructSize(bf);
				var curFilesInFat = nextFileIndex - curFileIndex;
				var curDirectoriesInFat = nextDirectoryIndex - curDirectoryIndex;
				fat.Big = bf;
				fat.PosFat = fat.Offset + BIG_FatFile.HeaderLength;
				fat.FirstIndex = curFileIndex;
				fat.LastIndex = nextFileIndex-1;
				fat.MaxFile = curFilesInFat;
				fat.MaxDir = curDirectoriesInFat;
				if (fatIndex < bf.NumFat - 1) {
					fat.NextPosFat = (int)(nextFatOffset.AbsoluteOffset - BIG_FatFile.HeaderLength);
				} else {
					fat.NextPosFat = -1;
				}
				fat.DirectoryInfos = new BIG_FatFile.DirectoryInfo[curDirectoriesInFat];
				fat.FileInfos = new BIG_FatFile.FileInfo[curFilesInFat];
				fat.Files = new BIG_FatFile.FileReference[curFilesInFat];
				for (int dir_i = 0; dir_i < curDirectoriesInFat; dir_i++) {
					var dir = directories[dir_i + curDirectoryIndex];
					fat.DirectoryInfos[dir_i] = new BIG_FatFile.DirectoryInfo() {
						Parent = dir.ParentIndex,
						Name = dir.DirectoryName,

						FirstSubDirectory = dir.FirstDirectoryID,
						FirstFile = dir.FirstFileID,
						Previous = dir.PreviousDirectoryID,
						Next = dir.NextDirectoryID,
						Big = bf,
					};
				}
				for (int file_i = 0; file_i < curFilesInFat; file_i++) {
					var f = files[file_i + curFileIndex];
					if (writeFilenameInPadding) {
						f.NameOffset = curFileStartOffset;
						curFileStartOffset += f.Filename.Length + 3;
					}
					// Align files with 16. Not really necessary, but nice.
					if (curFileStartOffset.AbsoluteOffset % 16 != 0) {
						curFileStartOffset = curFileStartOffset + 16 - curFileStartOffset.AbsoluteOffset % 16;
					}
					fat.Files[file_i] = new BIG_FatFile.FileReference() {
						Key = f.Key,
						FileOffset = curFileStartOffset
					};
					fat.FileInfos[file_i] = new BIG_FatFile.FileInfo() {
						Name = f.Filename,
						LengthOnDisk = f.FileSize,
						ParentDirectory = f.DirectoryIndex,
						Previous = f.PreviousFileInDirectoryIndex,
						Next = f.NextFileInDirectoryIndex,
						DateLastModified = f.DateLastModified,
						P4RevisionClient = f.P4Revision,
						Big = bf,
					};
					f.Offset = fat.Files[file_i].FileOffset;

					curFileStartOffset = curFileStartOffset + f.FileSize + paddingBetweenFiles + 4; // 4 for the filesize uint
				}
				curFatOffset = nextFatOffset;
				curFileIndex = nextFileIndex;
				curDirectoryIndex = nextDirectoryIndex;
			}
			return bf;
		}
		public class FileInfoForCreate {
			public Jade_Key Key { get; set; }
			public string Filename { get; set; }
			public int DirectoryIndex { get; set; } = -1;
			public uint FileSize { get; set; }
			public byte[] Bytes { get; set; }
			public Pointer Offset { get; set; }
			public Pointer NameOffset { get; set; }
			public int FileIndex { get; set; }
			public int NextFileInDirectoryIndex { get; set; } = -1;
			public int PreviousFileInDirectoryIndex { get; set; } = -1;

			// Temporary
			public DateTime DateLastModified { get; set; }
			public uint P4Revision { get; set; }
			public FileSource Source { get; set; }
			public string ModDirectory { get; set; }
			public string FullPath { get; set; }
			public string FullPathBeforeReplace { get; set; }
			public enum FileSource {
				Unbinarized,
				Existing,
				Mod
			}
		}
		public class DirectoryInfoForCreate {
			public string DirectoryName { get; set; }
			public int ParentIndex { get; set; } = -1;
			public string FullDirectoryString { get; set; }
			public int NextDirectoryID { get; set; } = -1;
			public int PreviousDirectoryID { get; set; } = -1;
			public int FirstDirectoryID { get; set; } = -1;
			public int FirstFileID { get; set; } = -1;
			public int DirectoryIndex { get; set; }
			public List<BIG_BigFile.DirectoryInfoForCreate> SubDirectories { get; set; } = new List<BIG_BigFile.DirectoryInfoForCreate>();
			public List<FileInfoForCreate> Files { get; set; } = new List<FileInfoForCreate>();
		}
	}
}
