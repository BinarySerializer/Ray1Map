using BinarySerializer;

namespace Ray1Map.Jade {
	public class BIG_PakFileInfo : BinarySerializable {
		public uint UncompressedSize { get; set; }
		public uint CompressedSize { get; set; }
		public uint MetaSize { get; set; } // Serialized right after data
		public uint Unknown { get; set; }
		public ulong FileOffset { get; set; }

		public bool IsCompressed => CompressedSize != 0;

		public override void SerializeImpl(SerializerObject s) {
			UncompressedSize = s.Serialize<uint>(UncompressedSize, name: nameof(UncompressedSize));
			CompressedSize = s.Serialize<uint>(CompressedSize, name: nameof(CompressedSize));
			MetaSize = s.Serialize<uint>(MetaSize, name: nameof(MetaSize));
			Unknown = s.Serialize<uint>(Unknown, name: nameof(Unknown));
			FileOffset = s.Serialize<ulong>(FileOffset, name: nameof(FileOffset));
		}
	}
}
