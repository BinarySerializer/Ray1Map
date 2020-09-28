using System;
using UnityEngine;

namespace R1Engine
{
    public class R1MemoryData
    {
        public Pointer EventArrayOffset { get; set; }
        public Pointer RayEventOffset { get; set; }

        public Pointer TileArrayOffset { get; set; }
        public R1_PC_BigMap BigMap { get; set; }
        
        public void Update(SerializerObject s)
        {
            // Get the game memory offset
            Pointer gameMemoryOffset = s.CurrentPointer;

            // Rayman 1 (PC - 1.21)
            if (s.GameSettings.GameModeSelection == GameModeSelection.RaymanPC_1_21)
            {
                EventArrayOffset = s.DoAt(gameMemoryOffset + 0x16DDF0, () => s.SerializePointer(EventArrayOffset, name: nameof(EventArrayOffset)));
                RayEventOffset = gameMemoryOffset + 0x16F650;

                TileArrayOffset = s.DoAt(gameMemoryOffset + 0x16F640, () => s.SerializePointer(TileArrayOffset, name: nameof(TileArrayOffset)));
                BigMap = s.DoAt(gameMemoryOffset + 0x1631D8, () => s.SerializeObject<R1_PC_BigMap>(BigMap, name: nameof(BigMap)));
            }

            // Rayman Designer (PC)
            else if (s.GameSettings.GameModeSelection == GameModeSelection.RaymanDesignerPC)
            {
                EventArrayOffset = s.DoAt(gameMemoryOffset + 0x14A600, () => s.SerializePointer(EventArrayOffset, name: nameof(EventArrayOffset)));
                RayEventOffset = gameMemoryOffset + 0x14A4E8;

                // TODO: Find these
                //TileArrayOffset = s.DoAt(gameMemoryOffset + 0x16F640, () => s.SerializePointer(TileArrayOffset, name: nameof(TileArrayOffset)));
                //BigMap = s.DoAt(gameMemoryOffset + 0x1631D8, () => s.SerializeObject<PC_BigMap>(BigMap, name: nameof(BigMap)));
            }

            // Rayman EDU (PC)
            else if (s.GameSettings.EngineVersion == EngineVersion.R1_PC_Edu)
            {
                EventArrayOffset = s.DoAt(gameMemoryOffset + 0x16E338, () => s.SerializePointer(EventArrayOffset, name: nameof(EventArrayOffset)));

                // TODO: Find these
                //RayEventOffset = gameMemoryOffset + 0x14A4E8;
                //TileArrayOffset = s.DoAt(gameMemoryOffset + 0x16F640, () => s.SerializePointer(TileArrayOffset, name: nameof(TileArrayOffset)));
                //BigMap = s.DoAt(gameMemoryOffset + 0x1631D8, () => s.SerializeObject<PC_BigMap>(BigMap, name: nameof(BigMap)));
            }

            // Rayman Advance (GBA)
            else if (s.GameSettings.EngineVersion == EngineVersion.R1_GBA)
            {
                // TODO: Update the event class to support the GBA structure when in memory so that this will work!
                EventArrayOffset = gameMemoryOffset + 0x020226AE;

                TileArrayOffset = gameMemoryOffset + 0x02002230 + 4; // skip the width + height ushorts
                
                // TODO: Find this
                RayEventOffset = null;
            }

            else if (s.GameSettings.EngineVersion == EngineVersion.R1_PS1)
            {
                var manager = (R1_PS1BaseXXXManager)s.GameSettings.GetGameManager;
                EventArrayOffset = gameMemoryOffset + (FileFactory.Read<R1_PS1_LevFile>(manager.GetLevelFilePath(s.GameSettings), LevelEditorData.MainContext).EventData.EventsPointer.AbsoluteOffset);

                if (s.GameSettings.GameModeSelection == GameModeSelection.RaymanPS1US)
                    RayEventOffset = gameMemoryOffset + 0x801F61A0;

                // TODO: Set
                TileArrayOffset = null;
            }

            else
                throw new NotImplementedException("The selected game mode does currently not support memory loading");

            Debug.Log($"EventArrayOffset: {EventArrayOffset:X8}");
            Debug.Log($"RayEventOffset: {RayEventOffset:X8}");
            Debug.Log($"TileArrayOffset: {TileArrayOffset:X8}");
        }
    }
}