using BinarySerializer;

namespace Ray1Map.Psychonauts
{
    public class PS2_GIF_Normal : BinarySerializable, ISerializerShortLog
    {
        public int Pre_JointInfluencesPerVertex { get; set; }

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public uint JointOffset2 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            X = s.Serialize<float>(X, name: nameof(X));
            Y = s.Serialize<float>(Y, name: nameof(Y));
            Z = s.Serialize<float>(Z, name: nameof(Z));

            if (Pre_JointInfluencesPerVertex >= 2)
                JointOffset2 = s.Serialize<uint>(JointOffset2, name: nameof(JointOffset2));
            else
                s.SerializePadding(4); // Indeterminate (repeated data)
        }

        public float XFloat => X - 65537f;
        public float YFloat => Y - 65537f;
        public float ZFloat => Z - 65537f;

        public string ShortLog => Pre_JointInfluencesPerVertex >= 2 
            ? $"Normal({XFloat}, {YFloat}, {ZFloat})+Joint({JointOffset2 / 4})"
            : $"Normal({XFloat}, {YFloat}, {ZFloat})";
    }
}