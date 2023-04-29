using BinarySerializer;

namespace Ray1Map.Jade
{
	public class AG_DynamicEnum : BinarySerializable {
		public uint Count { get; set; }
		public uint[] Values { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Count = s.Serialize<uint>(Count, name: nameof(Count));
			Values = s.SerializeArray<uint>(Values, 64, name: nameof(Values));
		}
	}
}