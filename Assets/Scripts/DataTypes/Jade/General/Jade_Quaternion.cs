using BinarySerializer;

namespace R1Engine.Jade {
	public class Jade_Quaternion : BinarySerializable {
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
		public override string ToString() {
			return $"Quaternion({X}, {Y}, {Z}, {W})";
		}

		public override bool IsShortLog => true;
		public override string ShortLog => ToString();
	}
}
