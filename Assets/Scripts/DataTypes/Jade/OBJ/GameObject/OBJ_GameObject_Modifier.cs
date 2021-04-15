using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace R1Engine.Jade {
	public class OBJ_GameObject_Modifier : BinarySerializable {
		public MDF_ModifierType Type { get; set; }
		public uint UInt_00 { get; set; }
		public MDF_Modifier Modifier { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Type = s.Serialize<MDF_ModifierType>(Type, name: nameof(Type));
			if(Type != MDF_ModifierType.None) UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));

            Modifier = Type switch
            {
                MDF_ModifierType.None => null,
                MDF_ModifierType.GEO_ModifierRLICarte => s.SerializeObject<GEO_ModifierRLICarte>((GEO_ModifierRLICarte)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.GAO_ModifierXMEN => s.SerializeObject<GAO_ModifierXMEN>((GAO_ModifierXMEN)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.MPAG_Modifier => s.SerializeObject<MPAG_Modifier>((MPAG_Modifier)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.GEO_ModifierMorphing => s.SerializeObject<GEO_ModifierMorphing>((GEO_ModifierMorphing)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.GEO_ModifierSnap => s.SerializeObject<GEO_ModifierSnap>((GEO_ModifierSnap)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.GEN_ModifierSound => s.SerializeObject<GEN_ModifierSound>((GEN_ModifierSound)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.GEN_ModifierSoundFx => s.SerializeObject<GEN_ModifierSoundFx>((GEN_ModifierSoundFx)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.GAO_ModifierShadow => s.SerializeObject<GAO_ModifierShadow>((GAO_ModifierShadow)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.PROTEX_Modifier => s.SerializeObject<PROTEX_Modifier>((PROTEX_Modifier)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.GAO_ModifierLazy => s.SerializeObject<GAO_ModifierLazy>((GAO_ModifierLazy)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.GAO_ModifierAnimatedGAO => s.SerializeObject<GAO_ModifierAnimatedGAO>((GAO_ModifierAnimatedGAO)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.GAO_ModifierSemiLookAt => s.SerializeObject<GAO_ModifierSemiLookAt>((GAO_ModifierSemiLookAt)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.GAO_ModifierSNAKE => s.SerializeObject<GAO_ModifierSNAKE>((GAO_ModifierSNAKE)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.GAO_ModifierPhoto => s.SerializeObject<GAO_ModifierPhoto>((GAO_ModifierPhoto)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.GAO_ModifierSpecialLookAt => s.SerializeObject<GAO_ModifierSpecialLookAt>((GAO_ModifierSpecialLookAt)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.GEO_ModifierSymetrie => s.SerializeObject<GEO_ModifierSymetrie>((GEO_ModifierSymetrie)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.SPG_Modifier => s.SerializeObject<SPG_Modifier>((SPG_Modifier)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.SPG2_Modifier => s.SerializeObject<SPG2_Modifier>((SPG2_Modifier)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.FUR_Modifier => s.SerializeObject<FUR_Modifier>((FUR_Modifier)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.GEO_ModifierOnduleTonCorps => s.SerializeObject<GEO_ModifierOnduleTonCorps>((GEO_ModifierOnduleTonCorps)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.UVTexWave_Modifier => s.SerializeObject<UVTexWave_Modifier>((UVTexWave_Modifier)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.SND_ModifierSoundVol => s.SerializeObject<SND_ModifierSoundVol>((SND_ModifierSoundVol)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.MDF_ModifierSFX => s.SerializeObject<MDF_ModifierSFX>((MDF_ModifierSFX)Modifier, name: nameof(Modifier)),
                _ => throw new NotImplementedException($"TODO: Implement Modifier Type {Type}")
            };
        }
	}
}
