using BinarySerializer;

namespace Ray1Map.Jade {
	public class AI_Predicate : AI_Chunk {
		public int PredicateValueId { get; set; }
		public int Inverse { get; set; } // Boolean

		public override void SerializeImpl(SerializerObject s) {
			base.SerializeImpl(s);
			PredicateValueId = s.Serialize<int>(PredicateValueId, name: nameof(PredicateValueId));
			Inverse = s.Serialize<int>(Inverse, name: nameof(Inverse));
		}
	}
}
