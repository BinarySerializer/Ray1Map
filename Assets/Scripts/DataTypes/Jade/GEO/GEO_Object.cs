using System;

namespace R1Engine.Jade {
	public class GEO_Object : Jade_File {
		public GRO_Struct RenderObject { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			RenderObject = s.SerializeObject<GRO_Struct>(RenderObject, name: nameof(RenderObject));
			throw new NotImplementedException($"TODO: Implement {GetType()}");
		}
	}
}
