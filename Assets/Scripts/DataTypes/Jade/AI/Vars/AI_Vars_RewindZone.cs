using BinarySerializer;

namespace R1Engine.Jade {
	public class AI_Vars_RewindZone : BinarySerializable {
		public int BufferOffset { get; set; }
		public int BufferLength { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			BufferOffset = s.Serialize<int>(BufferOffset, name: nameof(BufferOffset));
			BufferLength = s.Serialize<int>(BufferLength, name: nameof(BufferLength));
		}
	}
}
