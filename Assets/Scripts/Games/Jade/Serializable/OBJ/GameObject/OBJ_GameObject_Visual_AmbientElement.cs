using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace Ray1Map.Jade {
	public class OBJ_GameObject_Visual_AmbientElement : BinarySerializable {
		public uint TriangleCount { get; set; }
		public float[] Coords { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			TriangleCount = s.Serialize<uint>(TriangleCount, name: nameof(TriangleCount));
			Coords = s.SerializeArray<float>(Coords, (TriangleCount & 0xFFFF) * 6, name: nameof(Coords));
		}
	}
}
