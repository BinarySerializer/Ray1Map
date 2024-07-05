using System;
using BinarySerializer;
using System.Threading.Tasks;

namespace Ray1Map.Jade {
	public class BIG_PakFile : BinarySerializable {
		public static uint HeaderLength => 24;

        public byte PakVersion { get; set; }
        public uint Priority { get; set; }
		public uint Uint_0C { get; set; }
		public uint NumEntries { get; set; }
		public uint FooterSize { get; set; }

		public Pointer FilesBaseOffset { get; set; }
		public Pointer FileTableOffset { get; set; }

		public BIG_PakFileTableEntry[] FileTable { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			s.SerializeMagicString("BPAK", 4, name: "ID");
            PakVersion = s.Serialize<byte>(PakVersion, name: nameof(PakVersion));
            s.Align();
            Priority = s.Serialize<uint>(Priority, name: nameof(Priority));
			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_BGE_Anniversary)) {
				Uint_0C = s.Serialize<uint>(Uint_0C, name: nameof(Uint_0C));
			}
			NumEntries = s.Serialize<uint>(NumEntries, name: nameof(NumEntries));
			FooterSize = s.Serialize<uint>(FooterSize, name: nameof(FooterSize));
			FilesBaseOffset = s.CurrentPointer;
			FileTableOffset = s.CurrentBinaryFile.StartPointer + s.CurrentBinaryFile.Length - FooterSize;
		}

		public async Task SerializeFileTable(SerializerObject s) {
			Pointer off_current = s.CurrentPointer;
			Pointer off_target = FileTableOffset;
			s.Goto(off_target);

			await s.FillCacheForReadAsync(FooterSize);
			FileTable = s.SerializeObjectArray<BIG_PakFileTableEntry>(FileTable, NumEntries, name: nameof(FileTable));
			
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
