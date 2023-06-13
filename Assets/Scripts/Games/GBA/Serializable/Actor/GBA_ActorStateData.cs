using BinarySerializer;

namespace Ray1Map.GBA
{
    public class GBA_ActorStateData : GBA_BaseBlock
    {
        // TODO: Defines how the actor should behave. For example type 13 is MoveXY which changes x and y each frame based on speed, acceleration and max speed for both x and y
        public byte[] Data { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            Data = s.SerializeArray<byte>(Data, BlockSize, name: nameof(Data));
        }
    }
}