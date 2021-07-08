using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace R1Engine.Jade {
	public static class Ambiance_Filter {

		[Flags]
		public enum Flags : ushort {
			None = 0,
			ColorBurn = 1 << 0,
			Highlight = 1 << 1,
			Glow = 1 << 2,
			BlackWhite = 1 << 3,
			Monochromatic = 1 << 4,
			Colorize = 1 << 5,
			Overlay = 1 << 6,
			Contrast = 1 << 7,
			Brightness = 1 << 8,
			SpiralBlur = 1 << 9,
			CBalance = 1 << 10,
			Shadow = 1 << 11,
			Glow2 = 1 << 12,
			DOF = 1 << 13,
			Distortion = 1 << 14,
			Unknown15 = 1 << 15
		}

		public class ColorBurn : BinarySerializable {
			public Jade_Color Color { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				Color = s.SerializeObject<Jade_Color>(Color, name: nameof(Color));
			}
		}
		public class Highlight : BinarySerializable {
			public Jade_Color Color { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				Color = s.SerializeObject<Jade_Color>(Color, name: nameof(Color));
			}
		}
		public class BlackWhite : BinarySerializable {
			public float Factor { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				Factor = s.Serialize<float>(Factor, name: nameof(Factor));
			}
		}
		public class Glow : BinarySerializable {
			public GlowFlags Flags { get; set; }
			public uint LowerCutOff { get; set; }
			public uint ColorScale { get; set; }
			public uint ReductionFactor { get; set; }
			public uint FirstReductionPower { get; set; }
			public uint FilterBoost { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				Flags = s.Serialize<GlowFlags>(Flags, name: nameof(Flags));
				LowerCutOff = s.Serialize<uint>(LowerCutOff, name: nameof(LowerCutOff));
				ColorScale = s.Serialize<uint>(ColorScale, name: nameof(ColorScale));
				ReductionFactor = s.Serialize<uint>(ReductionFactor, name: nameof(ReductionFactor));
				FirstReductionPower = s.Serialize<uint>(FirstReductionPower, name: nameof(FirstReductionPower));
				FilterBoost = s.Serialize<uint>(FilterBoost, name: nameof(FilterBoost));
			}
			[Flags]
			public enum GlowFlags : uint {
				None = 0,
				UseReductionFilter = 1 << 0,
				BlackAndWhite = 1 << 1,
				UseAlphaMask = 1 << 2,
			}
		}
		public class Monochromatic : BinarySerializable {
			public float Amount { get; set; }
			public uint ColHue { get; set; }
			public uint ColSat { get; set; }
			public float LvlGamma { get; set; }
			public uint LvlMax { get; set; }
			public uint LvlMin { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				Amount = s.Serialize<float>(Amount, name: nameof(Amount));
				ColHue = s.Serialize<uint>(ColHue, name: nameof(ColHue));
				ColSat = s.Serialize<uint>(ColSat, name: nameof(ColSat));
				LvlGamma = s.Serialize<float>(LvlGamma, name: nameof(LvlGamma));
				LvlMax = s.Serialize<uint>(LvlMax, name: nameof(LvlMax));
				LvlMin = s.Serialize<uint>(LvlMin, name: nameof(LvlMin));
			}
		}
		public class Colorize : BinarySerializable {
			public Jade_Color Color { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				Color = s.SerializeObject<Jade_Color>(Color, name: nameof(Color));
			}
		}
		public class Overlay : BinarySerializable {
			public bool Pre_HasColor2 { get; set; }

			public Jade_Color Color1 { get; set; }
			public Jade_Color Color2 { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				Color1 = s.SerializeObject<Jade_Color>(Color1, name: nameof(Color1));
				if (Pre_HasColor2) {
					Color2 = s.SerializeObject<Jade_Color>(Color2, name: nameof(Color2));
				} else Color2 = Color1;
			}
		}
		public class Contrast : BinarySerializable {
			public float Factor { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				Factor = s.Serialize<float>(Factor, name: nameof(Factor));
			}
		}
		public class CBalance : BinarySerializable {
			public float Intensity { get; set; }
			public float Spectre { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				Intensity = s.Serialize<float>(Intensity, name: nameof(Intensity));
				Spectre = s.Serialize<float>(Spectre, name: nameof(Spectre));
			}
		}
		public class Brightness : BinarySerializable {
			public float Factor { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				Factor = s.Serialize<float>(Factor, name: nameof(Factor));
			}
		}
		public class SpiralBlur : BinarySerializable {
			public float AngleMax { get; set; }
			public float AngleOffset { get; set; }
			public float AngleOffsetSpeed { get; set; }
			public float AngleScale { get; set; }
			public float Persistence { get; set; }
			public float UVAngleDelta { get; set; }
			public float UVZoom { get; set; }
			public float XCenter { get; set; }
			public float YCenter { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				AngleMax = s.Serialize<float>(AngleMax, name: nameof(AngleMax));
				if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_TMNT)) {
					AngleOffset = s.Serialize<float>(AngleOffset, name: nameof(AngleOffset));
					AngleOffsetSpeed = s.Serialize<float>(AngleOffsetSpeed, name: nameof(AngleOffsetSpeed));
					AngleScale = s.Serialize<float>(AngleScale, name: nameof(AngleScale));
					Persistence = s.Serialize<float>(Persistence, name: nameof(Persistence));
					UVAngleDelta = s.Serialize<float>(UVAngleDelta, name: nameof(UVAngleDelta));
					UVZoom = s.Serialize<float>(UVZoom, name: nameof(UVZoom));
					XCenter = s.Serialize<float>(XCenter, name: nameof(XCenter));
					YCenter = s.Serialize<float>(YCenter, name: nameof(YCenter));
				}
			}
		}
		public class Shadow : BinarySerializable {
			public float Intensity { get; set; }
			public float FadeOutStart { get; set; }
			public float FadeOutLength { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				Intensity = s.Serialize<float>(Intensity, name: nameof(Intensity));
				FadeOutStart = s.Serialize<float>(FadeOutStart, name: nameof(FadeOutStart));
				FadeOutLength = s.Serialize<float>(FadeOutLength, name: nameof(FadeOutLength));
			}
		}
		public class Glow2 : BinarySerializable {
			public float IntensityThreshold { get; set; }
			public float OriginMultiplier { get; set; }
			public float GlowMultiplier { get; set; }
			public float AlphaThreshold { get; set; }
			public float Luminance { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				IntensityThreshold = s.Serialize<float>(IntensityThreshold, name: nameof(IntensityThreshold));
				OriginMultiplier = s.Serialize<float>(OriginMultiplier, name: nameof(OriginMultiplier));
				GlowMultiplier = s.Serialize<float>(GlowMultiplier, name: nameof(GlowMultiplier));
				AlphaThreshold = s.Serialize<float>(AlphaThreshold, name: nameof(AlphaThreshold));
				Luminance = s.Serialize<float>(Luminance, name: nameof(Luminance));
			}
		}
		public class DOF : BinarySerializable {
			public float FocusDistance { get; set; }
			public float BlurStartDistance { get; set; }
			public float BlurEndDistance { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				FocusDistance = s.Serialize<float>(FocusDistance, name: nameof(FocusDistance));
				BlurStartDistance = s.Serialize<float>(BlurStartDistance, name: nameof(BlurStartDistance));
				BlurEndDistance = s.Serialize<float>(BlurEndDistance, name: nameof(BlurEndDistance));
			}
		}
		public class Distortion : BinarySerializable {
			public float Scale { get; set; }
			public float Attenuation { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				Scale = s.Serialize<float>(Scale, name: nameof(Scale));
				Attenuation = s.Serialize<float>(Attenuation, name: nameof(Attenuation));
			}
		}
	}
}
