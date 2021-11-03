using BinarySerializer;

namespace Ray1Map.GBA
{
    public class GBA_Milan_ActionBlock : GBA_BaseBlock
    {
        public GBA_Action Action { get; set; }

        public byte[] RemainingData { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            Action = s.SerializeObject<GBA_Action>(Action, name: nameof(Action));

            RemainingData = s.SerializeArray<byte>(RemainingData, (Offset + BlockSize) - s.CurrentPointer, name: nameof(RemainingData));
        }
    }
}