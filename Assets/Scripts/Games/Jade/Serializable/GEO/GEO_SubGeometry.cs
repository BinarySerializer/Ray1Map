using BinarySerializer;

namespace Ray1Map.Jade {
	// Found in GEO_p_SubGeometry_CreateFromBuffer
	public class GEO_SubGeometry : GRO_GraphicRenderObject {
		public uint SubGeoIndex { get; set; }
		public Jade_Reference<GEO_Object> SubGeometryOf { get; set; }
		public uint ElementsCount { get; set; }
		public Element[] Elements { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			SubGeoIndex = s.Serialize<uint>(SubGeoIndex, name: nameof(SubGeoIndex));
			SubGeometryOf = s.SerializeObject<Jade_Reference<GEO_Object>>(SubGeometryOf, name: nameof(SubGeometryOf))?.Resolve();
			ElementsCount = s.Serialize<uint>(ElementsCount, name: nameof(ElementsCount));
			Elements = s.SerializeObjectArray<Element>(Elements, ElementsCount, name: nameof(Elements));
		}

		public class Element : BinarySerializable {
			public uint MaterialID { get; set; }
			public uint TrianglesCount { get; set; }
			public GEO_GeometricObjectElement.Triangle[] Triangles { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				MaterialID = s.Serialize<uint>(MaterialID, name: nameof(MaterialID));
				TrianglesCount = s.Serialize<uint>(TrianglesCount, name: nameof(TrianglesCount));
				Triangles = s.SerializeObjectArray<GEO_GeometricObjectElement.Triangle>(Triangles, TrianglesCount, name: nameof(Triangles));
			}
		}
	}
}
