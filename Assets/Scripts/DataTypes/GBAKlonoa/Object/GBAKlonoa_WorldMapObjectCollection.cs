using BinarySerializer;

namespace R1Engine
{
    public class GBAKlonoa_WorldMapObjectCollection : BinarySerializable
    {
        public GBAKlonoa_WorldMapObject[] Objects { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Objects = s.SerializeObjectArrayUntil(Objects, x => x.XPos == -1, () => new GBAKlonoa_WorldMapObject()
            {
                XPos = -1
            }, name: nameof(Objects));
        }
    }
}