using System.Text;
using BinarySerializer;


namespace R1Engine
{
    /// <summary>
    /// Chunk data for a WAV file
    /// </summary>
    public class WAVChunk : BinarySerializable
    {
        public string ChunkHeader { get; set; }
        public uint ChunkSize { get; set; }
        public byte[] Data { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize the chunk header
            ChunkHeader = s.SerializeString(ChunkHeader, 4, Encoding.ASCII, name: nameof(ChunkHeader));
            ChunkSize = s.Serialize<uint>(ChunkSize, name: nameof(ChunkSize));

            SerializeChunk(s);

            // Update chunk size
            ChunkSize = (uint)(s.CurrentFileOffset - Offset.FileOffset - 8);
            s.DoAt(Offset + 4, () => ChunkSize = s.Serialize<uint>(ChunkSize, name: nameof(ChunkSize)));
        }

        protected virtual void SerializeChunk(SerializerObject s) => Data = s.SerializeArray<byte>(Data, ChunkSize, name: nameof(Data));

        public T SerializeTo<T>(Context context)
            where T : WAVChunk, new()
        {
            var s = context.Deserializer;

            return s.DoAt(Offset, () => s.SerializeObject<T>(default, name: ChunkHeader));
        }
    }
}