using BinarySerializer;

namespace Ray1Map.Jade {
	public class AI_VarName : BinarySerializable {
		public string Name { get; set; }
		public uint NameChecksum { get; set; } // CRC32
		public int Editor_Index { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PoP_SoT_20030819)) {
				LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
				if (Loader.IsBinaryData) {
					NameChecksum = s.Serialize<uint>(NameChecksum, name: nameof(NameChecksum));
					Name = $"Var_{NameChecksum:X8}";
				} else {
					Name = s.SerializeString(Name, 30, encoding: Jade_BaseManager.Encoding, name: nameof(Name));
					Editor_Index = s.Serialize<int>(Editor_Index, name: nameof(Editor_Index));
				}
			} else {
				Name = s.SerializeString(Name, 30, encoding: Jade_BaseManager.Encoding, name: nameof(Name));
			}
		}
	}
}
