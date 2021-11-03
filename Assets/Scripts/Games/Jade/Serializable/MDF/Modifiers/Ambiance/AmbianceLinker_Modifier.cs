using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace Ray1Map.Jade {
	public class AmbianceLinker_Modifier : MDF_Modifier {
		public uint Version { get; set; }
		public byte Axis { get; set; }
		public TriggerFlags Flags { get; set; }
		public TriggerEffect TriggerEffectA { get; set; }
		public TriggerEffect TriggerEffectB { get; set; }
		public float TriggerEffectResetTime { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Version = s.Serialize<uint>(Version, name: nameof(Version));
			Axis = s.Serialize<byte>(Axis, name: nameof(Axis));
			if (Version >= 2) {
				Flags = s.Serialize<TriggerFlags>(Flags, name: nameof(Flags));
				TriggerEffectA = s.SerializeObject<TriggerEffect>(TriggerEffectA, onPreSerialize: t => t.Modifier = this, name: nameof(TriggerEffectA));
				TriggerEffectB = s.SerializeObject<TriggerEffect>(TriggerEffectB, onPreSerialize: t => t.Modifier = this, name: nameof(TriggerEffectB));
				TriggerEffectResetTime = s.Serialize<float>(TriggerEffectResetTime, name: nameof(TriggerEffectResetTime));
			}
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
