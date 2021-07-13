using BinarySerializer;

namespace R1Engine
{
    public class PS1Klonoa_ArchiveFile_SpriteFrames : PS1Klonoa_ArchiveFile<PS1Klonoa_ArchiveFile_SpriteFrames.SpriteFrame>
    {
        public class SpriteFrame : PS1Klonoa_BaseFile
        {
            public PS1Klonoa_Sprite[] Sprites { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                Sprites = s.SerializeObjectArray<PS1Klonoa_Sprite>(Sprites, Pre_FileSize / 12, name: nameof(Sprites));
            }
        }
    }
}