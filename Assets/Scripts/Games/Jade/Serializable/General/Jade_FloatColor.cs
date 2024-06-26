using BinarySerializer;

namespace Ray1Map.Jade {
	public class Jade_FloatColor : BaseColor {
        public Jade_FloatColor() { }
        public Jade_FloatColor(float r, float g, float b, float a = 1f) : base(r, g, b, a) { }

		public override void SerializeImpl(SerializerObject s) {
			Red = s.Serialize<float>(Red, name: nameof(Red));
			Green = s.Serialize<float>(Green, name: nameof(Green));
			Blue = s.Serialize<float>(Blue, name: nameof(Blue));
			Alpha = s.Serialize<float>(Alpha, name: nameof(Alpha));
		}
	}
}
