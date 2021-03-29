using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R1Engine.Jade {
	public class OBJ_GameObject_Modifier : R1Serializable {
		public MDF_ModifierType Type { get; set; }
		public uint UInt_00 { get; set; }
		public MDF_Modifier Modifier { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Type = s.Serialize<MDF_ModifierType>(Type, name: nameof(Type));
			if(Type != MDF_ModifierType.None) UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
			switch (Type) {
				case MDF_ModifierType.None:
					break;
				case MDF_ModifierType.GEO_ModifierRLICarte:
					Modifier = s.SerializeObject<GEO_ModifierRLICarte>((GEO_ModifierRLICarte)Modifier, name: nameof(Modifier));
					break;
				case MDF_ModifierType.GAO_ModifierXMEN:
					Modifier = s.SerializeObject<GAO_ModifierXMEN>((GAO_ModifierXMEN)Modifier, name: nameof(Modifier));
					break;
				default:
					throw new NotImplementedException($"TODO: Implement Modifier Type {Type}");
			}
		}
	}
}
