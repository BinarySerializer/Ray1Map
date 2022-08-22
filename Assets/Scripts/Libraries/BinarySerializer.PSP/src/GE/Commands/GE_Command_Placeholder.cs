namespace BinarySerializer.PSP 
{
    public class GE_Command_Placeholder : GE_CommandData 
    {
        public override void SerializeImpl(BitSerializerObject b) {
            b.Context.SystemLog?.LogWarning("{0}: Unparsed RSP Command: {1}", Offset, Pre_Command?.Command);
            b.SerializePadding(3 * 8);
        }
    }
}