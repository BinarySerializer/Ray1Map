using BinarySerializer;

namespace R1Engine
{
    public class GBAKlonoa_Objects : BinarySerializable
    {
        public GBAKlonoa_Object[] Objects { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Objects = s.SerializeObjectArrayUntil(Objects, x => x.SectorStates[0].XPos == -1, () => new GBAKlonoa_Object()
            {
                SectorStates = new GBAKlonoa_Object.SectorState[]
                {
                    new GBAKlonoa_Object.SectorState()
                    {
                        XPos = -1
                    },
                    new GBAKlonoa_Object.SectorState(),
                    new GBAKlonoa_Object.SectorState(),
                    new GBAKlonoa_Object.SectorState(),
                    new GBAKlonoa_Object.SectorState(),
                }
            }, name: nameof(Objects));
        }
    }
}