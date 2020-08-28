using UnityEngine;
using UnityEngine.Tilemaps;

namespace R1Engine
{
    public static class TextureHelpers
    {
        /// <summary>
        /// Creates a new <see cref="Texture2D"/>
        /// </summary>
        /// <param name="width">The texture width</param>
        /// <param name="height">The texture height</param>
        /// <param name="clear">Indicates if the image should start as fully transparent</param>
        /// <param name="applyClear">Indicates if the clear transparent pixels should be applied</param>
        /// <returns>The texture</returns>
        public static Texture2D CreateTexture2D(int width, int height, bool clear = false, bool applyClear = false)
        {
            var tex = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };

            if (clear)
            {
                tex.SetPixels(new Color[width * height]);

                if (applyClear)
                    tex.Apply();
            }

            return tex;
        }

        public static Sprite CreateSprite(this Texture2D tex, Rect? rect = null, Vector2? pivot = null)
        {
            return Sprite.Create(tex, rect ?? new Rect(0, 0, tex.width, tex.height), pivot ?? new Vector2(0f, 1f), Settings.PixelsPerUnit, 20);
        }

        public static Unity_TileTexture CreateTile(this Texture2D tex, Rect? rect = null)
        {
            var t = new Unity_TileTexture() {
                rect = rect ?? new Rect(0, 0, tex.width, tex.height),
                texture = tex
            };
            return t;
        }
    }
}