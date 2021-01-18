namespace R1Engine
{
    public class GBACrash_Isometric_CollisionTile : R1Serializable
    {
        public FixedPointInt Height { get; set; }
        public int TypeIndex { get; set; }
        public byte Byte_08 { get; set; }
        public byte Byte_09 { get; set; }
        public byte Shape { get; set; } // 0-12
        public byte Byte_0B { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Height = s.SerializeObject<FixedPointInt>(Height, name: nameof(Height));
            TypeIndex = s.Serialize<int>(TypeIndex, name: nameof(TypeIndex));
            Byte_08 = s.Serialize<byte>(Byte_08, name: nameof(Byte_08));
            Byte_09 = s.Serialize<byte>(Byte_09, name: nameof(Byte_09));
            Shape = s.Serialize<byte>(Shape, name: nameof(Shape));
            Byte_0B = s.Serialize<byte>(Byte_0B, name: nameof(Byte_0B));
        }
    }
}