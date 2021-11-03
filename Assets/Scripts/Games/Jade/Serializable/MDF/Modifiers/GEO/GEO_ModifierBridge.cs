using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace Ray1Map.Jade {
	public class GEO_ModifierBridge : MDF_Modifier {
		public uint Version { get; set; }
		public uint UInt_0 { get; set; }
		public uint UInt_1 { get; set; }
		public uint UInt_2 { get; set; }
		public uint UInt_3 { get; set; }
		public uint UInt_4 { get; set; }
		public Jade_Vector Vector_5 { get; set; }
		public Jade_Vector[] Vectors_1 { get; set; }
		public uint UInt_Editor_0 { get; set; }

		public uint Unknown0Count { get; set; }
		public Unknown[] Unknown0 { get; set; }
		public uint Unknown1Count { get; set; }
		public Unknown[] Unknown1 { get; set; }


		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			Version = s.Serialize<uint>(Version, name: nameof(Version));
			if (Version != 0) return;
			UInt_0 = s.Serialize<uint>(UInt_0, name: nameof(UInt_0));
			UInt_1 = s.Serialize<uint>(UInt_1, name: nameof(UInt_1));
			UInt_2 = s.Serialize<uint>(UInt_2, name: nameof(UInt_2));
			UInt_3 = s.Serialize<uint>(UInt_3, name: nameof(UInt_3));
			UInt_4 = s.Serialize<uint>(UInt_4, name: nameof(UInt_4));
			Vector_5 = s.SerializeObject<Jade_Vector>(Vector_5, name: nameof(Vector_5));
			Vectors_1 = s.SerializeObjectArray<Jade_Vector>(Vectors_1, UInt_1, name: nameof(Vectors_1));
			if (!Loader.IsBinaryData) UInt_Editor_0 = s.Serialize<uint>(UInt_Editor_0, name: nameof(UInt_Editor_0));

			Unknown0Count = s.Serialize<uint>(Unknown0Count, name: nameof(Unknown0Count));
			Unknown0 = s.SerializeObjectArray<Unknown>(Unknown0, Unknown0Count, name: nameof(Unknown0));
			Unknown1Count = s.Serialize<uint>(Unknown1Count, name: nameof(Unknown1Count));
			Unknown1 = s.SerializeObjectArray<Unknown>(Unknown1, Unknown1Count, name: nameof(Unknown1));

		}

		public class Unknown : BinarySerializable {
			public byte Byte_00 { get; set; }
			public float Float_01 { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				Byte_00 = s.Serialize<byte>(Byte_00, name: nameof(Byte_00));
				Float_01 = s.Serialize<float>(Float_01, name: nameof(Float_01));
			}
		}
	}
}
