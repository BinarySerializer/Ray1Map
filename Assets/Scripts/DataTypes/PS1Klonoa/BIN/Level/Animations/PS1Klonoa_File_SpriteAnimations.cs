using BinarySerializer;

namespace R1Engine
{
    public class PS1Klonoa_File_SpriteAnimations : PS1Klonoa_BaseFile
    {
        public uint AnimationsCount { get; set; }
        public PS1Klonoa_SpriteAnimation[] Animations { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            AnimationsCount = s.Serialize<uint>(AnimationsCount, name: nameof(AnimationsCount));
            Animations = s.SerializeObjectArray<PS1Klonoa_SpriteAnimation>(Animations, AnimationsCount, x => x.Pre_OffsetAnchor = Offset, name: nameof(Animations));

            // Go to the end of the file
            s.Goto(Offset + Pre_FileSize);
        }
    }
}