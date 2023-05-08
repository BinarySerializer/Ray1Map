using BinarySerializer;

namespace Ray1Map.Jade {
	public class AI_ChunkWeighted<T> : AI_Chunk where T : AI_Chunk {
		public int ChunkId { get; set; }
		public int HasWeight { get; set; }

		public AI_AttributeValue Weight { get; set; }
		public AI_AttributeValue MaxTimes { get; set; }
		public AI_AttributeValue TimesBeforeNext { get; set; }
		public AI_AttributeValue NotPlayedSince { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			base.SerializeImpl(s);
			ChunkId = s.Serialize<int>(ChunkId, name: nameof(ChunkId));
			HasWeight = s.Serialize<int>(HasWeight, name: nameof(HasWeight));

			Weight = s.SerializeObject<AI_AttributeValue>(Weight, name: nameof(Weight));
			MaxTimes = s.SerializeObject<AI_AttributeValue>(MaxTimes, name: nameof(MaxTimes));
			TimesBeforeNext = s.SerializeObject<AI_AttributeValue>(TimesBeforeNext, name: nameof(TimesBeforeNext));
			if (Version >= 6) NotPlayedSince = s.SerializeObject<AI_AttributeValue>(NotPlayedSince, name: nameof(NotPlayedSince));
		}
	}
}
