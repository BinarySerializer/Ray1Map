using BinarySerializer;

namespace Ray1Map.Jade {
	// Found in GEO_ul_ModifierOnduleTonCorps_Load
	public class GEO_ModifierOnduleTonCorps : MDF_Modifier {
		public uint Version { get; set; }
		public uint Flags { get; set; }
		public float Angle { get; set; }
		public float Amplitude { get; set; } // AmplitudeMin
		public float Factor { get; set; } // FactorMin
		public float Delta { get; set; } // DeltaMin
		public uint MatID { get; set; }

		// Montreal
		public float AmplitudeMax { get; set; }
		public float AmplitudeVar { get; set; }
		public float AmplitudeVarSpeed { get; set; }
		public float FactorMax { get; set; }
		public float FactorVar { get; set; }
		public float FactorVarSpeed { get; set; }
		public float DeltaMax { get; set; }
		public float DeltaVar { get; set; }
		public float DeltaVarSpeed { get; set; }
		public float MaxSpeedForZeroDamping { get; set; }
		public float SpeedSmooth { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			if(!Loader.IsBinaryData || s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal))
				Version = s.Serialize<uint>(Version, name: nameof(Version));
			Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
			Angle = s.Serialize<float>(Angle, name: nameof(Angle));
			Amplitude = s.Serialize<float>(Amplitude, name: nameof(Amplitude));
			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal) && Version >= 26) {
				AmplitudeMax = s.Serialize<float>(AmplitudeMax, name: nameof(AmplitudeMax));
				AmplitudeVar = s.Serialize<float>(AmplitudeVar, name: nameof(AmplitudeVar));
				AmplitudeVarSpeed = s.Serialize<float>(AmplitudeVarSpeed, name: nameof(AmplitudeVarSpeed));
			}
			Factor = s.Serialize<float>(Factor, name: nameof(Factor));
			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal) && Version >= 26) {
				FactorMax = s.Serialize<float>(FactorMax, name: nameof(FactorMax));
				FactorVar = s.Serialize<float>(FactorVar, name: nameof(FactorVar));
				FactorVarSpeed = s.Serialize<float>(FactorVarSpeed, name: nameof(FactorVarSpeed));
			}
			Delta = s.Serialize<float>(Delta, name: nameof(Delta));
			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal) && Version >= 26) {
				DeltaMax = s.Serialize<float>(DeltaMax, name: nameof(DeltaMax));
				DeltaVar = s.Serialize<float>(DeltaVar, name: nameof(DeltaVar));
				DeltaVarSpeed = s.Serialize<float>(DeltaVarSpeed, name: nameof(DeltaVarSpeed));
			}
			if (!Loader.IsBinaryData || s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal))
				MatID = s.Serialize<uint>(MatID, name: nameof(MatID));

			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal)) {
				if (Version >= 27) MaxSpeedForZeroDamping = s.Serialize<float>(MaxSpeedForZeroDamping, name: nameof(MaxSpeedForZeroDamping));
				if (Version >= 28) SpeedSmooth = s.Serialize<float>(SpeedSmooth, name: nameof(SpeedSmooth));
			}
		}
	}
}
