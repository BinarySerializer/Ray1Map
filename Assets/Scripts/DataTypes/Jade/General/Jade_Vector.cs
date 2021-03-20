namespace R1Engine.Jade {
	public class Jade_Vector : R1Serializable {
		public float X { get; set; }
		public float Y { get; set; }
		public float Z { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			X = s.Serialize<float>(X, name: nameof(X));
			Y = s.Serialize<float>(Y, name: nameof(Y));
			Z = s.Serialize<float>(Z, name: nameof(Z));
		}
		public override string ToString() {
			return $"Vector({X}, {Y}, {Z})";
		}
	}
}
