using BinarySerializer;
using System;

namespace Ray1Map.Jade {
	public class AI_Trigger : BinarySerializable {
		public Jade_Key KeyFile { get; set; }
		public int CFunctionPointer { get; set; }
		public string Name { get; set; }
		public AI_Message Message { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			KeyFile = s.SerializeObject<Jade_Key>(KeyFile, name: nameof(KeyFile));
			CFunctionPointer = s.Serialize<int>(CFunctionPointer, name: nameof(CFunctionPointer));
			Name = s.SerializeString(Name, length: 64, encoding: Jade_BaseManager.Encoding, name: nameof(Name));
			Message = s.SerializeObject<AI_Message>(Message, name: nameof(Message));
		}

		public override string ToString() {
			return $"Trigger({KeyFile}, {Name}, {Message})";
		}
	}
}
