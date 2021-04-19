using System;
using BinarySerializer;
using UnityEngine;

namespace R1Engine
{
    public class DDS : BinarySerializable
    {
        public uint Magic { get; set; }
        public DDS_Header Header { get; set; }
        public byte[] ImageData { get; set; }

        public bool IsCubeMap => Header.Caps.HasFlag(DDS_Header.DDS_CapsFlags.DDS_SURFACE_FLAGS_CUBEMAP);

        public Texture2D ToTexture2D()
        {
            if (IsCubeMap)
                Debug.LogWarning($"TODO: Implement remaining cub-map surfaces");

            Texture2D bitmap = new Texture2D((int)Header.Width, (int)Header.Height, TextureFormat.RGBA32, false);
            bitmap.LoadRawTextureData(ImageData);
            bitmap.Apply();
            return bitmap;
        }

        public override void SerializeImpl(SerializerObject s)
        {
            Magic = s.Serialize<uint>(Magic, name: nameof(Magic));

            if (Magic != 0x20534444)
                throw new Exception("Invalid DDS header");

            Header = s.SerializeObject<DDS_Header>(Header, name: nameof(Header));
            s.DoEncoded(new DDSEncoder(Header), () =>
            {
                ImageData = s.SerializeArray<byte>(ImageData, s.CurrentLength, name: nameof(ImageData));
            });
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
					Width = width,
					Depth = 1
				}
			};

			dds.ImageData = DDSParser.DecompressData(dds.Header, data, pixelFormat);

			return dds;
        }
	}
}