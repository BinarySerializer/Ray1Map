using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace R1Engine.Jade {
	public class Ambiance_Modifier : MDF_Modifier {
		public uint Version { get; set; }
		public FilterFlags Filters { get; set; }
		public uint UInt_00 { get; set; }
		public Ambiance_Filter_ColorBurn ColorBurn { get; set; }
		public Ambiance_Filter_Highlight Highlight { get; set; }
		public Ambiance_Filter_BlackWhite BlackWhite { get; set; }
		public Ambiance_Filter_Glow Glow { get; set; }
		public Ambiance_Filter_Monochromatic Monochromatic { get; set; }
		public Ambiance_Filter_Colorize Colorize { get; set; }
		public Ambiance_Filter_Overlay Overlay { get; set; }
		public Ambiance_Filter_Contrast Contrast { get; set; }
		public Ambiance_Filter_CBalance CBalance { get; set; }
		public Ambiance_Filter_Brightness Brightness { get; set; }
		public Ambiance_Filter_SpiralBlur SpiralBlur { get; set; }
		public Ambiance_Filter_Shadow Shadow { get; set; }
		public uint Ambient { get; set; }
		public uint LinkersCount { get; set; }
		public Jade_Reference<OBJ_GameObject>[] Linkers { get; set; }
		public Ambiance_Filter_Glow2 Glow2 { get; set; }
		public Ambiance_Filter_DOF DOF { get; set; }
		public Ambiance_Filter_Distortion Distortion { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			Version = s.Serialize<uint>(Version, name: nameof(Version));
			if (Version == 0) {
				Filters = (FilterFlags)s.Serialize<byte>((byte)Filters, name: nameof(Filters));
			} else {
				Filters = s.Serialize<FilterFlags>(Filters, name: nameof(Filters));
			}
			if (Filters.HasFlag(FilterFlags.ColorBurn)) ColorBurn = s.SerializeObject<Ambiance_Filter_ColorBurn>(ColorBurn, name: nameof(ColorBurn));
			if (Filters.HasFlag(FilterFlags.Highlight)) Highlight = s.SerializeObject<Ambiance_Filter_Highlight>(Highlight, name: nameof(Highlight));
			if (Filters.HasFlag(FilterFlags.BlackWhite)) BlackWhite = s.SerializeObject<Ambiance_Filter_BlackWhite>(BlackWhite, name: nameof(BlackWhite));
			if (Filters.HasFlag(FilterFlags.Glow)) Glow = s.SerializeObject<Ambiance_Filter_Glow>(Glow, name: nameof(Glow));
			if (Filters.HasFlag(FilterFlags.Monochromatic)) Monochromatic = s.SerializeObject<Ambiance_Filter_Monochromatic>(Monochromatic, name: nameof(Monochromatic));
			if (Filters.HasFlag(FilterFlags.Colorize)) Colorize = s.SerializeObject<Ambiance_Filter_Colorize>(Colorize, name: nameof(Colorize));
			if (Filters.HasFlag(FilterFlags.Overlay)) Overlay = s.SerializeObject<Ambiance_Filter_Overlay>(Overlay, onPreSerialize: f => f.Ambiance = this, name: nameof(Overlay));
			if (Filters.HasFlag(FilterFlags.Contrast)) Contrast = s.SerializeObject<Ambiance_Filter_Contrast>(Contrast, name: nameof(Contrast));
			if (Version >= 4 && Filters.HasFlag(FilterFlags.CBalance)) CBalance = s.SerializeObject<Ambiance_Filter_CBalance>(CBalance, name: nameof(CBalance));
			if (Filters.HasFlag(FilterFlags.Brightness)) Brightness = s.SerializeObject<Ambiance_Filter_Brightness>(Brightness, name: nameof(Brightness));
			if (Version >= 3 && Filters.HasFlag(FilterFlags.SpiralBlur)) SpiralBlur = s.SerializeObject<Ambiance_Filter_SpiralBlur>(SpiralBlur, name: nameof(SpiralBlur));
			if (Version >= 6 && Filters.HasFlag(FilterFlags.Shadow)) Shadow = s.SerializeObject<Ambiance_Filter_Shadow>(Shadow, name: nameof(Shadow));
			if (Version < 2) Ambient = s.Serialize<uint>(Ambient, name: nameof(Ambient));
			LinkersCount = s.Serialize<uint>(LinkersCount, name: nameof(LinkersCount));
			Linkers = s.SerializeObjectArray<Jade_Reference<OBJ_GameObject>>(Linkers, LinkersCount, name: nameof(Linkers))?.Resolve();
			if (Version >= 7 && Filters.HasFlag(FilterFlags.Glow2)) Glow2 = s.SerializeObject<Ambiance_Filter_Glow2>(Glow2, name: nameof(Glow2));
			if (Version >= 8 && Filters.HasFlag(FilterFlags.DOF)) DOF = s.SerializeObject<Ambiance_Filter_DOF>(DOF, name: nameof(DOF));
			if (Version >= 9 && Filters.HasFlag(FilterFlags.Distortion)) Distortion = s.SerializeObject<Ambiance_Filter_Distortion>(Distortion, name: nameof(Distortion));
		}

		[Flags]
		public enum FilterFlags : ushort {
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

		public class Ambiance_Filter_ColorBurn : BinarySerializable {
			public Jade_Color Color { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				Color = s.SerializeObject<Jade_Color>(Color, name: nameof(Color));
			}
		}
		public class Ambiance_Filter_Highlight : BinarySerializable {
			public Jade_Color Color { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				Color = s.SerializeObject<Jade_Color>(Color, name: nameof(Color));
			}
		}
		public class Ambiance_Filter_BlackWhite : BinarySerializable {
			public float Factor { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				Factor = s.Serialize<float>(Factor, name: nameof(Factor));
			}
		}
		public class Ambiance_Filter_Glow : BinarySerializable {
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
		public class Ambiance_Filter_Monochromatic : BinarySerializable {
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
		public class Ambiance_Filter_Colorize : BinarySerializable {
			public Jade_Color Color { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				Color = s.SerializeObject<Jade_Color>(Color, name: nameof(Color));
			}
		}
		public class Ambiance_Filter_Overlay : BinarySerializable {
			public Ambiance_Modifier Ambiance { get; set; }

			public Jade_Color Color1 { get; set; }
			public Jade_Color Color2 { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				Color1 = s.SerializeObject<Jade_Color>(Color1, name: nameof(Color1));
				if (Ambiance.Version >= 5) {
					Color2 = s.SerializeObject<Jade_Color>(Color2, name: nameof(Color2));
				} else Color2 = Color1;
			}
		}
		public class Ambiance_Filter_Contrast : BinarySerializable {
			public float Factor { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				Factor = s.Serialize<float>(Factor, name: nameof(Factor));
			}
		}
		public class Ambiance_Filter_CBalance : BinarySerializable {
			public float Intensity { get; set; }
			public float Spectre { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				Intensity = s.Serialize<float>(Intensity, name: nameof(Intensity));
				Spectre = s.Serialize<float>(Spectre, name: nameof(Spectre));
			}
		}
		public class Ambiance_Filter_Brightness : BinarySerializable {
			public float Factor { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				Factor = s.Serialize<float>(Factor, name: nameof(Factor));
			}
		}
		public class Ambiance_Filter_SpiralBlur : BinarySerializable {
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
				if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PoP_T2T)) {
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
		public class Ambiance_Filter_Shadow : BinarySerializable {
			public float Intensity { get; set; }
			public float FadeOutStart { get; set; }
			public float FadeOutLength { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				Intensity = s.Serialize<float>(Intensity, name: nameof(Intensity));
				FadeOutStart = s.Serialize<float>(FadeOutStart, name: nameof(FadeOutStart));
				FadeOutLength = s.Serialize<float>(FadeOutLength, name: nameof(FadeOutLength));
			}
		}
		public class Ambiance_Filter_Glow2 : BinarySerializable {
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
		public class Ambiance_Filter_DOF : BinarySerializable {
			public float FocusDistance { get; set; }
			public float BlurStartDistance { get; set; }
			public float BlurEndDistance { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				FocusDistance = s.Serialize<float>(FocusDistance, name: nameof(FocusDistance));
				BlurStartDistance = s.Serialize<float>(BlurStartDistance, name: nameof(BlurStartDistance));
				BlurEndDistance = s.Serialize<float>(BlurEndDistance, name: nameof(BlurEndDistance));
			}
		}
		public class Ambiance_Filter_Distortion : BinarySerializable {
			public float Scale { get; set; }
			public float Attenuation { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				Scale = s.Serialize<float>(Scale, name: nameof(Scale));
				Attenuation = s.Serialize<float>(Attenuation, name: nameof(Attenuation));
			}
		}
	}
}
