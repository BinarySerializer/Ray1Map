using System;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// A standard RGB color wrapper. We use this instead of <see cref="Color"/> so we can serialize to JSON.
    /// </summary>
    public class RGBColor
    {
        public RGBColor(byte red, byte green, byte blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        public byte Red { get; }
        
        public byte Green { get; }
        
        public byte Blue { get; }

        public Color GetColor() => new Color((float)Red / Byte.MaxValue, (float)Green / Byte.MaxValue, (float)Blue / Byte.MaxValue);
    }
}