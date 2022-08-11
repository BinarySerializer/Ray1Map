using BinarySerializer;
using BinarySerializer.PS2;

namespace Ray1Map.Psychonauts {

    public class PS2_GIF_Command_Cycle : BinarySerializable
    {
        public uint Pre_UVSetsCount { get; set; }

        public PS2_GIF_XYZF Vertex { get; set; }
        public PS2_GIF_Normal Normal { get; set; }
        public GIF_Packed_RGBA Color { get; set; }
        public PS2_GIF_STQ[] UVSets { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
			Vertex = s.SerializeObject<PS2_GIF_XYZF>(Vertex, name: nameof(Vertex));
			Normal = s.SerializeObject<PS2_GIF_Normal>(Normal, name: nameof(Normal));
			Color = s.SerializeObject<GIF_Packed_RGBA>(Color, name: nameof(Color));
			UVSets = s.SerializeObjectArray<PS2_GIF_STQ>(UVSets, Pre_UVSetsCount, name: nameof(UVSets));
        }
    }
}