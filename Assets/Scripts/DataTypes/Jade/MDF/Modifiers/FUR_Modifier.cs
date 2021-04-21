using BinarySerializer;

namespace R1Engine.Jade {
	public class FUR_Modifier : MDF_Modifier {
		public uint Type { get; set; }
		public float Float_04 { get; set; }
		public float Float_08 { get; set; }
		public float Float_0C { get; set; }
		public uint UInt_10 { get; set; }

		public uint Type13_UInt_0 { get; set; }
		public uint Type13_UInt_1 { get; set; }
		public uint Type21_UInt_0 { get; set; }
		public float Type25_Float_0 { get; set; }
		public float Type25_Float_1 { get; set; }


		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			if (s.GetR1Settings().EngineVersion >= EngineVersion.Jade_RRR || !Loader.IsBinaryData)
				Type = s.Serialize<uint>(Type, name: nameof(Type));
			Float_04 = s.Serialize<float>(Float_04, name: nameof(Float_04));
			Float_08 = s.Serialize<float>(Float_08, name: nameof(Float_08));
			Float_0C = s.Serialize<float>(Float_0C, name: nameof(Float_0C));
			UInt_10 = s.Serialize<uint>(UInt_10, name: nameof(UInt_10));
			if (Type >= 13) {
				Type13_UInt_0 = s.Serialize<uint>(Type13_UInt_0, name: nameof(Type13_UInt_0));
				Type13_UInt_1 = s.Serialize<uint>(Type13_UInt_1, name: nameof(Type13_UInt_1));
			}
			if (Type >= 21) Type21_UInt_0 = s.Serialize<uint>(Type21_UInt_0, name: nameof(Type21_UInt_0));
			if (Type >= 25) {
				Type25_Float_0 = s.Serialize<float>(Type25_Float_0, name: nameof(Type25_Float_0));
				Type25_Float_1 = s.Serialize<float>(Type25_Float_1, name: nameof(Type25_Float_1));
			}
		}
	}
}
