using BinarySerializer;

namespace Ray1Map.Gameloft
{
	public class Gameloft_RRR_MapLayerHeader : Gameloft_Resource {
		public ushort Width { get; set; }
		public ushort Height { get; set; }
		public LayerType Type { get; set; } // Set in onPreSerialize

		public override void SerializeImpl(SerializerObject s) {
			Width = s.Serialize<ushort>(Width, name: nameof(Width));
			Height = s.Serialize<ushort>(Height, name: nameof(Height));
		}

		public enum LayerType {
			Graphics,
			Collision
		}
	}
}