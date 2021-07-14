namespace BinarySerializer.KlonoaDTP
{
    public class PS1Klonoa_ArchiveFile_AnimationPack : PS1Klonoa_ArchiveFile
    {
        public PS1Klonoa_File_SpriteAnimations SpriteAnimations { get; set; }

        protected override void SerializeFiles(SerializerObject s)
        {
            SpriteAnimations = SerializeFile<PS1Klonoa_File_SpriteAnimations>(s, SpriteAnimations, 0, name: nameof(SpriteAnimations));
            // 1: Sprites
            // 2: Compressed files (TIM?)
            // 3: 
            // ...

            // TODO: Parse the rest
        }
    }
}