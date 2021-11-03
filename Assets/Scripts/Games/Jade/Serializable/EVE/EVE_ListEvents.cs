using BinarySerializer;

namespace Ray1Map.Jade
{
    public class EVE_ListEvents : BinarySerializable
    {
        public EVE_Track Track { get; set; } // Set before serializing
        public uint EventsCount { get; set; }

        public ushort FirstEvent_FramesCount { get; set; }
        public uint FirstEvent_Flags { get; set; }
        public ushort FirstEvent_UShort_04 { get; set; }
        public ushort FirstEvent_UShort_06 { get; set; }
        public byte[] FirstEvent_Bytes { get; set; }
        public int FirstEvent_Int { get; set; }

        public EVE_Event[] Events { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            EventsCount = s.Serialize<uint>(EventsCount, name: nameof(EventsCount));

            if (Track.DataLength > 0 && !(s is BinarySerializer.BinarySerializer))
            {
                // Lookahead
                s.DoAt(s.CurrentPointer, () => {
                    if (Track.Flags.HasFlag(EVE_Track.TrackFlags.Flag_9))
                        FirstEvent_FramesCount = s.Serialize<byte>((byte)FirstEvent_FramesCount, name: nameof(FirstEvent_FramesCount));
                    else
                        FirstEvent_FramesCount = s.Serialize<ushort>(FirstEvent_FramesCount, name: nameof(FirstEvent_FramesCount));

                    if (Track.ListTracks.Montreal_Version >= 0x8000)
						FirstEvent_Flags = s.Serialize<uint>(FirstEvent_Flags, name: nameof(FirstEvent_Flags));
					else
						FirstEvent_Flags = s.Serialize<ushort>((ushort)FirstEvent_Flags, name: nameof(FirstEvent_Flags));

                    FirstEvent_UShort_04 = s.Serialize<ushort>(FirstEvent_UShort_04, name: nameof(FirstEvent_UShort_04));
                    FirstEvent_UShort_06 = s.Serialize<ushort>(FirstEvent_UShort_06, name: nameof(FirstEvent_UShort_06));

                    if (Track.Flags.HasFlag(EVE_Track.TrackFlags.Flag_13) && (FirstEvent_UShort_06 & 8) != 0) {
                        long count = FirstEvent_UShort_04;

                        if ((FirstEvent_UShort_06 & 0x10) != 0 && (FirstEvent_UShort_06 & 0x80) != 0)
                            count = Track.Flags.HasFlag(EVE_Track.TrackFlags.Flag_12) ? 6 : 16;

                        if ((FirstEvent_UShort_06 & 3) != 0)
                            count = 12 * (FirstEvent_UShort_06 & 3);

                        FirstEvent_Bytes = s.SerializeArray(FirstEvent_Bytes, count, name: nameof(FirstEvent_Bytes));
                        FirstEvent_Int = s.Serialize<int>(FirstEvent_Int, name: nameof(FirstEvent_Int));
                    }
                });
            }

            if (Events == null)
                Events = new EVE_Event[EventsCount];

            for (int i = 0; i < Events.Length; i++)
            {
                Events[i] = s.SerializeObject<EVE_Event>(Events[i], x =>
                {
                    x.ListEvents = this;
                    x.Index = i;
                }, name: $"{nameof(Events)}[{i}]");
            }
        }
    }
}