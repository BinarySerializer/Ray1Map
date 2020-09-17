namespace R1Engine
{
    public class GBA_SceneSector : R1Serializable
    {
        public byte Length { get; set; }

        public byte NormalActorIndicesCount { get; set; }

        public byte BoxTriggerActorIndicesCount { get; set; }

        public byte[] NormalActorIndices { get; set; }
        public byte[] BoxTriggerActorIndices2 { get; set; }

        public ushort[] RemainingData { get; set; }

        public byte Batman_02 { get; set; }
        public byte Batman_03 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Length = s.Serialize<byte>(Length, name: nameof(Length));
            NormalActorIndicesCount = s.Serialize<byte>(NormalActorIndicesCount, name: nameof(NormalActorIndicesCount));
            if (s.GameSettings.EngineVersion == EngineVersion.GBA_BatmanVengeance) {
                Batman_02 = s.Serialize<byte>(Batman_02, name: nameof(Batman_02));
                Batman_03 = s.Serialize<byte>(Batman_03, name: nameof(Batman_03));
                NormalActorIndices = s.SerializeArray<byte>(NormalActorIndices, NormalActorIndicesCount, name: nameof(NormalActorIndices));
                RemainingData = s.SerializeArray<ushort>(RemainingData, (Length - (s.CurrentPointer - Offset)) / 2, name: nameof(RemainingData));
            } else {
                BoxTriggerActorIndicesCount = s.Serialize<byte>(BoxTriggerActorIndicesCount, name: nameof(BoxTriggerActorIndicesCount));
                NormalActorIndices = s.SerializeArray<byte>(NormalActorIndices, NormalActorIndicesCount, name: nameof(NormalActorIndices));
                BoxTriggerActorIndices2 = s.SerializeArray<byte>(BoxTriggerActorIndices2, BoxTriggerActorIndicesCount, name: nameof(BoxTriggerActorIndices2));
                s.Align(2);
                RemainingData = s.SerializeArray<ushort>(RemainingData, (Length - (s.CurrentPointer - Offset)) / 2, name: nameof(RemainingData));
            }
        }
    }
}