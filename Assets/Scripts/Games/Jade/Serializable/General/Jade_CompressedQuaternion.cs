using BinarySerializer;

namespace Ray1Map.Jade {
	public class Jade_CompressedQuaternion : BinarySerializable, ISerializerShortLog {
		public short X { get; set; }
		public short Y { get; set; }
		public short Z { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			X = s.Serialize<short>(X, name: nameof(X));
			Y = s.Serialize<short>(Y, name: nameof(Y));
			Z = s.Serialize<short>(Z, name: nameof(Z));
		}

        public string ShortLog => ToString();
        public override string ToString() => $"CompressedQuaternion({X}, {Y}, {Z})";
	}
}
