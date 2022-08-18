using BinarySerializer;
using BinarySerializer.PS2;

namespace Ray1Map.Psychonauts {

    public class PS2_GIF_Command_Cycle : BinarySerializable
    {
        public uint Pre_UVChannelsCount { get; set; }
        public int Pre_JointInfluencesPerVertex { get; set; }

        public PS2_GIF_XYZF Vertex { get; set; }
        public PS2_GIF_Normal Normal { get; set; }
        public GIF_Packed_RGBA Color { get; set; }
        public PS2_GIF_UV[] UVs { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
			Vertex = s.SerializeObject<PS2_GIF_XYZF>(Vertex, x => x.Pre_JointInfluencesPerVertex = Pre_JointInfluencesPerVertex, name: nameof(Vertex));
			Normal = s.SerializeObject<PS2_GIF_Normal>(Normal, x => x.Pre_JointInfluencesPerVertex = Pre_JointInfluencesPerVertex, name: nameof(Normal));
			Color = s.SerializeObject<GIF_Packed_RGBA>(Color, name: nameof(Color));
			UVs = s.SerializeObjectArray<PS2_GIF_UV>(UVs, Pre_UVChannelsCount, (x, i) =>
            {
                if (i == 0)
                    x.Pre_JointInfluencesPerVertex = Pre_JointInfluencesPerVertex;
            }, name: nameof(UVs));
        }
    }
}