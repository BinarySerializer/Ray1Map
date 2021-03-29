using System;

namespace R1Engine.Jade
{
    public class EVE_ListTracks : Jade_File
    {
        public ushort Count { get; set; }
        public ushort Flags { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Count = s.Serialize<ushort>(Count, name: nameof(Count));
            Flags = s.Serialize<ushort>(Flags, name: nameof(Flags));

            throw new NotImplementedException($"TODO: Implement {GetType()}");
        }
    }
}