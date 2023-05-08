using BinarySerializer;

namespace Ray1Map.Jade {
	public class AI_AttributeValue : BinarySerializable {
		public bool Pre_HasAttributeId { get; set; } = true;

		public int Value { get; set; }
		public float ValueFloat { get; set; }
		public AI_AttributeType Type { get; set; }
		public ushort AttributeId { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Value = s.Serialize<int>(Value, name: nameof(Value));
			Type = s.Serialize<AI_AttributeType>(Type, name: nameof(Type));
			if (Type == AI_AttributeType.Real) {
				s.DoAt(Offset, () => {
					ValueFloat = s.Serialize<float>(ValueFloat, name: nameof(ValueFloat));
				});
			}
			if (Pre_HasAttributeId) AttributeId = s.Serialize<ushort>(AttributeId, name: nameof(AttributeId));
		}
	}
}
