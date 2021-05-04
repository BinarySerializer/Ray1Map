using Cysharp.Threading.Tasks;
using System;
using BinarySerializer;

namespace R1Engine.Jade {
	public class BIG_BigFile : BinarySerializable {
		public static readonly byte[] XORKey = new byte[] { 0xb3, 0x98, 0xcc, 0x66 };
		public static uint HeaderLength => 44;
		public uint TotalFatFilesLength => NumFat * (
			BIG_FatFile.HeaderLength
			+ SizeOfFat * BIG_FatFile.FileReference.StructSize
			+ SizeOfFat * BIG_FatFile.FileInfo.StructSize(this)
			+ SizeOfFat * BIG_FatFile.DirectoryInfo.StructSize);
		
		public string BIG_gst { get; set; }
		public uint Version { get; set; }
		public uint MaxFile { get; set; }
		public uint MaxDir { get; set; }
		public uint MaxKey { get; set; }
		public uint Root { get; set; }
		public int FirstFreeFile { get; set; }
		public int FirstFreeDIrectory { get; set; }
		public uint SizeOfFat { get; set; }
		public uint NumFat { get; set; }
		public Jade_Key UniverseKey { get; set; } // First file it loads

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
				FirstFreeDIrectory = s.Serialize<int>(FirstFreeDIrectory, name: nameof(FirstFreeDIrectory));
				SizeOfFat = s.Serialize<uint>(SizeOfFat, name: nameof(SizeOfFat));
				NumFat = s.Serialize<uint>(NumFat, name: nameof(NumFat));
			});
			UniverseKey = s.SerializeObject<Jade_Key>(UniverseKey, name: nameof(UniverseKey));
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
			uint FileSize = 0;
			await s.FillCacheForReadAsync(4);
			FileSize = s.Serialize<uint>(FileSize, name: nameof(FileSize));
			//var actualFileSize = (indInSortedFileList+1 >= sortedFileList.Length ? s.CurrentLength : sortedFileList[indInSortedFileList+1].FileOffset.AbsoluteOffset) - off_target.AbsoluteOffset - 4;
			/*if (s.GetR1Settings().EngineVersion == EngineVersion.Jade_BGE_HD) {
				FileSize = fat.FileInfos[fileIndex].FileSize;
			}*/
			await s.FillCacheForReadAsync((int)FileSize);
			action(FileSize);
			s.Goto(off_current);
		}

		public async UniTask SerializeAt(SerializerObject s, Pointer off_target, Action<uint> action) {
			Pointer off_current = s.CurrentPointer;
			s.Goto(off_target);
			await s.FillCacheForReadAsync(4);
			var fileSize = s.Serialize<uint>(default, name: "FileSize");
			//var actualFileSize = (indInSortedFileList+1 >= sortedFileList.Length ? s.CurrentLength : sortedFileList[indInSortedFileList+1].FileOffset.AbsoluteOffset) - off_target.AbsoluteOffset - 4;
			await s.FillCacheForReadAsync((int)fileSize);
			action(fileSize);
			s.Goto(off_current);
		}
	}
}
