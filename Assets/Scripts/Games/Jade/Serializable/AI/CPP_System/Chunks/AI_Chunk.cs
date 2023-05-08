using BinarySerializer;

namespace Ray1Map.Jade {
	public abstract class AI_Chunk : BinarySerializable {
		public AI_System Pre_System { get; set; }
		public int Version => Pre_System.Version;

		public int ObjectID { get; set; }
		public AI_SystemStringID Name { get; set; }
		public uint V4_UInt_00 { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			ObjectID = s.Serialize<int>(ObjectID, name: nameof(ObjectID));
			Name = s.SerializeObject<AI_SystemStringID>(Name, n => n.Pre_System = Pre_System, name: nameof(Name));
			if (Version >= 4) V4_UInt_00 = s.Serialize<uint>(V4_UInt_00, name: nameof(V4_UInt_00));
		}
	}
}
