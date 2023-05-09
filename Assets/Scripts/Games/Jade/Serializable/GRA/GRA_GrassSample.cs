using BinarySerializer;

namespace Ray1Map.Jade {
	// Found in GRA_GrassField_CreateFromBuffer
	public class GRA_GrassSample : BinarySerializable {
		public uint Pre_ObjectVersion { get; set; }

		public uint Bits { get; set; }
		public uint Bits2 { get; set; }
		public Jade_Vector Normal { get; set; }

		public uint Editor_UInt0 { get; set; }
		public float Editor_Float1 { get; set; }
		public uint Editor_UInt2 { get; set; }
		public uint Editor_UInt3 { get; set; }
		public uint Editor_UInt4 { get; set; }
		public Jade_Vector Editor_Vector5 { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Bits = s.Serialize<uint>(Bits, name: nameof(Bits));
			if(Pre_ObjectVersion >= 5)
				Bits2 = s.Serialize<uint>(Bits2, name: nameof(Bits2));
			else
				Normal = s.SerializeObject<Jade_Vector>(Normal, name: nameof(Normal));
		}

		public void SerializeEditorData(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
			if (!Loader.IsBinaryData) {
				Editor_UInt0 = s.Serialize<uint>(Editor_UInt0, name: nameof(Editor_UInt0));
				Editor_Float1 = s.Serialize<float>(Editor_Float1, name: nameof(Editor_Float1));
				Editor_UInt2 = s.Serialize<uint>(Editor_UInt2, name: nameof(Editor_UInt2));
				Editor_UInt3 = s.Serialize<uint>(Editor_UInt3, name: nameof(Editor_UInt3));
				Editor_UInt4 = s.Serialize<uint>(Editor_UInt4, name: nameof(Editor_UInt4));
				Editor_Vector5 = s.SerializeObject<Jade_Vector>(Editor_Vector5, name: nameof(Editor_Vector5));
			}
		}
	}
}
