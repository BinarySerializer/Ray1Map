using BinarySerializer;

namespace Ray1Map.Jade {
	public class AI_LocalStackFrame : BinarySerializable {
		public int NodeIndexEntry { get; set; }
		public int NodeIndexExit { get; set; }
		public int LocalVarCount { get; set; }
		public int FirstVarIndex { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			NodeIndexEntry = s.Serialize<int>(NodeIndexEntry, name: nameof(NodeIndexEntry));
			NodeIndexExit = s.Serialize<int>(NodeIndexExit, name: nameof(NodeIndexExit));
			LocalVarCount = s.Serialize<int>(LocalVarCount, name: nameof(LocalVarCount));
			FirstVarIndex = s.Serialize<int>(FirstVarIndex, name: nameof(FirstVarIndex));
		}
	}
}
