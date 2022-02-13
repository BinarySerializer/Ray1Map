using BinarySerializer;

namespace Ray1Map.Jade {
	public class GAO_ModifierROTR : MDF_Modifier {
		public uint UInt_Editor_00 { get; set; }
		public int Int1 { get; set; }
		public float RetardX { get; set; } // Delay in French
		public float RetardY { get; set; }
		public float RetardZ { get; set; }
		public float Float3 { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			if(!Loader.IsBinaryData) UInt_Editor_00 = s.Serialize<uint>(UInt_Editor_00, name: nameof(UInt_Editor_00));
			Int1 = s.Serialize<int>(Int1, name: nameof(Int1));
			RetardX = s.Serialize<float>(RetardX, name: nameof(RetardX));
			RetardY = s.Serialize<float>(RetardY, name: nameof(RetardY));
			RetardZ = s.Serialize<float>(RetardZ, name: nameof(RetardZ));
			Float3 = s.Serialize<float>(Float3, name: nameof(Float3));
		}
	}
}
