namespace BinarySerializer.PSP
{
    public abstract class GE_CommandData : BitSerializable 
    {
        public GE_Command Pre_Command { get; set; }
    }
}