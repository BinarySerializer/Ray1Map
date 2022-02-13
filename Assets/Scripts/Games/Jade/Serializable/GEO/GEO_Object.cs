using BinarySerializer;

namespace Ray1Map.Jade {
	public class GEO_Object : Jade_File {
		public override string Export_Extension {
			get {
				switch (RenderObject?.Type) {
					case GRO_Type.GEO:
					case GRO_Type.GEO_StaticLOD:
					case GRO_Type.GEO_SubGeometry:
					case GRO_Type.PAG:
					case GRO_Type.STR:
						return "gro";
					case GRO_Type.MAT_MSM:
					case GRO_Type.MAT_MTT:
					case GRO_Type.MAT_SIN:
						return "grm";
					case GRO_Type.LIGHT:
						return "grl";
					default: return null;
				}
			}
		}
		public override bool HasHeaderBFFile => true;

		public GRO_Struct RenderObject { get; set; }

		protected override void SerializeFile(SerializerObject s) {
			RenderObject = s.SerializeObject<GRO_Struct>(RenderObject, onPreSerialize: o => o.Object = this, name: nameof(RenderObject));
		}
	}
}
