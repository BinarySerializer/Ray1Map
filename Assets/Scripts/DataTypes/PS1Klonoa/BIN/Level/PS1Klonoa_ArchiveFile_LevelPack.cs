using BinarySerializer;

namespace R1Engine
{
    public class PS1Klonoa_ArchiveFile_LevelPack : PS1Klonoa_ArchiveFile
    {
        // TODO: Parse this - most files are TMD, but not all
        public PS1Klonoa_ArchiveFile_RawData ObjectModels { get; set; }

        // TODO: Parse this - Seems to be a bunch of stuff like sprites etc.
        public PS1Klonoa_ArchiveFile<PS1Klonoa_ArchiveFile<PS1Klonoa_ArchiveFile<PS1Klonoa_File_RawData>>> File_1 { get; set; }
        
        public PS1Klonoa_ArchiveFile_AnimationPack AnimationPack { get; set; }

        // TODO: Parse these - not available in all levels
        public PS1Klonoa_File_RawData File_3 { get; set; }
        public PS1Klonoa_File_RawData File_4 { get; set; }
        public PS1Klonoa_File_RawData File_5 { get; set; } // Archive?
        public PS1Klonoa_File_RawData File_6 { get; set; }
        public PS1Klonoa_File_RawData File_7 { get; set; }

        // A level is made out of multiple sectors (changes when Klonoa walks through a door etc.)
        public PS1Klonoa_ArchiveFile_Sector[] Sectors { get; set; }

        protected override void SerializeFiles(SerializerObject s)
        {
            ObjectModels = SerializeFile<PS1Klonoa_ArchiveFile_RawData>(s, ObjectModels, 0, name: nameof(ObjectModels));
            File_1 = SerializeFile<PS1Klonoa_ArchiveFile<PS1Klonoa_ArchiveFile<PS1Klonoa_ArchiveFile<PS1Klonoa_File_RawData>>>>(s, File_1, 1, name: nameof(File_1));
            AnimationPack = SerializeFile<PS1Klonoa_ArchiveFile_AnimationPack>(s, AnimationPack, 2, name: nameof(AnimationPack));

            File_3 = SerializeFile<PS1Klonoa_File_RawData>(s, File_3, 3, name: nameof(File_3));
            File_4 = SerializeFile<PS1Klonoa_File_RawData>(s, File_4, 4, name: nameof(File_4));
            File_5 = SerializeFile<PS1Klonoa_File_RawData>(s, File_5, 5, name: nameof(File_5));
            File_6 = SerializeFile<PS1Klonoa_File_RawData>(s, File_6, 6, name: nameof(File_6));
            File_7 = SerializeFile<PS1Klonoa_File_RawData>(s, File_7, 7, name: nameof(File_7));

            // The last file is always a dummy file to show the game that it's the last sector
            var sectorsCount = OffsetTable.FilesCount - 9;

            Sectors ??= new PS1Klonoa_ArchiveFile_Sector[sectorsCount];

            for (int i = 0; i < sectorsCount; i++)
                Sectors[i] = SerializeFile<PS1Klonoa_ArchiveFile_Sector>(s, Sectors[i], 8 + i, name: $"{Sectors}[{i}]");
        }
    }
}