using BinarySerializer;

namespace Ray1Map.Psychonauts
{
    public class PS2_GIF_UV : BinarySerializable
    {
        public ushort U { get; set; }
        public ushort V { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            U = s.Serialize<ushort>(U, name: nameof(U));
            s.Align(4, Offset);
            V = s.Serialize<ushort>(V, name: nameof(V));
            s.Align(4, Offset);
            s.SerializePadding(8); // TODO: Is this padding?
        }

        public float UFloat => ((U - 0x8000) / 4096f);// / QFloat;
        public float VFloat => ((V - 0x8000) / 4096f);// / QFloat;

		public override string ShortLog => $"PS2_GIF_UV({U:X4}, {V:X4} | {UFloat}, {VFloat})";

    }
}