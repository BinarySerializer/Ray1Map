using System;
using BinarySerializer;
using Newtonsoft.Json;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// A standard ARGB color
    /// </summary>
    public class CustomColor : BaseColor {
        #region Constructors

        public CustomColor() : base() {}
        public CustomColor(float r, float g, float b, float a = 1f) : base(r, g, b, a: a) { }
        #endregion

        #region Private Fields
        private float _r, _g, _b, _a;
        #endregion


        #region Public Properties

        protected override float _A {
            get => _a;
            set => _a = value;
        }
        protected override float _R {
            get => _r;
            set => _r = value;
        }
        protected override float _G {
            get => _g;
            set => _g = value;
        }
        protected override float _B {
            get => _b;
            set => _b = value;
        }
        #endregion

        public override void SerializeImpl(SerializerObject s) {
            throw new NotImplementedException();
        }
        public static explicit operator CustomColor(Color color) {
            return new CustomColor() {
                CachedColor = color,
                _r = color.r,
                _g = color.g,
                _b = color.b,
                _a = color.a,
                Modified = false
            };
        }
    }
}