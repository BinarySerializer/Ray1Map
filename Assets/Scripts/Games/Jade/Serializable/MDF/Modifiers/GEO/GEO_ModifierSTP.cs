using BinarySerializer;

namespace Ray1Map.Jade {
	public class GEO_ModifierSTP : MDF_Modifier { // StoreTransformedPoint
		public uint UInt_Editor_00 { get; set; }
		public uint PointsCount { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			if(!Loader.IsBinaryData) UInt_Editor_00 = s.Serialize<uint>(UInt_Editor_00, name: nameof(UInt_Editor_00));
			PointsCount = s.Serialize<uint>(PointsCount, name: nameof(PointsCount));
		}
	}
}
