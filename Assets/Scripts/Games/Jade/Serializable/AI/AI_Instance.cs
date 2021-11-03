using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinarySerializer;

namespace Ray1Map.Jade {
	public class AI_Instance : Jade_File {
		public override bool HasHeaderBFFile => true;
		public override string Export_Extension => "oin";

		public Jade_Reference<AI_Model> Model { get; set; }
		public Jade_Reference<AI_Vars> Vars { get; set; }

		protected override void SerializeFile(SerializerObject s) {
			Model = s.SerializeObject<Jade_Reference<AI_Model>>(Model, name: nameof(Model))?.Resolve();
			Vars = s.SerializeObject<Jade_Reference<AI_Vars>>(Vars, name: nameof(Vars))?
				.Resolve(flags: LOA_Loader.ReferenceFlags.Log | LOA_Loader.ReferenceFlags.Flag3);
		}
	}
}
