using BinarySerializer;

namespace Ray1Map.GBAVV
{
    public class GBAVV_NitroKart_NGage_TEX : BinarySerializable
    {
        public GBAVV_NitroKart_NGage_Texture[] Textures { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Textures = s.SerializeObjectArray<GBAVV_NitroKart_NGage_Texture>(Textures, s.CurrentLength / 0x1500, name: nameof(Textures));
        }
    }
}