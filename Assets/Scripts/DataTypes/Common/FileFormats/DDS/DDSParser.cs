using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace R1Engine
{
    public static class DDSParser
    {
        #region Public Static Methods

        public static byte[] DecompressData(BinaryReader reader, DDS_Header header)
        {
            if (header.Depth == 0)
                header.Depth = 1;

            var pixelFormat = GetFormat(header);

            if (pixelFormat == PixelFormat.UNKNOWN)
                throw new Exception("Unknown pixel format");

            var data = ReadData(reader, header);

            if (data == null)
                return null;

            return DecompressData(header, data, pixelFormat);
        }

        public static byte[] DecompressData(DDS_Header header, byte[] data, PixelFormat pixelFormat)
        {
            var rawData = pixelFormat switch
            {
                PixelFormat.RGBA => DecompressRGBA(header, data, pixelFormat),
                PixelFormat.RGB => DecompressRGB(header, data, pixelFormat),
                PixelFormat.LUMINANCE => DecompressLum(header, data, pixelFormat),
                PixelFormat.LUMINANCE_ALPHA => DecompressLum(header, data, pixelFormat),
                PixelFormat.DXT1 => DecompressDXT1(header, data, pixelFormat),
                PixelFormat.DXT2 => DecompressDXT2(header, data, pixelFormat),
                PixelFormat.DXT3 => DecompressDXT3(header, data, pixelFormat),
                PixelFormat.DXT4 => DecompressDXT4(header, data, pixelFormat),
                PixelFormat.DXT5 => DecompressDXT5(header, data, pixelFormat),
                _ => throw new Exception("Unknown format: " + pixelFormat)
            };

            return rawData;
        }

        #endregion

        #region Private Static Methods

        private static byte[] ReadData(BinaryReader reader, DDS_Header header)
        {
            if (header.Flags.HasFlag(DDS_Header.DDS_HeaderFlags.DDSD_LINEARSIZE))
            {
                return reader.ReadBytes((int)header.PitchOrLinearSize);
            }
            else
            {
                var bps = header.Width * header.PixelFormat.GetRGBBitCount / sizeof(byte);
                return reader.ReadBytes((int)(bps * header.Height * header.Depth));
            }
        }

        private static PixelFormat GetFormat(DDS_Header header)
        {
            PixelFormat format;
            if (header.PixelFormat.Flags.HasFlag(DDS_PixelFormat.DDS_PixelFormatFlags.DDPF_FOURCC))
            {
                var fourCC = Encoding.Default.GetBytes(header.PixelFormat.FourCC);

                format = BitConverter.ToUInt32(fourCC, 0) switch
                {
                    FOURCC_DXT1 => PixelFormat.DXT1,
                    FOURCC_DXT2 => PixelFormat.DXT2,
                    FOURCC_DXT3 => PixelFormat.DXT3,
                    FOURCC_DXT4 => PixelFormat.DXT4,
                    FOURCC_DXT5 => PixelFormat.DXT5,
                    FOURCC_ATI1 => PixelFormat.ATI1N,
                    FOURCC_ATI2 => PixelFormat.THREEDC,
                    FOURCC_RXGB => PixelFormat.RXGB,
                    FOURCC_DOLLARNULL => PixelFormat.A16B16G16R16,
                    FOURCC_oNULL => PixelFormat.R16F,
                    FOURCC_pNULL => PixelFormat.G16R16F,
                    FOURCC_qNULL => PixelFormat.A16B16G16R16F,
                    FOURCC_rNULL => PixelFormat.R32F,
                    FOURCC_sNULL => PixelFormat.G32R32F,
                    FOURCC_tNULL => PixelFormat.A32B32G32R32F,
                    _ => PixelFormat.UNKNOWN
                };
            }
            else
            {
                // uncompressed image
                if (header.PixelFormat.Flags.HasFlag(DDS_PixelFormat.DDS_PixelFormatFlags.DDPF_LUMINANCE))
                {
                    if (header.PixelFormat.Flags.HasFlag(DDS_PixelFormat.DDS_PixelFormatFlags.DDPF_ALPHAPIXELS))
                        format = PixelFormat.LUMINANCE_ALPHA;
                    else
                        format = PixelFormat.LUMINANCE;
                }
                else
                {
                    if (header.PixelFormat.Flags.HasFlag(DDS_PixelFormat.DDS_PixelFormatFlags.DDPF_LUMINANCE))
                        format = PixelFormat.RGBA;
                    else
                        format = PixelFormat.RGB;
                }
            }

            return format;
        }

        #region Helper Methods

        // iCompFormatToBpp
        private static uint PixelFormatToBpp(PixelFormat pf, uint rgbbitcount)
        {
            switch (pf)
            {
                case PixelFormat.LUMINANCE:
                case PixelFormat.LUMINANCE_ALPHA:
                case PixelFormat.RGBA:
                case PixelFormat.RGB:
                    return rgbbitcount / 8;

                case PixelFormat.THREEDC:
                case PixelFormat.RXGB:
                    return 3;

                case PixelFormat.ATI1N:
                    return 1;

                case PixelFormat.R16F:
                    return 2;

                case PixelFormat.A16B16G16R16:
                case PixelFormat.A16B16G16R16F:
                case PixelFormat.G32R32F:
                    return 8;

                case PixelFormat.A32B32G32R32F:
                    return 16;

                default:
                    return 4;
            }
        }

        // iCompFormatToBpc
        private static uint PixelFormatToBpc(PixelFormat pf)
        {
            switch (pf)
            {
                case PixelFormat.R16F:
                case PixelFormat.G16R16F:
                case PixelFormat.A16B16G16R16F:
                    return 4;

                case PixelFormat.R32F:
                case PixelFormat.G32R32F:
                case PixelFormat.A32B32G32R32F:
                    return 4;

                case PixelFormat.A16B16G16R16:
                    return 2;

                default:
                    return 1;
            }
        }

        private static bool Check16BitComponents(DDS_Header header)
        {
            if (header.PixelFormat.GetRGBBitCount != 32)
                return false;
            // a2b10g10r10 format
            if (header.PixelFormat.RBitMask == 0x3FF00000 && header.PixelFormat.GBitMask == 0x000FFC00 && header.PixelFormat.BBitMask == 0x000003FF
                && header.PixelFormat.ABitMask == 0xC0000000)
                return true;
            // a2r10g10b10 format
            else if (header.PixelFormat.RBitMask == 0x000003FF && header.PixelFormat.GBitMask == 0x000FFC00 && header.PixelFormat.BBitMask == 0x3FF00000
                     && header.PixelFormat.ABitMask == 0xC0000000)
                return true;

            return false;
        }

        private static void CorrectPremult(uint pixnum, ref byte[] buffer)
        {
            for (uint i = 0; i < pixnum; i++)
            {
                byte alpha = buffer[i + 3];
                if (alpha == 0) continue;
                int red = (buffer[i] << 8) / alpha;
                int green = (buffer[i + 1] << 8) / alpha;
                int blue = (buffer[i + 2] << 8) / alpha;

                buffer[i] = (byte)red;
                buffer[i + 1] = (byte)green;
                buffer[i + 2] = (byte)blue;
            }
        }

        private static void ComputeMaskParams(uint mask, ref int shift1, ref int mul, ref int shift2)
        {
            shift1 = 0; mul = 1; shift2 = 0;
            while ((mask & 1) == 0)
            {
                mask >>= 1;
                shift1++;
            }
            uint bc = 0;
            while ((mask & (1 << (int)bc)) != 0) bc++;
            while ((mask * mul) < 255)
                mul = (mul << (int)bc) + 1;
            mask *= (uint)mul;

            while ((mask & ~0xff) != 0)
            {
                mask >>= 1;
                shift2++;
            }
        }

        private static void DxtcReadColors(byte[] data, int pos, ref Colour8888[] op)
        {
            byte r0, g0, b0, r1, g1, b1;

            b0 = (byte)(data[pos + 0] & 0x1F);
            g0 = (byte)(((data[pos + 0] & 0xE0) >> 5) | ((data[pos + 1] & 0x7) << 3));
            r0 = (byte)((data[pos + 1] & 0xF8) >> 3);

            b1 = (byte)(data[pos + 2] & 0x1F);
            g1 = (byte)(((data[pos + 2] & 0xE0) >> 5) | ((data[pos + 3] & 0x7) << 3));
            r1 = (byte)((data[pos + 3] & 0xF8) >> 3);

            op[0].red = (byte)(r0 << 3 | r0 >> 2);
            op[0].green = (byte)(g0 << 2 | g0 >> 3);
            op[0].blue = (byte)(b0 << 3 | b0 >> 2);

            op[1].red = (byte)(r1 << 3 | r1 >> 2);
            op[1].green = (byte)(g1 << 2 | g1 >> 3);
            op[1].blue = (byte)(b1 << 3 | b1 >> 2);
        }

        private static void DxtcReadColor(ushort data, ref Colour8888 op)
        {
            byte r, g, b;

            b = (byte)(data & 0x1f);
            g = (byte)((data & 0x7E0) >> 5);
            r = (byte)((data & 0xF800) >> 11);

            op.red = (byte)(r << 3 | r >> 2);
            op.green = (byte)(g << 2 | g >> 3);
            op.blue = (byte)(b << 3 | r >> 2);
        }

        private static void DxtcReadColors(byte[] data, int pos, ref Colour565 color_0, ref Colour565 color_1)
        {
            color_0.blue = (byte)(data[pos + 0] & 0x1F);
            color_0.green = (byte)(((data[pos + 0] & 0xE0) >> 5) | ((data[pos + 1] & 0x7) << 3));
            color_0.red = (byte)((data[pos + 1] & 0xF8) >> 3);

            color_0.blue = (byte)(data[pos + 2] & 0x1F);
            color_0.green = (byte)(((data[pos + 2] & 0xE0) >> 5) | ((data[pos + 3] & 0x7) << 3));
            color_0.red = (byte)((data[pos + 3] & 0xF8) >> 3);
        }

        private static void GetBitsFromMask(uint mask, ref uint shiftLeft, ref uint shiftRight)
        {
            uint temp, i;

            if (mask == 0)
            {
                shiftLeft = shiftRight = 0;
                return;
            }

            temp = mask;
            for (i = 0; i < 32; i++, temp >>= 1)
            {
                if ((temp & 1) != 0)
                    break;
            }
            shiftRight = i;

            // Temp is preserved, so use it again:
            for (i = 0; i < 8; i++, temp >>= 1)
            {
                if ((temp & 1) == 0)
                    break;
            }
            shiftLeft = 8 - i;
        }

        // This function simply counts how many contiguous bits are in the mask.
        private static uint CountBitsFromMask(uint mask)
        {
            uint i, testBit = 0x01, count = 0;
            bool foundBit = false;

            for (i = 0; i < 32; i++, testBit <<= 1)
            {
                if ((mask & testBit) != 0)
                {
                    if (!foundBit)
                        foundBit = true;
                    count++;
                }
                else if (foundBit)
                    return count;
            }

            return count;
        }

        private static uint HalfToFloat(ushort y)
        {
            int s = (y >> 15) & 0x00000001;
            int e = (y >> 10) & 0x0000001f;
            int m = y & 0x000003ff;

            if (e == 0)
            {
                if (m == 0)
                {
                    //
                    // Plus or minus zero
                    //
                    return (uint)(s << 31);
                }
                else
                {
                    //
                    // Denormalized number -- renormalize it
                    //
                    while ((m & 0x00000400) == 0)
                    {
                        m <<= 1;
                        e -= 1;
                    }

                    e += 1;
                    m &= ~0x00000400;
                }
            }
            else if (e == 31)
            {
                if (m == 0)
                {
                    //
                    // Positive or negative infinity
                    //
                    return (uint)((s << 31) | 0x7f800000);
                }
                else
                {
                    //
                    // Nan -- preserve sign and significand bits
                    //
                    return (uint)((s << 31) | 0x7f800000 | (m << 13));
                }
            }

            //
            // Normalized number
            //
            e = e + (127 - 15);
            m = m << 13;

            //
            // Assemble s, e and m.
            //
            return (uint)((s << 31) | (e << 23) | m);
        }

        #endregion

        #region Decompress Methods

        private static byte[] DecompressDXT1(DDS_Header header, byte[] data, PixelFormat pixelFormat)
        {
            // allocate bitmap
            int bpp = (int)(PixelFormatToBpp(pixelFormat, header.PixelFormat.GetRGBBitCount));
            int bps = (int)(header.Width * bpp * PixelFormatToBpc(pixelFormat)); // 1024
            int sizeofplane = (int)(bps * header.Height); // 1024
            int width = (int)header.Width;
            int height = (int)header.Height;
            int depth = (int)header.Depth;

            // DXT1 decompressor
            byte[] rawData = new byte[depth * sizeofplane + height * bps + width * bpp];

            Colour8888[] colours = new Colour8888[4];
            colours[0].alpha = 0xFF;
            colours[1].alpha = 0xFF;
            colours[2].alpha = 0xFF;

            int temp = 0;
            for (int z = 0; z < depth; z++)
            {
                for (int y = 0; y < height; y += 4)
                {
                    for (int x = 0; x < width; x += 4)
                    {
                        ushort colour0 = BitConverter.ToUInt16(data, temp);
                        ushort colour1 = BitConverter.ToUInt16(data, temp + 2);
                        DxtcReadColor(colour0, ref colours[0]);
                        DxtcReadColor(colour1, ref colours[1]);

                        uint bitmask = BitConverter.ToUInt32(data, temp + 4);
                        temp += 8;

                        if (colour0 > colour1)
                        {
                            // Four-color block: derive the other two colors.
                            // 00 = color_0, 01 = color_1, 10 = color_2, 11 = color_3
                            // These 2-bit codes correspond to the 2-bit fields
                            // stored in the 64-bit block.
                            colours[2].blue = (byte)((2 * colours[0].blue + colours[1].blue + 1) / 3);
                            colours[2].green = (byte)((2 * colours[0].green + colours[1].green + 1) / 3);
                            colours[2].red = (byte)((2 * colours[0].red + colours[1].red + 1) / 3);
                            //colours[2].alpha = 0xFF;

                            colours[3].blue = (byte)((colours[0].blue + 2 * colours[1].blue + 1) / 3);
                            colours[3].green = (byte)((colours[0].green + 2 * colours[1].green + 1) / 3);
                            colours[3].red = (byte)((colours[0].red + 2 * colours[1].red + 1) / 3);
                            colours[3].alpha = 0xFF;
                        }
                        else
                        {
                            // Three-color block: derive the other color.
                            // 00 = color_0,  01 = color_1,  10 = color_2,
                            // 11 = transparent.
                            // These 2-bit codes correspond to the 2-bit fields 
                            // stored in the 64-bit block. 
                            colours[2].blue = (byte)((colours[0].blue + colours[1].blue) / 2);
                            colours[2].green = (byte)((colours[0].green + colours[1].green) / 2);
                            colours[2].red = (byte)((colours[0].red + colours[1].red) / 2);
                            //colours[2].alpha = 0xFF;

                            colours[3].blue = (byte)((colours[0].blue + 2 * colours[1].blue + 1) / 3);
                            colours[3].green = (byte)((colours[0].green + 2 * colours[1].green + 1) / 3);
                            colours[3].red = (byte)((colours[0].red + 2 * colours[1].red + 1) / 3);
                            colours[3].alpha = 0x00;
                        }

                        for (int j = 0, k = 0; j < 4; j++)
                        {
                            for (int i = 0; i < 4; i++, k++)
                            {
                                int select = (int)((bitmask & (0x03 << k * 2)) >> k * 2);
                                Colour8888 col = colours[select];
                                if (((x + i) < width) && ((y + j) < height))
                                {
                                    uint offset = (uint)(z * sizeofplane + (y + j) * bps + (x + i) * bpp);
                                    rawData[offset + 0] = (byte)col.red;
                                    rawData[offset + 1] = (byte)col.green;
                                    rawData[offset + 2] = (byte)col.blue;
                                    rawData[offset + 3] = (byte)col.alpha;
                                }
                            }
                        }
                    }
                }
            }

            return rawData;
        }

        private static byte[] DecompressDXT2(DDS_Header header, byte[] data, PixelFormat pixelFormat)
        {
            // allocate bitmap
            int width = (int)header.Width;
            int height = (int)header.Height;
            int depth = (int)header.Depth;

            // Can do color & alpha same as dxt3, but color is pre-multiplied
            // so the result will be wrong unless corrected.
            byte[] rawData = DecompressDXT3(header, data, pixelFormat);
            CorrectPremult((uint)(width * height * depth), ref rawData);

            return rawData;
        }

        private static byte[] DecompressDXT3(DDS_Header header, byte[] data, PixelFormat pixelFormat)
        {
            // allocate bitmap
            int bpp = (int)(PixelFormatToBpp(pixelFormat, header.PixelFormat.GetRGBBitCount));
            int bps = (int)(header.Width * bpp * PixelFormatToBpc(pixelFormat));
            int sizeofplane = (int)(bps * header.Height);
            int width = (int)header.Width;
            int height = (int)header.Height;
            int depth = (int)header.Depth;

            // DXT3 decompressor
            byte[] rawData = new byte[depth * sizeofplane + height * bps + width * bpp];
            Colour8888[] colours = new Colour8888[4];

            int temp = 0;
            for (int z = 0; z < depth; z++)
            {
                for (int y = 0; y < height; y += 4)
                {
                    for (int x = 0; x < width; x += 4)
                    {
                        int alpha = temp;
                        temp += 8;

                        DxtcReadColors(data, temp, ref colours);
                        temp += 4;

                        uint bitmask;
                        try
                        {
                            bitmask = BitConverter.ToUInt32(data, temp);
                            temp += 4;
                        }
                        catch (Exception ex)
                        {
                            Debug.Log("StartIndex: " + (temp + 4) + " - Size: " + data.Length);
                            throw ex;
                        }

                        // Four-color block: derive the other two colors.
                        // 00 = color_0, 01 = color_1, 10 = color_2, 11	= color_3
                        // These 2-bit codes correspond to the 2-bit fields
                        // stored in the 64-bit block.
                        colours[2].blue = (byte)((2 * colours[0].blue + colours[1].blue + 1) / 3);
                        colours[2].green = (byte)((2 * colours[0].green + colours[1].green + 1) / 3);
                        colours[2].red = (byte)((2 * colours[0].red + colours[1].red + 1) / 3);
                        //colours[2].alpha = 0xFF;

                        colours[3].blue = (byte)((colours[0].blue + 2 * colours[1].blue + 1) / 3);
                        colours[3].green = (byte)((colours[0].green + 2 * colours[1].green + 1) / 3);
                        colours[3].red = (byte)((colours[0].red + 2 * colours[1].red + 1) / 3);
                        //colours[3].alpha = 0xFF;

                        for (int j = 0, k = 0; j < 4; j++)
                        {
                            for (int i = 0; i < 4; k++, i++)
                            {
                                int select = (int)((bitmask & (0x03 << k * 2)) >> k * 2);

                                if (((x + i) < width) && ((y + j) < height))
                                {
                                    uint offset = (uint)(z * sizeofplane + (y + j) * bps + (x + i) * bpp);
                                    rawData[offset + 0] = (byte)colours[select].red;
                                    rawData[offset + 1] = (byte)colours[select].green;
                                    rawData[offset + 2] = (byte)colours[select].blue;
                                }
                            }
                        }

                        for (int j = 0; j < 4; j++)
                        {
                            //ushort word = (ushort)(alpha[2 * j] + 256 * alpha[2 * j + 1]);
                            ushort word = (ushort)(data[alpha + 2 * j] | (data[alpha + 2 * j + 1] << 8));
                            for (int i = 0; i < 4; i++)
                            {
                                if (((x + i) < width) && ((y + j) < height))
                                {
                                    uint offset = (uint)(z * sizeofplane + (y + j) * bps + (x + i) * bpp + 3);
                                    rawData[offset] = (byte)(word & 0x0F);
                                    rawData[offset] = (byte)(rawData[offset] | (rawData[offset] << 4));
                                }
                                word >>= 4;
                            }
                        }
                    }
                }
            }
            return rawData;
        }

        private static byte[] DecompressDXT4(DDS_Header header, byte[] data, PixelFormat pixelFormat)
        {
            // allocate bitmap
            int width = (int)header.Width;
            int height = (int)header.Height;
            int depth = (int)header.Depth;

            // Can do color & alpha same as dxt5, but color is pre-multiplied
            // so the result will be wrong unless corrected.
            byte[] rawData = DecompressDXT5(header, data, pixelFormat);
            CorrectPremult((uint)(width * height * depth), ref rawData);

            return rawData;
        }

        private static byte[] DecompressDXT5(DDS_Header header, byte[] data, PixelFormat pixelFormat)
        {
            // allocate bitmap
            int bpp = (int)(PixelFormatToBpp(pixelFormat, header.PixelFormat.GetRGBBitCount));
            int bps = (int)(header.Width * bpp * PixelFormatToBpc(pixelFormat));
            int sizeofplane = (int)(bps * header.Height);
            int width = (int)header.Width;
            int height = (int)header.Height;
            int depth = (int)header.Depth;

            byte[] rawData = new byte[depth * sizeofplane + height * bps + width * bpp];
            Colour8888[] colours = new Colour8888[4];
            ushort[] alphas = new ushort[8];

            int temp = 0;
            for (int z = 0; z < depth; z++)
            {
                for (int y = 0; y < height; y += 4)
                {
                    for (int x = 0; x < width; x += 4)
                    {
                        if (y >= height || x >= width)
                            break;
                        alphas[0] = data[temp + 0];
                        alphas[1] = data[temp + 1];
                        int alphamask = (temp + 2);
                        temp += 8;

                        DxtcReadColors(data, temp, ref colours);
                        uint bitmask = BitConverter.ToUInt32(data, temp + 4);
                        temp += 8;

                        // Four-color block: derive the other two colors.
                        // 00 = color_0, 01 = color_1, 10 = color_2, 11	= color_3
                        // These 2-bit codes correspond to the 2-bit fields
                        // stored in the 64-bit block.
                        colours[2].blue = (byte)((2 * colours[0].blue + colours[1].blue + 1) / 3);
                        colours[2].green = (byte)((2 * colours[0].green + colours[1].green + 1) / 3);
                        colours[2].red = (byte)((2 * colours[0].red + colours[1].red + 1) / 3);
                        //colours[2].alpha = 0xFF;

                        colours[3].blue = (byte)((colours[0].blue + 2 * colours[1].blue + 1) / 3);
                        colours[3].green = (byte)((colours[0].green + 2 * colours[1].green + 1) / 3);
                        colours[3].red = (byte)((colours[0].red + 2 * colours[1].red + 1) / 3);
                        //colours[3].alpha = 0xFF;

                        int k = 0;
                        for (int j = 0; j < 4; j++)
                        {
                            for (int i = 0; i < 4; k++, i++)
                            {
                                int select = (int)((bitmask & (0x03 << k * 2)) >> k * 2);
                                Colour8888 col = colours[select];
                                // only put pixels out < width or height
                                if (((x + i) < width) && ((y + j) < height))
                                {
                                    uint offset = (uint)(z * sizeofplane + (y + j) * bps + (x + i) * bpp);
                                    rawData[offset] = (byte)col.red;
                                    rawData[offset + 1] = (byte)col.green;
                                    rawData[offset + 2] = (byte)col.blue;
                                }
                            }
                        }

                        // 8-alpha or 6-alpha block?
                        if (alphas[0] > alphas[1])
                        {
                            // 8-alpha block:  derive the other six alphas.
                            // Bit code 000 = alpha_0, 001 = alpha_1, others are interpolated.
                            alphas[2] = (ushort)((6 * alphas[0] + 1 * alphas[1] + 3) / 7); // bit code 010
                            alphas[3] = (ushort)((5 * alphas[0] + 2 * alphas[1] + 3) / 7); // bit code 011
                            alphas[4] = (ushort)((4 * alphas[0] + 3 * alphas[1] + 3) / 7); // bit code 100
                            alphas[5] = (ushort)((3 * alphas[0] + 4 * alphas[1] + 3) / 7); // bit code 101
                            alphas[6] = (ushort)((2 * alphas[0] + 5 * alphas[1] + 3) / 7); // bit code 110
                            alphas[7] = (ushort)((1 * alphas[0] + 6 * alphas[1] + 3) / 7); // bit code 111
                        }
                        else
                        {
                            // 6-alpha block.
                            // Bit code 000 = alpha_0, 001 = alpha_1, others are interpolated.
                            alphas[2] = (ushort)((4 * alphas[0] + 1 * alphas[1] + 2) / 5); // Bit code 010
                            alphas[3] = (ushort)((3 * alphas[0] + 2 * alphas[1] + 2) / 5); // Bit code 011
                            alphas[4] = (ushort)((2 * alphas[0] + 3 * alphas[1] + 2) / 5); // Bit code 100
                            alphas[5] = (ushort)((1 * alphas[0] + 4 * alphas[1] + 2) / 5); // Bit code 101
                            alphas[6] = 0x00; // Bit code 110
                            alphas[7] = 0xFF; // Bit code 111
                        }

                        // Note: Have to separate the next two loops,
                        // it operates on a 6-byte system.

                        // First three bytes
                        //uint bits = (uint)(alphamask[0]);
                        uint bits = (uint)((data[alphamask + 0]) | (data[alphamask + 1] << 8) | (data[alphamask + 2] << 16));
                        for (int j = 0; j < 2; j++)
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                // only put pixels out < width or height
                                if (((x + i) < width) && ((y + j) < height))
                                {
                                    uint offset = (uint)(z * sizeofplane + (y + j) * bps + (x + i) * bpp + 3);
                                    rawData[offset] = (byte)alphas[bits & 0x07];
                                }
                                bits >>= 3;
                            }
                        }

                        // Last three bytes
                        //bits = (uint)(alphamask[3]);
                        bits = (uint)((data[alphamask + 3]) | (data[alphamask + 4] << 8) | (data[alphamask + 5] << 16));
                        for (int j = 2; j < 4; j++)
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                // only put pixels out < width or height
                                if (((x + i) < width) && ((y + j) < height))
                                {
                                    uint offset = (uint)(z * sizeofplane + (y + j) * bps + (x + i) * bpp + 3);
                                    rawData[offset] = (byte)alphas[bits & 0x07];
                                }
                                bits >>= 3;
                            }
                        }
                    }
                }
            }

            return rawData;
        }

        private static byte[] DecompressRGB(DDS_Header header, byte[] data, PixelFormat pixelFormat)
        {
            // allocate bitmap
            int bpp = (int)(PixelFormatToBpp(pixelFormat, header.PixelFormat.GetRGBBitCount));
            int bps = (int)(header.Width * bpp * PixelFormatToBpc(pixelFormat));
            int sizeofplane = (int)(bps * header.Height);
            int width = (int)header.Width;
            int height = (int)header.Height;
            int depth = (int)header.Depth;

            byte[] rawData = new byte[depth * sizeofplane + height * bps + width * bpp];

            uint valMask = (uint)((header.PixelFormat.GetRGBBitCount == 32) ? ~0 : (1 << (int)header.PixelFormat.GetRGBBitCount) - 1);
            uint pixSize = (uint)(((int)header.PixelFormat.GetRGBBitCount + 7) / 8);
            int rShift1 = 0; int rMul = 0; int rShift2 = 0;
            ComputeMaskParams(header.PixelFormat.RBitMask, ref rShift1, ref rMul, ref rShift2);
            int gShift1 = 0; int gMul = 0; int gShift2 = 0;
            ComputeMaskParams(header.PixelFormat.GBitMask, ref gShift1, ref gMul, ref gShift2);
            int bShift1 = 0; int bMul = 0; int bShift2 = 0;
            ComputeMaskParams(header.PixelFormat.BBitMask, ref bShift1, ref bMul, ref bShift2);

            int offset = 0;
            int pixnum = width * height * depth;
            int temp = 0;
            while (pixnum-- > 0)
            {
                uint px = BitConverter.ToUInt32(data, temp) & valMask;
                temp += (int)pixSize;
                uint pxc = px & header.PixelFormat.RBitMask;
                rawData[offset + 0] = (byte)(((pxc >> rShift1) * rMul) >> rShift2);
                pxc = px & header.PixelFormat.GBitMask;
                rawData[offset + 1] = (byte)(((pxc >> gShift1) * gMul) >> gShift2);
                pxc = px & header.PixelFormat.BBitMask;
                rawData[offset + 2] = (byte)(((pxc >> bShift1) * bMul) >> bShift2);
                rawData[offset + 3] = 0xff;
                offset += 4;
            }
            return rawData;
        }

        private static byte[] DecompressRGBA(DDS_Header header, byte[] data, PixelFormat pixelFormat)
        {
            // allocate bitmap
            int bpp = (int)(PixelFormatToBpp(pixelFormat, header.PixelFormat.GetRGBBitCount));
            int bps = (int)(header.Width * bpp * PixelFormatToBpc(pixelFormat));
            int sizeofplane = (int)(bps * header.Height);
            int width = (int)header.Width;
            int height = (int)header.Height;
            int depth = (int)header.Depth;

            byte[] rawData = new byte[depth * sizeofplane + height * bps + width * bpp];

            uint valMask = (uint)((header.PixelFormat.GetRGBBitCount == 32) ? ~0 : (1 << (int)header.PixelFormat.GetRGBBitCount) - 1);
            // Funny x86s, make 1 << 32 == 1
            uint pixSize = (header.PixelFormat.GetRGBBitCount + 7) / 8;
            int rShift1 = 0; int rMul = 0; int rShift2 = 0;
            ComputeMaskParams(header.PixelFormat.RBitMask, ref rShift1, ref rMul, ref rShift2);
            int gShift1 = 0; int gMul = 0; int gShift2 = 0;
            ComputeMaskParams(header.PixelFormat.GBitMask, ref gShift1, ref gMul, ref gShift2);
            int bShift1 = 0; int bMul = 0; int bShift2 = 0;
            ComputeMaskParams(header.PixelFormat.BBitMask, ref bShift1, ref bMul, ref bShift2);
            int aShift1 = 0; int aMul = 0; int aShift2 = 0;
            ComputeMaskParams(header.PixelFormat.ABitMask, ref aShift1, ref aMul, ref aShift2);

            int offset = 0;
            int pixnum = width * height * depth;
            int temp = 0;

            while (pixnum-- > 0)
            {
                uint px = BitConverter.ToUInt32(data, temp) & valMask;
                temp += (int)pixSize;
                uint pxc = px & header.PixelFormat.RBitMask;
                rawData[offset + 0] = (byte)(((pxc >> rShift1) * rMul) >> rShift2);
                pxc = px & header.PixelFormat.GBitMask;
                rawData[offset + 1] = (byte)(((pxc >> gShift1) * gMul) >> gShift2);
                pxc = px & header.PixelFormat.BBitMask;
                rawData[offset + 2] = (byte)(((pxc >> bShift1) * bMul) >> bShift2);
                pxc = px & header.PixelFormat.ABitMask;
                rawData[offset + 3] = (byte)(((pxc >> aShift1) * aMul) >> aShift2);
                offset += 4;
            }
            return rawData;
        }

        private static byte[] DecompressLum(DDS_Header header, byte[] data, PixelFormat pixelFormat)
        {
            // allocate bitmap
            int bpp = (int)(PixelFormatToBpp(pixelFormat, header.PixelFormat.GetRGBBitCount));
            int bps = (int)(header.Width * bpp * PixelFormatToBpc(pixelFormat));
            int sizeofplane = (int)(bps * header.Height);
            int width = (int)header.Width;
            int height = (int)header.Height;
            int depth = (int)header.Depth;

            byte[] rawData = new byte[depth * sizeofplane + height * bps + width * bpp];

            int lShift1 = 0; int lMul = 0; int lShift2 = 0;
            ComputeMaskParams(header.PixelFormat.RBitMask, ref lShift1, ref lMul, ref lShift2);

            int offset = 0;
            int pixnum = width * height * depth;
            int temp = 0;
            while (pixnum-- > 0)
            {
                byte px = data[temp++];
                rawData[offset + 0] = (byte)(((px >> lShift1) * lMul) >> lShift2);
                rawData[offset + 1] = (byte)(((px >> lShift1) * lMul) >> lShift2);
                rawData[offset + 2] = (byte)(((px >> lShift1) * lMul) >> lShift2);
                rawData[offset + 3] = (byte)(((px >> lShift1) * lMul) >> lShift2);
                offset += 4;
            }
            return rawData;
        }

        #endregion

        #endregion

        #region Data Types

        [StructLayout(LayoutKind.Sequential)]
        private struct Colour8888
        {
            public byte red;
            public byte green;
            public byte blue;
            public byte alpha;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct Colour565
        {
            public ushort blue; //: 5;
            public ushort green; //: 6;
            public ushort red; //: 5;
        }

        private const uint FOURCC_DXT1 = 0x31545844;
        private const uint FOURCC_DXT2 = 0x32545844;
        private const uint FOURCC_DXT3 = 0x33545844;
        private const uint FOURCC_DXT4 = 0x34545844;
        private const uint FOURCC_DXT5 = 0x35545844;
        private const uint FOURCC_ATI1 = 0x31495441;
        private const uint FOURCC_ATI2 = 0x32495441;
        private const uint FOURCC_RXGB = 0x42475852;
        private const uint FOURCC_DOLLARNULL = 0x24;
        private const uint FOURCC_oNULL = 0x6f;
        private const uint FOURCC_pNULL = 0x70;
        private const uint FOURCC_qNULL = 0x71;
        private const uint FOURCC_rNULL = 0x72;
        private const uint FOURCC_sNULL = 0x73;
        private const uint FOURCC_tNULL = 0x74;

        /// <summary>
        /// Various pixel formats/compressors used by the DDS image.
        /// </summary>
        public enum PixelFormat
        {
            /// <summary>
            /// 32-bit image, with 8-bit red, green, blue and alpha.
            /// </summary>
            RGBA,
            /// <summary>
            /// 24-bit image with 8-bit red, green, blue.
            /// </summary>
            RGB,
            /// <summary>
            /// 16-bit DXT-1 compression, 1-bit alpha.
            /// </summary>
            DXT1,
            /// <summary>
            /// DXT-2 Compression
            /// </summary>
            DXT2,
            /// <summary>
            /// DXT-3 Compression
            /// </summary>
            DXT3,
            /// <summary>
            /// DXT-4 Compression
            /// </summary>
            DXT4,
            /// <summary>
            /// DXT-5 Compression
            /// </summary>
            DXT5,
            /// <summary>
            /// 3DC Compression
            /// </summary>
            THREEDC,
            /// <summary>
            /// ATI1n Compression
            /// </summary>
            ATI1N,
            LUMINANCE,
            LUMINANCE_ALPHA,
            RXGB,
            A16B16G16R16,
            R16F,
            G16R16F,
            A16B16G16R16F,
            R32F,
            G32R32F,
            A32B32G32R32F,
            /// <summary>
            /// Unknown pixel format.
            /// </summary>
            UNKNOWN
        }

        #endregion
    }
}