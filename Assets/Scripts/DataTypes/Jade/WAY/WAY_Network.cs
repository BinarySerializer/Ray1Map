using System;
using BinarySerializer;

namespace R1Engine.Jade {
	public class WAY_Network : Jade_File {
		public Jade_Reference<OBJ_GameObject> GameObject { get; set; }
		public uint UInt_04 { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			GameObject = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(GameObject, name: nameof(GameObject))?.Resolve();
			UInt_04 = s.Serialize<uint>(UInt_04, name: nameof(UInt_04));
		}
	}
}
