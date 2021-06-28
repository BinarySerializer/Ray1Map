using BinarySerializer;

namespace R1Engine.Jade {
	public class MDF_ModifierDisturber : MDF_Modifier {
		public uint Version { get; set; }
		public float DisturbBoost { get; set; }

		public uint IsStaticDisturber { get; set; } // Boolean
		public float StaticDisturberForce { get; set; }
		public float StaticDisturberDelay { get; set; }
		public uint V2_UInt_3 { get; set; }
		public uint V2_UInt_4 { get; set; }
		public Jade_Reference<OBJ_GameObject> V2_GameObject1 { get; set; }
		public uint V2_UInt_6 { get; set; }
		public uint V2_UInt_7 { get; set; }
		public Jade_Reference<OBJ_GameObject> V2_GameObject2 { get; set; }
		public uint V2_UInt_9 { get; set; }
		public uint V2_UInt_10 { get; set; }
		public Jade_Reference<OBJ_GameObject> V2_GameObject3 { get; set; }
		public uint V2_UInt_12 { get; set; }

		public uint V10_UInt_0 { get; set; }

		public uint IsFloatOnWater { get; set; }
		public Jade_Vector FloatOnWaterInitialVelocity { get; set; }
		public float FloatOnWaterInitialZRotationSpeed { get; set; }
		public float FloatOnWaterZOffset { get; set; }
		public float FloatOnWaterBankingDamping { get; set; }
		public float FloatOnWaterZStability { get; set; }
		public float FloatOnWaterWaveStrength { get; set; }
		public float FloatOnWaterEvaluationPlaneDelta { get; set; }
		public float FloatOnWaterVelocityBackToInitialStrength { get; set; }
		public float FloatOnWaterWaveInfluence { get; set; }
		public float FloatOnWaterVelocityDamping { get; set; }

		public uint IsPAGDispersionActive { get; set; }
		public float Radius { get; set; }
		public float FrontForceTrans { get; set; }
		public float FrontForceRot { get; set; }
		public float RearForceTrans { get; set; }
		public float RearForceRot { get; set; }
		public float DraftNear { get; set; }
		public float DraftFar { get; set; }

		public uint UseWaterBoundingBoxForCollision { get; set; }

		public float V11_Float_0 { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			Version = s.Serialize<uint>(Version, name: nameof(Version));
			DisturbBoost = s.Serialize<float>(DisturbBoost, name: nameof(DisturbBoost));

			if (Version >= 2) {
				IsStaticDisturber = s.Serialize<uint>(IsStaticDisturber, name: nameof(IsStaticDisturber));
				StaticDisturberForce = s.Serialize<float>(StaticDisturberForce, name: nameof(StaticDisturberForce));
				StaticDisturberDelay = s.Serialize<float>(StaticDisturberDelay, name: nameof(StaticDisturberDelay));
			}
			if (Version == 2 || (s.GetR1Settings().EngineVersion == EngineVersion.Jade_PoP_SoT_20030819 && Version >= 2)) {
				V2_UInt_3  = s.Serialize<uint>(V2_UInt_3 , name: nameof(V2_UInt_3 ));
				V2_UInt_4  = s.Serialize<uint>(V2_UInt_4 , name: nameof(V2_UInt_4 ));
				V2_GameObject1 = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(V2_GameObject1, name: nameof(V2_GameObject1));
				V2_UInt_6  = s.Serialize<uint>(V2_UInt_6 , name: nameof(V2_UInt_6 ));
				V2_UInt_7  = s.Serialize<uint>(V2_UInt_7 , name: nameof(V2_UInt_7 ));
				V2_GameObject2 = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(V2_GameObject2, name: nameof(V2_GameObject2));
				V2_UInt_9  = s.Serialize<uint>(V2_UInt_9 , name: nameof(V2_UInt_9 ));
				V2_UInt_10 = s.Serialize<uint>(V2_UInt_10, name: nameof(V2_UInt_10));
				V2_GameObject3 = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(V2_GameObject3, name: nameof(V2_GameObject3));
				V2_UInt_12 = s.Serialize<uint>(V2_UInt_12, name: nameof(V2_UInt_12));

				if (s.GetR1Settings().EngineVersion == EngineVersion.Jade_PoP_SoT_20030819) {
					V2_GameObject1?.Resolve();
					V2_GameObject2?.Resolve();
					V2_GameObject3?.Resolve();
				}
			}

			if(s.GetR1Settings().EngineVersion == EngineVersion.Jade_PoP_SoT_20030819) return;

			if (Version >= 10 && !s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_RRRTVParty)) V10_UInt_0 = s.Serialize<uint>(V10_UInt_0, name: nameof(V10_UInt_0));
			if (Version >= 3) {
				IsFloatOnWater = s.Serialize<uint>(IsFloatOnWater, name: nameof(IsFloatOnWater));
				FloatOnWaterInitialVelocity = s.SerializeObject<Jade_Vector>(FloatOnWaterInitialVelocity, name: nameof(FloatOnWaterInitialVelocity));
				FloatOnWaterInitialZRotationSpeed = s.Serialize<float>(FloatOnWaterInitialZRotationSpeed, name: nameof(FloatOnWaterInitialZRotationSpeed));
				FloatOnWaterZOffset = s.Serialize<float>(FloatOnWaterZOffset, name: nameof(FloatOnWaterZOffset));
				FloatOnWaterBankingDamping = s.Serialize<float>(FloatOnWaterBankingDamping, name: nameof(FloatOnWaterBankingDamping));
				FloatOnWaterZStability = s.Serialize<float>(FloatOnWaterZStability, name: nameof(FloatOnWaterZStability));
				FloatOnWaterWaveStrength = s.Serialize<float>(FloatOnWaterWaveStrength, name: nameof(FloatOnWaterWaveStrength));
				FloatOnWaterEvaluationPlaneDelta = s.Serialize<float>(FloatOnWaterEvaluationPlaneDelta, name: nameof(FloatOnWaterEvaluationPlaneDelta));
				FloatOnWaterVelocityBackToInitialStrength = s.Serialize<float>(FloatOnWaterVelocityBackToInitialStrength, name: nameof(FloatOnWaterVelocityBackToInitialStrength));
				FloatOnWaterWaveInfluence = s.Serialize<float>(FloatOnWaterWaveInfluence, name: nameof(FloatOnWaterWaveInfluence));
				FloatOnWaterVelocityDamping = s.Serialize<float>(FloatOnWaterVelocityDamping, name: nameof(FloatOnWaterVelocityDamping));
			}
			if (Version >= 8) {
				IsPAGDispersionActive = s.Serialize<uint>(IsPAGDispersionActive, name: nameof(IsPAGDispersionActive));
				if (IsPAGDispersionActive != 0) {
					Radius = s.Serialize<float>(Radius, name: nameof(Radius));
					FrontForceTrans = s.Serialize<float>(FrontForceTrans, name: nameof(FrontForceTrans));
					FrontForceRot = s.Serialize<float>(FrontForceRot, name: nameof(FrontForceRot));
					RearForceTrans = s.Serialize<float>(RearForceTrans, name: nameof(RearForceTrans));
					RearForceRot = s.Serialize<float>(RearForceRot, name: nameof(RearForceRot));
					DraftNear = s.Serialize<float>(DraftNear, name: nameof(DraftNear));
					DraftFar = s.Serialize<float>(DraftFar, name: nameof(DraftFar));
				}
			}
			if (Version >= 9) UseWaterBoundingBoxForCollision = s.Serialize<uint>(UseWaterBoundingBoxForCollision, name: nameof(UseWaterBoundingBoxForCollision));
			if (Version >= 11) V11_Float_0 = s.Serialize<float>(V11_Float_0, name: nameof(V11_Float_0));
		}
	}
}
