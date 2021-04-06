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
                MDF_ModifierType.GEN_ModifierSound => s.SerializeObject<GEN_ModifierSound>((GEN_ModifierSound)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.GAO_ModifierShadow => s.SerializeObject<GAO_ModifierShadow>((GAO_ModifierShadow)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.PROTEX_Modifier => s.SerializeObject<PROTEX_Modifier>((PROTEX_Modifier)Modifier, name: nameof(Modifier)),
                MDF_ModifierType.GAO_ModifierLazy => s.SerializeObject<GAO_ModifierLazy>((GAO_ModifierLazy)Modifier, name: nameof(Modifier)),
                _ => throw new NotImplementedException($"TODO: Implement Modifier Type {Type}")
            };
        }
	}
}
