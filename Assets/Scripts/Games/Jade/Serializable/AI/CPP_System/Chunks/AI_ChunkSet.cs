using BinarySerializer;

namespace Ray1Map.Jade {
	public class AI_ChunkSet : AI_ChunkPredicated {
		public uint ChunksCount { get; set; }
		public int[] ChunkIDs { get; set; }
		public int Active { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			base.SerializeImpl(s);
			ChunksCount = s.Serialize<uint>(ChunksCount, name: nameof(ChunksCount));
			ChunkIDs = s.SerializeArray<int>(ChunkIDs, ChunksCount, name: nameof(ChunkIDs));
			Active = s.Serialize<int>(Active, name: nameof(Active));
		}
	}
}
