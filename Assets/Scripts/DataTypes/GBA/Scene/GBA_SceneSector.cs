namespace R1Engine
{
    public class GBA_SceneSector : R1Serializable
    {
        public byte Length { get; set; }

        public byte ActorIndicesCount { get; set; }

        public byte ActorIndices2Count { get; set; }

        public byte[] ActorIndices { get; set; }
        public byte[] ActorIndices2 { get; set; }

        public ushort[] RemainingData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Length = s.Serialize<byte>(Length, name: nameof(Length));
            ActorIndicesCount = s.Serialize<byte>(ActorIndicesCount, name: nameof(ActorIndicesCount));
            ActorIndices2Count = s.Serialize<byte>(ActorIndices2Count, name: nameof(ActorIndices2Count));
            ActorIndices = s.SerializeArray<byte>(ActorIndices, ActorIndicesCount, name: nameof(ActorIndices));
            ActorIndices2 = s.SerializeArray<byte>(ActorIndices2, ActorIndices2Count, name: nameof(ActorIndices2));
            s.Align(2);
            RemainingData = s.SerializeArray<ushort>(RemainingData,( Length - (s.CurrentPointer - Offset)) / 2, name: nameof(RemainingData));
        }
    }
}