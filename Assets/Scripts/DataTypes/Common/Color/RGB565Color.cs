using System;
using BinarySerializer;

namespace R1Engine
{
    /// <summary>
    /// A standard ARGB color wrapper with serializing support for the encoding RGB-565
    /// </summary>
    public class RGB565Color : BaseColor
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public RGB565Color() : base() { }
        public RGB565Color(float r, float g, float b, float a = 1f) : base(r, g, b, a: a) { }

        /// <summary>
        /// The color as a raw 16-bit value
        /// </summary>
        public ushort Color565 { get; set; }

        #region RGBA Values
        protected override float _R {
            get => BitHelpers.ExtractBits(Color565, 5, 0) / 31f;
            set => Color565 = (ushort)BitHelpers.SetBits(Color565, (int)(value * 31), 5, 0);
        }
        protected override float _G {
            get => BitHelpers.ExtractBits(Color565, 6, 5) / 63f;
            set => Color565 = (ushort)BitHelpers.SetBits(Color565, (int)(value * 63), 6, 5);
        }
        protected override float _B {
            get => BitHelpers.ExtractBits(Color565, 5, 11) / 31f;
            set => Color565 = (ushort)BitHelpers.SetBits(Color565, (int)(value * 31), 5, 11);
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
            Color565 = s.Serialize<ushort>(Color565, name: nameof(Color565));
        }
    }
}