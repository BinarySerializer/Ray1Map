using BinarySerializer;

namespace R1Engine
{
    public class FLIC_FrameSubChunk : FLIC_BaseChunk
    {
        public FLIC Flic { get; set; } // Set before serializing

        // Chunk data
        public FLIC_Color256 Color256 { get; set; }
        public FLIC_ByteRun ByteRun { get; set; }
        public FLIC_DeltaFLC DeltaFLC { get; set; }
        public FLIC_LiteralFLC LiteralFLC { get; set; }

        public override void SerializeChunkData(SerializerObject s)
        {
            if (ChunkType == 0x04)
                Color256 = s.SerializeObject<FLIC_Color256>(Color256, name: nameof(Color256));
            else if (ChunkType == 0x07)
                DeltaFLC = s.SerializeObject<FLIC_DeltaFLC>(DeltaFLC, name: nameof(DeltaFLC));
            else if (ChunkType == 0x0F)
                ByteRun = s.SerializeObject<FLIC_ByteRun>(ByteRun, x => x.Flic = Flic, name: nameof(ByteRun));
            else if (ChunkType == 0x10)
                LiteralFLC = s.SerializeObject<FLIC_LiteralFLC>(LiteralFLC, x => x.Flic = Flic, name: nameof(LiteralFLC));
            else
                SerializeUnknownData(s);
        }
    }
}