using BinarySerializer;
using System.Linq;

namespace Ray1Map.Jade {
	public class SND_Insert : Jade_File {
		public override string Export_Extension => IsFade ? "fad" : "ins";
		public byte[] WaveData { get; set; }

		public string Magic { get; set; }
		public uint Version { get; set; }
		public uint DataSize { get; set; }
		public uint Flags { get; set; }

		public SND_InsertChunk_Container[] Chunks { get; set; }

		public bool IsFade { get; set; }

		public const string Inst_Magic = "tsni"; // Insert 

		protected override void SerializeFile(SerializerObject s) {
			//WaveData = s.SerializeArray<byte>(WaveData, FileSize, name: nameof(WaveData));

			Magic = s.SerializeString(Magic, length: 4, name: nameof(Magic));
			Version = s.Serialize<uint>(Version, name: nameof(Version));
			DataSize = s.Serialize<uint>(DataSize, name: nameof(DataSize));

			if (Magic != Inst_Magic) {
				throw new BinarySerializableException(this, $"Parsing failed: File was not of type {typeof(SND_Insert).Name}");
			}
			if (DataSize != FileSize - 12) {
				s?.SystemLogger?.LogWarning($"{Key}: Incorrect Insert FileSize {FileSize} vs DataSize {DataSize}");
			}
			Flags = s.Serialize<uint>(Flags, name: nameof(Flags));

			uint currentSize = 4; // Flags
			Chunks = s.SerializeObjectArrayUntil<SND_InsertChunk_Container>(Chunks, c => {
				currentSize += c.ChunkSize;
				return currentSize >= DataSize;
			}, name: nameof(Chunks));

			if (DataSize != 4 + Chunks.Sum(c => c.ChunkSize)) {
				s?.SystemLogger?.LogWarning($"{Key}: Incorrect data size for Insert");
			}
		}
	}
}
