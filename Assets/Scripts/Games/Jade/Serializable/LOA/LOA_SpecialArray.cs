using BinarySerializer;
using System.Collections.Generic;

namespace Ray1Map.Jade {
	public class LOA_SpecialArray : BinarySerializable {
		public Jade_Key[] Keys { get; set; }

		public HashSet<Jade_Key> Lookup { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Keys = s.SerializeObjectArray<Jade_Key>(Keys, s.CurrentLength / 4, name: nameof(Keys));
			Lookup = new HashSet<Jade_Key>(Keys);
		}
	}
}
