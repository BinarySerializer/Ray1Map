namespace R1Engine
{
    public class PS1_R1_EditedLevelBlock : R1Serializable
    {
        // We need to keep the original count and pointers so we can also parse the original events to get the DES/ETA pointers
        public uint OriginalEventCount { get; set; }
        public Pointer OriginalEventsPointer { get; set; }
        public Pointer OriginalEventLinksPointer { get; set; }

        // The data block - we don't ever need to parse this, so the length is technically not needed
        public uint DataBlockLength { get; set; }
        public byte[] DataBlock { get; set; }

        // Parsed
        public PS1_R1_Event[] OriginalEvents { get; set; }
        public byte[] OriginalEventLinkingTable { get; set; }

        public void UpdateAndFillDataBlock(Pointer offset, PS1_R1_EventBlock originalBlock, PS1_R1_Event[] events, byte[] eventLinkingTable, GameSettings settings)
        {
            // Copy the original data if not already copied over
            if (OriginalEventsPointer == null)
            {
                OriginalEvents = originalBlock.Events;
                OriginalEventLinkingTable = originalBlock.EventLinkingTable;
                OriginalEventCount = originalBlock.EventCount;
                OriginalEventsPointer = originalBlock.EventsPointer;
                OriginalEventLinksPointer = originalBlock.EventLinksPointer;
            }

            long currentOffset = 0;
            Pointer getCurrentBlockPointer() => offset + (4 * 4) + currentOffset;

            originalBlock.EventCount = (byte)events.Length;
            originalBlock.EventsPointer = getCurrentBlockPointer();
            originalBlock.Events = events;

            currentOffset += events.Length * 112;

            originalBlock.EventLinkCount = (byte)eventLinkingTable.Length;
            originalBlock.EventLinksPointer = getCurrentBlockPointer();
            originalBlock.EventLinkingTable = eventLinkingTable;

            currentOffset += eventLinkingTable.Length;

            foreach (var e in events)
            {
                if (e.Commands != null)
                {
                    e.CommandsPointer = getCurrentBlockPointer();
                    currentOffset += e.Commands.ToBytes(settings).Length;
                }

                if (e.LabelOffsets != null)
                {
                    e.LabelOffsetsPointer = getCurrentBlockPointer();
                    currentOffset += (e.LabelOffsets.Length + 1) * 2;
                }
            }

            // Set the data block size
            DataBlockLength = (uint)currentOffset;
        }

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize values
            OriginalEventCount = s.Serialize<uint>(OriginalEventCount, name: nameof(OriginalEventCount));
            OriginalEventsPointer = s.SerializePointer(OriginalEventsPointer, name: nameof(OriginalEventsPointer));
            OriginalEventLinksPointer = s.SerializePointer(OriginalEventLinksPointer, name: nameof(OriginalEventLinksPointer));
            DataBlockLength = s.Serialize<uint>(DataBlockLength, name: nameof(DataBlockLength));
            DataBlock = s.SerializeArray<byte>(DataBlock, DataBlockLength, name: nameof(DataBlock));

            // Parse data from pointers
            s.DoAt(OriginalEventsPointer, () => OriginalEvents = s.SerializeObjectArray<PS1_R1_Event>(OriginalEvents, OriginalEventCount, name: nameof(OriginalEvents)));
            s.DoAt(OriginalEventLinksPointer, () => OriginalEventLinkingTable = s.SerializeArray<byte>(OriginalEventLinkingTable, OriginalEventCount, name: nameof(OriginalEventLinkingTable)));
        }
    }
}