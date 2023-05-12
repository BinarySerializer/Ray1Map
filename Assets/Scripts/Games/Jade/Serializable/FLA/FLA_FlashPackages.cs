using BinarySerializer;
using System.Linq;

namespace Ray1Map.Jade {
	public class FLA_FlashPackages : Jade_File {
		public uint Version { get; set; }
		public uint FilesCount { get; set; }
		public FileInfo[] Files { get; set; }
		public byte[] Data { get; set; }

		protected override void SerializeFile(SerializerObject s) {
			Version = s.Serialize<uint>(Version, name: nameof(Version));
			FilesCount = s.Serialize<uint>(FilesCount, name: nameof(FilesCount));
			Files = s.SerializeObjectArray<FileInfo>(Files, FilesCount, name: nameof(Files));
			var baseOffset = s.CurrentPointer;
			foreach(var f in Files) f.SerializeFile(s, baseOffset);

			// Go to end of file
			s.Goto(baseOffset + (Files != null ? Files.Max(f => f.FileOffset + f.FileSize) : 0));
		}

		public class FileInfo : BinarySerializable {
			public string Name { get; set; }
			public uint FileSize { get; set; }
			public uint FileOffset { get; set; }

			public byte[] Data { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Name = s.SerializeString(Name, length: 72 - 8, name: nameof(Name));
				FileSize = s.Serialize<uint>(FileSize, name: nameof(FileSize));
				FileOffset = s.Serialize<uint>(FileOffset, name: nameof(FileOffset));
			}

			public void SerializeFile(SerializerObject s, Pointer baseOffset) {
				s.DoAt(baseOffset + FileOffset, () => {
					Data = s.SerializeArray<byte>(Data, FileSize, name: nameof(Data));
				});
			}
		}
	}
}
