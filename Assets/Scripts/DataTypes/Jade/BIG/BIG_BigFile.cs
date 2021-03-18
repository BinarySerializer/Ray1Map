using Cysharp.Threading.Tasks;
using System;

namespace R1Engine.Jade {
	public class BIG_BigFile : R1Serializable {
		public static readonly byte[] XORKey = new byte[] { 0xb3, 0x98, 0xcc, 0x66 };
		public static uint HeaderLength => 44;
		public uint TotalFatFilesLength => FatFilesCount * (
			BIG_FatFile.HeaderLength
			+ FatFileMaxEntries * BIG_FatFile.FileReference.StructSize
			+ FatFileMaxEntries * BIG_FatFile.FileInfo.StructSize
			+ FatFileMaxEntries * BIG_FatFile.DirectoryInfo.StructSize);
		
		public string BIG_gst { get; set; }
		public uint Version { get; set; }
		public uint FilesCount { get; set; }
		public uint DirectoriesCount { get; set; }
		public uint UInt_10 { get; set; }
		public uint UInt_14 { get; set; }
		public int Int_18 { get; set; }
		public int Int_1C { get; set; }
		public uint FatFileMaxEntries { get; set; }
		public uint FatFilesCount { get; set; }
		public Jade_Key UniversKey { get; set; } // First file it loads

		public Pointer FatFilesOffset { get; set; }
		public BIG_FatFile[] FatFiles { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			BIG_gst = s.SerializeString(BIG_gst, 4, name: nameof(BIG_gst));
			XORIfNecessary(s, () => {
				Version = s.Serialize<uint>(Version, name: nameof(Version));
				FilesCount = s.Serialize<uint>(FilesCount, name: nameof(FilesCount));
				DirectoriesCount = s.Serialize<uint>(DirectoriesCount, name: nameof(DirectoriesCount));
				UInt_10 = s.Serialize<uint>(UInt_10, name: nameof(UInt_10));
				UInt_14 = s.Serialize<uint>(UInt_14, name: nameof(UInt_14));
				Int_18 = s.Serialize<int>(Int_18, name: nameof(Int_18));
				Int_1C = s.Serialize<int>(Int_1C, name: nameof(Int_1C));
				FatFileMaxEntries = s.Serialize<uint>(FatFileMaxEntries, name: nameof(FatFileMaxEntries));
				FatFilesCount = s.Serialize<uint>(FatFilesCount, name: nameof(FatFilesCount));
			});
			UniversKey = s.SerializeObject<Jade_Key>(UniversKey, name: nameof(UniversKey));
			FatFilesOffset = s.CurrentPointer;
		}

		public void SerializeFatFiles(SerializerObject s) {
			s.DoAt(FatFilesOffset, () => {
				XORIfNecessary(s, () => {
					FatFiles = s.SerializeObjectArray<BIG_FatFile>(FatFiles, FatFilesCount, onPreSerialize: ff => ff.MaxEntries = FatFileMaxEntries, name: nameof(FatFiles));
				});
			});
		}

		public void XORIfNecessary(SerializerObject s, Action action) {
			if (BIG_gst == "BUG") {
				s.DoXOR(new XORArrayCalculator(XORKey, currentByte: (int)(s.CurrentPointer.AbsoluteOffset % 4)), action);
			} else {
				action();
			}
		}

		public async UniTask SerializeFile(SerializerObject s, int fatIndex, int fileIndex, Action<uint> action) {
			var fat = FatFiles[fatIndex];
			Pointer off_current = s.CurrentPointer;
			Pointer off_target = fat.Files[fileIndex].FileOffset;
			/*var sortedFileList = fat.Files.OrderBy(f => f.FileOffset.AbsoluteOffset).ToArray();
			var indInSortedFileList = sortedFileList.FindItemIndex(f => f.FileOffset == off_target);*/
			s.Goto(off_target);
			await s.FillCacheForRead(4);
			var fileSize = s.Serialize<uint>(default, name: "FileSize");
			//var actualFileSize = (indInSortedFileList+1 >= sortedFileList.Length ? s.CurrentLength : sortedFileList[indInSortedFileList+1].FileOffset.AbsoluteOffset) - off_target.AbsoluteOffset - 4;
			await s.FillCacheForRead((int)fileSize);
			action(fileSize);
			s.Goto(off_current);
		}

		public async UniTask SerializeAt(SerializerObject s, Pointer off_target, Action<uint> action) {
			Pointer off_current = s.CurrentPointer;
			s.Goto(off_target);
			await s.FillCacheForRead(4);
			var fileSize = s.Serialize<uint>(default, name: "FileSize");
			//var actualFileSize = (indInSortedFileList+1 >= sortedFileList.Length ? s.CurrentLength : sortedFileList[indInSortedFileList+1].FileOffset.AbsoluteOffset) - off_target.AbsoluteOffset - 4;
			await s.FillCacheForRead((int)fileSize);
			action(fileSize);
			s.Goto(off_current);
		}
	}
}
