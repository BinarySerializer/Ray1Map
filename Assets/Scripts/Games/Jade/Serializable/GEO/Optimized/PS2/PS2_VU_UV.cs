using BinarySerializer;

namespace Ray1Map.Jade
{
    public class PS2_VU_UV : BinarySerializable
    {
        public int U { get; set; }
        public int V { get; set; }
        public int IsNotIncludedInStrip { get; set; }
        public int UCopy { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            U = s.Serialize<int>(U, name: nameof(U));
            V = s.Serialize<int>(V, name: nameof(V));
            IsNotIncludedInStrip = s.Serialize<int>(IsNotIncludedInStrip, name: nameof(IsNotIncludedInStrip));
			UCopy = s.Serialize<int>(UCopy, name: nameof(UCopy));
		}

		public override bool UseShortLog => true;
		public override string ShortLog => $"{GetType()}({UFloat}, {VFloat}, {IsNotIncludedInStrip})";
        public float UFloat => (float)U / 0x200;
        public float VFloat => (float)V / 0x200;

    }
}