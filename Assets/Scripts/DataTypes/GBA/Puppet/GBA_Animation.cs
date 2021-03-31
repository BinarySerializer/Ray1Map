using System.Linq;
using BinarySerializer;

namespace R1Engine
{
    public class GBA_Animation : GBA_BaseBlock
    {
        public byte Flags { get; set; }
        public byte Byte_01 { get; set; }
        public ushort AffineMatricesIndex { get; set; }
        public byte Byte_03 { get; set; }
        public byte[] LayersPerFrame { get; set; }

        public ushort Milan_Ushort_02 { get; set; }
        public int Milan_Int_06 { get; set; }
        public ushort[] Milan_LayerOffsets { get; set; }

        // Parsed
        public byte FrameCount { get; set; }

        public GBA_AnimationChannel[][] Layers { get; set; }

        public override void SerializeBlock(SerializerObject s) 
        {
            Flags = s.Serialize<byte>(Flags, name: nameof(Flags));

            if (!s.GetR1Settings().GBA_IsMilan)
            {
                Byte_01 = s.Serialize<byte>(Byte_01, name: nameof(Byte_01));
                AffineMatricesIndex = s.Serialize<byte>((byte)AffineMatricesIndex, name: nameof(AffineMatricesIndex));
                Byte_03 = s.Serialize<byte>(Byte_03, name: nameof(Byte_03));
                FrameCount = (byte)BitHelpers.ExtractBits(Byte_03, 6, 0);

                LayersPerFrame = s.SerializeArray<byte>(LayersPerFrame, FrameCount, name: nameof(LayersPerFrame));

                s.Align();

                if (Layers == null)
                    Layers = new GBA_AnimationChannel[FrameCount][];

                for (int i = 0; i < FrameCount; i++)
                    Layers[i] = s.SerializeObjectArray<GBA_AnimationChannel>(Layers[i], LayersPerFrame[i], name: $"{nameof(Layers)}[{i}]");
            }
            else
            {
                FrameCount = s.Serialize<byte>(FrameCount, name: nameof(FrameCount));
                Milan_Ushort_02 = s.Serialize<ushort>(Milan_Ushort_02, name: nameof(Milan_Ushort_02));
                AffineMatricesIndex = s.Serialize<ushort>(AffineMatricesIndex, name: nameof(AffineMatricesIndex));
                Milan_Int_06 = s.Serialize<int>(Milan_Int_06, name: nameof(Milan_Int_06));

                var offsetBase = s.CurrentPointer;

                Milan_LayerOffsets = s.SerializeArray<ushort>(Milan_LayerOffsets, FrameCount + 1, name: nameof(Milan_LayerOffsets)); // +1 since last offset is the end

                if (Layers == null)
                    Layers = new GBA_AnimationChannel[FrameCount][];

                for (int i = 0; i < FrameCount; i++)
                    Layers[i] = s.DoAt(offsetBase + Milan_LayerOffsets[i], () => s.SerializeObjectArray<GBA_AnimationChannel>(Layers[i], (Milan_LayerOffsets[i + 1] - Milan_LayerOffsets[i]) / 6, name: $"{nameof(Layers)}[{i}]"));

                s.Goto(offsetBase + Milan_LayerOffsets.LastOrDefault());
            }
        }
    }
}