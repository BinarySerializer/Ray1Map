using BinarySerializer;

namespace R1Engine.Jade {
	public class WOR_MagmaGroup : Jade_File {
		public byte[] Data { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			// TODO: Maybe properly parse this later on
			Data = s.SerializeArray<byte>(Data, FileSize, name: nameof(Data));
		}
	}
}