using BinarySerializer;

namespace R1Engine.Jade {
	public class FUR_Modifier : MDF_Modifier {
		public uint Version { get; set; }
		public float NormalOffset { get; set; }
		public float UOffset { get; set; }
		public float VOffset { get; set; }
		public uint LayersCount { get; set; }

		public uint FurFlags { get; set; }
		public uint ExLineColor { get; set; }
		public uint FurNearLod { get; set; }
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
				FurFlags = s.Serialize<uint>(FurFlags, name: nameof(FurFlags));
				ExLineColor = s.Serialize<uint>(ExLineColor, name: nameof(ExLineColor));
			}
			if (Version >= 21) FurNearLod = s.Serialize<uint>(FurNearLod, name: nameof(FurNearLod));
			if (Version >= 25) {
				Type25_Float_0 = s.Serialize<float>(Type25_Float_0, name: nameof(Type25_Float_0));
				Type25_Float_1 = s.Serialize<float>(Type25_Float_1, name: nameof(Type25_Float_1));
			}
		}
	}
}
