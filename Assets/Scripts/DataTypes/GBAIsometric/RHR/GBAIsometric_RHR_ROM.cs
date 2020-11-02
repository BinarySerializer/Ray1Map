namespace R1Engine
{
    public class GBAIsometric_RHR_ROM : GBA_ROMBase
    {
        public GBAIsometric_RHR_LocalizationTable Localization { get; set; }

        public GBAIsometric_RHR_PaletteAnimationTable[] PaletteAnimations { get; set; }
        public GBAIsometric_RHR_LevelInfo[] LevelInfos { get; set; }
        public GBAIsometric_RHR_ObjectType[] ObjectTypes { get; set; }

        public GBAIsometric_RHR_MapLayer[] MenuMaps { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize ROM header
            base.SerializeImpl(s);

            var pointerTable = PointerTables.GBAIsometric_RHR_PointerTable(s.GameSettings.GameModeSelection, Offset.file);

            // Serialize localization
            Localization = s.DoAt(pointerTable[GBAIsometric_RHR_Pointer.Localization], () => s.SerializeObject<GBAIsometric_RHR_LocalizationTable>(Localization, name: nameof(Localization)));

            if (s.GameSettings.World == 0)
            {
                // Serialize level infos
                s.DoAt(pointerTable[GBAIsometric_RHR_Pointer.Levels], () =>
                {
                    if (LevelInfos == null)
                        LevelInfos = new GBAIsometric_RHR_LevelInfo[20];

                    for (int i = 0; i < LevelInfos.Length; i++)
                        LevelInfos[i] = s.SerializeObject(LevelInfos[i], x => x.SerializeData = i == s.GameSettings.Level, name: $"{nameof(LevelInfos)}[{i}]");
                });

                PaletteAnimations = new GBAIsometric_RHR_PaletteAnimationTable[3];
                s.DoAt(pointerTable[GBAIsometric_RHR_Pointer.PaletteAnimations0], () => {
                    PaletteAnimations[0] = s.SerializeObject<GBAIsometric_RHR_PaletteAnimationTable>(PaletteAnimations[0], name: $"{nameof(PaletteAnimations)}[0]");
                });
                s.DoAt(pointerTable[GBAIsometric_RHR_Pointer.PaletteAnimations1], () => {
                    PaletteAnimations[1] = s.SerializeObject<GBAIsometric_RHR_PaletteAnimationTable>(PaletteAnimations[1], name: $"{nameof(PaletteAnimations)}[1]");
                });
                s.DoAt(pointerTable[GBAIsometric_RHR_Pointer.PaletteAnimations2], () => {
                    PaletteAnimations[2] = s.SerializeObject<GBAIsometric_RHR_PaletteAnimationTable>(PaletteAnimations[2], name: $"{nameof(PaletteAnimations)}[2]");
                });

                // Serialize object types
                ObjectTypes = s.DoAt(pointerTable[GBAIsometric_RHR_Pointer.ObjTypes], () => s.SerializeObjectArray<GBAIsometric_RHR_ObjectType>(ObjectTypes, 105, name: nameof(ObjectTypes)));
            }
            else
            {
                var maps = ((GBAIsometric_RHR_Manager)s.GameSettings.GetGameManager).GetMenuMaps(s.GameSettings.Level);
                MenuMaps = new GBAIsometric_RHR_MapLayer[maps.Length];

                for (int i = 0; i < MenuMaps.Length; i++)
                    MenuMaps[i] = s.DoAt(pointerTable[maps[i]], () => s.SerializeObject<GBAIsometric_RHR_MapLayer>(default, name: $"{maps[i]}"));

                /*
                 * Palette shift for Digital Eclipse logo (StartIndex,EndIndex,speed)
                 *  ShiftPaletteDigitalEclipse(0x12,0x20,1);
                    ShiftPaletteDigitalEclipse(0x22,0x30,1);
                    ShiftPaletteDigitalEclipse(0x32,0x40,1);
                    ShiftPaletteDigitalEclipse(0x42,0xd0,1);
                    ShiftPaletteDigitalEclipse(0xd1,0xf0,1);
                 * */
            }

            /*
            s.DoAt(new Pointer(0x080efd28, Offset.file), () => {
                var sprite = s.SerializeObject<GBAIsometric_Sprite>(default, name: "Sprite");

            });
            s.DoAt(new Pointer(0x080ef918, Offset.file), () => {
                var sprite = s.SerializeObject<GBAIsometric_Sprite>(default, name: "Sprite");
            }); */
        }
    }
}