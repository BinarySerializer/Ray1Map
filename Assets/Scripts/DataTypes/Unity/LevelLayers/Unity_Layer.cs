
using UnityEngine;

namespace R1Engine {
	public abstract class Unity_Layer {
		public string Name { get; set; }

		public abstract void SetVisible(bool visible);
		public abstract bool ShowIn3DView { get; }
		public abstract bool IsAnimated { get; }

		public abstract Vector2Int GetDimensions(int cellSize, int? cellSizeOverrideCollision);
	}
}