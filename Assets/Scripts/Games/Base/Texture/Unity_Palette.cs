using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using UnityEngine;

namespace Ray1Map
{
    public class Unity_Palette
    {
        public Unity_Palette(IEnumerable<SerializableColor> colors, bool isFirstTransparent = false)
        {
            Colors = colors.Select((x, i) => isFirstTransparent && i == 0 ? Color.clear : x.GetColor()).ToArray();
        }

        public Unity_Palette(Color[] colors)
        {
            Colors = colors;
        }

        public Color[] Colors { get; }

        public static Unity_Palette[] SplitMultiple(SerializableColor[] colors, int paletteLength, bool isFirstTransparent = false)
        {
            return colors
                .Split(colors.Length / paletteLength, paletteLength)
                .Select(p => new Unity_Palette(p, isFirstTransparent))
                .ToArray();
        }
    }
}