namespace R1Engine
{
    public class GBA_ROM : GBA_ROMBase
    {
        public GBA_Data Data { get; set; }

        // Each pointer leads to a small index list. They all begin with 0x00, so read until next 0x00? - probably irrelevant
        public Pointer[] UnkPointerTable { get; set; }

        // Contains general info about levels, but not anything map related
        public GBA_R3_SceneInfo[] LevelInfo { get; set; }

        public GBA_LocLanguageTable Localization { get; set; }
        public GBA_Milan_LocTable Milan_Localization { get; set; }

        public GBA_ActorTypeTableEntry[] ActorTypeTable { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize ROM header
            base.SerializeImpl(s);

            var manager = ((GBA_Manager)s.Context.Settings.GetGameManager);

            // Get the pointer table
            var pointerTable = PointerTables.GBA_PointerTable(s.Context, Offset.file);

            // Serialize the offset table
            s.DoAt(pointerTable[GBA_Pointer.UiOffsetTable], () => Data = s.SerializeObject<GBA_Data>(Data, name: nameof(Data)));

            // Serialize level info
            if (pointerTable.ContainsKey(GBA_Pointer.LevelInfo))
                LevelInfo = s.DoAt(pointerTable[GBA_Pointer.LevelInfo], () => s.SerializeObjectArray<GBA_R3_SceneInfo>(LevelInfo, manager.LevelCount, name: nameof(LevelInfo)));

            // Serialize localization
            if (pointerTable.ContainsKey(GBA_Pointer.Localization))
            {
                if (s.GameSettings.GBA_IsMilan)
                    s.DoAt(pointerTable[GBA_Pointer.Localization], () => Milan_Localization = s.SerializeObject<GBA_Milan_LocTable>(Milan_Localization, name: nameof(Milan_Localization)));
                else
                    s.DoAt(pointerTable[GBA_Pointer.Localization], () => Localization = s.SerializeObject<GBA_LocLanguageTable>(Localization, name: nameof(Localization)));
            }

            // Serialize actor type data
            if (pointerTable.ContainsKey(GBA_Pointer.ActorTypeTable))
                ActorTypeTable = s.DoAt(pointerTable[GBA_Pointer.ActorTypeTable], () => s.SerializeObjectArray<GBA_ActorTypeTableEntry>(ActorTypeTable, manager.ActorTypeTableLength, name: nameof(ActorTypeTable)));
        }
    }
}