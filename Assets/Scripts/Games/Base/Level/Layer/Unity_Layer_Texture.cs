using System.Linq;
using UnityEngine;

namespace Ray1Map {
	public class Unity_Layer_Texture : Unity_Layer {
		public Texture2D Texture { get; set; }
		public Texture2D[] Textures { get; set; }
		public float AnimSpeed { get; set; }

		public Unity_Map.MapLayer Layer { get; set; } = Unity_Map.MapLayer.Middle;
		public Unity_Map.FreeCameraSettings Settings3D { get; set; }
		public override bool ShowIn3DView => Settings3D != null;
		public override bool IsAnimated => Textures != null;
		public int CellSize { get; set; } // Needed to calculate width & height in tiles

		// Renderers
		public SpriteRenderer Graphics { get; set; }

		// Additional data for renderers
		public float CurrentAnimatedTexture { get; set; }
		public Sprite Sprite { get; set; }
		public Sprite[] Sprites { get; set; }
		public Texture2D MainTexture => IsAnimated && Textures.Length > 0 ? Textures[0] : Texture;
		public Sprite MainSprite => IsAnimated && Sprites.Length > 0 ? Sprites[0] : Sprite;

		public override void SetVisible(bool visible) {
			if (Graphics != null) {
				if (Graphics.gameObject.activeSelf != visible) {
					Graphics.gameObject.SetActive(visible);
				}
			}
		}

		public void InitSprites(int pixelsPerUnit) {
			if(Texture != null) Sprite = CreateSprite(Texture, pixelsPerUnit);
			if (Textures != null) {
				Sprites = Textures.Select(tex => CreateSprite(tex, pixelsPerUnit)).ToArray();
			}
		}

		public Sprite CreateSprite(Texture2D tex, int pixelsPerUnit) {
			return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 1), pixelsPerUnit, 0, SpriteMeshType.FullRect);
		}

		public override Rect GetDimensions(int cellSize, int? cellSizeOverrideCollision) {
			var tex = MainTexture;
			return new Rect(
				0f,
				0f,
				Mathf.CeilToInt(tex.width / (float)cellSize),
				Mathf.CeilToInt(tex.height / (float)cellSize)
				);
		}
	}
}