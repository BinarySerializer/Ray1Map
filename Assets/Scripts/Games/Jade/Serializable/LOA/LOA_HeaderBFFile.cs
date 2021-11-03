using System;
using BinarySerializer;

namespace Ray1Map.Jade {
	public class LOA_HeaderBFFile : BinarySerializable {
		public Jade_GenericReference[] Dependencies { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Dependencies = s.SerializeObjectArrayUntil<Jade_GenericReference>(Dependencies, d => d.Key == 0xFFFFFFFF, name: nameof(Dependencies));
		}
	}
}
