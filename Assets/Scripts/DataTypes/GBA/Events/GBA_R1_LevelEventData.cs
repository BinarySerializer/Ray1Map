namespace R1Engine
{
    /// <summary>
    /// Level event data for Rayman Advance (GBA)
    /// </summary>
    public class GBA_R1_LevelEventData
    {
        public Pointer EventGraphicsPointer { get; set; }
        public Pointer EventDataPointer { get; set; }
        public Pointer GraphicsGroupCountTablePointer { get; set; }
        public uint GraphicsGroupCount { get; set; }

        public byte[] GraphicsGroupCountTable { get; set; }

        public Pointer[] GraphicDataPointers { get; set; }
        public GBA_R1_EventGraphicsData[] GraphicData { get; set; }
        
        public Pointer[] EventDataPointers { get; set; }
        public GBA_R1_EventData[][] EventData { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public void SerializeData(SerializerObject s, Pointer eventGraphicsPointers, Pointer eventDataPointers, Pointer eventGraphicsGroupCountTablePointers, Pointer levelEventGraphicsGroupCounts)
        {
            // Get the global level index
            var levelIndex = new GBA_R1_Manager().GetGlobalLevelIndex(s.GameSettings.World, s.GameSettings.Level);

            // Serialize data
            s.DoAt(eventGraphicsPointers + (uint)(4 * levelIndex), 
                () => EventGraphicsPointer = s.SerializePointer(EventGraphicsPointer, name: nameof(EventGraphicsPointer)));
            s.DoAt(eventDataPointers + (uint)(4 * levelIndex), 
                () => EventDataPointer = s.SerializePointer(EventDataPointer,  name: nameof(EventDataPointer)));
            s.DoAt(eventGraphicsGroupCountTablePointers + (uint)(4 * levelIndex), 
                () => GraphicsGroupCountTablePointer = s.SerializePointer(GraphicsGroupCountTablePointer, name: nameof(GraphicsGroupCountTablePointer)));
            s.DoAt(levelEventGraphicsGroupCounts + (uint)(4 * levelIndex), 
                () => GraphicsGroupCount = s.Serialize<uint>(GraphicsGroupCount, name: nameof(GraphicsGroupCount)));


            // Parse data from pointers
            s.DoAt(GraphicsGroupCountTablePointer, () => GraphicsGroupCountTable = s.SerializeArray<byte>(GraphicsGroupCountTable, GraphicsGroupCount, name: nameof(GraphicsGroupCountTable)));
            s.DoAt(EventGraphicsPointer, () => GraphicDataPointers = s.SerializePointerArray(GraphicDataPointers, GraphicsGroupCount, name: nameof(GraphicDataPointers)));

            if (GraphicData == null)
                GraphicData = new GBA_R1_EventGraphicsData[GraphicsGroupCount];

            for (int i = 0; i < GraphicData.Length; i++)
                s.DoAt(GraphicDataPointers[i], () => GraphicData[i] = s.SerializeObject<GBA_R1_EventGraphicsData>(GraphicData[i], name: $"{nameof(GraphicData)}[{i}]"));

            s.DoAt(EventDataPointer, () => EventDataPointers = s.SerializePointerArray(EventDataPointers, GraphicsGroupCount, name: nameof(EventDataPointers)));

            if (EventData == null)
                EventData = new GBA_R1_EventData[GraphicsGroupCount][];

            for (int i = 0; i < EventData.Length; i++)
            {
                if (EventDataPointers[i] != null)
                    s.DoAt(EventDataPointers[i], () => EventData[i] = s.SerializeObjectArray<GBA_R1_EventData>(EventData[i], GraphicsGroupCountTable[i], name: $"{nameof(EventData)}[{i}]"));
                else
                    EventData[i] = new GBA_R1_EventData[0];
            }
        }
    }
}