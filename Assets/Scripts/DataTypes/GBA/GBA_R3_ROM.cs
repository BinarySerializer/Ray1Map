namespace R1Engine
{
    public class GBA_R3_ROM : GBA_ROM
    {
        public GBA_Data Data { get; set; }

        // Each pointer leads to a small index list. They all begin with 0x00, so read until next 0x00? - probably irrelevant
        public Pointer[] UnkPointerTable { get; set; }

        // Contains general info about levels, but not anything map related
        public GBA_R3_LevelMapInfo[] LevelInfo { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize ROM header
            base.SerializeImpl(s);

            // TODO: Prototype has 64 maps
            const int levelCount = 65;

            // Get the pointer table
            var pointerTable = PointerTables.GetGBAR3PointerTable(s.GameSettings.GameModeSelection, Offset.file);

            // Serialize the offset table
            s.DoAt(pointerTable[GBA_R3_Pointer.UiOffsetTable], () => Data = s.SerializeObject<GBA_Data>(Data, name: nameof(Data)));

            // Serialize unknown pointer table
            if (pointerTable.ContainsKey(GBA_R3_Pointer.UnkPointerTable))
                UnkPointerTable = s.DoAt(pointerTable[GBA_R3_Pointer.UnkPointerTable], () => s.SerializePointerArray(UnkPointerTable, 252, name: nameof(UnkPointerTable)));

            // Serialize level info
            if (pointerTable.ContainsKey(GBA_R3_Pointer.LevelInfo))
                LevelInfo = s.DoAt(pointerTable[GBA_R3_Pointer.LevelInfo], () => s.SerializeObjectArray<GBA_R3_LevelMapInfo>(LevelInfo, levelCount, name: nameof(LevelInfo)));
        }
    }
}