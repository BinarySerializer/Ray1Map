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
		public uint UInt_00 { get; set; }
		public MDF_Modifier Modifier { get; set; }

		public override void SerializeImpl(SerializerObject s) {
            if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal)) {
                SerializeImpl_Montreal(s);
                return;
            }

			Type = s.Serialize<MDF_ModifierType>(Type, name: nameof(Type));
			if(Type != MDF_ModifierType.None) UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));

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
                MDF_ModifierType.MDF_ModifierSFX => s.SerializeObject<MDF_ModifierSFX>((MDF_ModifierSFX)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.MDF_LoadingSound => s.SerializeObject<MDF_LoadingSound>((MDF_LoadingSound)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.MDF_ModifierDisturber => s.SerializeObject<MDF_ModifierDisturber>((MDF_ModifierDisturber)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.MDF_ModifierWeather => s.SerializeObject<MDF_ModifierWeather>((MDF_ModifierWeather)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.MDF_ModifierWATER3D => s.SerializeObject<MDF_ModifierWATER3D>((MDF_ModifierWATER3D)Modifier, name: nameof(Modifier)),
                _ => throw new NotImplementedException($"TODO: Implement Modifier Type {Type}")
            };
        }


        public void SerializeImpl_Montreal(SerializerObject s) {
            Type_Montreal = s.Serialize<MDF_ModifierType_Montreal>(Type_Montreal, name: nameof(Type_Montreal));
            if (Type_Montreal != MDF_ModifierType_Montreal.None) UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));

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
                MDF_ModifierType_Montreal.Disturber_Modifier => s.SerializeObject<MDF_ModifierDisturber>((MDF_ModifierDisturber)Modifier, name: nameof(Modifier)),
                MDF_ModifierType_Montreal.GAO_ModifierBeamGen => s.SerializeObject<GAO_ModifierBeamGen>((GAO_ModifierBeamGen)Modifier, name: nameof(Modifier)),
                _ => throw new NotImplementedException($"TODO: Implement Modifier Type {Type_Montreal}")
            };
        }
    }
}
