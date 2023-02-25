using BinarySerializer;

namespace Ray1Map.Jade {
	public class FUR_Modifier : MDF_Modifier {
		public uint DataSize { get; set; }
		public float NormalOffset { get; set; }
		public float UOffset { get; set; }
		public float VOffset { get; set; }
		public uint LayersCount { get; set; }

		public uint FurFlags { get; set; }
		public uint ExLineColor { get; set; }
		public uint FurNearLod { get; set; }
		public float Near { get; set; }
		public float Far { get; set; }


		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_RRRPrototype) || !Loader.IsBinaryData)
				DataSize = s.Serialize<uint>(DataSize, name: nameof(DataSize));

			NormalOffset = s.Serialize<float>(NormalOffset, name: nameof(NormalOffset));
			UOffset = s.Serialize<float>(UOffset, name: nameof(UOffset));
			VOffset = s.Serialize<float>(VOffset, name: nameof(VOffset));
			LayersCount = s.Serialize<uint>(LayersCount, name: nameof(LayersCount));

			if (DataSize > 12) {
				FurFlags = s.Serialize<uint>(FurFlags, name: nameof(FurFlags));
				ExLineColor = s.Serialize<uint>(ExLineColor, name: nameof(ExLineColor));
			}
			if (DataSize >= 21) FurNearLod = s.Serialize<uint>(FurNearLod, name: nameof(FurNearLod));
			if (DataSize >= 25) {
				Near = s.Serialize<float>(Near, name: nameof(Near));
				Far = s.Serialize<float>(Far, name: nameof(Far));
			}
		}
	}
}
