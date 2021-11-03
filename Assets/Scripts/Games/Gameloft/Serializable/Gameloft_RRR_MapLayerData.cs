using BinarySerializer;
using Ray1Map;

namespace Ray1Map.Gameloft
{
	public class Gameloft_RRR_MapLayerData : Gameloft_Resource {
		public Gameloft_RRR_MapLayerHeader Header { get; set; } // Set in onPreSerialize
		public MapTile[] TileMap { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			switch (Header.Type) {
				case Gameloft_RRR_MapLayerHeader.LayerType.Graphics:
                    TileMap = s.SerializeObjectArray<MapTile>(TileMap, Header.Width * Header.Height, name: nameof(TileMap));
					break;
				case Gameloft_RRR_MapLayerHeader.LayerType.Collision:

					if (TileMap == null)
						TileMap = new MapTile[Header.Width * Header.Height];

                    for (int i = 0; i < TileMap.Length;)
                    {
                        s.SerializeBitValues<byte>(bitFunc =>
                        {
							int bpt = 2; // bits per tile
							if (s.GetR1Settings().GameModeSelection == GameModeSelection.RaymanRavingRabbidsMobile_128x128_CZ) {
								bpt = 1;
							}
							for (int j = 0; j < (8/bpt); j++) // 4 * 2 = 8 (2 bits per collision type)
							{
								if (i >= TileMap.Length)
									break;

								if (TileMap[i] == null)
									TileMap[i] = new MapTile();

								TileMap[i].CollisionType = (ushort)bitFunc(TileMap[i].CollisionType, bpt, name: $"{nameof(TileMap)}[{i}]");

								i++;
							}
						});
                    }

					break;
			}
		}
	}
}