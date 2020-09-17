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

        public byte Batman_02 { get; set; }
        public byte Batman_03 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Length = s.Serialize<byte>(Length, name: nameof(Length));
            ActorIndicesCount = s.Serialize<byte>(ActorIndicesCount, name: nameof(ActorIndicesCount));
            if (s.GameSettings.EngineVersion == EngineVersion.GBA_BatmanVengeance) {
                Batman_02 = s.Serialize<byte>(Batman_02, name: nameof(Batman_02));
                Batman_03 = s.Serialize<byte>(Batman_03, name: nameof(Batman_03));
                ActorIndices = s.SerializeArray<byte>(ActorIndices, ActorIndicesCount, name: nameof(ActorIndices));
                RemainingData = s.SerializeArray<ushort>(RemainingData, (Length - (s.CurrentPointer - Offset)) / 2, name: nameof(RemainingData));
            } else {
                ActorIndices2Count = s.Serialize<byte>(ActorIndices2Count, name: nameof(ActorIndices2Count));
                ActorIndices = s.SerializeArray<byte>(ActorIndices, ActorIndicesCount, name: nameof(ActorIndices));
                ActorIndices2 = s.SerializeArray<byte>(ActorIndices2, ActorIndices2Count, name: nameof(ActorIndices2));
                s.Align(2);
                RemainingData = s.SerializeArray<ushort>(RemainingData, (Length - (s.CurrentPointer - Offset)) / 2, name: nameof(RemainingData));
            }
        }
    }
}