namespace R1Engine
{
    /// <summary>
    /// Data for Rayman 1 (DSi)
    /// </summary>
    public class DSi_R1_DataFile : R1Serializable
    {
        /// <summary>
        /// The data for the level
        /// </summary>
        public DSi_R1_LevelMapData LevelMapData { get; set; }

        /// <summary>
        /// The event data for the current level
        /// </summary>
        public GBA_R1_LevelEventData LevelEventData { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Get the global level index
            var levelIndex = new DSi_R1_Manager().GetGlobalLevelIndex(s.GameSettings.World, s.GameSettings.Level);

            // Get the pointer table
            var pointerTable = PointerTables.GetDSiPointerTable(s.GameSettings.GameModeSelection, this.Offset.file);

            // Serialize data from the ROM
            s.DoAt((s.GameSettings.World == World.Jungle ? pointerTable[DSi_R1_Pointer.JungleMaps] : pointerTable[DSi_R1_Pointer.LevelMaps]) + (levelIndex * 32), 
                () => LevelMapData = s.SerializeObject<DSi_R1_LevelMapData>(LevelMapData, name: nameof(LevelMapData)));

            // Serialize the level event data
            LevelEventData = new GBA_R1_LevelEventData();
            LevelEventData.SerializeData(s, pointerTable[DSi_R1_Pointer.EventGraphicsPointers], pointerTable[DSi_R1_Pointer.EventDataPointers], pointerTable[DSi_R1_Pointer.EventGraphicsGroupCountTablePointers], pointerTable[DSi_R1_Pointer.LevelEventGraphicsGroupCounts]);
        }
    }

    // TODO: Merge with GBA_R1_LevelMapData
    public class DSi_R1_LevelMapData : R1Serializable
    {
        #region Level Data

        // Always 0
        public uint UnkDword_00 { get; set; }

        // Tile data?
        public Pointer UnkPointer_04 { get; set; }

        // Always 0
        public uint UnkDword_08 { get; set; }

        public Pointer MapDataPointer { get; set; }

        // Always 0
        public uint UnkDword_10 { get; set; }

        // Block indexes?
        public Pointer UnkPointer_14 { get; set; }
        
        public Pointer UnkPointer_18 { get; set; }

        // Flags?
        public uint Unk_1C { get; set; }

        #endregion

        #region Parsed from Pointers

        /// <summary>
        /// The map data
        /// </summary>
        public Mapper_Map MapData { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize values
            UnkDword_00 = s.Serialize<uint>(UnkDword_00, name: nameof(UnkDword_00));
            UnkPointer_04 = s.SerializePointer(UnkPointer_04, name: nameof(UnkPointer_04));
            UnkDword_08 = s.Serialize<uint>(UnkDword_08, name: nameof(UnkDword_08));
            MapDataPointer = s.SerializePointer(MapDataPointer, name: nameof(MapDataPointer));
            UnkDword_10 = s.Serialize<uint>(UnkDword_10, name: nameof(UnkDword_10));
            UnkPointer_14 = s.SerializePointer(UnkPointer_14, name: nameof(UnkPointer_14));
            UnkPointer_18 = s.SerializePointer(UnkPointer_18, name: nameof(UnkPointer_18));
            Unk_1C = s.Serialize<uint>(Unk_1C, name: nameof(Unk_1C));
        }

        public void SerializeLevelData(SerializerObject s)
        {
            s.DoAt(MapDataPointer, () => {
                s.DoEncoded(new LZSSEncoder(), () => MapData = s.SerializeObject<Mapper_Map>(MapData, name: nameof(MapData)));
            });
        }

        #endregion
    }
}