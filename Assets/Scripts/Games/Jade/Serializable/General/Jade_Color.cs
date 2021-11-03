using BinarySerializer;
using System.Collections.Generic;

namespace Ray1Map.Jade {
	public class Jade_Color : BaseBitwiseColor {
        public Jade_Color() { }
        public Jade_Color(float r, float g, float b, float a = 1f) : base(r, g, b, a) { }
        public Jade_Color(uint colorValue) : base(colorValue) { }

        protected override IReadOnlyDictionary<ColorChannel, ColorChannelFormat> ColorFormatting => Format;

        protected static IReadOnlyDictionary<ColorChannel, ColorChannelFormat> Format = new Dictionary<ColorChannel, ColorChannelFormat>() {
            [ColorChannel.Red] = new ColorChannelFormat(0, 8),
            [ColorChannel.Green] = new ColorChannelFormat(8, 8),
            [ColorChannel.Blue] = new ColorChannelFormat(16, 8),
            [ColorChannel.Alpha] = new ColorChannelFormat(24, 8)
        };
    }
}
