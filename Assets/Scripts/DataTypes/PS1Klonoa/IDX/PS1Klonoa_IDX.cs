using BinarySerializer;

namespace R1Engine
{
    public class PS1Klonoa_IDX : BinarySerializable
    {
        public string Header { get; set; }
        public PS1Klonoa_IDXEntry[] Entries { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Header = s.SerializeString(Header, 8, name: nameof(Header));
            Entries = s.SerializeObjectArray<PS1Klonoa_IDXEntry>(Entries, 25, name: nameof(Entries));
        }
    }
}