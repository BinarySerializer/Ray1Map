using System;
using Newtonsoft.Json;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// A standard ARGB color
    /// </summary>
    public class ARGBColor : R1Serializable, IEquatable<ARGBColor>
    {
        #region Constructors

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

        #endregion

        #region Public Properties

        public byte Alpha { get; set; }

        public byte Red { get; set; }

        public byte Green { get; set; }

        public byte Blue { get; set; }

        #endregion

        #region Public Methods

        public Color GetColor() => new Color((float)Red / Byte.MaxValue, (float)Green / Byte.MaxValue, (float)Blue / Byte.MaxValue, (float)Alpha / Byte.MaxValue);

        public override void SerializeImpl(SerializerObject s)
        {
            Alpha = s.Serialize<byte>(Alpha, name: nameof(Alpha));
            Red = s.Serialize<byte>(Red, name: nameof(Red));
            Green = s.Serialize<byte>(Green, name: nameof(Green));
            Blue = s.Serialize<byte>(Blue, name: nameof(Blue));
        }

        #endregion

        #region Equality

        public bool Equals(ARGBColor other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Alpha == other.Alpha && Red == other.Red && Green == other.Green && Blue == other.Blue;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ARGBColor)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Alpha.GetHashCode();
                hashCode = (hashCode * 397) ^ Red.GetHashCode();
                hashCode = (hashCode * 397) ^ Green.GetHashCode();
                hashCode = (hashCode * 397) ^ Blue.GetHashCode();
                return hashCode;
            }
        }

        #endregion
    }
}