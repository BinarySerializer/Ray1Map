using BinarySerializer;

namespace Ray1Map.Jade {
	public class BIG_PakFileTableEntry : BinarySerializable {
		public bool IsKeyID { get; set; }

		public Jade_Key Key { get; set; }
		public uint NameLength { get; set; }
		public string Name { get; set; }

		public BIG_PakFileInfo Info { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			IsKeyID = s.Serialize<bool>(IsKeyID, name: nameof(IsKeyID));
			if (IsKeyID) {
				Key = s.SerializeObject<Jade_Key>(Key, name: nameof(Key));
			} else {
				NameLength = s.Serialize<uint>(NameLength, name: nameof(NameLength));
				Name = s.SerializeString(Name, length: NameLength, name: nameof(Name));
			}
			Info = s.SerializeObject<BIG_PakFileInfo>(Info, name: nameof(Info));
		}
	}
}
