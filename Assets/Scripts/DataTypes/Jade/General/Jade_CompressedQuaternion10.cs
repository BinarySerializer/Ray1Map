using BinarySerializer;

namespace R1Engine.Jade {
	public class Jade_CompressedQuaternion10 : BinarySerializable {
		public uint Data { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Data = s.Serialize<uint>(Data, name: nameof(Data));
		}
		public override string ToString() {
			return $"CompressedQuaternion10({Data})";
		}

		public override bool IsShortLog => true;
		public override string ShortLog => ToString();
	}
}
