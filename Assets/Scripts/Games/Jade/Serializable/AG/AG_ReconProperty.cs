using BinarySerializer;

namespace Ray1Map.Jade
{
	public class AG_ReconProperty : BinarySerializable {
		public int Group { get; set; }
		public int Property { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Group = s.Serialize<int>(Group, name: nameof(Group));
			Property = s.Serialize<int>(Property, name: nameof(Property));
		}
	}
}