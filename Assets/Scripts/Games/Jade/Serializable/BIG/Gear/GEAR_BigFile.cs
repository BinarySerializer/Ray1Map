using System;
using System.Threading.Tasks;
using BinarySerializer;

namespace Ray1Map.Jade {
	public class GEAR_BigFile : BinarySerializable {
		public const string GearIDCheck = "[ GEAR BigFile ]";
		public string GearID { get; set; }
		public uint UInt0 { get; set; }
		public uint UInt1 { get; set; }
		public uint FilesCount { get; set; }
		public byte[] UnknownBytes { get; set; }
		public Pointer[] FileOffsets { get; set; }
		public uint[] FileSizes { get; set; }
		public int[] FileCRC32 { get; set; }
		public bool[] FileIsCompressed { get; set; }

		public static uint HeaderSize => 0x20;
		public uint ArraysSize => FilesCount * (8 + 4 + 4 + 1);

		public override void SerializeImpl(SerializerObject s) {
			GearID = s.SerializeString(GearID, length: 0x10, encoding: Jade_BaseManager.Encoding, name: nameof(GearID));
			if(GearID != GearIDCheck)
				throw new Exception($"Parsing failed: File at Offset {Offset} was not of type {GetType()}");
			UInt0 = s.Serialize<uint>(UInt0, name: nameof(UInt0));
			UInt1 = s.Serialize<uint>(UInt1, name: nameof(UInt1));
			FilesCount = s.Serialize<uint>(FilesCount, name: nameof(FilesCount));
			UnknownBytes = s.SerializeArray<byte>(UnknownBytes, 4, name: nameof(UnknownBytes));
		}
		public void SerializeArrays(SerializerObject s) {
			s.DoAt(Offset + HeaderSize, () => {
				FileOffsets = s.SerializePointerArray(FileOffsets, FilesCount, size: PointerSize.Pointer64, name: nameof(FileOffsets));
				FileSizes = s.SerializeArray<uint>(FileSizes, FilesCount, name: nameof(FileSizes));
				FileCRC32 = s.SerializeArray<int>(FileCRC32, FilesCount, name: nameof(FileCRC32));
				FileIsCompressed = s.SerializeArray<bool>(FileIsCompressed, FilesCount, name: nameof(FileIsCompressed));
			});
		}

		public async Task SerializeFile(SerializerObject s, int crc, Action<uint> action) {
			var fileIndex = FileCRC32.FindItemIndex(f_crc => f_crc == crc);
			if(fileIndex == -1) return;
			var fileSize = FileSizes[fileIndex];
			var fileOffset = FileOffsets[fileIndex];
			var fileIsCompressed = FileIsCompressed[fileIndex];

			Pointer off_current = s.CurrentPointer;
			Pointer off_target = fileOffset;
			s.Goto(off_target);
			await s.FillCacheForReadAsync(fileSize);
			s.DoEncoded(fileIsCompressed ? new Jade_ZlibEncoder(fileSize) : null, () => {
				action(fileSize);
			});
			s.Goto(off_current);
		}
	}
}
