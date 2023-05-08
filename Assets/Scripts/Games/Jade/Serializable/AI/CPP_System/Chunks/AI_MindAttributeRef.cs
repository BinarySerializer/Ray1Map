using BinarySerializer;

namespace Ray1Map.Jade {
	public class AI_MindAttributeRef : AI_MindAttribute {
		public AI_AttributeOpType OpType { get; set; }
		public ushort AttributeId { get; set; }
		public ushort AttributeRefId { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			base.SerializeImpl(s);
			OpType = s.Serialize<AI_AttributeOpType>(OpType, name: nameof(OpType));
			AttributeId = s.Serialize<ushort>(AttributeId, name: nameof(AttributeId));
			if (Version >= 3) AttributeRefId = s.Serialize<ushort>(AttributeRefId, name: nameof(AttributeRefId));
		}
	}
}
