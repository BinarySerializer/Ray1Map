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
            s.DoAt(pointerTable[GBAIsometric_Pointer.Levels], () =>
            {
                if (LevelInfos == null)
                    LevelInfos = new GBAIsometric_LevelInfo[20];

                for (int i = 0; i < LevelInfos.Length; i++)
                    LevelInfos[i] = s.SerializeObject(LevelInfos[i], x => x.SerializeData = i == s.GameSettings.Level, name: $"{nameof(LevelInfos)}[{i}]");
            });

            // Serialize object types
            ObjectTypes = s.DoAt(pointerTable[GBAIsometric_Pointer.ObjTypes], () => s.SerializeObjectArray<GBAIsometric_ObjectType>(ObjectTypes, 105, name: nameof(ObjectTypes)));

            /*
            s.DoAt(new Pointer(0x080efd28, Offset.file), () => {
                var sprite = s.SerializeObject<GBAIsometric_Sprite>(default, name: "Sprite");

            });
            s.DoAt(new Pointer(0x080ef918, Offset.file), () => {
                var sprite = s.SerializeObject<GBAIsometric_Sprite>(default, name: "Sprite");
            });
            s.DoAt(new Pointer(0x08481930, Offset.file), () => {
                s.SerializeObject<GBAIsometric_MapLayer>(default, name: "MapLayerPause");
            });
            s.DoAt(new Pointer(0x084818f0, Offset.file), () => {
                s.SerializeObject<GBAIsometric_MapLayer>(default, name: "MapLayerMenu");
            });
            s.DoAt(new Pointer(0x084817f4, Offset.file), () => {
                s.SerializeObject<GBAIsometric_MapLayer>(default, name: "MapLayerMenu");
            });
            s.DoAt(new Pointer(0x08481838, Offset.file), () => {
                s.SerializeObject<GBAIsometric_MapLayer>(default, name: "MapMenuChains");
            });
            s.DoAt(new Pointer(0x08481888, Offset.file), () => {
                s.SerializeObject<GBAIsometric_MapLayer>(default, name: "MapLayerMenu");
            });
            s.DoAt(new Pointer(0x08481bf0, Offset.file), () => {
                s.SerializeObject<GBAIsometric_MapLayer>(default, name: "WorldMap");
            });
            s.DoAt(new Pointer(0x08481970, Offset.file), () => {
                s.SerializeObject<GBAIsometric_MapLayer>(default, name: "???");
            });
            s.DoAt(new Pointer(0x08481970, Offset.file), () => {
                s.SerializeObject<GBAIsometric_MapLayer>(default, name: "WorldMap");
            });*/
        }
    }
}