using System;
using Newtonsoft.Json;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// A standard ARGB color wrapper. We use this instead of <see cref="Color"/> so we can serialize to JSON.
    /// </summary>
    public class ARGBColor : R1Serializable
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

        public byte Alpha { get; set; }

        public byte Red { get; set; }
        
        public byte Green { get; set; }
        
        public byte Blue { get; set; }

        public Color GetColor() => new Color((float)Red / Byte.MaxValue, (float)Green / Byte.MaxValue, (float)Blue / Byte.MaxValue, (float)Alpha / Byte.MaxValue);

        public override void SerializeImpl(SerializerObject s) {
            Alpha = s.Serialize<byte>(Alpha, name: nameof(Alpha));
            Red = s.Serialize<byte>(Red, name: nameof(Red));
            Green = s.Serialize<byte>(Green, name: nameof(Green));
            Blue = s.Serialize<byte>(Blue, name: nameof(Blue));
        }
    }
}