using System;

namespace R1Engine
{
    /// <summary>
    /// A standard ARGB color wrapper with serializing support for the encoding RGB-666
    /// </summary>
    public class RGB666Color : BaseColor
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public RGB666Color() : base() { }
        public RGB666Color(float r, float g, float b, float a = 1f) : base(r, g, b, a: a) { }

		#region Serialized fields
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
        #endregion

        #region RGBA Values
        protected override float _R {
            get => R / 63f;
            set => R = (byte)(value * 63);
        }
        protected override float _G {
            get => G / 63f;
            set => G = (byte)(value * 63);
        }
        protected override float _B {
            get => B / 63f;
            set => B = (byte)(value * 63);
        }
        protected override float _A {
            get => 1f;
            set { }
        }
        #endregion

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public override void SerializeImpl(SerializerObject s) {
            R = s.Serialize<byte>(R, name: nameof(R));
            G = s.Serialize<byte>(G, name: nameof(G));
            B = s.Serialize<byte>(B, name: nameof(B));
        }
    }
}