using BinarySerializer;

namespace Ray1Map.GEN {
	public class GEN_Palette : BinarySerializable {
		public byte[] Header { get; set; }
		public RGBA8888Color[] Palette { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Header = s.SerializeArray<byte>(Header, 0x18, name: nameof(Header));
			Palette = s.SerializeObjectArray<RGBA8888Color>(Palette, 256, name: nameof(Palette));
		}
	}
}