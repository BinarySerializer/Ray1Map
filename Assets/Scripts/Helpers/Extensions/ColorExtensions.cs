using BinarySerializer;
using UnityEngine;

namespace Ray1Map
{
    public static class ColorExtensions
    {
        public static Color GetColor(this BaseColor c) => new Color(c.Red, c.Green, c.Blue, c.Alpha);
        public static CustomColor GetColor(this Color c) => new CustomColor(c.r, c.g, c.b, c.a);
    }
}