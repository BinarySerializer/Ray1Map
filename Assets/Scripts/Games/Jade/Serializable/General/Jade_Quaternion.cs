using BinarySerializer;

namespace Ray1Map.Jade {
	public class Jade_Quaternion : BinarySerializable, ISerializerShortLog {
		public float X { get; set; }
		public float Y { get; set; }
		public float Z { get; set; }
		public float W { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			X = s.Serialize<float>(X, name: nameof(X));
			Y = s.Serialize<float>(Y, name: nameof(Y));
			Z = s.Serialize<float>(Z, name: nameof(Z));
			W = s.Serialize<float>(W, name: nameof(W));
		}

        public string ShortLog => ToString();
        public override string ToString() => $"Quaternion({X}, {Y}, {Z}, {W})";
	}
}
