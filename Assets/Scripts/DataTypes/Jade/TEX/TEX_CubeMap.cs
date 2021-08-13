using BinarySerializer;
using BinarySerializer.Image;

namespace R1Engine.Jade
{
    public class TEX_CubeMap : Jade_File 
    {
        public DDS_Header DDS_Header { get; set; }
        public DDS DDS { get; set; }
        public TEX_File Header { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            DDS_Header = s.SerializeObject<DDS_Header>(DDS_Header, name: nameof(DDS_Header));
            DDS = s.SerializeObject<DDS>(DDS, x =>
            {
                x.Pre_SkipHeader = true;
                x.Header = new DDS_Header()
                {
                    Flags = DDS_Header.DDS_HeaderFlags.DDS_HEADER_FLAGS_TEXTURE,
                    Height = DDS_Header.Height,
                    Width = DDS_Header.Width,
                    MipMapCount = DDS_Header.MipMapCount,
                    Caps = DDS_Header.DDS_CapsFlags.DDS_SURFACE_FLAGS_CUBEMAP,
                    Caps2 = DDS_Header.DDS_Caps2Flags.DDSCAPS2_CUBEMAP | DDS_Header.DDS_Caps2Flags.DDS_CUBEMAP_ALLFACES,
                    PixelFormat = new DDS_PixelFormat
                    {
                        Flags = DDS_PixelFormat.DDS_PixelFormatFlags.DDPF_FOURCC,
                        FourCC = DDS_Header.PixelFormat.FourCC,
                        RGBBitCount = 32
                    },
                };
            }, name: nameof(DDS));

            if (s.CurrentAbsoluteOffset + 0x20 <= Offset.AbsoluteOffset + FileSize) {
                Header = s.SerializeObject<TEX_File>(Header, onPreSerialize: h => {
                    h.FileSize = 0x20;
                    h.Loader = Loader;
                    h.Key = Key;
                    h.SetEditorMode();
                }, name: nameof(Header));
            }
        }
    }
}