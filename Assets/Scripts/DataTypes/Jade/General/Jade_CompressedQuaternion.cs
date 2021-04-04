using BinarySerializer;

namespace R1Engine.Jade {
	public class Jade_CompressedQuaternion : BinarySerializable {
		public short X { get; set; }
		public short Y { get; set; }
		public short Z { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			X = s.Serialize<short>(X, name: nameof(X));
			Y = s.Serialize<short>(Y, name: nameof(Y));
			Z = s.Serialize<short>(Z, name: nameof(Z));
		}
		public override string ToString() {
			return $"CompressedQuaternion({X}, {Y}, {Z})";
		}

		public override bool IsShortLog => true;
		public override string ShortLog => ToString();
	}
}
