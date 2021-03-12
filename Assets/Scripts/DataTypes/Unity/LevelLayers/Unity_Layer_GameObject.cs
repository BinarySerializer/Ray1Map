using UnityEngine;

namespace R1Engine {
	public class Unity_Layer_GameObject : Unity_Layer {
		// Keep renderers here
		public GameObject Graphics { get; set; }
		public GameObject Collision { get; set; }

		public override bool ShowIn3DView { get; }

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

		public Unity_Layer_GameObject(bool showIn3DView) {
			ShowIn3DView = showIn3DView;
		}
	}
}