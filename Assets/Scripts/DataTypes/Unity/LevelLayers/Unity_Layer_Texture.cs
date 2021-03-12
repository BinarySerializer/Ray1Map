using UnityEngine;

namespace R1Engine {
	public class Unity_Layer_Texture : Unity_Layer {
		public Texture2D Texture { get; set; }

		public Unity_Map.MapLayer Layer { get; set; } = Unity_Map.MapLayer.Middle;
		public Unity_Map.FreeCameraSettings Settings3D { get; set; }

		// Keep renderers here
		public SpriteRenderer Graphics { get; set; }
	}
}