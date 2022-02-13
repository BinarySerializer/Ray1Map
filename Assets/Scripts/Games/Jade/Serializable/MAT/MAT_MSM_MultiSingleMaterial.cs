using BinarySerializer;

namespace Ray1Map.Jade {
	public class MAT_MSM_MultiSingleMaterial : GRO_GraphicRenderObject {
		public uint Count { get; set; }
		public Jade_Reference<GEO_Object>[] Materials { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Count = s.Serialize<uint>(Count, name: nameof(Count));
			Materials = s.SerializeObjectArray<Jade_Reference<GEO_Object>>(Materials, Count, name: nameof(Materials));
			foreach (var mat in Materials) {
				mat?.Resolve();
			}
		}
	}
}
