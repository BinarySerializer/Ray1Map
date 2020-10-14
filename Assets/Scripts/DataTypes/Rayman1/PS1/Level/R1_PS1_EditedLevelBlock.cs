namespace R1Engine
{
    public class R1_PS1_EditedLevelBlock : R1Serializable
    {
        // The data block - we don't ever need to parse this, so the length is technically not needed
        public uint DataBlockLength { get; set; }
        public byte[] DataBlock { get; set; }

        public void UpdateAndFillDataBlock(Pointer offset, R1_PS1_EventBlock originalBlock, R1_EventData[] events, byte[] eventLinkingTable, GameSettings settings)
        {
            long currentOffset = 0;
            Pointer getCurrentBlockPointer() => offset + (1 * 4) + currentOffset;

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
                else
                {
                    e.CommandsPointer = null;
                }

                if (e.LabelOffsets != null)
                {
                    e.LabelOffsetsPointer = getCurrentBlockPointer();
                    currentOffset += e.LabelOffsets.Length * 2;
                }
                else
                {
                    e.LabelOffsetsPointer = null;
                }
            }

            // Set the data block size
            DataBlockLength = (uint)currentOffset;
        }

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize values
            DataBlockLength = s.Serialize<uint>(DataBlockLength, name: nameof(DataBlockLength));
            DataBlock = s.SerializeArray<byte>(DataBlock, DataBlockLength, name: nameof(DataBlock));
        }
    }
}