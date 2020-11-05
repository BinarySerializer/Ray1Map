namespace R1Engine
{
    /// <summary>
    /// Maybe animation frames?
    /// </summary>
    public class GBAIsometric_RHR_AnimFrame : R1Serializable
    {
        public ushort UnkStructIndex { get; set; }
        public ushort TileIndicesIndex { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            UnkStructIndex = s.Serialize<ushort>(UnkStructIndex, name: nameof(UnkStructIndex));
            TileIndicesIndex = s.Serialize<ushort>(TileIndicesIndex, name: nameof(TileIndicesIndex));
        }
    }
}