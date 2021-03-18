using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R1Engine.Jade {
	public class AI_Instance : Jade_File {
		public Jade_Key ModelKey { get; set; }
		public Jade_Key VarsKey { get; set; }

		public AI_Model Model { get; set; } 

		public override void SerializeImpl(SerializerObject s) {
			ModelKey = s.SerializeObject<Jade_Key>(ModelKey, name: nameof(ModelKey));
			VarsKey = s.SerializeObject<Jade_Key>(VarsKey, name: nameof(VarsKey));

			Loader.RequestFile(ModelKey, (s,configureAction) => {
				Model = s.SerializeObject<AI_Model>(Model, onPreSerialize: m => configureAction(m), name: nameof(Model));
			}, (f) => {
				Model = (AI_Model)f;
			});
		}
	}
}
