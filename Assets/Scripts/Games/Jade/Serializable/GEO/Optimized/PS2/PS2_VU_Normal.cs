using BinarySerializer;

namespace Ray1Map.Jade
{
    public class PS2_VU_Normal : BinarySerializable
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public int IsNotIncludedInStrip { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            X = s.Serialize<int>(X, name: nameof(X));
            Y = s.Serialize<int>(Y, name: nameof(Y));
            Z = s.Serialize<int>(Z, name: nameof(Z));
			IsNotIncludedInStrip = s.Serialize<int>(IsNotIncludedInStrip, name: nameof(IsNotIncludedInStrip));
		}
    }
}