using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace R1Engine.Jade {
	public class GEO_ModifierSymetrie : MDF_Modifier {
		public uint UInt_00_Editor { get; set; }
		public uint UInt_04 { get; set; }
		public float Float_08 { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			if(!Loader.IsBinaryData) UInt_00_Editor = s.Serialize<uint>(UInt_00_Editor, name: nameof(UInt_00_Editor));
			UInt_04 = s.Serialize<uint>(UInt_04, name: nameof(UInt_04));
			Float_08 = s.Serialize<float>(Float_08, name: nameof(Float_08));
		}
	}
}
