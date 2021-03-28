using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R1Engine.Jade {
	public class OBJ_GameObject_HierarchyData : R1Serializable {
		public Jade_Reference<OBJ_GameObject> Parent { get; set; }
		public Jade_Matrix LocalMatrix { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Parent = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(Parent, name: nameof(Parent))?.Resolve();
			LocalMatrix = s.SerializeObject<Jade_Matrix>(LocalMatrix, name: nameof(LocalMatrix));
		}
	}
}
