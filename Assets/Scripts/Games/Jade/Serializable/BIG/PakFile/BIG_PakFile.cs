using System;
using BinarySerializer;
using System.Threading.Tasks;

namespace Ray1Map.Jade {
	public class BIG_PakFile : BinarySerializable {
		public static uint HeaderLength => 24;

		public uint UInt0 { get; set; }
		public uint UInt1 { get; set; }
		public uint UInt2 { get; set; }
		public uint FilesCount { get; set; }
		public uint FileTableSize { get; set; }

		public Pointer FilesBaseOffset { get; set; }
		public Pointer FileTableOffset { get; set; }

		public BIG_PakFileTableEntry[] FileTable { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			s.SerializeMagicString("BPAK", 4);
			UInt0 = s.Serialize<uint>(UInt0, name: nameof(UInt0));
			UInt1 = s.Serialize<uint>(UInt1, name: nameof(UInt1));
			UInt2 = s.Serialize<uint>(UInt2, name: nameof(UInt2));
			FilesCount = s.Serialize<uint>(FilesCount, name: nameof(FilesCount));
			FileTableSize = s.Serialize<uint>(FileTableSize, name: nameof(FileTableSize));
			FilesBaseOffset = s.CurrentPointer;
			FileTableOffset = s.CurrentBinaryFile.StartPointer + s.CurrentBinaryFile.Length - FileTableSize;
		}

		public async Task SerializeFileTable(SerializerObject s) {
			Pointer off_current = s.CurrentPointer;
			Pointer off_target = FileTableOffset;
			s.Goto(off_target);

			await s.FillCacheForReadAsync(FileTableSize);
			FileTable = s.SerializeObjectArray<BIG_PakFileTableEntry>(FileTable, FilesCount, name: nameof(FileTable));
			
			s.Goto(off_current);
		}

		public async Task SerializeFile(SerializerObject s, int fileIndex, Action<uint, bool> action) {
			var info = FileTable[fileIndex].Info;
			Pointer off_current = s.CurrentPointer;
			Pointer off_target = FilesBaseOffset + (long)info.FileOffset;
			var actualSize = info.IsCompressed ? info.CompressedSize : info.UncompressedSize;

			s.Goto(off_target);
			const bool IsBranch = false;
			await s.FillCacheForReadAsync(actualSize);
			var FileSize = info.UncompressedSize;
			s.DoEncoded(info.IsCompressed ? new Lz4Encoder(info.CompressedSize, info.UncompressedSize) : null,
				() => { action(FileSize, IsBranch); });
			s.Goto(off_current);
		}
	}
}
