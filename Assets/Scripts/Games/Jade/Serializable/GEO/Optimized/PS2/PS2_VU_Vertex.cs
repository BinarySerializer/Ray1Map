using BinarySerializer;

namespace Ray1Map.Jade
{
    public class PS2_VU_Vertex : BinarySerializable
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

		public float F { get; set; }
		public uint Flags { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            X = s.Serialize<float>(X, name: nameof(X));
            Y = s.Serialize<float>(Y, name: nameof(Y));
            Z = s.Serialize<float>(Z, name: nameof(Z));
			F = s.Serialize<float>(F, name: nameof(F));
            s.DoAt(Offset + 12, () => {
				Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
			});
		}

        public bool WindingOrder(bool useFlags) => useFlags ? ((Flags & 0x20) != 0) : (F > 0);
    }
}