using BinarySerializer;

namespace Ray1Map.Jade {
	public class MDF_AlphaFade : MDF_Modifier {
		public uint Version { get; set; }
		public bool DistanceBased { get; set; }
		public float StartFadeDistance { get; set; }
		public float EndFadeDistance { get; set; }
		public uint TVP_UInt_00 { get; set; }
		public uint TVP_UInt_04 { get; set; }
		public float FadeValue { get; set; }
		public uint InvertDir { get; set; }
		public uint V3_UInt { get; set; }
		public bool UseLineEvents { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Version = s.Serialize<uint>(Version, name: nameof(Version));
			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PoP_TFS)) {
				if (Version >= 2) DistanceBased = s.Serialize<bool>(DistanceBased, name: nameof(DistanceBased));
			}
			StartFadeDistance = s.Serialize<float>(StartFadeDistance, name: nameof(StartFadeDistance));
			EndFadeDistance = s.Serialize<float>(EndFadeDistance, name: nameof(EndFadeDistance));
			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_RRRTVParty)) {
				TVP_UInt_00 = s.Serialize<uint>(TVP_UInt_00, name: nameof(TVP_UInt_00));
				TVP_UInt_04 = s.Serialize<uint>(TVP_UInt_04, name: nameof(TVP_UInt_04));
			} else if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PoP_TFS)) {
				if (Version >= 2) {
					FadeValue = s.Serialize<float>(FadeValue, name: nameof(FadeValue));
					InvertDir = s.Serialize<uint>(InvertDir, name: nameof(InvertDir));
				}
				if (Version >= 3) V3_UInt = s.Serialize<uint>(V3_UInt, name: nameof(V3_UInt));
				if (Version >= 4) UseLineEvents = s.Serialize<bool>(UseLineEvents, name: nameof(UseLineEvents));
			}
		}
	}
}
