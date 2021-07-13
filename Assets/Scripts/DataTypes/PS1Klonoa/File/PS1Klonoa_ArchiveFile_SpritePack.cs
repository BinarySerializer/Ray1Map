using BinarySerializer;
using BinarySerializer.PS1;

namespace R1Engine
{
    public class PS1Klonoa_ArchiveFile_SpritePack : PS1Klonoa_ArchiveFile
    {
        // The game does this:
        // - Modify all offsets to pointers
        // - Enumerate all files (always 71 of them) and copy the pointers to an array
        // - Enumerate every pointer in this new array, skipping the first 2 and ignoring any pointer which is the same as file 0 - then modifying these files sub-file offsets to pointers
        // - Add Unk1 file to this list of files (so Unk1 is fixed data) and increment the file count

        // First file is always a dummy file
        public PS1Klonoa_File_RawData DummyFile { get; set; }

        public PlayerSpritesArchive PlayerSprites { get; set; }

        // Each file seems to correspond to an object type. For example the first one is the Moo enemy.
        // If an object is unused in the level then the file is nulled out too (it points to the dummy entry).
        public PS1Klonoa_ArchiveFile_SpriteFrames[] SpriteFrames { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize base file
            base.SerializeImpl(s);

            DummyFile = SerializeFile<PS1Klonoa_File_RawData>(s, DummyFile, 0, name: nameof(DummyFile));
            PlayerSprites = SerializeFile<PlayerSpritesArchive>(s, PlayerSprites, 1, name: nameof(PlayerSprites));

            SpriteFrames ??= new PS1Klonoa_ArchiveFile_SpriteFrames[71 - 2];

            // Enumerate remaining file
            for (int i = 0; i < SpriteFrames.Length; i++)
            {
                // Ignore dummy files
                if (OffsetTable.FilePointers[2 + i] == OffsetTable.FilePointers[0])
                    continue;

                SpriteFrames[i] = SerializeFile<PS1Klonoa_ArchiveFile_SpriteFrames>(s, SpriteFrames[i], 2 + i, name: $"{nameof(SpriteFrames)}[{i}]");
            }
        }

        public class PlayerSpritesArchive : PS1Klonoa_ArchiveFile
        {
            public PlayerSpritesArchiveFile[] Files { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                base.SerializeImpl(s);

                Files ??= new PlayerSpritesArchiveFile[OffsetTable.FilesCount];

                // Enumerate remaining file
                for (int i = 0; i < Files.Length; i++)
                    Files[i] = SerializeFile<PlayerSpritesArchiveFile>(s, Files[i], i, name: $"{nameof(Files)}[{i}]");
            }

            public class PlayerSpritesArchiveFile : PS1Klonoa_BaseFile
            {
                public PS1_TIM TIM { get; set; }
                
                public uint Raw_Size { get; set; }
                public ushort Raw_Width { get; set; }
                public ushort Raw_Height { get; set; }
                public byte[] Raw_ImgData { get; set; }

                public override void SerializeImpl(SerializerObject s)
                {
                    // If ULZ compressed then it's a TIM file (the x and y coordinates are irrelevant though)
                    if (Pre_IsCompressed)
                    {
                        TIM = s.SerializeObject<PS1_TIM>(TIM, name: nameof(TIM));
                    }
                    // If not ULZ compressed it's raw data compressed using an unknown compression type
                    else
                    {
                        Raw_Size = s.Serialize<uint>(Raw_Size, name: nameof(Raw_Size));
                        Raw_Width = s.Serialize<ushort>(Raw_Width, name: nameof(Raw_Width));
                        Raw_Height = s.Serialize<ushort>(Raw_Height, name: nameof(Raw_Height));
                        s.DoEncoded(new PS1Klonoa_UnknownEncoder(Raw_Size), () =>
                        {
                            Raw_ImgData = s.SerializeArray<byte>(Raw_ImgData, Raw_Size, name: nameof(Raw_ImgData));
                        });
                    }
                }
            }
        }
    }
}