using System;
using BinarySerializer;
using UnityEngine;

namespace R1Engine.Jade
{
    public class TEX_Content_JTX_PSP : BinarySerializable {
        public TEX_Content_JTX JTX { get; set; }

        public byte[] Bytes_00 { get; set; }
        public TextureFormat Format { get; set; }
        public byte[] Bytes_01 { get; set; }
        public byte WidthPower { get; set; }
        public byte HeightPower { get; set; }
        public byte[] Bytes_02 { get; set; }
        public byte[] Content { get; set; }


        public int Width => 1 << WidthPower;
        public int Height => 1 << HeightPower;

        public override void SerializeImpl(SerializerObject s) {
            uint FileSize = JTX.PS2_Size;
            if (FileSize == 0) return;
            if (JTX.Format == TEX_Content_JTX.JTX_Format.Raw32) {
                Format = TextureFormat.RGBA8888;
                WidthPower = (byte)NextPow2(JTX.Width);
                HeightPower = (byte)NextPow2(JTX.Height);
                Content = s.SerializeArray<byte>(Content, FileSize, name: nameof(Content));
            } else {
                Bytes_00 = s.SerializeArray<byte>(Bytes_00, 0xC, name: nameof(Bytes_00));
                Format = s.Serialize<TextureFormat>(Format, name: nameof(Format));
                Bytes_01 = s.SerializeArray<byte>(Bytes_01, 0xF, name: nameof(Bytes_01));
                WidthPower = s.Serialize<byte>(WidthPower, name: nameof(WidthPower));
                HeightPower = s.Serialize<byte>(HeightPower, name: nameof(HeightPower));
                Bytes_02 = s.SerializeArray<byte>(Bytes_02, 0x12, name: nameof(Bytes_02));
                Content = s.SerializeArray<byte>(Content, FileSize - 0x30, name: nameof(Content));
            }
		}

        public byte[] UnswizzledContent {
            get {
                byte[] target = Defilter(Content, 0, Content.Length);
                return target;
            }
        }

        int NextPow2(uint x) {
            int power = 1;
            int mul = 0;
            while (power < x) {
                power *= 2;
                mul++;
            }
            return mul;
        }

        byte[] Defilter(byte[] originalData, int index, int length) {
            byte[] Buf = new byte[length];
            int bpp = Format switch {
                TextureFormat.Index4 => 4,
                TextureFormat.Index8 => 8,
                TextureFormat.RGBA8888 => 32,
                _ => throw new NotImplementedException($"Unsupported JTX PSP texture format {Format}")
            };
            int w = (Width * bpp) / 8;
            int tileWidth = Math.Min(Width, 16);
            int tileHeight = Math.Min(Height, 8);
            int lineSize = Math.Min(tileWidth, (tileWidth * bpp) / 8);
            int i = 0;

            for (int y = 0; y < Height; y += tileHeight) {
                for (int x = 0; x < w; x += lineSize) {
                    for (int tileY = y; tileY < y + tileHeight; tileY++) {
                        for (int tileX = x; tileX < x + lineSize; tileX++) {
                            byte data = originalData[index + i++];

                            if (tileX >= w || tileY >= Height) {
                                continue;
                            }

                            Buf[tileY * w + tileX] = data;
                        }
                    }
                }
            }

            return Buf;
        }

        public enum TextureFormat : byte {
            RGBA5650 = 0x00,
            RGBA5551 = 0x01,
            RGBA4444 = 0x02,
            RGBA8888 = 0x03,
            Index4 = 0x04,
            Index8 = 0x05,
            Index16 = 0x06,
            Index32 = 0x07,
            DXT1 = 0x08,
            DXT3 = 0x09,
            DXT5 = 0x0A,
        }
    }
}