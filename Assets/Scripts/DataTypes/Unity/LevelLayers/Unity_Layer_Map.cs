
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
		public override bool ShowIn3DView { get => Map.Settings3D != null; }

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