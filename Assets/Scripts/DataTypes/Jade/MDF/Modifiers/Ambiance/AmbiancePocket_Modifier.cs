using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace R1Engine.Jade {
	public class AmbiancePocket_Modifier : MDF_Modifier {
		public uint Version { get; set; }
		public Ambiance_Filter.Flags Filters { get; set; }
		public float RadiusMin { get; set; }
		public float RadiusMax { get; set; }
		public Jade_Reference<OBJ_GameObject> ReferenceGAO { get; set; }

		public Ambiance_Filter.ColorBurn ColorBurn { get; set; }
		public Ambiance_Filter.Highlight Highlight { get; set; }
		public Ambiance_Filter.BlackWhite BlackWhite { get; set; }
		public Ambiance_Filter.Glow Glow { get; set; }
		public Ambiance_Filter.Monochromatic Monochromatic { get; set; }
		public Ambiance_Filter.Colorize Colorize { get; set; }
		public Ambiance_Filter.Overlay Overlay { get; set; }
		public Ambiance_Filter.Contrast Contrast { get; set; }
		public Ambiance_Filter.Brightness Brightness { get; set; }
		public Ambiance_Filter.SpiralBlur SpiralBlur { get; set; }
		public Ambiance_Filter.Shadow Shadow { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Version = s.Serialize<uint>(Version, name: nameof(Version));
			if (Version == 0) {
				Filters = (Ambiance_Filter.Flags)s.Serialize<byte>((byte)Filters, name: nameof(Filters));
			} else {
				Filters = s.Serialize<Ambiance_Filter.Flags>(Filters, name: nameof(Filters));
			}
			RadiusMin = s.Serialize<float>(RadiusMin, name: nameof(RadiusMin));
			RadiusMax = s.Serialize<float>(RadiusMax, name: nameof(RadiusMax));
			ReferenceGAO = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(ReferenceGAO, name: nameof(ReferenceGAO))?.Resolve();

			if (Filters.HasFlag(Ambiance_Filter.Flags.ColorBurn)) ColorBurn = s.SerializeObject<Ambiance_Filter.ColorBurn>(ColorBurn, name: nameof(ColorBurn));
			if (Filters.HasFlag(Ambiance_Filter.Flags.Highlight)) Highlight = s.SerializeObject<Ambiance_Filter.Highlight>(Highlight, name: nameof(Highlight));
			if (Filters.HasFlag(Ambiance_Filter.Flags.BlackWhite)) BlackWhite = s.SerializeObject<Ambiance_Filter.BlackWhite>(BlackWhite, name: nameof(BlackWhite));
			if (Filters.HasFlag(Ambiance_Filter.Flags.Glow)) Glow = s.SerializeObject<Ambiance_Filter.Glow>(Glow, name: nameof(Glow));
			if (Filters.HasFlag(Ambiance_Filter.Flags.Monochromatic)) Monochromatic = s.SerializeObject<Ambiance_Filter.Monochromatic>(Monochromatic, name: nameof(Monochromatic));
			if (Filters.HasFlag(Ambiance_Filter.Flags.Colorize)) Colorize = s.SerializeObject<Ambiance_Filter.Colorize>(Colorize, name: nameof(Colorize));
			if (Filters.HasFlag(Ambiance_Filter.Flags.Overlay)) Overlay = s.SerializeObject<Ambiance_Filter.Overlay>(Overlay, onPreSerialize: f => f.Pre_HasColor2 = Version >= 3, name: nameof(Overlay));
			if (Filters.HasFlag(Ambiance_Filter.Flags.Contrast)) Contrast = s.SerializeObject<Ambiance_Filter.Contrast>(Contrast, name: nameof(Contrast));
			if (Filters.HasFlag(Ambiance_Filter.Flags.Brightness)) Brightness = s.SerializeObject<Ambiance_Filter.Brightness>(Brightness, name: nameof(Brightness));
			if (Version >= 2 && Filters.HasFlag(Ambiance_Filter.Flags.SpiralBlur)) SpiralBlur = s.SerializeObject<Ambiance_Filter.SpiralBlur>(SpiralBlur, name: nameof(SpiralBlur));
			if (Version >= 4 && Filters.HasFlag(Ambiance_Filter.Flags.Shadow)) Shadow = s.SerializeObject<Ambiance_Filter.Shadow>(Shadow, name: nameof(Shadow));

		}

		[Flags]
		public enum TriggerFlags : byte {
			None = 0,
			TriggerEffectAPonderationWithAngle = 1 << 0,
			TriggerEffectBPonderationWithAngle = 1 << 1,
			Unknown2 = 1 << 2,
			Unknown3 = 1 << 3,
			Unknown4 = 1 << 4,
			Unknown5 = 1 << 5,
			Unknown6 = 1 << 6,
			Unknown7 = 1 << 7,
		}

		public class TriggerEffect : BinarySerializable {
			public AmbianceLinker_Modifier Modifier { get; set; }

			public Ambiance_Filter.Flags Filters { get; set; }
			public float DeltaPos { get; set; }
			public float FadeInTime { get; set; }
			public float FadeOutTime { get; set; }
			public float LifeTime { get; set; }
			public Ambiance_Filter.Glow Glow { get; set; }
			public Ambiance_Filter.Contrast Contrast { get; set; }
			public Ambiance_Filter.CBalance CBalance { get; set; }
			public Ambiance_Filter.Brightness Brightness { get; set; }
			public Ambiance_Filter.Shadow Shadow { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				if (Modifier.Version < 3) {
					Filters = (Ambiance_Filter.Flags)s.Serialize<byte>((byte)Filters, name: nameof(Filters));
				} else {
					Filters = s.Serialize<Ambiance_Filter.Flags>(Filters, name: nameof(Filters));
				}
				DeltaPos = s.Serialize<float>(DeltaPos, name: nameof(DeltaPos));
				FadeInTime = s.Serialize<float>(FadeInTime, name: nameof(FadeInTime));
				FadeOutTime = s.Serialize<float>(FadeOutTime, name: nameof(FadeOutTime));
				LifeTime = s.Serialize<float>(LifeTime, name: nameof(LifeTime));
				if (Filters.HasFlag(Ambiance_Filter.Flags.Glow)) Glow = s.SerializeObject<Ambiance_Filter.Glow>(Glow, name: nameof(Glow));
				if (Filters.HasFlag(Ambiance_Filter.Flags.Contrast)) Contrast = s.SerializeObject<Ambiance_Filter.Contrast>(Contrast, name: nameof(Contrast));
				if (Modifier.Version >= 4 && Filters.HasFlag(Ambiance_Filter.Flags.CBalance)) CBalance = s.SerializeObject<Ambiance_Filter.CBalance>(CBalance, name: nameof(CBalance));
				if (Filters.HasFlag(Ambiance_Filter.Flags.Brightness)) Brightness = s.SerializeObject<Ambiance_Filter.Brightness>(Brightness, name: nameof(Brightness));
				if (Modifier.Version >= 5 && Filters.HasFlag(Ambiance_Filter.Flags.Shadow)) Shadow = s.SerializeObject<Ambiance_Filter.Shadow>(Shadow, name: nameof(Shadow));

			}
		}
	}
}
