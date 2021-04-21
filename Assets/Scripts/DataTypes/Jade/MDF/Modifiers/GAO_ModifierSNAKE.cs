using BinarySerializer;

namespace R1Engine.Jade {
	public class GAO_ModifierSNAKE : MDF_Modifier {
		public uint Type { get; set; }
		public uint UInt_04 { get; set; }
		public float Float_08 { get; set; }
		public float Float_0C { get; set; }
		public float Float_10 { get; set; }
		public uint[] UInts_14 { get; set; }
		public uint Type9_UInt { get; set; }


		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
			if (s.GetR1Settings().EngineVersion >= EngineVersion.Jade_KingKong || !Loader.IsBinaryData) {
				Type = s.Serialize<uint>(Type, name: nameof(Type));
			}
			UInt_04 = s.Serialize<uint>(UInt_04, name: nameof(UInt_04));
			Float_08 = s.Serialize<float>(Float_08, name: nameof(Float_08));
			Float_0C = s.Serialize<float>(Float_0C, name: nameof(Float_0C));
			Float_10 = s.Serialize<float>(Float_10, name: nameof(Float_10));
			UInts_14 = s.SerializeArray<uint>(UInts_14, 20, name: nameof(UInts_14));

			if (Type >= 9) Type9_UInt = s.Serialize<uint>(Type9_UInt, name: nameof(Type9_UInt));
		}
	}
}
