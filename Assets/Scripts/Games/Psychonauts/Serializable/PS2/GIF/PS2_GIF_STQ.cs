using BinarySerializer;

namespace Ray1Map.Psychonauts
{
    public class PS2_GIF_STQ : BinarySerializable
    {
        public ushort S { get; set; }
        public ushort T { get; set; }
        public ushort Q { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            S = s.Serialize<ushort>(S, name: nameof(S));
            s.Align(4, Offset);
            T = s.Serialize<ushort>(T, name: nameof(T));
            s.Align(4, Offset);
            Q = s.Serialize<ushort>(Q, name: nameof(Q));
            s.Align(4, Offset);
            s.SerializePadding(4); // TODO: Is this padding?
        }

        public float QFloat => (Q - 0x8000) / 4096f;
        public float U => ((S - 0x8000) / 4096f);// / QFloat;
        public float V => ((T - 0x8000) / 4096f);// / QFloat;

		public override string ShortLog => $"PS2_GIF_STQ({S:X4}, {T:X4}, {Q:X4} | {U}, {V})";

    }
}