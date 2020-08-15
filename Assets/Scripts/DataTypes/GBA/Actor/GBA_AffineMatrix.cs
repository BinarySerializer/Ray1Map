using System.Numerics;

namespace R1Engine
{
    public class GBA_AffineMatrix : R1Serializable
    {
        public short Pa { get; set; }
        public short Pb { get; set; }
        public short Pc { get; set; }
        public short Pd { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Pa = s.Serialize<short>(Pa, name: nameof(Pa));
            Pb = s.Serialize<short>(Pb, name: nameof(Pb));
            Pc = s.Serialize<short>(Pc, name: nameof(Pc));
            Pd = s.Serialize<short>(Pd, name: nameof(Pd));
        }

        public Vector4 ToFloat() {
            return new Vector4(Pa / 256f, Pb / 256f, Pc / 256f, Pd / 256f);
        }
    }
}