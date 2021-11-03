using BinarySerializer;
using Ray1Map.GBC;

namespace Ray1Map.GBA
{
    public class GBA_Shanghai_CaptorsBlock : GBA_BaseBlock
    {
        public ushort CaptorsCount { get; set; }
        public GBC_GameObject[] Captors { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            CaptorsCount = s.Serialize<ushort>(CaptorsCount, name: nameof(CaptorsCount));
            Captors = s.SerializeObjectArray<GBC_GameObject>(Captors, CaptorsCount, x => x.GBA_IsCaptor = true, name: nameof(Captors));
        }
    }
}