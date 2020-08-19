using R1Engine.Serialize;
using System;

namespace R1Engine
{
    public class GBA_AffineMatrixList : GBA_BaseBlock
    {
        public int FrameCount { get; set; }
        public ushort[] MatrixOffsets { get; set; }
        public GBA_AffineMatrix[][] Matrices { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if (s.GameSettings.EngineVersion == EngineVersion.GBA_PrinceOfPersia || s.GameSettings.EngineVersion == EngineVersion.GBA_StarWarsTrilogy) {
                MatrixOffsets = s.SerializeArray<ushort>(MatrixOffsets, FrameCount, name: nameof(MatrixOffsets));
                s.Align(4);
                if (Matrices == null) Matrices = new GBA_AffineMatrix[FrameCount][];

                for (int i = 0; i < Matrices.Length; i++) {
                    ushort offset = MatrixOffsets[i];
                    uint count = (uint)Math.Min((BlockSize - offset) / 8, 32);
                    s.DoAt(Offset + offset, () => {
                        Matrices[i] = s.SerializeObjectArray<GBA_AffineMatrix>(Matrices[i], count, name: $"{nameof(Matrices)}[{i}]");
                    });
                }
                s.Goto(Offset + BlockSize);
            } else {
                if (Matrices == null) Matrices = new GBA_AffineMatrix[1][];
                Matrices[0] = s.SerializeObjectArray<GBA_AffineMatrix>(Matrices[0], BlockSize / 8, name: $"{nameof(Matrices)}[{0}]");
            }
        }

        public GBA_AffineMatrix GetMatrix(int index, int frame) {
            GBA_AffineMatrix[] mats = Matrices[frame % Matrices.Length];
            if (mats != null && mats.Length > index) return mats[index];
            return null;
        }
    }
}