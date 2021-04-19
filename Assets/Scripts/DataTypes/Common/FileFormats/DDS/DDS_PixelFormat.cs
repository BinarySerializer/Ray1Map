using System;
using BinarySerializer;

namespace R1Engine
{
    public class DDS_PixelFormat : BinarySerializable
    {
        public uint StructSize { get; set; }
        public DDS_PixelFormatFlags Flags { get; set; }
        public string FourCC { get; set; }
        public uint RGBBitCount { get; set; }
        public uint RBitMask { get; set; }
        public uint GBitMask { get; set; }
        public uint BBitMask { get; set; }
        public uint ABitMask { get; set; }

        public uint GetRGBBitCount => RGBBitCount == 0 ? 32 : RGBBitCount;

        public override void SerializeImpl(SerializerObject s)
        {
            StructSize = s.Serialize<uint>(StructSize, name: nameof(StructSize));

            if (StructSize != 32)
                throw new Exception($"Invalid DDS pixel format struct size of {StructSize}. Should be 32.");

            Flags = s.Serialize<DDS_PixelFormatFlags>(Flags, name: nameof(Flags));
            FourCC = s.SerializeString(FourCC, 4, name: nameof(FourCC));
            RGBBitCount = s.Serialize<uint>(RGBBitCount, name: nameof(RGBBitCount));
            RBitMask = s.Serialize<uint>(RBitMask, name: nameof(RBitMask));
            GBitMask = s.Serialize<uint>(GBitMask, name: nameof(GBitMask));
            BBitMask = s.Serialize<uint>(BBitMask, name: nameof(BBitMask));
            ABitMask = s.Serialize<uint>(ABitMask, name: nameof(ABitMask));
        }

        public const string D3DFMT_DXT1 = "DXT1";
        public const string D3DFMT_DXT2 = "DXT2";
        public const string D3DFMT_DXT3 = "DXT3";
        public const string D3DFMT_DXT4 = "DXT4";
        public const string D3DFMT_DXT5 = "DXT5";
        public const string DXGI_DX10 = "DX10";
        public const string DXGI_FORMAT_BC4_UNORM = "BC4U";
        public const string DXGI_FORMAT_BC4_SNORM = "BC4S";
        public const string DXGI_FORMAT_BC5_UNORM = "ATI2";
        public const string DXGI_FORMAT_BC5_SNORM = "BC5S";
        public const string D3DFMT_R8G8_B8G8 = "RGBG";
        public const string D3DFMT_G8R8_G8B8 = "GRGB";
        public const string D3DFMT_UYVY = "UYVY";
        public const string D3DFMT_YUY2 = "YUY2";
        public const string D3DFMT_A16B16G16R16 = "\u0024";
        public const string D3DFMT_Q16W16V16U16 = "\u006E";
        public const string D3DFMT_R16F = "\u006F";
        public const string D3DFMT_G16R16F = "\u0070";
        public const string D3DFMT_A16B16G16R16F = "\u0071";
        public const string D3DFMT_R32F = "\u0072";
        public const string D3DFMT_G32R32F = "\u0073";
        public const string D3DFMT_A32B32G32R32F = "\u0074";
        public const string D3DFMT_CxV8U8 = "\u0075";

        [Flags]
        public enum DDS_PixelFormatFlags : uint
        {
            /// <summary>
            /// Texture contains alpha data; <see cref="RGBAlphaBitMask"/> contains valid data.
            /// </summary>
            DDPF_ALPHAPIXELS = 0x1,

            /// <summary>
            /// Used in some older DDS files for alpha channel only uncompressed data (<see cref="DDS_PixelFormat.RGBBitCount"/> contains the alpha channel bitcount; <see cref="DDS_PixelFormat.ABitMask"/> contains valid data)
            /// </summary>
            DDPF_ALPHA = 0x2,

            /// <summary>
            /// Texture contains compressed RGB data; <see cref="FourCC"/> contains valid data.
            /// </summary>
            DDPF_FOURCC = 0x4,

            /// <summary>
            /// Texture contains uncompressed RGB data; <see cref="DDS_PixelFormat.RGBBitCount"/> and the RGB masks (<see cref="DDS_PixelFormat.RBitMask"/>, <see cref="DDS_PixelFormat.GBitMask"/>, <see cref="DDS_PixelFormat.BBitMask"/>) contain valid data.
            /// </summary>
            DDPF_RGB = 0x40,

            /// <summary>
            /// Used in some older DDS files for YUV uncompressed data (<see cref="DDS_PixelFormat.RGBBitCount"/> contains the YUV bit count; <see cref="DDS_PixelFormat.RBitMask"/> contains the Y mask, <see cref="DDS_PixelFormat.GBitMask"/> contains the U mask, <see cref="DDS_PixelFormat.BBitMask"/> contains the V mask).
            /// </summary>
            DDPF_YUV = 0x200,

            /// <summary>
            /// Used in some older DDS files for single channel color uncompressed data (<see cref="DDS_PixelFormat.RGBBitCount"/> contains the luminance channel bit count; <see cref="DDS_PixelFormat.RBitMask"/> contains the channel mask). Can be combined with <see cref="DDPF_ALPHAPIXELS"/> for a two channel DDS file.
            /// </summary>
            DDPF_LUMINANCE = 0x20000
        }
    }
}