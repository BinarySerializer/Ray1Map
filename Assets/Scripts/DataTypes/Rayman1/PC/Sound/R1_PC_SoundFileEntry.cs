using BinarySerializer;

namespace R1Engine
{
    /// <summary>
    /// Sound file entry data for PC
    /// </summary>
    public class R1_PC_SoundFileEntry : BinarySerializable
    {
        /// <summary>
        /// The sound file offset
        /// </summary>
        public uint FileOffset { get; set; }

        /// <summary>
        /// The sound file size
        /// </summary>
        public uint FileSize { get; set; }

        public uint Unk1 { get; set; }

        public uint Unk2 { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            FileOffset = s.Serialize<uint>(FileOffset, name: nameof(FileOffset));
            FileSize = s.Serialize<uint>(FileSize, name: nameof(FileSize));
            Unk1 = s.Serialize<uint>(Unk1, name: nameof(Unk1));
            Unk2 = s.Serialize<uint>(Unk2, name: nameof(Unk2));
        }
    }
}