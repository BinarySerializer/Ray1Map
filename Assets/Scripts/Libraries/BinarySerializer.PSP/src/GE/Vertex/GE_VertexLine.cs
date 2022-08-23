namespace BinarySerializer.PSP
{
    /// <see href="http://hitmen.c02.at/files/yapspd/psp_doc/chap11.html#sec11.5.15">GE Vertex Type</see>
    /// <see cref="https://github.com/pspdev/pspsdk/blob/master/src/gu/pspgu.h#L617">sceGuDrawArray</see>
    public class GE_VertexLine : BinarySerializable
    {
        public GE_Command_VertexType Pre_VertexType { get; set; }
        public GE_Vertex[] Vertices { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Vertices = s.SerializeObjectArray<GE_Vertex>(Vertices, 
                Pre_VertexType.VerticesCount + 1,
                onPreSerialize: (v,i) => {
                    v.Pre_VertexType = Pre_VertexType;
                    v.Pre_Line = this;
                    if(i > 0) s.Align(4, Offset);
                },
                name: nameof(Vertices));
        }
    }
}