using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_Ice_ROM : GBAIsometric_IceDragon_BaseROM
    {


        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize ROM header and base data
            base.SerializeImpl(s);

            // TODO: Serialize Season of Ice specific data
        }
    }
}