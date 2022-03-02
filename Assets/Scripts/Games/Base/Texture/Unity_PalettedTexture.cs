using System;
using UnityEngine;

namespace Ray1Map
{
    /// <summary>
    /// A paletted texture
    /// </summary>
    public class Unity_PalettedTexture : IUnity_Texture
    {
        public Unity_PalettedTexture(byte[] imgData, Unity_Palette palette, int width, int height, Unity_TextureFormat format, int offset = 0)
        {
            Width = width;
            Height = height;
            Palette = palette;
            ImgData = imgData;
            Format = format;
            Offset = offset;
        }

        public byte[] ImgData { get; }
        public Unity_Palette Palette { get; }
        public int Width { get; }
        public int Height { get; }
        public Unity_TextureFormat Format { get; }
        public int Offset { get; }

        private Color[] _colors;
        public Color[] GetColors() => _colors ??= CreateColors();

        private Color[] CreateColors()
        {
            Color[] colors = new Color[Width * Height];
            int imgDataIndex = 0;

            switch (Format)
            {
                case Unity_TextureFormat.Indexed_4:
                    for (int i = 0; i < colors.Length; i += 2)
                    {
                        colors[i] = Palette.Colors[ImgData[Offset + imgDataIndex] & 0xF];
                        colors[i + 1] = Palette.Colors[ImgData[Offset + imgDataIndex] >> 4];
                        imgDataIndex++;
                    }
                    break;

                case Unity_TextureFormat.Indexed_4_Reversed:
                    for (int i = 0; i < colors.Length; i += 2)
                    {
                        colors[i] = Palette.Colors[ImgData[Offset + imgDataIndex] >> 4];
                        colors[i + 1] = Palette.Colors[ImgData[Offset + imgDataIndex] & 0xF];
                        imgDataIndex++;
                    }
                    break;

                case Unity_TextureFormat.Indexed_8:
                    for (int i = 0; i < colors.Length; i++)
                        colors[i] = Palette.Colors[ImgData[Offset + i]];
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(Format), Format, null);
            }

            return colors;
        }
    }
}