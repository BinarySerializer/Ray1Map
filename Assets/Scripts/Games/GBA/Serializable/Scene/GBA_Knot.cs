using System;
using BinarySerializer;

namespace Ray1Map.GBA
{
    /// <summary>
    /// Sector data
    /// </summary>
    public class GBA_Knot : BinarySerializable
    {
        public byte Length { get; set; }

        public byte ActorIndicesCount { get; set; }
        public byte CaptorIndicesCount { get; set; }

        public byte[] ActorIndices { get; set; }
        public byte[] CaptorIndices { get; set; }

        public ushort[] RemainingData { get; set; }

        public byte Batman_02 { get; set; }
        public byte Batman_03 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Length = s.Serialize<byte>(Length, name: nameof(Length));
            ActorIndicesCount = s.Serialize<byte>(ActorIndicesCount, name: nameof(ActorIndicesCount));

            if (Length == 0 && (s.GetR1Settings().EngineVersion is (EngineVersion.GBA_R3_20020418_NintendoE3Approval or EngineVersion.GBA_R3_20020301_PreAlpha)))
            {
                ActorIndicesCount = 0;
                CaptorIndicesCount = 0;
                ActorIndices = Array.Empty<byte>();
                CaptorIndices = Array.Empty<byte>();
                RemainingData = Array.Empty<ushort>();
                return;
            }

            if (s.GetR1Settings().EngineVersion == EngineVersion.GBA_BatmanVengeance) {
                Batman_02 = s.Serialize<byte>(Batman_02, name: nameof(Batman_02));
                Batman_03 = s.Serialize<byte>(Batman_03, name: nameof(Batman_03));
                ActorIndices = s.SerializeArray<byte>(ActorIndices, ActorIndicesCount, name: nameof(ActorIndices));
                RemainingData = s.SerializeArray<ushort>(RemainingData, (Length - (s.CurrentPointer - Offset)) / 2, name: nameof(RemainingData));
            } else {
                CaptorIndicesCount = s.Serialize<byte>(CaptorIndicesCount, name: nameof(CaptorIndicesCount));
                ActorIndices = s.SerializeArray<byte>(ActorIndices, ActorIndicesCount, name: nameof(ActorIndices));
                CaptorIndices = s.SerializeArray<byte>(CaptorIndices, CaptorIndicesCount, name: nameof(CaptorIndices));
                s.Align(2);
                RemainingData = s.SerializeArray<ushort>(RemainingData, (Length - (s.CurrentPointer - Offset)) / 2, name: nameof(RemainingData));
            }
        }
    }
}