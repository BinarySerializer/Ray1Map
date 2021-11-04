using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using BinarySerializer.GBA;

namespace Ray1Map.GBAVV
{
    public abstract class GBAVV_BaseROM : GBA_ROMBase
    {
        public bool SerializeFLC { get; set; } // Set before serializing

        public GBAVV_Script[] Scripts { get; set; }
        public GBAVV_Script[] HardCodedScripts { get; set; }
        public IEnumerable<GBAVV_Script> GetAllScripts => Scripts?.Concat(HardCodedScripts ?? new GBAVV_Script[0]) ?? new GBAVV_Script[0];
        public GBAVV_DialogScript[] DialogScripts { get; set; }
        public GBAVV_Graphics[] Map2D_Graphics { get; set; }

        protected void SerializeScripts(SerializerObject s)
        {
            // Get the pointer table
            var pointerTable = PointerTables.GBAVV_PointerTable(s.GetR1Settings().GameModeSelection, Offset.File);

            // Serialize scripts
            var scriptPointers = ((GBAVV_BaseManager)s.GetR1Settings().GetGameManager).ScriptPointers;

            if (scriptPointers != null)
            {
                if (Scripts == null)
                    Scripts = new GBAVV_Script[scriptPointers.Length];

                for (int i = 0; i < scriptPointers.Length; i++)
                    Scripts[i] = s.DoAt(new Pointer(scriptPointers[i], Offset.File), () => s.SerializeObject<GBAVV_Script>(Scripts[i], x =>
                    {
                        x.SerializeFLC = SerializeFLC;
                        x.BaseFile = Offset.File;
                    }, name: $"{nameof(Scripts)}[{i}]"));

                if (s.GetR1Settings().GBAVV_IsFusion && HardCodedScripts == null)
                    HardCodedScripts = s.DoAtBytes(((GBAVV_Fusion_Manager)s.GetR1Settings().GetGameManager).HardCodedScripts, nameof(HardCodedScripts), () => s.SerializeObjectArrayUntil<GBAVV_Script>(HardCodedScripts, x => s.CurrentFileOffset >= s.CurrentLength, onPreSerialize: x =>
                    {
                        x.SerializeFLC = SerializeFLC;
                        x.BaseFile = Offset.File;
                    }, name: nameof(HardCodedScripts)));

                DialogScripts = s.DoAt(pointerTable.TryGetItem(DefinedPointer.Fusion_DialogScripts), () => s.SerializeObjectArray<GBAVV_DialogScript>(DialogScripts, ((GBAVV_Fusion_Manager)s.GetR1Settings().GetGameManager).DialogScriptsCount, name: nameof(DialogScripts)));
            }
        }

        protected void SerializeGraphics(SerializerObject s)
        {
            // Get the graphics pointers
            var graphicsDataPointers = s.GetR1Settings().EngineVersion >= EngineVersion.GBAVV_CrashFusion && s.GetR1Settings().EngineVersion != EngineVersion.GBAVV_KidsNextDoorOperationSODA ? new uint[]
            {
                GBAConstants.Address_ROM // Dummy pointer
            } : s.GetR1Settings().GetGameManagerOfType<GBAVV_BaseManager>().GraphicsDataPointers;

            // Serialize graphics
            if (Map2D_Graphics == null)
                Map2D_Graphics = new GBAVV_Graphics[graphicsDataPointers.Length];

            for (int i = 0; i < graphicsDataPointers.Length; i++)
                Map2D_Graphics[i] = s.DoAt(new Pointer(graphicsDataPointers[i], Offset.File), () => s.SerializeObject<GBAVV_Graphics>(Map2D_Graphics[i], name: $"{nameof(Map2D_Graphics)}[{i}]"));
        }
    }
}