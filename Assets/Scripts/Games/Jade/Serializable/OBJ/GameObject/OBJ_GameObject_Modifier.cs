using System;
using BinarySerializer;

namespace Ray1Map.Jade {
	public class OBJ_GameObject_Modifier : BinarySerializable {
		public MDF_ModifierType Type { get; set; }
        public MDF_ModifierType_Montreal Type_Montreal { get; set; }
        public MDF_ModifierType_CPP Type_CPP { get; set; }
		public uint Flags { get; set; }
        public uint Platform { get; set; }
        public MDF_Modifier Modifier { get; set; }

        public bool IsNull {
            get {
                if (Context.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal)) {
                    return Type_Montreal == MDF_ModifierType_Montreal.None;
                } else {
                    return Type == MDF_ModifierType.None;
                }
            }
        }

		T SerializeModifier<T>(SerializerObject s) where T : MDF_Modifier, new() {
			return s.SerializeObject<T>((T)Modifier, name: nameof(Modifier));
		}

		public override void SerializeImpl(SerializerObject s) {
            if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal)) {
				if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_CPP)) {
					SerializeImpl_CPP(s);
				} else {
					SerializeImpl_Montreal(s);
				}
                return;
            }

			Type = s.Serialize<MDF_ModifierType>(Type, name: nameof(Type));
			if(Type != MDF_ModifierType.None) Flags = s.Serialize<uint>(Flags, name: nameof(Flags));

            
            Modifier = Type switch
            {
                MDF_ModifierType.None => null,
				MDF_ModifierType.GEO_ModifierRLICarte => SerializeModifier<GEO_ModifierRLICarte>(s),
				MDF_ModifierType.GAO_ModifierXMEN => SerializeModifier<GAO_ModifierXMEN>(s),
				MDF_ModifierType.GAO_ModifierXMEC => SerializeModifier<GAO_ModifierXMEC>(s),
				MDF_ModifierType.GAO_ModifierROTR => SerializeModifier<GAO_ModifierROTR>(s),
				MDF_ModifierType.MPAG_Modifier => SerializeModifier<MPAG_Modifier>(s),
				MDF_ModifierType.GEO_ModifierMorphing => SerializeModifier<GEO_ModifierMorphing>(s),
				MDF_ModifierType.GEO_ModifierSnap => SerializeModifier<GEO_ModifierSnap>(s),
				MDF_ModifierType.GEO_ModifierSTP => SerializeModifier<GEO_ModifierSTP>(s),
				MDF_ModifierType.GEN_ModifierSound => SerializeModifier<GEN_ModifierSound>(s),
				MDF_ModifierType.GEN_ModifierSoundFx => SerializeModifier<GEN_ModifierSoundFx>(s),
				MDF_ModifierType.GAO_ModifierShadow => SerializeModifier<GAO_ModifierShadow>(s),
				MDF_ModifierType.PROTEX_Modifier => SerializeModifier<PROTEX_Modifier>(s),
				MDF_ModifierType.GAO_ModifierLazy => SerializeModifier<GAO_ModifierLazy>(s),
				MDF_ModifierType.GAO_ModifierAnimatedGAO => SerializeModifier<GAO_ModifierAnimatedGAO>(s),
				MDF_ModifierType.GAO_ModifierSemiLookAt => SerializeModifier<GAO_ModifierSemiLookAt>(s),
				MDF_ModifierType.GAO_ModifierSNAKE => SerializeModifier<GAO_ModifierSNAKE>(s),
				MDF_ModifierType.GAO_ModifierPhoto => SerializeModifier<GAO_ModifierPhoto>(s),
				MDF_ModifierType.GAO_ModifierBoneRefine => SerializeModifier<GAO_ModifierBoneRefine>(s),
				MDF_ModifierType.GAO_ModifierSpecialLookAt => SerializeModifier<GAO_ModifierSpecialLookAt>(s),
				MDF_ModifierType.GAO_ModifierLegLink => SerializeModifier<GAO_ModifierLegLink>(s),
				MDF_ModifierType.GAO_ModifierFOGDY => SerializeModifier<GAO_ModifierFOGDY>(s),
				MDF_ModifierType.GAO_ModifierFOGDY_Emtr => SerializeModifier<GAO_ModifierFOGDY_Emtr>(s),
				MDF_ModifierType.GAO_ModifierODE => SerializeModifier<GAO_ModifierODE>(s),
				MDF_ModifierType.GAO_ModifierTranslationPaste => SerializeModifier<GAO_ModifierTranslationPaste>(s),
				MDF_ModifierType.GEO_ModifierSymetrie => SerializeModifier<GEO_ModifierSymetrie>(s),
				MDF_ModifierType.SPG_Modifier => SerializeModifier<SPG_Modifier>(s),
				MDF_ModifierType.SPG2_Modifier => SerializeModifier<SPG2_Modifier>(s),
				MDF_ModifierType.FUR_Modifier => SerializeModifier<FUR_Modifier>(s),
				MDF_ModifierType.FUR_ModifierDynFur => SerializeModifier<FUR_ModifierDynFur>(s),
				MDF_ModifierType.GEO_ModifierOnduleTonCorps => SerializeModifier<GEO_ModifierOnduleTonCorps>(s),
				MDF_ModifierType.UVTexWave_Modifier => SerializeModifier<UVTexWave_Modifier>(s),
				MDF_ModifierType.SND_ModifierSoundVol => SerializeModifier<SND_ModifierSoundVol>(s),
				MDF_ModifierType.GAO_ModifierSfx => SerializeModifier<GAO_ModifierSfxMontpellier>(s),
				MDF_ModifierType.MDF_LoadingSound => SerializeModifier<MDF_LoadingSound>(s),
				MDF_ModifierType.Disturber_Modifier => SerializeModifier<Disturber_Modifier>(s),
				MDF_ModifierType.MDF_ModifierWeather => SerializeModifier<MDF_ModifierWeather>(s),
				MDF_ModifierType.WATER3D_Modifier => SerializeModifier<WATER3D_Modifier>(s),
				MDF_ModifierType.GAO_ModifierExplode => SerializeModifier<GAO_ModifierExplode>(s),
				MDF_ModifierType.GAO_ModifierCharacterFX => SerializeModifier<GAO_ModifierCharacterFX>(s),
				MDF_ModifierType.Modifier56 => SerializeModifier<MDF_ModifierPhoenix56>(s),
				MDF_ModifierType.Modifier57 => SerializeModifier<MDF_ModifierPhoenix57>(s),
				_ => throw new NotImplementedException($"TODO: Implement Modifier Type {Type}")
            };
        }


        public void SerializeImpl_Montreal(SerializerObject s) {
            Type_Montreal = s.Serialize<MDF_ModifierType_Montreal>(Type_Montreal, name: nameof(Type_Montreal));
            if (Type_Montreal != MDF_ModifierType_Montreal.None) {
                Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
                if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PoP_WW_20040920) && (Flags & 0x20000000) != 0) {
					Platform = s.Serialize<uint>(Platform, name: nameof(Platform));
				}
            }

            Modifier = Type_Montreal switch
            {
                MDF_ModifierType_Montreal.None => null,
				MDF_ModifierType_Montreal.GAO_ModifierXMEN => SerializeModifier<GAO_ModifierXMEN>(s),
				MDF_ModifierType_Montreal.GAO_ModifierXMEC => SerializeModifier<GAO_ModifierXMEC>(s),
				MDF_ModifierType_Montreal.GAO_ModifierROTR => SerializeModifier<GAO_ModifierROTR>(s),
				MDF_ModifierType_Montreal.GEO_ModifierMorphing => SerializeModifier<GEO_ModifierMorphing>(s),
				MDF_ModifierType_Montreal.GEO_ModifierSnap => SerializeModifier<GEO_ModifierSnap>(s),
				MDF_ModifierType_Montreal.GEN_ModifierSound => SerializeModifier<GEN_ModifierSound>(s),
				MDF_ModifierType_Montreal.GEN_ModifierSoundFx => SerializeModifier<GEN_ModifierSoundFx>(s),
				MDF_ModifierType_Montreal.GAO_ModifierShadow => SerializeModifier<GAO_ModifierShadow>(s),
				MDF_ModifierType_Montreal.PROTEX_Modifier => SerializeModifier<PROTEX_Modifier>(s),
				MDF_ModifierType_Montreal.GAO_ModifierAnimatedGAO => SerializeModifier<GAO_ModifierAnimatedGAO>(s),
				MDF_ModifierType_Montreal.GAO_ModifierAnimatedPAG => SerializeModifier<GAO_ModifierAnimatedPAG>(s),
				MDF_ModifierType_Montreal.GAO_ModifierAnimatedMaterial => SerializeModifier<GAO_ModifierAnimatedMaterial>(s),
				MDF_ModifierType_Montreal.GAO_ModifierSemiLookAt => SerializeModifier<GAO_ModifierSemiLookAt>(s),
				MDF_ModifierType_Montreal.GAO_ModifierSNAKE => SerializeModifier<GAO_ModifierSNAKE>(s),
				MDF_ModifierType_Montreal.GAO_ModifierSpecialLookAt => SerializeModifier<GAO_ModifierSpecialLookAt>(s),
				MDF_ModifierType_Montreal.GAO_ModifierLegLink => SerializeModifier<GAO_ModifierLegLink>(s),
				MDF_ModifierType_Montreal.GAO_ModifierTranslationPaste => SerializeModifier<GAO_ModifierTranslationPaste>(s),
				MDF_ModifierType_Montreal.GEO_ModifierSymetrie => SerializeModifier<GEO_ModifierSymetrie>(s),
				MDF_ModifierType_Montreal.SPG_Modifier => SerializeModifier<SPG_Modifier>(s),
				MDF_ModifierType_Montreal.SPG2_Modifier => SerializeModifier<SPG2_Modifier>(s),
				MDF_ModifierType_Montreal.GEO_ModifierOnduleTonCorps => SerializeModifier<GEO_ModifierOnduleTonCorps>(s),
				MDF_ModifierType_Montreal.Disturber_Modifier => SerializeModifier<Disturber_Modifier>(s),
				MDF_ModifierType_Montreal.GAO_ModifierBeamGen => SerializeModifier<GAO_ModifierBeamGen>(s),
				MDF_ModifierType_Montreal.DARE_ModifierSound => SerializeModifier<DARE_ModifierSound>(s),
				MDF_ModifierType_Montreal.GAO_ModifierSpring => SerializeModifier<GAO_ModifierSpring>(s),
				MDF_ModifierType_Montreal.GAO_ModifierRotC => SerializeModifier<GAO_ModifierRotC>(s),
				MDF_ModifierType_Montreal.GAO_ModifierAnimIK => SerializeModifier<GAO_ModifierAnimIK>(s),
				MDF_ModifierType_Montreal.GEO_ModifierBridge => SerializeModifier<GEO_ModifierBridge>(s),
				MDF_ModifierType_Montreal.GAO_ModifierSfx => SerializeModifier<GAO_ModifierSfx>(s),
				MDF_ModifierType_Montreal.GAO_ModifierSoftBody => SerializeModifier<GAO_ModifierSoftBody>(s),
				MDF_ModifierType_Montreal.GAO_ModifierWind => SerializeModifier<GAO_ModifierWind>(s),
				MDF_ModifierType_Montreal.WATER3D_Modifier => SerializeModifier<WATER3D_Modifier>(s),
				MDF_ModifierType_Montreal.Halo_Modifier => SerializeModifier<Halo_Modifier>(s),
				MDF_ModifierType_Montreal.GAO_ModifierPlant => SerializeModifier<GAO_ModifierPlant>(s),
				MDF_ModifierType_Montreal.GAO_ModifierTree => SerializeModifier<GAO_ModifierTree>(s),
				MDF_ModifierType_Montreal.GAO_ModifierRope => SerializeModifier<GAO_ModifierRope>(s),
				MDF_ModifierType_Montreal.GAO_ModifierVirtualAnim => SerializeModifier<GAO_ModifierVirtualAnim>(s),
				MDF_ModifierType_Montreal.GAO_ModifierCharacterFX => SerializeModifier<GAO_ModifierCharacterFX>(s),
				MDF_ModifierType_Montreal.GAO_ModifierVoiceManager => SerializeModifier<GAO_ModifierVoiceManager>(s),
				MDF_ModifierType_Montreal.Ambiance_Modifier => SerializeModifier<Ambiance_Modifier>(s),
				MDF_ModifierType_Montreal.AmbianceLinker_Modifier => SerializeModifier<AmbianceLinker_Modifier>(s),
				MDF_ModifierType_Montreal.AmbiancePocket_Modifier => SerializeModifier<AmbiancePocket_Modifier>(s),
				MDF_ModifierType_Montreal.GAO_ModifierSectorisationElement => SerializeModifier<GAO_ModifierSectorisationElement>(s),
				MDF_ModifierType_Montreal.GAO_ModifierEyeTrail => SerializeModifier<GAO_ModifierEyeTrail>(s),
				MDF_ModifierType_Montreal.GAO_ModifierPendula => SerializeModifier<GAO_ModifierPendula>(s),
				MDF_ModifierType_Montreal.GEO_ModifierMeshToParticles => SerializeModifier<GEO_ModiferMeshToParticles>(s),
				MDF_ModifierType_Montreal.GAO_ModifierRotationPaste => SerializeModifier<GAO_ModifierRotationPaste>(s),
				MDF_ModifierType_Montreal.GAO_ModifierExplode => SerializeModifier<GAO_ModifierExplode>(s),
				MDF_ModifierType_Montreal.GAO_ModifierEcharpe => SerializeModifier<GAO_ModifierEcharpe>(s),
				MDF_ModifierType_Montreal.GAO_ModifierSoftBodyColl => SerializeModifier<GAO_ModifierSoftBodyColl>(s),
				MDF_ModifierType_Montreal.GAO_ModifierInteractivePlant => SerializeModifier<GAO_ModifierInteractivePlant>(s),
				MDF_ModifierType_Montreal.GAO_ModifierMotionBlur => SerializeModifier<GAO_ModifierMotionBlur>(s),
				MDF_ModifierType_Montreal.GAO_ModifierAlphaOccluder => SerializeModifier<GAO_ModifierAlphaOccluder>(s),
				MDF_ModifierType_Montreal.Outline_Modifier => SerializeModifier<Outline_Modifier>(s),
                _ => throw new NotImplementedException($"TODO: Implement Modifier Type {Type_Montreal}")
            };
        }


		public void SerializeImpl_CPP(SerializerObject s) {
			Type_CPP = s.Serialize<MDF_ModifierType_CPP>(Type_CPP, name: nameof(Type_CPP));
			if (Type_CPP != MDF_ModifierType_CPP.None) {
				Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
				if ((Flags & 0x20000000) != 0) {
					Platform = s.Serialize<uint>(Platform, name: nameof(Platform));
				}
			}
			if (Type_CPP >= MDF_ModifierType_CPP.MDF_EngineSplit) {
				if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_RRRTVParty)) {
					Modifier = Type_CPP switch {
						//MDF_ModifierType_CPP.MDF_SoundBank => SerializeModifier<>(s),
						_ => throw new NotImplementedException($"TODO: Implement Modifier Type {Type_CPP}")
					};
				} else if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_SeanWhiteSkateboarding)) {
					Modifier = Type_CPP switch {
						MDF_ModifierType_CPP.MDF_AnimIK => SerializeModifier<GAO_ModifierAnimIK>(s),
						// MDF_ModifierType_CPP.MDF_ClothDistort  => SerializeModifier<>(s),
						// MDF_ModifierType_CPP.MDF_Skybox => SerializeModifier<>(s),
						// MDF_ModifierType_CPP.MDF_3DText => SerializeModifier<>(s),
						// MDF_ModifierType_CPP.MDF_AudioAmbiancePortal_SW => SerializeModifier<>(s),
						// MDF_ModifierType_CPP.MDF_RadioTrigger => SerializeModifier<>(s),
						// MDF_ModifierType_CPP.MDF_ConstColorBlend => SerializeModifier<>(s),
						_ => throw new NotImplementedException($"TODO: Implement Modifier Type {Type_CPP}")
					};
				} else if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PoP_TFS)) {
					Modifier = Type_CPP switch {
						MDF_ModifierType_CPP.MDF_AnimIK => SerializeModifier<GAO_ModifierAnimIK>(s),
						// MDF_ModifierType_CPP.MDF_Melt => SerializeModifier<>(s),
						// MDF_ModifierType_CPP.MDF_AudioAmbiancePortal => SerializeModifier<>(s),
						// MDF_ModifierType_CPP.MDF_EvilPlant => SerializeModifier<>(s),
						_ => throw new NotImplementedException($"TODO: Implement Modifier Type {Type_CPP}")
					};
				} else {
					throw new NotImplementedException();
				}
			} else {
				Modifier = Type_CPP switch {
					MDF_ModifierType_CPP.None => null,
					MDF_ModifierType_CPP.MDF_WaveYourBody => SerializeModifier<GEO_ModifierOnduleTonCorps>(s),
					MDF_ModifierType_CPP.MDF_Morphing => SerializeModifier<GEO_ModifierMorphing>(s),
					MDF_ModifierType_CPP.MDF_Lazy => SerializeModifier<GAO_ModifierLazy>(s),
					// MDF_ModifierType_CPP.MDF_BoneMeca => SerializeModifier<>(s),
					MDF_ModifierType_CPP.MDF_SemiLookAt => SerializeModifier<GAO_ModifierSemiLookAt>(s),
					MDF_ModifierType_CPP.MDF_Shadow => SerializeModifier<GAO_ModifierShadow>(s),
					MDF_ModifierType_CPP.MDF_SpecialLookAt => SerializeModifier<GAO_ModifierSpecialLookAt>(s),
					MDF_ModifierType_CPP.MDF_BoneRefine => SerializeModifier<GAO_ModifierBoneRefine>(s),
					MDF_ModifierType_CPP.MDF_XMEN => SerializeModifier<GAO_ModifierXMEN>(s),

					MDF_ModifierType_CPP.MDF_RotR => SerializeModifier<GAO_ModifierROTR>(s),
					MDF_ModifierType_CPP.MDF_Snake => SerializeModifier<GAO_ModifierSNAKE>(s),
					// MDF_ModifierType_CPP.MDF_Audio => SerializeModifier<>(s),
					// MDF_ModifierType_CPP.MDF_AudioAttenuator => SerializeModifier<>(s),

					MDF_ModifierType_CPP.MDF_SoftBody => SerializeModifier<GAO_ModifierSoftBody>(s),
					MDF_ModifierType_CPP.MDF_Spring => SerializeModifier<GAO_ModifierSpring>(s),

					MDF_ModifierType_CPP.MDF_Water => SerializeModifier<WATER3D_Modifier>(s),
					MDF_ModifierType_CPP.MDF_RotC => SerializeModifier<GAO_ModifierRotC>(s),
					MDF_ModifierType_CPP.MDF_BeamGen => SerializeModifier<GAO_ModifierBeamGen>(s),
					MDF_ModifierType_CPP.MDF_Disturber => SerializeModifier<Disturber_Modifier>(s),
					// MDF_ModifierType_CPP.MDF_Nop => SerializeModifier<>(s),
					//MDF_ModifierType_CPP.MDF_AnimatedScale => SerializeModifier<>(s),
					MDF_ModifierType_CPP.MDF_Wind => SerializeModifier<GAO_ModifierWind>(s),

					MDF_ModifierType_CPP.MDF_Halo => SerializeModifier<Halo_Modifier>(s),
					MDF_ModifierType_CPP.MDF_SectoElement => SerializeModifier<GAO_ModifierSectorisationElement>(s),
					MDF_ModifierType_CPP.MDF_AnimatedMaterial => SerializeModifier<GAO_ModifierAnimatedMaterial>(s),
					MDF_ModifierType_CPP.MDF_RotationPaste => SerializeModifier<GAO_ModifierRotationPaste>(s),
					MDF_ModifierType_CPP.MDF_Ambiance => SerializeModifier<Ambiance_Modifier>(s),
					MDF_ModifierType_CPP.MDF_AmbianceLinker => SerializeModifier<AmbianceLinker_Modifier>(s),
					MDF_ModifierType_CPP.MDF_AmbiancePocket => SerializeModifier<AmbiancePocket_Modifier>(s),
					MDF_ModifierType_CPP.MDF_Pendula => SerializeModifier<GAO_ModifierPendula>(s),
					MDF_ModifierType_CPP.MDF_TranslationPaste => SerializeModifier<GAO_ModifierTranslationPaste>(s),
					MDF_ModifierType_CPP.MDF_AnimatedPAG => SerializeModifier<GAO_ModifierAnimatedPAG>(s),
					MDF_ModifierType_CPP.MDF_AnimatedGAO => SerializeModifier<GAO_ModifierAnimatedGAO>(s),

					MDF_ModifierType_CPP.MDF_CharacterFX => SerializeModifier<GAO_ModifierCharacterFX>(s),

					// MDF_ModifierType_CPP.MDF_SoftBodyColl => SerializeModifier<>(s),

					// MDF_ModifierType_CPP.MDF_AlphaFade => SerializeModifier<>(s),
					MDF_ModifierType_CPP.MDF_AlphaOccluder => SerializeModifier<GAO_ModifierAlphaOccluder>(s),
					MDF_ModifierType_CPP.MDF_InteractivePlant => SerializeModifier<GAO_ModifierInteractivePlant>(s),
					// MDF_ModifierType_CPP.MDF_PreDepthPass => SerializeModifier<>(s),
					// MDF_ModifierType_CPP.MDF_VolumetricSound => SerializeModifier<>(s),
					// MDF_ModifierType_CPP.MDF_ProceduralBone => SerializeModifier<>(s),
					// MDF_ModifierType_CPP.MDF_AudioReverbZone => SerializeModifier<>(s),

					// MDF_ModifierType_CPP.MDF_CharacterFxRef  => SerializeModifier<>(s),
					_ => throw new NotImplementedException($"TODO: Implement Modifier Type {Type_CPP}")
				};
			}
		}
	}
}
