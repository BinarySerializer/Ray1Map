﻿
using UnityEngine;

namespace Ray1Map {
	public abstract class Unity_Layer {
		public string Name { get; set; }
		public string ShortName { get; set; }

		public abstract void SetVisible(bool visible);
		public abstract bool ShowIn3DView { get; }
		public abstract bool IsAnimated { get; }

		public abstract Rect GetDimensions(int cellSize, int? cellSizeOverrideCollision);

		public Vector3 PositionOffset { get; set; }
	}
}