namespace R1Engine
{
    public class GBA_AffineMatrixList : GBA_BaseBlock
    {
        public GBA_AffineMatrix[] Matrices { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Matrices = s.SerializeObjectArray<GBA_AffineMatrix>(Matrices, BlockSize / 8, name: nameof(Matrices));
        }
    }
}