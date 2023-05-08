using BinarySerializer;

namespace Ray1Map.Jade {
	public class AI_BehaviourInline : AI_Behaviour {
		public int CodeStringId { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			base.SerializeImpl(s);
			CodeStringId = s.Serialize<int>(CodeStringId, name: nameof(CodeStringId));
		}
	}
}
