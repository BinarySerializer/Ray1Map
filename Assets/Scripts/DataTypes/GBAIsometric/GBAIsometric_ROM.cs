namespace R1Engine
{
    public class GBAIsometric_ROM : GBA_ROMBase
    {
        public GBAIsometric_LocalizationTable Localization { get; set; }

        public GBAIsometric_LevelInfo[] LevelInfos { get; set; }
        public GBAIsometric_ObjectType[] ObjectTypes { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize ROM header
            base.SerializeImpl(s);

            var pointerTable = PointerTables.GBAIsometric_PointerTable(s.GameSettings.GameModeSelection, Offset.file);

            // Serialize localization
            Localization = s.DoAt(pointerTable[GBAIsometric_Pointer.Localization], () => s.SerializeObject<GBAIsometric_LocalizationTable>(Localization, name: nameof(Localization)));

            // Serialize level infos
            LevelInfos = s.DoAt(pointerTable[GBAIsometric_Pointer.Levels], () => s.SerializeObjectArray<GBAIsometric_LevelInfo>(LevelInfos, 20, name: nameof(LevelInfos)));

            // Serialize object types
            ObjectTypes = s.DoAt(pointerTable[GBAIsometric_Pointer.ObjTypes], () => s.SerializeObjectArray<GBAIsometric_ObjectType>(ObjectTypes, 105, name: nameof(ObjectTypes)));
        }
    }
}