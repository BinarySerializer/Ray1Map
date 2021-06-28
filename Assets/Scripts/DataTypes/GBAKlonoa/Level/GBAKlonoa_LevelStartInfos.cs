using BinarySerializer;

namespace R1Engine
{
    public class GBAKlonoa_LevelStartInfos : BinarySerializable
    {
        public GBAKlonoa_LevelStartInfo[] StartInfos { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            StartInfos = s.SerializeObjectArray<GBAKlonoa_LevelStartInfo>(StartInfos, 21, name: nameof(StartInfos));
        }
    }
}