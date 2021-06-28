using BinarySerializer;

namespace R1Engine
{
    public class GBAKlonoa_LevelObjectCollection : BinarySerializable
    {
        public GBAKlonoa_LevelObject[] Objects { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Objects = s.SerializeObjectArrayUntil(Objects, x => x.SectorStates[0].XPos == -1, () => new GBAKlonoa_LevelObject()
            {
                SectorStates = new GBAKlonoa_LevelObject.SectorState[]
                {
                    new GBAKlonoa_LevelObject.SectorState()
                    {
                        XPos = -1
                    },
                    new GBAKlonoa_LevelObject.SectorState(),
                    new GBAKlonoa_LevelObject.SectorState(),
                    new GBAKlonoa_LevelObject.SectorState(),
                    new GBAKlonoa_LevelObject.SectorState(),
                }
            }, name: nameof(Objects));
        }
    }
}