using BinarySerializer;

namespace R1Engine
{
    public class PS1Klonoa_ArchiveFile_LevelSpritePack : PS1Klonoa_ArchiveFile
    {
        // The game does this:
        // - Modify all offsets to pointers
        // - Enumerate all files (always 71 of them) and copy the pointers to an array
        // - Enumerate every pointer in this new array, skipping the first 2 and ignoring any pointer which is the same as file 0 - then modifying these files sub-file offsets to pointers
        // - Add Unk1 file to this list of files (so Unk1 is fixed data) and increment the file count

        // First file is always a dummy file
        public PS1Klonoa_File_RawData DummyFile { get; set; }

        public PS1Klonoa_ArchiveFile_PlayerSprites PlayerSprites { get; set; }

        // Each file seems to correspond to an object type. For example the first one is the Moo enemy.
        // If an object is unused in the level then the file is nulled out too (it points to the dummy entry).
        public PS1Klonoa_ArchiveFile_Sprites[] Sprites { get; set; }

        protected override void SerializeFiles(SerializerObject s)
        {
            DummyFile = SerializeFile<PS1Klonoa_File_RawData>(s, DummyFile, 0, name: nameof(DummyFile));
            PlayerSprites = SerializeFile<PS1Klonoa_ArchiveFile_PlayerSprites>(s, PlayerSprites, 1, name: nameof(PlayerSprites));

            Sprites ??= new PS1Klonoa_ArchiveFile_Sprites[71 - 2];

            // Enumerate remaining file
            for (int i = 0; i < Sprites.Length; i++)
            {
                // Ignore dummy files
                if (OffsetTable.FilePointers[2 + i] == OffsetTable.FilePointers[0])
                    continue;

                Sprites[i] = SerializeFile<PS1Klonoa_ArchiveFile_Sprites>(s, Sprites[i], 2 + i, name: $"{nameof(Sprites)}[{i}]");
            }
        }
    }
}