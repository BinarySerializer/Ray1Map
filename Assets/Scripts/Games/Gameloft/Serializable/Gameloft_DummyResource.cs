using BinarySerializer;

namespace Ray1Map.Gameloft
{
	public class Gameloft_DummyResource : Gameloft_Resource {
		public byte[] Data { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Data = s.SerializeArray<byte>(Data, ResourceSize, name: nameof(Data));
		}
	}
}