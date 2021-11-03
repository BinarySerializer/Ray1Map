﻿using BinarySerializer;

namespace Ray1Map.GBAVV
{
    public class GBAVV_ROM_MapArray : GBAVV_BaseROM
    {
        // Helpers
        public GBAVV_Map CurrentMap => Maps[Context.GetR1Settings().Level];

        // Common
        public GBAVV_Map[] Maps { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize ROM header
            base.SerializeImpl(s);

            // Get the pointer table
            var pointerTable = PointerTables.GBAVV_PointerTable(s.GetR1Settings().GameModeSelection, Offset.File);

            // Serialize level infos
            s.DoAt(pointerTable.TryGetItem(GBAVV_Pointer.LevelInfo), () =>
            {
                if (Maps == null)
                    Maps = new GBAVV_Map[s.GetR1Settings().GetGameManagerOfType<GBAVV_MapArray_BaseManager>().LevelsCount];

                for (int i = 0; i < Maps.Length; i++)
                    Maps[i] = s.SerializeObject<GBAVV_Map>(Maps[i], x => x.SerializeData = i == s.GetR1Settings().Level, name: $"{nameof(Maps)}[{i}]");
            });

            // Serialize graphics
            SerializeGraphics(s);

            // Serialize scripts
            SerializeScripts(s);
        }
    }
}