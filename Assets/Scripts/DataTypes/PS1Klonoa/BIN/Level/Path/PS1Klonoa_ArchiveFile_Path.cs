using BinarySerializer;

namespace R1Engine
{
    public class PS1Klonoa_ArchiveFile_Path : PS1Klonoa_BaseFile
    {
        public PS1Klonoa_ArchiveFile_PathBlock[] Blocks { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Blocks = s.SerializeObjectArray<PS1Klonoa_ArchiveFile_PathBlock>(Blocks, Pre_FileSize / 28, name: nameof(Blocks));
        }
    }
}