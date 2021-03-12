using UnityEngine;

namespace R1Engine {
	public class Unity_Layer_GameObject : Unity_Layer {
		public override bool ShowIn3DView { get; }
		public override bool IsAnimated { get; }
		public Vector2 Dimensions { get; set; }

		// Renderers
		public GameObject Graphics { get; set; }
		public GameObject Collision { get; set; }

		public override void SetVisible(bool visible) {
			if (Graphics != null) {
				if (Graphics.activeSelf != visible) {
					Graphics.SetActive(visible);
				}
			}
			if (Collision != null) {
				if (Collision.activeSelf != visible) {
					Collision.SetActive(visible);
				}
			}
		}

		public override Vector2Int GetDimensions(int cellSize, int? cellSizeOverrideCollision) {
			return new Vector2Int(Mathf.CeilToInt(Dimensions.x / cellSize), Mathf.CeilToInt(Dimensions.y / cellSize));
		}

		public Unity_Layer_GameObject(bool showIn3DView, bool isAnimated = false) {
			ShowIn3DView = showIn3DView;
			IsAnimated = isAnimated;
		}
	}
}