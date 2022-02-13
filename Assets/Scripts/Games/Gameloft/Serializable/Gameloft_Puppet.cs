using System;
using System.Linq;
using BinarySerializer;

namespace Ray1Map.Gameloft
{
	public class Gameloft_Puppet : Gameloft_Resource {
		public byte[] Header { get; set; }
		public ushort ImagesCount { get; set; }
		public ImageDescriptor[] ImageDescriptors { get; set; }
		public ushort LayersCount { get; set; }
		public AnimationLayer[] Layers { get; set; }
		public ushort LayerGroupsCount { get; set; }
		public AnimationLayerGroupGraphics[] LayerGroupsGraphics { get; set; }
		public AnimationLayerGroupCollision[] LayerGroupsCollision { get; set; }
		public ushort FramesCount { get; set; }
		public AnimationFrame[] Frames { get; set; }
		public ushort AnimationsCount { get; set; }
		public Animation[] Animations { get; set; }

		public ushort ColorFormat { get; set; }
		public int PalettesCount { get; set; }
		public PaletteBlock[] Palettes { get; set; }
		public ushort ImageFormat { get; set; }
		public Image[] Images { get; set; }

		public bool UseOtherPuppetImageData { get; set; } // Set in onPreSerialize
		public ushort ImageDataLength { get; set; }
		public ImageBlock ImageData { get; set; }

		public bool HasMultiplePalettes => (Header[2] & 0x4) != 0;
		public bool HasImages => (Header[2] & 0x2) != 0;
		public bool HasImageBuffer => (Header[5] & 0x20) != 0;

		public static bool UseImageData(GameSettings s) {
			return s.GameModeSelection == GameModeSelection.RaymanRavingRabbidsMobile_128x128_s40v2 ||
				s.GameModeSelection == GameModeSelection.RaymanRavingRabbidsMobile_128x160_s40v2a ||
				s.GameModeSelection == GameModeSelection.RaymanRavingRabbidsMobile_128x160_SamsungX660 || 
				s.GameModeSelection == GameModeSelection.RaymanRavingRabbidsMobile_128x160_SamsungE360;
		}

		public override void SerializeImpl(SerializerObject s) {
			Header = s.SerializeArray<byte>(Header, 6, name: nameof(Header));
			ImagesCount = s.Serialize<ushort>(ImagesCount, name: nameof(ImagesCount));
			ImageDescriptors = s.SerializeObjectArray<ImageDescriptor>(ImageDescriptors, ImagesCount, onPreSerialize: id => {
				id.HasPalette = HasMultiplePalettes;
				id.HasImageOffset = HasImages;
			}, name: nameof(ImageDescriptors));
			PalettesCount = ImageDescriptors.Max(id => id.Palette) + 1;
			LayersCount = s.Serialize<ushort>(LayersCount, name: nameof(LayersCount));
			Layers = s.SerializeObjectArray<AnimationLayer>(Layers, LayersCount, name: nameof(Layers));
			LayerGroupsCount = s.Serialize<ushort>(LayerGroupsCount, name: nameof(LayerGroupsCount));
			LayerGroupsGraphics = s.SerializeObjectArray<AnimationLayerGroupGraphics>(LayerGroupsGraphics, LayerGroupsCount, name: nameof(LayerGroupsGraphics));
			if (s.GetR1Settings().GameModeSelection != GameModeSelection.RaymanKartMobile_128x128) {
				LayerGroupsCollision = s.SerializeObjectArray<AnimationLayerGroupCollision>(LayerGroupsCollision, LayerGroupsCount, name: nameof(LayerGroupsCollision));
			}
			FramesCount = s.Serialize<ushort>(FramesCount, name: nameof(FramesCount));
			Frames = s.SerializeObjectArray<AnimationFrame>(Frames, FramesCount, name: nameof(Frames));
			AnimationsCount = s.Serialize<ushort>(AnimationsCount, name: nameof(AnimationsCount));
			Animations = s.SerializeObjectArray<Animation>(Animations, AnimationsCount, name: nameof(Animations));

			if(s.CurrentPointer.AbsoluteOffset >= (Offset + ResourceSize).AbsoluteOffset) return;

			if (ImagesCount > 0) {
				if (UseImageData(s.GetR1Settings())) {
					ImageDataLength = s.Serialize<ushort>(ImageDataLength, name: nameof(ImageDataLength));
					if(ImageDataLength > 0) {
						ImageData = s.SerializeObject<ImageBlock>(ImageData, name: nameof(ImageData));
					}
				} else {
					ColorFormat = s.Serialize<ushort>(ColorFormat, name: nameof(ColorFormat));
					Palettes = s.SerializeObjectArray<PaletteBlock>(Palettes, PalettesCount, onPreSerialize: pb => pb.ColorFormat = ColorFormat, name: nameof(Palettes));
					ImageFormat = s.Serialize<ushort>(ImageFormat, name: nameof(ImageFormat));
					Images = s.SerializeObjectArray<Image>(Images, ImagesCount, name: nameof(Images));
				}
			}

		}

		public class PaletteBlock : BinarySerializable {
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

		public class ImageDescriptor : BinarySerializable {
			public bool HasPalette { get; set; }
			public bool HasImageOffset { get; set; }

			public byte Palette { get; set; }
			public byte XPosition { get; set; }
			public byte YPosition { get; set; }
			public byte Width { get; set; }
			public byte Height { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				if (HasPalette) {
					Palette = s.Serialize<byte>(Palette, name: nameof(Palette));
				}
				if (UseImageData(s.GetR1Settings()) && HasImageOffset) {
					XPosition = s.Serialize<byte>(XPosition, name: nameof(XPosition));
					YPosition = s.Serialize<byte>(YPosition, name: nameof(YPosition));
				}
				Width = s.Serialize<byte>(Width, name: nameof(Width));
				Height = s.Serialize<byte>(Height, name: nameof(Height));
			}
		}
		public class AnimationLayer : BinarySerializable {
			public byte ImageIndex { get; set; }
			public sbyte XPosition { get; set; }
			public sbyte YPosition { get; set; }
			public Flag Flags { get; set; } // 0 = flip hor, 1 = flip ver

			[Flags]
			public enum Flag : byte {
				HorizontalFlip = 1 << 0,
				VerticalFlip = 1 << 1,
				Bit2 = 1 << 2,
				Bit3 = 1 << 3,
				Bit4 = 1 << 4,
				Bit5 = 1 << 5,
				Bit6 = 1 << 6,
				Bit7 = 1 << 7,
			}


			public override void SerializeImpl(SerializerObject s) {
				ImageIndex = s.Serialize<byte>(ImageIndex, name: nameof(ImageIndex));
				XPosition = s.Serialize<sbyte>(XPosition, name: nameof(XPosition));
				YPosition = s.Serialize<sbyte>(YPosition, name: nameof(YPosition));
				Flags = s.Serialize<Flag>(Flags, name: nameof(Flags));
			}
		}
		public class AnimationLayerGroupGraphics : BinarySerializable {
			public byte Length { get; set; }
			public byte LayerGroupUnusedByte { get; set; }
			public ushort StartIndex { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Length = s.Serialize<byte>(Length, name: nameof(Length));
				if (s.GetR1Settings().GameModeSelection != GameModeSelection.RaymanKartMobile_128x128) {
					LayerGroupUnusedByte = s.Serialize<byte>(LayerGroupUnusedByte, name: nameof(LayerGroupUnusedByte));
				}
				StartIndex = s.Serialize<ushort>(StartIndex, name: nameof(StartIndex));
			}
		}
		public class AnimationLayerGroupCollision : BinarySerializable {
			public sbyte XPosition { get; set; }
			public sbyte YPosition { get; set; }
			public byte Width { get; set; }
			public byte Height { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				XPosition = s.Serialize<sbyte>(XPosition, name: nameof(XPosition));
				YPosition = s.Serialize<sbyte>(YPosition, name: nameof(YPosition));
				Width = s.Serialize<byte>(Width, name: nameof(Width));
				Height = s.Serialize<byte>(Height, name: nameof(Height));
			}
		}
		public class AnimationFrame : BinarySerializable {
			public byte LayerGroupIndex { get; set; }
			public byte Duration { get; set; }
			public sbyte XPosition { get; set; }
			public sbyte YPosition { get; set; }
			public Flag Flags { get; set; } // 0 = flip hor, 1 = flip ver

			[Flags]
			public enum Flag : byte {
				HorizontalFlip = 1 << 0,
				VerticalFlip = 1 << 1,
				Bit2 = 1 << 2,
				Bit3 = 1 << 3,
				Bit4 = 1 << 4,
				Bit5 = 1 << 5,
				Bit6 = 1 << 6,
				Bit7 = 1 << 7,
			}

			public override void SerializeImpl(SerializerObject s) {
				LayerGroupIndex = s.Serialize<byte>(LayerGroupIndex, name: nameof(LayerGroupIndex));
				Duration = s.Serialize<byte>(Duration, name: nameof(Duration));
				XPosition = s.Serialize<sbyte>(XPosition, name: nameof(XPosition));
				YPosition = s.Serialize<sbyte>(YPosition, name: nameof(YPosition));
				Flags = s.Serialize<Flag>(Flags, name: nameof(Flags));
			}
		}
		public class Animation : BinarySerializable {
			public byte Length { get; set; }
			public byte AnimationUnusedByte { get; set; }
			public ushort FrameIndex { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Length = s.Serialize<byte>(Length, name: nameof(Length));
				if (s.GetR1Settings().EngineVersion >= EngineVersion.Gameloft_RK
					&& s.GetR1Settings().GameModeSelection != GameModeSelection.RaymanKartMobile_128x128) {
					AnimationUnusedByte = s.Serialize<byte>(AnimationUnusedByte, name: nameof(AnimationUnusedByte));
				}
				FrameIndex = s.Serialize<ushort>(FrameIndex, name: nameof(FrameIndex));
			}
		}
		public class Image : BinarySerializable {
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


		public class ImageBlock : BinarySerializable {
			public byte PaletteLength { get; set; }
			public int Width { get; set; }
			public int Height { get; set; }
			public byte BitsPerPixel { get; set; }
			public byte Unknown2 { get; set; }
			public RGB888Color[] Palette { get; set; }
			public byte[] Data { get; set; }
			public byte[] UnknownData { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				s.DoEndian(Endian.Big, () => {
					s.DoBits<uint>(b => {
						PaletteLength = (byte)b.SerializeBits<int>(PaletteLength, 8, name: nameof(PaletteLength));
						Height = b.SerializeBits<int>(Height, 9, name: nameof(Height));
						Width = b.SerializeBits<int>(Width, 9, name: nameof(Width));
						BitsPerPixel = (byte)b.SerializeBits<int>(BitsPerPixel, 4, name: nameof(BitsPerPixel));
						Unknown2 = (byte)b.SerializeBits<int>(Unknown2, 2, name: nameof(Unknown2));
					});
				});
				Palette = s.SerializeObjectArray<RGB888Color>(Palette, PaletteLength + 1, name: nameof(Palette));
				int bytesPerRow = (Width*BitsPerPixel+7)/8;
				Data = s.SerializeArray<byte>(Data, (bytesPerRow+1) * (Height), name: nameof(Data));
				UnknownData = s.SerializeArray<byte>(UnknownData, 16, name: nameof(UnknownData));
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
							int numColors = paletteLength - 1;
							int colorBits = 0;
							while (numColors != 0) {
								numColors >>= 1;
								colorBits++;
							}
							while (outPos < outBuf.Length) {
								byte color = (byte)BitHelpers.ExtractBits(Data[inPos], colorBits, 0);
								int repeatCount = BitHelpers.ExtractBits(Data[inPos], 8 - colorBits, colorBits);
								inPos++;
								for (int i = 0; i < repeatCount + 1; i++) {
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