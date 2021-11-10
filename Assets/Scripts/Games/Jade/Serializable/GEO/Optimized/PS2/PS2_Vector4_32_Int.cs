using BinarySerializer;

namespace Ray1Map.Jade
{
    /// <summary>
    /// A vector with 32-bit values
    /// </summary>
    public class PS2_Vector4_32_Int : BinarySerializable
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public int W { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            X = s.Serialize<int>(X, name: nameof(X));
            Y = s.Serialize<int>(Y, name: nameof(Y));
            Z = s.Serialize<int>(Z, name: nameof(Z));
			W = s.Serialize<int>(W, name: nameof(W));
		}

		public override bool UseShortLog => true;
		public override string ShortLog => $"{GetType()}({X:X8}, {Y:X8}, {Z:X8}, {W:X8})";

    }
}