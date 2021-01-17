using System.Linq;

namespace R1Engine
{
	public class Gameloft_Graphics : Gameloft_Resource {
		public byte[] Header { get; set; }
		public ushort ImagesCount { get; set; }
		public ImageDescriptor[] ImageDescriptors { get; set; }
		public ushort Count2 { get; set; }
		public Struct2[] Structs2 { get; set; }
		public ushort Count3 { get; set; }
		public Struct3[] Structs3 { get; set; }
		public Struct3_2[] Structs3_2 { get; set; }
		public ushort Count4 { get; set; }
		public Struct4[] Structs4 { get; set; }
		public ushort Count5 { get; set; }
		public Struct5[] Structs5 { get; set; }

		public ushort ColorFormat { get; set; }
		public int PaletteCount { get; set; }
		public PaletteBlock[] Palettes { get; set; }
		public ushort ImageFormat { get; set; }
		public Image[] Images { get; set; }

		public bool HasMultiplePalettes => (Header[2] & 0x4) != 0;

		public override void SerializeImpl(SerializerObject s) {
			Header = s.SerializeArray<byte>(Header, 6, name: nameof(Header));
			ImagesCount = s.Serialize<ushort>(ImagesCount, name: nameof(ImagesCount));
			ImageDescriptors = s.SerializeObjectArray<ImageDescriptor>(ImageDescriptors, ImagesCount, onPreSerialize: id => id.HasPalette = HasMultiplePalettes, name: nameof(ImageDescriptors));
			PaletteCount = ImageDescriptors.Max(id => id.Palette) + 1;
			Count2 = s.Serialize<ushort>(Count2, name: nameof(Count2));
			Structs2 = s.SerializeObjectArray<Struct2>(Structs2, Count2, name: nameof(Structs2));
			Count3 = s.Serialize<ushort>(Count3, name: nameof(Count3));
			Structs3 = s.SerializeObjectArray<Struct3>(Structs3, Count3, name: nameof(Structs3));
			Structs3_2 = s.SerializeObjectArray<Struct3_2>(Structs3_2, Count3, name: nameof(Structs3_2));
			Count4 = s.Serialize<ushort>(Count4, name: nameof(Count4));
			Structs4 = s.SerializeObjectArray<Struct4>(Structs4, Count4, name: nameof(Structs4));
			Count5 = s.Serialize<ushort>(Count5, name: nameof(Count5));
			Structs5 = s.SerializeObjectArray<Struct5>(Structs5, Count5, name: nameof(Structs5));

			if (ImagesCount > 0) {
				ColorFormat = s.Serialize<ushort>(ColorFormat, name: nameof(ColorFormat));
				Palettes = s.SerializeObjectArray<PaletteBlock>(Palettes, PaletteCount, onPreSerialize: pb => pb.ColorFormat = ColorFormat, name: nameof(Palettes));
				ImageFormat = s.Serialize<ushort>(ImageFormat, name: nameof(ImageFormat));
				Images = s.SerializeObjectArray<Image>(Images, ImagesCount, name: nameof(Images));
			}

		}

		public class PaletteBlock : R1Serializable {
			public byte PaletteCount { get; set; }
			public byte PaletteLength { get; set; }
			public BaseColor[][] Palettes { get; set; }

			// Set in onPreSerialize
			public ushort ColorFormat { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				PaletteCount = s.Serialize<byte>(PaletteCount, name: nameof(PaletteCount));
				PaletteLength = s.Serialize<byte>(PaletteLength, name: nameof(PaletteLength));
				if (Palettes == null) Palettes = new BaseColor[PaletteCount][];
				if (ColorFormat == 0x5515) { // 1555
					for (int i = 0; i < Palettes.Length; i++) {
						Palettes[i] = s.SerializeObjectArray<BGRA5551Color>((BGRA5551Color[])Palettes[i], PaletteLength == 0 ? 256 : PaletteLength, name: $"{nameof(Palettes)}[{i}]");
					}
				} else if (ColorFormat == 0x8888) { // 8888
					for (int i = 0; i < Palettes.Length; i++) {
						Palettes[i] = s.SerializeObjectArray<BGRA8888Color>((BGRA8888Color[])Palettes[i], PaletteLength == 0 ? 256 : PaletteLength, name: $"{nameof(Palettes)}[{i}]");
					}
				} else if (ColorFormat == 0x4444) {
					for (int i = 0; i < Palettes.Length; i++) {
						Palettes[i] = s.SerializeObjectArray<BGRA4444Color>((BGRA4444Color[])Palettes[i], PaletteLength == 0 ? 256 : PaletteLength, name: $"{nameof(Palettes)}[{i}]");
					}
				}
			}
		}

		public class ImageDescriptor : R1Serializable {
			public bool HasPalette { get; set; }
			public byte Palette { get; set; }
			public byte Width { get; set; }
			public byte Height { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				if (HasPalette) {
					Palette = s.Serialize<byte>(Palette, name: nameof(Palette));
				}
				Width = s.Serialize<byte>(Width, name: nameof(Width));
				Height = s.Serialize<byte>(Height, name: nameof(Height));
			}
		}
		public class Struct2 : R1Serializable {
			public byte[] Struct2Bytes { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Struct2Bytes = s.SerializeArray<byte>(Struct2Bytes, 4, name: nameof(Struct2Bytes));
			}
		}
		public class Struct3 : R1Serializable {
			public byte Key { get; set; }
			public byte Struct3Unused { get; set; }
			public ushort Value { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Key = s.Serialize<byte>(Key, name: nameof(Key));
				Struct3Unused = s.Serialize<byte>(Struct3Unused, name: nameof(Struct3Unused));
				Value = s.Serialize<ushort>(Value, name: nameof(Value));
			}
		}
		public class Struct3_2 : R1Serializable {
			public byte[] Struct3Bytes { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Struct3Bytes = s.SerializeArray<byte>(Struct3Bytes, 4, name: nameof(Struct3Bytes));
			}
		}
		public class Struct4 : R1Serializable {
			public byte[] Struct4Bytes { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Struct4Bytes = s.SerializeArray<byte>(Struct4Bytes, 5, name: nameof(Struct4Bytes));
			}
		}
		public class Struct5 : R1Serializable {
			public byte Key { get; set; }
			public byte Struct5Unused { get; set; }
			public ushort Value { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Key = s.Serialize<byte>(Key, name: nameof(Key));
				if (s.GameSettings.EngineVersion >= EngineVersion.Gameloft_RK) {
					Struct5Unused = s.Serialize<byte>(Struct5Unused, name: nameof(Struct5Unused));
				}
				Value = s.Serialize<ushort>(Value, name: nameof(Value));
			}
		}
		public class Image : R1Serializable {
			public ushort Length { get; set; }
			public byte[] Data { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Length = s.Serialize<ushort>(Length, name: nameof(Length));
				Data = s.SerializeArray<byte>(Data, Length, name: nameof(Data));
			}

			public byte[] Convert(ushort imageFormat, int width, int height, int paletteLength) {
				int inPos = 0;
				int outPos = 0;
				byte[] outBuf = new byte[width * height];
				while (outPos < outBuf.Length) {
					switch (imageFormat) {
						case 0x27F1:
							if (Data[inPos] >= 128) {
								int repeatCount = Data[inPos++] - 128;
								byte color = Data[inPos++];
								for (int i = 0; i < repeatCount; i++) {
									outBuf[outPos++] = color;
								}
							} else {
								outBuf[outPos++] = Data[inPos++];
							}
							break;
						case 0x1600: // 16 colors
							if (outPos < outBuf.Length) outBuf[outPos++] = (byte)BitHelpers.ExtractBits(Data[inPos], 4, 4);
							if (outPos < outBuf.Length) outBuf[outPos++] = (byte)BitHelpers.ExtractBits(Data[inPos], 4, 0);
							inPos++;
							break;
						case 0x400: // 4 colors
							if (outPos < outBuf.Length) outBuf[outPos++] = (byte)BitHelpers.ExtractBits(Data[inPos], 2, 6);
							if (outPos < outBuf.Length) outBuf[outPos++] = (byte)BitHelpers.ExtractBits(Data[inPos], 2, 4);
							if (outPos < outBuf.Length) outBuf[outPos++] = (byte)BitHelpers.ExtractBits(Data[inPos], 2, 2);
							if (outPos < outBuf.Length) outBuf[outPos++] = (byte)BitHelpers.ExtractBits(Data[inPos], 2, 0);
							inPos++;
							break;
						case 0x200: // 2 colors
							if (outPos < outBuf.Length) outBuf[outPos++] = (byte)BitHelpers.ExtractBits(Data[inPos], 1, 7);
							if (outPos < outBuf.Length) outBuf[outPos++] = (byte)BitHelpers.ExtractBits(Data[inPos], 1, 6);
							if (outPos < outBuf.Length) outBuf[outPos++] = (byte)BitHelpers.ExtractBits(Data[inPos], 1, 5);
							if (outPos < outBuf.Length) outBuf[outPos++] = (byte)BitHelpers.ExtractBits(Data[inPos], 1, 4);
							if (outPos < outBuf.Length) outBuf[outPos++] = (byte)BitHelpers.ExtractBits(Data[inPos], 1, 3);
							if (outPos < outBuf.Length) outBuf[outPos++] = (byte)BitHelpers.ExtractBits(Data[inPos], 1, 2);
							if (outPos < outBuf.Length) outBuf[outPos++] = (byte)BitHelpers.ExtractBits(Data[inPos], 1, 1);
							if (outPos < outBuf.Length) outBuf[outPos++] = (byte)BitHelpers.ExtractBits(Data[inPos], 1, 0);
							inPos++;
							break;
						case 0x5602:
							System.Array.Copy(Data, outBuf, outBuf.Length);
							outPos = outBuf.Length;
							break;
						case 0x64F0:
							int numColors = paletteLength-1;
							int colorBits = 0;
							while(numColors != 0) {
								numColors >>= 1;
								colorBits++;
							}
							while (outPos < outBuf.Length) {
								byte color = (byte)BitHelpers.ExtractBits(Data[inPos], colorBits, 0);
								int repeatCount = BitHelpers.ExtractBits(Data[inPos], 8-colorBits, colorBits);
								inPos++;
								for (int i = 0; i < repeatCount+1; i++) {
									outBuf[outPos++] = color;
								}
							}
							break;
						default:
							outPos = outBuf.Length;
							break;
					}
				}
				return outBuf;
			}
		}
	}
}