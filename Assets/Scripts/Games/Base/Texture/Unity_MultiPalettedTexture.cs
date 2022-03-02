using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ray1Map
{
    /// <summary>
    /// A texture which can be displayed with multiple palettes
    /// </summary>
    public class Unity_MultiPalettedTexture : IUnity_Texture
    {
        public Unity_MultiPalettedTexture(byte[] imgData, IEnumerable<Unity_Palette> palettes, int width, int height, Unity_TextureFormat format, int offset = 0)
        {
            Textures = palettes.Select(p => new Unity_PalettedTexture(imgData, p, width, height, format, offset)).ToArray();
            Width = width;
            Height = height;
        }

        private int _lastPal;

        public Unity_PalettedTexture[] Textures { get; }
        public int Width { get; }
        public int Height { get; }

        public Color[] GetColors() => Textures[_lastPal].GetColors();
        public Color[] GetColors(int palIndex) => GetTexture(palIndex).GetColors();

        public Unity_PalettedTexture GetTexture(int palIndex)
        {
            _lastPal = palIndex;
            return palIndex >= Textures.Length ? Textures.First() : Textures[palIndex];
        }
    }
}