namespace BinarySerializer.PSP 
{
    public class GE_Command_Address : GE_CommandData 
    {
        public uint Address { get; set; }

        public override void SerializeImpl(BitSerializerObject b) {
            Address = b.SerializeBits<uint>(Address, 24, name: nameof(Address));
        }
    }
}