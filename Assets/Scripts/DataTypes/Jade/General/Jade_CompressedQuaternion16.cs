using BinarySerializer;

namespace R1Engine.Jade {
	public class Jade_CompressedQuaternion16 : BinarySerializable {
		public short X { get; set; }
		public short Y { get; set; }
		public short Z { get; set; }
		public short W { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			X = s.Serialize<short>(X, name: nameof(X));
			Y = s.Serialize<short>(Y, name: nameof(Y));
			Z = s.Serialize<short>(Z, name: nameof(Z));
			W = s.Serialize<short>(W, name: nameof(W));
		}
        public override bool UseShortLog => true;
		public override string ToString() => $"CompressedQuaternion16({X}, {Y}, {Z}, {W})";
	}
}
