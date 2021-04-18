using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace R1Engine.Jade {
	// Found in GEO_ul_ModifierOnduleTonCorps_Load
	public class GAO_ModifierLegLink : MDF_Modifier {
		public uint UInt_00 { get; set; }
		public Jade_Reference<OBJ_GameObject> GameObject0 { get; set; }
		public Jade_Reference<OBJ_GameObject> GameObject1 { get; set; }
		public float Float_0C { get; set; }
		public float Float_10 { get; set; }
		public Jade_Reference<OBJ_GameObject> GameObject2 { get; set; }
		public uint Flags { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
			GameObject0 = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(GameObject0, name: nameof(GameObject0));
			GameObject1 = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(GameObject1, name: nameof(GameObject1));
			Float_0C = s.Serialize<float>(Float_0C, name: nameof(Float_0C));
			Float_10 = s.Serialize<float>(Float_10, name: nameof(Float_10));
			GameObject2 = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(GameObject2, name: nameof(GameObject2));
			Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
			if ((Flags & 0x10) == 0) {
				GameObject0?.Resolve();
				GameObject1?.Resolve();
				GameObject2?.Resolve();
			}
		}
	}
}
