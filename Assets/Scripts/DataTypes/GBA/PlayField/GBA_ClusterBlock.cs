namespace R1Engine
{
    public class GBA_ClusterBlock : GBA_BaseBlock
    {
        public GBA_Cluster ClusterData { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            ClusterData = s.SerializeObject<GBA_Cluster>(ClusterData, name: nameof(ClusterData));
        }
    }
}