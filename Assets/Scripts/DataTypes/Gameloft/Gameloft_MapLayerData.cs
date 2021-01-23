namespace R1Engine
{
	public class Gameloft_MapLayerData : Gameloft_Resource {
		public Gameloft_MapLayerHeader Header { get; set; } // Set in onPreSerialize
		public byte[] Data { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			switch (Header.Type) {
				case Gameloft_MapLayerHeader.LayerType.Graphics:
					Data = s.SerializeArray<byte>(Data, Header.Width * Header.Height, name: nameof(Data));
					break;
				case Gameloft_MapLayerHeader.LayerType.Collision:
					Data = s.SerializeArray<byte>(Data, Header.Width * Header.Height / 4, name: nameof(Data));
					// For collision, each tile is 2 bits (so 4 types max)
					break;
			}
		}
	}
}