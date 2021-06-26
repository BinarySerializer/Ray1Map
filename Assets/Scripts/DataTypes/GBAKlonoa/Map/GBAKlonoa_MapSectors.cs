using BinarySerializer;

namespace R1Engine
{
    public class GBAKlonoa_MapSectors : BinarySerializable
    {
        public GBAKlonoa_MapSector[] Sectors { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Sectors = s.SerializeObjectArray<GBAKlonoa_MapSector>(Sectors, 6, name: nameof(Sectors));
            s.SerializePadding(112, logIfNotNull: true);
        }
    }
}