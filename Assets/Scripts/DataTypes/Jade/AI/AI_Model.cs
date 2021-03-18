using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R1Engine.Jade {
	public class AI_Model: Jade_File {
		public Reference[] References { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			References = s.SerializeObjectArray<Reference>(References, FileSize / 8, name: nameof(References));
		}

		public class Reference : R1Serializable {
			public Jade_Key Key { get; set; }
			public string Type { get; set; } // Can be: .ova (vars), .fce (proclist), .ofc (function)

			public override void SerializeImpl(SerializerObject s) {
				Key = s.SerializeObject<Jade_Key>(Key, name: nameof(Key));
				Type = s.SerializeString(Type, 4, name: nameof(Type));
				
			}
		}
	}
}
