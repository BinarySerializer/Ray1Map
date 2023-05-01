using BinarySerializer;

namespace Ray1Map.Jade
{
	public class AG_Property : BinarySerializable {
		public int Id { get; set; }
		public float Value { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Id = s.Serialize<int>(Id, name: nameof(Id));
			Value = s.Serialize<float>(Value, name: nameof(Value));
		}
	}
}