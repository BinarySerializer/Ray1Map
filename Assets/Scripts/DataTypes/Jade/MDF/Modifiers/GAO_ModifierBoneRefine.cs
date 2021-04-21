using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace R1Engine.Jade {
	public class GAO_ModifierBoneRefine : MDF_Modifier {
		public uint UInt_00 { get; set; }
		public uint Version { get; set; }

		public uint V0_UInt_0 { get; set; }
		public uint V0_UInt_1 { get; set; }
		public uint V0_UInt_2 { get; set; }

		public uint V1_UInt_0 { get; set; }
		public float V3_Float_0 { get; set; }


		public override void SerializeImpl(SerializerObject s) {
			UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
			Version = s.Serialize<uint>(Version, name: nameof(Version));

			if (Version < 2) {
				V0_UInt_0 = s.Serialize<uint>(V0_UInt_0, name: nameof(V0_UInt_0));
				V0_UInt_1 = s.Serialize<uint>(V0_UInt_1, name: nameof(V0_UInt_1));
				V0_UInt_2 = s.Serialize<uint>(V0_UInt_2, name: nameof(V0_UInt_2));
			}
			if (Version >= 1) V1_UInt_0 = s.Serialize<uint>(V1_UInt_0, name: nameof(V1_UInt_0));
			if (Version >= 3) V3_Float_0 = s.Serialize<float>(V3_Float_0, name: nameof(V3_Float_0));
		}
	}
}
