using BinarySerializer;

namespace Ray1Map.GBA
{
    public class GBA_BatmanVengeance_Animation : BinarySerializable {
        #region Data
        // Set in onPreSerialize
        public GBA_BatmanVengeance_Puppet Puppet { get; set; }

        public uint FrameCount { get; set; }

        public ushort Milan_Ushort_02 { get; set; }
        public byte[] Milan_Bytes_04 { get; set; }

        public uint[] FrameOffsets { get; set; }

        #endregion

        #region Parsed

        public GBA_BatmanVengeance_AnimationFrame[] Frames { get; set; }

        #endregion

        #region Public Methods

        public override void SerializeImpl(SerializerObject s)
        {
            if (s.GetR1Settings().GBA_IsMilan)
            {
                FrameCount = s.Serialize<ushort>((ushort)FrameCount, name: nameof(FrameCount));
                Milan_Ushort_02 = s.Serialize<ushort>(Milan_Ushort_02, name: nameof(Milan_Ushort_02));
                Milan_Bytes_04 = s.SerializeArray<byte>(Milan_Bytes_04, 8, name: nameof(Milan_Bytes_04));
            }
            else
            {
                FrameCount = s.Serialize<uint>(FrameCount, name: nameof(FrameCount));
            }

            var baseOffset = s.CurrentPointer;

            FrameOffsets = s.SerializeArray<uint>(FrameOffsets, FrameCount, name: nameof(FrameOffsets));

            if (Frames == null) 
                Frames = new GBA_BatmanVengeance_AnimationFrame[FrameCount];

            for (int i = 0; i < FrameOffsets.Length; i++)
                Frames[i] = s.DoAt(baseOffset + FrameOffsets[i], () => s.SerializeObject<GBA_BatmanVengeance_AnimationFrame>(Frames[i], onPreSerialize: f => f.Puppet = Puppet, name: $"{nameof(Frames)}[{i}]"));
        }

        #endregion
    }
}