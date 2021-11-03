using BinarySerializer;

namespace Ray1Map.Jade {
	public class WATER3D_Modifier : MDF_Modifier {
		public uint Version { get; set; }
		public Jade_Matrix FixChromeMatrix { get; set; }
		public float Damping { get; set; }
		public float PropagationSpeed { get; set; }
		public float PerturbanceAmplitudeModifier { get; set; }
		public float ImpactForceAttenuation { get; set; }
		public float TurbulanceAmplitude { get; set; }
		public uint TurbulanceFactor { get; set; }
		public uint Density { get; set; }
		public float Radius { get; set; }
		public float DampingOutsideRadius { get; set; }
		public uint TurbulanceOffIfOutsideRadius { get; set; } // Boolean
		public float RadiusCut { get; set; }

		public uint WaterChrome { get; set; } // Boolean

		public float V5_Float_0 { get; set; }
		public float V5_Float_1 { get; set; }

		public float V6_Float { get; set; }

		public float V7_Float_0 { get; set; }
		public float V7_Float_1 { get; set; }

		public float V8_Float { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			Version = s.Serialize<uint>(Version, name: nameof(Version));
			FixChromeMatrix = s.SerializeObject<Jade_Matrix>(FixChromeMatrix, name: nameof(FixChromeMatrix));
			Damping = s.Serialize<float>(Damping, name: nameof(Damping));
			PropagationSpeed = s.Serialize<float>(PropagationSpeed, name: nameof(PropagationSpeed));
			PerturbanceAmplitudeModifier = s.Serialize<float>(PerturbanceAmplitudeModifier, name: nameof(PerturbanceAmplitudeModifier));
			ImpactForceAttenuation = s.Serialize<float>(ImpactForceAttenuation, name: nameof(ImpactForceAttenuation));
			TurbulanceAmplitude = s.Serialize<float>(TurbulanceAmplitude, name: nameof(TurbulanceAmplitude));
			TurbulanceFactor = s.Serialize<uint>(TurbulanceFactor, name: nameof(TurbulanceFactor));
			Density = s.Serialize<uint>(Density, name: nameof(Density));
			Radius = s.Serialize<float>(Radius, name: nameof(Radius));
			DampingOutsideRadius = s.Serialize<float>(DampingOutsideRadius, name: nameof(DampingOutsideRadius));
			TurbulanceOffIfOutsideRadius = s.Serialize<uint>(TurbulanceOffIfOutsideRadius, name: nameof(TurbulanceOffIfOutsideRadius));
			RadiusCut = s.Serialize<float>(RadiusCut, name: nameof(RadiusCut));

			if (Version >= 4) WaterChrome = s.Serialize<uint>(WaterChrome, name: nameof(WaterChrome));

			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montpellier)) {
				if (Version >= 5) {
					V5_Float_0 = s.Serialize<float>(V5_Float_0, name: nameof(V5_Float_0));
					V5_Float_1 = s.Serialize<float>(V5_Float_1, name: nameof(V5_Float_1));
				}

				if (Version >= 6) V6_Float = s.Serialize<float>(V6_Float, name: nameof(V6_Float));

				if (Version >= 7) {
					V7_Float_0 = s.Serialize<float>(V7_Float_0, name: nameof(V7_Float_0));
					V7_Float_1 = s.Serialize<float>(V7_Float_1, name: nameof(V7_Float_1));
				}

				if (Version >= 8) V8_Float = s.Serialize<float>(V8_Float, name: nameof(V8_Float));
			}
		}
	}
}
