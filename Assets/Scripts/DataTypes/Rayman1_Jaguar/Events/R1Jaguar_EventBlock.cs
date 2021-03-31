using System.Collections.Generic;
using System.Linq;
using BinarySerializer;

namespace R1Engine
{
    /// <summary>
    /// Event block data for Rayman 1 (Jaguar)
    /// </summary>
    public class R1Jaguar_EventBlock : BinarySerializable
    {
        public R1Jaguar_MapEvents MapEvents { get; set; }

        // Indexed, with offsets to the data table
        public ushort[] EventOffsetTable { get; set; }

        public R1Jaguar_EventInstance[][] EventData { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Get offsets
            var offsetTableOffset = s.GetR1Settings().EngineVersion != EngineVersion.R1Jaguar_Proto ? Offset + 0x1208 : new R1Jaguar_Proto_Manager().GetDataPointer(s.Context, R1Jaguar_Proto_References.test_offlist);
            var eventTableOffset = s.GetR1Settings().EngineVersion != EngineVersion.R1Jaguar_Proto ? Offset + 0x1608 : new R1Jaguar_Proto_Manager().GetDataPointer(s.Context, R1Jaguar_Proto_References.test_event);

            MapEvents = s.SerializeObject<R1Jaguar_MapEvents>(MapEvents, name: nameof(MapEvents));

            // Serialize next data block, skipping the padding
            s.DoAt(offsetTableOffset, () => EventOffsetTable = s.SerializeArray<ushort>(EventOffsetTable, MapEvents.EventIndexMap.Max(), name: nameof(EventOffsetTable)));

            if (EventData == null)
                EventData = new R1Jaguar_EventInstance[EventOffsetTable.Length][];

            // Serialize the events based on the offsets
            for (int i = 0; i < EventData.Length; i++)
            {
                s.DoAt(eventTableOffset + EventOffsetTable[i], () =>
                {
                    if (EventData[i] == null)
                    {
                        var temp = new List<R1Jaguar_EventInstance>();

                        var index = 0;
                        while (temp.LastOrDefault()?.Unk_00 != 0)
                        {
                            temp.Add(s.SerializeObject<R1Jaguar_EventInstance>(default, name: $"{nameof(EventData)}[{i}][{index}]"));
                            index++;
                        }

                        // Remove last entry as it's invalid
                        temp.RemoveAt(temp.Count - 1);

                        EventData[i] = temp.ToArray();
                    }
                    else
                    {
                        for (int j = 0; j < EventData[i].Length; j++)
                            EventData[i][j] = s.SerializeObject<R1Jaguar_EventInstance>(EventData[i][j], name: $"{nameof(EventData)}[{i}][{j}]");

                        s.Serialize<ushort>(0, name: nameof(R1Jaguar_EventInstance.Unk_00));
                    }
                });
            }
        }
    }
}