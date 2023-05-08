using BinarySerializer;

namespace Ray1Map.Jade {
	public class AI_Tools_StringPool : BinarySerializable {
		public int Version { get; set; }
		public uint StringBufferSize { get; set; }
		public byte[] StringBuffer { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Version = s.Serialize<int>(Version, name: nameof(Version));
			StringBufferSize = s.Serialize<uint>(StringBufferSize, name: nameof(StringBufferSize));
			StringBuffer = s.SerializeArray<byte>(StringBuffer, StringBufferSize, name: nameof(StringBuffer));
		}
	}
}
