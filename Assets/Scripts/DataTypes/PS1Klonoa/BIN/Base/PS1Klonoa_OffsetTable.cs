namespace BinarySerializer.KlonoaDTP
{
    /// <summary>
    /// An offset table used for archives
    /// </summary>
    public class PS1Klonoa_OffsetTable : BinarySerializable
    {
        public int FilesCount { get; set; }
        public Pointer[] FilePointers { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            FilesCount = s.Serialize<int>(FilesCount, name: nameof(FilesCount));

            if (FilesCount == -1)
                FilesCount = 0;

            FilePointers = s.SerializePointerArray(FilePointers, FilesCount, anchor: Offset, name: nameof(FilePointers));
        }
    }
}