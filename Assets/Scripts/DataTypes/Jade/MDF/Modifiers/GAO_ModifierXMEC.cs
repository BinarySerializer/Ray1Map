using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace R1Engine.Jade {
	public class GAO_ModifierXMEC : MDF_Modifier {
		public uint UInt_Editor_00 { get; set; }
		public uint Type { get; set; }
		public float Float_08 { get; set; }
		public float Float_0C { get; set; }
		public float Float_10 { get; set; }
		public float Float_14 { get; set; }
		public float Float_18 { get; set; }
		public float Float_1C { get; set; }
		public uint UInt_20 { get; set; }
		public uint UInt_24 { get; set; }
		public uint UInt_28 { get; set; }
		public int[] Ints { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			if(!Loader.IsBinaryData) UInt_Editor_00 = s.Serialize<uint>(UInt_Editor_00, name: nameof(UInt_Editor_00));
			Type = s.Serialize<uint>(Type, name: nameof(Type));
			if (Type >= 1) {
				Float_08 = s.Serialize<float>(Float_08, name: nameof(Float_08));
				Float_0C = s.Serialize<float>(Float_0C, name: nameof(Float_0C));
				Float_10 = s.Serialize<float>(Float_10, name: nameof(Float_10));
				Float_14 = s.Serialize<float>(Float_14, name: nameof(Float_14));
				Float_18 = s.Serialize<float>(Float_18, name: nameof(Float_18));
				Float_1C = s.Serialize<float>(Float_1C, name: nameof(Float_1C));
				UInt_20 = s.Serialize<uint>(UInt_20, name: nameof(UInt_20));
				UInt_24 = s.Serialize<uint>(UInt_24, name: nameof(UInt_24));
				UInt_28 = s.Serialize<uint>(UInt_28, name: nameof(UInt_28));
				Ints = s.SerializeArray<int>(Ints, 50, name: nameof(Ints));
			}
		}
	}
}
