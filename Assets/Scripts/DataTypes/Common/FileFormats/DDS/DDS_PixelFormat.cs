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
            DDPF_LUMINANCE = 0x20000,

            /// <summary>
            /// Custom NVTT Flag
            /// </summary>
            DDPF_NORMAL = 0x80000000,
        }


        private const int DefaultSize = 32;
        public static DDS_PixelFormat DdsPfA8R8G8B8() {
            DDS_PixelFormat pixelFormat = new DDS_PixelFormat {
                Size = DefaultSize,
                Flags = DDS_PixelFormatFlags.DDPF_RGB | DDS_PixelFormatFlags.DDPF_ALPHAPIXELS,
                FourCC = "",
                RGBBitCount = 32,
                RBitMask = 0x00ff0000,
                GBitMask = 0x0000ff00,
                BBitMask = 0x000000ff,
                ABitMask = 0xff000000
            };
            return pixelFormat;
        }

        public static DDS_PixelFormat DdsPfA1R5G5B5() {
            DDS_PixelFormat pixelFormat = new DDS_PixelFormat {
                Size = DefaultSize,
                Flags = DDS_PixelFormatFlags.DDPF_RGB | DDS_PixelFormatFlags.DDPF_ALPHAPIXELS,
                FourCC = "",
                RGBBitCount = 16,
                RBitMask = 0x00007c00,
                GBitMask = 0x000003e0,
                BBitMask = 0x0000001f,
                ABitMask = 0x00008000
            };
            return pixelFormat;
        }

        public static DDS_PixelFormat DdsPfA4R4G4B4() {
            DDS_PixelFormat pixelFormat = new DDS_PixelFormat {
                Size = DefaultSize,
                Flags = DDS_PixelFormatFlags.DDPF_RGB | DDS_PixelFormatFlags.DDPF_ALPHAPIXELS,
                FourCC = "",
                RGBBitCount = 16,
                RBitMask = 0x00000f00,
                GBitMask = 0x000000f0,
                BBitMask = 0x0000000f,
                ABitMask = 0x0000f000
            };
            return pixelFormat;
        }

        public static DDS_PixelFormat DdsPfR8G8B8() {
            DDS_PixelFormat pixelFormat = new DDS_PixelFormat {
                Size = DefaultSize,
                Flags = DDS_PixelFormatFlags.DDPF_RGB,
                FourCC = "",
                RGBBitCount = 24,
                RBitMask = 0x00ff0000,
                GBitMask = 0x0000ff00,
                BBitMask = 0x000000ff,
                ABitMask = 0x00000000
            };
            return pixelFormat;
        }

        public static DDS_PixelFormat DdsPfR5G6B5() {
            DDS_PixelFormat pixelFormat = new DDS_PixelFormat {
                Size = DefaultSize,
                Flags = DDS_PixelFormatFlags.DDPF_RGB,
                FourCC = "",
                RGBBitCount = 16,
                RBitMask = 0x0000f800,
                GBitMask = 0x000007e0,
                BBitMask = 0x0000001f,
                ABitMask = 0x00000000
            };
            return pixelFormat;
        }

        public static DDS_PixelFormat DdsPfDx10() {
            return DdsPfDx("DX10"); // DX10
        }

        private static DDS_PixelFormat DdsPfDx(string FourCC, bool normalFlag = false) {
            DDS_PixelFormat pixelFormat = new DDS_PixelFormat {
                Size = DefaultSize,
                Flags = DDS_PixelFormatFlags.DDPF_FOURCC,
                FourCC = FourCC
            };
            if (normalFlag) {
                pixelFormat.Flags |= DDS_PixelFormatFlags.DDPF_NORMAL;
            }
            return pixelFormat;
        }

        public static DDS_PixelFormat DdsLuminance() {
            DDS_PixelFormat pixelFormat = new DDS_PixelFormat {
                Size = DefaultSize,
                Flags = DDS_PixelFormatFlags.DDPF_LUMINANCE,
                RGBBitCount = 32,
                RBitMask = 0x000000ff
            };
            return pixelFormat;
        }

        public static DDS_PixelFormat DdsATI2() {
            DDS_PixelFormat pixelFormat = new DDS_PixelFormat {
                Size = DefaultSize,
                RGBBitCount = 24,
                Flags = DDS_PixelFormatFlags.DDPF_FOURCC,
                FourCC = "ATI2"
            };
            return pixelFormat;
        }

        public static DDS_PixelFormat GetDefaultPixelFormat(DDSParser.PixelFormat format) {
            var fmt = format switch
            {
                DDSParser.PixelFormat.RGBA => DdsPfA8R8G8B8(),
                DDSParser.PixelFormat.RGB => DdsPfR8G8B8(),
                DDSParser.PixelFormat.DXT1 => DdsPfDx("DXT1"),
                DDSParser.PixelFormat.DXT2 => DdsPfDx("DXT2"),
                DDSParser.PixelFormat.DXT3 => DdsPfDx("DXT3"),
                DDSParser.PixelFormat.DXT4 => DdsPfDx("DXT4"),
                DDSParser.PixelFormat.DXT5 => DdsPfDx("DXT5"),
                DDSParser.PixelFormat.DXT5n => DdsPfDx("DXT5", true),
                DDSParser.PixelFormat.THREEDC => DdsATI2(),
                DDSParser.PixelFormat.LUMINANCE => DdsLuminance(),
                _ => new DDS_PixelFormat()
            };
            return fmt;
        }

    }
}