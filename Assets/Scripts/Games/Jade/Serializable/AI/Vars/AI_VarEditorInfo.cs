using System.Text;
using BinarySerializer;

namespace Ray1Map.Jade {
	public class AI_VarEditorInfo : BinarySerializable {
		public int BufferOffset { get; set; }
		public uint PointerSelectionString { get; set; }
		public uint PointerDescription { get; set; }
		public ushort Flags { get; set; }
		public ushort P1 { get; set; }
		public int P2 { get; set; }

		public int Editor_Index { get; set; }

		public string SelectionString { get; set; } // StringCst
		public string Description { get; set; } // StringHelp

		public override void SerializeImpl(SerializerObject s) {
			BufferOffset = s.Serialize<int>(BufferOffset, name: nameof(BufferOffset));
			PointerSelectionString = s.Serialize<uint>(PointerSelectionString, name: nameof(PointerSelectionString));
			PointerDescription = s.Serialize<uint>(PointerDescription, name: nameof(PointerDescription));
			Flags = s.Serialize<ushort>(Flags, name: nameof(Flags));
			P1 = s.Serialize<ushort>(P1, name: nameof(P1));
			P2 = s.Serialize<int>(P2, name: nameof(P2));
			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal)) {
				Editor_Index = s.Serialize<int>(Editor_Index, name: nameof(Editor_Index));
			}
		}

		public void SerializeStrings(SerializerObject s) {
			if (PointerSelectionString != 0)
				SelectionString = s.SerializeString(SelectionString, encoding: Jade_BaseManager.Encoding, name: nameof(SelectionString));
			if (PointerDescription != 0)
				Description = s.SerializeString(Description, encoding: Jade_BaseManager.Encoding, name: nameof(Description));
		}
	}
}
