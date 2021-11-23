using BinarySerializer;

namespace Ray1Map.GBAVV
{
    public class GBAVV_MapCollision : BinarySerializable
    {
        public ushort Width { get; set; }
        public ushort Height { get; set; }
        public byte[] CollisionMap { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Width = s.Serialize<ushort>(Width, name: nameof(Width));
            Height = s.Serialize<ushort>(Height, name: nameof(Height));

            if (CollisionMap == null)
                CollisionMap = new byte[Width * Height];

            for (int i = 0; i < CollisionMap.Length; i+=2)
            {
                s.DoBits<byte>(b =>
                {
                    CollisionMap[i] = (byte)b.SerializeBits<int>(CollisionMap[i], 4, name: $"{nameof(CollisionMap)}[{i}]");
                    CollisionMap[i + 1] = (byte)b.SerializeBits<int>(CollisionMap[i + 1], 4, name: $"{nameof(CollisionMap)}[{i + 1}]");
                });
            }
        }
    }
}