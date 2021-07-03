using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace R1Engine.Jade {
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

			public Ambiance_Modifier.FilterFlags Filters { get; set; }
			public float DeltaPos { get; set; }
			public float FadeInTime { get; set; }
			public float FadeOutTime { get; set; }
			public float LifeTime { get; set; }
			public Ambiance_Modifier.Ambiance_Filter_Glow Glow { get; set; }
			public Ambiance_Modifier.Ambiance_Filter_Contrast Contrast { get; set; }
			public Ambiance_Modifier.Ambiance_Filter_CBalance CBalance { get; set; }
			public Ambiance_Modifier.Ambiance_Filter_Brightness Brightness { get; set; }
			public Ambiance_Modifier.Ambiance_Filter_Shadow Shadow { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				if (Modifier.Version < 3) {
					Filters = (Ambiance_Modifier.FilterFlags)s.Serialize<byte>((byte)Filters, name: nameof(Filters));
				} else {
					Filters = s.Serialize<Ambiance_Modifier.FilterFlags>(Filters, name: nameof(Filters));
				}
				DeltaPos = s.Serialize<float>(DeltaPos, name: nameof(DeltaPos));
				FadeInTime = s.Serialize<float>(FadeInTime, name: nameof(FadeInTime));
				FadeOutTime = s.Serialize<float>(FadeOutTime, name: nameof(FadeOutTime));
				LifeTime = s.Serialize<float>(LifeTime, name: nameof(LifeTime));
				if (Filters.HasFlag(Ambiance_Modifier.FilterFlags.Glow)) Glow = s.SerializeObject<Ambiance_Modifier.Ambiance_Filter_Glow>(Glow, name: nameof(Glow));
				if (Filters.HasFlag(Ambiance_Modifier.FilterFlags.Contrast)) Contrast = s.SerializeObject<Ambiance_Modifier.Ambiance_Filter_Contrast>(Contrast, name: nameof(Contrast));
				if (Modifier.Version >= 4 && Filters.HasFlag(Ambiance_Modifier.FilterFlags.CBalance)) CBalance = s.SerializeObject<Ambiance_Modifier.Ambiance_Filter_CBalance>(CBalance, name: nameof(CBalance));
				if (Filters.HasFlag(Ambiance_Modifier.FilterFlags.Brightness)) Brightness = s.SerializeObject<Ambiance_Modifier.Ambiance_Filter_Brightness>(Brightness, name: nameof(Brightness));
				if (Modifier.Version >= 5 && Filters.HasFlag(Ambiance_Modifier.FilterFlags.Shadow)) Shadow = s.SerializeObject<Ambiance_Modifier.Ambiance_Filter_Shadow>(Shadow, name: nameof(Shadow));

			}
		}
	}
}
