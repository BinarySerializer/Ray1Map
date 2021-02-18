using ImageMagick;
using UnityEngine;

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

        public static MagickImage ToMagickImage(this Texture2D tex)
        {
            var pixels = tex.GetPixels();
            var bytes = new byte[pixels.Length * 4];

            for (int i = 0; i < pixels.Length; i++)
            {
                bytes[i * 4 + 0] = (byte)(pixels[i].a * 255);
                bytes[i * 4 + 1] = (byte)(pixels[i].b * 255);
                bytes[i * 4 + 2] = (byte)(pixels[i].g * 255);
                bytes[i * 4 + 3] = (byte)(pixels[i].r * 255);
            }

            var img = new MagickImage(bytes, new PixelReadSettings(tex.width, tex.height, StorageType.Char, PixelMapping.ABGR));
            img.Flip();
            return img;
        }

        public static MagickImage ToMagickImage(this Sprite sprite)
        {
            var pixels = sprite.texture.GetPixels((int)sprite.rect.x, (int)sprite.rect.y, (int)sprite.rect.width, (int)sprite.rect.height);
            var bytes = new byte[pixels.Length * 4];

            for (int i = 0; i < pixels.Length; i++)
            {
                bytes[i * 4 + 0] = (byte)(pixels[i].a * 255);
                bytes[i * 4 + 1] = (byte)(pixels[i].b * 255);
                bytes[i * 4 + 2] = (byte)(pixels[i].g * 255);
                bytes[i * 4 + 3] = (byte)(pixels[i].r * 255);
            }

            var img = new MagickImage(bytes, new PixelReadSettings((int)sprite.rect.width, (int)sprite.rect.height, StorageType.Char, PixelMapping.ABGR));
            img.Flip();
            return img;
        }

        public static Texture2D Crop(this Texture2D tex, RectInt rect, bool destroyTex, bool flipY = true)
        {
            var newTex = CreateTexture2D(rect.width, rect.height);

            if (flipY)
                rect.y = tex.height - rect.height - rect.y;

            newTex.SetPixels(tex.GetPixels(rect.x, rect.y, rect.width, rect.height));

            newTex.Apply();

            if (destroyTex)
                Object.DestroyImmediate(tex);

            return newTex;
        }
    }
}