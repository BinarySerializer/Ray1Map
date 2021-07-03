using BinarySerializer;

namespace R1Engine
{
    public class GBAKlonoa_LevelStartInfos : BinarySerializable
    {
        public GBAKlonoa_LevelStartInfo StartInfo_Entry { get; set; }
        public GBAKlonoa_LevelStartInfo[] StartInfos_Yellow { get; set; }
        public GBAKlonoa_LevelStartInfo[] StartInfos_Green { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            StartInfo_Entry = s.SerializeObject<GBAKlonoa_LevelStartInfo>(StartInfo_Entry, name: nameof(StartInfo_Entry));
            StartInfos_Yellow = s.SerializeObjectArray<GBAKlonoa_LevelStartInfo>(StartInfos_Yellow, 10, name: nameof(StartInfos_Yellow));
            StartInfos_Green = s.SerializeObjectArray<GBAKlonoa_LevelStartInfo>(StartInfos_Green, 10, name: nameof(StartInfos_Green));
        }
    }
}