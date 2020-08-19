namespace R1Engine
{
    public class GBA_ROM : GBA_ROMBase
    {
        public GBA_Data Data { get; set; }

        // Each pointer leads to a small index list. They all begin with 0x00, so read until next 0x00? - probably irrelevant
        public Pointer[] UnkPointerTable { get; set; }

        // Contains general info about levels, but not anything map related
        public GBA_R3_LevelMapInfo[] LevelInfo { get; set; }

        public GBA_LocLanguageTable Localization { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize ROM header
            base.SerializeImpl(s);

            var levelCount = ((GBA_Manager)s.Context.Settings.GetGameManager).LevelCount;

            // Get the pointer table
            var pointerTable = PointerTables.GBA_PointerTable(s.GameSettings.GameModeSelection, Offset.file);

            // Serialize the offset table
            s.DoAt(pointerTable[GBA_Pointer.UiOffsetTable], () => Data = s.SerializeObject<GBA_Data>(Data, name: nameof(Data)));

            // Serialize level info
            if (pointerTable.ContainsKey(GBA_Pointer.LevelInfo))
                LevelInfo = s.DoAt(pointerTable[GBA_Pointer.LevelInfo], () => s.SerializeObjectArray<GBA_R3_LevelMapInfo>(LevelInfo, levelCount, name: nameof(LevelInfo)));

            // Serialize localization
            if (pointerTable.ContainsKey(GBA_Pointer.Localization))
                s.DoAt(pointerTable[GBA_Pointer.Localization], () => Localization = s.SerializeObject<GBA_LocLanguageTable>(Localization, name: nameof(Localization)));
        }
    }
}