using BinarySerializer;

namespace R1Engine
{
    public class GBC_ActionTableBlock : GBC_BaseBlock 
    {
        public GBC_ActionTable ActionTable { get; set; }
        public GBC_Puppet Puppet { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            s.DoEndian(Endian.Little, () => {
                ActionTable = s.SerializeObject<GBC_ActionTable>(ActionTable, name: nameof(ActionTable));
            });
            Puppet = s.DoAt(DependencyTable.GetPointer(0), () => s.SerializeObject<GBC_Puppet>(Puppet, name: $"{nameof(Puppet)}"));
        }
    }
}