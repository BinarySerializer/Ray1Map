using BinarySerializer;

namespace R1Engine
{
    public class GBAVV_NitroKart_NGage_Texture : BinarySerializable
    {
        public byte[] Texture_64px { get; set; }
        public byte[] Texture_32px { get; set; }
        public byte[] Texture_16px { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Texture_64px = s.SerializeArray<byte>(Texture_64px, 64 * 64, name: nameof(Texture_64px));
            Texture_32px = s.SerializeArray<byte>(Texture_32px, 32 * 32, name: nameof(Texture_32px));
            Texture_16px = s.SerializeArray<byte>(Texture_16px, 16 * 16, name: nameof(Texture_16px));
        }
    }
}