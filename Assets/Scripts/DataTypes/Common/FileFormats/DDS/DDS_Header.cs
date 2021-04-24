using System;
using BinarySerializer;

namespace R1Engine
{
    public class DDS_Header : BinarySerializable
    {
        public uint Magic { get; set; }

        public uint StructSize { get; set; }
        public DDS_HeaderFlags Flags { get; set; }
        public uint Height { get; set; }
        public uint Width { get; set; }

        /// <summary>
        /// The pitch or number of bytes per scan line in an uncompressed texture; the total number of bytes in the top level texture for a compressed texture
        /// </summary>
        public uint PitchOrLinearSize { get; set; }

        public uint Depth { get; set; }
        public uint GetDepth => Depth == 0 ? 1 : Depth;
        public uint MipMapCount { get; set; }
        public uint GetMipMapCount => MipMapCount == 0 ? 1 : MipMapCount;
        public uint[] Reserved { get; set; }
        public DDS_PixelFormat PixelFormat { get; set; }
        public DDS_CapsFlags Caps { get; set; }
        public DDS_Caps2Flags Caps2 { get; set; }
        public uint Caps3 { get; set; }
        public uint Caps4 { get; set; }
        public uint Reserved2 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Magic = s.Serialize<uint>(Magic, name: nameof(Magic));

            if (Magic != 0x20534444)
                throw new Exception("Invalid DDS header");

            StructSize = s.Serialize<uint>(StructSize, name: nameof(StructSize));

            if (StructSize != 124)
                throw new Exception($"Invalid DDS header size of {StructSize}. Should be 124.");

            Flags = s.Serialize<DDS_HeaderFlags>(Flags, name: nameof(Flags));
            Height = s.Serialize<uint>(Height, name: nameof(Height));
            Width = s.Serialize<uint>(Width, name: nameof(Width));
            PitchOrLinearSize = s.Serialize<uint>(PitchOrLinearSize, name: nameof(PitchOrLinearSize));
            Depth = s.Serialize<uint>(Depth, name: nameof(Depth));
            MipMapCount = s.Serialize<uint>(MipMapCount, name: nameof(MipMapCount));
            Reserved = s.SerializeArray<uint>(Reserved, 11, name: nameof(Reserved));
            PixelFormat = s.SerializeObject<DDS_PixelFormat>(PixelFormat, name: nameof(PixelFormat));
            Caps = s.Serialize<DDS_CapsFlags>(Caps, name: nameof(Caps));
            Caps2 = s.Serialize<DDS_Caps2Flags>(Caps2, name: nameof(Caps2));
            Caps3 = s.Serialize<uint>(Caps3, name: nameof(Caps3));
            Caps4 = s.Serialize<uint>(Caps4, name: nameof(Caps4));
            Reserved2 = s.Serialize<uint>(Reserved2, name: nameof(Reserved2));
        }

        [Flags]
        public enum DDS_HeaderFlags : uint
        {
            DDSD_CAPS = 0x1,
            DDSD_HEIGHT = 0x2,
            DDSD_WIDTH = 0x4,
            DDSD_PITCH = 0x8,
            DDSD_PIXELFORMAT = 0x1000,
            DDSD_MIPMAPCOUNT = 0x20000,
            DDSD_LINEARSIZE = 0x80000,
            DDSD_DEPTH = 0x800000,

            DDS_HEADER_FLAGS_TEXTURE = DDSD_CAPS | DDSD_HEIGHT | DDSD_WIDTH | DDSD_PIXELFORMAT,
            DDS_HEADER_FLAGS_MIPMAP = DDSD_MIPMAPCOUNT,
            DDS_HEADER_FLAGS_VOLUME = DDSD_DEPTH,
            DDS_HEADER_FLAGS_PITCH = DDSD_PITCH,
            DDS_HEADER_FLAGS_LINEARSIZE = DDSD_LINEARSIZE
        }

        [Flags]
        public enum DDS_CapsFlags : uint
        {
            DDSCAPS_COMPLEX = 0x8,
            DDSCAPS_MIPMAP = 0x400000,
            DDSCAPS_TEXTURE = 0x1000,

            DDS_SURFACE_FLAGS_MIPMAP = DDSCAPS_COMPLEX | DDSCAPS_MIPMAP,
            DDS_SURFACE_FLAGS_TEXTURE = DDSCAPS_TEXTURE,
            DDS_SURFACE_FLAGS_CUBEMAP = DDSCAPS_COMPLEX
        }

        [Flags]
        public enum DDS_Caps2Flags : uint
        {
            DDSCAPS2_CUBEMAP = 0x200,
            DDSCAPS2_CUBEMAP_POSITIVEX = 0x400,
            DDSCAPS2_CUBEMAP_NEGATIVEX = 0x800,
            DDSCAPS2_CUBEMAP_POSITIVEY = 0x1000,
            DDSCAPS2_CUBEMAP_NEGATIVEY = 0x2000,
            DDSCAPS2_CUBEMAP_POSITIVEZ = 0x4000,
            DDSCAPS2_CUBEMAP_NEGATIVEZ = 0x8000,
            DDSCAPS2_VOLUME = 0x200000,

            DDS_CUBEMAP_POSITIVEX = DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_POSITIVEX,
            DDS_CUBEMAP_NEGATIVEX = DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_NEGATIVEX,
            DDS_CUBEMAP_POSITIVEY = DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_POSITIVEY,
            DDS_CUBEMAP_NEGATIVEY = DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_NEGATIVEY,
            DDS_CUBEMAP_POSITIVEZ = DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_POSITIVEZ,
            DDS_CUBEMAP_NEGATIVEZ = DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_NEGATIVEZ,
            DDS_CUBEMAP_ALLFACES = DDS_CUBEMAP_POSITIVEX | DDS_CUBEMAP_NEGATIVEX | DDS_CUBEMAP_POSITIVEY | DDS_CUBEMAP_NEGATIVEY | DDS_CUBEMAP_POSITIVEZ | DDSCAPS2_CUBEMAP_NEGATIVEZ,
            DDS_FLAGS_VOLUME = DDSCAPS2_VOLUME
        }
    }
}