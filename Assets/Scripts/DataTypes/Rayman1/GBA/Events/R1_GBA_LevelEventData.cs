namespace R1Engine
{
    /// <summary>
    /// Level event data for Rayman Advance (GBA)
    /// </summary>
    public class R1_GBA_LevelEventData
    {
        public Pointer EventGraphicsPointer { get; set; }
        public Pointer EventDataPointer { get; set; }
        public Pointer GraphicsGroupCountTablePointer { get; set; }
        public uint GraphicsGroupCount { get; set; }

        public byte[] GraphicsGroupCountTable { get; set; }

        public Pointer[] GraphicDataPointers { get; set; }
        public R1_GBA_EventGraphicsData[] GraphicData { get; set; }
        
        public Pointer[] EventDataPointers { get; set; }
        public R1_GBA_EventData[][] EventData { get; set; }

        public void SerializeData(SerializerObject s, Pointer eventGraphicsPointers, Pointer eventDataPointers, Pointer eventGraphicsGroupCountTablePointers, Pointer levelEventGraphicsGroupCounts)
        {
            // Get the global level index
            var levelIndex = ((R1_GBA_Manager)s.Context.Settings.GetGameManager).LoadData(s.Context).WorldLevelOffsetTable[s.GameSettings.World] + (s.GameSettings.Level - 1);

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
                GraphicData = new R1_GBA_EventGraphicsData[GraphicsGroupCount];

            for (int i = 0; i < GraphicData.Length; i++)
                s.DoAt(GraphicDataPointers[i], () => GraphicData[i] = s.SerializeObject<R1_GBA_EventGraphicsData>(GraphicData[i], name: $"{nameof(GraphicData)}[{i}]"));

            s.DoAt(EventDataPointer, () => EventDataPointers = s.SerializePointerArray(EventDataPointers, GraphicsGroupCount, name: nameof(EventDataPointers)));

            if (EventData == null)
                EventData = new R1_GBA_EventData[GraphicsGroupCount][];

            for (int i = 0; i < EventData.Length; i++)
            {
                if (EventDataPointers[i] != null)
                    s.DoAt(EventDataPointers[i], () => EventData[i] = s.SerializeObjectArray<R1_GBA_EventData>(EventData[i], GraphicsGroupCountTable[i], name: $"{nameof(EventData)}[{i}]"));
                else
                    EventData[i] = new R1_GBA_EventData[0];
            }
        }
    }
}