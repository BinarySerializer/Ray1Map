using BinarySerializer;
using BinarySerializer.PS2;

namespace Ray1Map.Jade {
    public class PS2_VU_Data : BinarySerializable {
        public PS2_VU_Vertex[] Vertices { get; set; }
        public PS2_VU_UV[] UVs { get; set; }
        public PS2_VU_Normal[] Normals { get; set; }
        public GIF_Packed_RGBA[] Colors { get; set; }

        public bool Pre_HasNormals { get; set; }
        public bool Pre_HasColors { get; set; }

        public int Count { get; set; } = 0x40;
        // TODO: after this, sometimes color, sometimes normal.
        // Normal in 130033F7. UseNormalsInEngine, SkinnedMesh, CanBeInstanciated
        // Color in 3000044A. StaticMesh (vertex colors specified in gao)

        public override void SerializeImpl(SerializerObject s) {
			Vertices = s.SerializeObjectArray<PS2_VU_Vertex>(Vertices, Count, name: nameof(Vertices));
			UVs = s.SerializeObjectArray<PS2_VU_UV>(UVs, Count, name: nameof(UVs));
            if (Pre_HasNormals) {
				Normals = s.SerializeObjectArray<PS2_VU_Normal>(Normals, Count, name: nameof(Normals));
			}
            if (Pre_HasColors) {
				Colors = s.SerializeObjectArray<GIF_Packed_RGBA>(Colors, Count, name: nameof(Colors));
			}
		}
    }
}