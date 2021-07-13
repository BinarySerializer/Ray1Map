using BinarySerializer;

namespace R1Engine
{
    public class PS1Klonoa_ArchiveFile_PlayerSprites : PS1Klonoa_ArchiveFile
    {
        public PS1Klonoa_File_PlayerSprite[] Files { get; set; }

        protected override void SerializeFiles(SerializerObject s)
        {
            Files ??= new PS1Klonoa_File_PlayerSprite[OffsetTable.FilesCount];

            // Enumerate remaining file
            for (int i = 0; i < Files.Length; i++)
                Files[i] = SerializeFile<PS1Klonoa_File_PlayerSprite>(s, Files[i], i, name: $"{nameof(Files)}[{i}]");
        }
    }
}