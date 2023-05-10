namespace BinarySerializer.PSP 
{
    public class GE_Command_Enable : GE_CommandData 
    {
        public bool Enable { get; set; }

        public override void SerializeImpl(BitSerializerObject b) {
            Enable = b.SerializeBits<bool>(Enable, 1, name: nameof(Enable));
			b.SerializePadding(3 * 8 - 1);
		}
    }
}