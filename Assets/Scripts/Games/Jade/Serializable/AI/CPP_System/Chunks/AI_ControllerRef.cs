using BinarySerializer;

namespace Ray1Map.Jade {
	public class AI_ControllerRef : AI_Controller {
		public int ControllerId { get; set; }
		public AI_AttributeValue Value { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			base.SerializeImpl(s);
			if (Version >= 5) {
				ControllerId = s.Serialize<int>(ControllerId, name: nameof(ControllerId));
				Value = s.SerializeObject<AI_AttributeValue>(Value, name: nameof(Value));
			}
		}
	}
}
