using BinarySerializer;
using System.Text;

namespace R1Engine.Jade {
	public class SND_WaveChunk_Cue : SND_WaveChunk {
		public uint Count { get; set; }
		public CuePoint[] Points { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Count = s.Serialize<uint>(Count, name: nameof(Count));
			Points = s.SerializeObjectArray<CuePoint>(Points, Count, name: nameof(Points));

			EditorSize = 4 + 24 * Count;
		}

		public class CuePoint : BinarySerializable {
			public uint ID { get; set; }
			public uint Position { get; set; }
			public string DataChunkID { get; set; }

			public uint ChunkStart { get; set; }
			public uint BlockStart { get; set; }
			public uint SampleOffset { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				ID = s.Serialize<uint>(ID, name: nameof(ID));
				Position = s.Serialize<uint>(Position, name: nameof(Position));
				DataChunkID = s.SerializeString(DataChunkID, 4, Encoding.ASCII, name: nameof(DataChunkID));

				ChunkStart = s.Serialize<uint>(ChunkStart, name: nameof(ChunkStart));
				BlockStart = s.Serialize<uint>(BlockStart, name: nameof(BlockStart));
				SampleOffset = s.Serialize<uint>(SampleOffset, name: nameof(SampleOffset));
			}
		}
	}
}
