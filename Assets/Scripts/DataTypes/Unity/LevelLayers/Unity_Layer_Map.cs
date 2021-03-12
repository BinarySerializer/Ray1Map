
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace R1Engine {
	public class Unity_Layer_Map : Unity_Layer {
		public int MapIndex { get; set; }
		public Unity_Map Map => LevelEditorData.Level.Maps[MapIndex];

		// Keep renderers here
		public SpriteRenderer Graphics { get; set; }
		public Tilemap CollisionTilemap { get; set; }
		public TilemapRenderer Collision { get; set; }

		// Additional data for the renderers
		public int?[,][] TileIndexOverrides { get; set; }
		public Dictionary<Unity_AnimatedTile, List<Unity_AnimatedTile.Instance>> AnimatedTiles { get; set; }
	}
}