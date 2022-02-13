using BinarySerializer;

namespace Ray1Map.Jade {
	public class GAO_ModifierWind : MDF_Modifier {
		public uint Version { get; set; }
		public WindSource Source { get; set; }
		public uint AmbientSoundPlayEvent { get; set; }
		public uint AmbientSoundStopEvent { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Version = s.Serialize<uint>(Version, name: nameof(Version));
			if (Version == 0) {
				Source = s.SerializeObject<WindSource>(Source, name: nameof(Source));
			} else if (Version == 1) {
				Source = s.SerializeObject<WindSource>(Source, name: nameof(Source));
				AmbientSoundPlayEvent = s.Serialize<uint>(AmbientSoundPlayEvent, name: nameof(AmbientSoundPlayEvent));
				AmbientSoundStopEvent = s.Serialize<uint>(AmbientSoundStopEvent, name: nameof(AmbientSoundStopEvent));
			}
		}

		public class WindSource : BinarySerializable {
			public uint Version { get; set; }
			public float MinForce { get; set; }
			public float MaxForce { get; set; }
			public float Amplitude { get; set; }
			public float Shape { get; set; }
			public float Frequency { get; set; }
			public float Speed { get; set; }
			public float Dx { get; set; }
			public float Dz { get; set; }
			public float ErrorDirection { get; set; }
			public float VariationDirection { get; set; }
			public uint V1_UInt0 { get; set; }
			public uint V1_UInt1 { get; set; }
			public float Near { get; set; }
			public float Far { get; set; }
			public uint V2_UInt { get; set; }
			public uint V3_UInt { get; set; }
			public float V3_Float { get; set; }
			public int ThroughObjects { get; set; } // Boolean
			public int MovingWindSource { get; set; } // Boolean
			public int UseWaveAttenuation { get; set; } // Boolean
			public float WaveAttenuationSpeed { get; set; }
			public float WaveAttenuationFrequency { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Version = s.Serialize<uint>(Version, name: nameof(Version));
				MinForce = s.Serialize<float>(MinForce, name: nameof(MinForce));
				MaxForce = s.Serialize<float>(MaxForce, name: nameof(MaxForce));
				Amplitude = s.Serialize<float>(Amplitude, name: nameof(Amplitude));
				Shape = s.Serialize<float>(Shape, name: nameof(Shape));
				Frequency = s.Serialize<float>(Frequency, name: nameof(Frequency));
				Speed = s.Serialize<float>(Speed, name: nameof(Speed));
				Dx = s.Serialize<float>(Dx, name: nameof(Dx));
				Dz = s.Serialize<float>(Dz, name: nameof(Dz));
				ErrorDirection = s.Serialize<float>(ErrorDirection, name: nameof(ErrorDirection));
				VariationDirection = s.Serialize<float>(VariationDirection, name: nameof(VariationDirection));
				if (Version >= 1 && Version < 4) {
					V1_UInt0 = s.Serialize<uint>(V1_UInt0, name: nameof(V1_UInt0));
					V1_UInt1 = s.Serialize<uint>(V1_UInt1, name: nameof(V1_UInt1));
					Near = s.Serialize<float>(Near, name: nameof(Near));
					Far = s.Serialize<float>(Far, name: nameof(Far));
					if (Version >= 2) V2_UInt = s.Serialize<uint>(V2_UInt, name: nameof(V2_UInt));
					if (Version == 3) {
						V3_UInt = s.Serialize<uint>(V3_UInt, name: nameof(V3_UInt));
						V3_Float = s.Serialize<float>(V3_Float, name: nameof(V3_Float));
					}
				} else if (Version >= 4) {
					Near = s.Serialize<float>(Near, name: nameof(Near));
					Far = s.Serialize<float>(Far, name: nameof(Far));
					if (Version >= 5) ThroughObjects = s.Serialize<int>(ThroughObjects, name: nameof(ThroughObjects));
					if (Version >= 6) MovingWindSource = s.Serialize<int>(MovingWindSource, name: nameof(MovingWindSource));
					if (Version >= 7) {
						UseWaveAttenuation = s.Serialize<int>(UseWaveAttenuation, name: nameof(UseWaveAttenuation));
						WaveAttenuationSpeed = s.Serialize<float>(WaveAttenuationSpeed, name: nameof(WaveAttenuationSpeed));
						WaveAttenuationFrequency = s.Serialize<float>(WaveAttenuationFrequency, name: nameof(WaveAttenuationFrequency));
					}
				}
			}
		}
	}
}
