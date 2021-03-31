using BinarySerializer;

namespace R1Engine.Jade {
	public class AI_Node_Unknown : BinarySerializable {
		public int Index { get; set; }
		public int Key { get; set; }

		public bool IsNull => Index == -1 && Key == -1;

		public override void SerializeImpl(SerializerObject s) {
			Index = s.Serialize<int>(Index, name: nameof(Index));
			Key = s.Serialize<int>(Key, name: nameof(Key));
		}
	}
}
