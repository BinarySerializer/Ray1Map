using System;
using Newtonsoft.Json;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// A standard ARGB color
    /// </summary>
    public class ARGBColor : R1Serializable, IEquatable<ARGBColor> {
        #region Constructors

        public ARGBColor(byte red, byte green, byte blue) {
            Alpha = Byte.MaxValue;
            Red = red;
            Green = green;
            Blue = blue;
        }

        [JsonConstructor]
        public ARGBColor(byte alpha, byte red, byte green, byte blue) {
            Alpha = alpha;
            Red = red;
            Green = green;
            Blue = blue;
        }

        #endregion

        #region Private Fields

        private byte _alpha;
        private byte _red;
        private byte _green;
        private byte _blue;

        #endregion

        #region Protected Properties

        protected Color CachedColor { get; set; }
        protected bool Modified { get; set; } = true;

        #endregion

        #region Public Properties

        public byte Alpha
        {
            get => _alpha;
            set
            {
                _alpha = value;
                Modified = true;
            }
        }

        public byte Red
        {
            get => _red;
            set
            {
                _red = value;
                Modified = true;
            }
        }

        public byte Green
        {
            get => _green;
            set
            {
                _green = value;
                Modified = true;
            }
        }

        public byte Blue
        {
            get => _blue;
            set
            {
                _blue = value;
                Modified = true;
            }
        }

        #endregion

        #region Public Methods

        public Color GetColor()
        {
            if (Modified)
            {
                Modified = false;
                CachedColor = new Color((float)Red / Byte.MaxValue, (float)Green / Byte.MaxValue,
                    (float)Blue / Byte.MaxValue, (float)Alpha / Byte.MaxValue);
            }

            return CachedColor;
        }

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