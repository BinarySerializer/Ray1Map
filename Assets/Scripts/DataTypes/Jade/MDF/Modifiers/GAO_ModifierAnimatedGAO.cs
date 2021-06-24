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
		public float TimeFactor { get; set; }
		public Parameters[] Params { get; set; }


		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			Version = s.Serialize<uint>(Version, name: nameof(Version));
			Flags = s.Serialize<AnimatedGAOFlags>(Flags, name: nameof(Flags));
			if (Version >= 2) {
				GameObject = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(GameObject, name: nameof(GameObject))?.Resolve();
				AnimatedGAOOffset = s.SerializeObject<Jade_Vector>(AnimatedGAOOffset, name: nameof(AnimatedGAOOffset));
			}
			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal) && Version >= 3)
				TimeFactor = s.Serialize<float>(TimeFactor, name: nameof(TimeFactor));
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

			public ParamsType_Montpellier Type { get; set; }
			public ParamsType_Montreal Type_Montreal { get; set; }

			public LinearParams Linear { get; set; }
			public RandomParams Random { get; set; }
			public NoiseParams Noise { get; set; }
			public SinusParams Sinus { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal)) {
					Type_Montreal = s.Serialize<ParamsType_Montreal>(Type_Montreal, name: nameof(Type_Montreal));
					switch (Type_Montreal) {
						case ParamsType_Montreal.Linear:
							Linear = s.SerializeObject<LinearParams>(Linear, name: nameof(Linear));
							break;
						case ParamsType_Montreal.Random:
							Random = s.SerializeObject<RandomParams>(Random, name: nameof(Random));
							break;
						case ParamsType_Montreal.Noise:
							Noise = s.SerializeObject<NoiseParams>(Noise, name: nameof(Noise));
							break;
						case ParamsType_Montreal.Sinus:
							Sinus = s.SerializeObject<SinusParams>(Sinus, onPreSerialize: sin => sin.Params = this, name: nameof(Sinus));
							break;
					}
				} else {
					Type = s.Serialize<ParamsType_Montpellier>(Type, name: nameof(Type));
					switch (Type) {
						case ParamsType_Montpellier.Linear:
							Linear = s.SerializeObject<LinearParams>(Linear, name: nameof(Linear));
							break;
						case ParamsType_Montpellier.Noise:
							Noise = s.SerializeObject<NoiseParams>(Noise, name: nameof(Noise));
							break;
						case ParamsType_Montpellier.Sinus:
							Sinus = s.SerializeObject<SinusParams>(Sinus, onPreSerialize: sin => sin.Params = this, name: nameof(Sinus));
							break;
					}
				}
			}

			public enum ParamsType_Montpellier : uint {
				Linear = 0,
				Noise = 1,
				Sinus = 2,
				NumberOfTypes = 3,
				Align = 0xFFFFFFFF
			}
			public enum ParamsType_Montreal : uint {
				Linear = 0,
				Random = 1,
				Noise = 2,
				Sinus = 3,
				NumberOfTypes = 4,
				Align = 0xFFFFFFFF
			}

			public class LinearParams : BinarySerializable {
				public float Min { get; set; }
				public float Max { get; set; }
				public float StartTime { get; set; }
				public float StopTime { get; set; }
				public float TotalTime { get; set; }
				public float TimeScale { get; set; }
				public uint BackAndForth { get; set; } // bool

				public override void SerializeImpl(SerializerObject s) {
					Min = s.Serialize<float>(Min, name: nameof(Min));
					Max = s.Serialize<float>(Max, name: nameof(Max));
					StartTime = s.Serialize<float>(StartTime, name: nameof(StartTime));
					StopTime = s.Serialize<float>(StopTime, name: nameof(StopTime));
					TotalTime = s.Serialize<float>(TotalTime, name: nameof(TotalTime));
					if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal)) {
						TimeScale = s.Serialize<float>(TimeScale, name: nameof(TimeScale));
					}
					BackAndForth = s.Serialize<uint>(BackAndForth, name: nameof(BackAndForth));
				}
			}


			public class RandomParams : BinarySerializable {
				public float MinA { get; set; }
				public float MinB { get; set; }
				public float MaxA { get; set; }
				public float MaxB { get; set; }
				public float StartTimeA { get; set; }
				public float StartTimeB { get; set; }
				public float StopTime { get; set; }
				public float TotalTime { get; set; }
				public float TimeScaleA { get; set; }
				public float TimeScaleB { get; set; }
				public uint BackAndForth { get; set; } // bool

				public override void SerializeImpl(SerializerObject s) {
					MinA = s.Serialize<float>(MinA, name: nameof(MinA));
					MinB = s.Serialize<float>(MinB, name: nameof(MinB));
					MaxA = s.Serialize<float>(MaxA, name: nameof(MaxA));
					MaxB = s.Serialize<float>(MaxB, name: nameof(MaxB));
					StartTimeA = s.Serialize<float>(StartTimeA, name: nameof(StartTimeA));
					StartTimeB = s.Serialize<float>(StartTimeB, name: nameof(StartTimeB));
					StopTime = s.Serialize<float>(StopTime, name: nameof(StopTime));
					TotalTime = s.Serialize<float>(TotalTime, name: nameof(TotalTime));
					TimeScaleA = s.Serialize<float>(TimeScaleA, name: nameof(TimeScaleA));
					TimeScaleB = s.Serialize<float>(TimeScaleB, name: nameof(TimeScaleB));
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
			ApplyToRotation = (uint)1 << 0,
			ApplyToRotationX = 1 << 1,
			ApplyToRotationY = 1 << 2,
			ApplyToRotationZ = 1 << 3,
			ApplyToTranslation = 1 << 4,
			ApplyToTranslationX = 1 << 5,
			ApplyToTranslationY = 1 << 6,
			ApplyToTranslationZ = 1 << 7,
			ApplyToScale = 1 << 8,
			ApplyToScaleX = 1 << 9,
			ApplyToScaleY = 1 << 10,
			ApplyToScaleZ = 1 << 11,
			SyncWithGameTime = 1 << 12,
			UseReferenceGAO = 1 << 13,
			UsePositionOffset = 1 << 14,
			UseFatherAsReferenceGAO = 1 << 15,
		}
	}
}
