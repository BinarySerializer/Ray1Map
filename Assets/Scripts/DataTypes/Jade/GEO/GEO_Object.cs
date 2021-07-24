using System;
using BinarySerializer;

namespace R1Engine.Jade {
	public class GEO_Object : Jade_File {
		public override bool HasHeaderBFFile => true;

		public GRO_Struct RenderObject { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			RenderObject = s.SerializeObject<GRO_Struct>(RenderObject, onPreSerialize: o => o.Object = this, name: nameof(RenderObject));
		}
	}
}
