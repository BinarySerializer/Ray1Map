namespace BinarySerializer.PSP
{
    /// <see href="http://hitmen.c02.at/files/yapspd/psp_doc/chap11.html#sec11.1">GE Command Format</see>
    public class GE_Command : BinarySerializable
    {
        public GE_CommandType Command { get; set; }
        public GE_CommandData Data { get; set; }


        // Linked data
        public GE_VertexLine[] LinkedVertices { get; set; }
        public GE_Texture LinkedTextureData { get; set; }

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
                    GE_CommandType.CALL or
                    GE_CommandType.TBP0 or
					GE_CommandType.TBP1 or
					GE_CommandType.TBP2 or
					GE_CommandType.TBP3 or
					GE_CommandType.TBP4 or
					GE_CommandType.TBP5 or
					GE_CommandType.TBP6 or
					GE_CommandType.TBP7 => SerializeData<GE_Command_Address>(),

                    GE_CommandType.END or
                    GE_CommandType.RET or
                    GE_CommandType.NOP or
                    GE_CommandType.TFLUSH => null,

                    GE_CommandType.LTE or
                    GE_CommandType.LTE0 or
					GE_CommandType.LTE1 or
					GE_CommandType.LTE2 or
					GE_CommandType.LTE3 or
					GE_CommandType.CPE or
					GE_CommandType.BCE or
					GE_CommandType.TME or
					GE_CommandType.FGE or
					GE_CommandType.DTE or
					GE_CommandType.ABE or
					GE_CommandType.ATE or
					GE_CommandType.ZTE or
					GE_CommandType.STE or
					GE_CommandType.PCE or
					GE_CommandType.CTE or
					GE_CommandType.LOE or
					GE_CommandType.RNORM => SerializeData<GE_Command_Enable>(),

                    GE_CommandType.VTYPE => SerializeData<GE_Command_VertexType>(),

                    GE_CommandType.BASE => SerializeData<GE_Command_Base>(),

                    GE_CommandType.PRIM => SerializeData<GE_Command_PrimitiveKick>(),

                    GE_CommandType.TFUNC => SerializeData<GE_Command_TextureFunction>(),

                    GE_CommandType.TMODE => SerializeData<GE_Command_TextureMode>(),

                    GE_CommandType.TPSM => SerializeData<GE_Command_TexturePixelStorageMode>(),

                    GE_CommandType.TBW0 or
					GE_CommandType.TBW1 or
					GE_CommandType.TBW2 or
					GE_CommandType.TBW3 or
					GE_CommandType.TBW4 or
					GE_CommandType.TBW5 or
					GE_CommandType.TBW6 or
                    GE_CommandType.TBW7 => SerializeData<GE_Command_TextureBufferWidth>(),

					GE_CommandType.TSIZE0 or
					GE_CommandType.TSIZE1 or
					GE_CommandType.TSIZE2 or
					GE_CommandType.TSIZE3 or
					GE_CommandType.TSIZE4 or
					GE_CommandType.TSIZE5 or
					GE_CommandType.TSIZE6 or
					GE_CommandType.TSIZE7 => SerializeData<GE_Command_TextureSize>(),

					_ => SerializeData<GE_Command_Placeholder>(),
                };
            });
        }
    }
}