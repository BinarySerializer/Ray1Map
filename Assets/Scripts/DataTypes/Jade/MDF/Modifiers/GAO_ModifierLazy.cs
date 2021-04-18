using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace R1Engine.Jade {
	public class GAO_ModifierLazy : MDF_Modifier {
		public uint UInt_Editor_00 { get; set; }
		public uint Type { get; set; }
		public float Float_04 { get; set; }
		public float Float_08 { get; set; }
		public uint UInt_08_BGE { get; set; }
		public float Type1_Float_0 { get; set; }
		public float Type1_Float_1 { get; set; }
		public float Type1_Float_2 { get; set; }
		public float Type1_Float_3 { get; set; }
		public float Type1_Float_4 { get; set; }
		public float Type1_Float_5 { get; set; }
		public float Type2_Float { get; set; }
		public float Type3_Float { get; set; }


		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			if(!Loader.IsBinaryData) UInt_Editor_00 = s.Serialize<uint>(UInt_Editor_00, name: nameof(UInt_Editor_00));
			Type = s.Serialize<uint>(Type, name: nameof(Type));
			Float_04 = s.Serialize<float>(Float_04, name: nameof(Float_04));
			if (s.GetR1Settings().Game == Game.Jade_BGE) {
				UInt_08_BGE = s.Serialize<uint>(UInt_08_BGE, name: nameof(UInt_08_BGE));
			} else {
				Float_08 = s.Serialize<float>(Float_08, name: nameof(Float_08));
			}
			if (Type >= 1) {
				Type1_Float_0 = s.Serialize<float>(Type1_Float_0, name: nameof(Type1_Float_0));
				Type1_Float_1 = s.Serialize<float>(Type1_Float_1, name: nameof(Type1_Float_1));
				Type1_Float_2 = s.Serialize<float>(Type1_Float_2, name: nameof(Type1_Float_2));
				Type1_Float_3 = s.Serialize<float>(Type1_Float_3, name: nameof(Type1_Float_3));
				Type1_Float_4 = s.Serialize<float>(Type1_Float_4, name: nameof(Type1_Float_4));
				Type1_Float_5 = s.Serialize<float>(Type1_Float_5, name: nameof(Type1_Float_5));
			}
			if (Type >= 2) Type2_Float = s.Serialize<float>(Type2_Float, name: nameof(Type2_Float));
			if (Type >= 3) Type3_Float = s.Serialize<float>(Type3_Float, name: nameof(Type3_Float));
		}
	}
}
