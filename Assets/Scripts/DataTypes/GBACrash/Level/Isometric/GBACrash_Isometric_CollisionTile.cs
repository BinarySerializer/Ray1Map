namespace R1Engine
{
    public class GBACrash_Isometric_CollisionTile : R1Serializable
    {
        public byte Byte_00 { get; set; }
        public byte Height { get; set; }
        public short Short_02 { get; set; }
        public int TypeIndex { get; set; }
        public byte Byte_08 { get; set; }
        public byte Byte_09 { get; set; }
        public byte Byte_0A { get; set; }
        public byte Byte_0B { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Byte_00 = s.Serialize<byte>(Byte_00, name: nameof(Byte_00));
            Height = s.Serialize<byte>(Height, name: nameof(Height));
            Short_02 = s.Serialize<short>(Short_02, name: nameof(Short_02));
            TypeIndex = s.Serialize<int>(TypeIndex, name: nameof(TypeIndex));
            Byte_08 = s.Serialize<byte>(Byte_08, name: nameof(Byte_08));
            Byte_09 = s.Serialize<byte>(Byte_09, name: nameof(Byte_09));
            Byte_0A = s.Serialize<byte>(Byte_0A, name: nameof(Byte_0A));
            Byte_0B = s.Serialize<byte>(Byte_0B, name: nameof(Byte_0B));
        }
    }
}