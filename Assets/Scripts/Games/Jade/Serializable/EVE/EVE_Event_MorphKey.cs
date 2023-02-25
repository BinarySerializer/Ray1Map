using BinarySerializer;

namespace Ray1Map.Jade
{
    public class EVE_Event_MorphKey : BinarySerializable
    {
        public ushort DataSize { get; set; }
        public ushort Flags { get; set; }
        public EVE_Event_MorphKey_Old Param { get; set; }
        public uint Count { get; set; }
        public float[] Floats { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            DataSize = s.Serialize<ushort>(DataSize, name: nameof(DataSize));
            Flags = s.Serialize<ushort>(Flags, name: nameof(Flags));
            Param = s.SerializeObject<EVE_Event_MorphKey_Old>(Param, name: nameof(Param));
            if ((Flags & 1) == 1) {
                Count = s.Serialize<uint>(Count, name: nameof(Count));
                Floats = s.SerializeArray<float>(Floats, Count * 2 - 1, name: nameof(Floats));
            }
        }
    }
}