using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace R1Engine.Jade {
	public class OBJ_GameObject_ActionData : BinarySerializable {
		public uint Action0 { get; set; }
		public uint Action1 { get; set; }
		public uint Action2 { get; set; }
		public Jade_Reference<ACT_ActionKit> ActionKit { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Action0 = s.Serialize<uint>(Action0, name: nameof(Action0));
			Action1 = s.Serialize<uint>(Action1, name: nameof(Action1));
			Action2 = s.Serialize<uint>(Action2, name: nameof(Action2));
			ActionKit = s.SerializeObject<Jade_Reference<ACT_ActionKit>>(ActionKit, name: nameof(ActionKit))?.Resolve();
		}
	}
}
