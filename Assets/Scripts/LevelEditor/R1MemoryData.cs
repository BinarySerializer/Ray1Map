using System;
using BinarySerializer;
using BinarySerializer.Ray1;
using UnityEngine;

namespace R1Engine
{
    public class R1MemoryData
    {
        public Pointer EventArrayOffset { get; set; }
        public Pointer RayEventOffset { get; set; }

        public Pointer TileArrayOffset { get; set; }
        public PC_BigMap BigMap { get; set; }
        
        public void Update(SerializerObject s)
        {
            // Get the game memory offset
            Pointer gameMemoryOffset = s.CurrentPointer;

            // Rayman 1 (PC - 1.21)
            if (s.GetR1Settings().GameModeSelection == GameModeSelection.RaymanPC_1_21)
            {
                EventArrayOffset = s.DoAt(gameMemoryOffset + 0x16DDF0, () => s.SerializePointer(EventArrayOffset, name: nameof(EventArrayOffset)));
                RayEventOffset = gameMemoryOffset + 0x16F650;

                TileArrayOffset = s.DoAt(gameMemoryOffset + 0x16F640, () => s.SerializePointer(TileArrayOffset, name: nameof(TileArrayOffset)));
                BigMap = s.DoAt(gameMemoryOffset + 0x1631D8, () => s.SerializeObject<PC_BigMap>(BigMap, name: nameof(BigMap)));
            }

            // Rayman Designer (PC)
            else if (s.GetR1Settings().GameModeSelection == GameModeSelection.RaymanDesignerPC)
            {
                EventArrayOffset = s.DoAt(gameMemoryOffset + 0x14A600, () => s.SerializePointer(EventArrayOffset, name: nameof(EventArrayOffset)));
                RayEventOffset = gameMemoryOffset + 0x14A4E8;

                // TODO: Find these
                //TileArrayOffset = s.DoAt(gameMemoryOffset + 0x16F640, () => s.SerializePointer(TileArrayOffset, name: nameof(TileArrayOffset)));
                //BigMap = s.DoAt(gameMemoryOffset + 0x1631D8, () => s.SerializeObject<PC_BigMap>(BigMap, name: nameof(BigMap)));
            }

            // Rayman EDU (PC)
            else if (s.GetR1Settings().EngineVersion == EngineVersion.R1_PC_Edu)
            {
                EventArrayOffset = s.DoAt(gameMemoryOffset + 0x16E338, () => s.SerializePointer(EventArrayOffset, name: nameof(EventArrayOffset)));

                // TODO: Find these
                //RayEventOffset = gameMemoryOffset + 0x14A4E8;
                //TileArrayOffset = s.DoAt(gameMemoryOffset + 0x16F640, () => s.SerializePointer(TileArrayOffset, name: nameof(TileArrayOffset)));
                //BigMap = s.DoAt(gameMemoryOffset + 0x1631D8, () => s.SerializeObject<PC_BigMap>(BigMap, name: nameof(BigMap)));
            }

            // Rayman Advance (GBA)
            else if (s.GetR1Settings().EngineVersion == EngineVersion.R1_GBA)
            {
                // TODO: Update the event class to support the GBA structure when in memory so that this will work!
                EventArrayOffset = gameMemoryOffset + 0x020226AE;

                TileArrayOffset = gameMemoryOffset + 0x02002230 + 4; // skip the width + height ushorts
                
                // TODO: Find this
                RayEventOffset = null;
            }

            // Rayman 1 (PS1 - US/EU/JP)
            else if (s.GetR1Settings().EngineVersion == EngineVersion.R1_PS1 || s.GetR1Settings().EngineVersion == EngineVersion.R1_PS1_JP)
            {
                var manager = (R1_PS1BaseXXXManager)s.GetR1Settings().GetGameManager;
                var lvl = FileFactory.Read<PS1_LevFile>(manager.GetLevelFilePath(s.GetR1Settings()), LevelEditorData.MainContext);
                EventArrayOffset = gameMemoryOffset + lvl.ObjData.ObjectsPointer.AbsoluteOffset;

                // US
                if (s.GetR1Settings().GameModeSelection == GameModeSelection.RaymanPS1US || s.GetR1Settings().GameModeSelection == GameModeSelection.RaymanPS1USDemo)
                    RayEventOffset = gameMemoryOffset + 0x801F61A0;

                // TODO: Find ray event offset for PAL, PAL demo & JP

                TileArrayOffset = gameMemoryOffset + lvl.MapData.Offset.AbsoluteOffset + 4; // skip the width + height ushorts
            }

            // Rayman 1 (PS1 - JP Demo 3/6)
            else if (s.GetR1Settings().EngineVersion == EngineVersion.R1_PS1_JPDemoVol3 || s.GetR1Settings().EngineVersion == EngineVersion.R1_PS1_JPDemoVol6)
            {
                var lvlPath = (s.GetR1Settings().EngineVersion == EngineVersion.R1_PS1_JPDemoVol3 
                    ? ((R1_PS1JPDemoVol3_Manager)s.GetR1Settings().GetGameManager).GetLevelFilePath(s.GetR1Settings()) 
                    : ((R1_PS1JPDemoVol6_Manager)s.GetR1Settings().GetGameManager).GetLevelFilePath(s.GetR1Settings()));

                var mapPath = (s.GetR1Settings().EngineVersion == EngineVersion.R1_PS1_JPDemoVol3 
                    ? ((R1_PS1JPDemoVol3_Manager)s.GetR1Settings().GetGameManager).GetMapFilePath(s.GetR1Settings()) 
                    : ((R1_PS1JPDemoVol6_Manager)s.GetR1Settings().GetGameManager).GetMapFilePath(s.GetR1Settings()));

                var lvl = FileFactory.Read<PS1_JPDemo_LevFile>(lvlPath, LevelEditorData.MainContext);
                var map = FileFactory.Read<MapData>(mapPath, LevelEditorData.MainContext);

                EventArrayOffset = gameMemoryOffset + lvl.ObjectsPointer.AbsoluteOffset;

                if (s.GetR1Settings().EngineVersion == EngineVersion.R1_PS1_JPDemoVol3)
                    RayEventOffset = gameMemoryOffset + 0x801DA898;

                // TODO: Get ray event offset for vol 6
                
                TileArrayOffset = gameMemoryOffset + map.Offset.AbsoluteOffset + 4; // skip the width + height ushorts
            }

            // Rayman 2 (PS1)
            else if (s.GetR1Settings().EngineVersion == EngineVersion.R2_PS1)
            {
                var manager = (R1_PS1R2_Manager)s.GetR1Settings().GetGameManager;

                EventArrayOffset = gameMemoryOffset + FileFactory.Read<R2_LevDataFile>(manager.GetLevelDataPath(s.GetR1Settings()), LevelEditorData.MainContext).LoadedObjectsPointer.AbsoluteOffset;
                RayEventOffset = gameMemoryOffset + 0x80178df0;

                // TODO: Find tile offsets
                //TileArrayOffset = gameMemoryOffset + lvl.MapData.Offset.AbsoluteOffset + 4; // skip the width + height ushorts
            }

            else
                throw new NotImplementedException("The selected game mode does currently not support memory loading");

            Debug.Log($"EventArrayOffset: {EventArrayOffset:X8}");
            Debug.Log($"RayEventOffset: {RayEventOffset:X8}");
            Debug.Log($"TileArrayOffset: {TileArrayOffset:X8}");
        }
    }
}