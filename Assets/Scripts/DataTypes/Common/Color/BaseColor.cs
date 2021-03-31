using BinarySerializer;
using System;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// A standard ARGB color
    /// </summary>
    public abstract class BaseColor : BinarySerializable, IEquatable<BaseColor> {
        #region Constructors

        public BaseColor() { Alpha = 1f; }
        public BaseColor(float r, float g, float b, float a = 1f) {
            Red = r;
            Green = g;
            Blue = b;
            Alpha = a;
        }
        #endregion

        #region Protected Properties

        protected Color CachedColor { get; set; }
        protected bool Modified { get; set; } = true;

        #endregion

        #region Public Properties

        public static BaseColor clear => new CustomColor(0, 0, 0, 0);
        public static BaseColor black => new CustomColor(0, 0, 0, 1);
        public static BaseColor white => new CustomColor(1, 1, 1, 1);

        public float Red {
            get => _R;
            set { _R = value; Modified = true; }
}
        public float Green {
            get => _G;
            set { _G = value; Modified = true; }
        }
        public float Blue {
            get => _B;
            set { _B = value; Modified = true; }
        }
        public float Alpha {
            get => _A;
            set { _A = value; Modified = true; }
        }

        protected abstract float _R { get; set; }
        protected abstract float _G { get; set; }
        protected abstract float _B { get; set; }
        protected abstract float _A { get; set; }
        #endregion

        #region Public Methods

        public Color GetColor()
        {
            if (Modified)
            {
                Modified = false;
                CachedColor = new Color(Red, Green, Blue, Alpha);
            }

            return CachedColor;
        }
        #endregion

        #region Equality

        public bool Equals(BaseColor other)
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
            return Equals((BaseColor)obj);
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

        #region Serializable

        public override bool IsShortLog => true;
        public override string ShortLog => $"RGBA({(int)(Red*255)}, {(int)(Green * 255)}, {(int)(Blue * 255)}, {Alpha})";

        #endregion
    }
}