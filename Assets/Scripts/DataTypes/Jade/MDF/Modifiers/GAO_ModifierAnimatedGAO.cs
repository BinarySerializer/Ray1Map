using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace R1Engine.Jade {
	public class GAO_ModifierAnimatedGAO : MDF_Modifier {
		public uint Version { get; set; }
		public AnimatedGAOFlags Flags { get; set; }
		public Jade_Reference<OBJ_GameObject> GameObject { get; set; }
		public Jade_Vector AnimatedGAOOffset { get; set; }
		public Parameters[] Params { get; set; }


		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			Version = s.Serialize<uint>(Version, name: nameof(Version));
			Flags = s.Serialize<AnimatedGAOFlags>(Flags, name: nameof(Flags));
			if (Version >= 2) {
				GameObject = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(GameObject, name: nameof(GameObject))?.Resolve();
				AnimatedGAOOffset = s.SerializeObject<Jade_Vector>(AnimatedGAOOffset, name: nameof(AnimatedGAOOffset));
			}
			if(Params == null) Params = new Parameters[9];
			for (int i = 0; i < Params.Length; i++) {
				if (BitHelpers.ExtractBits((int)Flags, 1, (i + i / 3 + 1)) == 1) {
					Params[i] = s.SerializeObject<Parameters>(Params[i], onPreSerialize: e => {
						e.Modifier = this;
					}, name: $"{nameof(Params)}[{i}]");
				}
			}
		}

		public class Parameters : BinarySerializable {
			public GAO_ModifierAnimatedGAO Modifier { get; set; }

			public ParamsType Type { get; set; }

			public LinearParams Linear { get; set; }
			public NoiseParams Noise { get; set; }
			public SinusParams Sinus { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Type = s.Serialize<ParamsType>(Type, name: nameof(Type));
				switch (Type) {
					case ParamsType.Linear:
						Linear = s.SerializeObject<LinearParams>(Linear, name: nameof(Linear));
						break;
					case ParamsType.Noise:
						Noise = s.SerializeObject<NoiseParams>(Noise, name: nameof(Noise));
						break;
					case ParamsType.Sinus:
						Sinus = s.SerializeObject<SinusParams>(Sinus, onPreSerialize: sin => sin.Params = this, name: nameof(Sinus));
						break;
				}
			}

			public enum ParamsType : uint {
				Linear = 0,
				Noise = 1,
				Sinus = 2,
				NumberOfTypes = 3,
				Align = 0xFFFFFFFF
			}

			public class LinearParams : BinarySerializable {
				public float Min { get; set; }
				public float Max { get; set; }
				public float StartTime { get; set; }
				public float StopTime { get; set; }
				public float TotalTime { get; set; }
				public uint BackAndForth { get; set; } // bool

				public override void SerializeImpl(SerializerObject s) {
					Min = s.Serialize<float>(Min, name: nameof(Min));
					Max = s.Serialize<float>(Max, name: nameof(Max));
					StartTime = s.Serialize<float>(StartTime, name: nameof(StartTime));
					StopTime = s.Serialize<float>(StopTime, name: nameof(StopTime));
					TotalTime = s.Serialize<float>(TotalTime, name: nameof(TotalTime));
					BackAndForth = s.Serialize<uint>(BackAndForth, name: nameof(BackAndForth));
				}
			}

			public class NoiseParams : BinarySerializable {
				public float Min { get; set; }
				public float Max { get; set; }
				public float Var { get; set; }
				public float Speed { get; set; }

				public override void SerializeImpl(SerializerObject s) {
					Min = s.Serialize<float>(Min, name: nameof(Min));
					Max = s.Serialize<float>(Max, name: nameof(Max));
					Var = s.Serialize<float>(Var, name: nameof(Var));
					Speed = s.Serialize<float>(Speed, name: nameof(Speed));
				}
			}

			public class SinusParams : BinarySerializable {
				public Parameters Params { get; set; }

				public float Angle { get; set; }
				public float TotalTime { get; set; }
				public float TimeBias { get; set; }
				public uint BackAndForth { get; set; }

				public override void SerializeImpl(SerializerObject s) {
					Angle = s.Serialize<float>(Angle, name: nameof(Angle));
					TotalTime = s.Serialize<float>(TotalTime, name: nameof(TotalTime));
					if (Params.Modifier.Version > 0) TimeBias = s.Serialize<float>(TimeBias, name: nameof(TimeBias));
					BackAndForth = s.Serialize<uint>(BackAndForth, name: nameof(BackAndForth));
				}
			}
		}

		[Flags]
		public enum AnimatedGAOFlags : uint {
			None = 0,
			Flag0 = 1 << 0,
			HasParam0 = 1 << 1,
			HasParam1 = 1 << 2,
			HasParam2 = 1 << 3,
			Flag4 = 1 << 4,
			HasParam3 = 1 << 5,
			HasParam4 = 1 << 6,
			HasParam5 = 1 << 7,
			Flag8 = 1 << 8,
			HasParam6 = 1 << 9,
			HasParam7 = 1 << 10,
			HasParam8 = 1 << 11,
			Flag12 = 1 << 12,
			Flag13 = 1 << 13,
			Flag14 = 1 << 14,
			Flag15 = 1 << 15,
			Flag16 = 1 << 16,
			UsePositionOffset = 1 << 17,
			UseReferenceGAO = 1 << 18,
			SyncWithGameTime = 1 << 19,
			ApplyToScaleZ = 1 << 20,
			ApplyToScaleY = 1 << 21,
			ApplyToScaleX = 1 << 22,
			ApplyToScale = 1 << 23,
			ApplyToTranslationZ = 1 << 24,
			ApplyToTranslationY = 1 << 25,
			ApplyToTranslationX = 1 << 26,
			ApplyToTranslation = 1 << 27,
			ApplyToRotationZ = 1 << 28,
			ApplyToRotationY = 1 << 29,
			ApplyToRotationX = 1 << 30,
			ApplyToRotation = (uint)1 << 31,
		}
	}
}
