using BinarySerializer;

namespace Ray1Map.Psychonauts
{
    public class PS2_GIF_RGBA : BaseColor
    {
        public PS2_GIF_RGBA() { }
        public PS2_GIF_RGBA(float r, float g, float b, float a = 1f) : base(r, g, b, a) { }

        public override float Red
        {
            get => R - 32768f;
            set => R = value + 32768f;
        }
        public override float Green
        {
            get => G - 32768f;
            set => G = value + 32768f;
        }
        public override float Blue
        {
            get => B - 32768f;
            set => B = value + 32768f;
        }
        public override float Alpha
        {
            get => A - 32768f;
            set => A = value + 32768f;
        }

        public float R { get; set; }
        public float G { get; set; }
        public float B { get; set; }
        public float A { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            R = s.Serialize<float>(R, name: nameof(R));
            G = s.Serialize<float>(G, name: nameof(G));
            B = s.Serialize<float>(B, name: nameof(B));
            A = s.Serialize<float>(A, name: nameof(A));
        }

        public byte RByte => (byte)(Red * 256f);
        public byte GByte => (byte)(Green * 256f);
        public byte BByte => (byte)(Blue * 256f);
        public byte AByte => (byte)(Alpha * 256f);
    }
}