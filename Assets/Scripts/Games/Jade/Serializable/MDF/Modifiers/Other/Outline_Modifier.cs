using BinarySerializer;

namespace Ray1Map.Jade {
	public class Outline_Modifier : MDF_Modifier {
		public byte Version { get; set; }
		public byte Flags { get; set; }
		public SerializableColor Color { get; set; }
		public float Width { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Version = s.Serialize<byte>(Version, name: nameof(Version));
			if (Version >= 2) {
				Flags = s.Serialize<byte>(Flags, name: nameof(Flags));
				Color = s.SerializeInto<SerializableColor>(Color, BitwiseColor.RGBA8888, name: nameof(Color));
				Width = s.Serialize<float>(Width, name: nameof(Width));
			}
		}
	}
}
