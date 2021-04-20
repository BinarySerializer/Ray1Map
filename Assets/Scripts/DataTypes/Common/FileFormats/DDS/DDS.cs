using BinarySerializer;
using System;
using System.IO;
using System.Linq;

namespace R1Engine
{
    public class DDS : BinarySerializable
    {
        public uint Magic { get; set; }
        public DDS_Header Header { get; set; }
        public DDS_Texture[] Textures { get; set; }

        public DDS_TextureItem PrimaryTexture => Textures?.FirstOrDefault()?.Items?.FirstOrDefault();

        public override void SerializeImpl(SerializerObject s)
        {
            Magic = s.Serialize<uint>(Magic, name: nameof(Magic));

            if (Magic != 0x20534444)
                throw new Exception("Invalid DDS header");

            Header = s.SerializeObject<DDS_Header>(Header, name: nameof(Header));

            var texturesCount = 1;

            // TODO: Improve this - a cubemap doesn't require all surfaces to be available
            if (Header.Caps.HasFlag(DDS_Header.DDS_CapsFlags.DDS_SURFACE_FLAGS_CUBEMAP))
                texturesCount = 6;

            Textures = s.SerializeObjectArray(Textures, texturesCount, x => x.Header = Header, name: nameof(Textures));
        }

		public static DDS FromRawData(byte[] data, DDSParser.PixelFormat pixelFormat, uint width, uint height)
        {
			if (data == null)
				return null;

			var dds = new DDS()
			{
				Header = new DDS_Header
				{
					PixelFormat = new DDS_PixelFormat(),
					Height = height,
					Width = width
				}
			};

            using var memStream = new MemoryStream(data);
            using var reader = new Reader(memStream);

            dds.Textures = new DDS_Texture[]
            {
                new DDS_Texture()
                {
                    Header = dds.Header,
                    Items = new DDS_TextureItem[]
                    {
                        new DDS_TextureItem()
                        {
                            ImageData = DDSParser.DecompressData(dds.Header, reader, pixelFormat, width, height),
                            Header = dds.Header,
                            Width = width,
                            Height = height
                        }
                    }
                }
            };

            return dds;
        }
    }
}