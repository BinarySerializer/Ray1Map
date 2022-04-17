using BinarySerializer;
using BinarySerializer.PS1;
using UnityEngine;

namespace Ray1Map
{
    public static class ColorExtensions
    {
        public static Color GetColor(this BaseColor c) => new Color(c.Red, c.Green, c.Blue, c.Alpha);
        public static Color GetColor(this PS1_TMD_Color c) => new Color(c.Color.Red * 2, c.Color.Green * 2, c.Color.Blue * 2);
        public static CustomColor GetColor(this Color c) => new CustomColor(c.r, c.g, c.b, c.a);
    }
}