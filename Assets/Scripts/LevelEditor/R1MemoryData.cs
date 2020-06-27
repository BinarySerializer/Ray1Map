using System;

namespace R1Engine
{
    public class R1MemoryData
    {
        public Pointer EventArrayOffset { get; set; }
        public Pointer RayEventOffset { get; set; }
        public Pointer TileArrayOffset { get; set; }

        public void UpdatePointers(SerializerObject s, Pointer gameMemoryOffset)
        {
            if (s.GameSettings.EngineVersion == EngineVersion.RayPC)
            {
                // For version 1.21 - verify it works for other ones too!
                EventArrayOffset = s.DoAt(gameMemoryOffset + 0x16DDF0, () => s.SerializePointer(EventArrayOffset, anchor: gameMemoryOffset, name: nameof(EventArrayOffset)));
                TileArrayOffset = s.DoAt(gameMemoryOffset + 0x16F640, () => s.SerializePointer(TileArrayOffset, anchor: gameMemoryOffset, name: nameof(TileArrayOffset)));
                RayEventOffset = gameMemoryOffset + 0x16F650;
            }
            else
            {
                throw new NotImplementedException("The selected game mode does currently not support memory loading");
            }
        }
    }
}