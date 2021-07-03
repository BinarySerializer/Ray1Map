using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace R1Engine.Jade {
	public class OBJ_GameObject_Modifier : BinarySerializable {
		public MDF_ModifierType Type { get; set; }
        public MDF_ModifierType_Montreal Type_Montreal { get; set; }
        public MDF_ModifierType_RRRTVP Type_RRRTVP { get; set; }
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

		public override void SerializeImpl(SerializerObject s) {
            if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal)) {
                SerializeImpl_Montreal(s);
                return;
            }

			Type = s.Serialize<MDF_ModifierType>(Type, name: nameof(Type));
			if(Type != MDF_ModifierType.None) Flags = s.Serialize<uint>(Flags, name: nameof(Flags));

            Modifier = Type switch
            {
                MDF_ModifierType.None => null,
                MDF_ModifierType.GEO_ModifierRLICarte => s.SerializeObject<GEO_ModifierRLICarte>((GEO_ModifierRLICarte)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.GAO_ModifierXMEN => s.SerializeObject<GAO_ModifierXMEN>((GAO_ModifierXMEN)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.GAO_ModifierXMEC => s.SerializeObject<GAO_ModifierXMEC>((GAO_ModifierXMEC)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.GAO_ModifierROTR => s.SerializeObject<GAO_ModifierROTR>((GAO_ModifierROTR)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.MPAG_Modifier => s.SerializeObject<MPAG_Modifier>((MPAG_Modifier)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.GEO_ModifierMorphing => s.SerializeObject<GEO_ModifierMorphing>((GEO_ModifierMorphing)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.GEO_ModifierSnap => s.SerializeObject<GEO_ModifierSnap>((GEO_ModifierSnap)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.GEO_ModifierSTP => s.SerializeObject<GEO_ModifierSTP>((GEO_ModifierSTP)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.GEN_ModifierSound => s.SerializeObject<GEN_ModifierSound>((GEN_ModifierSound)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.GEN_ModifierSoundFx => s.SerializeObject<GEN_ModifierSoundFx>((GEN_ModifierSoundFx)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.GAO_ModifierShadow => s.SerializeObject<GAO_ModifierShadow>((GAO_ModifierShadow)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.PROTEX_Modifier => s.SerializeObject<PROTEX_Modifier>((PROTEX_Modifier)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.GAO_ModifierLazy => s.SerializeObject<GAO_ModifierLazy>((GAO_ModifierLazy)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.GAO_ModifierAnimatedGAO => s.SerializeObject<GAO_ModifierAnimatedGAO>((GAO_ModifierAnimatedGAO)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.GAO_ModifierSemiLookAt => s.SerializeObject<GAO_ModifierSemiLookAt>((GAO_ModifierSemiLookAt)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.GAO_ModifierSNAKE => s.SerializeObject<GAO_ModifierSNAKE>((GAO_ModifierSNAKE)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.GAO_ModifierPhoto => s.SerializeObject<GAO_ModifierPhoto>((GAO_ModifierPhoto)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.GAO_ModifierBoneRefine => s.SerializeObject<GAO_ModifierBoneRefine>((GAO_ModifierBoneRefine)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.GAO_ModifierSpecialLookAt => s.SerializeObject<GAO_ModifierSpecialLookAt>((GAO_ModifierSpecialLookAt)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.GAO_ModifierLegLink => s.SerializeObject<GAO_ModifierLegLink>((GAO_ModifierLegLink)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.GAO_ModifierFOGDY => s.SerializeObject<GAO_ModifierFOGDY>((GAO_ModifierFOGDY)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.GAO_ModifierFOGDY_Emtr => s.SerializeObject<GAO_ModifierFOGDY_Emtr>((GAO_ModifierFOGDY_Emtr)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.GAO_ModifierODE => s.SerializeObject<GAO_ModifierODE>((GAO_ModifierODE)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.GAO_ModifierTranslationPaste => s.SerializeObject<GAO_ModifierTranslationPaste>((GAO_ModifierTranslationPaste)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.GEO_ModifierSymetrie => s.SerializeObject<GEO_ModifierSymetrie>((GEO_ModifierSymetrie)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.SPG_Modifier => s.SerializeObject<SPG_Modifier>((SPG_Modifier)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.SPG2_Modifier => s.SerializeObject<SPG2_Modifier>((SPG2_Modifier)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.FUR_Modifier => s.SerializeObject<FUR_Modifier>((FUR_Modifier)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.FUR_ModifierDynFur => s.SerializeObject<FUR_ModifierDynFur>((FUR_ModifierDynFur)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.GEO_ModifierOnduleTonCorps => s.SerializeObject<GEO_ModifierOnduleTonCorps>((GEO_ModifierOnduleTonCorps)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.UVTexWave_Modifier => s.SerializeObject<UVTexWave_Modifier>((UVTexWave_Modifier)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.SND_ModifierSoundVol => s.SerializeObject<SND_ModifierSoundVol>((SND_ModifierSoundVol)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.GAO_ModifierSfx => s.SerializeObject<GAO_ModifierSfxMontpellier>((GAO_ModifierSfxMontpellier)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.MDF_LoadingSound => s.SerializeObject<MDF_LoadingSound>((MDF_LoadingSound)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.Disturber_Modifier => s.SerializeObject<Disturber_Modifier>((Disturber_Modifier)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.MDF_ModifierWeather => s.SerializeObject<MDF_ModifierWeather>((MDF_ModifierWeather)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.WATER3D_Modifier => s.SerializeObject<WATER3D_Modifier>((WATER3D_Modifier)Modifier, name: nameof(Modifier)),
                _ => throw new NotImplementedException($"TODO: Implement Modifier Type {Type}")
            };
        }


        public void SerializeImpl_Montreal(SerializerObject s) {
            Type_Montreal = s.Serialize<MDF_ModifierType_Montreal>(Type_Montreal, name: nameof(Type_Montreal));
            if (Type_Montreal != MDF_ModifierType_Montreal.None) {
                Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
                if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PoP_WW) && (Flags & 0x20000000) != 0) {
					Platform = s.Serialize<uint>(Platform, name: nameof(Platform));
				}
            }

            Modifier = Type_Montreal switch
            {
                MDF_ModifierType_Montreal.None => null,
                MDF_ModifierType_Montreal.GAO_ModifierXMEN => s.SerializeObject<GAO_ModifierXMEN>((GAO_ModifierXMEN)Modifier, name: nameof(Modifier)),
                MDF_ModifierType_Montreal.GAO_ModifierXMEC => s.SerializeObject<GAO_ModifierXMEC>((GAO_ModifierXMEC)Modifier, name: nameof(Modifier)),
                MDF_ModifierType_Montreal.GAO_ModifierROTR => s.SerializeObject<GAO_ModifierROTR>((GAO_ModifierROTR)Modifier, name: nameof(Modifier)),
                MDF_ModifierType_Montreal.GEO_ModifierMorphing => s.SerializeObject<GEO_ModifierMorphing>((GEO_ModifierMorphing)Modifier, name: nameof(Modifier)),
                MDF_ModifierType_Montreal.GEO_ModifierSnap => s.SerializeObject<GEO_ModifierSnap>((GEO_ModifierSnap)Modifier, name: nameof(Modifier)),
                MDF_ModifierType_Montreal.GEN_ModifierSound => s.SerializeObject<GEN_ModifierSound>((GEN_ModifierSound)Modifier, name: nameof(Modifier)),
                MDF_ModifierType_Montreal.GEN_ModifierSoundFx => s.SerializeObject<GEN_ModifierSoundFx>((GEN_ModifierSoundFx)Modifier, name: nameof(Modifier)),
                MDF_ModifierType_Montreal.GAO_ModifierShadow => s.SerializeObject<GAO_ModifierShadow>((GAO_ModifierShadow)Modifier, name: nameof(Modifier)),
                MDF_ModifierType_Montreal.PROTEX_Modifier => s.SerializeObject<PROTEX_Modifier>((PROTEX_Modifier)Modifier, name: nameof(Modifier)),
                MDF_ModifierType_Montreal.GAO_ModifierAnimatedGAO => s.SerializeObject<GAO_ModifierAnimatedGAO>((GAO_ModifierAnimatedGAO)Modifier, name: nameof(Modifier)),
                MDF_ModifierType_Montreal.GAO_ModifierSemiLookAt => s.SerializeObject<GAO_ModifierSemiLookAt>((GAO_ModifierSemiLookAt)Modifier, name: nameof(Modifier)),
                MDF_ModifierType_Montreal.GAO_ModifierSNAKE => s.SerializeObject<GAO_ModifierSNAKE>((GAO_ModifierSNAKE)Modifier, name: nameof(Modifier)),
                MDF_ModifierType_Montreal.GAO_ModifierSpecialLookAt => s.SerializeObject<GAO_ModifierSpecialLookAt>((GAO_ModifierSpecialLookAt)Modifier, name: nameof(Modifier)),
                MDF_ModifierType_Montreal.GAO_ModifierLegLink => s.SerializeObject<GAO_ModifierLegLink>((GAO_ModifierLegLink)Modifier, name: nameof(Modifier)),
                MDF_ModifierType_Montreal.GAO_ModifierTranslationPaste => s.SerializeObject<GAO_ModifierTranslationPaste>((GAO_ModifierTranslationPaste)Modifier, name: nameof(Modifier)),
                MDF_ModifierType_Montreal.GEO_ModifierSymetrie => s.SerializeObject<GEO_ModifierSymetrie>((GEO_ModifierSymetrie)Modifier, name: nameof(Modifier)),
                MDF_ModifierType_Montreal.SPG_Modifier => s.SerializeObject<SPG_Modifier>((SPG_Modifier)Modifier, name: nameof(Modifier)),
                MDF_ModifierType_Montreal.SPG2_Modifier => s.SerializeObject<SPG2_Modifier>((SPG2_Modifier)Modifier, name: nameof(Modifier)),
                MDF_ModifierType_Montreal.GEO_ModifierOnduleTonCorps => s.SerializeObject<GEO_ModifierOnduleTonCorps>((GEO_ModifierOnduleTonCorps)Modifier, name: nameof(Modifier)),
                MDF_ModifierType_Montreal.Disturber_Modifier => s.SerializeObject<Disturber_Modifier>((Disturber_Modifier)Modifier, name: nameof(Modifier)),
                MDF_ModifierType_Montreal.GAO_ModifierBeamGen => s.SerializeObject<GAO_ModifierBeamGen>((GAO_ModifierBeamGen)Modifier, name: nameof(Modifier)),
                MDF_ModifierType_Montreal.DARE_ModifierSound => s.SerializeObject<DARE_ModifierSound>((DARE_ModifierSound)Modifier, name: nameof(Modifier)),
                MDF_ModifierType_Montreal.GAO_ModifierSpring => s.SerializeObject<GAO_ModifierSpring>((GAO_ModifierSpring)Modifier, name: nameof(Modifier)),
                MDF_ModifierType_Montreal.GAO_ModifierRotC => s.SerializeObject<GAO_ModifierRotC>((GAO_ModifierRotC)Modifier, name: nameof(Modifier)),
                MDF_ModifierType_Montreal.GAO_ModifierAnimIK => s.SerializeObject<GAO_ModifierAnimIK>((GAO_ModifierAnimIK)Modifier, name: nameof(Modifier)),
                MDF_ModifierType_Montreal.GEO_ModifierBridge => s.SerializeObject<GEO_ModifierBridge>((GEO_ModifierBridge)Modifier, name: nameof(Modifier)),
                MDF_ModifierType_Montreal.GAO_ModifierSfx => s.SerializeObject<GAO_ModifierSfx>((GAO_ModifierSfx)Modifier, name: nameof(Modifier)),
                MDF_ModifierType_Montreal.GAO_ModifierSoftBody => s.SerializeObject<GAO_ModifierSoftBody>((GAO_ModifierSoftBody)Modifier, name: nameof(Modifier)),
                MDF_ModifierType_Montreal.GAO_ModifierWind => s.SerializeObject<GAO_ModifierWind>((GAO_ModifierWind)Modifier, name: nameof(Modifier)),
                MDF_ModifierType_Montreal.WATER3D_Modifier => s.SerializeObject<WATER3D_Modifier>((WATER3D_Modifier)Modifier, name: nameof(Modifier)),
                MDF_ModifierType_Montreal.Halo_Modifier => s.SerializeObject<Halo_Modifier>((Halo_Modifier)Modifier, name: nameof(Modifier)),
                MDF_ModifierType_Montreal.GAO_ModifierPlant => s.SerializeObject<GAO_ModifierPlant>((GAO_ModifierPlant)Modifier, name: nameof(Modifier)),
                MDF_ModifierType_Montreal.GAO_ModifierTree => s.SerializeObject<GAO_ModifierTree>((GAO_ModifierTree)Modifier, name: nameof(Modifier)),
                MDF_ModifierType_Montreal.GAO_ModifierRope => s.SerializeObject<GAO_ModifierRope>((GAO_ModifierRope)Modifier, name: nameof(Modifier)),
                MDF_ModifierType_Montreal.GAO_ModifierVirtualAnim => s.SerializeObject<GAO_ModifierVirtualAnim>((GAO_ModifierVirtualAnim)Modifier, name: nameof(Modifier)),
                MDF_ModifierType_Montreal.GAO_ModifierCharacterFX => s.SerializeObject<GAO_ModifierCharacterFX>((GAO_ModifierCharacterFX)Modifier, name: nameof(Modifier)),
                MDF_ModifierType_Montreal.GAO_ModifierVoiceManager => s.SerializeObject<GAO_ModifierVoiceManager>((GAO_ModifierVoiceManager)Modifier, name: nameof(Modifier)),
                MDF_ModifierType_Montreal.Ambiance_Modifier => s.SerializeObject<Ambiance_Modifier>((Ambiance_Modifier)Modifier, name: nameof(Modifier)),
                MDF_ModifierType_Montreal.GAO_ModifierSectorisationElement => s.SerializeObject<GAO_ModifierSectorisationElement>((GAO_ModifierSectorisationElement)Modifier, name: nameof(Modifier)),
                _ => throw new NotImplementedException($"TODO: Implement Modifier Type {Type_Montreal}")
            };
        }
    }
}
