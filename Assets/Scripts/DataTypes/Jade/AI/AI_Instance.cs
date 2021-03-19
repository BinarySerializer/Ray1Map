using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R1Engine.Jade {
	public class AI_Instance : Jade_File {
		public Jade_Reference<AI_Model> ModelKey { get; set; }
		public Jade_Reference<AI_Vars> VarsKey { get; set; }

		public AI_Model Model { get; set; } 

		public override void SerializeImpl(SerializerObject s) {
			ModelKey = s.SerializeObject<Jade_Reference<AI_Model>>(ModelKey, name: nameof(ModelKey));
			ModelKey.Resolve();
			VarsKey = s.SerializeObject<Jade_Reference<AI_Vars>>(VarsKey, name: nameof(VarsKey));
			VarsKey.Resolve();
		}
	}
}
