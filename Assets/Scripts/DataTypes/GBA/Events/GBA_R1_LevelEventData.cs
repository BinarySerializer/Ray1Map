namespace R1Engine
{
    /// <summary>
    /// Level event data for Rayman Advance (GBA)
    /// </summary>
    public class GBA_R1_LevelEventData : R1Serializable
    {
        /// <summary>
        /// The level index. Set this before serializing.
        /// </summary>
        public int LevelIndex { get; set; }

        public Pointer EventGraphicsPointer { get; set; }
        public Pointer Pointer2 { get; set; }
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
        public override void SerializeImpl(SerializerObject s)
        {
            // Get the pointer table
            var pointerTable = GBA_R1_PointerTable.GetPointerTable(s.GameSettings.GameModeSelection);

            // Serialize data
            s.DoAt(new Pointer(pointerTable[GBA_R1_ROMPointer.EventGraphicsPointers] + (uint)(4 * LevelIndex), this.Offset.file), 
                () => EventGraphicsPointer = s.SerializePointer(EventGraphicsPointer, name: nameof(EventGraphicsPointer)));
            s.DoAt(new Pointer(pointerTable[GBA_R1_ROMPointer.UnkLevelPointerArray2] + (uint)(4 * LevelIndex), this.Offset.file), 
                () => Pointer2 = s.SerializePointer(Pointer2,  name: nameof(Pointer2)));
            s.DoAt(new Pointer(pointerTable[GBA_R1_ROMPointer.EventGraphicsGroupCountTablePointers] + (uint)(4 * LevelIndex), this.Offset.file), 
                () => GraphicsGroupCountTablePointer = s.SerializePointer(GraphicsGroupCountTablePointer, name: nameof(GraphicsGroupCountTablePointer)));
            s.DoAt(new Pointer(pointerTable[GBA_R1_ROMPointer.LevelEventGraphicsGroupCounts] + (uint)(4 * LevelIndex), this.Offset.file), 
                () => GraphicsGroupCount = s.Serialize<uint>(GraphicsGroupCount, name: nameof(GraphicsGroupCount)));


            // Parse data from pointers
            s.DoAt(GraphicsGroupCountTablePointer, () => GraphicsGroupCountTable = s.SerializeArray<byte>(GraphicsGroupCountTable, GraphicsGroupCount, name: nameof(GraphicsGroupCountTable)));
            s.DoAt(EventGraphicsPointer, () => GraphicDataPointers = s.SerializePointerArray(GraphicDataPointers, GraphicsGroupCount, name: nameof(GraphicDataPointers)));

            if (GraphicData == null)
                GraphicData = new GBA_R1_EventGraphicsData[GraphicsGroupCount];

            for (int i = 0; i < GraphicData.Length; i++)
                s.DoAt(GraphicDataPointers[i], () => GraphicData[i] = s.SerializeObject<GBA_R1_EventGraphicsData>(GraphicData[i], name: $"{nameof(GraphicData)}[{i}]"));

            s.DoAt(Pointer2, () => EventDataPointers = s.SerializePointerArray(EventDataPointers, GraphicsGroupCount, name: nameof(EventDataPointers)));

            if (EventData == null)
                EventData = new GBA_R1_EventData[GraphicsGroupCount][];

            for (int i = 0; i < EventData.Length; i++)
                s.DoAt(EventDataPointers[i], () => EventData[i] = s.SerializeObjectArray<GBA_R1_EventData>(EventData[i], GraphicsGroupCountTable[i], name: $"{nameof(EventData)}[{i}]"));
        }
    }
}