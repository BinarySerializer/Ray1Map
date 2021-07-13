using BinarySerializer;

namespace R1Engine
{
    public class PS1Klonoa_OffsetTable : BinarySerializable
    {
        public int FilesCount { get; set; }
        public Pointer[] FilePointers { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            FilesCount = s.Serialize<int>(FilesCount, name: nameof(FilesCount));
            FilePointers = s.SerializePointerArray(FilePointers, FilesCount, anchor: Offset, name: nameof(FilePointers));
        }
    }
}