namespace BinarySerializer.Ubisoft.Onyx.NDS
{
    public class FileReference : BinarySerializable
    {
        public uint FilePathLength { get; set; }
        public string FilePath { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            FilePathLength = s.Serialize<uint>(FilePathLength, name: nameof(FilePathLength));
            FilePath = s.SerializeString(FilePath, length: FilePathLength, name: nameof(FilePath));
        }

        public override bool UseShortLog => true;
        public override string ShortLog => $"FileRef: {FilePath}";
    }

    public class FileReference<T> : FileReference
        where T : OnyxFile, new()
    {
        public T Value { get; set; }

        public void Resolve(SerializerObject s)
        {
            OnyxFileResolver resolver = s.Context.GetRequiredStoredObject<OnyxFileResolver>(OnyxFileResolver.Key);

            OnyxFileResolver.FilePointer filePointer = resolver.NDS_GetFilePointer(FilePath);

            if (filePointer == null)
            {
                s.SystemLog?.LogWarning("File {0} was not found", FilePath);
                return;
            }

            s.DoAt(filePointer.Pointer, () =>
            {
                Value = s.SerializeObject<T>(Value, onPreSerialize: x => x.Pre_FileSize = filePointer.Length, name: nameof(Value));
            });
        }
    }
}