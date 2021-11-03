﻿
using System;
using BinarySerializer;

namespace Ray1Map.GBA
{
    public class GBA_AffineMatrixList : GBA_BaseBlock
    {
        public byte[] Milan_Header { get; set; } // First byte is the frame count

        public int FrameCount { get; set; }
        public ushort[] MatrixOffsets { get; set; }
        public GBA_AffineMatrix[][] Matrices { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            if (s.GetR1Settings().GBA_IsMilan)
                Milan_Header = s.SerializeArray<byte>(Milan_Header, 4, name: nameof(Milan_Header));

            if (s.GetR1Settings().EngineVersion >= EngineVersion.GBA_SplinterCell) {
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