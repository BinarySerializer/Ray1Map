using BinarySerializer;

namespace Ray1Map.Jade {
	public abstract class AI_ChunkPredicated : AI_Chunk {
		public uint PredicatesCount { get; set; }
		public int[] PredicatesIDs { get; set; }
		public AI_PredicateOpType Operator { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			base.SerializeImpl(s);
			PredicatesCount = s.Serialize<uint>(PredicatesCount, name: nameof(PredicatesCount));
			PredicatesIDs = s.SerializeArray<int>(PredicatesIDs, PredicatesCount, name: nameof(PredicatesIDs));
			Operator = s.Serialize<AI_PredicateOpType>(Operator, name: nameof(Operator));
		}
	}
}
