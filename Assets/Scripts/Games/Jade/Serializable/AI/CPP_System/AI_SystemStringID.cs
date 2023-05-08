using BinarySerializer;

namespace Ray1Map.Jade {
	public class AI_SystemStringID : BinarySerializable {
		public AI_System Pre_System { get; set; }

		public int NameDebugStringId { get; set; }
		public uint NameCrc32 { get; set; }

		public string Name { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if (Pre_System.Version != 0) NameCrc32 = s.Serialize<uint>(NameCrc32, name: nameof(NameCrc32));
			NameDebugStringId = s.Serialize<int>(NameDebugStringId, name: nameof(NameDebugStringId));

			if (Pre_System.Version != 0 && NameDebugStringId >= 0) {
				if (Pre_System.DebugStringPool != null) {
					s.DoAt(Pre_System.DebugStringPool.Offset + 8 + NameDebugStringId, () => {
						Name = s.SerializeString(Name, name: nameof(Name));
					});
				}
			}
		}
	}
}
