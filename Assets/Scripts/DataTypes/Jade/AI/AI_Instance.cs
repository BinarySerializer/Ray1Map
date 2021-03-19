using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R1Engine.Jade {
	public class AI_Instance : Jade_File {
		public Jade_Reference<AI_Model> Model { get; set; }
		public Jade_Reference<AI_Vars> Vars { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Model = s.SerializeObject<Jade_Reference<AI_Model>>(Model, name: nameof(Model));
			Model.Resolve();
			Vars = s.SerializeObject<Jade_Reference<AI_Vars>>(Vars, name: nameof(Vars));
			Vars.Resolve();
		}
	}
}
