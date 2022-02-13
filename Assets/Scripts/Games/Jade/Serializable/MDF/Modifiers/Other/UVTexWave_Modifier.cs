using BinarySerializer;

namespace Ray1Map.Jade {
	// Found in UVTexWave_Modifier_Load
	public class UVTexWave_Modifier : MDF_Modifier {
		public uint UInt_Editor_00 { get; set; }
		public uint UInt_00 { get; set; }
		public float Float_04 { get; set; }
		public float Float_08 { get; set; }
		public float Float_0C { get; set; }
		public float Float_10 { get; set; }
		public float Phoenix_Float_14 { get; set; }
		public uint Phoenix_UInt_18 { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			if(!Loader.IsBinaryData) UInt_Editor_00 = s.Serialize<uint>(UInt_Editor_00, name: nameof(UInt_Editor_00));
			UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
			Float_04 = s.Serialize<float>(Float_04, name: nameof(Float_04));
			Float_08 = s.Serialize<float>(Float_08, name: nameof(Float_08));
			Float_0C = s.Serialize<float>(Float_0C, name: nameof(Float_0C));
			Float_10 = s.Serialize<float>(Float_10, name: nameof(Float_10));

			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PetzHorseClub)) {
				Phoenix_Float_14 = s.Serialize<float>(Phoenix_Float_14, name: nameof(Phoenix_Float_14));
				Phoenix_UInt_18 = s.Serialize<uint>(Phoenix_UInt_18, name: nameof(Phoenix_UInt_18));
			}
		}
	}
}
