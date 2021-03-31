using BinarySerializer;

namespace R1Engine
{
    /// <summary>
    /// A standard ARGB color wrapper with serializing support for the encoding BGR-444
    /// </summary>
    public class BGRA4444Color : BaseColor
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public BGRA4444Color() : base() { }
        public BGRA4444Color(float r, float g, float b, float a = 1f) : base(r,g,b,a:a) { }

        /// <summary>
        /// The color as a raw 16-bit value
        /// </summary>
        public ushort Color4444 { get; set; }

        #region RGBA Values
        protected override float _R {
            get => BitHelpers.ExtractBits(Color4444, 4, 8) / 15f;
            set => Color4444 = (ushort)BitHelpers.SetBits(Color4444, (int)(value * 15), 4, 8);
        }
        protected override float _G {
            get => BitHelpers.ExtractBits(Color4444, 4, 4) / 15f;
            set => Color4444 = (ushort)BitHelpers.SetBits(Color4444, (int)(value * 15), 4, 4);
        }
        protected override float _B {
            get => BitHelpers.ExtractBits(Color4444, 4, 0) / 15f;
            set => Color4444 = (ushort)BitHelpers.SetBits(Color4444, (int)(value * 15), 4, 0);
        }
        protected override float _A {
            get => BitHelpers.ExtractBits(Color4444, 4, 12) / 15f;
            set => Color4444 = (ushort)BitHelpers.SetBits(Color4444, (int)(value * 15), 4, 12);
        }
        #endregion

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) {
            Color4444 = s.Serialize<ushort>(Color4444, name: nameof(Color4444));
        }

        public static BGRA4444Color From4444(ushort bgra4444) {
            BGRA4444Color col = new BGRA4444Color();
            col.Color4444 = bgra4444;
            return col;
        }
    }
}