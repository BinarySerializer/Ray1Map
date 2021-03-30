using R1Engine.Serialize;
using System;

namespace R1Engine.Jade {
	public class Jade_TextReference : R1Serializable {
		public Jade_Key Key { get; set; }
		public bool IsNull => Key.IsNull;

		public override void SerializeImpl(SerializerObject s) {
			Key = s.SerializeObject<Jade_Key>(Key, name: nameof(Key));
		}

		public Jade_TextReference() { }
		public Jade_TextReference(Context c, Jade_Key key) {
			Context = c;
			Key = key;
		}
		
		// Dummy resolve method for now
		public Jade_TextReference Resolve() {
			return this;
		}
	}
}
