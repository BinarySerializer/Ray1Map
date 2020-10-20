using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace R1Engine {
	public class Unity_TileTexture {
		public Texture2D texture;
		public Rect rect;

		private Dictionary<Flip, Color[]> Pixels = new Dictionary<Flip, Color[]>();
		[Flags]
		public enum Flip {
			None,
			Horizontal,
			Vertical
		}
		public Color[] GetPixels(bool flipX, bool flipY) {
			Flip flip = Flip.None;
			if (flipX) flip |= Flip.Horizontal;
			if (flipY) flip |= Flip.Vertical;
			return GetPixels(flip);
		}
		public Color[] GetPixels(Flip flip) {
			if (!Pixels.ContainsKey(flip)) {
				if (!Pixels.ContainsKey(Flip.None)) {
					int w = (int)rect.width;
					int h = (int)rect.height;
					int x = (int)rect.x;
					int y = (int)rect.y;
					Pixels[Flip.None] = texture.GetPixels(x, y, w, h);
				}
				if (flip != Flip.None) {
					int w = (int)rect.width;
					int h = (int)rect.height;
					bool flipX = flip.HasFlag(Flip.Horizontal);
					bool flipY = flip.HasFlag(Flip.Vertical);
					Color[] pix = Pixels[Flip.None];
					Color[] tar = new Color[pix.Length];
					for (int j = 0; j < h; j++) {
						for (int k = 0; k < w; k++) {
							int tileY = flipY ? (h - 1 - j) : j;
							int tileX = flipX ? (w - 1 - k) : k;
							tar[j*w+k] = pix[tileY * w + tileX];
						}
					}
					Pixels[flip] = tar;
				}
			}
			return Pixels[flip];
		}
	}
}
