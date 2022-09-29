using BinarySerializer;

namespace Ray1Map.Jade {
	public class Jade_CompressedQuaternion10 : BinarySerializable, ISerializerShortLog {
		public uint Data { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Data = s.Serialize<uint>(Data, name: nameof(Data));
		}

        public string ShortLog => ToString();
        public override string ToString() => $"CompressedQuaternion10({Data})";
	}
}
