using BinarySerializer;

namespace Ray1Map.Jade {
	public class AI_PredicateSetRef : AI_Predicate {
		public int PredicateSetId { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			base.SerializeImpl(s);
			PredicateSetId = s.Serialize<int>(PredicateSetId, name: nameof(PredicateSetId));
		}
	}
}
