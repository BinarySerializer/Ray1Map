using BinarySerializer;
using BinarySerializer.GBA;

namespace Ray1Map.GBAVV
{
    public class GBAVV_ROM_Default : GBAVV_BaseROM
    {
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize ROM header
            base.SerializeImpl(s);

            // Serialize graphics
            SerializeGraphics(s);

            // Serialize scripts
            SerializeScripts(s);
        }
    }
}