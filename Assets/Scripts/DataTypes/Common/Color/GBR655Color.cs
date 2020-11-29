namespace R1Engine
{
    /// <summary>
    /// A standard ARGB color wrapper with serializing support for the encoding RGB-556
    /// </summary>
    public class GBR655Color : BaseColor
    {
        /// <summary>
        /// Default constructor
        /// </summary>

        public GBR655Color() : base() { }
        public GBR655Color(float r, float g, float b, float a = 1f) : base(r, g, b, a: a) { }

        /// <summary>
        /// The color as a raw 16-bit value
        /// </summary>
        public ushort Color655 { get; set; }

        #region RGBA Values
        protected override float _R {
            get => BitHelpers.ExtractBits(Color655, 5, 11) / 31f;
            set => Color655 = (ushort)BitHelpers.SetBits(Color655, (int)(value * 31), 5, 11);
        }
        protected override float _G {
            get => BitHelpers.ExtractBits(Color655, 6, 0) / 63f;
            set => Color655 = (ushort)BitHelpers.SetBits(Color655, (int)(value * 63), 6, 0);
        }
        protected override float _B {
            get => BitHelpers.ExtractBits(Color655, 5, 6) / 31f;
            set => Color655 = (ushort)BitHelpers.SetBits(Color655, (int)(value * 31), 5, 6);
        }
        protected override float _A {
            get => 1f;
            set { }
        }
        #endregion

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) => Color655 = s.Serialize<ushort>(Color655, name: nameof(Color655));
    }
}