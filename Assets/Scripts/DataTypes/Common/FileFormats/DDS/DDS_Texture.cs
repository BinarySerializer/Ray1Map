using BinarySerializer;

namespace R1Engine
{
    public class DDS_Texture : BinarySerializable
    {
        public DDS_Header Header { get; set; } // Set before serializing

        public DDS_TextureItem[] Items { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            var mipmapsCount = Header.MipMapCount;

            if (mipmapsCount == 0)
                mipmapsCount = 1;

            if (Items == null)
                Items = new DDS_TextureItem[mipmapsCount];

            var w = Header.Width;
            var h = Header.Height;

            for (int i = 0; i < Items.Length; i++)
            {
                Items[i] = s.SerializeObject<DDS_TextureItem>(Items[i], x =>
                {
                    x.Header = Header;
                    x.Width = w;
                    x.Height = h;

                    w /= 2;
                    h /= 2;
                }, name: $"{nameof(Items)}[{i}]");
            }
        }
    }
}