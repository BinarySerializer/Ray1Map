using BinarySerializer;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// A sector of a level
    /// </summary>
    public class PS1Klonoa_ArchiveFile_Sector : PS1Klonoa_ArchiveFile
    {
        // TODO: Parse these
        public PS1Klonoa_File_RawData LevelModel { get; set; } // TMD file
        public PS1Klonoa_File_RawData File_1 { get; set; }
        public PS1Klonoa_File_RawData File_2 { get; set; }
        public PS1Klonoa_File_RawData File_3 { get; set; } // 28-byte entries
        public PS1Klonoa_ArchiveFile<PS1Klonoa_ArchiveFile_Path> Paths { get; set; }
        public PS1Klonoa_File_RawData File_5 { get; set; } // Array of int32?

        public override void SerializeImpl(SerializerObject s)
        {
            // Every third level (every boss) is not compressed
            var isCompressed = s.GetR1Settings().Level % 3 != 2;

            // TODO: Decompress sector archive
            if (isCompressed)
            {
                Debug.LogWarning($"TODO: Decompress sector archive");
                return;
            }

            base.SerializeImpl(s);
        }

        protected override void SerializeFiles(SerializerObject s)
        {
            LevelModel = SerializeFile<PS1Klonoa_File_RawData>(s, LevelModel, 0, name: nameof(LevelModel));
            File_1 = SerializeFile<PS1Klonoa_File_RawData>(s, File_1, 1, name: nameof(File_1));
            File_2 = SerializeFile<PS1Klonoa_File_RawData>(s, File_2, 2, name: nameof(File_2));
            File_3 = SerializeFile<PS1Klonoa_File_RawData>(s, File_3, 3, name: nameof(File_3));
            Paths = SerializeFile<PS1Klonoa_ArchiveFile<PS1Klonoa_ArchiveFile_Path>>(s, Paths, 4, name: nameof(Paths));
            File_5 = SerializeFile<PS1Klonoa_File_RawData>(s, File_5, 5, name: nameof(File_5));
        }
    }
}