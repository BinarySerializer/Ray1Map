using BinarySerializer;
using BinarySerializer.PlayStation.PS2;

namespace Ray1Map.Jade {
    public class PS2_VU_Data : BinarySerializable {
        public float[] FirstTwoRows { get; set; }
        public GIFtag GIFTag { get; set; }

        public PS2_VU_BoneIndex[] BoneIndices { get; set; }
        public PS2_VU_BoneWeight[] BoneWeights { get; set; }
        public PS2_VU_Vertex[] Vertices { get; set; }
        public PS2_VU_UV[] UVs { get; set; }
        public PS2_VU_Normal[] Normals { get; set; }
        public GIF_Packed_RGBA[] Colors { get; set; }

        public int Pre_DMAId { get; set; }
        public bool Pre_HasBones { get; set; }

        public int Count => Pre_HasBones ? 40 : 64;
        // TODO: after this, sometimes color, sometimes normal.
        // Normal in 130033F7. UseNormalsInEngine, SkinnedMesh, CanBeInstanciated
        // Color in 3000044A. StaticMesh (vertex colors specified in gao)

        public override void SerializeImpl(SerializerObject s) {
			FirstTwoRows = s.SerializeArray<float>(FirstTwoRows, 8, name: nameof(FirstTwoRows));
			GIFTag = s.SerializeObject<GIFtag>(GIFTag, name: nameof(GIFTag));
            if (Pre_HasBones) {
				BoneIndices = s.SerializeObjectArray<PS2_VU_BoneIndex>(BoneIndices, Count, name: nameof(BoneIndices));
				BoneWeights = s.SerializeObjectArray<PS2_VU_BoneWeight>(BoneWeights, Count, name: nameof(BoneWeights));
			}
			Vertices = s.SerializeObjectArray<PS2_VU_Vertex>(Vertices, Count, name: nameof(Vertices));
            switch (Pre_DMAId) {
                case -1:
                    s.SerializePadding(Count * 16, logIfNotNull: true);
                    Colors = s.SerializeObjectArray<GIF_Packed_RGBA>(Colors, Count, name: nameof(Colors));
                    break;
                case 0:
                    UVs = s.SerializeObjectArray<PS2_VU_UV>(UVs, Count, name: nameof(UVs));
                    Colors = s.SerializeObjectArray<GIF_Packed_RGBA>(Colors, Count, name: nameof(Colors));
                    break;
                case 1:
                    UVs = s.SerializeObjectArray<PS2_VU_UV>(UVs, Count, name: nameof(UVs));
                    Normals = s.SerializeObjectArray<PS2_VU_Normal>(Normals, Count, name: nameof(Normals));
                    break;
                case 2:
                    Normals = s.SerializeObjectArray<PS2_VU_Normal>(Normals, Count, name: nameof(Normals));
                    Colors = s.SerializeObjectArray<GIF_Packed_RGBA>(Colors, Count, name: nameof(Colors));
                    break;
                case 3:
                    UVs = s.SerializeObjectArray<PS2_VU_UV>(UVs, Count, name: nameof(UVs));
                    s.SerializePadding(Count * 16, logIfNotNull: true);
                    break;
                default:
                    throw new BinarySerializableException(this, $"Unknown DMAId: {Pre_DMAId}");

            }
		}
    }
}