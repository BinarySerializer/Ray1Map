using BinarySerializer;

namespace Ray1Map.Jade {
	public class WOR_MagmaGroup : Jade_File {
		public byte[] Data { get; set; }

		protected override void SerializeFile(SerializerObject s) {
			// TODO: Maybe properly parse this later on
			Data = s.SerializeArray<byte>(Data, FileSize, name: nameof(Data));
		}
	}
}