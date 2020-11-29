namespace R1Engine
{
    /// <summary>
    /// A standard ARGB color wrapper with serializing support for the encoding BGR-444
    /// </summary>
    public class BGRA4441Color : BaseColor
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public BGRA4441Color() : base() { }
        public BGRA4441Color(float r, float g, float b, float a = 1f) : base(r,g,b,a:a) { }

        /// <summary>
        /// The color as a raw 16-bit value
        /// </summary>
        public ushort Color4441 { get; set; }

        #region RGBA Values
        protected override float _R {
            get => BitHelpers.ExtractBits(Color4441, 4, 8) / 15f;
            set => Color4441 = (ushort)BitHelpers.SetBits(Color4441, (int)(value * 15), 4, 8);
        }
        protected override float _G {
            get => BitHelpers.ExtractBits(Color4441, 4, 4) / 15f;
            set => Color4441 = (ushort)BitHelpers.SetBits(Color4441, (int)(value * 15), 4, 4);
        }
        protected override float _B {
            get => BitHelpers.ExtractBits(Color4441, 4, 0) / 15f;
            set => Color4441 = (ushort)BitHelpers.SetBits(Color4441, (int)(value * 15), 4, 0);
        }
        protected override float _A {
            get => BitHelpers.ExtractBits(Color4441, 1, 15);
            set => Color4441 = (ushort)BitHelpers.SetBits(Color4441, (int)value, 1, 15);
        }
        #endregion

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) {
            Color4441 = s.Serialize<ushort>(Color4441, name: nameof(Color4441));
        }

        public static BGRA4441Color From4441(ushort bgra4441) {
            BGRA4441Color col = new BGRA4441Color();
            col.Color4441 = bgra4441;
            return col;
        }
    }
}