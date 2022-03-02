using UnityEngine;

namespace Ray1Map
{
    public static class Unity_TextureExtensions
    {
        public static Texture2D GetTexture2D(this IUnity_Texture tex)
        {
            Texture2D texture = TextureHelpers.CreateTexture2D(tex.Width, tex.Height);
            texture.SetPixels(tex.GetColors());
            texture.Apply();
            return texture;
        }
    }
}