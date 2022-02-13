using BinarySerializer;

namespace Ray1Map.Jade {
	public class GAO_ModifierSemiLookAt : MDF_Modifier {
		public uint UInt_Editor_00 { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			if(!Loader.IsBinaryData) UInt_Editor_00 = s.Serialize<uint>(UInt_Editor_00, name: nameof(UInt_Editor_00));
		}
	}
}
