using BinarySerializer;

namespace R1Engine.Jade
{
    public class EVE_ListEvents : BinarySerializable
    {
        public EVE_Track Track { get; set; } // Set before serializing
        public uint EventsCount { get; set; }

        public short Header_Short_00 { get; set; }
        public short Header_Short_02 { get; set; }
        public ushort Header_Ushort_04 { get; set; }
        public ushort Header_Flags { get; set; }
        public byte[] Header_Bytes { get; set; }
        public int Header_Int { get; set; }

        public EVE_Event[] Events { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            EventsCount = s.Serialize<uint>(EventsCount, name: nameof(EventsCount));

            if (Track.UInt_04 > 0)
            {
                s.DoAt(s.CurrentPointer, () => {
                    if (Track.Flags.HasFlag(EVE_Track.TrackFlags.Flag_9))
                        Header_Short_00 = s.Serialize<byte>((byte)Header_Short_00, name: nameof(Header_Short_00));
                    else
                        Header_Short_00 = s.Serialize<short>(Header_Short_00, name: nameof(Header_Short_00));

                    Header_Short_02 = s.Serialize<short>(Header_Short_02, name: nameof(Header_Short_02));
                    Header_Ushort_04 = s.Serialize<ushort>(Header_Ushort_04, name: nameof(Header_Ushort_04));
                    Header_Flags = s.Serialize<ushort>(Header_Flags, name: nameof(Header_Flags));

                    if (Track.Flags.HasFlag(EVE_Track.TrackFlags.Flag_13) && (Header_Flags & 8) != 0) {
                        long count = Header_Ushort_04;

                        if ((Header_Flags & 0x10) != 0 && (Header_Flags & 0x80) != 0)
                            count = Track.Flags.HasFlag(EVE_Track.TrackFlags.Flag_12) ? 6 : 16;

                        if ((Header_Flags & 3) != 0)
                            count = 12 * (Header_Flags & 3);

                        Header_Bytes = s.SerializeArray(Header_Bytes, count, name: nameof(Header_Bytes));
                        Header_Int = s.Serialize<int>(Header_Int, name: nameof(Header_Int));
                    }
                });
            }

            if (Events == null)
                Events = new EVE_Event[EventsCount];

            for (int i = 0; i < Events.Length; i++)
            {
                Events[i] = s.SerializeObject(Events[i], x =>
                {
                    x.ListEvents = this;
                    x.Index = i;
                }, name: $"{nameof(Events)}[{i}]");
            }
        }
    }
}