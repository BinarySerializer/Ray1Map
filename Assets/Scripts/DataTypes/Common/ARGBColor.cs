using System;
using Newtonsoft.Json;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// A standard ARGB color wrapper. We use this instead of <see cref="Color"/> so we can serialize to JSON.
    /// </summary>
    public class ARGBColor
    {
        public ARGBColor(byte red, byte green, byte blue)
        {
            Alpha = Byte.MaxValue;
            Red = red;
            Green = green;
            Blue = blue;
        }

        [JsonConstructor]
        public ARGBColor(byte alpha, byte red, byte green, byte blue)
        {
            Alpha = alpha;
            Red = red;
            Green = green;
            Blue = blue;
        }

        public byte Alpha { get; }

        public byte Red { get; }
        
        public byte Green { get; }
        
        public byte Blue { get; }

        public Color GetColor() => new Color((float)Red / Byte.MaxValue, (float)Green / Byte.MaxValue, (float)Blue / Byte.MaxValue, (float)Alpha / Byte.MaxValue);
    }
}