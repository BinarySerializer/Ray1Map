using BinarySerializer;

namespace R1Engine.Jade {
	public class FUR_Modifier : MDF_Modifier {
		public uint Version { get; set; }
		public float NormalOffset { get; set; }
		public float UOffset { get; set; }
		public float VOffset { get; set; }
		public uint LayersCount { get; set; }

		public uint Type13_UInt_0 { get; set; }
		public uint Type13_UInt_1 { get; set; }
		public uint Type21_UInt_0 { get; set; }
		public float Type25_Float_0 { get; set; }
		public float Type25_Float_1 { get; set; }


		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_RRR) || !Loader.IsBinaryData)
				Version = s.Serialize<uint>(Version, name: nameof(Version));

			NormalOffset = s.Serialize<float>(NormalOffset, name: nameof(NormalOffset));
			UOffset = s.Serialize<float>(UOffset, name: nameof(UOffset));
			VOffset = s.Serialize<float>(VOffset, name: nameof(VOffset));
			LayersCount = s.Serialize<uint>(LayersCount, name: nameof(LayersCount));

			if (Version >= 13) {
				Type13_UInt_0 = s.Serialize<uint>(Type13_UInt_0, name: nameof(Type13_UInt_0));
				Type13_UInt_1 = s.Serialize<uint>(Type13_UInt_1, name: nameof(Type13_UInt_1));
			}
			if (Version >= 21) Type21_UInt_0 = s.Serialize<uint>(Type21_UInt_0, name: nameof(Type21_UInt_0));
			if (Version >= 25) {
				Type25_Float_0 = s.Serialize<float>(Type25_Float_0, name: nameof(Type25_Float_0));
				Type25_Float_1 = s.Serialize<float>(Type25_Float_1, name: nameof(Type25_Float_1));
			}
		}
	}
}
