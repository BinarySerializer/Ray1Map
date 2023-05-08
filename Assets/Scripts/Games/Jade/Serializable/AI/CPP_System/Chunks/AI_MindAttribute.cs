using BinarySerializer;

namespace Ray1Map.Jade {
	public class AI_MindAttribute : AI_Chunk {
		public AI_AttributeValue Value { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			base.SerializeImpl(s);
			Value = s.SerializeObject<AI_AttributeValue>(Value, onPreSerialize: v => v.Pre_HasAttributeId = false, name: nameof(Value));
		}
	}
}
