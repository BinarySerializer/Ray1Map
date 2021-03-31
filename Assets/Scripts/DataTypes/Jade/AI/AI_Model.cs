using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinarySerializer;

namespace R1Engine.Jade {
	public class AI_Model: Jade_File {
		public Jade_GenericReference[] References { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			References = s.SerializeObjectArray<Jade_GenericReference>(References, FileSize / 8, name: nameof(References));
			foreach (var reference in References) {
				reference.Resolve();
			}
		}
	}
}
