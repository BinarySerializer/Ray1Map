namespace BinarySerializer.KlonoaDTP
{
    public class PS1Klonoa_File_Sprite : PS1Klonoa_BaseFile
    {
        public PS1Klonoa_SpriteTexture[] Sprites { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Sprites = s.SerializeObjectArray<PS1Klonoa_SpriteTexture>(Sprites, Pre_FileSize / 12, name: nameof(Sprites));
        }
    }
}