using System;
using UnityEngine;

namespace Ray1Map
{
    public class Unity_TileTexture
    {
        public Unity_TileTexture(IUnity_Texture unityTexture)
        {
            UnityTexture = unityTexture ?? throw new ArgumentNullException(nameof(unityTexture));
            Rect = new RectInt(0, 0, unityTexture.Width, unityTexture.Height);
        }
        public Unity_TileTexture(Texture2D texture, RectInt rect)
        {
            UnityTexture = null;
            Texture = texture;
            Rect = rect;
        }

        public IUnity_Texture UnityTexture { get; }
        public RectInt Rect { get; }

        // Legacy - should be replaced
        private Color[] _texturePixels;
        public Texture2D Texture { get; }

        public bool IsNull => Texture == null && UnityTexture == null;

        private Color[] GetPixels(Unity_Tile mapTile)
        {
            if (UnityTexture != null)
            {
                if (UnityTexture is Unity_MultiPalettedTexture p)
                    return p.GetColors(mapTile.Data.PaletteIndex);
                else
                    return UnityTexture.GetColors();
            }
            else
            {
                return _texturePixels ??= Texture.GetPixels(Rect.x, Rect.y, Rect.width, Rect.height);
            }
        }

        public Color[] GetPixels(bool flipX, bool flipY, Unity_Tile mapTile) 
        {
            Color[] pixels = GetPixels(mapTile);

            // TODO: Cache flipped colors
            if (flipX || flipY)
            {
                var flippedPixels = new Color[pixels.Length];

                int width = Rect.width;
                int height = Rect.height;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int tileX = flipX ? (width - 1 - x) : x;
                        int tileY = flipY ? (height - 1 - y) : y;
                        flippedPixels[y * width + x] = pixels[tileY * width + tileX];
                    }
                }

                return flippedPixels;
            }

            return pixels;
        }
    }
}