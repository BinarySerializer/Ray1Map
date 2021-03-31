using BinarySerializer;

namespace R1Engine
{
    public class FLIC_Frame : BinarySerializable
    {
        public FLIC Flic { get; set; } // Set before serializing

        public ushort SubChunksCount { get; set; }
        public byte[] Reserved { get; set; }

        public FLIC_FrameSubChunk[] SubChunks { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            SubChunksCount = s.Serialize<ushort>(SubChunksCount, name: nameof(SubChunksCount));
            Reserved = s.SerializeArray<byte>(Reserved, 8, name: nameof(Reserved));
            SubChunks = s.SerializeObjectArray<FLIC_FrameSubChunk>(SubChunks, SubChunksCount, x => x.Flic = Flic, name: nameof(SubChunks));
        }
    }
}