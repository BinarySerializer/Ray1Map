using BinarySerializer;

namespace Ray1Map.GEN {
	public class GEN_Palette : BinarySerializable {
		public byte[] Header { get; set; }
		public SerializableColor[] Palette { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Header = s.SerializeArray<byte>(Header, 0x18, name: nameof(Header));
			Palette = s.SerializeIntoArray<SerializableColor>(Palette, 256, BytewiseColor.RGBA8888, name: nameof(Palette));
		}
	}
}