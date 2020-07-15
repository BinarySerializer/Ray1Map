using System.Collections.Generic;
using System.Linq;

namespace R1Engine
{
    /// <summary>
    /// Event block data for Rayman 1 (Jaguar)
    /// </summary>
    public class Jaguar_R1_EventBlock : R1Serializable
    {
        // Is this correct?
        public bool HasEvents { get; set; }

        // Event map dimensions, always the map size divided by 4
        public ushort Width { get; set; }
        public ushort Height { get; set; }

        // Mapped to a 2D plane based on width and height
        public ushort[] EventIndexMap { get; set; }

        // Indexed, with offsets to the data table
        public ushort[] EventOffsetTable { get; set; }

        public Jaguar_R1_EventInstance[][] EventData { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Get offsets
            var offsetTableOffset = s.GameSettings.EngineVersion != EngineVersion.RayJaguarProto ? Offset + 0x1208 : new Jaguar_R1Proto_Manager().GetDataPointer(s.Context, Jaguar_R1Proto_References.test_offlist);
            var eventTableOffset = s.GameSettings.EngineVersion != EngineVersion.RayJaguarProto ? Offset + 0x1608 : new Jaguar_R1Proto_Manager().GetDataPointer(s.Context, Jaguar_R1Proto_References.test_event);

            HasEvents = s.Serialize<bool>(HasEvents, name: nameof(HasEvents));
            s.SerializeArray<byte>(new byte[3], 3, name: "Padding");
            
            // Serialize event map dimensions
            Width = s.Serialize<ushort>(Width, name: nameof(Width));
            Height = s.Serialize<ushort>(Height, name: nameof(Height));

            EventIndexMap = s.SerializeArray<ushort>(EventIndexMap, Width * Height, name: nameof(EventIndexMap));

            // Serialize next data block, skipping the padding
            s.DoAt(offsetTableOffset, () => EventOffsetTable = s.SerializeArray<ushort>(EventOffsetTable, EventIndexMap.Max(), name: nameof(EventIndexMap)));

            if (EventData == null)
                EventData = new Jaguar_R1_EventInstance[EventOffsetTable.Length][];

            // Serialize the events based on the offsets
            for (int i = 0; i < EventData.Length; i++)
            {
                s.DoAt(eventTableOffset + EventOffsetTable[i], () =>
                {
                    if (EventData[i] == null)
                    {
                        var temp = new List<Jaguar_R1_EventInstance>();

                        var index = 0;
                        while (temp.LastOrDefault()?.Unk_00 != 0)
                        {
                            temp.Add(s.SerializeObject<Jaguar_R1_EventInstance>(default, name: $"{nameof(EventData)}[{i}][{index}]"));
                            index++;
                        }

                        // Remove last entry as it's invalid
                        temp.RemoveAt(temp.Count - 1);

                        EventData[i] = temp.ToArray();
                    }
                    else
                    {
                        for (int j = 0; j < EventData[i].Length; j++)
                            EventData[i][j] = s.SerializeObject<Jaguar_R1_EventInstance>(EventData[i][j], name: $"{nameof(EventData)}[{i}][{j}]");

                        s.Serialize<ushort>(0, name: nameof(Jaguar_R1_EventInstance.Unk_00));
                    }
                });
            }
        }
    }
}