using BinarySerializer;

namespace Ray1Map.Jade {
	public class AI_BehaviourSetRef : AI_BehaviourSet {
		public int BehaviourSetId { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			base.SerializeImpl(s);
			BehaviourSetId = s.Serialize<int>(BehaviourSetId, name: nameof(BehaviourSetId));
		}
	}
}
