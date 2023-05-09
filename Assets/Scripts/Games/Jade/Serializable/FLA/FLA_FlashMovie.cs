using BinarySerializer;

namespace Ray1Map.Jade {
	public class FLA_FlashMovie : Jade_File {
		public byte[] Data { get; set; }

		protected override void SerializeFile(SerializerObject s) {
			Data = s.SerializeArray<byte>(Data, FileSize, name: nameof(Data));
		}
	}
}
