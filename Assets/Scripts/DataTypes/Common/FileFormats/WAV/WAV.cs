using System.Collections.Generic;
using System.Text;
using BinarySerializer;

namespace R1Engine
{
    /// <summary>
    /// WAV audio file data
    /// </summary>
    public class WAV : BinarySerializable
    {
        public string Magic { get; set; }

        public uint FileSize { get; set; }

        public string FileTypeHeader { get; set; }

        public WAVChunk[] Chunks { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize header
            Magic = s.SerializeString(Magic, 4, Encoding.ASCII, name: nameof(Magic));
            FileSize = s.Serialize<uint>(FileSize, name: nameof(FileSize));
            FileTypeHeader = s.SerializeString(FileTypeHeader, 4, Encoding.ASCII, name: nameof(FileTypeHeader));

            // Serialize chunks
            if (Chunks == null)
            {
                var tempChunks = new List<WAVChunk>();
                var index = 0;

                while (FileSize - 8 > s.CurrentPointer.FileOffset)
                {
                    tempChunks.Add(s.SerializeObject<WAVChunk>(default, name: $"{nameof(Chunks)}[{index}]"));
                    index++;
                }

                Chunks = tempChunks.ToArray();
            }
            else
            {
                s.SerializeObjectArray<WAVChunk>(Chunks, Chunks.Length, name: nameof(Chunks));

                // Update file size
                FileSize = (uint)(s.CurrentPointer.FileOffset - Offset.FileOffset - 8);
                s.DoAt(Offset + 4, () => FileSize = s.Serialize<uint>(FileSize, name: nameof(FileSize)));
            }
        }
    }

    public class WAVListChunk : WAVChunk
    {
        public string ListHeader { get; set; }
        public WAVChunk[] Chunks { get; set; }

        protected override void SerializeChunk(SerializerObject s)
        {
            ListHeader = s.SerializeString(ListHeader, 4, Encoding.ASCII, name: nameof(ListHeader));

            // Serialize chunks
            if (Chunks == null)
            {
                var tempChunks = new List<WAVChunk>();
                var index = 0;

                while (ChunkSize - 8 > s.CurrentPointer.FileOffset)
                {
                    tempChunks.Add(s.SerializeObject<WAVChunk>(default, name: $"{nameof(Chunks)}[{index}]"));
                    index++;
                }

                Chunks = tempChunks.ToArray();
            }
            else
            {
                s.SerializeObjectArray<WAVChunk>(Chunks, Chunks.Length, name: nameof(Chunks));
            }
        }
    }
}