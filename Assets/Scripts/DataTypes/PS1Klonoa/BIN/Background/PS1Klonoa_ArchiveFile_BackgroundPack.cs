using BinarySerializer.PS1;

namespace BinarySerializer.KlonoaDTP
{
    /// <summary>
    /// The backgrounds in a level
    /// </summary>
    public class PS1Klonoa_ArchiveFile_BackgroundPack : PS1Klonoa_ArchiveFile
    {
        public PS1Klonoa_ArchiveFile<PS1_TIM> TIMFiles { get; set; } // Tilesets
        public PS1Klonoa_ArchiveFile<PS1_CEL> CELFiles { get; set; } // Tiles
        public PS1Klonoa_ArchiveFile<PS1_BGD> BGDFiles { get; set; } // Maps
        public PS1Klonoa_ArchiveFile<PS1Klonoa_File_TileAnimations> TileAnimationFiles { get; set; }

        protected override void SerializeFiles(SerializerObject s)
        {
            TIMFiles = SerializeFile<PS1Klonoa_ArchiveFile<PS1_TIM>>(s, TIMFiles, 0, name: nameof(TIMFiles));
            CELFiles = SerializeFile<PS1Klonoa_ArchiveFile<PS1_CEL>>(s, CELFiles, 1, name: nameof(CELFiles));
            BGDFiles = SerializeFile<PS1Klonoa_ArchiveFile<PS1_BGD>>(s, BGDFiles, 2, name: nameof(BGDFiles));
            TileAnimationFiles = SerializeFile<PS1Klonoa_ArchiveFile<PS1Klonoa_File_TileAnimations>>(s, TileAnimationFiles, 3, name: nameof(TileAnimationFiles));
        }
    }
}