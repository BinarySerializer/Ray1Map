using BinarySerializer;

namespace Ray1Map.Psychonauts
{
    public class PS2_GIF_Vertex : BinarySerializable
    {
        public int Pre_JointInfluencesPerVertex { get; set; }

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public uint JointOffset1 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            X = s.Serialize<float>(X, name: nameof(X));
            Y = s.Serialize<float>(Y, name: nameof(Y));
            Z = s.Serialize<float>(Z, name: nameof(Z));

            if (Pre_JointInfluencesPerVertex >= 1)
                JointOffset1 = s.Serialize<uint>(JointOffset1, name: nameof(JointOffset1));
            else
                s.SerializePadding(4); // Indeterminate (repeated data)
        }

        public override bool UseShortLog => true;
        public override string ShortLog => Pre_JointInfluencesPerVertex >= 1
            ? $"Vertex({X}, {Y}, {Z})+Joint({JointOffset1 / 4})"
            : $"Vertex({X}, {Y}, {Z})";
    }
}