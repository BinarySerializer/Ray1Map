using UnityEngine;

namespace R1Engine {
	public class Unity_Layer_Texture : Unity_Layer {
		public Texture2D Texture { get; set; }

		public Texture2D[] TextureFrames { get; set; }
		public float AnimSpeed { get; set; }

		public Unity_Map.MapLayer Layer { get; set; } = Unity_Map.MapLayer.Middle;
		public Unity_Map.FreeCameraSettings Settings3D { get; set; }
		public override bool ShowIn3DView => Settings3D != null;
		public bool IsAnimated => TextureFrames != null;

		// Keep renderers here
		public SpriteRenderer Graphics { get; set; }

		public override void SetVisible(bool visible) {
			if (Graphics != null) {
				if (Graphics.gameObject.activeSelf != visible) {
					Graphics.gameObject.SetActive(visible);
				}
			}
		}
	}
}