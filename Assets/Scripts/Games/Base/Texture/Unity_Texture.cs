using UnityEngine;

namespace Ray1Map
{
    /// <summary>
    /// A standard texture
    /// </summary>
    public class Unity_Texture : IUnity_Texture
    {
        public Unity_Texture(Color[] colors, int width, int height)
        {
            Colors = colors;
            Width = width;
            Height = height;
        }

        public Color[] Colors { get; }
        public int Width { get; }
        public int Height { get; }

        public Color[] GetColors() => Colors;
    }
}