using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace Ray1Map.Jade {
	public class OBJ_GameObject_Hierarchy : BinarySerializable {
		public Jade_Reference<OBJ_GameObject> Father { get; set; }
		public Jade_Matrix LocalMatrix { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Father = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(Father, name: nameof(Father))?.Resolve();
			LocalMatrix = s.SerializeObject<Jade_Matrix>(LocalMatrix, name: nameof(LocalMatrix));
		}
	}
}
