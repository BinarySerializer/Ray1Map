using BinarySerializer;

namespace R1Engine.Jade
{
    public class EVE_ListTracks : Jade_File
    {
        public ushort TracksCount { get; set; }
        public ushort Flags { get; set; }
        public EVE_Track[] Tracks { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            TracksCount = s.Serialize<ushort>(TracksCount, name: nameof(TracksCount));
            Flags = s.Serialize<ushort>(Flags, name: nameof(Flags));
            Tracks = s.SerializeObjectArray<EVE_Track>(Tracks, TracksCount, name: nameof(Tracks));
        }
    }
}