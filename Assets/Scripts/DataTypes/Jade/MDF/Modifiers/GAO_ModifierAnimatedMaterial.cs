using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace R1Engine.Jade {
	public class GAO_ModifierAnimatedMaterial : MDF_Modifier {
		public uint Version { get; set; }
		public AnimatedMaterialFlags Flags { get; set; }
		public AnimatedMaterialType Type { get; set; }
		public int LayerID { get; set; }


		public UVSinusParams UVSinus { get; set; }
		public UVNoiseParams UVNoise { get; set; }
		public LocalAlphaParams LocalAlpha { get; set; }
		public LocalAlphaNoiseParams LocalAlphaNoise { get; set; }
		public UVScrollParams UVScroll { get; set; }


		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			Version = s.Serialize<uint>(Version, name: nameof(Version));
			Flags = s.Serialize<AnimatedMaterialFlags>(Flags, name: nameof(Flags));
			Type = s.Serialize<AnimatedMaterialType>(Type, name: nameof(Type));
			LayerID = s.Serialize<int>(LayerID, name: nameof(LayerID));

			switch (Type) {
				case AnimatedMaterialType.UV_Sinus:
					UVSinus = s.SerializeObject<UVSinusParams>(UVSinus, name: nameof(UVSinus));
					break;
				case AnimatedMaterialType.UV_Noise:
					UVNoise = s.SerializeObject<UVNoiseParams>(UVNoise, onPreSerialize: p => p.Modifier = this, name: nameof(UVNoise));
					break;
				case AnimatedMaterialType.LocalAlpha:
					LocalAlpha = s.SerializeObject<LocalAlphaParams>(LocalAlpha, name: nameof(LocalAlpha));
					break;
				case AnimatedMaterialType.LocalAlphaNoise:
					LocalAlphaNoise = s.SerializeObject<LocalAlphaNoiseParams>(LocalAlphaNoise, name: nameof(LocalAlphaNoise));
					break;
				case AnimatedMaterialType.UV_Scroll:
					UVScroll = s.SerializeObject<UVScrollParams>(UVScroll, name: nameof(UVScroll));
					break;
			}
		}
		public class UVSinusParams : BinarySerializable {
			public float SinAmplitude { get; set; }
			public float SinBias { get; set; }
			public float SinSpeed { get; set; }
			public float TimeStart { get; set; }
			public float TimeStop { get; set; }
			public float TimeTotal { get; set; }
			public float UVRecal { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				SinAmplitude = s.Serialize<float>(SinAmplitude, name: nameof(SinAmplitude));
				SinBias = s.Serialize<float>(SinBias, name: nameof(SinBias));
				SinSpeed = s.Serialize<float>(SinSpeed, name: nameof(SinSpeed));
				TimeStart = s.Serialize<float>(TimeStart, name: nameof(TimeStart));
				TimeStop = s.Serialize<float>(TimeStop, name: nameof(TimeStop));
				TimeTotal = s.Serialize<float>(TimeTotal, name: nameof(TimeTotal));
				UVRecal = s.Serialize<float>(UVRecal, name: nameof(UVRecal));
			}
		}
		public class UVNoiseParams : BinarySerializable {
			public GAO_ModifierAnimatedMaterial Modifier { get; set; }

			public float AmplitudeU { get; set; }
			public float BiasU { get; set; }
			public float SpeedU { get; set; }
			public byte OneWayU { get; set; }
			public float AmplitudeV { get; set; }
			public float BiasV { get; set; }
			public float SpeedV { get; set; }
			public byte OneWayV { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				AmplitudeU = s.Serialize<float>(AmplitudeU, name: nameof(AmplitudeU));
				BiasU = s.Serialize<float>(BiasU, name: nameof(BiasU));
				SpeedU = s.Serialize<float>(SpeedU, name: nameof(SpeedU));
				if (Modifier.Version >= 1) {
					OneWayU = s.Serialize<byte>(OneWayU, name: nameof(OneWayU));
					AmplitudeV = s.Serialize<float>(AmplitudeV, name: nameof(AmplitudeV));
					BiasV = s.Serialize<float>(BiasV, name: nameof(BiasV));
					SpeedV = s.Serialize<float>(SpeedV, name: nameof(SpeedV));
					OneWayV = s.Serialize<byte>(OneWayV, name: nameof(OneWayV));
				}
			}
		}
		public class LocalAlphaParams : BinarySerializable {
			public float AlphaA { get; set; }
			public float AlphaB { get; set; }
			public float AlphaC { get; set; }
			public float TimeA { get; set; }
			public float TimeB { get; set; }
			public float TimeC { get; set; }
			public float TimeTotal { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				AlphaA = s.Serialize<float>(AlphaA, name: nameof(AlphaA));
				AlphaB = s.Serialize<float>(AlphaB, name: nameof(AlphaB));
				AlphaC = s.Serialize<float>(AlphaC, name: nameof(AlphaC));
				TimeA = s.Serialize<float>(TimeA, name: nameof(TimeA));
				TimeB = s.Serialize<float>(TimeB, name: nameof(TimeB));
				TimeC = s.Serialize<float>(TimeC, name: nameof(TimeC));
				TimeTotal = s.Serialize<float>(TimeTotal, name: nameof(TimeTotal));
			}
		}
		public class LocalAlphaNoiseParams : BinarySerializable {
			public float Amplitude { get; set; }
			public float Bias { get; set; }
			public float Speed { get; set; }
			public float AlphaMin { get; set; }
			public float AlphaMax { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				AlphaMin = s.Serialize<float>(AlphaMin, name: nameof(AlphaMin));
				AlphaMax = s.Serialize<float>(AlphaMax, name: nameof(AlphaMax));
				Amplitude = s.Serialize<float>(Amplitude, name: nameof(Amplitude));
				Bias = s.Serialize<float>(Bias, name: nameof(Bias));
				Speed = s.Serialize<float>(Speed, name: nameof(Speed));
			}
		}
		public class UVScrollParams : BinarySerializable {
			public float Speed { get; set; }
			public float TimeStart { get; set; }
			public float TimeStop { get; set; }
			public float TimeTotal { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Speed = s.Serialize<float>(Speed, name: nameof(Speed));
				TimeStart = s.Serialize<float>(TimeStart, name: nameof(TimeStart));
				TimeStop = s.Serialize<float>(TimeStop, name: nameof(TimeStop));
				TimeTotal = s.Serialize<float>(TimeTotal, name: nameof(TimeTotal));
			}
		}

		[Flags]
		public enum AnimatedMaterialFlags : uint {
			None = 0,
			ApplyToU = 1 << 0,
			ApplyToV = 1 << 1,
			MatDuplicated = 1 << 2,
		}
		public enum AnimatedMaterialType : uint {
			UV_Sinus = 0,
			UV_Noise = 1,
			LocalAlpha = 2,
			LocalAlphaNoise = 3,
			UV_Scroll = 4,
			UV_NumberOfTypes = 5,
		}
	}
}
