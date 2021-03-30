using System;

namespace R1Engine.Jade {
	// Found in GEO_p_CreateFromBuffer
	public class GEO_RenderObject : GRO_GraphicRenderObject {
		public uint Code_00 { get; set; }
		public uint Type { get; set; }
		public uint VerticesCount { get; set; }
		public int HasMRM { get; set; }
		public int HasShortPerVertex { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Code_00 = s.Serialize<uint>(Code_00, name: nameof(Code_00));
			if (Code_00 == (uint)Jade_Code.Code2002) {
				Type = s.Serialize<uint>(Type, name: nameof(Type));
				VerticesCount = s.Serialize<uint>(VerticesCount, name: nameof(VerticesCount));
				HasMRM = s.Serialize<int>(HasMRM, name: nameof(HasMRM));
				if(HasMRM != 0) HasShortPerVertex = s.Serialize<int>(HasShortPerVertex, name: nameof(HasShortPerVertex));
			} else {
				VerticesCount = Code_00;
				Type = 0;
			}
			throw new NotImplementedException($"TODO: Implement {GetType()}");
		}
	}
}
