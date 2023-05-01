using BinarySerializer;

namespace Ray1Map.Jade
{
	public class AG_Bitfield : BinarySerializable {
		public uint UInt0 { get; set; }
		public uint UInt1 { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			UInt0 = s.Serialize<uint>(UInt0, name: nameof(UInt0));
			UInt1 = s.Serialize<uint>(UInt1, name: nameof(UInt1));
		}
	}
}