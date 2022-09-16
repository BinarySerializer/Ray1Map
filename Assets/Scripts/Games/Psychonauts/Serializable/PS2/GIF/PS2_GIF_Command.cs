using BinarySerializer;
using BinarySerializer.PS2;

namespace Ray1Map.Psychonauts 
{
    public class PS2_GIF_Command : BinarySerializable
    {
        public uint Pre_UVChannelsCount { get; set; }
        public int Pre_JointInfluencesPerVertex { get; set; }

        public GIFtag GIFTag { get; set; }
        public float[] FirstRow { get; set; }

        public PS2_GIF_Command_Cycle[] Cycles { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            GIFTag = s.SerializeObject<GIFtag>(GIFTag, name: nameof(GIFTag));
			FirstRow = s.SerializeArray<float>(FirstRow, 4, name: nameof(FirstRow));

			if (GIFTag.FLG == GIF_FLG.PACKED) 
            {
				Cycles = s.SerializeObjectArray<PS2_GIF_Command_Cycle>(Cycles, GIFTag.NLOOP, onPreSerialize: c =>
                {
                    c.Pre_UVChannelsCount = Pre_UVChannelsCount;
                    c.Pre_JointInfluencesPerVertex = Pre_JointInfluencesPerVertex;
                }, name: nameof(Cycles));
			} 
            else 
            {
                throw new BinarySerializableException(this, $"Unsupported FLG {GIFTag.FLG}");
            }
        }
    }
}