using System;
using BinarySerializer;

namespace Ray1Map.Jade {
	public class STR_StringRenderObject : GRO_GraphicRenderObject {
		public uint Count { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Count = s.Serialize<uint>(Count, name: nameof(Count));
		}
	}
}
