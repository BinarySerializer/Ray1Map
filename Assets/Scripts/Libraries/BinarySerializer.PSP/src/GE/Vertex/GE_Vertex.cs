using System;

namespace BinarySerializer.PSP
{
    /// <see href="http://hitmen.c02.at/files/yapspd/psp_doc/chap11.html#sec11.5.15">GE Vertex Type</see>
    /// <see href="https://github.com/pspdev/pspsdk/blob/master/src/gu/pspgu.h#L617">sceGuDrawArray</see>
    /// <see href="https://github.com/pspdev/pspsdk/blob/d019dbc7ecc198102229d0cdfe02976b6bef4e4d/src/samples/gu/vertex/vertex.c#L37">Vertex alignment calculation</see>
    /// Vertex order: [for vertices(1-8)] [weights (0-8)] [texture uv] [color] [normal] [vertex] [/for]
    public class GE_Vertex : BinarySerializable
    {
        public GE_VertexLine Pre_Line { get; set; }
        public GE_Command_VertexType Pre_VertexType { get; set; }

        public GE_VertexNumber[] Weights { get; set; }
        public GE_VertexNumber[] UV { get; set; }
        public GE_VertexColor Color { get; set; }
        public GE_VertexNumber[] Normal { get; set; }
        public GE_VertexNumber[] Position { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            // Calculate alignment
            int alignBytes = 1;
            if(Pre_VertexType.WeightFormat == GE_VertexNumberFormat.FloatingPoint_32Bit ||
                Pre_VertexType.TextureFormat == GE_VertexNumberFormat.FloatingPoint_32Bit ||
                Pre_VertexType.ColorFormat == GE_ColorFormat.ABGR8888 ||
                Pre_VertexType.NormalFormat == GE_VertexNumberFormat.FloatingPoint_32Bit ||
                Pre_VertexType.PositionFormat == GE_VertexNumberFormat.FloatingPoint_32Bit)
                alignBytes = 4;
            else if (Pre_VertexType.WeightFormat == GE_VertexNumberFormat.FixedPoint_16Bit ||
				Pre_VertexType.TextureFormat == GE_VertexNumberFormat.FixedPoint_16Bit ||
				Pre_VertexType.ColorFormat == GE_ColorFormat.ABGR4444 ||
				Pre_VertexType.ColorFormat == GE_ColorFormat.ABGR5551 ||
				Pre_VertexType.ColorFormat == GE_ColorFormat.BGR565 ||
				Pre_VertexType.NormalFormat == GE_VertexNumberFormat.FixedPoint_16Bit ||
				Pre_VertexType.PositionFormat == GE_VertexNumberFormat.FixedPoint_16Bit)
                alignBytes = 2;

            //if(alignBytes > 1) s.Align(alignBytes, Pre_Line.Offset, logIfNotNull: true);

			if (Pre_VertexType.WeightFormat != GE_VertexNumberFormat.None)
                Weights = s.SerializeObjectArray<GE_VertexNumber>(Weights, Pre_VertexType.WeightsCount + 1, onPreSerialize: v => {
                    v.Pre_AlignOffset = Pre_Line.Offset;
                    v.Pre_Format = Pre_VertexType.WeightFormat;
                    v.Pre_Type = GE_VertexNumberType.Weight;
                }, name: nameof(Weights));

            if (Pre_VertexType.TextureFormat != GE_VertexNumberFormat.None)
                UV = s.SerializeObjectArray<GE_VertexNumber>(UV, 2, onPreSerialize: v => {
                    v.Pre_AlignOffset = Pre_Line.Offset;
                    v.Pre_Format = Pre_VertexType.TextureFormat;
                    v.Pre_Type = GE_VertexNumberType.Texture;
                }, name: nameof(UV));

            if (Pre_VertexType.ColorFormat != GE_ColorFormat.None)
                Color = s.SerializeObject<GE_VertexColor>(Color, onPreSerialize: v => {
                    v.Pre_AlignOffset = Pre_Line.Offset;
                    v.Pre_Format = Pre_VertexType.ColorFormat;
                }, name: nameof(Color));

            if (Pre_VertexType.NormalFormat != GE_VertexNumberFormat.None)
                Normal = s.SerializeObjectArray<GE_VertexNumber>(Normal, 3, onPreSerialize: v => {
                    v.Pre_AlignOffset = Pre_Line.Offset;
                    v.Pre_Format = Pre_VertexType.NormalFormat;
                    v.Pre_Type = GE_VertexNumberType.Normal;
                }, name: nameof(Normal));

            if (Pre_VertexType.PositionFormat != GE_VertexNumberFormat.None)
                Position = s.SerializeObjectArray<GE_VertexNumber>(Position, 3, onPreSerialize: v => {
                    v.Pre_AlignOffset = Pre_Line.Offset;
                    v.Pre_Format = Pre_VertexType.PositionFormat;
                    v.Pre_Type = GE_VertexNumberType.Position;
                }, name: nameof(Position));

			if (alignBytes > 1) s.Align(alignBytes, Pre_Line.Offset, logIfNotNull: true);
		}
    }
}