using BinarySerializer;

namespace Ray1Map.Jade {
	// Found in GRA_GrassField_CreateFromBuffer
	public class GRA_GrassTile : BinarySerializable {
		public byte Z { get; set; }
		public byte T { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Z = s.Serialize<byte>(Z, name: nameof(Z));
			T = s.Serialize<byte>(T, name: nameof(T));
		}
	}
}
