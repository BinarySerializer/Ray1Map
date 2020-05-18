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
        public GBA_R1_LevelMapData LevelMapData { get; set; }

        /// <summary>
        /// The event data for the current level
        /// </summary>
        public GBA_R1_LevelEventData LevelEventData { get; set; }

        public ARGB1555Color[] SpritePalettes { get; set; }

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
                () => LevelMapData = s.SerializeObject<GBA_R1_LevelMapData>(LevelMapData, name: nameof(LevelMapData)));

            // Serialize the level event data
            LevelEventData = new GBA_R1_LevelEventData();
            LevelEventData.SerializeData(s, pointerTable[DSi_R1_Pointer.EventGraphicsPointers], pointerTable[DSi_R1_Pointer.EventDataPointers], pointerTable[DSi_R1_Pointer.EventGraphicsGroupCountTablePointers], pointerTable[DSi_R1_Pointer.LevelEventGraphicsGroupCounts]);

            // TODO: Fix
            s.DoAt(pointerTable[DSi_R1_Pointer.JungleMaps], () =>
            {
                var jun1 = s.SerializeObject<GBA_R1_LevelMapData>(LevelMapData, name: nameof(LevelMapData));
                jun1.SerializeLevelData(s);
                SpritePalettes = jun1.TilePalettes;
            });
        }
    }
}