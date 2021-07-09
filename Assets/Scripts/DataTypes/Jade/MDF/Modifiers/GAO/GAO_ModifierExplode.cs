using BinarySerializer;

namespace R1Engine.Jade {
	public class GAO_ModifierExplode : MDF_Modifier {
		public uint Version { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
			if (!Loader.IsBinaryData) Version = s.Serialize<uint>(Version, name: nameof(Version));
		}
	}
}
