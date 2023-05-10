using System;

namespace BinarySerializer.PSP
{
    public class GE_Texture : BinarySerializable
    {
        public GE_Command_TextureBufferWidth Pre_TBW { get; set; }
        public GE_Command_TextureSize Pre_TSIZE { get; set; }
        public GE_PixelStorageMode Pre_Format { get; set; }
        public bool Pre_IsSwizzled { get; set; }
		public bool Pre_UseLineSizeLimit { get; set; }

		public int Width => 1 << Pre_TSIZE.TW;
		public int Height => 1 << Pre_TSIZE.TH;

		public byte[] Data { get; set; }

		public override void SerializeImpl(SerializerObject s)
        {
			int bpp = Pre_Format switch {
				GE_PixelStorageMode.Index4 => 4,
				GE_PixelStorageMode.Index8 => 8,
				GE_PixelStorageMode.RGBA8888 => 32,
				_ => throw new BinarySerializableException(this, $"Unsupported PSP texture format {Pre_Format}")
			};
			int w = (Width * bpp) / 8;

			Data = s.SerializeArray<byte>(Data, Height * w, name: nameof(Data));
		}


		public byte[] UnswizzledContent {
			get {
				if(!Pre_IsSwizzled) return Data;
				byte[] target = Defilter(Data, 0, Data.Length);
				return target;
			}
		}

		byte[] Defilter(byte[] originalData, int index, int length) {
			byte[] Buf = new byte[length];
			int bpp = Pre_Format switch {
				GE_PixelStorageMode.Index4 => 4,
				GE_PixelStorageMode.Index8 => 8,
				GE_PixelStorageMode.RGBA8888 => 32,
				_ => throw new NotImplementedException($"Unsupported PSP texture format {Pre_Format}")
			};
			int w = (Width * bpp) / 8;
			int tileWidth = Math.Min(Width, 16);
			int tileHeight = Math.Min(Height, 8);
			int lineSize = Math.Min(tileWidth, (tileWidth * bpp) / 8);

			if (bpp == 4 && Pre_UseLineSizeLimit) {
				if (Width > 16) lineSize = tileWidth;
			}
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
	}
}