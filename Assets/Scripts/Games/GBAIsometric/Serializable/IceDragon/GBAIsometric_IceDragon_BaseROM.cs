using System.Collections.Generic;
using BinarySerializer;
using BinarySerializer.GBA;

namespace Ray1Map.GBAIsometric
{
    public abstract class GBAIsometric_IceDragon_BaseROM : GBA_ROMBase
    {
        public GBAIsometric_IceDragon_Resources Resources { get; set; }
        public GBAIsometric_IceDragon_Localization Localization { get; set; }
        public GBAIsometric_IceDragon_CutsceneMap[] CutsceneMaps { get; set; }
        public GBAIsometric_IceDragon_MenuPage[] MenuPages { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize ROM header
            base.SerializeImpl(s);

            // Get the settings and pointers
            Dictionary<Spyro_DefinedPointer, Pointer> pointerTable = PointerTables.GBAIsometric_Spyro_PointerTable(s.GetR1Settings().GameModeSelection, Offset.File);
            GBAIsometricSettings settings = s.GetSettings<GBAIsometricSettings>();

            // Serialize the resources and store it so we can access them later
            Resources = s.DoAt(pointerTable[Spyro_DefinedPointer.DataTable], 
                () => s.SerializeObject<GBAIsometric_IceDragon_Resources>(Resources, name: nameof(Resources)));
            s.Context.StoreObject(nameof(Resources), Resources);

            // Serialize the localization data
            if (settings.EngineVersion != GBAIsometricEngineVersion.Tron)
                Localization = s.SerializeObject<GBAIsometric_IceDragon_Localization>(Localization, name: nameof(Localization));

            // Serialize cutscene maps
            CutsceneMaps = s.DoAt(pointerTable.TryGetItem(Spyro_DefinedPointer.CutsceneMaps), () =>
                s.SerializeObjectArray<GBAIsometric_IceDragon_CutsceneMap>(CutsceneMaps, settings.CutsceneMapsCount, name: nameof(CutsceneMaps)));

            // Serialize menu pages
            MenuPages = s.DoAt(pointerTable.TryGetItem(Spyro_DefinedPointer.MenuPages), () => 
                s.SerializeObjectArray<GBAIsometric_IceDragon_MenuPage>(MenuPages, settings.MenuPageCount, name: nameof(MenuPages)));
        }
    }
}