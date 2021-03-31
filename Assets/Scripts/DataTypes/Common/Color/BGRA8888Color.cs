using BinarySerializer;

namespace R1Engine
{
    /// <summary>
    /// A standard ARGB color wrapper with serializing support for the encoding RGB-888
    /// </summary>
    public class BGRA8888Color : BaseColor
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public BGRA8888Color() : base() { }
        public BGRA8888Color(float r, float g, float b, float a = 1f) : base(r, g, b, a: a) { }

        #region Serialized fields
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
        public byte A { get; set; }
        #endregion

        #region RGBA Values
        protected override float _R {
            get => R / 255f;
            set => R = (byte)(value * 255);
        }
        protected override float _G {
            get => G / 255f;
            set => G = (byte)(value * 255);
        }
        protected override float _B {
            get => B / 255f;
            set => B = (byte)(value * 255);
        }
        protected override float _A {
            get => A / 255f;
            set => A = (byte)(value * 255);
        }
        #endregion

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) {
            B = s.Serialize<byte>(B, name: nameof(B));
            G = s.Serialize<byte>(G, name: nameof(G));
            R = s.Serialize<byte>(R, name: nameof(R));
            A = s.Serialize<byte>(A, name: nameof(A));
        }
    }
}