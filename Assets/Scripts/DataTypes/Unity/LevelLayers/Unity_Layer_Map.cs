
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace R1Engine {
	public class Unity_Layer_Map : Unity_Layer {
		public int MapIndex { get; set; }
		public Unity_Map Map { get; set; }

		// Renderers
		public SpriteRenderer Graphics { get; set; }
		public Tilemap CollisionTilemap { get; set; }
		public TilemapRenderer Collision { get; set; }

		// Additional data for the renderers
		public int?[,][] TileIndexOverrides { get; set; }
		public Dictionary<Unity_AnimatedTile, List<Unity_AnimatedTile.Instance>> AnimatedTiles { get; set; }
		public bool HasAnimatedTiles { get; set; } = false;
		public override bool ShowIn3DView { get => Map.Settings3D != null; }

		public override bool IsAnimated => HasAnimatedTiles;

		public override Rect GetDimensions(int cellSize, int? cellSizeOverrideCollision) {
			var width = cellSizeOverrideCollision != null && Map.Type == Unity_Map.MapType.Collision ? (ushort)(Map.Width / (cellSize / (float)cellSizeOverrideCollision)) : Map.Width;
			var height = cellSizeOverrideCollision != null && Map.Type == Unity_Map.MapType.Collision ? (ushort)(Map.Height / (cellSize / (float)cellSizeOverrideCollision)) : Map.Height;
			return new Rect(0f, 0f, width, height);
		}

		public override void SetVisible(bool visible) {
			if (Graphics != null) {
				if (Graphics.gameObject.activeSelf != visible) {
					Graphics.gameObject.SetActive(visible);
				}
			}
			if (Collision != null) {
				if (Collision.gameObject.activeSelf != visible) {
					Collision.gameObject.SetActive(visible);
				}
			}
		}
	}
}