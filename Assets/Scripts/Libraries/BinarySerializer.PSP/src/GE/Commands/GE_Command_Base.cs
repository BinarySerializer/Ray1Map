namespace BinarySerializer.PSP 
{
    public class GE_Command_Base : GE_CommandData 
    {
        public uint Base { get; set; }

        public override void SerializeImpl(BitSerializerObject b) {
            b.SerializePadding(16, logIfNotNull: true);
            Base = b.SerializeBits<uint>(Base, 4, name: nameof(Base));
            b.SerializePadding(4, logIfNotNull: true);
        }
    }
}