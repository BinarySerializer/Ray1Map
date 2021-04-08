using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace R1Engine.Jade {
	// Found in GEO_ul_ModifierOnduleTonCorps_Load
	public class GEO_ModifierOnduleTonCorps : MDF_Modifier {
		public uint UInt_Editor_00 { get; set; }
		public uint UInt_00 { get; set; }
		public float Float_04 { get; set; }
		public float Float_08 { get; set; }
		public float Float_0C { get; set; }
		public float Float_10 { get; set; }
		public uint UInt_Editor_14 { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			if(!Loader.IsBinaryData) UInt_Editor_00 = s.Serialize<uint>(UInt_Editor_00, name: nameof(UInt_Editor_00));
			UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
			Float_04 = s.Serialize<float>(Float_04, name: nameof(Float_04));
			Float_08 = s.Serialize<float>(Float_08, name: nameof(Float_08));
			Float_0C = s.Serialize<float>(Float_0C, name: nameof(Float_0C));
			Float_10 = s.Serialize<float>(Float_10, name: nameof(Float_10));
			if (!Loader.IsBinaryData) UInt_Editor_14 = s.Serialize<uint>(UInt_Editor_14, name: nameof(UInt_Editor_14));
		}
	}
}
