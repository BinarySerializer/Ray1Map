using BinarySerializer;

namespace R1Engine.Jade {
	public class SND_ModifierSoundVol : MDF_Modifier {
		public uint UInt_00 { get; set; }
		public uint UInt_04 { get; set; }
		public uint UInt_08 { get; set; }
		public float Float_0C { get; set; }
		public float Float_10 { get; set; }
		public float Float_14 { get; set; }
		public float Float_18 { get; set; }
		public float Float_1C { get; set; }
		public float Float_20 { get; set; }
		public float Float_24 { get; set; }
		public float Float_28 { get; set; }
		public float Float_2C_Editor { get; set; }
		public uint UInt_30_Editor { get; set; }
		public byte[] Bytes_34_Editor { get; set; }


		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
			UInt_04 = s.Serialize<uint>(UInt_04, name: nameof(UInt_04));
			UInt_08 = s.Serialize<uint>(UInt_08, name: nameof(UInt_08));
			Float_0C = s.Serialize<float>(Float_0C, name: nameof(Float_0C));
			Float_10 = s.Serialize<float>(Float_10, name: nameof(Float_10));
			Float_14 = s.Serialize<float>(Float_14, name: nameof(Float_14));
			Float_18 = s.Serialize<float>(Float_18, name: nameof(Float_18));
			Float_1C = s.Serialize<float>(Float_1C, name: nameof(Float_1C));
			Float_20 = s.Serialize<float>(Float_20, name: nameof(Float_20));
			Float_24 = s.Serialize<float>(Float_24, name: nameof(Float_24));
			Float_28 = s.Serialize<float>(Float_28, name: nameof(Float_28));

			if (!Loader.IsBinaryData) {
				Float_2C_Editor = s.Serialize<float>(Float_2C_Editor, name: nameof(Float_2C_Editor));
				UInt_30_Editor = s.Serialize<uint>(UInt_30_Editor, name: nameof(UInt_30_Editor));
				Bytes_34_Editor = s.SerializeArray<byte>(Bytes_34_Editor, 128, name: nameof(Bytes_34_Editor));
			}
		}
	}
}
