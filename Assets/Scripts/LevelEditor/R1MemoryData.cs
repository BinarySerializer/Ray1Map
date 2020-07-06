using System;

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
            Pointer gameMemoryOffset = s.CurrentPointer;

            // Rayman 1 (PC)
            if (s.GameSettings.EngineVersion == EngineVersion.RayPC)
            {
                // For version 1.21 - verify it works for other ones too!
                EventArrayOffset = s.DoAt(gameMemoryOffset + 0x16DDF0, () => s.SerializePointer(EventArrayOffset, name: nameof(EventArrayOffset)));
                RayEventOffset = gameMemoryOffset + 0x16F650;

                TileArrayOffset = s.DoAt(gameMemoryOffset + 0x16F640, () => s.SerializePointer(TileArrayOffset, name: nameof(TileArrayOffset)));
                BigMap = s.DoAt(gameMemoryOffset + 0x1631D8, () => s.SerializeObject<PC_BigMap>(BigMap, name: nameof(BigMap)));
            }
            // Rayman Designer (PC)
            else if (s.GameSettings.EngineVersion == EngineVersion.RayKitPC)
            {
                EventArrayOffset = s.DoAt(gameMemoryOffset + 0x14A600, () => s.SerializePointer(EventArrayOffset, name: nameof(EventArrayOffset)));
                RayEventOffset = gameMemoryOffset + 0x14A4E8;

                // TODO: Find these
                //TileArrayOffset = s.DoAt(gameMemoryOffset + 0x16F640, () => s.SerializePointer(TileArrayOffset, name: nameof(TileArrayOffset)));
                //BigMap = s.DoAt(gameMemoryOffset + 0x1631D8, () => s.SerializeObject<PC_BigMap>(BigMap, name: nameof(BigMap)));
            }
            // Rayman Advance (GBA)
            else if (s.GameSettings.EngineVersion == EngineVersion.RayGBA)
            {
                // TODO: Update the event class to support the GBA structure when in memory so that this will work!
                EventArrayOffset = gameMemoryOffset + 0x020226AE;

                TileArrayOffset = gameMemoryOffset + 0x02002230 + 4; // skip the width + height ushorts
                
                // TODO: Find this
                RayEventOffset = null;
            }
            else
            {
                throw new NotImplementedException("The selected game mode does currently not support memory loading");
            }
        }
    }
}