using System;
using BinarySerializer;

namespace R1Engine.Jade {
	public class STR_StringRenderObject : GRO_GraphicRenderObject {
		public uint Count { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Count = s.Serialize<uint>(Count, name: nameof(Count));
		}
	}
}
