using BinarySerializer;

namespace R1Engine.Jade {
	public class FUR_ModifierDynFur : MDF_Modifier {
		public uint UInt_00_Editor { get; set; }
		public uint UInt_00 { get; set; }
		public float Float_04 { get; set; }
		public float Float_08 { get; set; }
		public float Float_0C { get; set; }
		public uint UInt_10 { get; set; }
		public float Float_14 { get; set; }
		public float Float_18 { get; set; }
		public float Float_1C { get; set; }
		public float Float_20 { get; set; }
		public float Float_24 { get; set; }


		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			if (!Loader.IsBinaryData) UInt_00_Editor = s.Serialize<uint>(UInt_00_Editor, name: nameof(UInt_00_Editor));
			UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
			Float_04 = s.Serialize<float>(Float_04, name: nameof(Float_04));
			Float_08 = s.Serialize<float>(Float_08, name: nameof(Float_08));
			Float_0C = s.Serialize<float>(Float_0C, name: nameof(Float_0C));
			UInt_10 = s.Serialize<uint>(UInt_10, name: nameof(UInt_10));
			Float_14 = s.Serialize<float>(Float_14, name: nameof(Float_14));
			Float_18 = s.Serialize<float>(Float_18, name: nameof(Float_18));
			Float_1C = s.Serialize<float>(Float_1C, name: nameof(Float_1C));
			Float_20 = s.Serialize<float>(Float_20, name: nameof(Float_20));
			if (UInt_00 > 1) Float_24 = s.Serialize<float>(Float_24, name: nameof(Float_24));
		}
	}
}
