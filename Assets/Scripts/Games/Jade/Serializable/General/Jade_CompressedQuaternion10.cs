using BinarySerializer;

namespace Ray1Map.Jade {
	public class Jade_CompressedQuaternion10 : BinarySerializable {
		public uint Data { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Data = s.Serialize<uint>(Data, name: nameof(Data));
		}
        public override bool UseShortLog => true;
		public override string ToString() => $"CompressedQuaternion10({Data})";
	}
}
