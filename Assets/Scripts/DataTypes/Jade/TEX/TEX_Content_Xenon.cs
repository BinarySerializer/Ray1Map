using BinarySerializer;
using System;
using UnityEngine;

namespace R1Engine.Jade
{
    public class TEX_Content_Xenon : BinarySerializable {
        public uint FileSize { get; set; } // Set in onPreSerialize

        public string Header { get; set; }
        public const string XenonHeader = "D2KK";

        public uint Width { get; set; }
        public uint Height { get; set; }
        public XenonFormat Format { get; set; }
        public uint UInt_10 { get; set; }
        public short Short_14 { get; set; }
        public short Short_16 { get; set; }
        public uint DataSize { get; set; }
        public uint UInt_1C { get; set; }

        public byte[] Data { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            s.DoEndian(Endian.Big, () => {
                Header = s.SerializeString(Header, length: 4, encoding: Jade_BaseManager.Encoding, name: nameof(Header));
                if(Header != XenonHeader) throw new Exception($"File at {Offset} is not a XenonTexture");

                Width = s.Serialize<uint>(Width, name: nameof(Width));
                Height = s.Serialize<uint>(Height, name: nameof(Height));
                Format = s.Serialize<XenonFormat>(Format, name: nameof(Format));
                UInt_10 = s.Serialize<uint>(UInt_10, name: nameof(UInt_10));
                Short_14 = s.Serialize<short>(Short_14, name: nameof(Short_14));
                Short_16 = s.Serialize<short>(Short_16, name: nameof(Short_16));
                DataSize = s.Serialize<uint>(DataSize, name: nameof(DataSize));
                UInt_1C = s.Serialize<uint>(UInt_1C, name: nameof(UInt_1C));

                Data = s.SerializeArray<byte>(Data, DataSize, name: nameof(Data));
            });
        }

        public Texture2D ToTexture2D() {
            //var tex = TextureHelpers.CreateTexture2D((int)Width, (int)Height);

            byte[] data = Data;
            if (!Enum.IsDefined(typeof(XenonFormat), Format)) {
                throw new NotImplementedException($"Unsupported XenonTexture format {Format}");
            }
            if (Format != XenonFormat.LUMINANCE) { // Add all 1-byte formats here
                data = new byte[Data.Length];
                for (int i = 0; i < Data.Length / 2; i++) {
                    data[i * 2] = Data[i * 2 + 1];
                    data[i * 2 + 1] = Data[i * 2];
                }
            }
            if (((uint)Format & 0x100) == 0x100) { // Is swizzled
                byte[] outData = new byte[data.Length];
                switch (Format) {
                    case XenonFormat.DXT1:
                        UntileXbox360Texture(data, outData, (int)Width, (int)Height, 8, 128, 128, 4, 4);
                        break;
                    case XenonFormat.DXT5:
                        UntileXbox360Texture(data, outData, (int)Width, (int)Height, 16, 128, 128, 4, 4);
                        break;
                    case XenonFormat.RGBA8888:
                        UntileXbox360Texture(data, outData, (int)Width, (int)Height, 4, 32, 32, 1, 1);
                        break;
                    case XenonFormat.BC5U:
                        UntileXbox360Texture(data, outData, (int)Width, (int)Height, 16, 128, 128, 4, 4);
                        break;
                    case XenonFormat.LUMINANCE:
                        UntileXbox360Texture(data, outData, (int)Width, (int)Height, 1, 0, 0, 1, 1);
                        break;
                }
                data = outData;
            }

            /*if (Format == XenonFormat.LUMINANCE) {
                Util.ByteArrayToFile($"{Context.BasePath}/../tex/raw/{Format}_{Offset.StringFileOffset}.bin", data);
            }*/

            var dds = Format switch
            {
                XenonFormat.DXT1 => DDS.FromRawData(data, DDSParser.PixelFormat.DXT1, Width, Height),
                XenonFormat.DXT5 => DDS.FromRawData(data, DDSParser.PixelFormat.DXT5, Width, Height),
                XenonFormat.BC5U => DDS.FromRawData(data, DDSParser.PixelFormat.THREEDC, Width, Height),
                XenonFormat.LUMINANCE => DDS.FromRawData(data, DDSParser.PixelFormat.LUMINANCE, Width, Height),
                XenonFormat.RGBA8888 => DDS.FromRawData(data, DDSParser.PixelFormat.RGBA, Width, Height),
                _ => null
            };
            return dds?.PrimaryTexture?.ToTexture2D();
        }

        public enum XenonFormat : uint {
            LUMINANCE    = 0x04900102,
            DXT1 = 0x1A200152,
            DXT5 = 0x1A200154,
            BC5U   = 0x1A200171, // For normal maps
            RGBA8888    = 0x18280186,
        }

        // Based on https://github.com/gildor2/UEViewer/blob/eaba2837228f9fe39134616d7bff734acd314ffb/Unreal/UnrealMaterial/UnTexture.cpp#L562
        private void UntileXbox360Texture(byte[] srcData, byte[] dstData,
            int originalWidth, int originalHeight,
            int bytesPerBlock,
            int alignX, int alignY, int blockSizeX, int blockSizeY) {

            int alignedWidth = Align(originalWidth, alignX);
            int alignedHeight = Align(originalHeight, alignY);

            int tiledBlockWidth = alignedWidth / blockSizeX;       // width of image in blocks
            int originalBlockWidth = originalWidth / blockSizeX;       // width of image in blocks
            int tiledBlockHeight = alignedHeight / blockSizeY;     // height of image in blocks
            int originalBlockHeight = originalHeight / blockSizeY;      // height of image in blocks
            int logBpp = appLog2(bytesPerBlock);

            // XBox360 has packed multiple lower mip levels into a single tile - should use special code
            // to unpack it. Textures are aligned to bottom-right corder.
            // Packing looks like this:
            // ....CCCCBBBBBBBBAAAAAAAAAAAAAAAA
            // ....CCCCBBBBBBBBAAAAAAAAAAAAAAAA
            // E.......BBBBBBBBAAAAAAAAAAAAAAAA
            // ........BBBBBBBBAAAAAAAAAAAAAAAA
            // DD..............AAAAAAAAAAAAAAAA
            // ................AAAAAAAAAAAAAAAA
            // ................AAAAAAAAAAAAAAAA
            // ................AAAAAAAAAAAAAAAA
            // (Where mips are A,B,C,D,E - E is 1x1, D is 2x2 etc)
            // Force sxOffset=0 and enable DEBUG_MIPS in UnRender.cpp to visualize this layout.
            // So we should offset X coordinate when unpacking to the width of mip level.
            // Note: this doesn't work with non-square textures.
            int sxOffset = 0, syOffset = 0;

            // We're handling only size=16 here.
            if ((tiledBlockWidth >= originalBlockWidth * 2) && (originalWidth == 16)) {
                sxOffset = originalBlockWidth;
            }

            if ((tiledBlockHeight >= originalBlockHeight * 2) && (originalHeight == 16)) {
                syOffset = originalBlockHeight;
            }

            int numImageBlocks = tiledBlockWidth * tiledBlockHeight;    // used for verification

            // Iterate over image blocks
            for (int dy = 0; dy < originalBlockHeight; dy++) {
                for (int dx = 0; dx < originalBlockWidth; dx++) {
                    // Unswizzle only once for a whole block
                    uint swzAddr = GetXbox360TiledOffset(dx + sxOffset, dy + syOffset, tiledBlockWidth, logBpp);
                    if (swzAddr >= numImageBlocks) throw new Exception("Error in XenonTexture parsing");
                    int sy = (int)(swzAddr / tiledBlockWidth);
                    int sx = (int)(swzAddr % tiledBlockWidth);

                    int dstStart = (dy * originalBlockWidth + dx) * bytesPerBlock;
                    int srcStart = (sy * tiledBlockWidth + sx) * bytesPerBlock;
                    Array.Copy(srcData, srcStart, dstData, dstStart, bytesPerBlock);
                }
            }
        }
        uint GetXbox360TiledOffset(int x, int y, int width, int logBpb) {
            if (width > 8192) throw new Exception($"XenonTexture: Width={width} too large");
            if (width <= x) throw new Exception($"XenonTexture: X={x} too large for width={width}");

            int alignedWidth = Align(width, 32);
            // top bits of coordinates
            int macro = ((x >> 5) + (y >> 5) * (alignedWidth >> 5)) << (logBpb + 7);
            // lower bits of coordinates (result is 6-bit value)
            int micro = ((x & 7) + ((y & 0xE) << 2)) << logBpb;
            // mix micro/macro + add few remaining x/y bits
            int offset = macro + ((micro & ~0xF) << 1) + (micro & 0xF) + ((y & 1) << 4);
            // mix bits again
            return (uint)((((offset & ~0x1FF) << 3) +                  // upper bits (offset bits [*-9])
                    ((y & 16) << 7) +                           // next 1 bit
                    ((offset & 0x1C0) << 2) +                   // next 3 bits (offset bits [8-6])
                    (((((y & 8) >> 2) + (x >> 3)) & 3) << 6) +  // next 2 bits
                    (offset & 0x3F)                             // lower 6 bits (offset bits [5-0])
                    ) >> logBpb);
        }

        int Align(int value, int align) {
            if(align == 0) return value;
            return (value % align != 0) ? ((value / align) + 1) * (align) : value;
        }
        int appLog2(int n) {
            int r;
            for (r = -1; n != 0; n >>= 1, r++) { /*empty*/ }
            return r;
        }
    }
}