using BinarySerializer;
using UnityEngine;

namespace R1Engine
{
    public class DDS_TextureItem : BinarySerializable
    {
        // Set before serializing
        public DDS_Header Header { get; set; }
        public uint Width { get; set; }
        public uint Height { get; set; }

        public byte[] ImageData { get; set; }

        public Texture2D ToTexture2D()
        {
            TextureFormat fmt = TextureFormat.RGBA32;
            if (Header?.PixelFormat?.FourCC == "ATI2") {
                fmt = TextureFormat.RGB24;
            }
            Texture2D bitmap = new Texture2D((int)Width, (int)Height, fmt, false);
            bitmap.LoadRawTextureData(ImageData);
            bitmap.Apply();
            return bitmap;
        }

        public override void SerializeImpl(SerializerObject s)
        {
            s.DoEncoded(new DDSEncoder(Header, Width, Height), () =>
            {
                ImageData = s.SerializeArray<byte>(ImageData, s.CurrentLength, name: nameof(ImageData));
            });
        }
    }
}