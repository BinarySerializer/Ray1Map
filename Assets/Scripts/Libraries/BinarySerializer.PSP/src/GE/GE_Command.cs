namespace BinarySerializer.PSP
{
    /// <see href="http://hitmen.c02.at/files/yapspd/psp_doc/chap11.html#sec11.1">GE Command Format</see>
    public class GE_Command : BinarySerializable
    {
        public GE_CommandType Command { get; set; }
        public GE_CommandData Data { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            s.DoBits<uint>(b => {
                b.Position = 24;
                Command = b.SerializeBits<GE_CommandType>(Command, 8, name: nameof(Command));
                b.Position = 0;

                T SerializeData<T>() where T : GE_CommandData, new() {
                    return b.SerializeObject<T>((T)Data, onPreSerialize: cmd => cmd.Pre_Command = this, name: nameof(Data));
                }

                Data = Command switch {
                    GE_CommandType.IADDR or
                    GE_CommandType.VADDR or
                    GE_CommandType.JUMP or
                    GE_CommandType.BJUMP or
                    GE_CommandType.CALL => SerializeData<GE_Command_Address>(),
                    _ => SerializeData<GE_Command_Placeholder>(),
                };
            });
        }
    }
}